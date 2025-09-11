using LastLinkApi.Api.Controllers.v1;
using LastLinkApi.Api.DTOs;
using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Queries;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LastLinkApi.Tests.Application;

public class AdvanceRequestsControllerV1Tests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AdvanceRequestsController _controller;

    public AdvanceRequestsControllerV1Tests()
    {
        _mediatorMock = new Mock<IMediator>();
        var loggerMock = new Mock<ILogger<AdvanceRequestsController>>();
        _controller = new AdvanceRequestsController(_mediatorMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task CreateAdvanceRequest_DeveRetornarCreated()
    {
        var dto = new CreateAdvanceRequestDto { CreatorId = "user-123", RequestedAmount = 500 };
        var entity = new AdvanceRequest("user-123", 500);

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAdvanceRequestCommand>(), default))
            .ReturnsAsync(entity);

        var result = await _controller.CreateAdvanceRequest(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var responseDto = Assert.IsType<AdvanceRequestResponseDto>(createdResult.Value);

        Assert.Equal("user-123", responseDto.CreatorId);
        Assert.Equal(nameof(RequestStatus.Pending), responseDto.Status);
    }

    [Fact]
    public async Task GetByCreator_DeveRetornarOk()
    {
        var requests = new List<AdvanceRequest> { new AdvanceRequest("user-123", 1000) };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetAdvanceRequestsByCreatorQuery>(), default))
            .ReturnsAsync(requests);

        var result = await _controller.GetByCreator("user-123");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CreatorRequestsResponseDto>(okResult.Value);

        Assert.Single(response.Requests);
    }

    [Fact]
    public void GetById_DeveRetornarNotFound()
    {
        var result = _controller.GetById(99);

        var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
        var response = notFound.Value;

        Assert.Equal("Solicitação de antecipação não encontrada.", ((ErrorResponseDto)response!)?.Message);
    }
}
