using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using pyttogpanne_api.Models;
using Xunit;

namespace pyttogpanne_api.Tests;

/// <summary>
/// The app from Program.cs answers an unreadable JSON body with 400, after the 401 for a caller who is
/// not signed in, and refuses email stats for more days than Brevo reports on. It runs in Development,
/// where ASP.NET Core throws for a body that is not valid JSON.
/// </summary>
public class BadRequestsTests : IClassFixture<BadRequestsTests.ApiFactory>
{
    private const string AdminEmail = "admin@pyttogpanne.invalid";
    private const string AdminPassword = "Password1!";
    private const string ValidSettings = """
        {"contactRecipientEmail":"post@example.com","companyName":"Pytt og panne","streetAddress":"Gata 1","publicEmail":"post@example.com"}
        """;

    private readonly ApiFactory _factory;

    public BadRequestsTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task SignedOut_InvalidUtf8_Answers401()
    {
        var client = _factory.CreateClient();

        var response = await client.SendAsync(PutSettings(InvalidUtf8()), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Admin_InvalidUtf8_Answers400()
    {
        var client = await SignedInAsync();

        var response = await client.SendAsync(PutSettings(InvalidUtf8()), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Admin_MalformedJson_Answers400()
    {
        var client = await SignedInAsync();

        var response = await client.SendAsync(PutSettings(Encoding.UTF8.GetBytes("{\"companyName\":")), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Admin_ValidBody_IsSaved()
    {
        var client = await SignedInAsync();

        var response = await client.SendAsync(PutSettings(Encoding.UTF8.GetBytes(ValidSettings)), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"companyName\":\"Pytt og panne\"", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Admin_EmailStatsForMoreThan90Days_Answers400()
    {
        var client = await SignedInAsync();

        var response = await client.GetAsync("/api/admin/email-stats?days=91", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Must be at most 90.", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>The valid settings with a lone continuation byte in the company name.</summary>
    private static byte[] InvalidUtf8()
    {
        var bytes = Encoding.UTF8.GetBytes(ValidSettings.Replace("Pytt og panne", "Pytt ? panne"));
        bytes[Array.IndexOf(bytes, (byte)'?')] = 0x80;
        return bytes;
    }

    private static HttpRequestMessage PutSettings(byte[] body) => new(HttpMethod.Put, "/api/settings")
    {
        Content = new ByteArrayContent(body) { Headers = { ContentType = new MediaTypeHeaderValue("application/json") } }
    };

    private async Task<HttpClient> SignedInAsync()
    {
        var client = _factory.CreateClient();
        var login = await client.PostAsJsonAsync("/api/auth/login", new { email = AdminEmail, password = AdminPassword }, TestContext.Current.CancellationToken);
        login.EnsureSuccessStatusCode();
        var token = (await login.Content.ReadFromJsonAsync<TokenResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private sealed record TokenResponse(string Token);

    /// <summary>Program.cs on an in-memory database, with the first admin seeded no background services and keys kept in memory.</summary>
    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _database = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:AutoMigrate"] = "false",
                ["Jwt:Key"] = "a-test-signing-key-that-is-long-enough-0123456789",
                ["Jwt:Issuer"] = "pyttogpanne-tests",
                ["Seed:AdminEmail"] = AdminEmail,
                ["Seed:AdminPassword"] = AdminPassword,
                ["BrevoSettings:ApiKey"] = "unused",
                ["BrevoSettings:SenderEmail"] = "post@pyttogpanne.invalid",
                ["BrevoSettings:SenderName"] = "Pytt og panne",
            }));
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IHostedService>();
                services.AddSingleton<IDataProtectionProvider, EphemeralDataProtectionProvider>();
                services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(o => o
                    .UseInMemoryDatabase(_database)
                    .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            });
        }
    }
}
