using LastLinkApi.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LastLinkApi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvanceRequestsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdvanceRequestCommand command)
        {
            try
            {
                var result = await mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Não foi possível criar a solicitação: {ex.Message}");
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro inesperado ao criar a solicitação.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var query = new Application.Queries.GetAdvanceRequestsByCreatorQuery("user-123");
                var result = await mediator.Send(query);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno ao buscar a solicitação.");
            }
        }
    }
}