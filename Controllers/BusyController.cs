using busylight_server.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace busylight_server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BusyController : ControllerBase
    {
        private IHubContext<BusyHub> HubContext { get; set; }
        public BusyController(IHubContext<BusyHub> hubcontext) => HubContext = hubcontext;


        [HttpPost("{color}")]
        public async Task Color(string color, string group)
        {
            await HubContext.Clients.Group(group).SendAsync("SetColor", color);
        }

        [HttpPost("{group}")]
        public async Task Ring(string group, string color, BusylightSoundClip tune)
        {
            await HubContext.Clients.Group(group).SendAsync("Ring", color, tune);
        }

    }

    public enum BusylightSoundClip
    {
        OpenOffice = 1,
        Quiet = 2,
        Funky = 3,
        FairyTale = 4,
        KuandoTrain = 5,
        TelephoneNordic = 6,
        TelephoneOriginal = 7,
        TelephonePickMeUp = 8,
        IM1 = 9,
        IM2 = 10
    }
}