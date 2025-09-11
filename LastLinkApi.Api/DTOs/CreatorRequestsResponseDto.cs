namespace LastLinkApi.Api.DTOs;

/// <summary>
/// DTO 
/// </summary>
public class CreatorRequestsResponseDto
{
 
    public string CreatorId { get; set; } = string.Empty;
    
    public IEnumerable<AdvanceRequestResponseDto> Requests { get; set; } = new List<AdvanceRequestResponseDto>();
}