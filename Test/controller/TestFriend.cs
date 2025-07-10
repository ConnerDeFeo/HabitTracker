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

public class TestFriend
{
    FriendController friendController;

    public TestFriend()
    {
        var mockFriendService = new Mock<IFriendService>();

        mockFriendService
        .Setup(hs => hs.GetFriends(It.IsAny<string>()))
        .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                    return Task.FromResult<Dictionary<string, string>?>([]);
                else
                    return Task.FromResult<Dictionary<string, string>?>(null);
            }
        );

        mockFriendService
        .Setup(hs => hs.GetFriendProfile(It.IsAny<string>(), It.IsAny<string>()))
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

        mockFriendService
        .Setup(hs => hs.SendFriendRequest(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, friendUsername) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (friendUsername == "test")
                        return Task.FromResult<bool>(true);
                    return Task.FromResult<bool>(false);
                }
                else
                    return Task.FromResult<bool>(false);
            }
        );

        mockFriendService
        .Setup(hs => hs.UnSendFriendRequest(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, friendUsername) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (friendUsername == "test")
                        return Task.FromResult<bool>(true);
                    return Task.FromResult<bool>(false);
                }
                else
                    return Task.FromResult<bool>(false);
            }
        );

        mockFriendService
        .Setup(hs => hs.AcceptFriendRequest(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, friendUsername) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (friendUsername == "test")
                        return Task.FromResult<Dictionary<string, string>?>([]);
                    return Task.FromResult<Dictionary<string, string>?>(null);
                }
                else
                    return Task.FromResult<Dictionary<string, string>?>(null);
            }
        );

        mockFriendService
        .Setup(hs => hs.RemoveFriend(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, friendUsername) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (friendUsername == "test")
                        return Task.FromResult<Dictionary<string, string>?>([]);
                    return Task.FromResult<Dictionary<string, string>?>(null);
                }
                else
                    return Task.FromResult<Dictionary<string, string>?>(null);
            }
        );

        mockFriendService
        .Setup(hs => hs.RejectFriendRequest(It.IsAny<string>(), It.IsAny<string>()))
        .Returns<string, string>((sessionKey, friendUsername) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                {
                    if (friendUsername == "test")
                        return Task.FromResult<bool>(true);
                    return Task.FromResult<bool>(false);
                }
                else
                    return Task.FromResult<bool>(false);
            }
        );

        mockFriendService
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

        mockFriendService
        .Setup(hs => hs.GetRandomUsers(It.IsAny<string>()))
        .Returns<string>((sessionKey) =>
            {
                if (sessionKey.Equals("TestSessionKey"))
                    return Task.FromResult<Dictionary<string, string>?>([]);
                else
                    return Task.FromResult<Dictionary<string, string>?>(null);
            }
        );

        friendController = new FriendController(mockFriendService.Object);
    }

    private void SetValidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        friendController.ControllerContext.HttpContext = httpContext;
    }

    private void SetInvalidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        friendController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task TestGetFriends()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.GetFriends();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestGetFriendsFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.GetFriends();
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestGetFriendsProfile()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.GetFriendsProfile("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Profile?>(okResult.Value);
    }

    [Fact]
    public async Task TestGetFriendsProfileFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.GetFriendsProfile("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendController.GetFriendsProfile("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestSendFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.SendFriendRequest("test");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestSendFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.SendFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendController.SendFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestUnSendFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.UnSendFriendRequest("test");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestUnSendFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.UnSendFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendController.UnSendFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestAcceptFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.AcceptFriendRequest("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestAcceptFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.AcceptFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendController.AcceptFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestRejectFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.RejectFriendRequest("test");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestRejectFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.RejectFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendController.RejectFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestRemoveFriend()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.RemoveFriend("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestRemoveFriendFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.RemoveFriend("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendController.RemoveFriend("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestFindUser()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.FindUser("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestFindUserFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.FindUser("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendController.FindUser("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }
    
    [Fact]
    public async Task TestGetRandomUsers()
    {
        SetValidSessionKey();
        IActionResult result = await friendController.GetRandomUsers();
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestGetRandomUsersFailure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendController.GetRandomUsers();
        Assert.IsType<NotFoundResult>(result);

    }
}