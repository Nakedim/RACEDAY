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
using RaceDay.Contracts.RaceRequestDTO;
using Xunit;

namespace RaceDay.Tests
{
    public class RacesControllerTests : IDisposable
    {
        private readonly RacedayDbContext _context;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly RacesController _controller;

        public RacesControllerTests()
        {
            // 1. Setup In-Memory Database
            var options = new DbContextOptionsBuilder<RacedayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new RacedayDbContext(options);

            // 2. Setup Mock UserManager
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            // 3. Initialize Controller
            _controller = new RacesController(_context, _mockUserManager.Object);
        }

        // Helper method to simulate an authenticated user context on the controller
        private void SetupUserContext(string userId)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetAllRace_ReturnsListOfRaceResponseDTO()
        {
            // Arrange
            _context.Races.Add(new Races { RaceID = 1, RaceName = "5K Run", Distance = 5, Location = "Park", StartTime = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetAllRace();

            // Assert
            var actionResult = Assert.IsType<ActionResult<List<RaceResponseDTO>>>(result);
            var list = Assert.IsType<List<RaceResponseDTO>>(actionResult.Value);
            Assert.Single(list);
            Assert.Equal("5K Run", list[0].RaceName);
        }

        [Fact]
        public async Task CreateRace_ReturnsCreatedAtAction_WhenUserIsOrganiser()
        {
            // Arrange
            string testUserId = "user-123";
            SetupUserContext(testUserId);

            var organiserUser = new ApplicationUser
            {
                Id = testUserId,
                profileRole = ProfileRole.Organiser
            };

            _mockUserManager.Setup(um => um.FindByIdAsync(testUserId))
                .ReturnsAsync(organiserUser);

            var raceDto = new RaceDTO
            {
                RaceName = "Marathon",
                Distance = 42,
                Location = "City Center",
                StartTime = DateTime.UtcNow,
                EventID = 1
            };

            // Act
            var result = await _controller.CreateRace(raceDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdRace = Assert.IsType<Races>(createdAtActionResult.Value);
            Assert.Equal("Marathon", createdRace.RaceName);
            Assert.Equal(1, _context.Races.Count());
        }

        [Fact]
        public async Task CreateRace_ReturnsForbid_WhenUserIsNotOrganiser()
        {
            // Arrange
            string testUserId = "user-456";
            SetupUserContext(testUserId);

            var participantUser = new ApplicationUser
            {
                Id = testUserId,
                profileRole = ProfileRole.Participant // 🛑 Not an Organiser
            };

            _mockUserManager.Setup(um => um.FindByIdAsync(testUserId))
                .ReturnsAsync(participantUser);

            var raceDto = new RaceDTO { RaceName = "Sprint" };

            // Act
            var result = await _controller.CreateRace(raceDto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task CreateRace_ReturnsUnauthorized_WhenUserClaimIsMissing()
        {
            // Arrange
            // Empty controller context simulation (No ClaimsPrincipal NameIdentifier setup)
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var raceDto = new RaceDTO { RaceName = "Sprint" };

            // Act
            var result = await _controller.CreateRace(raceDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
