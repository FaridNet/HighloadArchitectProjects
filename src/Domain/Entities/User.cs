namespace Domain.Entities;

/// <summary>
/// Пользователь
/// </summary>
public class User
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Логин
    /// </summary>
    public required string Login { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public required string SecondName { get; set; }

    /// <summary>
    /// День рождения
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Интересы
    /// </summary>
    public string? Biography { get; set; }

    /// <summary>
    /// Город
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public required string Password { get; set; }
}
