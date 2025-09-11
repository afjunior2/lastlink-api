using Xunit;
using Moq;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using LastLinkApi.Api.Controllers.v2;
using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Queries;
using LastLinkApi.Api.DTOs;
using LastLinkApi.Domain.Entities;
using LastLinkApi.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace LastLinkApi.Tests.Application
{
    public class AdvanceRequestsControllerV2Tests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AdvanceRequestsController _controller;

        public AdvanceRequestsControllerV2Tests()
        {
            _mediatorMock = new Mock<IMediator>();
            var loggerMock = new Mock<ILogger<AdvanceRequestsController>>();
            _controller = new AdvanceRequestsController(_mediatorMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task CreateAdvanceRequest_DeveRetornarCreatedComNetAmount()
        {
            var dto = new CreateAdvanceRequestDto { CreatorId = "user-123", RequestedAmount = 500 };
            var entity = new AdvanceRequest("user-123", 500);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateAdvanceRequestCommand>(), default))
                .ReturnsAsync(entity);

            var result = await _controller.CreateAdvanceRequest(dto);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<AdvanceRequestResponseDto>(created.Value);
            Assert.Equal(entity.NetAmount, response.NetAmount);
        }

        [Fact]
        public async Task GetByCreator_DeveRetornarOkComNetAmount()
        {
            var creatorId = "user-123";
            var requests = new List<AdvanceRequest> { new AdvanceRequest(creatorId, 1500) };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAdvanceRequestsByCreatorQuery>(), default))
                .ReturnsAsync(requests);

            var result = await _controller.GetByCreator(creatorId);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<CreatorRequestsResponseDto>(ok.Value);
            Assert.Single(response.Requests);
            Assert.Equal(requests.First().NetAmount, response.Requests.First().NetAmount);
        }

        [Fact]
        public async Task GetById_DeveRetornarOkQuandoEncontrado()
        {
            var entity = new AdvanceRequest("user-123", 2000);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAdvanceRequestsByCreatorQuery>(), default))
                .ReturnsAsync(new List<AdvanceRequest> { entity });

            var result = await _controller.GetById(entity.Id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AdvanceRequestResponseDto>(ok.Value);
            Assert.Equal(entity.Id, response.Id);
        }

        [Fact]
        public async Task ApproveAdvanceRequest_DeveRetornarOkComNetAmount()
        {
            var entity = new AdvanceRequest("user-123", 2000);
            entity.Approve();

            _mediatorMock.Setup(m => m.Send(It.IsAny<ApproveAdvanceRequestCommand>(), default))
                .ReturnsAsync(entity);

            var result = await _controller.ApproveAdvanceRequest(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AdvanceRequestResponseDto>(ok.Value);
            Assert.Equal(nameof(RequestStatus.Approved), response.Status);
            Assert.Equal(entity.NetAmount, response.NetAmount);
        }

        [Fact]
        public async Task RejectAdvanceRequest_DeveRetornarOkComNetAmount()
        {
            var entity = new AdvanceRequest("user-123", 2000);
            entity.Reject();

            _mediatorMock.Setup(m => m.Send(It.IsAny<RejectAdvanceRequestCommand>(), default))
                .ReturnsAsync(entity);

            var result = await _controller.RejectAdvanceRequest(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AdvanceRequestResponseDto>(ok.Value);
            Assert.Equal(nameof(RequestStatus.Rejected), response.Status);
            Assert.Equal(entity.NetAmount, response.NetAmount);
        }

        [Fact]
        public async Task SimulateAdvanceRequest_DeveRetornarOk()
        {
            var simulation = AdvanceRequest.Simulate(2000);

            _mediatorMock.Setup(m => m.Send(It.IsAny<SimulateAdvanceRequestQuery>(), default))
                .ReturnsAsync(simulation);

            var result = await _controller.SimulateAdvanceRequest(2000);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<SimulationResponseDto>(ok.Value);
            Assert.Equal(2000, response.RequestedAmount);
        }
    }
}
