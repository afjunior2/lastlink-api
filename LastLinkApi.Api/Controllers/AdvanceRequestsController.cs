using Microsoft.AspNetCore.Mvc;
using MediatR;
using LastLinkApi.Application.Commands;
using LastLinkApi.Application.Queries;
using LastLinkApi.Api.DTOs;

namespace LastLinkApi.Api.Controllers;

/// <summary>
/// API para gestão de solicitações de antecipação
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AdvanceRequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdvanceRequestsController> _logger;

    public AdvanceRequestsController(IMediator mediator, ILogger<AdvanceRequestsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// cria uma nova solicitação de antecipação
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AdvanceRequestResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdvanceRequestResponseDto>> CreateAdvanceRequest([FromBody] CreateAdvanceRequestDto request)
    {
        try
        {
            _logger.LogInformation("Criando solicitação de antecipação para o criador {CreatorId}", request.CreatorId);

            var command = new CreateAdvanceRequestCommand(
                request.CreatorId,
                request.RequestedAmount,
                request.RequestDate
            );

            var advanceRequest = await _mediator.Send(command);
            var response = AdvanceRequestResponseDto.FromEntity(advanceRequest);

            _logger.LogInformation("Solicitação de antecipação {Id} criada com sucesso", advanceRequest.Id);
            return CreatedAtAction(nameof(GetById), new { id = advanceRequest.Id }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Erro de validação ao criar solicitação: {Message}", ex.Message);
            return BadRequest(new { error = "Dados inválidos para criar a solicitação de antecipação." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro de negócio ao criar solicitação: {Message}", ex.Message);
            return Conflict(new { error = "O criador já possui uma solicitação pendente." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao criar solicitação de antecipação");
            return StatusCode(500, new { error = "Erro interno ao criar a solicitação de antecipação." });
        }
    }

    /// <summary>
    /// retorna todas as solicitações de antecipação
    /// </summary>
    [HttpGet("creator/{creatorId}")]
    [ProducesResponseType(typeof(CreatorRequestsResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreatorRequestsResponseDto>> GetByCreator(string creatorId)
    {
        try
        {
            _logger.LogInformation("Buscando solicitações de antecipação para o criador {CreatorId}", creatorId);

            var query = new GetAdvanceRequestsByCreatorQuery(creatorId);
            var requests = await _mediator.Send(query);
            var requestDtos = requests.Select(AdvanceRequestResponseDto.FromEntity);

            var response = new CreatorRequestsResponseDto
            {
                CreatorId = creatorId,
                Requests = requestDtos
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar solicitações de antecipação para o criador {CreatorId}", creatorId);
            return StatusCode(500, new { error = "Erro interno ao buscar as solicitações do criador." });
        }
    }

    /// <summary>
    /// Retorna uma solicitação de antecipação pelo ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AdvanceRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public ActionResult<AdvanceRequestResponseDto> GetById(int id)
    {
        return NotFound(new { error = "Solicitação de antecipação não encontrada." });
    }
    
    /// <summary>
    /// Aprova uma solicitação de antecipação
    /// </summary>
    [HttpPut("{id:int}/approve")]
    [ProducesResponseType(typeof(AdvanceRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdvanceRequestResponseDto>> ApproveAdvanceRequest(int id)
    {
        try
        {
            _logger.LogInformation("Aprovando solicitação de antecipação {Id}", id);

            var command = new ApproveAdvanceRequestCommand(id);
            var advanceRequest = await _mediator.Send(command);
            var response = AdvanceRequestResponseDto.FromEntity(advanceRequest);

            _logger.LogInformation("Solicitação de antecipação {Id} aprovada com sucesso", id);
            return Ok(response);
        }
        catch (ArgumentException)
        {
            _logger.LogWarning("Solicitação de antecipação {Id} não encontrada", id);
            return NotFound(new { error = "Solicitação de antecipação não encontrada." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao aprovar solicitação {Id}: {Message}", id, ex.Message);
            return Conflict(new { error = "Não é possível aprovar esta solicitação (status inválido)." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao aprovar solicitação {Id}", id);
            return StatusCode(500, new { error = "Erro interno ao aprovar a solicitação de antecipação." });
        }
    }

    /// <summary>
    /// Recusa uma solicitação de antecipação
    /// </summary>
    [HttpPut("{id:int}/reject")]
    [ProducesResponseType(typeof(AdvanceRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AdvanceRequestResponseDto>> RejectAdvanceRequest(int id)
    {
        try
        {
            _logger.LogInformation("Recusando solicitação de antecipação {Id}", id);

            var command = new RejectAdvanceRequestCommand(id);
            var advanceRequest = await _mediator.Send(command);
            var response = AdvanceRequestResponseDto.FromEntity(advanceRequest);

            _logger.LogInformation("Solicitação de antecipação {Id} recusada com sucesso", id);
            return Ok(response);
        }
        catch (ArgumentException)
        {
            _logger.LogWarning("Solicitação de antecipação {Id} não encontrada", id);
            return NotFound(new { error = "Solicitação de antecipação não encontrada." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Erro ao recusar solicitação {Id}: {Message}", id, ex.Message);
            return Conflict(new { error = "Não é possível recusar esta solicitação (status inválido)." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao recusar solicitação {Id}", id);
            return StatusCode(500, new { error = "Erro interno ao recusar a solicitação de antecipação." });
        }
    }

    /// <summary>
    /// Simula uma solicitação de antecipação
    /// </summary>
    [HttpGet("simulate")]
    [ProducesResponseType(typeof(SimulationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SimulationResponseDto>> SimulateAdvanceRequest([FromQuery] decimal requestedAmount)
    {
        try
        {
            _logger.LogInformation("Simulando solicitação de antecipação com valor {Amount}", requestedAmount);

            var query = new SimulateAdvanceRequestQuery(requestedAmount);
            var simulation = await _mediator.Send(query);
            var response = SimulationResponseDto.FromValueObject(simulation);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Erro de validação na simulação: {Message}", ex.Message);
            return BadRequest(new { error = "Valor inválido: a simulação exige valor maior que R$100,00." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno na simulação de antecipação");
            return StatusCode(500, new { error = "Erro interno ao simular a solicitação de antecipação." });
        }
    }
}
