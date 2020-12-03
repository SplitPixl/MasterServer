using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Text.RegularExpressions;

namespace MasterServer
{
    class Http
    {
        HttpServer httpsv;
        SubManager manager;
        SubController sc;
        Router router = new Router();
        public Http(HttpServer httpsv, SubManager manager)
        {
            this.httpsv = httpsv;
            this.manager = manager;
            httpsv.OnGet += router.Route;
            httpsv.OnPost += router.Route;
            sc = new SubController(router, manager);
        }
    }
}
