using SnackShackAPI.Models;
using SnackShackAPI.Models.DTOs;

namespace SnackShackAPI.Services
{
    public class GamesService : IGamesService
    {
        public GamesService() { }


        public async Task<SlotMachineResult> RollSlotMachine(SlotMachineRollRequest request)
        {
            SlotMachineResult result = null;


            return result;
        }

    }
    public interface IGamesService
    {
        Task<SlotMachineResult> RollSlotMachine(SlotMachineRollRequest request);
    }
}
