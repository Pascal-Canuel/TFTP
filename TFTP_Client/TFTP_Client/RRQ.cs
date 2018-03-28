using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TFTP_Client
{
    public class RRQ : TFTP
    {
      
        public RRQ() :base()
        {

        }

        public override void Thread()
        {           
            FileStream fs;
            int nOctetsLus = 0, nTimeOuts = 0, nErreurACK = 0;

            try
            {
                RRQrequest();
                m_socket.SendTo(m_tamponEnvoi, 2 + m_strFichier.Length + 1 + 8 + 1, SocketFlags.None, m_PointDistant);

                Send();

                if (File.Exists(FullPath))
                {
                    Output.Text("File already exist");
                    SendError(CodeErreur.FileExisting);
                }
                else
                {
                    fs = File.Open(FullPath, FileMode.CreateNew, FileAccess.Write, FileShare.Write);

                    do
                    {
                        do
                        {
                            if (m_lire = !m_socket.Poll(5000000, SelectMode.SelectRead))
                                nTimeOuts++; 
                            else
                            {
                                nOctetsLus = m_socket.ReceiveFrom(m_tamponReception, ref m_PointDistant);
                                if (Receive(m_tamponReception))
                                {
                                    m_lire = false;
                                    nErreurACK++;
                                    //TODO réenvoyer trame
                                }
                                else
                                {
                                    m_lire = true;

                                    fs.Write(m_tamponReception, 4, nOctetsLus - 4);

                                    
                                    m_socket.SendTo(m_tamponEnvoi, 4, SocketFlags.None, m_PointDistant);
                                    Send();
                                }
                            }
                        }
                        while (m_lire == false && nTimeOuts < 10 && nErreurACK < 3);
                    }
                    while (nOctetsLus == 516 && nTimeOuts < 10 && nErreurACK < 3);
                    Output.Text($"  RRQ closed from IP: {((IPEndPoint)PointDistant).Address} Port: {((IPEndPoint)PointDistant).Port} ---> {m_strFichier}");
                    fs.Close();
                    m_socket.Close();
                }
            }
            catch (SocketException se)
            {
                Output.Text(se.Message);
            }
        }

        public bool Receive(byte[] bTrame)
        {
            //si fichier non trouvé, le supprimer
            return (bTrame[0] != 0x00 || bTrame[1] != 0x03 || bTrame[2] != m_tamponEnvoi[2] || bTrame[3] != m_tamponEnvoi[3]);
        }

        public void Send()
        {
            m_noBloc++;
            m_tamponEnvoi[0] = (byte)((ushort)CodeOP.ACK >> 8);
            m_tamponEnvoi[1] = (byte)((ushort)CodeOP.ACK & 0xFF);
            m_tamponEnvoi[2] = (byte)(m_noBloc >> 8);
            m_tamponEnvoi[3] = (byte)(m_noBloc & 0xFF);          
        }

        public void RRQrequest()
        {
            m_tamponEnvoi[0] = (byte)((ushort)CodeOP.RRQ >> 8);
            m_tamponEnvoi[1] = (byte)((ushort)CodeOP.RRQ & 0xFF);
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(m_strFichier + '\0' + "netascii" + '\0'), 0, m_tamponEnvoi, 2, m_strFichier.Length + 1 + 8 + 1); 
        }

        public override string FullPath { get { return m_directory + "\\" + m_nomFichier; } }
    }
}
