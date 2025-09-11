using System.Net;
using System.Net.Http.Json;
using LastLinkApi.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using LastLinkApi.Api;
using Xunit;

namespace LastLinkApi.Tests.Integration;

public class AdvanceRequestsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AdvanceRequestsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Simulate_ComValorInvalido_DeveRetornarBadRequest()
    {
        var response = await _client.GetAsync("/api/v1/AdvanceRequests/simulate?requestedAmount=50");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        Assert.NotNull(error);
        Assert.Equal("O valor solicitado deve ser maior que R$100,00.", error.Message);
    }

    [Fact]
    public async Task GetByCreator_DeveRetornarOk()
    {
        var response = await _client.GetAsync("/api/v1/AdvanceRequests/creator/user-123");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Create_DeveRetornarCreated()
    {
        var random = new Random();
        int valor = random.Next();
        int user = random.Next();
        
        var request = new CreateAdvanceRequestDto
        {
            CreatorId = "user-" + user.ToString(),
            RequestedAmount = valor,
            RequestDate = DateTime.UtcNow
        };

        var response = await _client.PostAsJsonAsync("/api/v1/AdvanceRequests", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}