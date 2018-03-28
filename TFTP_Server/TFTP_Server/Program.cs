using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace TFTP_Server
{
    class Program
    {   

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("TFTP Server by Pascal Canuel and Justin-Roberge Lavoie");
            //ip address
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    Console.WriteLine($"Current IP: {ip}");
            }
            Console.Write("Please select an option : ");  
            Output.Text("Start | Stop");

            string input;
            bool started = false;
            Thread thread;
           
            Creator cr = new Creator();

            Console.ForegroundColor = ConsoleColor.Yellow; //color for input
            while (true)
            {
                input = Console.ReadLine();
                switch (input)
                {
                    case "Start":
                        {
                            if (!started)
                            {
                                Output.Text("Server started");
                       
                                thread = new Thread(new ThreadStart(cr.Listen));
                                thread.IsBackground = true; //not  best
                                thread.Start();

                                started = true;
                            }
                            else
                                Output.Text("Server already started");
                            
                            break;
                        }
                    case "Stop":
                        {
                            cr.Fin = true;
                            Environment.Exit(0);
                            break;
                        }
                    default:
                        {
                            Output.Text("You have not selected an option");
                            break;
                        }
                }
            }
        }


    }
}
