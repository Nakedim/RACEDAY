using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RaceDay.Api.Controllers;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using Xunit;

namespace RaceDay.Tests.Controllers
{
    public class CategoriesControllerTests : IDisposable
    {
        private readonly RacedayDbContext _context;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            // 1. Set up an isolated In-Memory Database for EF Core
            var options = new DbContextOptionsBuilder<RacedayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new RacedayDbContext(options);

            // 2. Set up the Identity Mock UserManager
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // 3. Instantiate the Controller
            _controller = new CategoriesController(_context, _mockUserManager.Object);
        }

        // Helper to mimic a logged-in user with claims on the Controller context
        private void SetupUserClaims(string userId, string claimType = "sub")
        {
            var claims = new List<Claim> { new Claim(claimType, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Fact]
        public async Task GetCategory_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            // Act
            var result = await _controller.GetCategory(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCategory_ReturnsOk_WithCorrectCategory()
        {
            // Arrange
            var testCategory = new Categories { CategoryID = 1, CategoryName = "Elite Men" };
            _context.Categories.Add(testCategory);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetCategory(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategory = Assert.IsType<Categories>(okResult.Value);
            Assert.Equal("Elite Men", returnedCategory.CategoryName);
        }

        [Fact]
        public async Task CreateCategory_ReturnsUnauthorized_WhenUserClaimsAreMissing()
        {
            // Arrange
            SetupUserClaims(string.Empty); // Empty ID to simulate failed claim extraction
            var dto = new CategoriesDTO { CategoryName = "Masters" };

            // Act
            var result = await _controller.CreateCategory(dto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task CreateCategory_ReturnsForbid_WhenUserIsNotAnOrganiser()
        {
            // Arrange
            string userId = "user-123";
            SetupUserClaims(userId, ClaimTypes.NameIdentifier); // Tests standard claim type fallback

            var nonOrganiserUser = new ApplicationUser { Id = userId, profileRole = ProfileRole.Participant };
            _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(nonOrganiserUser);

            var dto = new CategoriesDTO { CategoryName = "Masters" };

            // Act
            var result = await _controller.CreateCategory(dto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CreateCategory_ReturnsCreated_AndSavesToDatabase_WhenUserIsOrganiser()
        {
            // Arrange
            string userId = "organiser-456";
            SetupUserClaims(userId, "sub"); // Tests "sub" claim extraction path

            var organiserUser = new ApplicationUser { Id = userId, profileRole = ProfileRole.Organiser };
            _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(organiserUser);

            var dto = new CategoriesDTO { CategoryName = "Junior Girls" };

            // Act
            var result = await _controller.CreateCategory(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var savedCategory = Assert.IsType<Categories>(createdResult.Value);
            Assert.Equal("Junior Girls", savedCategory.CategoryName);

            // Verify database tracking state
            var dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "Junior Girls");
            Assert.NotNull(dbCategory);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsBadRequest_WhenCategoryDoesNotExist()
        {
            // Act
            var result = await _controller.DeleteCategory(999);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Category not found", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteCategory_ReturnsOk_AndRemovesFromDatabase_WhenSuccessful()
        {
            // Arrange
            var testCategory = new Categories { CategoryID = 10, CategoryName = "To Be Deleted" };
            _context.Categories.Add(testCategory);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeleteCategory(10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Category deleted successfully", okResult.Value);

            var dbCategory = await _context.Categories.FindAsync(10);
            Assert.Null(dbCategory); // Confirm removal
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
