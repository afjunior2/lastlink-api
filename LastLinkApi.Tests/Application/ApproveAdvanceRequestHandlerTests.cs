using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Handlers;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.Repositories;
using LastLinkApi.Domain.ValueObjects;
using Moq;
using Xunit;

namespace LastLinkApi.Tests.Application
{
    public class ApproveAdvanceRequestHandlerTests
    {
        [Fact]
        public async Task Deve_Aprovar_Solicitacao()
        {
            // Arrange
            var request = new AdvanceRequest("user-123", 1000);
            request.SetId(1);

            var repoMock = new Mock<IAdvanceRequestRepository>();
            repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<AdvanceRequest>()))
                .Returns(Task.CompletedTask);

            var handler = new ApproveAdvanceRequestHandler(repoMock.Object);
            var command = new ApproveAdvanceRequestCommand(1);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(RequestStatus.Approved, result.Status);
            repoMock.Verify(r => r.UpdateAsync(It.Is<AdvanceRequest>(a => a.Id == 1)), Times.Once);
        }
    }
}