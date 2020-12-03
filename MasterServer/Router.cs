using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WebSocketSharp.Server;

namespace MasterServer
{
    class Router
    {
        private List<Route> routes = new List<Route>();

        public void Route(object sender, HttpRequestEventArgs evt) {
            Route r = routes.Find(r => r.Method.ToLower() == evt.Request.HttpMethod.ToLower() && r.Path.IsMatch(evt.Request.RawUrl));
            if (r != null)
            {
                r.Call(evt, r.Path.Matches(evt.Request.RawUrl));
            } 
            else
            {
                evt.Response.StatusCode = 404;
                byte[] contents = Encoding.UTF8.GetBytes("Not Found");
                evt.Response.ContentLength64 = contents.LongLength;
                evt.Response.Close(contents, true);
            }
            
        }

        public void AddRoute(Route r)
        {
            routes.Add(r);
        }

        public void AddRoute(string method, Regex path, Action<HttpRequestEventArgs, MatchCollection> call)
        {
            routes.Add(new Route(method, path, call));
        }
    }

    class Route
    {
        public string Method { get; private set; }
        public Regex Path { get; private set; }
        public Action<HttpRequestEventArgs, MatchCollection> Call { get; private set; }

        public Route(string method, Regex path, Action<HttpRequestEventArgs, MatchCollection> call)
        {
            Method = method;
            Path = path;
            Call = call;
        }

        public override string ToString()
        {
            return Method.ToUpper() + " " + Path.ToString();
        }
    }
}
