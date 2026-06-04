namespace AI.SemanticKernel.Models;

public record Product(
    string SKU,
    string Name,
    string Category,
    decimal UnitPrice,
    decimal CostPrice,
    int StockQuantity,
    string Supplier
);