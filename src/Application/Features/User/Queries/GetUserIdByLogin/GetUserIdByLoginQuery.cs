using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Queries.GetUserIdByLogin;

public class GetUserIdByLoginQuery : IRequest<UserDto>
{
    /// <summary>
    /// Логин
    /// </summary>
    public required string Login { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public required string Password { get; set; }
}

internal class GetUserByLoginQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetUserIdByLoginQuery, UserDto>
{
    private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;

    public async Task<UserDto> Handle(GetUserIdByLoginQuery request, CancellationToken cancellationToken)
    {
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken)
            ?? throw new Exception("Пользователь не найден");

        return new UserDto
        {
            Id = user.Id
        };
    }
}
