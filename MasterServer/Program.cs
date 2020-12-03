using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace MasterServer
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var httpsv = new HttpServer("http://0.0.0.0:6969");

#if DEBUG
            // To change the logging level.
            //httpsv.Log.Level = LogLevel.Trace;

            // To change the wait time for the response to the WebSocket Ping or Close.
            //httpsv.WaitTime = TimeSpan.FromSeconds (2);

            // Not to remove the inactive WebSocket sessions periodically.
            httpsv.KeepClean = false;
#endif

            SubManager manager = new SubManager(httpsv);
            new Http(httpsv, manager);

            httpsv.Start();
            if (httpsv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", httpsv.Port);
                foreach (var path in httpsv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            Logging.Log("\nPress Enter key to stop the server...");
            Console.ReadLine();

            httpsv.Stop();
        }
    }
}
