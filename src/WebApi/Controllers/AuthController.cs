using Application.Features.User.Queries.GetUserIdByLogin;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Common.Dtos;
using WebApi.Common.Options;

using UserDto = Application.Features.User.Queries.GetUserIdByLogin.UserDto;

namespace WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IMediator mediator, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    /// <summary>
    /// Упрощенный процесс аутентификации путем передачи идентификатор пользователя и получения токена для дальнейшего прохождения авторизации
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (string.IsNullOrEmpty(loginRequest.Login) || string.IsNullOrEmpty(loginRequest.Password))
        {
            return BadRequest("Невалидные данные");
        }

        UserDto? user = await _mediator.Send(new GetUserIdByLoginQuery
        {
            Login = loginRequest.Login,
            Password = loginRequest.Password
        });

        if (user == null)
        {
            return NotFound("Пользователь не найден");
        }

        var nowUtc = DateTime.UtcNow;
        var expirationDuration = TimeSpan.FromMinutes(60);
        var expirationUtc = nowUtc.Add(expirationDuration);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, _jwtOptions.Subject),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(nowUtc).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Exp, EpochTime.GetIntDate(expirationUtc).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
            new(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
            new("UserId", user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expirationUtc,
            signingCredentials: signIn);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new TokenDto
        {
            Token = tokenString
        });
    }
}
