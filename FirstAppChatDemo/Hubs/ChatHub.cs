using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;


namespace FirstAppChatDemo.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> UsersOnline = new Dictionary<string, string>();
        private static Dictionary<string, DateTime> LastSeen = new Dictionary<string, DateTime>();

        public override System.Threading.Tasks.Task OnConnected()
        {
            if (UsersOnline.Keys.Contains(Context.ConnectionId) == false)
            {
                var uName = Context.QueryString["clientName"];
                if (uName == null) 
                {
                    uName = Context.ConnectionId;
                }
                else
                {
                    uName = uName.Trim();
                }
               
                if (LastSeen.ContainsKey(Context.ConnectionId))
                {
                    LastSeen[Context.ConnectionId] = DateTime.UtcNow;
                }
                else
                {
                    LastSeen.Add(Context.ConnectionId, DateTime.UtcNow);
                }

                if (UsersOnline.ContainsKey(Context.ConnectionId))
                {
                    UsersOnline[Context.ConnectionId] = uName;
                }
                else
                {
                    UsersOnline.Add(Context.ConnectionId, uName);
                }

                Clients.Others.broadcastJoin(uName);
            }
            
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnReconnected()
        {

            if (LastSeen.ContainsKey(Context.ConnectionId))
            {
                LastSeen[Context.ConnectionId] = DateTime.UtcNow;
            }
            else
            {
                LastSeen.Add(Context.ConnectionId, DateTime.UtcNow);
            }
            
            return base.OnReconnected();
        }
        
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            Clients.Others.broadcastLeave(UsersOnline[Context.ConnectionId]);

            if (UsersOnline.ContainsKey(Context.ConnectionId))
            {
                UsersOnline.Remove(Context.ConnectionId);
            }
            if (LastSeen.ContainsKey(Context.ConnectionId))
            {
                LastSeen.Remove(Context.ConnectionId);
            }

            return base.OnDisconnected(stopCalled);
        }
        
        public void Send(string message)
        {
            if (UsersOnline.Keys.Contains(Context.ConnectionId)) {

                Clients.All.broadcastMessage(UsersOnline[Context.ConnectionId], message);

            } else {

                Clients.All.broadcastMessage("???", message);

            }
        }
    }
}