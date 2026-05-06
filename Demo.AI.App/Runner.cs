namespace Demo.AI_Agents;

public static class Runner
{
    public static async Task RunGitHubProviderAsync<T>(IConfiguration config,
        MicrosoftAIMessage.ChatMessage? message = null,
        AITool? aITool = null)
    {
        var key = config["GITHUB_TOKEN"]!;

        var endpoint = config["GIT_HUB_END_POINT"]!;

        var instructions = new StringBuilder();

        instructions.AppendLine(string.Format(Instructions.YOU_ARE_EXPERT, Instructions.DOT_NET_ECOSYS));

        instructions.AppendLine(string.Format(Instructions.SPEAK_BACK_IN, "german"));

        List<AITool> tools = [
            AIFunctionFactory.Create(GetTimeZoneInfo),
            AIFunctionFactory.Create(GetDateTimeUtc),
        ];

        if (aITool is not null)
            tools.Add(aITool);

        var chatClientAgent = Client.OnlineProvider(AIModelName.GPT_4_1_NANO, key, endpoint, chatOptions: new()
        {
            Instructions = instructions.ToString(),
            Tools = tools,

        });

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

}