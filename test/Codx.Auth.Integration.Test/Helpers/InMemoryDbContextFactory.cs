using Codx.Auth.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Codx.Auth.Integration.Test.Helpers
{
    /// <summary>
    /// Creates an isolated <see cref="UserDbContext"/> backed by the EF Core in-memory
    /// provider. Each call produces a uniquely-named database so tests do not share state.
    /// Transactions are suppressed (in-memory store ignores them) so cascade service
    /// methods run without exception.
    /// </summary>
    internal static class InMemoryDbContextFactory
    {
        public static UserDbContext Create(string? dbName = null)
        {
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new UserDbContext(options);
        }
    }
}
