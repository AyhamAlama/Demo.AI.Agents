namespace Demo.AI_Agents;

public static class Runner
{
    public static async Task RunGitHubProviderAsync<T>(MicrosoftAIMessage.ChatMessage? message = null,
        AITool? aITool = null)
    {
        var instructions = new StringBuilder();

        instructions.AppendLine(string.Format(Instructions.YOU_ARE_EXPERT, Instructions.DOT_NET_ECOSYS));

        //instructions.AppendLine(string.Format(Instructions.SPEAK_BACK_IN, "german"));

        List<AITool> tools = [
            AIFunctionFactory.Create(GetTimeZoneInfo),
            AIFunctionFactory.Create(GetDateTimeUtc),
            //new HostedWebSearchTool()
            //AIFunctionFactory.Create(GetWeather)
        ];

        if (aITool is not null)
            tools.Add(aITool);

        var chatClientAgent = Client.OnlineChatClientProvider(AIModelName.GPT_4_1_NANO,
            AppConfiguration.GitHubToken,
            AppConfiguration.GitHubEndpoint,
            chatOptions: new()
            {
                Instructions = instructions.ToString(),
                //  Tools = tools,
            });

        message ??= new(ChatRole.User, "Hello!");

        Utili.DarkGray("GitHub Provider");

        var session = await chatClientAgent.CreateSessionAsync();

        while (true)
        {
            Console.Write("> ");

            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
                continue;

            var agentResponse = await chatClientAgent.RunAsync<T>(userInput, session);

            if (agentResponse.Result is null) continue;

            if (agentResponse.Result.GetType().IsGenericType &&
                agentResponse.Result.GetType().GetGenericTypeDefinition() == typeof(List<>))
            {
                dynamic list = agentResponse.Result;

                foreach (var item in list)
                    Utili.Blue($" {item}");
            }
            else
                Utili.Blue($" {agentResponse.Result}");
        }
    }


    public static async Task RunLocalProviderAsync<T>(MicrosoftAIMessage.ChatMessage? message = null,
        AITool? aITool = null)
    {
        var instructions = new StringBuilder();

        instructions.AppendLine(string.Format(Instructions.YOU_ARE_EXPERT, Instructions.DOT_NET_ECOSYS));

        instructions.AppendLine(string.Format(Instructions.SPEAK_BACK_IN, "german"));

        List<AITool> tools = [
            AIFunctionFactory.Create(GetTimeZoneInfo),
            AIFunctionFactory.Create(GetDateTimeUtc),
        ];

        if (aITool is not null)
            tools.Add(aITool);

        var chatClientAgent = Client.LocalProvider(AIModelName.SMOLLM);

        message ??= new(ChatRole.User, "Hello!");

        var agentResponse = await chatClientAgent.RunAsync<T>(message);

        if (agentResponse.Result!.GetType().IsGenericType &&
            agentResponse.Result.GetType().GetGenericTypeDefinition() == typeof(List<>))
        {
            dynamic list = agentResponse.Result;

            foreach (var item in list)
                Console.WriteLine(item);

        }
        else
            Console.WriteLine(agentResponse.Result);
    }

    private static TimeZoneInfo GetTimeZoneInfo() => TimeZoneInfo.Local;
    private static DateTime GetDateTimeUtc() => DateTime.UtcNow;
    private static string GetWeather() => "its good and sunny";
}