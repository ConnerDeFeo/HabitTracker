namespace Test.controller;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Http;
using Server.service.interfaces;
using Server.controller;
using Server.model.user;
using Server.model.habit;
using Server.dtos;
using Moq;

public class TestSocialData
{
    SocialDataController socialDataController;

    public TestSocialData()
    {
        var mockSocialDataService = new Mock<ISocialDataService>();

        mockSocialDataService
        .Setup(hs => hs.GetFriends(It.IsAny<string>()))
        .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                    return Task.FromResult<Dictionary<string, string>?>([]);
                else
                    return Task.FromResult<Dictionary<string, string>?>(null);
            }
        );

        mockSocialDataService
        .Setup(hs => hs.GetProfile(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, friendUsername) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (friendUsername == "test")
                        return Task.FromResult<Profile?>(new());
                    return Task.FromResult<Profile?>(null);
                }
                else
                    return Task.FromResult<Profile?>(null);
            }
        );

        mockSocialDataService
        .Setup(hs => hs.FindUser(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, phrase) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (phrase == "test")
                        return Task.FromResult<Dictionary<string, string>?>([]);
                    return Task.FromResult<Dictionary<string, string>?>(null);
                }
                else
                    return Task.FromResult<Dictionary<string, string>?>(null);
            }
        );

        mockSocialDataService
        .Setup(hs => hs.GetRandomUsers(It.IsAny<string>()))
        .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                    return Task.FromResult<Dictionary<string, string>?>([]);
                else
                    return Task.FromResult<Dictionary<string, string>?>(null);
            }
        );

        socialDataController = new SocialDataController(mockSocialDataService.Object);
    }

    private void SetValidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        socialDataController.ControllerContext.HttpContext = httpContext;
    }

    private void SetInvalidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        socialDataController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task TestGetFriends()
    {
        SetValidSessionKey();
        IActionResult result = await socialDataController.GetFriends();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestGetFriendsFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await socialDataController.GetFriends();
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestGetFriendsProfile()
    {
        SetValidSessionKey();
        IActionResult result = await socialDataController.GetProfile("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Profile?>(okResult.Value);
    }

    [Fact]
    public async Task TestGetFriendsProfileFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await socialDataController.GetProfile("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await socialDataController.GetProfile("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestFindUser()
    {
        SetValidSessionKey();
        IActionResult result = await socialDataController.FindUser("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestFindUserFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await socialDataController.FindUser("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await socialDataController.FindUser("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task TestGetRandomUsers()
    {
        SetValidSessionKey();
        IActionResult result = await socialDataController.GetRandomUsers();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestGetRandomUsersFailure()
    {
        SetInvalidSessionKey();
        IActionResult result = await socialDataController.GetRandomUsers();
        Assert.IsType<NotFoundResult>(result);

    }
}