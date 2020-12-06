using Newtonsoft.Json;
using System;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MasterServer
{
    public class SubClient : WebSocketBehavior
    {
        public string Name { get; private set; } = "anon";
        public string Device { get; private set; } = "unknown";

        private bool authenticated = false;

        public void Transmit(string data)
        {
            //Logging.Log(string.Format("[TX] Sending {0} to {1}", data, this.GetName()));
            if (State == WebSocketState.Open)
            {
                Send(data);
            } else if (State == WebSocketState.Closing || State == WebSocketState.Closed)
            {
                OnDisconnect(Name, this);
            }
        }

        public delegate void Connect(string Name, SubClient sub);
        public event Connect OnConnect;

        public delegate void Disconnect(string Name, SubClient sub);
        public event Disconnect OnDisconnect;

        protected override void OnOpen()
        {
            base.OnOpen();
            BasicAuth auth = new BasicAuth(Context.Headers.Get("Authorization"));
            //if (!auth.Valid)
            //{
            //    Context.WebSocket.Close(4401, "Please authenticate with HTTP Basic Auth.");
            //    return;
            //}
            //Name = auth.Name;
            //Device = auth.Pass;
            //OnConnect(Name, this);
            //Logging.Log(string.Format("[WS: Connect] {0}-{1}", Name, Device));
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            //Logging.Log(string.Format("[WS: Message] {0}: {1}", Name, e.Data));
            if (!authenticated)
            {
                Auth(e.Data);
            }
            if (e.Data.StartsWith("echo:"))
            {
                Send(e.Data.Substring(5));
            }
        }

        private void Auth(string data)
        {
            try
            {
                WsAuth auth = JsonConvert.DeserializeObject<WsAuth>(data);
                if (auth.op == "auth")
                {
                    Name = auth.name;
                    Device = auth.device;
                    OnConnect(Name, this);
                    Logging.Log(string.Format("[WS: Connect] {0}-{1}", Name, Device));
                }
                else
                {
                    Context.WebSocket.Close(4401, "Please authenticate.");
                }
            }
            catch (Exception e)
            {
                Context.WebSocket.Close(4401, "Please authenticate.");
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            if (authenticated)
            {
                Logging.Log(string.Format("[WS: Disconnect] {0}-{1}: {2}", Name, Device, e.Code));
                OnDisconnect(Name, this);
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            {
                Logging.Log(string.Format("[WS: Disconnect] {0}-{1}: {2}", Name, Device, -1));
                OnDisconnect(Name, this);
            }
        }

        private class BasicAuth
        {
            public bool Valid { get; private set; } = true;
            public string Name { get; private set; }
            public string Pass{ get; private set; }

            public BasicAuth(string auth)
            {
                if (auth.IsNullOrEmpty() || !auth.StartsWith("Basic "))
                {
                    Valid = false;
                    return;
                } else
                {
                    byte[] data = Convert.FromBase64String(auth.Substring(6));
                    string decoded = Encoding.UTF8.GetString(data);
                    try
                    {
                        Name = decoded.Split(":")[0];
                        Pass = decoded.Split(":")[1];
                        Valid = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Valid = false;
                    }
                }
            }
        }

        private class WsAuth
        {
            public string op;
            public string name;
            public string device;
        }
    }
}
