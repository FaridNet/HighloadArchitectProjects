using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.User.Command.RegisterUser;

public class RegisterUserCommand : IRequest<long>
{
    /// <summary>
    /// Логин
    /// </summary>
    public required string Login { get; init; }

    /// <summary>
    /// Имя
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public required string SecondName { get; init; }

    /// <summary>
    /// День рождения
    /// </summary>
    public DateTime BirthDate { get; init; }

    /// <summary>
    /// Интересы
    /// </summary>
    public string? Biography { get; init; }

    /// <summary>
    /// Город
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Пароль
    /// </summary>
    public required string Password { get; init; }
}

internal class RegisterUserCommandHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<RegisterUserCommand, long>
{
    private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;

    public async Task<long> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.User newUser = new()
        {
            Login = request.Login,
            FirstName = request.FirstName,
            SecondName = request.SecondName,
            BirthDate = request.BirthDate.ToUniversalTime(),
            Biography = request.Biography,
            City = request.City,
            Password = request.Password,
        };

        await _applicationDbContext.Users.AddAsync(newUser, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return newUser.Id;
    }
}