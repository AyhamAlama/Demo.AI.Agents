# Demo.AI.Agents

A collection of practical AI agent demos built with **C#**, **Semantic Kernel**, and **OpenAI-compatible models** via GitHub Models.

---

## Projects

### `AI.Chatbot`
A conversational agent with session-based memory. Demonstrates how to build a multi-turn chat experience using an LLM backend.

### `AI.SemanticKernel.Vector`
A semantic search system backed by **SQLite (SqliteVec)**. Embeds structured business data (products, customers, sales) into a vector store and answers natural language queries using RAG (Retrieval-Augmented Generation).

### `AI.Shared`
Shared utilities, model name constants, and OpenAI client configuration used across all projects.

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET 9 |
| AI Framework | Microsoft Semantic Kernel |
| Models | GPT-4.1 Nano, text-embedding-3-small |
| Model Provider | GitHub Models |
| Vector Store | SQLite via `SqliteVec` |

---

## Getting Started

### 1. Clone the repo
```bash
git clone https://github.com/AyhamAlama/Demo.AI.Agents.git
cd Demo.AI.Agents
```

### 2. Set your credentials
The projects read credentials from `AppConfiguration`. Set your GitHub Models endpoint and token:
Use user secrets or you can directly edit the `AppConfiguration.cs` file:
```csharp
// AI.Shared/AppConfiguration.cs
public static string GitHubEndpoint = "https://models.inference.ai.azure.com";
public static string GitHubToken    = "your_github_token_here";
```

> Get your token from [github.com/settings/tokens](https://github.com/settings/tokens)

### 3. Run a project
```bash
cd AI.SemanticKernel.Vector
dotnet run
```

---

## How the Vector Search Works

```
Sales Data (C#)
     │
     ▼
Knowledge Entries (Customer / Product / Category summaries)
     │
     ▼
text-embedding-3-small  →  SQLite (SqliteVec)
     │
     ▼
User Query  →  Vector Search  →  Top 3 Results  →  GPT-4.1 Nano  →  Answer
```

---

## License

MIT