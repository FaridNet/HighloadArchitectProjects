namespace Application.Features.User.Queries.SearchUser;

public class UserDto
{
    public long Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime Birthdate { get; set; }

    public string? Biography { get; set; }

    public string City { get; set; } = string.Empty;
}
