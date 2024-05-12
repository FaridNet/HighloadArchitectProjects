using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Initialiser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<ApplicationDbContext>((sp, options) =>
    {
      Console.WriteLine(connectionString);

      options.UseNpgsql(connectionString);
      options.EnableSensitiveDataLogging();
    });

    services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

    services.AddScoped<ApplicationDbContextInitialiser>();

    return services;
  }
}
