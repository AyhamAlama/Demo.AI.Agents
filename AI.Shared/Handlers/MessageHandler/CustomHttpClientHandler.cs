using AI.Shared.Utilities;
using System.Text.Json;

namespace AI.Shared.Handlers.MessageHandler;

public class CustomHttpClientHandler : HttpClientHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var requestString = await request.Content?.ReadAsStringAsync(cancellationToken)!;

        Utili.Yellow($"\n --- Request to {request.RequestUri}");
        Console.WriteLine(MakePrettyJson(requestString));
        Console.WriteLine(" --- end Raw request ---");
        Console.WriteLine();

        var response = await base.SendAsync(request, cancellationToken);


        var responseString = await response.Content?.ReadAsStringAsync(cancellationToken)!;


        Console.WriteLine("\n --- Raw Response ---");
        Console.WriteLine(MakePrettyJson(responseString));
        Console.WriteLine(" --- end Raw Response ---");

        return response;

    }

    private string MakePrettyJson(string json)
    {
        try
        {
            var parsedJson = JsonDocument.Parse(json);

            return JsonSerializer.Serialize(parsedJson,
              options: new() { WriteIndented = true });
        }
        catch
        {
            return json;
        }
    }
}