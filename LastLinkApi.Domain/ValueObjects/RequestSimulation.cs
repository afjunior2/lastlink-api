namespace LastLinkApi.Domain.ValueObjects;

/// <summary>
/// Value Object for advance payment request simulation
/// </summary>
public record RequestSimulation(
    decimal RequestedAmount,
    decimal FeeAmount,
    decimal NetAmount,
    decimal FeePercentage
);