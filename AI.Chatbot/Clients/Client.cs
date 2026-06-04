using AI.Shared.Handlers.MessageHandler;
using System.ClientModel.Primitives;

namespace Demo.AI_Agents.Clients;

public static class Client
{
    public static ChatClientAgent LocalProvider(string model, string? endpoint = null, ChatOptions? chatOptions = null)
    {
        endpoint ??= "http://localhost:12434/engines/v1";

        var client = AI.Shared.Extensions.OpenAIClientExtensions.SetClient(endpoint, null);

        return chatOptions is null
            ? client.GetChatClient(model).AsAIAgent()
            : client.GetChatClient(model).AsAIAgent(new ChatClientAgentOptions { ChatOptions = chatOptions });
    }

    public static ChatClientAgent OnlineChatClientProvider(string model, string key,
        string? endpoint = null,
        bool justByKey = false,
        ChatOptions? chatOptions = null)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model name cannot be null.", nameof(model));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("API key cannot be null.", nameof(key));

        OpenAIClient openAIClient;

        var httpClientPipelineTransport =
            new HttpClientPipelineTransport(new HttpClient(new CustomHttpClientHandler()));

        if (justByKey)
            openAIClient = AI.Shared.Extensions.OpenAIClientExtensions.SetClient(key,
                (string?)null!,
                httpClientPipelineTransport);
        else
            openAIClient = AI.Shared.Extensions.OpenAIClientExtensions.SetClient(endpoint ?? throw new Exception("you should pass a endpoint"),
                key,
                httpClientPipelineTransport);

        var chatClient = openAIClient.GetChatClient(model);

        return chatOptions is null
            ? chatClient.AsAIAgent()
            : chatClient.AsAIAgent(new ChatClientAgentOptions { ChatOptions = chatOptions });
    }

    public static IEmbeddingGenerator<string, Embedding<float>> OnlineEmbaddingClient(string model, string key,
        string? endpoint = null,
        bool justByKey = false)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model name cannot be null.", nameof(model));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("API key cannot be null.", nameof(key));

        OpenAIClient openAIClient;

        CustomHttpClientHandler customHandler = new();

        HttpClient httpClient = new(customHandler);

        if (justByKey)
            openAIClient = AI.Shared.Extensions.OpenAIClientExtensions.SetClient(key,
                (string?)null!,
                new HttpClientPipelineTransport(httpClient));
        else
            openAIClient = AI.Shared.Extensions.OpenAIClientExtensions.SetClient(endpoint ?? throw new Exception("you should pass a endpoint"),
                key,
                new HttpClientPipelineTransport(httpClient));

        var embeddingClient = openAIClient.GetEmbeddingClient(model);

        return embeddingClient.AsIEmbeddingGenerator();
    }
}