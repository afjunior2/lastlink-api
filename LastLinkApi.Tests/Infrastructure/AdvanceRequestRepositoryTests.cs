using LastLinkApi.Domain.Entities;
using LastLinkApi.Infrastructure.Data;
using LastLinkApi.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LastLinkApi.Tests.Infrastructure
{
    public class AdvanceRequestRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Deve_Salvar_E_Buscar_Por_Id()
        {
            await using var db = GetInMemoryDb();
            var repo = new AdvanceRequestRepository(db);

            var request = new AdvanceRequest("user-123", 1000);
            var saved = await repo.SaveAsync(request);

            var fetched = await repo.GetByIdAsync(saved.Id);

            Assert.NotNull(fetched);
            Assert.Equal("user-123", fetched.CreatorId);
        }

        [Fact]
        public async Task Deve_Retornar_Todas_As_Solicitacoes()
        {
            await using var db = GetInMemoryDb();
            var repo = new AdvanceRequestRepository(db);

            await repo.SaveAsync(new AdvanceRequest("user-1", 1000));
            await repo.SaveAsync(new AdvanceRequest("user-2", 2000));

            var all = await repo.GetAllAsync();

            Assert.NotNull(all);
            Assert.Equal(2, all.Count());
        }

        [Fact]
        public async Task Deve_Verificar_Se_Ha_Solicitacoes_Pendentes()
        {
            await using var db = GetInMemoryDb();
            var repo = new AdvanceRequestRepository(db);

            var request = new AdvanceRequest("user-123", 1500);
            await repo.SaveAsync(request);

            var hasPending = await repo.HasPendingRequestAsync("user-123");

            Assert.True(hasPending);
        }

        [Fact]
        public async Task Deve_Retornar_Falso_Quando_Nao_Houver_Solicitacoes_Pendentes()
        {
            await using var db = GetInMemoryDb();
            var repo = new AdvanceRequestRepository(db);

            var hasPending = await repo.HasPendingRequestAsync("user-999");

            Assert.False(hasPending);
        }
    }
}
