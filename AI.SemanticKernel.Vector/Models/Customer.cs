namespace AI.SemanticKernel.Models;

public record Customer(
    string CustomerCode,
    string Name,
    string Segment,
    string Region,
    string ContactEmail
);