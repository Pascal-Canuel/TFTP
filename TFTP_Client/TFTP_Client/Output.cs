using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTP_Client
{
    class Output
    {
        public static void Text(string msg)
        {
            ConsoleColor current = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.WriteLine(msg);

            Console.ForegroundColor = current;
        }
    }
}
