using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lounge55Networking.Logging
{
    public class Logger
    {
        public static void LogInfo(string Info, string LogSource = "Lounge55Networking")
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            if(LogSource != "Lounge55Networking")
            {
                Console.WriteLine($"[Lounge55Networking : {LogSource}] " + Info);
            }
            else
            {
                Console.WriteLine($"[{LogSource}] " + Info);
            }
            Console.ForegroundColor = prevColor;
        }

        public static void LogWarning(string Warning, string LogSource = "Lounge55Networking")
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (LogSource != "Lounge55Networking")
            {
                Console.WriteLine($"[Lounge55Networking : {LogSource}] Warning: " + Warning);
            }
            else
            {
                Console.WriteLine($"[{LogSource}] Warning: " + Warning);
            }
            Console.ForegroundColor = prevColor;
        }

        public static void LogError(string Error, string LogSource = "Lounge55Networking")
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (LogSource != "Lounge55Networking")
            {
                Console.WriteLine($"[Lounge55Networking : {LogSource}] " + Error);
            }
            else
            {
                Console.WriteLine($"[{LogSource}] Error: " + Error);
            }
            Console.ForegroundColor = prevColor;
        }
        public static void LogMessage(string Info, string LogSource = "Lounge55Networking")
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            if (LogSource != "Lounge55Networking")
            {
                Console.WriteLine($"[Lounge55Networking : {LogSource}] " + Info);
            }
            else
            {
                Console.WriteLine($"[{LogSource}] " + Info);
            }
            Console.ForegroundColor = prevColor;
        }
    }
}
