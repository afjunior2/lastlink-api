namespace LastLinkApi.Api.DTOs;

/// <summary>
/// DTO for paged advance requests response
/// </summary>
public class PagedAdvanceRequestsResponseDto
{
    /// <summary>
    /// List of advance requests
    /// </summary>
    public IEnumerable<AdvanceRequestResponseDto> Requests { get; set; } = new List<AdvanceRequestResponseDto>();

    /// <summary>
    /// Current page number
    /// </summary>
    /// <example>1</example>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    /// <example>5</example>
    public int TotalPages { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    /// <example>10</example>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of records
    /// </summary>
    /// <example>47</example>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Indicates if there is a previous page
    /// </summary>
    /// <example>false</example>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Indicates if there is a next page
    /// </summary>
    /// <example>true</example>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Applied filters summary
    /// </summary>
    public FilterSummaryDto? FilterSummary { get; set; }
}

/// <summary>
/// Summary of applied filters
/// </summary>
public class FilterSummaryDto
{
    /// <summary>
    /// Total requests without filters
    /// </summary>
    public int TotalRequests { get; set; }

    /// <summary>
    /// Filtered requests count
    /// </summary>
    public int FilteredRequests { get; set; }

    /// <summary>
    /// Applied filters description
    /// </summary>
    public List<string> AppliedFilters { get; set; } = new();
}

