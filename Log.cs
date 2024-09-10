using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win_Labs
{
    public static class Log
    {
        public static string GetCurrentTime()
        {
            return DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
        }

        public static void log(string message)
        {
            string timestamp = GetCurrentTime();
            Console.WriteLine($"[{timestamp}] Message: {message}");
        }
    }

}
