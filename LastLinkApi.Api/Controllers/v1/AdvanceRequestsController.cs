using Microsoft.AspNetCore.Mvc;
using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Queries;
using LastLinkApi.Api.DTOs;

namespace LastLinkApi.Api.Controllers.v1;

/// <summary>
/// API v1 para gestão de solicitações de antecipação
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AdvanceRequestsController(IMediator mediator, ILogger<AdvanceRequestsController> loggerMockObject)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AdvanceRequestResponseDto>> CreateAdvanceRequest([FromBody] CreateAdvanceRequestDto request)
    {
        var command = new CreateAdvanceRequestCommand(request.CreatorId, request.RequestedAmount, request.RequestDate);
        var advanceRequest = await mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = advanceRequest.Id }, AdvanceRequestResponseDto.FromEntity(advanceRequest));
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<ActionResult<CreatorRequestsResponseDto>> GetByCreator(string creatorId)
    {
        var query = new GetAdvanceRequestsByCreatorQuery(creatorId);
        var requests = await mediator.Send(query);

        return Ok(new CreatorRequestsResponseDto
        {
            CreatorId = creatorId,
            Requests = requests.Select(AdvanceRequestResponseDto.FromEntity)
        });
    }

    [HttpGet("{id:int}")]
    public ActionResult<AdvanceRequestResponseDto> GetById(int id)
    {
        return NotFound(new ErrorResponseDto { Message = "Solicitação de antecipação não encontrada." });
    }

    [HttpPut("{id:int}/approve")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> ApproveAdvanceRequest(int id)
    {
        var command = new ApproveAdvanceRequestCommand(id);
        var advanceRequest = await mediator.Send(command);
        return Ok(AdvanceRequestResponseDto.FromEntity(advanceRequest));
    }

    [HttpPut("{id:int}/reject")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> RejectAdvanceRequest(int id)
    {
        var command = new RejectAdvanceRequestCommand(id);
        var advanceRequest = await mediator.Send(command);
        return Ok(AdvanceRequestResponseDto.FromEntity(advanceRequest));
    }

    [HttpGet("simulate")]
    public async Task<ActionResult<SimulationResponseDto>> SimulateAdvanceRequest([FromQuery] decimal requestedAmount)
    {
        var query = new SimulateAdvanceRequestQuery(requestedAmount);
        var simulation = await mediator.Send(query);
        return Ok(SimulationResponseDto.FromValueObject(simulation));
    }
}
