using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace TFTP_Server
{
    public class Creator
    {
        private bool m_fin;

        public void Listen()
        {
            EndPoint PointLocal = new IPEndPoint(0, 69);
            EndPoint PointDistant = new IPEndPoint(0, 0);
         
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Bind(PointLocal);

                byte[] bTamponReception = new byte[516];

                while (!m_fin)
                {
                    if(socket.Available > 0)
                    {
                        socket.ReceiveFrom(bTamponReception, ref PointDistant);

                        switch (ValiderTrame(bTamponReception))
                        {
                            case 1:
                                //créer thread rrq
                                Output.Text($"Request RRQ received from IP: {((IPEndPoint)PointDistant).Address} Port: {((IPEndPoint)PointDistant).Port} ---> {FindFile(bTamponReception)}");

                                TFTP rrq = new RRQ
                                {
                                    PointDistant = PointDistant,
                                    StrFichier = FindFile(bTamponReception)
                                };

                                Thread threadRRQ = new Thread(new ThreadStart(rrq.Thread));
                                threadRRQ.Start();
                                break;
                            case 2:
                                //créer thread wrq   
                                Output.Text($"Request WRQ received from IP: {((IPEndPoint)PointDistant).Address} Port: {((IPEndPoint)PointDistant).Port} ---> {FindFile(bTamponReception)}");

                                TFTP wrq = new WRQ
                                {
                                    PointDistant = PointDistant,
                                    StrFichier = FindFile(bTamponReception)
                                };

                                Thread threadWRQ = new Thread(new ThreadStart(wrq.Thread));
                                threadWRQ.Start();
                                break;
                            case -1:
                                Output.Text($"Request not identified received from IP: {((IPEndPoint)PointDistant).Address} Port: {((IPEndPoint)PointDistant).Port}");

                                break;
                        }
                    }
                }
                socket.Close();
            }
            catch(SocketException se)
            {
                Output.Text(se.Message);
            }
        }

        private int ValiderTrame(byte[] bTrame)
        {
            if (bTrame[0] == 0 && bTrame[1] == 1)
            {
                return 1;
            }
            if (bTrame[0] == 0 && bTrame[1] == 2)
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }

        private string FindFile(byte[] btrame)
        {
            string filepath = "";
            bool match = false;
            Match m;
 
            for (int i = 2; i < btrame.Length && match == false; i++)
            {
                filepath += (char)btrame[i];
                m = Regex.Match(filepath, @"^.*\.(jpg|JPG|gif|GIF|doc|DOC|pdf|PDF|txt|png|mp4)$");
                match = m.Success;
            }
            return filepath;
        }

        public bool Fin { set { m_fin = value; } }
    }
}
