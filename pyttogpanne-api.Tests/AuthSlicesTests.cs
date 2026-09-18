using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Moq;
using pyttogpanne_api.Features.Auth;
using pyttogpanne_api.Infrastructure;
using pyttogpanne_api.Models;
using Xunit;

namespace pyttogpanne_api.Tests;

public class AuthSlicesTests : TestBase
{
    [Fact]
    public async Task Login_UnknownEmail_ReturnsUnauthorized()
    {
        await using var db = CreateDbContext();
        MockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var result = await Login.Handle(new LoginModel { Email = "x@y.no", Password = "nope" },
            MockUserManager.Object, MockSignInManager.Object, Configuration, db);

        var json = Assert.IsType<JsonHttpResult<MessageResponse>>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, json.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        await using var db = CreateDbContext();
        var user = MakeUser();
        MockUserManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        MockSignInManager.Setup(m => m.PasswordSignInAsync(user.UserName!, It.IsAny<string>(), false, true))
            .ReturnsAsync(SignInResult.Success);
        MockUserManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });

        var result = await Login.Handle(new LoginModel { Email = "a@b.no", Password = "Password1" },
            MockUserManager.Object, MockSignInManager.Object, Configuration, db);

        Assert.IsType<Ok<TokenResponse>>(result);
        Assert.Single(db.RefreshTokens);
    }
}
