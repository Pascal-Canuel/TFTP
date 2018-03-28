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
    public abstract class TFTP
    {
        public TFTP()
        {
            //try catchhhhhhh needed
            m_pointLocal = new IPEndPoint(0, 0);

            try
            {
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                m_socket.Bind(m_pointLocal);
                m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            }
            catch (SocketException se)
            {
                Output.Text(se.Message);
            }

            CreateDirectory(); 

            m_tamponEnvoi = new byte[516];
            m_tamponReception = new byte[516];
        }

        protected EndPoint m_PointDistant;
        protected EndPoint m_pointLocal;

        protected string m_strFichier;
        protected string m_directory;
        //protected string m_fullPath;
       
        protected bool m_lire;
        protected Socket m_socket;

        protected byte[] m_tamponEnvoi;
        protected byte[] m_tamponReception;

        protected ushort m_noBloc;

        public EndPoint PointDistant { get { return m_PointDistant; } set { m_PointDistant = value; } }
        public string StrFichier { get { return m_strFichier; } set { m_strFichier = value; } }
        public string FullPath { get { return m_directory + "\\" + m_strFichier; } }

        public abstract void Thread();
        public abstract void Send();
        public abstract bool Receive(byte[] bTrame);

        public void SendError(CodeErreur codeErreur, string MsgErreur)
        {
            byte[] tamponErreur = new byte[516];
            tamponErreur[0] = (byte)((ushort)CodeOP.ERROR >> 8);
            tamponErreur[1] = (byte)((ushort)CodeOP.ERROR & 0xFF);
            tamponErreur[2] = (byte)((ushort)codeErreur >> 8);
            tamponErreur[3] = (byte)((ushort)codeErreur & 0xFF);
            tamponErreur[4] = 0x00;

            Encoding.ASCII.GetBytes(MsgErreur, 0, MsgErreur.Length, tamponErreur, 4);

            tamponErreur[4 + MsgErreur.Length] = 0x00;

            m_socket.SendTo(tamponErreur, 4 + MsgErreur.Length + 1, SocketFlags.None, m_PointDistant);

            m_socket.Close(); 
        }

        public void CreateDirectory()
        {
            m_directory = Directory.GetCurrentDirectory() + "\\TFTP";
            bool exist = Directory.Exists(m_directory);
            if (!exist)
                Directory.CreateDirectory(m_directory);
        }
    }
}
