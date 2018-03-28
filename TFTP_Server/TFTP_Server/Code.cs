using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFTP_Server
{
    public enum CodeOP : ushort
    {
        RRQ = 0x01,
        WRQ = 0x02,
        DATA = 0x03,
        ACK = 0x04,
        ERROR = 0x05
    }

    public enum CodeErreur : ushort
    {
        [AttributCode("Non défini, voir le message d'erreur (si présent)")]
        NotDefine = 0x00,
        [AttributCode("Fichier non trouvé")]
        FileNotFound = 0x01,
        [AttributCode("Violation de l'accès")]
        AccessViolation = 0x02,
        [AttributCode("Disque plein ou dépassement de l'espacement alloué")]
        DiskFull = 0x03,
        [AttributCode("Opération TFTP illégale")]
        IllegalOperation = 0x04,
        [AttributCode("Transfert ID inconnu")]
        IdUnknown = 0x05,
        [AttributCode("Le fichier existe déjà")]
        FileExisting = 0x06,
        [AttributCode("Utilisateur inconnu")]
        UserUnknown = 0x07,
    }

    public class AttributCode : System.Attribute
    {

        private string _value;

        public AttributCode(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }
}
