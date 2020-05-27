﻿using busylight_server.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace busylight_server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
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
        public async Task Ring(string group, string color, string tune)
        {
            await HubContext.Clients.Group(group).SendAsync("Ring", color, tune);
        }

    }
}