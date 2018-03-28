using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TFTP_Server
{
    public class WRQ : TFTP
    {
        
        public override bool Receive(byte[] bTrame)
        {
            bool ret = false;
            //si on à recu des données
            m_noBloc++;

            int high = (byte)(m_noBloc >> 8);
            int low = (byte)(m_noBloc & 0xFF);
            if (bTrame[0] == 0x00 && bTrame[1] == 0x03 && bTrame[2] == high && bTrame[3] == low)
                ret = true;
            m_noBloc--;

            return ret; 
        }

        public override void Send()
        {
            //construire ack
            m_noBloc++;
            m_tamponEnvoi[0] = (byte)((ushort)CodeOP.ACK >> 8);
            m_tamponEnvoi[1] = (byte)((ushort)CodeOP.ACK & 0xFF);
            m_tamponEnvoi[2] = (byte)(m_noBloc >> 8);
            m_tamponEnvoi[3] = (byte)(m_noBloc & 0xFF);
             //le premier est le bloc 0

        }

        public override void Thread()
        {
            FileStream fs;
            int nOctetsLus = 0, nTimeOuts = 0, nErreurACK = 0;
            try
            {
                if (File.Exists(FullPath))
                {
                    Output.Text("File already exist");
                    SendError(CodeErreur.FileExisting, "File already exists");
                }                  
                else
                {
                    fs = File.Open(FullPath, FileMode.CreateNew, FileAccess.Write, FileShare.Write);


                    m_tamponEnvoi[0] = (byte)((ushort)CodeOP.ACK >> 8);
                    m_tamponEnvoi[1] = (byte)((ushort)CodeOP.ACK & 0xFF);
                    m_tamponEnvoi[2] = (byte)(m_noBloc >> 8);
                    m_tamponEnvoi[3] = (byte)(m_noBloc & 0xFF);
                    m_socket.SendTo(m_tamponEnvoi, 4, SocketFlags.None, m_PointDistant);

                    do
                    {                       
                        do
                        {                           
                            if (m_lire = !m_socket.Poll(5000000, SelectMode.SelectRead))
                                nTimeOuts++; //TODO réenvoyer trame
                            else
                            {
                                nOctetsLus = m_socket.ReceiveFrom(m_tamponReception, ref m_PointDistant);
                                if (!Receive(m_tamponReception))
                                {
                                    m_lire = false;
                                    nErreurACK++;                        
                                }
                                else
                                {
                                    m_lire = true;

                                    fs.Write(m_tamponReception, 4, nOctetsLus - 4);
                                   
                                    Send();
                                    m_socket.SendTo(m_tamponEnvoi, 4, SocketFlags.None, m_PointDistant);                              
                                }
                            }
                        }
                        while (m_lire == false && nTimeOuts < 10 && nErreurACK < 3);
                    }
                    while (nOctetsLus == 516 && nTimeOuts < 10 && nErreurACK < 3); 
                    Output.Text($"  WRQ closed from IP: {((IPEndPoint)PointDistant).Address} Port: {((IPEndPoint)PointDistant).Port} ---> {m_strFichier}");
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
