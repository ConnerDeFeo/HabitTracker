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

public class TestFriendModification
{
    FriendModificationController friendModificationController;

    public TestFriendModification()
    {
        var mockFriendModificationService = new Mock<IFriendModificationService>();

        mockFriendModificationService
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

        mockFriendModificationService
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

        mockFriendModificationService
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

        mockFriendModificationService
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

        mockFriendModificationService
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


        friendModificationController = new FriendModificationController(mockFriendModificationService.Object);
    }

    private void SetValidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKey";
        friendModificationController.ControllerContext.HttpContext = httpContext;
    }

    private void SetInvalidSessionKey()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = "SessionKey=TestSessionKeyInvalid";
        friendModificationController.ControllerContext.HttpContext = httpContext;
    }

    [Fact]
    public async Task TestSendFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendModificationController.SendFriendRequest("test");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestSendFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendModificationController.SendFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendModificationController.SendFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestUnSendFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendModificationController.UnSendFriendRequest("test");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestUnSendFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendModificationController.UnSendFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendModificationController.UnSendFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestAcceptFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendModificationController.AcceptFriendRequest("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestAcceptFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendModificationController.AcceptFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendModificationController.AcceptFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestRejectFriendRequest()
    {
        SetValidSessionKey();
        IActionResult result = await friendModificationController.RejectFriendRequest("test");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task TestRejectFriendRequestFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendModificationController.RejectFriendRequest("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendModificationController.RejectFriendRequest("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task TestRemoveFriend()
    {
        SetValidSessionKey();
        IActionResult result = await friendModificationController.RemoveFriend("test");
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Dictionary<string, string>>(okResult.Value);
    }

    [Fact]
    public async Task TestRemoveFriendFaliure()
    {
        SetInvalidSessionKey();
        IActionResult result = await friendModificationController.RemoveFriend("test");
        Assert.IsType<NotFoundResult>(result);

        SetValidSessionKey();
        result = await friendModificationController.RemoveFriend("Invalidtest");
        Assert.IsType<NotFoundResult>(result);
    }
}