
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Moq;
//using RaceDay.Api.Controllers;
//using RaceDay.Api.Models;
//using RaceDay.Contracts.Logins;
//using RaceDay.Contracts.RegisterRegquest;
//using System.Security.Claims;
//using Xunit;

//namespace RaceDay.Api.Tests
//{
//    public class AuthUsersControllerTests
//    {
//        private readonly Mock<UserManager<ApplicationUser>> _userManager;
//        private readonly Mock<SignInManager<ApplicationUser>> _signInManager;
//        private readonly Mock<RoleManager<IdentityRole>> _roleManager;
//        private readonly Mock<IConfiguration> _configuration;

//        private readonly AuthUsersController _controller;

//        public AuthUsersControllerTests()
//        {
//            var userStore =
//                new Mock<IUserStore<ApplicationUser>>();

//            _userManager = new Mock<UserManager<ApplicationUser>>(
//                userStore.Object,
//                Mock.Of<IOptions<IdentityOptions>>(),
//                Mock.Of<IPasswordHasher<ApplicationUser>>(),
//                Array.Empty<IUserValidator<ApplicationUser>>(),
//                Array.Empty<IPasswordValidator<ApplicationUser>>(),
//                Mock.Of<ILookupNormalizer>(),
//                Mock.Of<IdentityErrorDescriber>(),
//                Mock.Of<IServiceProvider>(),
//                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

//            _signInManager =
//                new Mock<SignInManager<ApplicationUser>>(
//                    _userManager.Object,
//                    Mock.Of<IHttpContextAccessor>(),
//                    Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
//                    Mock.Of<IOptions<IdentityOptions>>(),
//                    Mock.Of<ILogger<SignInManager<ApplicationUser>>>(),
//                    Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
//                    Mock.Of<IUserConfirmation<ApplicationUser>>());

//            var roleStore = new Mock<IRoleStore<IdentityRole>>();

//            _roleManager = new Mock<RoleManager<IdentityRole>>(
//                roleStore.Object,
//                Array.Empty<IRoleValidator<IdentityRole>>(),
//                Mock.Of<ILookupNormalizer>(),
//                Mock.Of<IdentityErrorDescriber>(),
//                Mock.Of<ILogger<RoleManager<IdentityRole>>>());

//            _configuration = new Mock<IConfiguration>();

//            _controller = new AuthUsersController(
//                _userManager.Object,
//                _signInManager.Object,
//                _roleManager.Object,
//                _configuration.Object);
//        }

//        [Fact]
//        public async Task Login_UserDoesNotExist_ReturnsUnauthorized()
//        {
//            // Arrange
//            var request = new LoginRequest
//            {
//                Email = "unknown@example.com",
//                Password = "Password123!"
//            };

//            _userManager
//                .Setup(x => x.FindByEmailAsync(request.Email))
//                .ReturnsAsync((ApplicationUser)null!);

//            // Act
//            var result = await _controller.Login(request);

//            // Assert
//            Assert.IsType<UnauthorizedObjectResult>(result);
//        }

//        [Fact]
//        public async Task Login_ValidUserWithoutRoles_ReturnsForbidden()
//        {
//            // Arrange
//            var request = new LoginRequest
//            {
//                Email = "participant@example.com",
//                Password = "Password123!"
//            };

//            var user = new ApplicationUser
//            {
//                Id = "user-123",
//                Email = request.Email,
//                UserName = request.Email
//            };

//            _userManager
//                .Setup(x => x.FindByEmailAsync(request.Email))
//                .ReturnsAsync(user);

//            _signInManager
//                .Setup(x => x.CheckPasswordSignInAsync(
//                    user, request.Password, false))
//                .ReturnsAsync(SignInResult.Success);

//            _userManager
//                .Setup(x => x.GetRolesAsync(user))
//                .ReturnsAsync(new List<string>());

//            // Act
//            var result = await _controller.Login(request);

//            // Assert
//            var status = Assert.IsType<ObjectResult>(result);
//            Assert.Equal(403, status.StatusCode);
//        }

//        [Fact]
//    internal class RegisterRequestDTO

//        public async Task Register_InvalidRole_ReturnsBadRequest([FromBody] )
//        {
//            // Arrange
//            var request = new RegisterRequest
//            {
//                Email = "test@example.com",
//                Password = "Password123!",
//                FirstName = "Test",
//                Surname = "User",
//                ProfileRole = "InvalidRole"
//            };

//            // Act
//            var result = await _controller.Register(request);

//            // Assert
//            Assert.IsType<BadRequestObjectResult>(result);

//            _userManager.Verify(
//                x => x.CreateAsync(
//                    It.IsAny<ApplicationUser>(),
//                    It.IsAny<string>()),
//                Times.Never);
//        }

//        [Fact]
//        public void Logout_ClearsSession_ReturnsOk()
//        {
//            // Arrange
//            var session = new Mock<ISession>();

//            var httpContext = new DefaultHttpContext
//            {
//                Session = session.Object
//            };

//            _controller.ControllerContext =
//                new ControllerContext
//                {
//                    HttpContext = httpContext
//                };

//            // Act
//            var result = _controller.Logout();

//            // Assert
//            Assert.IsType<OkObjectResult>(result);
//            session.Verify(x => x.Clear(), Times.Once);
//        }
//    }
//}
