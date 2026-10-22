using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RaceDay.Api.Controllers;
using RaceDay.Api.Models;
using Xunit;

namespace RaceDay.Api.Tests
{
    public class ProfileControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ProfileController _controller;
        private readonly Mock<ISession> _mockSession;

        public ProfileControllerTests()
        {
            // 1. UserManager requires a complex constructor, so we mock its UserStore first
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            // 2. Mock ISession to simulate HttpContext Session state
            _mockSession = new Mock<ISession>();

            // 3. Create a fake HttpContext and assign our mocked session to it
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _mockSession.Object;

            // 4. Instantiate controller and attach the fake HttpContext context
            _controller = new ProfileController(_mockUserManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        [Fact]
        public async Task GetProfile_ReturnsUnauthorized_WhenSessionUserIdIsEmpty()
        {
            // Arrange: Setup session to return null/empty for "UserId"
            byte[]? value = null;
            _mockSession.Setup(s => s.TryGetValue("UserId", out value)).Returns(false);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.NotNull(unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetProfile_ReturnsNotFound_WhenUserDoesNotExistInDatabase()
        {
            // Arrange: Setup session to successfully return a fake user ID string
            var userId = "user-123";
            var userIdBytes = Encoding.UTF8.GetBytes(userId);
            _mockSession.Setup(s => s.TryGetValue("UserId", out @userIdBytes)).Returns(true);

            // Simulate UserManager failing to find this user account in DB
            _mockUserManager.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetProfile_ReturnsOkWithData_WhenUserIsValidAndLoggedIn()
        {
            // Arrange
            var userId = "user-789";
            var userIdBytes = Encoding.UTF8.GetBytes(userId);
            _mockSession.Setup(s => s.TryGetValue("UserId", out @userIdBytes)).Returns(true);

            var expectedUser = new ApplicationUser
            {
                Id = userId,
                Email = "test@raceday.com",
                FirstName = "John",
                Surname = "Doe",
                profileRole = ProfileRole.Participant
            };

            // Simulate UserManager finding the user cleanly
            _mockUserManager.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            // Verifies properties on the anonymous return object implicitly using reflection
            var jsonProperties = okResult.Value.GetType().GetProperties();
            var emailValue = jsonProperties.FirstOrDefault(p => p.Name == "Email")?.GetValue(okResult.Value);

            Assert.Equal("test@raceday.com", emailValue);
        }
    }
}
