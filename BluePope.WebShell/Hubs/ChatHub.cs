using BluePope.WebShell.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BluePope.WebShell.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public ChatHub()
        {
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }

        public async Task SendMessageToCaller(string message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", message);
        }

        public async Task SendMessageToGroups(string message)
        {
            List<string> groups = new List<string>() { "SignalR Users" };
            await Clients.Groups(groups).SendAsync("ReceiveMessage", message);
        }

        /// <summary>
        /// 인증이 안된 사용자에게 보내는 메시지
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        private async Task SendMessageToIsNotAuthenticated(string sendMessage)
        {
            await Clients.All.SendAsync("NoAuth", sendMessage);
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name}님 입장 알림");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.User.Identity.Name}님 퇴장 알림");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
