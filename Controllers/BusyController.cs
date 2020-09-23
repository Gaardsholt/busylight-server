using busylight_server.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Prometheus;
using System.Threading.Tasks;

namespace busylight_server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BusyController : ControllerBase
    {
        private static readonly Counter _metricsColor = Metrics.CreateCounter("color_requests_total", "Number of requests to change color.");
        private static readonly Counter _metricsRing = Metrics.CreateCounter("ring_requests_total", "Number of requests to do a ring.");
        private static readonly Counter _metricsPolice = Metrics.CreateCounter("police_requests_total", "Number of requests to call the Police.");

        private IHubContext<BusyHub> HubContext { get; set; }
        public BusyController(IHubContext<BusyHub> hubcontext) => HubContext = hubcontext;


        [HttpPost("{color}")]
        public async Task Color(string color, string group)
        {
            _metricsColor.Inc();
            await HubContext.Clients.Group(group).SendAsync("SetColor", color);
        }

        [HttpPost("{group}")]
        public async Task Ring(string group)
        {
            _metricsRing.Inc();
            await HubContext.Clients.Group(group).SendAsync("Ring");
        }

        [HttpPost("{group}")]
        public async Task Police(string group)
        {
            _metricsPolice.Inc();
            await HubContext.Clients.Group(group).SendAsync("Police");
        }

    }
}