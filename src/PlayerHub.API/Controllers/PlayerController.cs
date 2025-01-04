using Core.Head.Wrappers;
using IdentityAuthGuard.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using PlayerHub.API.Filters;
using PlayerHub.Application.Commands;
using PlayerHub.Application.DTOs.PlayerDTOs;
using PlayerHub.Application.Queries;
using PlayerHub.Domain;

namespace PlayerHub.API.Controllers
{
    [Route("api/player")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Policy = ApiKeyRequirement.Scheme, Roles = $"{DefaultRoles.User}, {DefaultRoles.Admin}")]
    public class PlayerController(IMediator mediator) : AppController
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Get players
        /// </summary>
        /// <param name="oData"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = Schemes.UserScheme)]
        [ProducesResponseType<PageResponse<PlayerDTO>>(200)]
        public async Task<IActionResult> Get(ODataQueryOptions<Player> oData, CancellationToken cancellationToken)
        {
            var query = new PlayerQuery(oData);
            var response = await _mediator.Send(query, cancellationToken);

            return GenerateResponse(response);
        }

        /// <summary>
        /// Create player
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = Schemes.UserScheme)]
        [ProducesResponseType<Response<PlayerDTO>>(200)]
        [ProducesResponseType<ProblemDetails>(400)]
        public async Task<IActionResult> Create([FromBody] CreatePlayerDTO dto, CancellationToken cancellationToken)
        {
            var command = new CreatePlayerCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            return GenerateResponse(response);
        }

        /// <summary>
        /// Update player
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = Schemes.UserScheme)]
        [ProducesResponseType<Response<PlayerDTO>>(200)]
        [ProducesResponseType<ProblemDetails>(400)]
        public async Task<IActionResult> Update([FromBody] UpdatePlayerDTO dto, CancellationToken cancellationToken)
        {
            var command = new UpdatePlayerCommand(dto);
            var response = await _mediator.Send(command, cancellationToken);

            return GenerateResponse(response);
        }

        /// <summary>
        /// Delete player
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = Schemes.UserScheme)]
        [ProducesResponseType<Response>(200)]
        [ProducesResponseType<ProblemDetails>(400)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeletePlayerCommand(id);
            var response = await _mediator.Send(command, cancellationToken);

            return GenerateResponse(response);
        }

        /// <summary>
        /// Get best players
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("best-players")]
        [Authorize(Policy = Schemes.UserScheme)]
        [ProducesResponseType<Response<List<PlayerDTO>>>(200)]
        public async Task<IActionResult> GetBestPlayer([FromQuery] List<BestPlayerRequestDTO> dto, CancellationToken cancellationToken)
        {
            var query = new BestPlayerQuery(dto);
            var response = await _mediator.Send(query, cancellationToken);

            return GenerateResponse(response);
        }
    }
}
