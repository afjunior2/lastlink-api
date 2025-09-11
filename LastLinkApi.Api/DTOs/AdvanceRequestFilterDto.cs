using LastLinkApi.Domain.ValueObjects;

namespace LastLinkApi.Api.DTOs;

/// <summary>
/// DTO for filtering advance requests
/// </summary>
public class AdvanceRequestFilterDto
{
    /// <summary>
    /// Filter by creator ID
    /// </summary>
    /// <example>creator123</example>
    public string? CreatorId { get; set; }

    /// <summary>
    /// Filter by status
    /// </summary>
    /// <example>pending</example>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by minimum amount
    /// </summary>
    /// <example>100.00</example>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Filter by maximum amount
    /// </summary>
    /// <example>5000.00</example>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Filter by start date
    /// </summary>
    /// <example>2024-01-01</example>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Filter by end date
    /// </summary>
    /// <example>2024-12-31</example>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Sort field (requestDate, requestedAmount, creatorId)
    /// </summary>
    /// <example>requestDate</example>
    public string SortBy { get; set; } = "requestDate";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    /// <example>desc</example>
    public string SortDirection { get; set; } = "desc";

    /// <summary>
    /// Page number for pagination
    /// </summary>
    /// <example>1</example>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size for pagination
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Convert string status to enum
    /// </summary>
    public RequestStatus? GetStatusEnum()
    {
        if (string.IsNullOrWhiteSpace(Status))
            return null;

        return Status.ToLower() switch
        {
            "pending" => RequestStatus.Pending,
            "approved" => RequestStatus.Approved,
            "rejected" => RequestStatus.Rejected,
            _ => null
        };
    }
}

