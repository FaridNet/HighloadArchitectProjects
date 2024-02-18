using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Queries.SearchUser;

public class SearchUserQuery : IRequest<UserDto[]>
{
    public string? SearchByFirstName { get; init; }

    public string? SearchByLastName { get; init; }
}

internal class SearchUserQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<SearchUserQuery, UserDto[]>
{
    private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;

    public async Task<UserDto[]> Handle(SearchUserQuery request, CancellationToken cancellationToken)
    {
        var query = _applicationDbContext.Users.AsNoTracking();

        if (string.IsNullOrEmpty(request.SearchByFirstName) && string.IsNullOrEmpty(request.SearchByLastName))
        {
            return [];
        }

        if (!string.IsNullOrEmpty(request.SearchByFirstName))
        {
            query = query.Where(x => x.FirstName.Contains(request.SearchByFirstName));
        }

        if (!string.IsNullOrEmpty(request.SearchByLastName))
        {
            query = query.Where(x => x.LastName.Contains(request.SearchByLastName));
        }

        var result = await query
            .Select(x => new UserDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Biography = x.Biography,
                Birthdate = x.BirthDate,
                City = x.City,
            })
            .OrderBy(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return result;
    }
}
