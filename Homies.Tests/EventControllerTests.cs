using NUnit.Framework;
using Moq;
using Homies.Controllers;
using Homies.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace Homies.Tests
{
    [TestFixture]
    public class EventControllerTests
    {
        private Mock<IEventService> _mockEventService;
        private EventController _controller;

        [SetUp]
        public void Setup()
        {
            _mockEventService = new Mock<IEventService>();

            var user = new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity("User"), null));
            _controller = new EventController(_mockEventService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        [Test]
        public async Task Add_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Add();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Join_UserNotJoined_ReturnsRedirectToActionResult()
        {
            // Arrange
            int eventId = 1;
            _mockEventService.Setup(s => s.IsUserJoinedEventAsync(eventId, It.IsAny<string>()))
                             .ReturnsAsync(false);
            _mockEventService.Setup(s => s.JoinEventAsync(eventId, It.IsAny<string>()))
                             .ReturnsAsync(true);

            // Act
            var result = await _controller.Join(eventId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Joined", redirectResult.ActionName);
            Assert.AreEqual("Event", redirectResult.ControllerName);
        }

        [Test]
        public async Task Join_UserAlreadyJoined_ReturnsRedirectToActionResult()
        {
            // Arrange
            int eventId = 1;
            _mockEventService.Setup(s => s.IsUserJoinedEventAsync(eventId, It.IsAny<string>()))
                             .ReturnsAsync(true);

            // Act
            var result = await _controller.Join(eventId);

            // Assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Joined", redirectResult.ActionName);
            Assert.AreEqual("Event", redirectResult.ControllerName);
        }
    }
}
