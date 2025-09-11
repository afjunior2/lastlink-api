using LastLinkApi.Application.Handlers;
using LastLinkApi.Application.Queries;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;
using Moq;
using Xunit;

namespace LastLinkApi.Tests.Application
{
    public class GetAdvanceRequestsByCreatorHandlerTests
    {
        [Fact]
        public async Task Deve_Retornar_Solicitacoes_Por_Criador()
        {
            // Arrange
            var solicitacoes = new List<AdvanceRequest>
            {
                new AdvanceRequest("user-123", 1000),
                new AdvanceRequest("user-123", 2000)
            };

            var repoMock = new Mock<IAdvanceRequestRepository>();
            repoMock.Setup(r => r.GetByCreatorAsync("user-123"))
                .ReturnsAsync(solicitacoes);

            var handler = new GetAdvanceRequestsByCreatorHandler(repoMock.Object);
            var query = new GetAdvanceRequestsByCreatorQuery("user-123");

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            var advanceRequests = result as AdvanceRequest[] ?? result.ToArray();
            Assert.Equal(2, advanceRequests.Count());
            Assert.All(advanceRequests, r => Assert.Equal("user-123", r.CreatorId));

            repoMock.Verify(r => r.GetByCreatorAsync("user-123"), Times.Once);
        }

    }
}