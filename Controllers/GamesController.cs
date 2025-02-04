using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnackShackAPI.Models.DTOs;
using SnackShackAPI.Services;

namespace SnackShackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly GamesService _gamesService;
        public GamesController(GamesService gamesService) 
        {
            this._gamesService = gamesService;
        }

        [HttpPut("RollSlotMachine")]
        public async Task<IActionResult> RollSlotMachine(SlotMachineRollRequest request)
        {
            var result = await this._gamesService.RollSlotMachine(request);
            return Ok(result);
        }
    }
}
