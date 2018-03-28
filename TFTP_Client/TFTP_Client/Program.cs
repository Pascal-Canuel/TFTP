using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TFTP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("TFTP Client by Pascal Canuel and Justin-Roberge Lavoie");
            Console.Write("Please select an option : ");
            Output.Text("tftp [<Host>] [{get | put}] <Source> [<Destination>]");

            string input;
            string[] cmd;
            Creator cr = new Creator();

            Console.ForegroundColor = ConsoleColor.Yellow; //input color
            while (true)
            {
                input = Console.ReadLine();
                cmd = input.Split().Where(x => x != string.Empty).ToArray();
           
                if ((cmd.Length == 4 || cmd.Length == 5) && cmd[0].ToUpper() == "TFTP") //beaucoup de validation a effectuer dans un certain ordre. Remplacer par RULE PATTERN
                {
                    Match resultIP = Regex.Match(cmd[1], @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
                    Match resultFile = Regex.Match(cmd[3], @"^.*\.(jpg|JPG|gif|GIF|doc|DOC|pdf|PDF|txt|png|mp4)$"); //à optimiser

                    if (resultIP.Success && resultFile.Success)
                    {
                        if (cmd[2].ToUpper() == "GET")
                        {
                            if (cmd.Length == 4)
                                cr.Request<RRQ>(cmd[1], cmd[3]);
                            else
                            {
                                Match resultFileDest = Regex.Match(cmd[4], @"^.*\.(jpg|JPG|gif|GIF|doc|DOC|pdf|PDF|txt|png|mp4)$");
                                if (resultFileDest.Success)
                                    cr.Request<RRQ>(cmd[1], cmd[3], cmd[4]);
                                else
                                    Output.Text("Destination incorrect");
                            }
                                
                        }
                        else if (cmd[2].ToUpper() == "PUT")
                        {
                            if (cmd.Length == 4)
                                cr.Request<WRQ>(cmd[1], cmd[3]);
                            else
                            {
                                Match resultFileDest = Regex.Match(cmd[4], @"^.*\.(jpg|JPG|gif|GIF|doc|DOC|pdf|PDF|txt|png|mp4)$"); 
                                if (resultFileDest.Success)
                                    cr.Request<WRQ>(cmd[1], cmd[3], cmd[4]);
                                else
                                    Output.Text("Destination incorrect");
                            }                               
                        }
                        else
                        {
                            Output.Text("Commande invalide");
                        }
                    }                  
                }
                else
                    Output.Text("Format invalide");
            }
        }
    }
}
