using AI.SemanticKernel;

var app = new SalesKnowledgeApp();

Console.Write("Import Data? (Y/N): ");

if (Console.ReadKey().Key == ConsoleKey.Y)
{
    Console.Clear();
    await app.ImportAsync();
}

Console.Clear();

await app.SearchLoopAsync();