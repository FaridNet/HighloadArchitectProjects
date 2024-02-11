using Application.Features.User.Command.RegisterUser;
using Application.Features.User.Queries.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("user")]
[Authorize]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("register")]
    public async Task<long> Register(RegisterUserCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpGet("get/{id}")]
    public async Task<IActionResult> Get(long? id)
    {
        if (!id.HasValue)
        {
            return BadRequest("Невалидные данные");
        }

        UserDto? result = await _mediator.Send(new GetUserByIdQuery
        {
            UserId = id.Value
        });

        if (result == null)
        {
            return NotFound("Анкета не найдена");
        }

        return Ok(result);
    }
}
