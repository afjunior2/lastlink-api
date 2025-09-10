using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Domain.Entities;

/// <summary>
/// Aggregate root for advance payment requests
/// 
/// Encapsulates all business rules related to advance payment requests.
/// </summary>
public class AdvanceRequest
{
    public int Id { get; set; }
    public string CreatorId { get; private set; } = string.Empty;
    public decimal RequestedAmount { get; private set; }
    public DateTime RequestDate { get; private set; }
    public RequestStatus Status { get; private set; }

    // Calculated properties (business rules)
    public decimal FeeAmount => RequestedAmount * 0.05m; // Fixed 5% fee
    public decimal NetAmount => RequestedAmount - FeeAmount;

    // Private constructor for Entity Framework
    private AdvanceRequest() { }

    // Main factory method
    public AdvanceRequest(string creatorId, decimal requestedAmount, DateTime? requestDate = null)
    {
        ValidateCreatorId(creatorId);
        ValidateRequestedAmount(requestedAmount);

        CreatorId = creatorId;
        RequestedAmount = requestedAmount;
        RequestDate = requestDate ?? DateTime.UtcNow;
        Status = RequestStatus.Pending;
    }

    // Domain behaviors
    public void Approve()
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException($"Cannot approve a request with status {Status.ToString()}");

        Status = RequestStatus.Approved;
    }

    public void Reject()
    {
        if (Status != RequestStatus.Pending)
            throw new InvalidOperationException($"Cannot reject a request with status {Status.ToString()}");

        Status = RequestStatus.Rejected;
    }

    // Factory method for simulation
    public static RequestSimulation Simulate(decimal requestedAmount)
    {
        ValidateRequestedAmount(requestedAmount);

        var feeAmount = requestedAmount * 0.05m;
        var netAmount = requestedAmount - feeAmount;

        return new RequestSimulation(requestedAmount, feeAmount, netAmount, 5.0m);
    }

    // Private validations
    private static void ValidateCreatorId(string creatorId)
    {
        if (string.IsNullOrWhiteSpace(creatorId))
            throw new ArgumentException("Creator ID is required", nameof(creatorId));
    }

    private static void ValidateRequestedAmount(decimal requestedAmount)
    {
        if (requestedAmount <= 100)
            throw new ArgumentException("Requested amount must be greater than $100.00", nameof(requestedAmount));
    }
}