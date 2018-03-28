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
    public class WRQ : TFTP
    {
        public WRQ() : base()
        {
        }
        public override void Thread()
        {
            FileStream fs;
            int nOctetsLus = 0, nTimeOuts = 0, nErreurACK = 0;
            
            try
            {
                WRQrequest();
                m_socket.SendTo(m_tamponEnvoi, 2 + m_nomFichier.Length + 1 + 8 + 1, SocketFlags.None, m_PointDistant);

                if (!File.Exists(FullPath))
                {
                    Output.Text("File not found");
                    SendError(CodeErreur.FileNotFound);
                }
                else
                {
                    //WRQ

                    fs = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                    Send();
                    do
                    {
                        nOctetsLus = fs.Read(m_tamponEnvoi, 4, 512);
                       
                        do
                        {
                            if (m_lire = !m_socket.Poll(5000000, SelectMode.SelectRead))
                                nTimeOuts++;
                            else
                            {
                                m_socket.ReceiveFrom(m_tamponReception, ref m_PointDistant);
                                if (!Receive(m_tamponReception))
                                {
                                    m_lire = false;
                                    nErreurACK++;
                       
                                }
                                else
                                {
                                    m_lire = true;
                                    
                                    m_socket.SendTo(m_tamponEnvoi, nOctetsLus + 4, SocketFlags.None, m_PointDistant);
                                    Send();

                                    if(nOctetsLus < 512)
                                    {
                                        if (m_lire = !m_socket.Poll(5000000, SelectMode.SelectRead))
                                            nTimeOuts++;
                                        else
                                        {
                                            m_socket.ReceiveFrom(m_tamponReception, ref m_PointDistant);
                                            if (!Receive(m_tamponReception))
                                            {
                                                m_lire = false;
                                                nErreurACK++;
                                            }
                                            else
                                                m_lire = true;
                                        }
                                    }
                                }
                                    
                            }
                        }
                        while (m_lire == false && nTimeOuts < 10 && nErreurACK < 3);
                    }
                    while (nOctetsLus == 512 && nTimeOuts < 10 && nErreurACK < 3);
                    Output.Text($"  WRQ closed from IP: {((IPEndPoint)PointDistant).Address} Port: {((IPEndPoint)PointDistant).Port} ---> {m_strFichier}");
                    fs.Close();
                    m_socket.Close();
                }

            }
            catch (SocketException se)
            {
                Output.Text(se.Message);
            }
        }
        
        public void WRQrequest()
        {
            m_tamponEnvoi[0] = (byte)((ushort)CodeOP.WRQ >> 8);
            m_tamponEnvoi[1] = (byte)((ushort)CodeOP.WRQ & 0xFF);
            Array.Copy(System.Text.Encoding.ASCII.GetBytes(m_nomFichier + '\0' + "netascii" + '\0'), 0, m_tamponEnvoi, 2, m_strFichier.Length + 1 + 8 + 1); //à vérifier et 0 en chiffre ou nulle
        }

        public bool Receive(byte[] bTrame)
        {
            bool ret = false;
            //si on à recu des données
            m_noBloc--;

            int high = (byte)(m_noBloc >> 8);
            int low = (byte)(m_noBloc & 0xFF);
            if (bTrame[0] == 0x00 && bTrame[1] == 0x04 && bTrame[2] == high && bTrame[3] == low)
                ret = true;
            m_noBloc++;

            return ret;
        }

        public void Send()
        {
            m_noBloc++;
            m_tamponEnvoi[0] = (byte)((ushort)CodeOP.DATA >> 8);
            m_tamponEnvoi[1] = (byte)((ushort)CodeOP.DATA & 0xFF);
            m_tamponEnvoi[2] = (byte)(m_noBloc >> 8);
            m_tamponEnvoi[3] = (byte)(m_noBloc & 0xFF);
        }

        public override string FullPath { get { return m_directory + "\\" + m_strFichier; } }
    }
}
