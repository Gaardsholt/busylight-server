using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Prometheus;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace busylight_server.Hubs
{
    [Authorize]
    public class BusyHub : Hub
    {
        private static readonly Gauge UsersLoggedIn = Metrics.CreateGauge("logged_in_users", "Number of active users");

        private static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            UsersLoggedIn.Inc();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            UsersLoggedIn.Dec();

            await LeaveGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinGroup(string groupName)
        {
            await LeaveGroup();

            _connections[Context.ConnectionId] = groupName;

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup()
        {
            if (_connections.TryRemove(Context.ConnectionId, out string oldGroupName))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldGroupName);

        }

        public async Task SendToSelf(string color)
        {
            await Clients.Caller.SendAsync("SetColor", color);
        }

    }

}
