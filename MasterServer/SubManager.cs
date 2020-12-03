using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace MasterServer
{
    internal class SubManager
    {
        private List<SubClient> subs = new List<SubClient>();

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
            subs.Add(sub);
        }

        private void Sub_OnDisconnect(string name, SubClient sub)
        {
            subs.Remove(sub);
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