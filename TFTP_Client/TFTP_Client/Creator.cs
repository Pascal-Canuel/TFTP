using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace TFTP_Client
{
    public class Creator
    {
        public void Request<T>(string ipDistant, string nomFichier, string nomFichierDest = null) where T : TFTP, new()
        {
            TFTP rq = new T
            {
                StrFichier = nomFichier,
                PointDistant = new IPEndPoint(IPAddress.Parse(ipDistant), 69)
            };

            if (nomFichierDest == null)
                rq.NomFichier = nomFichier;
            else
                rq.NomFichier = nomFichierDest;

            Thread threadRq = new Thread(new ThreadStart(rq.Thread));
            threadRq.Start();

            Output.Text($"{typeof(T).Name} at IP: {ipDistant} Port: 69");
        }
     
    }
}
