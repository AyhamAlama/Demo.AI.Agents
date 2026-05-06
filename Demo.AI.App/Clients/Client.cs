namespace Demo.AI_Agents.Clients;

public static class Client
{
    public static ChatClientAgent LocalProvider(string model, string? endpoint = null, ChatOptions? chatOptions = null)
    {
        endpoint ??= "http://localhost:12434/engines/v1";

        var client = Extensions.OpenAIClientExtensions.SetClient(endpoint);

        return chatOptions is null
            ? client.GetChatClient(model).AsAIAgent()
            : client.GetChatClient(model).AsAIAgent(new ChatClientAgentOptions { ChatOptions = chatOptions });
    }

    public static ChatClientAgent OnlineProvider(string model, string key,
        string? endpoint = null,
        bool justByKey = false,
        ChatOptions? chatOptions = null)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("Model name cannot be null.", nameof(model));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("API key cannot be null.", nameof(key));

        OpenAIClient openAIClient;

        if (justByKey)
            openAIClient = Extensions.OpenAIClientExtensions.SetClient(key);
        else
            openAIClient = Extensions.OpenAIClientExtensions.SetClient(endpoint ?? throw new Exception("you should pass a endpoint"), key);

        var chatClient = openAIClient.GetChatClient(model);

        return chatOptions is null
            ? chatClient.AsAIAgent()
            : chatClient.AsAIAgent(new ChatClientAgentOptions { ChatOptions = chatOptions });
    }
}