namespace AI.SemanticKernel.Models;

public class SalesKnowledgeVectorRecord
{
    [VectorStoreKey]
    public required Guid Id { get; set; }

    [VectorStoreData]
    public required string Topic { get; set; }

    [VectorStoreData]
    public required string Summary { get; set; }

    [VectorStoreData]
    public required string RawJson { get; set; }

    [VectorStoreVector(1536)]
    public string Vector => $"{Topic}: {Summary}";
}