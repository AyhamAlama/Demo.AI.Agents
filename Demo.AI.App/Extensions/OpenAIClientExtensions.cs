namespace Demo.AI_Agents.Extensions;

public static class OpenAIClientExtensions
{
    public static OpenAIClient SetClient(string key)
    => new(new ApiKeyCredential(key));

    public static OpenAIClient SetClient(string endpoint, string? key = null)
    => new(new ApiKeyCredential(key ?? "no-needed-key"), new()
    {
        Endpoint = new(endpoint)
    });
}