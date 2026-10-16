//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using RaceDay.Api.Data;
//using RaceDay.Api.Models;
//using RaceDay.Contracts;

//namespace RaceDay.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class RaceEntriesController : ControllerBase
//    {
//        private readonly RacedayDbContext _context;

//        public RaceEntriesController(RacedayDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        [Authorize(Roles = "Organiser")]
//        public async Task<ActionResult<IEnumerable<RaceEntriesDTO>>> GetRaceEntries()
//        {
//            return await _context.RaceEntries
//                .Select(re => new RaceEntriesDTO
//                {
//                    RaceEntryID = re.RaceEntryID,
//                    RaceID = re.RaceID,
//                    ParticipantID = re.ParticipantID,
//                    CategoryID = re.CategoryID,
//                    EntryDate = re.EntryDate,
//                    //Category = new CategoriesDTO
//                    //{
//                    //    CategoryID = re.Category.
//                    //    CategoryName = re.Category.CategoryName
//                    //}
//                })
//                .ToListAsync();
//        }

//        [HttpPost("enter")]
//        [Authorize(Roles = "Participant")]
//        public async Task<ActionResult<IEnumerable<RaceEventDTO>>> EnterRace()
//        {
//            return await _context.RaceEvents
//                .Select(re => new RaceEventDTO
//                {
//                    EventID = re.EventID,
//                    Description = re.Description,
//                    Title = re.Title,
//                    location = re.location,
//                    OrganiserID = re.OrganiserID,
//                    CategoryID = re.CategoryID,
//                    EventType = re.EventType
//                })
//                .ToListAsync();
//        }

//        [HttpGet("all")]
//        [Authorize(Roles = "Organiser")]
//        public async Task<ActionResult<IEnumerable<RaceEventDTO>>> ViewAllEntries()
//        {
//            return await _context.RaceEvents
//                .Select(re => new RaceEventDTO
//                {
//                    EventID = re.EventID,
//                    Description = re.Description,
//                    Title = re.Title,
//                    location = re.location,
//                    OrganiserID = re.OrganiserID,
//                    CategoryID = re.CategoryID,
//                    EventType = re.EventType
//                })
//                .ToListAsync();
//        }
//    }
//}
