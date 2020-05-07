using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using static Sinex.DynamicsGp;
using static Sinex.AppTools.AppConnection;
using System.Xml.Linq;

namespace Sinex
{
    public class Http
    {
        
        public class Connection
        {
            string id;
            string type;
            string url;
            string api;
            string options;

            public Connection(string inUrl)
            {
                url = inUrl;
            }
            public string Get()
            {
                try
                {
                    string rt = "";
                    WebRequest request = WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);

                    rt = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                    return rt;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            public string Test()
            {
                Log log = new Tools.Log("C:\\Temp\\SinexLog.txt");
                SinexPoLine poLine = new SinexPoLine();
                string result = "";
                string html = "";
                //string url = @"http://integration.sinexmarine.com/webservices/integrationservice.asmx/getVendors?companyid=bf8385ef-b76d-4f5e-9b0c-6b2dac723e6f";
                string url = @"http://integration.sinexmarine.com/webservices/integrationservice.asmx/poLineItemExport?companyid=bf8385ef-b76d-4f5e-9b0c-6b2dac723e6f&status=Approved";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                log.Write(result);
                log.Write("-");
                log.Write("-");
                log.Write("-");
                log.Write("-");
                log.Write("-");

                string record = result;
                //MessageBox.Show(record);
                int startRecord = result.IndexOf("{") - 1;
                int endRecord = result.LastIndexOf("]") - 1;
                int endIndex = endRecord - startRecord + 1;
                record = result.Substring(startRecord + 1, endIndex);
                log.Write(record);
                MessageBox.Show(record);
                poLine = JsonConvert.DeserializeObject<SinexPoLine>(record);
                return "";
            }
            
        }
        public class HttpFile
        {
            public static string GetFile(string fileName)
            {
                Log log = new Tools.Log("C:\\Temp\\SinexLog.txt");
                SinexPoLine poLine = new SinexPoLine();
                string result = "";
                string html = "";
                //string url = @"http://integration.sinexmarine.com/webservices/integrationservice.asmx/getVendors?companyid=bf8385ef-b76d-4f5e-9b0c-6b2dac723e6f";
                string url = @"http://integration.sinexmarine.com/webservices/integrationservice.asmx/poLineItemExport?companyid=bf8385ef-b76d-4f5e-9b0c-6b2dac723e6f&status=Approved";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
                return result;
            }
            public static void CreateFile(string fileName)
            {
                string file = GetFile(fileName);
                File.AppendAllText(fileName, file);
                ProcessFile(file);
            }
            public static void ProcessFile(string file)
            {
                int fileStart = file.IndexOf("[");
                int fileEnd = file.LastIndexOf("]");
                string newFile = file.SubFromTo(fileStart, fileEnd);
                File.AppendAllText(@"C:\Temp\Sinex\SinexFile.txt", newFile);
            }
            public static string GetPos(string file)
            {
                int fileStart = file.IndexOf("[");
                int fileEnd = file.LastIndexOf("]");
                string newFile = file.SubFromTo(fileStart, fileEnd);
                return newFile;
            }
            private enum CurrentStructure
            {
                start,
                poLoop,
                lineLoop,
            }
            public static void TraverseFile(string file)
            {
                List<PurchaseOrderHeader> listPurchaseOrders = new List<PurchaseOrderHeader>();
                PurchaseOrderHeader poHeader;
                PmTransaction pmTransaction;
                Log log = new Log(@"C:\Sinex\TraverseLog.txt");
                log.Clear();
                TextFile.WriteTextFile(@"C:\Sinex\File.txt", file);
                CurrentStructure currentStructure = CurrentStructure.start;
                int currentIndex = 0;
                int fileLength = file.Length;
                Bracket nextBracket = new Bracket();
                nextBracket = NextBracket(file, currentIndex);
                bool continueLoop = true;
                int recordStart = 0;
                int recordEnd = 0;
                string record = "";
                SinexPo po = new SinexPo();
                SinexPoLine poLine = new SinexPoLine();
                string recordString;
                Log xmlLog = new Tools.Log(@"C:\Sinex\recordXml.txt");
                xmlLog.Clear();
                while (continueLoop)
                {
                    nextBracket = NextBracket(file, currentIndex);
                   // log.Write("Next => " + nextBracket.ToString());
                    if (nextBracket.Index != -1)
                    {
                        #region switch
                        switch (nextBracket.BracketType)
                        {
                            case "[":
                                switch (currentStructure)
                                {
                                    case CurrentStructure.start:
                                        currentStructure = CurrentStructure.poLoop;
                                        break;
                                    case CurrentStructure.poLoop:
                                        currentStructure = CurrentStructure.lineLoop;
                                        break;
                                    case CurrentStructure.lineLoop:
                                        MessageBox.Show($"1 - Bad Config " + currentStructure + ", " + currentStructure.ToString());
                                        break;
                                    default:
                                        MessageBox.Show("Bad Config");
                                        break;
                                }
                                break;
                            case "]":
                                switch (currentStructure)
                                {
                                    case CurrentStructure.start:
                                        MessageBox.Show($"2 - Bad Config " + currentStructure + ", " + currentStructure.ToString());
                                        break;
                                    case CurrentStructure.poLoop:
                                        currentStructure = CurrentStructure.start;
                                        break;
                                    case CurrentStructure.lineLoop:
                                        currentStructure = CurrentStructure.poLoop;
                                        break;
                                    default:
                                        MessageBox.Show($"3 - Bad Config " + currentStructure + ", " + currentStructure.ToString());
                                        break;
                                }
                                break;
                            case "{":
                                switch (currentStructure)
                                {
                                    case CurrentStructure.poLoop:
                                        recordStart = nextBracket.Index;
                                        recordEnd = file.IndexOf("lineItems", recordStart) - 2;
                                        record = file.SubFromTo(recordStart, recordEnd) + "}";
                                        JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                                        {
                                            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
                                        };
                                        //log.Write(record);
                                        po = JsonConvert.DeserializeObject<SinexPo>(record, microsoftDateFormatSettings);
                                        XNode node = JsonConvert.DeserializeXNode(record, "Root");
                                        //xmlLog.Write(node.ToString());
                                        recordString = po.ToString();
                                        log.Write(" ");
                                        log.Write(" ");
                                        log.Write("HEADER => " + recordString);
                                        log.Write($" - {po.requiredBy}, {po.poCreatedBy}, {po.shipTo}");
                                        pmTransaction = new PmTransaction(po, GetGPConnection());
                                        //pmTransaction.Insert("TWO");
                                        string xml = pmTransaction.ToXML();
                                        xmlLog.Write("----------------------------------------------");
                                        xmlLog.Write(xml);
                                        xml = xml.RemoveNode("<connection>");
                                        xmlLog.Write("----------------------------------------------");
                                        xmlLog.Write(xml);
                                        xml = xml.RemoveNode("PmTransaction", true);
                                        xmlLog.Write("----------------------------------------------");
                                        xmlLog.Write(xml);
                                        xml = xml.RemoveNode("?xml", true);
                                        xmlLog.Write("----------------------------------------------");
                                        xmlLog.Write(xml);
                                        xml = xml.Replace("</PmTransaction>", "");
                                        xmlLog.Write("----------------------------------------------");
                                        xmlLog.Write(xml);
                                        string header = "<eConnect xmlns:dt=\"urn:schemas - microsoft - com:datatypes\">";
                                        header = header + "<RMCustomerMasterType>";
                                        header = header + "<eConnectProcessInfo>";
                                        header = header + "</eConnectProcessInfo>";
                                        header = header + "<taPMTransactionInsert>";
                                        string footer ="</taPMTransactionInsert>";
                                        footer = footer + "</RMCustomerMasterType>";
                                        footer = footer + "</eConnect>";
                                        xml = header + xml + footer;
                                        xmlLog.Write("----------------------------------------------");
                                        xmlLog.Write(xml);
                                        pmTransaction.InsertXml(xml);
                                        break;

                                    case CurrentStructure.lineLoop:
                                        recordStart = nextBracket.Index;
                                        recordEnd = file.IndexOf("}", recordStart) - 1;
                                        record = file.SubFromTo(recordStart, recordEnd) + "}";

                                        poLine = JsonConvert.DeserializeObject<SinexPoLine>(record);
                                        recordString = poLine.ToString();
                                        log.Write("LINE => " + recordString);
                                        po.AddLine(poLine);
                                        break;
                                }
                                break;
                                po.ToLog(log);
                        }
                        #endregion Switch
                        currentIndex = nextBracket.Index + 1;
                        if (currentIndex >= fileLength)
                        {
                            continueLoop = false;
                        }
                    }
                    else
                    {
                        continueLoop = false;
                    }
                }
            }
        }
        private static Bracket NextBracket(string file, int currentIndex)
        {
            Bracket bracket = new Bracket();
            int nextCurlyOpen = file.IndexOf("{", currentIndex);
            int nextCurlyClose = file.IndexOf("}", currentIndex);
            int nextSquareOpen = file.IndexOf("[", currentIndex);
            int nextSquareClose = file.IndexOf("]", currentIndex);

            int minIndex = nextCurlyOpen;
            string minBracket = "{";

            if(nextCurlyClose < minIndex)
            {
                minIndex = nextCurlyClose;
                minBracket = "}";
            }

            if (nextSquareOpen < minIndex)
            {
                minIndex = nextSquareOpen;
                minBracket = "[";
            }

            if (nextSquareClose < minIndex)
            {
                minIndex = nextSquareClose;
                minBracket = "]";
            }
            bracket.BracketType = minBracket;
            bracket.Index = minIndex;
            return bracket;
        }
        public class Bracket
        {
            string bracketType;
            int index;
            public Bracket()
            {

            }

            public string BracketType
            {
                get
                {
                    return bracketType;
                }
                set
                {
                    bracketType = value;
                }
            }
            public int Index
            {
                get
                {
                    return index;
                }
                set
                {
                    index = value;
                }
            }
            public void Show()
            {
                MessageBox.Show($"Bracket => {bracketType} : {index.ToString()}");
            }
            public string ToString()
            {
                return $"{bracketType} : {index.ToString()}";
            }
        }
        // lineStatus => Approved, Installed, Ordered
        // poStatus => Open 
        public class SinexPo
        {
            #region Variables
            string id;
            string number;
            public string requiredBy { get; set; }
            public string poCreatedBy { get; set; }
            public string shipTo { get; set; }
            public string terms { get; set; }
            public string poStatus { get; set; }
            public string shippingMethod { get; set; }
            public string localtax { get; set; }
            public string shippingCost { get; set; }
            public string orderAddress { get; set; }
            public string invoiceNum { get; set; }
            public string invoiceDate { get; set; }
            public string voucherNum { get; set; }
            public string vendorname { get; set; }
            public string clientVendorId { get; set; }
            public string username { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string poTotalParts { get; set; }
            public string poTotal { get; set; }
            List<SinexPoLine> lines;

            #endregion Variables

            #region Properties
            public string poId
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }
            public string poNumber
            {
                get
                {
                    return number;
                }
                set
                {
                    number = value;
                }
            }
            #endregion Properties

            #region Constructors
            public SinexPo()
            {
                poId = "";
                poNumber = "";
                CommonConstructor();
            }
            public SinexPo(string inId, string inNumber)
            {
                poId = inId;
                poNumber = inNumber;
                CommonConstructor();
            }
            public void CommonConstructor()
            {
                lines = new List<SinexPoLine>();
            }
            #endregion Constructors

            #region Methods
            public void AddLine(SinexPoLine line)
            {
                lines.Add(line);
            }
                public void ProcessFile()
            {

            }
            public string ToString()
            {
                return $"H: {id} - {number}";
            }
            public void ToLog(Log log)
            {
                log.Write ($"HEADER: {id} - {number}");
                foreach(SinexPoLine line in lines)
                {
                    log.Write($"{line.lineNumber}, {line.lineStatus}");
                }
            }
            #endregion Methods

        }
        public class SinexPoLine
        {
            #region Variables
            public string poid { get; set; }
            public string lineId { get; set; }
            public string lineNumber { get; set; }
            public string parts { get; set; }
            public string toplevelasset { get; set; }
            public string toplevelassetclass { get; set; }
            public string assetname { get; set; }
            public string assetposition { get; set; }
            public string taskName { get; set; }
            public string taskDescription { get; set; }
            public string priceea { get; set; }
            public string qty { get; set; }
            public string partnumber { get; set; }
            public string name { get; set; }
            public string budgetCode { get; set; }
            public string capitalExpense { get; set; }
            public string warranty { get; set; }
            public string routine { get; set; }
            public string calendarperday { get; set; }
            public string limit { get; set; }
            public string limittypename { get; set; }
            public string averageperday { get; set; }
            public string limitdays { get; set; }
            public string lineTotal { get; set; }
            public string lineStatus { get; set; }
            public string lineInvoice { get; set; }
            public string lineVoucher { get; set; }
            public string workorderid { get; set; }
            public string workordernumber { get; set; }
            public string workorderType { get; set; }
            #endregion Variables

            #region Properties

            #endregion Properties

            #region Constructors
            public SinexPoLine()
            {
                poid = "";
                lineId = "";
                lineNumber = "";
            }
            public SinexPoLine(string inPoId, string inLineNumber)
            {
                poid = inPoId;
                lineNumber = inLineNumber;
            }
            #endregion Constructors

            #region Methods
            public string ToString()
            {
                return $"L: {lineId} - {lineNumber}";
            }
            #endregion Methods

        }
    }
}
