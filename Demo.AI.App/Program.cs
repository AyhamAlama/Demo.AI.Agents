var config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

await Runner.RunGitHubProviderAsync<string>(config);