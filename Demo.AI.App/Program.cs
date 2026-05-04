using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;


var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var key = config["GITHUB_TOKEN"]!;

var llmModel = config["LLM_MODEL"]!;

var endpoint = config["GIT_HUB_END_POINT"]!;

var client = new OpenAIClient(new ApiKeyCredential(key), new OpenAIClientOptions
{
    Endpoint = new(endpoint)
});

var chatClientAgent = client.GetChatClient(llmModel)
    .AsAIAgent(instructions: "speak back in franch and just 25 charaters");

var agentResponse = await chatClientAgent.RunAsync("What is .Net sdk");

Console.WriteLine(agentResponse);