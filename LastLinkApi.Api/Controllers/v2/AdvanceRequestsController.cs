using Microsoft.AspNetCore.Mvc;
using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Queries;
using LastLinkApi.Api.DTOs;

namespace LastLinkApi.Api.Controllers.v2;

/// <summary>
/// API v2 para gestão de solicitações de antecipação
/// Inclui payload expandido (ex.: retorna NetAmount em todos os endpoints).
/// </summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AdvanceRequestsController(IMediator mediator, ILogger<AdvanceRequestsController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AdvanceRequestResponseDto>> CreateAdvanceRequest([FromBody] CreateAdvanceRequestDto request)
    {
        logger.LogInformation("v2: Criando solicitação de antecipação. Criador={CreatorId}, Valor={Amount}", request.CreatorId, request.RequestedAmount);

        var command = new CreateAdvanceRequestCommand(request.CreatorId, request.RequestedAmount, request.RequestDate);
        var advanceRequest = await mediator.Send(command);

        logger.LogInformation("v2: Solicitação criada com sucesso. Id={Id}, Criador={CreatorId}", advanceRequest.Id, advanceRequest.CreatorId);

        var dto = AdvanceRequestResponseDto.FromEntity(advanceRequest);
        dto.NetAmount = advanceRequest.NetAmount;
        return CreatedAtAction(nameof(GetById), new { id = advanceRequest.Id }, dto);
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<ActionResult<CreatorRequestsResponseDto>> GetByCreator(string creatorId)
    {
        logger.LogInformation("v2: Listando solicitações do Criador={CreatorId}", creatorId);

        var query = new GetAdvanceRequestsByCreatorQuery(creatorId);
        var requests = await mediator.Send(query);

        var advanceRequests = requests.ToList();
        logger.LogInformation("v2: Consulta concluída. Total={Count} solicitações encontradas", advanceRequests.Count());

        return Ok(new CreatorRequestsResponseDto
        {
            CreatorId = creatorId,
            Requests = advanceRequests.Select(r =>
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
        logger.LogInformation("v2: Buscando solicitação Id={Id}", id);

        var query = new GetAdvanceRequestsByCreatorQuery("user-123");
        var requests = await mediator.Send(query);
        var result = requests.FirstOrDefault(r => r.Id == id);

        if (result == null)
        {
            logger.LogWarning("v2: Solicitação Id={Id} não encontrada", id);
            return NotFound(new { error = "Solicitação não encontrada." });
        }

        logger.LogInformation("v2: Solicitação encontrada. Id={Id}, Criador={CreatorId}", result.Id, result.CreatorId);

        var dto = AdvanceRequestResponseDto.FromEntity(result);
        dto.NetAmount = result.NetAmount;
        return Ok(dto);
    }

    [HttpPut("{id:int}/approve")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> ApproveAdvanceRequest(int id)
    {
        logger.LogInformation("v2: Aprovando solicitação Id={Id}", id);

        var command = new ApproveAdvanceRequestCommand(id);
        var advanceRequest = await mediator.Send(command);

        logger.LogInformation("v2: Solicitação Id={Id} aprovada", id);

        var dto = AdvanceRequestResponseDto.FromEntity(advanceRequest);
        dto.NetAmount = advanceRequest.NetAmount;
        return Ok(dto);
    }

    [HttpPut("{id:int}/reject")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> RejectAdvanceRequest(int id)
    {
        logger.LogInformation("v2: Recusando solicitação Id={Id}", id);

        var command = new RejectAdvanceRequestCommand(id);
        var advanceRequest = await mediator.Send(command);

        logger.LogInformation("v2: Solicitação Id={Id} recusada", id);

        var dto = AdvanceRequestResponseDto.FromEntity(advanceRequest);
        dto.NetAmount = advanceRequest.NetAmount;
        return Ok(dto);
    }

    [HttpGet("simulate")]
    public async Task<ActionResult<SimulationResponseDto>> SimulateAdvanceRequest([FromQuery] decimal requestedAmount)
    {
        logger.LogInformation("v2: Simulando solicitação. Valor={Amount}", requestedAmount);

        var query = new SimulateAdvanceRequestQuery(requestedAmount);
        var simulation = await mediator.Send(query);

        logger.LogInformation("v2: Simulação concluída. Valor Líquido={NetAmount}", simulation.NetAmount);

        return Ok(SimulationResponseDto.FromValueObject(simulation));
    }
}
