using AI.SemanticKernel.Models;
using AI.Shared.Handlers.MessageHandler;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using System.ClientModel.Primitives;
using System.Text;
using System.Text.Json;

namespace AI.SemanticKernel;

public class SalesKnowledgeApp
{
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
    private readonly VectorStoreCollection<Guid, SalesKnowledgeVectorRecord> _collection;
    private readonly ChatClientAgent _agent;

    private readonly List<Product> _products =
    [
        new("LAPTOP-PRO-15",   "ProBook Laptop 15\"",  "Electronics", 1_299.99m, 820.00m, 120, "TechSupply GmbH"),
        new("MONITOR-27-4K",   "UltraView 27\" 4K",    "Electronics",   449.99m, 260.00m,  85, "DisplayCo Ltd"),
        new("CHAIR-ERG-PRO",   "ErgoSeat Pro Chair",   "Furniture",     349.99m, 175.00m,  60, "OfficeFit Inc"),
        new("HEADSET-NC-700",  "QuietZone NC Headset", "Accessories",   249.99m, 110.00m, 300, "AudioWorld"),
        new("SOFTW-SUITE-BIZ", "BizSuite 365 License", "Software",      299.99m,  30.00m, 999, "SoftCorp"),
    ];

    private readonly List<Customer> _customers =
    [
        new("ENT-001", "Horizon Corp",    "Enterprise", "North", "procurement@horizoncorp.com"),
        new("ENT-002", "Apex Solutions",  "Enterprise", "West",  "orders@apexsolutions.com"),
        new("SMB-001", "BlueSky Studio",  "SMB",        "South", "admin@blueskystudio.com"),
        new("RET-001", "Ahmed Al-Rashid", "Retail",     "North", "ahmed.rashid@email.com"),
    ];

    private readonly List<SaleRecord> _sales;

    public SalesKnowledgeApp()
    {
        _sales =
        [
            new("ORD-2024-0001", _customers[0], _products[0],  15, 0.10m, new DateTime(2024,  1, 15), "Completed"),
            new("ORD-2024-0002", _customers[0], _products[1],  15, 0.10m, new DateTime(2024,  2,  3), "Completed"),
            new("ORD-2024-0003", _customers[1], _products[4],  50, 0.20m, new DateTime(2024,  3,  7), "Completed"),
            new("ORD-2024-0004", _customers[2], _products[2],   4, 0.00m, new DateTime(2024,  4, 10), "Completed"),
            new("ORD-2024-0005", _customers[3], _products[3],   1, 0.00m, new DateTime(2024,  5, 22), "Completed"),
            new("ORD-2024-0006", _customers[0], _products[4],  20, 0.20m, new DateTime(2024,  6,  1), "Completed"),
            new("ORD-2024-0007", _customers[1], _products[0],  10, 0.10m, new DateTime(2024,  7, 14), "Refunded"),
            new("ORD-2024-0008", _customers[2], _products[3],   6, 0.05m, new DateTime(2024,  9, 30), "Completed"),
            new("ORD-2024-0009", _customers[3], _products[1],   1, 0.00m, new DateTime(2024, 11, 11), "Pending"),
            new("ORD-2024-0010", _customers[0], _products[2],   2, 0.00m, new DateTime(2024, 12, 20), "Completed"),
        ];

        var httpClientPipelineTransport =
            new HttpClientPipelineTransport(new HttpClient(new CustomHttpClientHandler()));

        var client = AI.Shared.Extensions.OpenAIClientExtensions.SetClient(
            AppConfiguration.GitHubEndpoint,
            AppConfiguration.GitHubToken);

        _embeddingGenerator = client
            .GetEmbeddingClient(AIModelName.EMBEDDING_3_SMALL)
            .AsIEmbeddingGenerator();

        string connectionString = @"Data Source=.\vector-store.db";

        VectorStore vectorStore = new Microsoft.SemanticKernel.Connectors.SqliteVec.SqliteVectorStore(
            connectionString,
            new SqliteVectorStoreOptions { EmbeddingGenerator = _embeddingGenerator });

        _collection = vectorStore.GetCollection<Guid, SalesKnowledgeVectorRecord>("Sales_knowledge_base");

        _agent = client
            .GetChatClient(AIModelName.GPT_4_1_NANO)
            .AsAIAgent(instructions: "You are a sales analyst expert for the company's data.");
    }

    public async Task ImportAsync()
    {
        await _collection.EnsureCollectionDeletedAsync();
        await _collection.EnsureCollectionExistsAsync();

        var entries = BuildKnowledgeEntries();
        int counter = 0;

        foreach (var (topic, summary, payload) in entries)
        {
            counter++;
            Console.Write($"\rEmbedding: {counter}/{entries.Count}  [{topic}]");

            var embeddings = await _embeddingGenerator.GenerateAsync([$"{topic}: {summary}"]);

            await _collection.UpsertAsync(new SalesKnowledgeVectorRecord
            {
                Id = Guid.NewGuid(),
                Topic = topic,
                Summary = summary,
                RawJson = JsonSerializer.Serialize(payload)
            });
        }

        Console.WriteLine($"\n✅ Imported {counter} records successfully.");
    }

    public async Task SearchLoopAsync()
    {
        AgentSession session = await _agent.CreateSessionAsync();

        while (true)
        {
            Console.Write("\n> ");
            string input = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(input)) continue;
            if (input.ToLower() == "exit") break;

            // Vector Search
            var context = new StringBuilder();
            await foreach (var result in _collection.SearchAsync(input, 3))
            {
                string entry = $"Topic: {result.Record.Topic} - Summary: {result.Record.Summary}";
                Utili.Yellow($"[Score: {result.Score:F3}] {entry}\n");
                context.AppendLine(entry);
            }

            List<Microsoft.Extensions.AI.ChatMessage> messages =
            [
                new(ChatRole.User, "Relevant knowledge base context:\n" + context),
                new(ChatRole.User, input)
            ];

            AgentResponse response = await _agent.RunAsync(messages, session);
            Utili.Yellow("── Answer ──────────────────────────────");
            Console.WriteLine(response);
        }
    }

    private List<(string Topic, string Summary, object Payload)> BuildKnowledgeEntries()
    {
        var entries = new List<(string, string, object)>();
        var completed = _sales.Where(s => s.Status == "Completed").ToList();

        foreach (var group in completed.GroupBy(s => s.Customer.CustomerCode))
        {
            var cust = group.First().Customer;
            decimal rev = group.Sum(CalcRevenue);
            decimal prof = group.Sum(CalcProfit);

            entries.Add((
                $"Customer Summary – {cust.Name}",
                $"{cust.Name} is a {cust.Segment} customer from the {cust.Region} region. " +
                $"Total completed orders: {group.Count()}. " +
                $"Total revenue: ${rev:N2} | Total profit: ${prof:N2}.",
                new
                {
                    cust.CustomerCode,
                    cust.Name,
                    cust.Segment,
                    cust.Region,
                    TotalOrders = group.Count(),
                    TotalRevenue = rev,
                    TotalProfit = prof
                }
            ));
        }

        foreach (var group in completed.GroupBy(s => s.Product.SKU))
        {
            var prod = group.First().Product;
            int units = group.Sum(s => s.Quantity);
            decimal rev = group.Sum(CalcRevenue);
            decimal prof = group.Sum(CalcProfit);

            entries.Add((
                $"Product Performance – {prod.Name}",
                $"{prod.Name} ({prod.Category}) sold {units} units. " +
                $"Revenue: ${rev:N2} | Profit: ${prof:N2} | " +
                $"Avg margin: {(rev > 0 ? prof / rev * 100 : 0):N1}%. Supplier: {prod.Supplier}.",
                new
                {
                    prod.SKU,
                    prod.Name,
                    prod.Category,
                    prod.Supplier,
                    UnitsSold = units,
                    Revenue = rev,
                    Profit = prof
                }
            ));
        }

        foreach (var group in completed.GroupBy(s => s.Product.Category))
        {
            decimal rev = group.Sum(CalcRevenue);
            decimal prof = group.Sum(CalcProfit);
            int qty = group.Sum(s => s.Quantity);

            entries.Add((
                $"Category Rollup – {group.Key}",
                $"Category '{group.Key}' generated ${rev:N2} in revenue and ${prof:N2} profit " +
                $"across {qty} units sold.",
                new { Category = group.Key, TotalRevenue = rev, TotalProfit = prof, TotalUnits = qty }
            ));
        }

        return entries;
    }

    private static decimal CalcRevenue(SaleRecord s) =>
        s.Quantity * s.Product.UnitPrice * (1 - s.Discount);

    private static decimal CalcProfit(SaleRecord s) =>
        s.Quantity * (s.Product.UnitPrice * (1 - s.Discount) - s.Product.CostPrice);
}