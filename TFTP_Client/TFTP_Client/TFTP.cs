using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using static TFTP_Client.CodeErreur;

namespace TFTP_Client
{
    public abstract class TFTP
    {
        public TFTP()
        {
            m_pointLocal = new IPEndPoint(0, 0);

            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_socket.Bind(m_pointLocal);
            m_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);

            CreateDirectory(); 

            m_tamponEnvoi = new byte[516];
            m_tamponReception = new byte[516];

        }

        protected EndPoint m_PointDistant;
        protected EndPoint m_pointLocal;

        protected string m_strFichier;
        protected string m_nomFichier;
        protected string m_directory;

        protected bool m_lire;
        protected Socket m_socket;

        protected ushort m_noBloc;

        protected byte[] m_tamponEnvoi;
        protected byte[] m_tamponReception;

        public EndPoint PointDistant { get { return m_PointDistant; } set { m_PointDistant = value; } }
        public string StrFichier { get { return m_strFichier; } set { m_strFichier = value; } }
        public string NomFichier { get { return m_nomFichier; } set { m_nomFichier = value; } }

        public abstract string FullPath { get; }
        public abstract void Thread();

        public void SendError(CodeErreur ce)
        {
            byte[] tamponErreur = new byte[516];
            tamponErreur[0] = (byte)((ushort)CodeOP.ERROR >> 8);
            tamponErreur[1] = (byte)((ushort)CodeOP.ERROR & 0xFF);
            tamponErreur[2] = (byte)((ushort)ce >> 8);
            tamponErreur[3] = (byte)((ushort)ce & 0xFF);
            tamponErreur[4] = 0x00;

            m_socket.SendTo(tamponErreur, m_PointDistant);
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
