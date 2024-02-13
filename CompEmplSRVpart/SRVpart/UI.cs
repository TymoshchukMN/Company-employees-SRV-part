using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRVpart
{
    internal class UI
    {
        public static void PrintColorfull(
            string str,
            ConsoleColor color = ConsoleColor.Cyan)
        {
            ConsoleColor def = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(str);
            Console.ForegroundColor = def;
        }
    }
}
