using MediatR;
using Microsoft.AspNetCore.Mvc;
using UsersAPI.Application.Commands;
using UsersAPI.Application.DTOs;
using UsersAPI.Application.Queries;
using UsersAPI.Domain.Exceptions;

namespace UsersAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> Get(Guid id)
        {
            var result = await _mediator.Send(new GetUserQuery(id));            // Exception handled at middleware to live up to single responsibility
            return Ok(result);
        }

        [HttpGet("{id:guid},{secondVariationofGetById:bool}")]
        public async Task<ActionResult<UserDto>> GetWithExceptionHandling(Guid id, Boolean idWithExceptionHandling) //This is second variation of GetUser handing its own error
        {
            try
            {
                var user = await _mediator.Send(new GetUserQuery(id));
                return Ok(user);
            }
            catch (NotFoundException ex)
            {   
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            if (id != command.Id)
                return BadRequest("Mismatched user ID");

            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}
