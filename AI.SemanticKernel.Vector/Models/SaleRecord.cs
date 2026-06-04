namespace AI.SemanticKernel.Models;

public record SaleRecord(
    string OrderId,
    Customer Customer,
    Product Product,
    int Quantity,
    decimal Discount,
    DateTime OrderDate,
    string Status
);