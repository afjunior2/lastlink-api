using System.ComponentModel.DataAnnotations;

namespace LastLinkApi.Api.DTOs;

public class CreateAdvanceRequestDto
{
    [Required(ErrorMessage = "O campo CreatorId é obrigatório.")]
    public string CreatorId { get; set; } = string.Empty;

    [Required(ErrorMessage = "O valor solicitado é obrigatório.")]
    [Range(100.01, double.MaxValue, ErrorMessage = "O valor solicitado deve ser maior que R$100,00.")]
    public decimal RequestedAmount { get; set; }

    public DateTime? RequestDate { get; set; }
}