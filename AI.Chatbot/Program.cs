// Juts change generic type to what you want to return as structured data.
// for example: in AI.Chatbot.Models
// we have a record called DotNetVersion if you ask what is the latest .NET version
// or what are the .NET versions, you can return List<DotNetVersion> instead of DotNetVersion.
await Runner.RunGitHubProviderAsync<string>();