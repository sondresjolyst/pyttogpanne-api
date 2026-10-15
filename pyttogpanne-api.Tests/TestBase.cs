using Microsoft.EntityFrameworkCore;
using pyttogpanne_api.Models;

namespace pyttogpanne_api.Tests;

/// <summary>The helpers the tests share: an in-memory database and a sample user.</summary>
public abstract class TestBase
{
    protected static ApplicationDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options);

    protected static User MakeUser(string id = "user-1", string email = "test@example.com") => new()
    {
        Id = id,
        Email = email,
        UserName = "testuser",
        FirstName = "Test",
        LastName = "User",
        EmailConfirmed = true
    };
}
