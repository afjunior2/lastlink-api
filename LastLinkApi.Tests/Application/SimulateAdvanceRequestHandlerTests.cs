using LastLinkApi.Application.Handlers;
using LastLinkApi.Application.Queries;
using Xunit;

namespace LastLinkApi.Tests.Application
{
    public class SimulateAdvanceRequestHandlerTests
    {
        [Fact]
        public async Task Deve_Simular_Solicitacao()
        {
            // Arrange
            var handler = new SimulateAdvanceRequestHandler();
            var query = new SimulateAdvanceRequestQuery(2000);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2000, result.RequestedAmount);
            Assert.Equal(100, result.FeeAmount); // 5% de 2000
            Assert.Equal(1900, result.NetAmount);
            Assert.Equal(5.0m, result.FeePercentage);
        }
    }
}