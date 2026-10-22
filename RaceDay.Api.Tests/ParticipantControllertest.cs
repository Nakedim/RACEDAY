//using System.Security.Claims;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using RaceDay.Api.Controllers;
//using RaceDay.Api.Data;
//using RaceDay.Api.Models;
//using RaceDay.Contracts;
//using Xunit;

//namespace RaceDay.Api.Tests.Controllers
//{
//    public class ParticipantControllerTests : IDisposable
//    {
//        private readonly RacedayDbContext _context;
//        private readonly ParticipantController _controller;
//        private const string TestUserId = "TestUser_12345";

//        public ParticipantControllerTests()
//        {
//            var options = new DbContextOptionsBuilder<RacedayDbContext>()
//                .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                .Options;

//            _context = new RacedayDbContext(options);
//            _controller = new ParticipantController(_context);

//            // Inject mocked claims token identity context profile down to the engine instance
//            SetupMockUserContext(TestUserId);
//        }

//        private void SetupMockUserContext(string userId)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, userId),
//                new Claim(ClaimTypes.Role, "Participant")
//            };
//            var identity = new ClaimsIdentity(claims, "TestAuth");
//            var claimsPrincipal = new ClaimsPrincipal(identity);

//            _controller.ControllerContext = new ControllerContext
//            {
//                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
//            };
//        }

//        // TEST 1: Get participant by ID
//        [Fact]
//        public async Task GetParticipant_ExistingID_ReturnsParticipant()
//        {
//            // Arrange
//            var participant = new Participant
//            {
//                FirstName = "John",
//                Surname = "Smith",
//                Age = 25,
//                Location = "Durban",
               
//            };

//            _context.Participants.Add(participant);
//            await _context.SaveChangesAsync();

//            // Act
//            var result = await _controller.GetParticipant(participant.ParticipantID);

//            // Assert
//            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
//            var returnedParticipant = Assert.IsType<ParticipantProfileDTO>(actionResult.Value);

//            Assert.Equal("John", returnedParticipant.FirstName);
//            Assert.Equal(participant.ParticipantID, returnedParticipant.ParticipantID);
//        }

//        // TEST 2: Get a participant that does not exist
//        [Fact]
//        public async Task GetParticipant_InvalidID_ReturnsNotFound()
//        {
//            // Act
//            var result = await _controller.GetParticipant(999);

//            // Assert
//            var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
//            Assert.Equal("Participant profile not found.", actionResult.Value);
//        }

//        // TEST 3: Create participant profile
//        [Fact]
//        public async Task CreateParticipant_ValidParticipant_CreatesRecord()
//        {
//            // Arrange
//            var participantDto = new ParticipantProfileDTO
//            {
//                FirstName = "Peter",
//                Surname = "Brown",
//                Age = 22,
//                Location = "Pretoria"
//            };

//            // Act
//            var result = await _controller.CreateParticipant(participantDto);

//            // Assert
//            var actionResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
//            var returnedValue = Assert.IsType<ParticipantProfileDTO>(actionResult.Value);

//            Assert.Equal("Peter", returnedValue.FirstName);
//            Assert.Equal(1, await _context.Participants.CountAsync());

//            var savedRecord = await _context.Participants.FirstAsync();
//            Assert.Equal(TestUserId, savedRecord.ApplicationUserId);
//        }

//        // TEST 4: Create duplicate profile breaks with conflict
//        [Fact]
//        public async Task CreateParticipant_ProfileAlreadyExists_ReturnsConflict()
//        {
//            // Arrange
//            var existingProfile = new Participant
//            {
//                FirstName = "Existing",
//                Surname = "User",
//                Age = 40,
//                Location = "Durban",
                
//            };
//            _context.Participants.Add(existingProfile);
//            await _context.SaveChangesAsync();

//            var newProfileDto = new ParticipantProfileDTO { FirstName = "New" };

//            // Act
//            var result = await _controller.CreateParticipant(newProfileDto);

//            // Assert
//            var actionResult = Assert.IsType<ConflictObjectResult>(result.Result);
//            Assert.Equal("A participant profile already exists.", actionResult.Value);
//        }

//        // TEST 5: Update participant profile
//        [Fact]
//        public async Task UpdateParticipant_ExistingID_UpdatesRecord()
//        {
//            // Arrange
//            var participant = new Participant
//            {
//                FirstName = "John",
//                Surname = "Smith",
//                Age = 25,
//                Location = "Durban",
               
//            };

//            _context.Participants.Add(participant);
//            await _context.SaveChangesAsync();

//            var updatedParticipantDto = new ParticipantProfileDTO
//            {
//                FirstName = "John",
//                Surname = "Smith",
//                Age = 26,
//                Location = "Johannesburg"
//            };

//            // Act
//            var result = await _controller.UpdateParticipantProfile(updatedParticipantDto);

//            // Assert
//            Assert.IsType<NoContentResult>(result);

//            var savedParticipant = await _context.Participants.FindAsync(participant.ParticipantID);
//            Assert.Equal(26, savedParticipant!.Age);
//            Assert.Equal("Johannesburg", savedParticipant.Location);
//        }

//        // TEST 6: Delete participant profile
//        [Fact]
//        public async Task DeleteParticipant_ExistingID_DeletesRecord()
//        {
//            // Arrange
//            var participant = new Participant
//            {
//                FirstName = "Mary",
//                Surname = "Jones",
//                Age = 30,
//                Location = "Cape Town",
  
//            };

//            _context.Participants.Add(participant);
//            await _context.SaveChangesAsync();

//            // Act
//            var result = await _controller.DeleteParticipantProfile();

//            // Assert
//            Assert.IsType<NoContentResult>(result);
//            Assert.Equal(0, await _context.Participants.CountAsync());
//        }

//        // TEST 7: Delete profile when not created yet returns 404
//        [Fact]
//        public async Task DeleteParticipant_NoProfileExists_ReturnsNotFound()
//        {
//            // Act
//            var result = await _controller.DeleteParticipantProfile();

//            // Assert
//            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
//            Assert.Equal("Profile not found.", actionResult.Value);
//        }

//        public void Dispose()
//        {
//            _context.Database.EnsureDeleted();
//            _context.Dispose();
//        }
//    }
//}
