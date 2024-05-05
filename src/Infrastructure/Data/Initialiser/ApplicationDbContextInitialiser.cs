using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Reflection;

namespace Infrastructure.Data.Initialiser;

public class ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (!_context.Users.Any())
        {
            _context.Users.Add(new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                Login = "Admin",
                Password = "Admin",
                City = "Москва",
                BirthDate = new DateTime(1990, 1, 1).ToUniversalTime(),
                Biography = "Admin"
            });

            await _context.SaveChangesAsync();

            var currentDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);
            var filePath = Path.Combine(currentDir, "./Data/Initialiser/people.v2.csv");

            using StreamReader sr = File.OpenText(filePath);
            string? strRow = string.Empty;
            const int batch = 1000;
            int rowCount = 0;
            List<User> users = [];

            while ((strRow = sr.ReadLine()) != null)
            {
                string[] splitedRow = strRow.Split(",");

                // ФИО
                string[] fio = splitedRow[0].Split(" ");
                string firstName = fio[1];
                string lastName = fio[0];

                // Дата рождения                
                if (!DateTime.TryParseExact(splitedRow[1], "yyyy-dd-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
                {
                    continue;
                }

                // Город
                string city = splitedRow[2];

                string login = Translit($"{lastName}_{firstName}");

                rowCount++;
                users.Add(new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = DateTime.SpecifyKind(birthDate, DateTimeKind.Utc),
                    City = city,
                    Login = login,
                    Password = login
                });
 
                if (rowCount % batch == 0)
                {
                    await _context.BulkInsertAsync(users);
                    await _context.BulkSaveChangesAsync();

                    rowCount = 0;
                    users = [];
                }                
            }

            if (users.Count != 0)
            {
                await _context.BulkInsertAsync(users);
                await _context.BulkSaveChangesAsync();
            }
        }
    }

    private static string Translit(string str)
    {
        string[] lat_up = { "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh", "Shch", "\"", "Y", "'", "E", "Yu", "Ya" };
        string[] lat_low = { "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh", "shch", "\"", "y", "'", "e", "yu", "ya" };
        string[] rus_up = { "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я" };
        string[] rus_low = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ", "ъ", "ы", "ь", "э", "ю", "я" };
        for (int i = 0; i <= 32; i++)
        {
            str = str.Replace(rus_up[i], lat_up[i]);
            str = str.Replace(rus_low[i], lat_low[i]);
        }
        return str;
    }
}
