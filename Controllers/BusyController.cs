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
        public async Task Ring(string group)
        {
            await HubContext.Clients.Group(group).SendAsync("Ring");
        }

        [HttpPost("{group}")]
        public async Task Police(string group)
        {
            await HubContext.Clients.Group(group).SendAsync("Police");
        }

    }
}