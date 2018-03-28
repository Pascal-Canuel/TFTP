using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TFTP_Server
{
    public class RRQ : TFTP
    {
        public RRQ() : base()
        {

        }

        public override bool Receive(byte[] bTrame)
        {
            //vérifier si la trame n'est pas un ack
            return (bTrame[0] != 0x00 || bTrame[1] != 0x04 || bTrame[2] != m_tamponEnvoi[2] || bTrame[3] != m_tamponEnvoi[3]);
        }

        public override void Send()
        {
            m_noBloc++;    

            m_tamponEnvoi[0] = (byte)((ushort)CodeOP.DATA >> 8);
            m_tamponEnvoi[1] = (byte)((ushort)CodeOP.DATA & 0xFF);
            m_tamponEnvoi[2] = (byte)(m_noBloc >> 8);
            m_tamponEnvoi[3] = (byte)(m_noBloc & 0xFF);
        }

        public override void Thread()
        {
            FileStream fs;
            int nOctetsLus = 0, nTimeOuts = 0, nErreurACK = 0;//some should go into TFTP class

            try
            {
                if (!File.Exists(FullPath))
                {
                    Output.Text("File not found");
                    SendError(CodeErreur.FileNotFound, "File not found");
                }
                else
                {            

                    fs = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    do
                    {
                        nOctetsLus = fs.Read(m_tamponEnvoi, 4, 512);
                        Send();
                        do
                        {
                            m_socket.SendTo(m_tamponEnvoi, nOctetsLus + 4, SocketFlags.None, m_PointDistant);
                            if (m_lire = !m_socket.Poll(5000000, SelectMode.SelectRead))
                                nTimeOuts++;
                            else
                            {
                                m_socket.ReceiveFrom(m_tamponReception, ref m_PointDistant);
                                if (Receive(m_tamponReception))
                                {
                                    m_lire = false;
                                    nErreurACK++;
                                }
                                else
                                    m_lire = true;
                            }
                        }
                        while (m_lire == false && nTimeOuts < 10 && nErreurACK < 3);
                    }
                    while (nOctetsLus == 512 && nTimeOuts < 10 && nErreurACK < 3);
                    Output.Text($"  RRQ closed from IP: {((IPEndPoint)PointDistant).Address} Port: {((IPEndPoint)PointDistant).Port} ---> {m_strFichier}");
                    fs.Close();
                    m_socket.Close();
                }
            
            }
            catch(SocketException se)
            {
                Output.Text(se.Message);
            }
        }
    }
}
