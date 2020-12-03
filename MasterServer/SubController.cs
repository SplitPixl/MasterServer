using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MasterServer
{
    class SubController
    {
        SubManager manager;

        public SubController(Router router, SubManager manager)
        {
            this.manager = manager;
            router.AddRoute("POST", new Regex(@"^/command"), Command);
        }

        public void Command(HttpRequestEventArgs e, MatchCollection m)
        {
            var req = e.Request;
            var res = e.Response;

            res.Headers.Add("Access-Control-Allow-Origin", "*");

            string command = new StreamReader(req.InputStream).ReadToEnd();
            string antagonist = req.QueryString["user"];
            //string command = req.QueryString["command"];
            string targets = req.QueryString["target"];
            bool random = !req.QueryString["random"].IsNullOrEmpty();

            List<string> foundTargets = new List<string>();

            int total = 0;
            bool targetFound = targets.IsNullOrEmpty();
            string response;

            if (!antagonist.IsNullOrEmpty())
            {
                if (random)
                {
                    //SubClient[] subConnections = GetRandomSub();
                    //foreach (SubClient sub in subConnections)
                    //{
                    //    foundTargets.Add(sub.Name);
                    //    sub.Transmit(command);
                    //    targetFound = true;
                    //}

                }
                else if (targets.IsNullOrEmpty())
                {
                    Logging.Log(string.Format("[Command] {0} -> ({1}) ==> Broadcast", antagonist, command));
                    manager.Broadcast(command);
                }
                else
                {
                    foreach (string target in targets.Split(","))
                    {
                        int connections = manager.SendTo(target, command);
                        if (connections > 0)
                        {
                            foundTargets.Add(string.Format("{0}({1})", target, connections));
                            total += connections;
                        }
                    }

                }
                if (total > 0)
                {
                    response = "sent to ";
                    if (foundTargets.Count > 0)
                    {
                        string t = string.Join(", ", foundTargets);
                        Logging.Log(string.Format("[HTTP] {0} -> [{1}] -> {2}", antagonist, command.Substring(0, Math.Min(command.Length, 10)), t));
                        response += t;
                    }
                }
                else
                {
                    response = "couldn't find target";
                    res.StatusCode = 404;
                }
            }
            else
            {
                response = "you need to authenticate with ?user=you";
                res.StatusCode = 403;
            }

            byte[] contents = Encoding.UTF8.GetBytes(response);
            res.ContentLength64 = contents.LongLength;
            res.Close(contents, true);
        }
    }
}
