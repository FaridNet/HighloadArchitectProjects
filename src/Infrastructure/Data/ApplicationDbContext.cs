using Application.Common.Interfaces;
using Domain.Entities;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();

    public async Task BulkInsertAsync<T>(IList<T> entities, CancellationToken cancellationToken = default)
        where T : class
    {
        await ((DbContext)this).BulkInsertAsync(entities, cancellationToken: cancellationToken);
    }

    public async Task BulkSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await ((DbContext)this).BulkSaveChangesAsync(cancellationToken: cancellationToken);
    }

    public async Task BulkUpdateAsync<T>(IList<T> entities, CancellationToken cancellationToken = default)
        where T : class
    {
        await ((DbContext)this).BulkUpdateAsync(entities);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
