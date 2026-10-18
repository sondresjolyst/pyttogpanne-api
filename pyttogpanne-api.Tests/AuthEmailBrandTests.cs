using pyttogpanne_api.Models.Admin;
using pyttogpanne_api.Services;
using Xunit;

namespace pyttogpanne_api.Tests;

public class AuthEmailBrandTests : TestBase
{
    [Fact]
    public async Task GetBrandAsync_PassesTheStoredOrgNumberAndVatFlag()
    {
        await using var db = CreateDbContext();
        db.AppSettings.Add(new AppSettings
        {
            CompanyName = "Pyttogpanne",
            CompanyLegalName = "Pyttogpanne AS",
            OrgNumber = "923202374",
            VatRegistered = true,
            PublicEmail = "post@pyttogpanne.no",
        });
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var brand = await new AuthEmailBrand(db).GetBrandAsync(TestContext.Current.CancellationToken);

        Assert.Equal("Pyttogpanne", brand.Name);
        Assert.Equal("Pyttogpanne AS", brand.LegalName);
        Assert.Equal("923202374", brand.OrgNumber);
        Assert.True(brand.VatRegistered);
        Assert.Equal("post@pyttogpanne.no", brand.Email);
    }

    [Fact]
    public async Task GetBrandAsync_WithoutSettings_UsesTheDefaults()
    {
        await using var db = CreateDbContext();

        var brand = await new AuthEmailBrand(db).GetBrandAsync(TestContext.Current.CancellationToken);

        Assert.Equal("Pyttogpanne", brand.Name);
        Assert.Equal("", brand.OrgNumber);
        Assert.False(brand.VatRegistered);
    }
}
