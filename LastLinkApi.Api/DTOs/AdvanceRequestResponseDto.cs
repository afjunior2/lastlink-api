using LastLinkApi.Domain.Entities;

namespace LastLinkApi.Api.DTOs;

/// <summary>
/// DTO for advance request response
/// </summary>
public class AdvanceRequestResponseDto
{
    /// <summary>
    /// Request unique identifier
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Creator identifier
    /// </summary>
    /// <example>creator123</example>
    public string CreatorId { get; set; } = string.Empty;

    /// <summary>
    /// Requested amount
    /// </summary>
    /// <example>1000.00</example>
    public decimal RequestedAmount { get; set; }

    /// <summary>
    /// Request creation date
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// Fee amount (5% of requested amount)
    /// </summary>
    /// <example>50.00</example>
    public decimal FeeAmount { get; set; }

    /// <summary>
    /// Net amount (requested amount - fee)
    /// </summary>
    /// <example>950.00</example>
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Request status
    /// </summary>
    /// <example>pending</example>
    public string Status { get; set; } = string.Empty;

    public static AdvanceRequestResponseDto FromEntity(AdvanceRequest request)
    {
        return new AdvanceRequestResponseDto
        {
            Id = request.Id,
            CreatorId = request.CreatorId,
            RequestedAmount = request.RequestedAmount,
            RequestDate = request.RequestDate,
            FeeAmount = request.FeeAmount,
            NetAmount = request.NetAmount,
            Status = request.Status.ToString()
        };
    }
}