using Microsoft.Dynamics.GP.eConnect;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using static Sinex.Http;
using static Tools.Query.DataType;
using static Sinex.AppTools.AppConnection;
using System.Xml;
using System.Xml.Serialization;

namespace Sinex
{
    public class DynamicsGp
    {
        public class PmTransaction
        {
            #region Variables
            string bachnumb;
            //string bachsrc;
            string vchnumwk;
            string vendorId;
            string docnumbr;
            int doctype;
            //string srcdoc;
            decimal docamnt;
            string docdate;
            decimal mscchamt;
            decimal prchamnt;
            decimal taxamnt;
            decimal frtamnt;
            decimal trdisamt;
            decimal chrgamnt;
            decimal cashamnt;
            decimal checkamnt;
            decimal crcrdamt;
            decimal distknam;
            public ServerConnection connection { get; set; }
            public string xmlDocument { get; set; }
            List<PurchaseOrderLine> distributions;
            #endregion Variables

            #region Properties
            public string BACHNUMB
            {
                get { return bachnumb; }
                set { bachnumb = value;  }
            }
            public string VCHNUMWK
            {
                get { return vchnumwk; }
                set { vchnumwk = value; }
            }
            public string VENDORID
            {
                get { return vendorId; }
                set { vendorId = value; }
            }
            public string DOCNUMBR
            {
                get { return docnumbr; }
                set { docnumbr = value; }
            }
            public int DOCTYPE
            {
                get { return doctype; }
                set { doctype = value; }
            }
            public decimal DOCAMNT
            {
                get { return docamnt; }
                set { docamnt = value; }
            }
            public string DOCDATE
            {
                get { return docdate; }
                set { docdate = value; }
            }
            public decimal MSCCHAMT
            {
                get { return mscchamt; }
                set { mscchamt = value; }
            }
            public decimal PRCHAMNT
            {
                get { return prchamnt; }
                set { prchamnt = value; }
            }
            public decimal TAXAMNT
            {
                get { return taxamnt; }
                set { taxamnt = value; }
            }
            public decimal FRTAMNT
            {
                get { return frtamnt; }
                set { frtamnt = value; }
            }
            public decimal TRDISAMT
            {
                get { return trdisamt; }
                set { trdisamt = value; }
            }
            public decimal CHRGAMNT
            {
                get { return chrgamnt; }
                set { chrgamnt = value; }
            }
            public decimal CASHAMNT
            {
                get { return cashamnt; }
                set { cashamnt = value; }
            }
            public decimal CHEKAMNT
            {
                get { return checkamnt; }
                set { checkamnt = value; }
            }
            public decimal CRCRDAMT
            {
                get { return crcrdamt; }
                set { crcrdamt = value; }
            }
            public decimal DISTKNAM
            {
                get { return distknam; }
                set { distknam = value; }
            }
            #endregion Properties

            #region Constructors
            public PmTransaction()
            {
                CommonConstructor();
            }
            public PmTransaction(string inVoucherNumber)
            {
                vchnumwk = inVoucherNumber;
                CommonConstructor();
            }
            public PmTransaction(SinexPo sinexPo, ServerConnection inConnection)
            {
                connection = inConnection;
                convertFromSinexPo(sinexPo);
                CommonConstructor();
            }
            public void CommonConstructor()
            {
                bachnumb = "1";
                distributions = new List<PurchaseOrderLine>();
            }
            #endregion Constructors

            #region Methods
            public void AddDistributions(PurchaseOrderLine line)
            {
                distributions.Add(line);
            }
            public void Insert(string databaseName)
            {
                try
                {
                    //StringBuilder qs = new StringBuilder();
                    //qs.Append($"INSERT INTO {databaseName}..PM10000");
                    //qs.Append("(BACHNUMB,VCHNUMWK,VENDORID,DOCAMNT,DOCNUMBR,DOCDATE)");
                    //qs.Append(" VALUES ");
                    //qs.Append($"('{bachnumb}','{vchnumwk}','{vendorId}',{docamnt.ToString()},'{docnumbr}','{docdate} + " 00:00:00.000"}')");
                    //Query q = new Query("Insert", qs.ToString(), dtVoid, connection, false);
                    //q.Execute();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            private void convertFromSinexPo(SinexPo po)
            {
                vchnumwk = po.poNumber;
                vendorId = VendorId(po.vendorname, connection.Database);
                docamnt = decimal.Parse(po.poTotal);
                docnumbr = vchnumwk;
                bachnumb = "1";
                doctype = 1;
                mscchamt = 0.00M;
                prchamnt = docamnt;
                taxamnt = 0.00M;
                frtamnt = 0.00M;
                trdisamt = 0.00M;
                chrgamnt = docamnt;
                cashamnt = 0.00M;
                checkamnt = 0.00M;
                crcrdamt = 0.00M;
                distknam = 0.00M;
                //long milliseconds = DateTime.Now.Ticks;
                DateTime requiredByDate = po.requiredBy.FromMicrosoftTime();
                docdate = $"{requiredByDate.ToString("yyyy-MM-dd")} 00:00:00.000";
            }
            public void ToLog(Log log)
            {
                log.Write($"PO HEADER => {vchnumwk}, {vendorId}, {docamnt}");
            }
            public string ToXML()
            {
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(stringwriter, this);
                    return stringwriter.ToString();
                }
            }
            public string ToEconnectXML()
            {
                string xml = "";
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(stringwriter, this);
                    xml = stringwriter.ToString();
                    xml = xml.RemoveNode("<connection>");
                    xml = xml.RemoveNode("PmTransaction", true);
                    xml = xml.RemoveNode("?xml", true);
                    xml = xml.Replace("</PmTransaction>", "");
                    string header = "<eConnect xmlns:dt=\"urn:schemas - microsoft - com:datatypes\">";
                    header = header + "<PMTransactionType>";
                    header = header + "<eConnectProcessInfo>";
                    header = header + "</eConnectProcessInfo>";
                    header = header + "<taPMTransactionInsert>";
                    string footer = "</taPMTransactionInsert>";
                    footer = footer + "</PMTransactionType>";
                    footer = footer + "</eConnect>";
                    xml = header + xml + footer;

                }
                return xml;
            }
            public int InsertXml(string xmlString)
            {
                string sConnectionString = "";
                Log errorLog = new Log(@"C:\Sinex\errorLog.txt");
                int errorCount = 0;
                errorLog.Clear();
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb = new StringBuilder();
                    sb.Append("<eConnect xmlns:dt=\"urn: schemas - microsoft - com:datatypes\">");
                   // sb.Append("<RMCustomerMasterType>");
                    sb.Append("<eConnectProcessInfo>");
                    sb.Append("</eConnectProcessInfo>");
                    sb.Append("<taPMTransactionInsert>");
                    sb.Append("<BACHNUMB>1</BACHNUMB>");
                    sb.Append("<VCHNUMWK>99999</VCHNUMWK>");
                    sb.Append("<VENDORID>ACETRAVE0001</VENDORID>");
                    sb.Append("<DOCNUMBR>99999</DOCNUMBR>");
                    sb.Append("<DOCTYPE>1</DOCTYPE>");
                    sb.Append("<DOCAMNT>99.99</DOCAMNT>");
                    sb.Append("<DOCDATE>2020-01-01</DOCDATE>");
                    sb.Append("<MSCCHAMT>0.00</MSCCHAMT>");
                    sb.Append("<PRCHAMNT>99.99</PRCHAMNT>");
                    sb.Append("<TAXAMNT>0.00</TAXAMNT>");
                    sb.Append("<FRTAMNT>0.00</FRTAMNT>");
                    sb.Append("<TRDISAMT>0.00</TRDISAMT>");
                    sb.Append("<CHRGAMNT>99.99</CHRGAMNT>");
                    sb.Append("<CASHAMNT>0.00</CASHAMNT>");
                    sb.Append("<CHEKAMNT>0.00</CHEKAMNT>");
                    sb.Append("<CRCRDAMT>0.00</CRCRDAMT>");
                    sb.Append("<DISTKNAM>0.00</DISTKNAM>");
                    sb.Append("</taPMTransactionInsert>");
                    //sb.Append("</RMCustomerMasterType>");
                    sb.Append("</eConnect>");

                    sConnectionString = "server=LAPTOP-G581GE4V\\SQL2016_GP2018R2;database=TWO;Integrated Security=SSPI;persist security info=False";
                    eConnectMethods eConnectObject = new eConnectMethods();
                    XmlDocument xmlDoc = new XmlDocument();
                    XmlDocument xmlDoc2 = new XmlDocument();
                    xmlString.Replace(" ", "");
                    xmlDoc.LoadXml(xmlString);

                    bool eConnectResult;
                    eConnectResult = eConnectObject.CreateEntity(sConnectionString, xmlDoc.OuterXml);
                    return errorCount;
                }
                catch (Exception ex)
                {// If an error occurs, diplay the error information to the user
                    errorCount = errorCount + 1;
                    int errorLocation = ex.Message.IndexOf("Error Number =");
                    int errorNumberLocation = errorLocation + 14;
                    string errorNumber = ex.Message.Substring(errorNumberLocation, 6).Trim();
                    switch (errorNumber)
                    {
                        case "305":
                            errorLog.Write($"ERROR: Document Number {docnumbr} already exists");
                            break;
                        case "306":
                            errorLog.Write($"ERROR: Voucher Number {vchnumwk} already exists");
                            break;
                        default:
                            errorLog.Error("Pmtransaction.InsertXml", ex);
                            break;
                    }
                }
                return errorCount;
            }
            private string VendorId(string VendorName, string databaseName)
            {
                string vendorId = "ACETRAVE0001";
                string qs = $"SELECT VENDORID FROM {databaseName}..PM00200 WHERE VENDNAME = '{VendorName}'";
                Query q = new Tools.Query("VendorId", qs, dtString, connection, false);
                q.Execute();
                string vendId = q.stringResult.Trim();
                return vendorId;
            }
            #endregion Methods
        }
        public class PurchaseOrderHeader
        {
            #region Variables
            public string batchNumber { get; set; }
            public string batchSource { get; set; }
            public string voucherNumber { get; set; }
            public string vendorId { get; set; }
            public string documentNumber { get; set; }
            public int documentType { get; set; }
            public string sourceDocument { get; set; }
            public decimal documentAmount { get; set; }
            public DateTime documentDate { get; set; }
            public string shippingMethod { get; set; }
            public string paymentTermsId { get; set; }

            List<PurchaseOrderLine> lines;

            #endregion Variables

            #region Properties
            #endregion Properties

            #region Constructors
            public PurchaseOrderHeader()
            {
                CommonConstructor();
            }
            public PurchaseOrderHeader(string inVoucherNumber)
            {
                voucherNumber = inVoucherNumber;
                CommonConstructor();
            }
            public PurchaseOrderHeader(SinexPo sinexPo)
            {
                convertFromSinexPo(sinexPo);
                CommonConstructor();
            }
            public void CommonConstructor()
            {
                lines = new List<PurchaseOrderLine>();
            }
            #endregion Constructors

            #region Methods
            public void AddLine(PurchaseOrderLine line)
            {
                lines.Add(line);
            }
            public void Insert()
            {

            }
            private void convertFromSinexPo(SinexPo po)
            {
                voucherNumber = po.poNumber;
                vendorId = po.clientVendorId;
                documentAmount = decimal.Parse(po.poTotal);
                paymentTermsId = po.terms;
                shippingMethod = po.shippingMethod;

            }
            public void ToLog(Log log)
            {
                log.Write($"PO HEADER => {voucherNumber}, {vendorId}, {documentAmount}");
            }
            #endregion Methods

        }
        public class PurchaseOrderLine
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
            public PurchaseOrderLine()
            {
                poid = "";
                lineId = "";
                lineNumber = "";
            }
            public PurchaseOrderLine(string inPoId, string inLineNumber)
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
        public static string NextSopDocumentNumber(string salesDocID)
        {
            GetNextDocNumbers sopTransNumber = new GetNextDocNumbers();
            string nextTransactionNumber = string.Empty;
            string sConnectionString = "";
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<eConnect xmlns:dt=\"urn: schemas - microsoft - com:datatypes\">");
                sb.Append("<RMCustomerMasterType>");
                sb.Append("<eConnectProcessInfo>");
                sb.Append("</eConnectProcessInfo>");
                sb.Append("<taUpdateCreateCustomerRcd>");
                sb.Append("<CUSTNMBR>JEFF0002</CUSTNMBR>");
                sb.Append("<CUSTNAME>JL Lawn Care Service</CUSTNAME>");
                sb.Append("<STMTNAME>JL Lawn Care Service </STMTNAME>");
                sb.Append("<SHRTNAME>JL Lawn Care</SHRTNAME>");
                sb.Append("<ADRSCODE>PRIMARY </ADRSCODE>");
                sb.Append("<ADDRESS1>123 Main Street</ADDRESS1>");
                sb.Append("<CITY>Valley City </CITY>");
                sb.Append("<STATE>ND </STATE>");
                sb.Append("<ZIPCODE>58072 </ZIPCODE>");
                sb.Append("<COUNTRY>USA </COUNTRY>");
                sb.Append("<PHNUMBR1>55532336790000 </PHNUMBR1>");
                sb.Append("<PHNUMBR2>55551161817181 </PHNUMBR2>");
                sb.Append("<FAX>55584881000000 </FAX>");
                sb.Append("<UPSZONE>red </UPSZONE>");
                sb.Append("<SHIPMTHD>PICKUP </SHIPMTHD>");
                sb.Append("<TAXSCHID>ILLINOIS</TAXSCHID>");
                sb.Append("<PRBTADCD>PRIMARY </PRBTADCD>");
                sb.Append("<PRSTADCD>PRIMARY </PRSTADCD>");
                sb.Append("<STADDRCD>PRIMARY </STADDRCD>");
                sb.Append("<SLPRSNID>GREG E.</SLPRSNID>");
                sb.Append("<SALSTERR>TERRITORY 6</SALSTERR>");
                sb.Append("<COMMENT1>comment1</COMMENT1>");
                sb.Append("<COMMENT2>comment2</COMMENT2>");
                sb.Append("<PYMTRMID>Net 30 </PYMTRMID>");
                sb.Append("<CHEKBKID>PAYROLL </CHEKBKID>");
                sb.Append("<KPCALHST>0 </KPCALHST>");
                //sb.Append("<RMCSHACTNUMST>000-1200-00 </RMCSHACTNUMST>");
                sb.Append("<UseCustomerClass>0</UseCustomerClass>");
                sb.Append("<UpdateIfExists>1</UpdateIfExists>");
                sb.Append("</taUpdateCreateCustomerRcd>");
                sb.Append("</RMCustomerMasterType>");
                sb.Append("</eConnect>");

                sb = new StringBuilder();
                sb.Append("<eConnect xmlns:dt=\"urn: schemas - microsoft - com:datatypes\">");
                sb.Append("<RMCustomerMasterType>");
                sb.Append("<eConnectProcessInfo>");
                sb.Append("</eConnectProcessInfo>");
                sb.Append("<taPMTransactionInsert>");
                sb.Append("<BACHNUMB>1</BACHNUMB>");
                sb.Append("<VCHNUMWK>99999</VCHNUMWK>");
                sb.Append("<VENDORID>ACETRAVE0001</VENDORID>");
                sb.Append("<DOCNUMBR>99999</DOCNUMBR>");
                sb.Append("<DOCTYPE>1</DOCTYPE>");
                sb.Append("<DOCAMNT>99.99</DOCAMNT>");
                sb.Append("<DOCDATE>2020-01-01</DOCDATE>");
                sb.Append("<MSCCHAMT>0.00</MSCCHAMT>");
                sb.Append("<PRCHAMNT>99.99</PRCHAMNT>");
                sb.Append("<TAXAMNT>0.00</TAXAMNT>");
                sb.Append("<FRTAMNT>0.00</FRTAMNT>");
                sb.Append("<TRDISAMT>0.00</TRDISAMT>");
                sb.Append("<CHRGAMNT>99.99</CHRGAMNT>");
                sb.Append("<CASHAMNT>0.00</CASHAMNT>");
                sb.Append("<CHEKAMNT>0.00</CHEKAMNT>");
                sb.Append("<CRCRDAMT>0.00</CRCRDAMT>");
                sb.Append("<DISTKNAM>0.00</DISTKNAM>");

                sb.Append("</taPMTransactionInsert>");
                sb.Append("</RMCustomerMasterType>");
                sb.Append("</eConnect>");

                sConnectionString = "server=LAPTOP-G581GE4V\\SQL2016_GP2018R2;database=TWO;Integrated Security=SSPI;persist security info=False";
                eConnectMethods eConnectObject = new eConnectMethods();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sb.ToString());
                bool eConnectResult;
                eConnectResult = eConnectObject.CreateEntity(sConnectionString, xmlDoc.OuterXml);
                return nextTransactionNumber;
            }
            catch (Exception ex)
            {// If an error occurs, diplay the error information to the user
                MessageBox.Show(ex.Message);
                throw ex;
            }
            finally
            {
                sopTransNumber.Dispose();
            }
        }
    }
}
