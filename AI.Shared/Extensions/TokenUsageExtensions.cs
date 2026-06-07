namespace AI.Shared.Extensions;

public static class TokenUsageExtensions
{
    public static void ToConsole(this UsageDetails? usageDetails)
    {
        if (usageDetails is null) return;

        Console.WriteLine();

        Utili.Green($" --- Prompt Tokens (InputTokenCount): {usageDetails.InputTokenCount}");

        Utili.Green($" --- Output Tokens: {usageDetails.OutputTokenCount} ({usageDetails.ReasoningTokenCount ?? 0} was used for reasoning)");

        Utili.Yellow($" = TotalTokenCount: {usageDetails.TotalTokenCount}");

        Console.WriteLine();
    }
}
