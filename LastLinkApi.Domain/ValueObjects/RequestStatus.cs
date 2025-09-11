namespace LastLinkApi.Domain.ValueObjects;

/// <summary>
/// Possible statuses for an advance payment request
/// </summary>
public enum RequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

/// <summary>
/// Extensions for RequestStatus enum
/// </summary>
public static class RequestStatusExtensions
{
    public static string ToString(this RequestStatus status)
    {
        return status switch
        {
            RequestStatus.Pending => "pending",
            RequestStatus.Approved => "approved",
            RequestStatus.Rejected => "rejected",
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }
}