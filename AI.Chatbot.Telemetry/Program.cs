using var tracerProvider =
    Sdk.CreateTracerProviderBuilder()
        .AddSource(ServiceConstants.ActivitySource)
        .SetResourceBuilder(
          ResourceBuilder.CreateDefault()
              .AddService(
                  serviceName: ServiceConstants.ServiceName,
                  serviceVersion: ServiceConstants.ServiceVersion))
        .AddConsoleExporter()
        .AddOtlpExporter(options => // To OpenTelemetry Collector
        {
            options.Protocol =
                OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;

            options.Endpoint =
                new Uri("http://localhost:5341/ingest/otlp/v1/traces"); // To Seq that supports OTLP ingestion
        })
        .Build();

var source = new ActivitySource(ServiceConstants.ActivitySource);

var client = AI.Shared.Extensions.OpenAIClientExtensions.SetClient(
            AppConfiguration.GitHubEndpoint,
            AppConfiguration.GitHubToken);

var agent = client
            .GetChatClient(AIModelName.GPT_4_1_NANO)
            .AsAIAgent(
            name: "ai_chat_bot_observed",
            instructions: "Friendly Assistant")
            .AsBuilder()
            .UseOpenTelemetry(ServiceConstants.ActivitySource, options =>
            {
                options.EnableSensitiveData = true; //log actual messages if you want, be careful with this in production environments!
            })
            .Build();

var session = await agent.CreateSessionAsync();

var response1 = await agent.RunAsync("what is .net sdk?", session);

Console.WriteLine(response1);

var response2 = await agent.RunAsync("give me the latest version in .net sdks", session);

Console.WriteLine(response2);