using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceResultsController : ControllerBase
    {
        private readonly RacedayDbContext _context;

        public RaceResultsController(RacedayDbContext context)
        {
            _context = context;
        }

        //[HttpGet("all")]
        //public async Task<ActionResult<IEnumerable<RaceResultsDTO>>> GetRaceResults()
        //{
        //    return await _context.RaceResults
        //        .Select(r => new RaceResultsDTO
        //        {
        //            ResultsID = r.ResultsID,
        //            ParticipantID = r.ParticipantID,
        //            FinishTime = r.FinishTime,
        //            Position = r.Position
        //        })
        //        .ToListAsync();
        //}

        //organisers can enter results
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult EnterResults([FromBody] RaceResultsDTO resultsDto)
        {
            return Ok();
        }

        //organisers can view RaceResult
        [HttpGet("view")]
        [Authorize(Roles = "Participant, Organiser")]
        public IActionResult ViewRaceResults()
        {
            return Ok();
        }
    }
}
