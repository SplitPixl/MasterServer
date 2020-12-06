using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace MasterServer
{
    internal class SubManager
    {
        private SubClient[] subs = { };

        public SubManager(HttpServer httpsv)
        {
            httpsv.AddWebSocketService("/events", (SubClient sub) =>
            {
                sub.OnConnect += Sub_OnConnect;
                sub.OnDisconnect += Sub_OnDisconnect;
            });
        }

        private void Sub_OnConnect(string name, SubClient sub)
        {
            List<SubClient> clients = new List<SubClient>(subs);
            clients.Add(sub);
            subs = clients.ToArray();
        }

        private void Sub_OnDisconnect(string name, SubClient sub)
        {
            List<SubClient> clients = new List<SubClient>(subs);
            clients.Remove(sub);
            subs = clients.ToArray();
        }

        public int SendTo(string name, string data)
        {
            int count = 0;
            foreach (SubClient sub in subs)
            {
                if (sub.Name == name) {
                    sub.Transmit(data);
                    count += 1;
                }
            }
            return count;
        }

        internal void Broadcast(string command)
        {
            foreach (SubClient sub in subs)
            {
                sub.Transmit(command);
            }
        }
    }
}