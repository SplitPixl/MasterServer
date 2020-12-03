using System;
using System.Collections.Generic;
using System.Text;

namespace MasterServer
{
    class Logging
    {
        public static void Log(string s)
        {
            string nowString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine(string.Format("[{0}]{1}", nowString, s));
        }
    }
}
