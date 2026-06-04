namespace AI.Shared.Extensions;

public class OpenAIClientExtensions
{
    public static OpenAIClient SetClient(string key, PipelineTransport? transport = null)
    {
        var options = new OpenAIClientOptions();

        if (transport is not null)
            options.Transport = transport;

        return new(new ApiKeyCredential(key), options);
    }

    public static OpenAIClient SetClient(string endpoint, string key = null, PipelineTransport? transport = null)
    {
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(endpoint)
        };

        if (transport is not null)
            options.Transport = transport;

        return new(new ApiKeyCredential(key ?? "no-needed-key"), options);
    }
}