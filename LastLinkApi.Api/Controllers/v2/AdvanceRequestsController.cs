using Microsoft.AspNetCore.Mvc;
using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Queries;
using LastLinkApi.Api.DTOs;

namespace LastLinkApi.Api.Controllers.v2;

/// <summary>
/// API v2 para gestão de solicitações de antecipação
/// Inclui payload expandido (NetAmount em todas as respostas).
/// </summary>
[ApiController]
[ApiVersion("2.0")]
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

        var dto = AdvanceRequestResponseDto.FromEntity(advanceRequest);
        dto.NetAmount = advanceRequest.NetAmount;
        return CreatedAtAction(nameof(GetById), new { id = advanceRequest.Id }, dto);
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<ActionResult<CreatorRequestsResponseDto>> GetByCreator(string creatorId)
    {
        var query = new GetAdvanceRequestsByCreatorQuery(creatorId);
        var requests = await mediator.Send(query);

        return Ok(new CreatorRequestsResponseDto
        {
            CreatorId = creatorId,
            Requests = requests.Select(r =>
            {
                var dto = AdvanceRequestResponseDto.FromEntity(r);
                dto.NetAmount = r.NetAmount;
                return dto;
            })
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> GetById(int id)
    {
        var query = new GetAdvanceRequestsByCreatorQuery("user-123"); // exemplo
        var requests = await mediator.Send(query);
        var result = requests.FirstOrDefault(r => r.Id == id);

        if (result == null)
            return NotFound(new ErrorResponseDto { Message = "Solicitação não encontrada." });

        var dto = AdvanceRequestResponseDto.FromEntity(result);
        dto.NetAmount = result.NetAmount;
        return Ok(dto);
    }

    [HttpPut("{id:int}/approve")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> ApproveAdvanceRequest(int id)
    {
        var command = new ApproveAdvanceRequestCommand(id);
        var advanceRequest = await mediator.Send(command);

        var dto = AdvanceRequestResponseDto.FromEntity(advanceRequest);
        dto.NetAmount = advanceRequest.NetAmount;
        return Ok(dto);
    }

    [HttpPut("{id:int}/reject")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> RejectAdvanceRequest(int id)
    {
        var command = new RejectAdvanceRequestCommand(id);
        var advanceRequest = await mediator.Send(command);

        var dto = AdvanceRequestResponseDto.FromEntity(advanceRequest);
        dto.NetAmount = advanceRequest.NetAmount;
        return Ok(dto);
    }

    [HttpGet("simulate")]
    public async Task<ActionResult<SimulationResponseDto>> SimulateAdvanceRequest([FromQuery] decimal requestedAmount)
    {
        var query = new SimulateAdvanceRequestQuery(requestedAmount);
        var simulation = await mediator.Send(query);
        return Ok(SimulationResponseDto.FromValueObject(simulation));
    }
}
