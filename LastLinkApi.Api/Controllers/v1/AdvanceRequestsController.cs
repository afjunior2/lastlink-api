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
public class AdvanceRequestsController(IMediator mediator, ILogger<AdvanceRequestsController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AdvanceRequestResponseDto>> CreateAdvanceRequest([FromBody] CreateAdvanceRequestDto request)
    {
        logger.LogInformation("Recebida requisição para criar solicitação de antecipação. Criador={CreatorId}, Valor={Amount}", request.CreatorId, request.RequestedAmount);

        try
        {
            var command = new CreateAdvanceRequestCommand(request.CreatorId, request.RequestedAmount, request.RequestDate);
            var advanceRequest = await mediator.Send(command);

            logger.LogInformation("Solicitação criada com sucesso. Id={Id}, Criador={CreatorId}", advanceRequest.Id, advanceRequest.CreatorId);

            return CreatedAtAction(nameof(GetById), new { id = advanceRequest.Id }, AdvanceRequestResponseDto.FromEntity(advanceRequest));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao criar solicitação de antecipação para Criador={CreatorId}", request.CreatorId);
            throw;
        }
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<ActionResult<CreatorRequestsResponseDto>> GetByCreator(string creatorId)
    {
        logger.LogInformation("Recebida requisição para listar solicitações do Criador={CreatorId}", creatorId);

        try
        {
            var query = new GetAdvanceRequestsByCreatorQuery(creatorId);
            var requests = await mediator.Send(query);

            var advanceRequests = requests.ToList();
            logger.LogInformation("Consulta concluída. Total={Count} solicitações encontradas para Criador={CreatorId}", advanceRequests.Count(), creatorId);

            return Ok(new CreatorRequestsResponseDto
            {
                CreatorId = creatorId,
                Requests = advanceRequests.Select(AdvanceRequestResponseDto.FromEntity)
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao listar solicitações para Criador={CreatorId}", creatorId);
            throw;
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<AdvanceRequestResponseDto> GetById(int id)
    {
        logger.LogInformation("Recebida requisição para buscar solicitação Id={Id}", id);

        // v1 retorna sempre NotFound (placeholder)
        logger.LogWarning("Solicitação Id={Id} não encontrada", id);

        return NotFound(new { error = "Solicitação de antecipação não encontrada." });
    }

    [HttpPut("{id:int}/approve")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> ApproveAdvanceRequest(int id)
    {
        logger.LogInformation("Recebida requisição para aprovar solicitação Id={Id}", id);

        try
        {
            var command = new ApproveAdvanceRequestCommand(id);
            var advanceRequest = await mediator.Send(command);

            logger.LogInformation("Solicitação Id={Id} aprovada com sucesso", id);

            return Ok(AdvanceRequestResponseDto.FromEntity(advanceRequest));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao aprovar solicitação Id={Id}", id);
            throw;
        }
    }

    [HttpPut("{id:int}/reject")]
    public async Task<ActionResult<AdvanceRequestResponseDto>> RejectAdvanceRequest(int id)
    {
        logger.LogInformation("Recebida requisição para recusar solicitação Id={Id}", id);

        try
        {
            var command = new RejectAdvanceRequestCommand(id);
            var advanceRequest = await mediator.Send(command);

            logger.LogInformation("Solicitação Id={Id} recusada com sucesso", id);

            return Ok(AdvanceRequestResponseDto.FromEntity(advanceRequest));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao recusar solicitação Id={Id}", id);
            throw;
        }
    }

    [HttpGet("simulate")]
    public async Task<ActionResult<SimulationResponseDto>> SimulateAdvanceRequest([FromQuery] decimal requestedAmount)
    {
        logger.LogInformation("Recebida requisição para simulação de solicitação. Valor={Amount}", requestedAmount);

        try
        {
            var query = new SimulateAdvanceRequestQuery(requestedAmount);
            var simulation = await mediator.Send(query);

            logger.LogInformation("Simulação concluída com sucesso. Valor Líquido={NetAmount}", simulation.NetAmount);

            return Ok(SimulationResponseDto.FromValueObject(simulation));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao simular solicitação de antecipação com Valor={Amount}", requestedAmount);
            throw;
        }
    }
}
