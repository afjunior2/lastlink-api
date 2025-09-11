using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;
using LastLinkApi.Domain.ValueObjects;
using Xunit;

namespace LastLinkApi.Tests.Infrastructure
{
    public class InMemoryAdvanceRequestRepositoryTests
    {
        private IAdvanceRequestRepository GetRepository()
        {
            return new InMemoryAdvanceRequestRepository();
        }

        [Fact]
        public async Task Deve_Salvar_E_Buscar_Por_Id()
        {
            var repo = GetRepository();
            var request = new AdvanceRequest("user-123", 1000);

            var saved = await repo.SaveAsync(request);
            var fetched = await repo.GetByIdAsync(saved.Id);

            Assert.NotNull(fetched);
            Assert.Equal("user-123", fetched.CreatorId);
            Assert.Equal(1000, fetched.RequestedAmount);
        }

        [Fact]
        public async Task Deve_Retornar_Todas_As_Solicitacoes()
        {
            var repo = GetRepository();

            await repo.SaveAsync(new AdvanceRequest("user-1", 1000));
            await repo.SaveAsync(new AdvanceRequest("user-2", 2000));

            var all = await repo.GetAllAsync();

            Assert.Equal(2, all.Count());
        }

        [Fact]
        public async Task Deve_Retornar_Solicitacoes_Por_Criador()
        {
            var repo = GetRepository();

            await repo.SaveAsync(new AdvanceRequest("user-123", 1500));
            await repo.SaveAsync(new AdvanceRequest("user-123", 2500));
            await repo.SaveAsync(new AdvanceRequest("user-456", 3000));

            var results = await repo.GetByCreatorAsync("user-123");

            var advanceRequests = results as AdvanceRequest[] ?? results.ToArray();
            Assert.Equal(2, advanceRequests.Count());
            Assert.All(advanceRequests, r => Assert.Equal("user-123", r.CreatorId));
        }

        [Fact]
        public async Task Deve_Verificar_Se_Ha_Solicitacoes_Pendentes()
        {
            var repo = GetRepository();
            await repo.SaveAsync(new AdvanceRequest("user-123", 1800));

            var hasPending = await repo.HasPendingRequestAsync("user-123");

            Assert.True(hasPending);
        }

        [Fact]
        public async Task Deve_Retornar_Falso_Quando_Nao_Houver_Solicitacoes_Pendentes()
        {
            var repo = GetRepository();

            var hasPending = await repo.HasPendingRequestAsync("user-999");

            Assert.False(hasPending);
        }

        [Fact]
        public async Task Deve_Atualizar_Solicitacao()
        {
            var repo = GetRepository();
            var request = await repo.SaveAsync(new AdvanceRequest("user-123", 1000));

            request.Approve(); // altera o status
            await repo.UpdateAsync(request);

            var updated = await repo.GetByIdAsync(request.Id);

            Assert.NotNull(updated);
            Assert.Equal(RequestStatus.Approved, updated.Status);
        }

        [Fact]
        public async Task Deve_Excluir_Solicitacao()
        {
            var repo = GetRepository();
            var request = await repo.SaveAsync(new AdvanceRequest("user-123", 1000));

            await repo.DeleteAsync(request.Id);
            var deleted = await repo.GetByIdAsync(request.Id);

            Assert.Null(deleted);
        }
    }
}
