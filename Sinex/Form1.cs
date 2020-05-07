// http://integration.sinexmarine.com/webservices/integrationservice.asmx
// AuthID: EB0CD5DF-E866-443C-9940-730B39DAA75F
// CompanyID: bf8385ef-b76d-4f5e-9b0c-6b2dac723e6f


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using static Sinex.Http;
using Newtonsoft.Json;
using static Sinex.Http.HttpFile;
using static Tools.TextFile;
using static Sinex.AppTools;
using static Sinex.DynamicsGp;

namespace Sinex
{ 
    public partial class Form1 : Form
    {
        AppLog log;
        public Form1()
        {
            InitializeComponent();
            log = new AppLog(false, AppConfigFile.GetValue("LOGPATH"));
            log.Clear();
            log.Write("SINEX Initialized");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AppConfigFile.GetValue("LOGPATH");
            string url = "";
            string result = "";
            string resultPrelim = "";
            SinexPoLine poLine = new Sinex.Http.SinexPoLine();
            string inputType = "TextFile";
            switch (inputType)
            {
                case "Download":
                    Http.Connection connection = new Http.Connection(url);
                    result = connection.Test();
                    break;
                case "TextFile":
                    resultPrelim = LoadTextFile(@"C:\Temp\Sinex\SinexFile.txt");
                    WriteTextFile(@"C:\Temp\Sinex\RawFile.txt", resultPrelim);
                    result = GetPos(resultPrelim);
                    WriteTextFile(@"C:\Temp\Sinex\Pos.txt", result);
                    break;
                case "TextBox":
                    result = textBox1.Text;
                    break;
                
            }
            bool continueLoop = true;
            int currentIndex = 0;
            int poStart = 0;
            int lineItemsArrayStart = result.IndexOf("[");
            int lineItemsArrayEnd = result.IndexOf("]");
            int poArrayEnd = result.LastIndexOf("]");
            int poHeaderStart = 0;
            int poHeaderEnd = result.IndexOf("lineItems") - 3;
            int poEnd = lineItemsArrayEnd + 1;
            string poHeader = "";
            string poHeaderNumber = "";
            DataTable gridResults = new DataTable();
            gridResults.Columns.Add(poHeaderNumber, typeof(string));
            while (continueLoop)
            {
                poHeaderStart = currentIndex;
                lineItemsArrayStart = result.IndexOf("[", currentIndex);
                lineItemsArrayEnd = result.IndexOf("]", currentIndex);
                poHeaderEnd = lineItemsArrayEnd; //result.IndexOf("lineItems", currentIndex) - 3;
                poHeader = result.SubFromTo(poHeaderStart, poHeaderEnd) + "}";
                WriteTextFile(@"C:\Temp\Sinex\poHeader-PO.txt", poHeader);
                SinexPo po = new SinexPo();
                po = JsonConvert.DeserializeObject<SinexPo>(poHeader);
                poHeaderNumber = po.poNumber;
                WriteTextFile(@"C:\Temp\Sinex\" + poHeaderNumber + " - PO.txt", poHeader);

                currentIndex = poHeaderEnd + 3;
                if(currentIndex == poArrayEnd)
                {
                    continueLoop = false;
                }
                DataRow dr = gridResults.NewRow();
                dr[0] = poHeaderNumber;
                gridResults.Rows.Add(dr);
            }
            grid.DataSource = gridResults;

            //string poHeader = result.SubFromTo(poHeaderStart, poHeaderEnd) + "}";
            //WriteTextFile(@"C:\Temp\Sinex\PoHeader.txt", poHeader);
            //Po po = new Po();
            //po = JsonConvert.DeserializeObject<Po>(poHeader);

            string poLines = result.SubFromTo(lineItemsArrayStart, lineItemsArrayEnd);
            WriteTextFile(@"C:\Temp\Sinex\PoLines.txt", poLines);

            poLine = JsonConvert.DeserializeObject<SinexPoLine>(result);
            //MessageBox.Show(poLine.poNumber);
            log.Write(result);
            MessageBox.Show(result);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CreateFile(@"C:\Temp\Sinex\PoFile.txt");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Run();
        }
        private void Run()
        {
            //string test = NextSopDocumentNumber("");
            string resultPrelim = LoadTextFile(@"C:\Temp\Sinex\SinexFile.txt");
            WriteTextFile(@"C:\Temp\Sinex\RawFile.txt", resultPrelim);
            string result = GetPos(resultPrelim);
            WriteTextFile(@"C:\Temp\Sinex\Pos.txt", result);
            TraverseFile(result);
        }
    }
}
