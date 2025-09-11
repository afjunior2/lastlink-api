using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.ValueObjects;
using Xunit;

namespace LastLinkApi.Tests.Domain
{
    public class AdvanceRequestTests
    {
        [Fact]
        public void Deve_Criar_Solicitacao_Valida()
        {
            var request = new AdvanceRequest("user-123", 1000);

            Assert.Equal("user-123", request.CreatorId);
            Assert.Equal(1000, request.RequestedAmount);
            Assert.Equal(50, request.FeeAmount); // 5%
            Assert.Equal(950, request.NetAmount);
            Assert.Equal(RequestStatus.Pending, request.Status);
        }

        [Fact]
        public void Nao_Deve_Criar_Com_Valor_Invalido()
        {
            Assert.Throws<ArgumentException>(() =>
                new AdvanceRequest("user-123", 50));
        }

        [Fact]
        public void Deve_Aprovar_Solicitacao_Pendente()
        {
            var request = new AdvanceRequest("user-123", 1000);
            request.Approve();

            Assert.Equal(RequestStatus.Approved, request.Status);
        }

        [Fact]
        public void Nao_Deve_Aprovar_Se_Ja_Aprovada()
        {
            var request = new AdvanceRequest("user-123", 1000);
            request.Approve();

            Assert.Throws<InvalidOperationException>(() => request.Approve());
        }
    }
}