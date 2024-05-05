using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.User.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public required long UserId { get; init; }
}

internal class GetUserByIdQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var userEntity = _applicationDbContext.Users.FirstOrDefault(x => x.Id == request.UserId)
            ?? throw new Exception("Пользователь не найден.");

        return new UserDto
        {
            Id = userEntity.Id,
            FirstName = userEntity.FirstName,
            SecondName = userEntity.LastName,
            Birthdate = userEntity.BirthDate,
            Biography = userEntity.Biography,
            City = userEntity.City
        };
    }
}