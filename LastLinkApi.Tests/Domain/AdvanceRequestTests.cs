using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.ValueObjects;
using Xunit;

namespace LastLinkApi.Tests.Domain;

public class AdvanceRequestTests
{
    [Fact]
    public void CriarSolicitacao_ComValorInvalido_DeveLancarExcecao()
    {
        var ex = Assert.Throws<ArgumentException>(() => new AdvanceRequest("user-123", 50, DateTime.UtcNow));

        Assert.Contains("O valor solicitado deve ser maior que R$100,00", ex.Message);
    }

    [Fact]
    public void CriarSolicitacao_Valida_DeveTerStatusPendente()
    {
        var request = new AdvanceRequest("user-123", 1000);

        Assert.Equal(RequestStatus.Pending, request.Status);
        Assert.Equal("user-123", request.CreatorId);
        Assert.Equal(1000, request.RequestedAmount);
        Assert.Equal(950, request.NetAmount); // 5% fee aplicado
    }

    [Fact]
    public void AprovarSolicitacao_DeveAlterarStatusParaAprovado()
    {
        var request = new AdvanceRequest("user-123", 2000);

        request.Approve();

        Assert.Equal(RequestStatus.Approved, request.Status);
    }

    [Fact]
    public void RejeitarSolicitacao_DeveAlterarStatusParaRejeitado()
    {
        var request = new AdvanceRequest("user-123", 2000);

        request.Reject();

        Assert.Equal(RequestStatus.Rejected, request.Status);
    }
}