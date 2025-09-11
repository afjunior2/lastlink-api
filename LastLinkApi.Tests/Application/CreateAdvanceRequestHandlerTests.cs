using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Handlers;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;
using LastLinkApi.Domain.ValueObjects;
using Moq;
using Xunit;

namespace LastLinkApi.Tests.Application
{
    public class CreateAdvanceRequestHandlerTests
    {
        [Fact]
        public async Task Deve_Criar_Solicitacao()
        {
            // Arrange
            var repoMock = new Mock<IAdvanceRequestRepository>();

            repoMock.Setup(r => r.SaveAsync(It.IsAny<AdvanceRequest>()))
                .ReturnsAsync((AdvanceRequest req) =>
                {
                    req.SetId(1); // simula auto-incremento
                    return req;
                });

            var handler = new CreateAdvanceRequestHandler(repoMock.Object);
            var command = new CreateAdvanceRequestCommand("user-123", 1500);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user-123", result.CreatorId);
            Assert.Equal(1500, result.RequestedAmount);
            Assert.Equal(RequestStatus.Pending, result.Status);

            repoMock.Verify(r => r.SaveAsync(It.Is<AdvanceRequest>(a => a.CreatorId == "user-123")), Times.Once);
        }
    }
}