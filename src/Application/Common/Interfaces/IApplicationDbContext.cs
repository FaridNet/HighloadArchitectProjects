using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task BulkInsertAsync<T>(IList<T> entities, CancellationToken cancellationToken = default) where T : class;

    Task BulkUpdateAsync<T>(IList<T> entities, CancellationToken cancellationToken = default) where T : class;

    Task BulkSaveChangesAsync(CancellationToken cancellationToken = default);
}
