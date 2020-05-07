using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System.Xml;

namespace Sinex
{
    class Econnect
    {
        string xmlPurchaseOrdfe = "";
        string connectionString = "";
        eConnectMethods eConnectMethods = new eConnectMethods();

        public void SerializePoObject(string fileName)
        {
           // System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(GetType(eConnectType));
        }
    }
}
