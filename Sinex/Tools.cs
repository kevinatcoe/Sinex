using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Documents;
using System.Windows.Forms;
using static Tools.Query.DataType;
using static Tools.Log;
using static Tools.DtTools;
using static Tools.Strings;
using static Tools.SystemTools;
using static System.Console;

namespace Tools
{
    public static class ArrayTools
    {
        public static void ToLog(this string[] array, Log log)
        {
            if (array != null)
            {
                StringBuilder buildString = new StringBuilder();
                buildString.Append("{");
                foreach (string s in array)
                {
                    buildString.Append($" {s},");
                }
                log.Write($"{buildString.ToString().RemoveLastChar(1)} }}");
            }
            else
            {
                log.Write("ArrayTools.ToLog(): The array is null.");
            }
        }
        public static string ToCsv(this string[] array)
        {
            if (array != null)
            {
                StringBuilder buildString = new StringBuilder();
                foreach (string s in array)
                {
                    buildString.Append($" {s},");
                }
                return buildString.ToString().RemoveLastChar(1);
            }
            else
            {
                return null;
            }
        }
    }
    public static class ContextMenuStripTools
    {
        public static bool ContainsItem(this ContextMenuStrip toolStrip, string itemText)
        {
            bool endLoop = false;
            bool contains = false;
            int itemIndex = 0;
            if (toolStrip.Items.Count > 0)
            {

                while (!endLoop)
                {
                    string currentItemText = toolStrip.Items[itemIndex].Text;
                    if (currentItemText == itemText)
                    {
                        contains = true;
                        endLoop = true;
                    }
                    itemIndex++;
                    if (itemIndex == toolStrip.Items.Count)
                    {
                        endLoop = true;
                    }
                }
                return contains;
            }
            else
            {
                return false;
            }
        }
    }
    public class CheckBoxVisualSwitch
    {
        #region Variables
        CheckBox checkBox;
        string checkedText;
        string uncheckedText;
        System.Drawing.Color fontColor;
        System.Drawing.Color checkedColor;
        System.Drawing.Color uncheckedColor;
        #endregion #Variables
        #region Properties

        #endregion Properties
        public string CheckedText
        {
            get
            {
                return checkedText;
            }
            set
            {
                checkedText = value;
            }
        }
        public string UncheckedText
        {
            get
            {
                return uncheckedText;
            }
            set
            {
                uncheckedText = value;
            }
        }
        public System.Drawing.Color FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }
        }
        public System.Drawing.Color CheckedColor
        {
            get
            {
                return checkedColor;
            }
            set
            {
                checkedColor = value;
            }
        }
        public System.Drawing.Color UncheckedColor
        {
            get
            {
                return uncheckedColor;
            }
            set
            {
                uncheckedColor = value;
            }
        }
        #region Constructors
        public CheckBoxVisualSwitch() { }
        public CheckBoxVisualSwitch(CheckBox inCheckBox, string inCheckedText, string inUncheckedText)
        {
            checkBox = inCheckBox;
            CheckedText = inCheckedText;
            UncheckedText = inUncheckedText;
        }
        #endregion Constructors
        #region Methods
        public void Switch()
        {
            checkBox.ForeColor = fontColor;
            if (checkBox.Checked)
            {
                checkBox.Text = checkedText;
                checkBox.BackColor = checkedColor;
            }
            else
            {
                checkBox.Text = uncheckedText;
                checkBox.BackColor = uncheckedColor;
            }
        }
        public void SetChecked()
        {
            //if (!checkBox.Checked)
            //{
            checkBox.ForeColor = fontColor;
            checkBox.Text = checkedText;
            checkBox.BackColor = checkedColor;
            //}
        }
        public void SetUnchecked()
        {
            //if (checkBox.Checked)
            //{
            checkBox.ForeColor = fontColor;
            checkBox.Text = uncheckedText;
            checkBox.BackColor = uncheckedColor;
            //}
        }
        #endregion Methods
    }
    public class DropButton
    {

        #region Variables
        Button button;
        ContextMenuStrip menuStrip;
        #endregion Variables

        #region Properties
        #endregion Properties
        public DropButton(Button inButton, ContextMenuStrip inMenuStrip)
        {
            button = inButton;
            menuStrip = inMenuStrip;
            menuStrip.Items.Clear();
        }
        #region Constructors

        #endregion Constructors

        #region Methods
        public void AddMenuItem(string menuItem)
        {
            if (!menuStrip.ContainsItem(menuItem))
            {
                menuStrip.Items.Add(menuItem);
            }
        }
        public void Change()
        {

        }
        #endregion Methods

    }
    public static class ComboBoxTools
    {
        public static void fill(this ComboBox comboBox, string[] items, int defaultIndex, System.Windows.Forms.TextBox textBox = null)
        {
            foreach (string s in items)
            {
                comboBox.Items.Add(s);
                comboBox.SelectedIndex = 0;
            }
            comboBox.SelectedIndex = defaultIndex;
            if (textBox != null)
            {
                textBox.Text = comboBox.Items[0].ToString();
            }
        }
    }
    public class ConfigFile
    {
        public char Delimeter { get; set; }
        public string Comment { get; set; }
        string Application;
        public string Path { get; set; }
        public string[] keyArray { get; set; }
        Log log;
        struct configPair
        {
            string strKey;
            string strValue;
            public configPair(string fileLine, char Delimeter)
            {
                strKey = "";
                strValue = "";
                int delimiterIndex = fileLine.IndexOf(Delimeter);
                //int delimiter = Strings.InStr(1,fileLine, DELIMITER, CompareMethod.Text);
                if (delimiterIndex > 0)
                {
                    strKey = fileLine.Substring(0, delimiterIndex);
                    strValue = fileLine.Substring(delimiterIndex + 1, fileLine.Length - delimiterIndex - 1);
                }
            }
            public configPair(string key, string value)
            {
                strKey = key;
                strValue = value;
            }
            public string Key
            {
                get
                {
                    return strKey;
                }
                set
                {
                    strKey = value;
                }
            }
            public string Value
            {
                get
                {
                    return strValue;
                }
                set
                {
                    strValue = value;
                }
            }
            public string toString(string Delimeter)
            {
                return strKey + Delimeter + strValue;
            }
        }
        List<configPair> Pairs;
        public ConfigFile()
        {
            Application = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Pairs = new List<configPair>();
            log = new Log();
        }
        public ConfigFile(string inPath)
        {
            log = new Log();
            Path = inPath;
            Exists(true);
            Pairs = new List<configPair>();
            Load();
        }
        public ConfigFile(string inPath, string inComment, char inDelimeter, string[] keyArray)
        {
            log = new Log();
            Path = inPath;
            Comment = inComment;
            Delimeter = inDelimeter;
            PopulatePairs();
            Exists(true);
            Pairs = new List<configPair>();
            Load();
        }
        public ConfigFile(string inApplication, string inPath, bool inLoadList)
        {
            log = new Log();
            Application = inApplication;
            //log.Write("Path => " + Application + ", " + Path);
            Path = Path + inApplication + ".config";
            Exists(true);
            Pairs = new List<configPair>();

            if (inLoadList)
            {
                Load();
            }
        }
        public string Item(int Index, Boolean ToLower = false)
        {
            try
            {
                if (ToLower == true)
                {
                    return Pairs[Index].Value.ToLower();
                }
                else
                {
                    return Pairs[Index].Value;
                }
            }
            catch (Exception ex)
            {
                logit("ERROR: ConfigFile.Item(int Index) " + ex.Message);
                return "";
            }
        }
        public string Item(string Key, Boolean ToLower = false)
        {
            try
            {
                Boolean exitLoop = false;
                int index = 0;
                string returnValue = "";
                while (index < Pairs.Count && !exitLoop)
                {
                    if (Pairs[index].Key.ToLower() == Key.ToLower())
                    {
                        if (ToLower == true)
                        {
                            returnValue = Pairs[index].Value.Trim().ToLower();
                        }
                        else
                        {
                            returnValue = Pairs[index].Value.Trim();
                        }
                        exitLoop = true;
                    }
                    index++;
                }
                if (returnValue == "")
                {
                    logit("Config Item Missing: " + Key);
                }
                return returnValue;
            }
            catch (Exception ex)
            {
                logit("ERROR: ConfigFile.Item(string Key) " + ex.Message);
                return "";
            }
        }
        public void Show()
        {
            logit("APPCONFIG");
            logit("---------");
            string displayString = "";
            foreach (configPair cp in Pairs)
            {
                displayString = displayString + " (" + cp.Key + ":" + cp.Value + ") " + System.Environment.NewLine;
            }
            logit(displayString);
        }
        public void Create()
        {
            PopulatePairs();
            using (StreamWriter sw = File.CreateText(Path))
            {
                //sw.WriteLine(Comment + " " + Path);
            }
            Save();
        }
        public Boolean Exists(bool CreateIfMissing)
        {
            if (!File.Exists(Path))
            {
                Create();
            }
            return File.Exists(Path);
        }
        public void Add(string Key, string Value)
        {
            configPair cp = new configPair(Key, Value);
            Pairs.Add(cp);
        }
        public void Save()
        {
            Clear();
            foreach (configPair cp in Pairs)
            {
                File.AppendAllText(Path, cp.Key + Delimeter + cp.Value + Environment.NewLine);
            }
        }
        public int Load()
        {
            string line = "";
            try
            {
                if (File.Exists(Path))
                {
                    configPair cp;
                    using (StreamReader sr = new StreamReader(Path))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Substring(0, 1) != Comment)
                            {
                                cp = new configPair(line, Delimeter);
                                Pairs.Add(cp);
                            }
                        }
                    }
                    return 0;
                }
                else
                {
                    //Create();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                log.Error("AppConfig.Load", ex, true);
                log.Write("Path = " + Path);
                log.Write("Line = " + line);
                return -1;
            }
        }
        public void Clear()
        {
            File.WriteAllText(Path, String.Empty);
            using (StreamWriter sw = File.CreateText(Path))
            {
                sw.WriteLine(Comment + " " + Path);
            }
        }
        public int inList(string inKey)
        {
            int returnValue = -1;
            int currentIndex = 0;
            foreach (configPair cp in Pairs)
            {
                if (cp.Key == inKey)
                {
                    returnValue = currentIndex;
                }
                currentIndex++;
            }
            return returnValue;
        }
        public void Update(string Key, string Value)
        {
            Log log = new Tools.Log();
            try
            {
                configPair cp = new configPair(Key, Value);
                int existingIndex = inList(Key);
                if (existingIndex != -1)
                {
                    Pairs[existingIndex] = cp;
                    Save();
                }
            }
            catch (Exception ex)
            {
                log.Error("Update", ex, true);
            }
        }
        public DataTable GetItemsAsTable()
        {
            Log log = new Log();
            DataTable dt = new DataTable();
            dt.Columns.Add("Key", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            foreach (configPair pair in Pairs)
            {
                DataRow dr = dt.NewRow();
                dr[0] = pair.Key;
                dr[1] = pair.Value;
                log.Write("=> " + dr[0] + ", " + dr[1]);
                dt.Rows.Add(dr);
            }
            return dt;
        }
        private void PopulatePairs()
        {
            foreach (string str in keyArray)
            {
                configPair pair = new configPair(str, "");
                Pairs.Add(pair);
            }
        }
    }
    public static class DataTypeTools
    {
        public static string QuoteFromDatatype(this object obj, System.Type datatype)
        {
            //Type str = typeof(string);
            //Type igr = typeof(int);
            //Type boo = typeof(bool);
            //Type dbl = typeof(double);
            //Type date = typeof(DateTime);
            //Type guid = typeof(Guid);

            string returnString = obj.ToString();
            switch (datatype.Name)
            {
                case "String":
                case "DateTime":
                case "Guid":
                    returnString = $"'{obj.ToString()}'";
                    break;
            }
            return returnString;
        }
    }
    public static class DateTools
    {
        public static DateTime FromMicrosoftTime(this string microsoftTime)
        {
            // /Date(1587452400000)\
            string timeInMilliseconds = microsoftTime.Between("(", ")");
            double ticks = double.Parse(timeInMilliseconds);
            TimeSpan time = TimeSpan.FromMilliseconds(ticks);
            DateTime returnTime = new DateTime(1970, 1, 1) + time;
            return returnTime;
        }
    }
    public static class DisplayTools
    {
        public static void showit(string message)
        {
            MessageBox.Show(message);
        }
        public static void showit(int message)
        {
            showit(message.ToString());
        }
    }
    public static class DrTools
    {
        //public static string[] DataRowAsArray(DataRow dr)
        //{
        //    string[] array =  

        //}
    }
    public static class DtTools
    {
        public static void List(DataTable dt, String Name = "")
        {
            Log log = new Tools.Log();
            StringBuilder sb = new StringBuilder();
            log.WriteBlank();
            log.WriteBlank();
            log.Write(":: DataTable :: " + Name);
            if (dt != null)
            {
                sb.Clear();
                log.Write("+ Data");
                foreach (DataColumn dc in dt.Columns)
                {
                    String columnName = dc.ColumnName.Trim();
                    sb.Append("     " + columnName.PadRight(40 - columnName.Length, ' '));
                }
                log.Write(sb.ToString());

                foreach (DataRow dr in dt.Rows)
                {
                    sb.Clear();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        String fieldName = dr[dc].ToString().Trim();
                        sb.Append("     " + fieldName.Trim().PadRight(40 - fieldName.Length, ' '));
                    }
                    log.Write(sb.ToString());
                }
                log.Write("- Data");

                log.WriteDivider();

                log.Write("+ Columns");
                int columnNumber = 0;
                foreach (DataColumn column in dt.Columns)
                {
                    log.Write("     " + columnNumber.ToString() + " " + column.ColumnName);
                    columnNumber++;
                }
                log.Write("- Columns");

                DataColumn[] columns;
                columns = dt.PrimaryKey;

                log.WriteDivider();
                log.Write("+ Primary Key");
                for (int i = 0; i < columns.Length; i++)
                {
                    log.Write("     " + columns[i].ColumnName + " " + columns[i].DataType);
                }
                log.Write("- Primary Key");
            }
            else
            {
                log.Write("null");
            }
        }
        public static List<string> ToList(this DataTable dt, String Name = "")
        {
            Log log = new Tools.Log();
            List<string> list = new List<string>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(dr.ItemArray[0].ToString().Trim());
                }
                return list;
            }
            else
            {
                log.Write("DtTools.ToList: List is null");
                return null;
            }
        }
        public static string ToCsv(this DataTable dt, String Name = "")
        {
            Log log = new Tools.Log();
            StringBuilder csv = new StringBuilder();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    csv.Append($"{dr.ItemArray[0].ToString().Trim()},");
                }
                return csv.ToString().RemoveLastChar(1);
            }
            else
            {
                log.Write("DtTools.ToCsv: Table is null");
                return null;
            }
        }
        public static DataTable Transpose(DataTable dataTable)
        {
            DataTable dtNew = new DataTable();
            if (dataTable != null)
            {
                Log log = new Tools.Log();
                String columnName = "";

                for (int i = 0; i <= dataTable.Rows.Count; i++)
                {
                    dtNew.Columns.Add(i.ToString());
                }

                for (int k = 0; k < dataTable.Columns.Count; k++)
                {
                    DataRow r = dtNew.NewRow();
                    r[0] = dataTable.Columns[k].ToString();
                    for (int j = 1; j <= dataTable.Rows.Count; j++)
                    {
                        r[j] = dataTable.Rows[j - 1][k];
                    }
                    dtNew.Rows.Add(r);
                }
                //DtTools.List(dtNew);
            }
            return dtNew;
        }
        public static DataTable FromGridViewRow(DataGridViewRow gvr)
        {
            Log log = new Tools.Log();
            DataTable dt = new DataTable();
            //dt.Columns.Add("Col");
            DataRow datarw;

            //  log.Write("FromGridViewRow");
            //foreach(GridViewRow row in gvr.Rows)
            //{
            datarw = dt.NewRow();
            for (int i = 0; i < gvr.Cells.Count; i++)
            {
                dt.Columns.Add(i.ToString());
                //        log.Write(gvr.Cells[i].Text);
                datarw[i] = gvr.Cells[i].Value;
            }
            dt.Rows.Add(datarw);
            //}
            return dt;
        }
        public static void Show(DataTable dt, string Source)
        {
            if (dt != null)
            {
                StringBuilder sb = new StringBuilder();
                logit("DIV");
                logit("DataTable = " + Source);
                logit("DIV");
                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        sb = new StringBuilder();
                        foreach (object s in dr.ItemArray)
                        {
                            sb.Append(Strings.MakeString(s) + ", ");
                        }
                        logit(sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    logerror("DtShow ", ex, true);
                }
            }
            else
            {
                logit($"dtTools.Show({Source}) - DataTable is null");
            }
        }
        public static void ToLog(this DataTable dt, string Source)
        {
            if (dt != null)
            {
                StringBuilder sb = new StringBuilder();
                logit("DIV");
                logit("DataTable = " + Source);
                logit("DIV");
                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        sb = new StringBuilder();
                        foreach (object s in dr.ItemArray)
                        {
                            sb.Append(Strings.MakeString(s) + ", ");
                        }
                        logit(sb.ToString());
                    }
                }
                catch (Exception ex)
                {
                    logerror("DtTools.ToLog " + Source, ex, true);
                }
            }
            else
            {
                logit($"dtTools.ToLog({Source}) - DataTable is null");
            }
        }
        public static void ToLog(DataTable dt, string Source, string LineNumber)
        {
            ToLog(dt, $"{Source} ({LineNumber})");
        }
        public static string CSV(this DataTable dtValues)
        {
            StringBuilder builder = new StringBuilder();
            foreach (DataRow dr in dtValues.Rows)
            {
                if (builder.ToString().IndexOf(dr.ItemArray[0].ToString().Trim()) == -1)
                {
                    builder.Append(dr.ItemArray[0].ToString().Trim());
                    builder.Append(",");
                }
            }
            return builder.ToString().RemoveLastChar(1);
        }
        public static string[] ToArray(this DataTable dt, string Source)
        {
            if (dt != null)
            {
                int numberOfElements = dt.Rows.Count;
                int currentArrayIndex = 0;
                if (numberOfElements > 0)
                {
                    string[] array = new string[numberOfElements];
                    try
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            array[currentArrayIndex] = dr.ItemArray[0].ToString();
                            currentArrayIndex++;
                        }
                        return array;
                    }
                    catch (Exception ex)
                    {
                        logerror("DtTools.ToArray " + Source, ex, false);
                        return null;
                    }
                }
                else
                {
                    logit($"dtTools.ToArray({Source}) - No elements exist");
                    return null;
                }
            }
            else
            {
                logit($"dtTools.ToArray({Source}) - DataTable is null");
                return null;
            }
        }
        public static string[] GetColumnNamesAsArray(this DataTable dt)
        {
            try
            {
                string[] columnNames = new string[dt.Columns.Count];
                int index = 0;
                foreach (DataColumn dc in dt.Columns)
                {
                    columnNames[index] = dc.ColumnName;
                }
                return columnNames;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static object GetObject(this DataTable dt, int rowNumber, int columnNumber)
        {
            return dt.Rows[rowNumber].ItemArray[columnNumber];
        }
        public static string GetString(this DataTable dt, int rowNumber, int columnNumber)
        {
            return dt.GetObject(rowNumber, columnNumber).ToString();
        }
        public static int GetInt(this DataTable dt, int rowNumber, int columnNumber)
        {
            return dt.GetObject(rowNumber, columnNumber).ToInt();
        }
        public static double GetDouble(this DataTable dt, int rowNumber, int columnNumber)
        {
            return dt.GetObject(rowNumber, columnNumber).ToDouble();
        }
    }
    public static class ListBoxTools
    {
        public static ListBox AddAll(this ListBox destListBox, ListBox sourceListBox)
        {
            foreach (string item in sourceListBox.Items)
            {
                destListBox.AddListBoxItem(item);
            }
            return destListBox;
        }
        public static ListBox AddAll(this ListBox destListBox, ListBox sourceListBox, ListBox filterListBox)
        {
            foreach (string item in sourceListBox.Items)
            {
                if (filterListBox.Items.IndexOf(item) == -1)
                {
                    destListBox.AddListBoxItem(item);
                }
            }
            return destListBox;
        }
        public static bool AddListBoxItem(this ListBox listBox, string Value)
        {
            bool alreadyExists = false;
            if (Value != "")
            {
                int selectedIndex;
                int itemIndex = listBox.Items.IndexOf(Value);
                if (itemIndex == -1)
                {
                    alreadyExists = false;
                    listBox.Items.Add(Value);
                    selectedIndex = listBox.Items.Count - 1;
                    listBox.SelectedIndex = selectedIndex;
                }
                else
                {
                    alreadyExists = true;
                    listBox.SelectedItem = Value;
                    listBox.SelectedIndex = itemIndex;
                }
            }
            return alreadyExists;
        }
        public static ListBox FillFromList(this ListBox lb, List<string> list)
        {
            if (list != null)
            {
                lb.Items.Clear();
                foreach (string s in list)
                {
                    lb.Items.Add(s);
                }
                return lb;
            }
            else
            {
                return null;
            }
        }
        public static ListBox FillFromDataTable(this ListBox listBox, DataTable dt, int columnIndex = 0)
        {
            if (dt != null)
            {
                listBox.Items.Clear();
                foreach (DataRow dr in dt.Rows)
                {
                    string item = dr.ItemArray[columnIndex].ToString().Trim();
                    if (!listBox.ItemExists(item))
                    {
                        listBox.Items.Add(item);
                    }
                }
            }
            return listBox;
        }
        public static ListBox FillFromTable(ListBox lb, DataTable dt)
        {
            lb.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                lb.Items.Add(dr.ItemArray[0].ToString());
            }
            return lb;
        }
        public static ListBox FillFromTable(DataTable dt)
        {
            ListBox lb = new ListBox();
            lb.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                lb.Items.Add(dr.ItemArray[0].ToString());
            }
            return lb;
        }
        public static Boolean ItemExists(this ListBox lb, string Value)
        {
            return lb.Items.Contains(Value);
        }
        public static string ToCsv(this ListBox lb, bool AddSingleQuote = false)
        {
            string csv = "";
            string quote = "";
            if (AddSingleQuote)
            {
                quote = "'";
            }
            foreach (string item in lb.Items)
            {
                csv = csv + quote + item.ToString().Trim() + quote + ",";
            }
            return csv.RemoveLastChar(1);
        }
        public static ListBox RemoveListBoxItem(this ListBox listBox, string Value)
        {
            if (!listBox.Items.Contains(Value))
            {
                listBox.Items.Remove(Value);
            }
            return listBox;
        }
    }
    public static class ListTools
    {
        public static void AddItem(this List<string> list, string value)
        {
            if (value != null)
            {
                if (!list.Contains(value))
                {
                    list.Add(value);
                }
            }
        }
        public static void ToLog(this List<string> list, Log log)
        {
            foreach (string item in list)
            {
                log.Write($"- {item}");
            }
        }
        public static string[] ToArray(this List<string> list)
        {
            string[] array = new string[list.Count];
            int arrayIndex = 0;
            foreach (string item in list)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
            return array;
        }
    }
    public class Log
    {
        #region Variables
        string strPath;
        string strSessionPath;
        public ServerConnection connection { get; set; }
        string database;
        string strApplication;
        public int type { get; set; } // 0 = text, 1 = sql
        public string typeName { get; set; } // ttext | sql
        public bool useSql { get; set; }
        public string appUser { get; set; }
        public string appVersion { get; set; }
        public bool IncludeTime { get; set; }
        public string Path
        {
            get
            {
                return strPath;
            }

            set
            {
                strPath = value;
            }
        }
        public string SessionPath
        {
            get
            {
                return strSessionPath;
            }

            set
            {
                strSessionPath = value;
            }
        }
        #endregion Variables

        #region Properties
        public string Database
        {
            get
            {
                return database;
            }
            set
            {
                database = value;
            }
        }

        #endregion Properties

        #region Contructors
        // 0 = Text File
        // 1 = SQL Database
        public Log()
        {
            useSql = false;
            PrepareTextLog();
            ConstructorCommon();
        }
        public Log(string Path)
        {
            useSql = false;
            strPath = Path;
            type = 0;
            ConstructorCommon();
        }
        public Log(string Path, bool ClearLog)
        {
            useSql = false;
            strPath = Path;
            type = 0;
            ConstructorCommon();
            if (ClearLog)
            {
                ClearText();
            }
        }
        public Log(ServerConnection connSql)
        {
            useSql = true;
            strApplication = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            connection = connSql;
            database = strApplication;
            type = 1;
            ConstructorCommon();
        }
        private void ConstructorCommon()
        {
            //AppConfig appConfig = new AppConfig(true);
            typeName = "text";
            type = 0;
        }
        #endregion Contructors

        #region Methods
        public void Test()
        {
            logit("This is a test");
        }
        public static void WriteTemp(string Message)
        {
            File.AppendAllText("C:\\Temp\\Log.log", "WriteTemp");
            File.AppendAllText("C:\\Temp\\Log.log", Message + Environment.NewLine);
        }
        private void PrepareTextLog()
        {
            type = 0;
            strPath = @"C:\Temp\" + Assembly.GetExecutingAssembly().GetName().Name + ".log";
            strSessionPath = strPath;
        }
        public void Error(string source, Exception e, Boolean showDetails = false)
        {
            Write("ERROR in " + source);
            Write(e.Message);
            if (showDetails)
            {
                if (e.InnerException != null)
                {
                    Write(e.InnerException.ToString());
                }
                if (e.Source != null)
                {
                    Write(e.Source);
                }
                if (e.StackTrace != null)
                {
                    Write(e.StackTrace);
                }
            }
        }
        public void WriteText(string Message)
        {
            switch (Message)
            {
                case "DIV":
                    Message = "-------------------";
                    break;
                default:
                    string timeString = DateTime.Now.ToString() + " :: ";
                    if (!IncludeTime)
                    {
                        timeString = "";
                    }
                    Message = timeString + Message;
                    break;
            }
            if (Message != null)
            {
                File.AppendAllText(strPath, Message + Environment.NewLine);
            }
        }
        public void WriteSessionText(string Message)
        {
            File.AppendAllText(strSessionPath, Message + Environment.NewLine);
        }
        public void WriteSql(string Message)
        {
            try
            {
                string QueryString = "INSERT INTO Log (Message,Time) VALUES ('" + Message + "', GETDATE())";
                Query q = new Query("Tools.Log.WriteSql", QueryString, dtVoid, connection, false);
                q.ExecuteNoReturn();
            }
            catch (Exception e)
            {
                WriteText(e.Message);
            }
        }
        public void WriteSql(string message, string source)
        {
            try
            {
                string qs = $"INSERT INTO Log (Message,Time, UserId, Source, Version, Computer) VALUES ('{message}', GETDATE(),'{appUser}','{source}','{appVersion}','{ComputerName()}')";
                Query q = new Query("Tools.Log.WriteSql", qs, dtVoid, connection, false);
                q.ExecuteNoReturn();
            }
            catch (Exception e)
            {
                WriteText(e.Message);
            }
        }
        public void Write(string Message)
        {
            switch (Message)
            {
                case "CLEAR":
                    ClearText();
                    break;
                default:
                    switch (useSql)
                    {
                        case false:
                            WriteText(Message);
                            break;
                        case true:
                            WriteSql(Message);
                            break;
                    }
                    break;
            }
        }
        public void WriteCommas(String Comment, String Message1, String Message2, String Message3 = "")
        {
            WriteText(Comment + " => ||" + Message1 + "||" + Message2 + "||" + Message3);
        }
        public void Write(string Message, Boolean enabled)
        {
            if (enabled)
            {
                switch (useSql)
                {
                    case false:
                        WriteText(Message);
                        break;
                    case true:
                        WriteSql(Message);
                        break;
                }
            }
        }
        public void Write(string source, string message)
        {
            message = message.Replace("'", "~");
            switch (useSql)
            {
                case false:
                    WriteText(message);
                    break;
                case true:
                    WriteSql(message, source);
                    break;
            }
        }
        public void Write(string Message, int IndentSpaces, Boolean enabled)
        {
            if (enabled)
            {
                string indent = "                   ".Substring(0, IndentSpaces);
                switch (useSql)
                {
                    case false:
                        WriteText(indent + Message);
                        break;
                    case true:
                        WriteSql(Message);
                        break;
                }
            }
        }
        public void WriteWithTime(string Message)
        {
            Message = DateTime.Now.ToString() + " :: " + Message;
            Write(Message);
        }
        public void ClearText()
        {
            try
            {
                File.WriteAllText(strPath, String.Empty);
                Write("**** LOG CLEARED ****");
            }
            catch (Exception e)
            {
                Error("Tools.Log.ClearText", e, true);
            }
        }
        public void ClearSessionText()
        {
            try
            {
                File.WriteAllText(strPath, String.Empty);
            }
            catch (Exception e)
            {
                Error("Tools.Log.ClearText", e, true);
            }
        }
        public void ClearSql()
        {
            try
            {
                string qs = "DELETE FROM Log";
                Query q = new Query("Tools.Log.ClearSql", qs, dtVoid, connection, false);
                q.ExecuteNoReturn();
                Write("**** LOG CLEARED ****");
            }
            catch (Exception e)
            {
                Error("Tools.Log.ClearSql", e, true);
            }
        }
        public void Clear()
        {
            switch (useSql)
            {
                case false:
                    ClearText();
                    break;
                case true:
                    ClearSql();
                    break;
            }
        }
        public void WriteDivider()
        {
            Write("--------------------------------------------------");
        }
        public void WriteDiv(bool Enabled = true)
        {
            if (Enabled)
            {
                Write("--------------------------------------------------");
            }
        }
        public void WriteBlank()
        {
            Write("");
        }
        public void WriteHeader()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            WriteDivider();
            Write("Application : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
            Write("Time        : " + DateTime.Now.ToString("h:mm:ss tt"));
            //    Write("Computer    : " + HttpContext.Current.Server.MachineName);
            //    Write("User        : " + System.Web.HttpContext.Current.User.ToString());
            Write("Version     : " + version);
            WriteDivider();
        }
        public DataTable GetTable()
        {
            DataTable dt = new DataTable();
            DataColumn E = dt.Columns.Add("Message");
            DataRow dr;
            string line;
            StreamReader sr = new StreamReader(strPath);
            while ((line = sr.ReadLine()) != null)
            {
                dr = dt.NewRow();
                dr["Message"] = line;
                dt.Rows.Add(dr);
            }
            sr.Close();
            return dt;
        }
        public static void logit(string Message)
        {
            System.IO.Directory.CreateDirectory(@"C:\Temp\");
            Log log = new Log(@"C:\Temp\" + Assembly.GetExecutingAssembly().GetName().Name + ".log");
            log.Write(Message);
        }
        public static void logit(string message, string source)
        {
            Log log = new Log();
            log.Write(source, message);
        }
        public static void logerror(string Source, Exception ex, bool ShowDetails = true)
        {
            System.IO.Directory.CreateDirectory(@"C:\Temp\");
            Log log = new Log(@"C:\Temp\" + Assembly.GetExecutingAssembly().GetName().Name + ".log");
            log.Error(Source, ex, ShowDetails);
        }
        #endregion Methods
    }
    public static class ObjectTools
    {
        public static int ToInt(this object obj)
        {
            int returnInt = 0;
            int parseResult = 0;
            if (obj != null)
            {
                bool parseSucceeded = int.TryParse(obj.ToString(), out parseResult);
                if (parseSucceeded)
                {
                    returnInt = parseResult;
                }
                else
                {
                    returnInt = 0;
                }
            }
            return returnInt;
        }
        public static double ToDouble(this object obj)
        {
            double returnDbl = 0;
            double parseResult = 0;
            if (obj != null)
            {
                bool parseSucceeded = double.TryParse(obj.ToString(), out parseResult);
                if (parseSucceeded)
                {
                    returnDbl = parseResult;
                }
                else
                {
                    returnDbl = 0;
                }
            }
            return returnDbl;
        }
    }
    public class Query
    {
        #region Variables
        public enum DataType { dtVoid, dtString, dtInteger, dtDouble, dtDecimal, dtBoolean, dtDataTable, dtIntList, dtStringList, dtObject, dtGuid };
        DataType enumDataType;
        string strSource;
        string strQuery;
        ServerConnection connSql;
        Boolean isVerbose;

        String fieldName;
        string resultString;
        int resultInteger;
        double resultDouble;
        decimal resultDecimal;
        Boolean resultBoolean;
        object resultObject;
        DataTable resultDataTable;
        List<int> resultIntList;
        List<String> resultStringList;
        Guid resultGuid;

        Log log;
        List<SqlParameter> parameters;
        #endregion Variables
        #region Properties
        public string source
        {
            get
            {
                return strSource;
            }
            set
            {
                strSource = value;
            }
        }
        public string FieldName
        {
            get
            {
                return fieldName;
            }
            set
            {
                fieldName = value;
            }
        }
        public bool IsVerbose
        {
            set
            {
                isVerbose = IsVerbose;
            }
            get
            {
                return isVerbose;
            }

        }
        public string stringResult
        {
            get
            {
                return resultString;
            }
        }
        public int integerResult
        {
            get
            {
                return resultInteger;
            }
        }
        public double doubleResult
        {
            get
            {
                return resultDouble;
            }
        }
        public decimal decimalResult
        {
            get
            {
                return resultDecimal;
            }
        }
        public Boolean booleanResult
        {
            get
            {
                return resultBoolean;
            }

        }
        public object objectResult
        {
            get
            {
                return resultObject;
            }
        }
        public DataTable dataTableResult
        {
            get
            {
                return resultDataTable;
            }
        }
        public List<int> intListResult
        {
            get
            {
                return resultIntList;
            }
        }
        public List<String> stringListResult
        {
            get
            {
                return resultStringList;
            }
        }
        public Guid guidResult
        {
            get
            {
                return resultGuid;
            }
        }
        #endregion Properties
        #region Constructors
        public Query()
        {
            connSql = new ServerConnection();
            ConstructorCommon();
        }
        public Query(ServerConnection Sql, Boolean Verbose)
        {
            isVerbose = Verbose;
            connSql = Sql;
            ConstructorCommon();
        }
        public Query(string Source, ServerConnection Sql, Boolean Verbose)
        {
            strSource = Source;
            isVerbose = Verbose;
            connSql = Sql;

            ConstructorCommon();
        }
        public Query(string Source, string Query, DataType type, Boolean Verbose)
        {
            enumDataType = type;
            strSource = Source;
            strQuery = Query;
            isVerbose = Verbose;
            connSql = new ServerConnection();

            ConstructorCommon();
        }
        public Query(string Source, string Query, DataType type, ServerConnection Sql)
        {
            enumDataType = type;
            strSource = Source;
            strQuery = Query;
            isVerbose = false;
            connSql = Sql;
            ConstructorCommon();
        }
        public Query(string Source, string Query, DataType type, ServerConnection Sql, Boolean Verbose)
        {
            enumDataType = type;
            strSource = Source;
            strQuery = Query;
            isVerbose = Verbose;
            connSql = Sql;
            ConstructorCommon();
        }
        private void ConstructorCommon()
        {
            log = new Tools.Log();
            parameters = new List<SqlParameter>();
        }
        #endregion Constructors
        #region Methods
        public void AddParameter(String Name, SqlDbType Type, String Value)
        {
            try
            {
                // SqlParameter parameter = new SqlParameter(Name, Type, Value);
                //  parameters.Add(parameter);
            }
            catch (Exception ex)
            {
                log.Error("Query.AddParameter", ex, true);
            }
        }
        public int ParameterCount()
        {
            return parameters.Count();
        }
        public void ParameterList()
        {
            foreach (SqlParameter s in parameters)
            {
                //   log.Write("Parameter = > " + s.Name + " = " + s.Value);
            }
        }
        public Boolean Exists(string TableName, string FieldName, string FieldValue)
        {
            enumDataType = dtInteger;
            strSource = "Query.Exists";
            strQuery = "SELECT COUNT(*) FROM " + TableName + " WHERE " + FieldName + " = " + FieldValue;
            Execute();
            try
            {
                return (integerResult != 0);
            }
            catch (Exception ex)
            {
                log.Error("Query.Exists()", ex, true);
                return false;
            }
        }
        public void Update(string TableName, DataTable values, DataTable criteria)
        {
            string ValueString = ValueStringFormat(values, ",");
            string CriteriaString = ValueStringFormat(criteria, "AND");

            strQuery = "UPDATE " + TableName + " SET " + ValueString + " WHERE " + CriteriaString;
            log.WriteText(strQuery);
            ExecuteNoReturn();
        }
        private string ValueStringFormat(DataTable dt, string delimiter)
        {
            string FormattedString = "";
            log.WriteText(dt.Rows.Count.ToString());
            int index = 0;
            int rowCount = dt.Rows.Count;
            foreach (DataRow row in dt.Rows)
            {
                FormattedString = FormattedString + row[0].ToString() + " = '" + row[1].ToString() + "'";
                log.WriteText("index = " + index.ToString());
                if (index < rowCount - 1)
                {
                    FormattedString = FormattedString + " " + delimiter + " ";
                }
                index++;
            }

            return FormattedString;
        }
        public object Result()
        {
            return resultObject;
        }
        private void WriteMessasge(string Message)
        {
            if (isVerbose == true)
            {
                log.Write(Message);
            }
        }
        public void Execute()
        {
            try
            {
                WriteMessasge("Execute START:" + strSource);
                dynamic result = "";
                WriteMessasge("Execute :: " + connSql.ConnectionString);
                SqlConnection conn = connSql.Connection();
                conn.Open();
                SqlCommand cmd = new SqlCommand(strQuery, conn);
                if (enumDataType == DataType.dtDataTable)
                {
                    WriteMessasge("Execute START:" + strSource);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    resultDataTable = new DataTable();
                    sqlDataAdapter.Fill(resultDataTable);
                    WriteMessasge($"Result Count = {resultDataTable.Rows.Count.ToString()}");
                    sqlDataAdapter.Dispose();
                }
                else if (enumDataType == DataType.dtStringList)
                {
                    WriteMessasge("Execute START:" + strSource);
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    resultDataTable = new DataTable();
                    sqlDataAdapter.Fill(resultDataTable);
                    resultStringList = new List<string>();
                    foreach (DataRow dr in resultDataTable.Rows)
                    {
                        resultStringList.Add(dr.ItemArray[0].ToString());
                    }
                    WriteMessasge($"Result Count = {resultDataTable.Rows.Count.ToString()}");
                    sqlDataAdapter.Dispose();
                }
                else if (enumDataType == DataType.dtVoid)
                {
                    int r = 0;
                    WriteMessasge("Execute Void START:" + strSource);
                    WriteMessasge(connSql.ConnectionString);
                    WriteMessasge(strQuery);
                    r = cmd.ExecuteNonQuery();
                    WriteMessasge("Execute Done " + r.ToString());
                }
                else
                {
                    WriteMessasge("NOT DataType.dtDataTable");
                    result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        switch (enumDataType)
                        {
                            case DataType.dtString:
                                resultString = (String)result;
                                break;
                            case DataType.dtInteger:
                                resultInteger = (int)result;
                                break;
                            case DataType.dtBoolean:
                                resultBoolean = (Boolean)result;
                                break;
                            case DataType.dtDouble:
                                resultDouble = (double)result;
                                break;
                            case DataType.dtDecimal:
                                resultDecimal = (decimal)result;
                                break;
                            case DataType.dtObject:
                                resultObject = (object)result;
                                break;
                            case DataType.dtGuid:
                                resultGuid = (Guid)result;
                                break;
                        }
                    }
                }
                conn.Close();
                WriteMessasge("Execute END");
                WriteMessasge("-------------------");
            }
            catch (Exception ex)
            {
                log.Error("Execute :: " + strSource, ex, true);
                log.Write(strQuery);
            }
        }
        public void ExecuteNoReturn()
        {
            WriteMessasge("ExecuteNoReturn START: " + strSource);
            int line = 0;
            try
            {
                line = 1;
                dynamic result = "";
                SqlConnection conn;
                SqlCommand cmd;
                line = 2;
                //WriteMessasge(strQuery);
                //WriteMessasge(connSql.ConnectionString);
                conn = connSql.Connection();
                line = 3;
                conn.Open();
                line = 4;
                cmd = new SqlCommand(strQuery, conn);
                line = 5;
                result = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                log.WriteText("ExecuteNoReturn(" + line.ToString() + ") : " + ex.Message);
                log.WriteText(strQuery);

            }
            WriteMessasge("ExecuteNoReturn END");
        }
        public void UpdateTable(DataTable dt, String TableName)
        {
            Log log = new Tools.Log();
            log.Write("in UpdateTable");
            try
            {
                StringBuilder sb = new StringBuilder();
                String TableKey = "";
                String TableValue = "";
                String FieldKey = "";
                String FieldValue = "";
                String FieldAssignment = "";
                String Delimeter = "";
                String EmptyValue = "";

                foreach (DataRow dr in dt.Rows)
                {
                    sb.Clear();
                    sb.Append(" UPDATE [" + TableName + "]");
                    sb.Append(" SET ");
                    foreach (DataColumn dc in dt.Columns)
                    {
                        //log.Write(dc.ColumnName + " " + dc.DataType.ToString());
                        FieldKey = dc.ColumnName;
                        if (FieldKey != "ID")
                        {
                            FieldValue = dr[dc.ColumnName].ToString();

                            switch (dc.DataType.ToString())
                            {
                                case "System.String":
                                    Delimeter = "'";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "";
                                    }
                                    break;
                                case "System.Int64":
                                    Delimeter = "";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "0";
                                    }
                                    break;
                                case "System.Int32":
                                    Delimeter = "";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "0";
                                    }
                                    break;
                                case "System.Decimal":
                                    Delimeter = "";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "0.00000";
                                    }
                                    break;
                                case "System.Boolean":
                                    Delimeter = "";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "";
                                    }
                                    break;
                            }

                            FieldAssignment = "[" + FieldKey + "] = " + Delimeter + FieldValue.Trim() + Delimeter + ", ";
                            sb.Append(FieldAssignment);
                        }
                        else
                        {
                            TableKey = "ID";
                            TableValue = dr[dc.ColumnName].ToString();
                        }
                    }
                    sb.Remove(sb.Length - 2, 1);
                    sb.Append(" WHERE " + TableKey + " = '" + TableValue + "'");
                    log.Write("(----)" + sb.ToString());
                    strSource = "UpdateQuery";
                    strQuery = sb.ToString();
                    enumDataType = dtVoid;
                    Execute();
                }
            }
            catch (Exception ex)
            {
                log.Error("Query.UpdateTable", ex, true);
            }
        }
        public void InsertTable(String TableName, String FieldName, String FieldValue)
        {
            String queryString = " INSERT INTO [" + TableName + "] ( " + FieldName + ") ";
            queryString = queryString + " VALUES ( '" + FieldValue + "' ) ";

            strQuery = queryString;
            Execute();
        }
        public void InsertTable(DataTable dt, String TableName)
        {
            Log l = new Tools.Log();
            try
            {
                StringBuilder FieldList = new StringBuilder();
                StringBuilder ValueList = new StringBuilder();

                String TableKey = "";
                String TableValue = "";
                String FieldKey = "";
                String FieldValue = "";
                String FieldAssignment = "";
                String ValueAssignment = "";
                String Delimeter = "";
                String EmptyValue = "";

                foreach (DataRow dr in dt.Rows)
                {
                    FieldList.Clear();
                    ValueList.Clear();

                    FieldList.Append(" INSERT INTO [" + TableName + "] (");
                    ValueList.Append(" VALUES (");
                    foreach (DataColumn dc in dt.Columns)
                    {
                        FieldKey = dc.ColumnName;
                        if (FieldKey != "ID")
                        {
                            FieldValue = dr[dc.ColumnName].ToString();

                            switch (dc.DataType.ToString())
                            {
                                case "System.String":
                                    Delimeter = "'";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "";
                                    }
                                    break;
                                case "System.Int64":
                                    Delimeter = "";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "0";
                                    }
                                    break;
                                case "System.Int32":
                                    Delimeter = "";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "0";
                                    }
                                    break;
                                case "System.Decimal":
                                    Delimeter = "";
                                    if (FieldValue == "")
                                    {
                                        FieldValue = "0.00000";
                                    }
                                    break;
                            }

                            FieldAssignment = "[" + FieldKey + "]" + ", ";
                            FieldList.Append(FieldAssignment);
                            ValueAssignment = Delimeter + FieldValue.Trim() + Delimeter + ", ";
                            ValueList.Append(ValueAssignment);
                        }
                        else
                        {
                            TableKey = "ID";
                            TableValue = dr[dc.ColumnName].ToString();
                        }
                    }
                    FieldList.Remove(FieldList.Length - 2, 1);
                    ValueList.Remove(ValueList.Length - 2, 1);
                    FieldList.Append(")");
                    ValueList.Append(")");
                    log.Write(FieldList.ToString() + ValueList.ToString());
                    strQuery = FieldList.ToString() + ValueList.ToString();
                    Execute();

                }
            }
            catch (Exception ex)
            {
                l.Error("Query.InsertTable", ex, true);
            }
        }
        #endregion Methods
    } // public class Query
    public class ServerConnection
    {
        string strComputer;
        string strServer;
        string strInstance;
        string strDatabase;
        string strUserId;
        string strPassword;
        string strQuery;
        string strMessage;
        string strConnectionString;
        Log log;

        // Properties
        public string Server
        {
            get
            {
                return strServer;
            }
            set
            {
                strServer = value;
                strConnectionString = GetConnectionString();
            }
        }
        public string UserId
        {
            get
            {
                return strUserId;
            }
            set
            {
                strUserId = value;
                strConnectionString = GetConnectionString();
            }
        }
        public string Password
        {
            get
            {
                return strPassword;
            }
            set
            {
                strPassword = value;
                strConnectionString = GetConnectionString();
            }
        }
        public string Database
        {
            get
            {
                return strDatabase;
            }
            set
            {
                strDatabase = value;
                strConnectionString = GetConnectionString();
            }
        }
        public string ConnectionString
        {
            get
            {
                return strConnectionString;
            }
            set
            {
                strConnectionString = value;
            }
        }
        public string Query
        {
            get
            {
                return strQuery;
            }
            set
            {
                strQuery = value;
            }
        }
        public string Message
        {
            get
            {
                return strMessage;
            }
        }

        // Constructors
        public ServerConnection()
        {
            strServer = "";
            strInstance = "";
            strDatabase = "";
            strUserId = "";
            strPassword = "";
        }
        public ServerConnection(string ConnectionString)
        {
            strConnectionString = ConnectionString;
        }
        public ServerConnection(string Server, string Instance, string Database, string UserId, string Password)
        {
            strComputer = Server;
            strServer = Server;
            strInstance = Instance;
            strDatabase = Database;
            strUserId = UserId;
            strPassword = Password;
            strConnectionString = GetConnectionString();
            log = new Tools.Log();
        }
        public ServerConnection(string Computer, string Server, string Instance, string Database, string UserId, string Password)
        {
            strComputer = Computer;
            strServer = Server;
            strInstance = Instance;
            strDatabase = Database;
            strUserId = UserId;
            strPassword = Password;
            strConnectionString = GetConnectionString();
            log = new Tools.Log();
        }
        public ServerConnection(string Computer, string Server, string Instance, string Database, string UserId, string Password, string Query)
        {
            strComputer = Computer;
            strServer = Server;
            strInstance = Instance;
            strDatabase = Database;
            strUserId = UserId;
            strPassword = Password;
            strConnectionString = GetConnectionString();
            strQuery = Query;
            log = new Tools.Log();
        }

        // Methods
        public int Test()
        {
            int ReturnCode = 0;
            SqlConnection connTest = new SqlConnection();
            try
            {
                connTest = Connection();
                connTest.Open();
            }
            catch (Exception ex)
            {
                strMessage = ex.Message;
                ReturnCode = 1;
            }
            finally
            {
                connTest.Close();
            }
            return ReturnCode;
        }
        public string GetConnectionString()
        {
            string FormattedInstance = "";
            if (strInstance != "")
            {
                FormattedInstance = "\\" + strInstance;
            }
            return "Server=" + strServer + FormattedInstance + ";Database=" + strDatabase + ";User Id=" + strUserId + ";Password=" + strPassword + ";";
        }
        public SqlConnection Connection()
        {
            SqlConnection conn = new SqlConnection(strConnectionString);
            return conn;
        }
        public SqlCommand Command()
        {
            SqlCommand cmd = new SqlCommand(strQuery, Connection());
            return cmd;
        }
        public dynamic ExecuteScalar()
        {
            SqlCommand cmd = new SqlCommand(strQuery, Connection());
            return cmd.ExecuteScalar();
        }
        public SqlDataAdapter DataAdapter()
        {
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(Command());
                return da;
            }
            catch (Exception ex)
            {
                log.WriteText("ERROR :: " + ex.Message);
                return null;
            }
        }
        public System.Data.DataTable DataTable()
        {
            using (SqlDataAdapter da = new SqlDataAdapter(Command()))
            {
                try
                {
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    da.Dispose();
                    log.WriteText("ERROR :: " + ex.Message);
                    return null;
                }
            }
        }
        public bool ValidConnection()
        {
            try
            {
                SqlConnection conn = new SqlConnection(strConnectionString);
                conn.Open();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(" ValidConnection()", ex, true);
                return false;
            }
        }
    }
    public class SqlColumn
    {
        #region Variables
        string sqlName;
        string displayName;
        object value;
        System.Type datatype;
        Boolean readOnly;
        string sourceTable;
        string sourceColumn;
        #endregion Variables

        #region Properties
        public string SqlName
        {
            get
            {
                return sqlName;
            }
            set
            {
                sqlName = value;
            }
        }
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }
        public System.Type Datatype
        {
            get
            {
                return datatype;
            }
            set
            {
                datatype = value;
            }
        }
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
            }
        }
        public string SourceTable
        {
            get
            {
                return sourceTable;
            }
            set
            {
                sourceTable = value;
            }
        }
        public string SourceColumn
        {
            get
            {
                return sourceColumn;
            }
            set
            {
                sourceColumn = value;
            }
        }
        #endregion Properties

        #region Constructors
        public SqlColumn(string inSqlName)
        {
            sqlName = inSqlName;
            displayName = inSqlName;
            value = "";
            datatype = null;
            readOnly = false;
        }
        public SqlColumn(string inSqlName, string inDisplayName, System.Type inDataType)
        {
            sqlName = inSqlName;
            displayName = inDisplayName;
            value = "";
            datatype = inDataType;
            readOnly = false;
        }
        public SqlColumn(string inSqlName, string inDisplayName, System.Type inDataType, Boolean inReadOnly)
        {
            sqlName = inSqlName;
            displayName = inDisplayName;
            value = "";
            datatype = inDataType;
            readOnly = inReadOnly;
        }
        public SqlColumn(string inSqlName, string inDisplayName, System.Type inDataType, Boolean inReadOnly, string inSourceTable, string inSourceColumn)
        {
            sqlName = inSqlName;
            displayName = inDisplayName;
            value = "";
            datatype = inDataType;
            readOnly = inReadOnly;
            sourceTable = inSourceTable;
            sourceColumn = inSourceColumn;
        }
        public SqlColumn(string inSqlName, string inDisplayName, object inValue, System.Type inDataType)
        {
            sqlName = inSqlName;
            displayName = inDisplayName;
            value = inValue;
            datatype = inDataType;
        }
        #endregion Constructors

        #region Methods
        public List<string> GetSourceValues()
        {
            List<string> values = new List<string>();
            SqlTable table = new SqlTable();
            //table.Connection = MBCTSV2.Reporting.AppTools.AppConnection.GetMbctsConnection();
            table.PhysicalName = sourceTable;
            values = table.GetAll(sourceColumn, false, sourceColumn);
            return values;
        }
        #endregion Methods

    }
    public class SqlField
    {
        public static string StringFormat(string value)
        {
            return Strings.AddQuote(value.ToString().Trim());
        }
    }
    /// <summary>
    /// 
    /// </summary>


    public class SqlTable
    {
        #region Variables
        string displayName;
        string physicalName;
        string keyField;
        string sortField;
        bool readOnly;
        DataTable table;
        bool virtualtable;
        string[] fields;
        List<SqlColumn> fieldList;
        public ServerConnection Connection { get; set; }
        #endregion Variables
        #region Properties
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }
        public string PhysicalName
        {
            get
            {
                return physicalName;
            }
            set
            {
                physicalName = value;
            }
        }
        public string KeyField
        {
            get
            {
                return keyField;
            }
            set
            {
                keyField = value;
            }
        }
        public string SortField
        {
            get
            {
                return sortField;
            }
            set
            {
                sortField = value;
            }
        }
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
            }
        }
        public DataTable Table
        {
            get
            {
                return table;
            }
            set
            {
                table = value;
            }
        }
        public bool Virtualtable
        {
            get
            {
                return virtualtable;
            }
            set
            {
                virtualtable = value;
            }
        }
        public string[] Fields
        {
            get
            {
                return fields;
            }
            set
            {
                fields = value;
            }
        }
        #endregion Properties
        #region Constructors
        public SqlTable()
        {
            fieldList = new List<SqlColumn>();
        }
        #endregion Constructors
        #region Methods
        public void GetAll()
        {
            if (PhysicalName.Trim() != "")
            {
                try
                {
                    string qs = $"SELECT * FROM {PhysicalName}";
                    Query q = new Tools.Query($"SqlTable.GetAll() {DisplayName}", qs, dtDataTable, Connection, false);
                    q.Execute();
                    Table = q.dataTableResult;
                    if (Table.Rows.Count == 0)
                    {
                        logit($"* No rows returned for table {DisplayName}");
                        logit(qs);
                    }
                    else
                    {
                        Table.ToLog($"SqlTable.GetAll() {DisplayName}");
                    }
                }
                catch (Exception ex)
                {
                    logerror("SqlTable.GetAll()", ex);
                }
            }
            else
            {
                logit($"SqlTable.GetAll(): Physical Name missing in {DisplayName}");
            }
        }
        public List<string> GetAll(string columnName, bool ShowQuery, string orderByColumn = "")
        {
            try
            {
                string qs = $"SELECT {columnName} FROM {PhysicalName}";
                if (orderByColumn != "")
                {
                    qs = qs + $" ORDER BY {orderByColumn}";
                }
                Query q = new Tools.Query("SqlTable.GetWhere(criteria)", qs, dtStringList, Connection, false);
                q.Execute();
                if (ShowQuery)
                {
                    logit($"SqlTable.GetWhere(criteria) => {qs}");
                    logit($"- Connection => {Connection.ConnectionString}");
                    logit($"- Count => {q.dataTableResult.Rows.Count.ToString()}");
                }
                return q.stringListResult;
            }
            catch (Exception ex)
            {
                logerror("SqlTable.GetAll(columnName, showQuery, orderByColumn)", ex);
                return null;
            }
        }
        public int GetNext(string valueField, string KeyField, string KeyValue)
        {
            string qs = $"SELECT MAX({valueField}) as 'Max' FROM {PhysicalName}";
            qs = qs + $" WHERE {KeyField.Trim()} = '{KeyValue.Trim()}'";
            Query q = new Tools.Query("SqlTable.GetMax()", qs, dtString, Connection, true);
            q.Execute();
            if (q.stringResult != "" && q.stringResult != null)
            {
                return int.Parse(q.stringResult) + 1;
            }
            else
            {
                return 0;
            }
        }
        public void GetWhere(string KeyField, string KeyValue)
        {
            string qs = $"SELECT * FROM {PhysicalName}";
            qs = qs + $" WHERE {KeyField.Trim()} = '{KeyValue.Trim()}'";
            Query q = new Tools.Query("SqlTable.GetWhere()", qs, dtDataTable, Connection, false);
            q.Execute();
            Table = q.dataTableResult;
        }
        public void GetWhere(string[] KeyField, string[] KeyValue)
        {
            // Not yet implemented
        }
        public DataTable GetWhere(string criteria, bool ShowQuery)
        {
            try
            {
                string qs = $"SELECT * FROM {PhysicalName}";
                qs = qs + $" WHERE {criteria}";
                Query q = new Tools.Query("SqlTable.GetWhere(criteria)", qs, dtDataTable, Connection, false);
                q.Execute();
                if (ShowQuery)
                {
                    logit($"SqlTable.GetWhere(criteria) => {qs}");
                    logit($"- Connection => {Connection.ConnectionString}");
                    logit($"- Count => {q.dataTableResult.Rows.Count.ToString()}");
                }
                Table = q.dataTableResult;
                return q.dataTableResult;
            }
            catch (Exception ex)
            {
                logerror("SqlTable.GetWhere()", ex);
                return null;
            }
        }
        public int GetCount(string criteria, bool ShowQuery)
        {
            try
            {
                string qs = $"SELECT COUNT(*) FROM {PhysicalName}";
                qs = qs + $" WHERE {criteria}";
                Query q = new Tools.Query("SqlTable.GetCount(criteria)", qs, dtInteger, Connection, false);
                q.Execute();
                if (ShowQuery)
                {
                    logit($"SqlTable.GetCount(criteria) => {qs}");
                    logit($"- Connection => {Connection.ConnectionString}");
                    logit($"- Count => {q.dataTableResult.Rows.Count.ToString()}");
                }
                return q.integerResult;
            }
            catch (Exception ex)
            {
                logerror("SqlTable.GetCount()", ex);
                return 0;
            }
        }
        public int GetInt(string columnName, string criteria, bool ShowQuery = false)
        {
            try
            {
                string qs = $"SELECT {columnName} FROM {PhysicalName}";
                qs = qs + $" WHERE {criteria}";
                Query q = new Tools.Query("SqlTable.GetInt(criteria)", qs, dtInteger, Connection, false);
                q.Execute();
                if (ShowQuery)
                {
                    logit($"SqlTable.GetCount(criteria) => {qs}");
                    logit($"- Connection => {Connection.ConnectionString}");
                    logit($"- Count => {q.dataTableResult.Rows.Count.ToString()}");
                }
                return q.integerResult;
            }
            catch (Exception ex)
            {
                logerror("SqlTable.GetCount()", ex);
                return 0;
            }
        }
        public DataTable GetWhere(string columnName, string criteria, bool ShowQuery, string orderByColumn = "")
        {
            try
            {
                string qs = $"SELECT {columnName} FROM {PhysicalName}";
                qs = qs + $" WHERE {criteria}";
                if (orderByColumn != "")
                {
                    qs = qs + $" ORDER BY {orderByColumn}";
                }
                Query q = new Tools.Query("SqlTable.GetWhere(criteria)", qs, dtDataTable, Connection, false);
                q.Execute();
                if (ShowQuery)
                {
                    logit($"SqlTable.GetWhere(criteria) => {qs}");
                    logit($"- Connection => {Connection.ConnectionString}");
                    logit($"- Count => {q.dataTableResult.Rows.Count.ToString()}");
                }
                Table = q.dataTableResult;
                return q.dataTableResult;
            }
            catch (Exception ex)
            {
                logerror("SqlTable.GetWhere(columnName, criteria, showQuery, orderByColumn)", ex);
                return null;
            }
        }
        public bool Exists(string criteria)
        {
            return (GetCount(criteria, false) != 0);
        }
        public void Update(string keyField, string keyValue, string updateField, string updateValue)
        {
            if (ReadOnly)
            {
                logit($"The table {DisplayName} cannot be updated. It is marked as read only");
            }
            else
            {
                string qs = $"UPDATE {PhysicalName} ";
                qs = qs + $" SET {updateField.Trim()} = '{updateValue.Trim()}'";
                qs = qs + $" WHERE {keyField.Trim()} = '{keyValue.Trim()}'";
                Query q = new Tools.Query("SqlTable.Update()", qs, dtDataTable, Connection, false);
                q.Execute();
            }
        }
        public void Update(string criteria, string updateField, string updateValue)
        {
            if (ReadOnly)
            {
                logit($"The table {DisplayName} cannot be updated. It is marked as read only");
            }
            else
            {
                string qs = $"UPDATE {PhysicalName} ";
                qs = qs + $" SET {updateField.Trim()} = '{updateValue.Trim()}'";
                qs = qs + $" WHERE {criteria} ";
                Query q = new Tools.Query("SqlTable.Update(criteria,updateField,updateValue)", qs, dtDataTable, Connection, false);
                q.Execute();
            }
        }
        public void UpdateColumn(string tableName, string criteria, string updateField, string updateValue)
        {
            //  public Guid GetColumnId(string columnName, string keyValue)

            logit("in UpdateColumn");
            criteria = criteria.Replace("WHERE", "");
            if (ReadOnly)
            {
                logit($"The table {DisplayName} cannot be updated. It is marked as read only");
            }
            else
            {
                string qs = $"UPDATE {PhysicalName} ";
                qs = qs + $" SET {updateField.Trim()} = '{updateValue.Trim()}'";
                qs = qs + $" WHERE {criteria} ";
                logit(qs);

                Query q = new Tools.Query("SqlTable.Update(tableName,criteria,updateField,updateValue)", qs, dtVoid, Connection, false);
                q.Execute();
            }
        }
        public void UpdateColumns(string tableName, string criteria, string[] updatecolumns, string[] updateValue, string[] updateTypes)
        {
            StringBuilder qs = new StringBuilder();
            logit("in UpdateColumn");
            criteria = criteria.Replace("WHERE", "");
            if (ReadOnly)
            {
                logit($"The table {DisplayName} cannot be updated. It is marked as read only");
            }
            else
            {
                qs.Append($"UPDATE {PhysicalName} SET ");

                qs.Append($"{UpdateClause(updatecolumns, updateValue, updateTypes)}");
                qs.Append($" WHERE {criteria} ");
                logit(qs.ToString());

                Query q = new Tools.Query("SqlTable.Update(tableName,criteria,updateField,updateValue)", qs.ToString(), dtVoid, Connection, false);
                q.Execute();
            }
        }
        public void Insert(string[] fieldArray, string[] valueArray)
        {
            string qs = "";
            try
            {
                if (ReadOnly)
                {
                    logit($"The table {DisplayName} cannot be updated. It is marked as read only");
                }
                else
                {
                    string fieldString = string.Join(",", fieldArray);
                    string valueString = string.Join(",", valueArray);
                    qs = $"INSERT INTO {PhysicalName} ";
                    qs = qs + $"( {fieldString.Trim()} )";
                    qs = qs + " VALUES ";
                    qs = qs + $"( {valueString.Trim()} )";
                    Query q = new Tools.Query("SqlTable.Insert()", qs, dtVoid, Connection, false);
                    q.Execute();
                }
            }
            catch (Exception ex)
            {
                logerror("SqlTable.Insert", ex);
                logit("SqlTable.Insert " + qs);
            }
        }
        public void Save(string[] fieldArray, string[] valueArray)
        {
            // check for existance
            string qs = "";
            try
            {
                if (ReadOnly)
                {
                    logit($"The table {DisplayName} cannot be updated. It is marked as read only");
                }
                else
                {
                    string fieldString = string.Join(",", fieldArray);
                    string valueString = string.Join(",", valueArray);
                    qs = $"INSERT INTO {PhysicalName} ";
                    qs = qs + $"( {fieldString.Trim()} )";
                    qs = qs + " VALUES ";
                    qs = qs + $"( {valueString.Trim()} )";
                    logit($"Insert {qs}");
                    Query q = new Tools.Query("SqlTable.Insert()", qs, dtVoid, Connection, false);
                    q.Execute();
                }
            }
            catch (Exception ex)
            {
                logerror("SqlTable.Insert", ex);
                logit("SqlTable.Insert " + qs);
            }
        }
        public void Save(string criteria, string[] fieldArray, string[] valueArray)
        {
            // check for existance
            if (ReadOnly)
            {
                logit($"The table {DisplayName} cannot be updated. It is marked as read only");
            }
            else
            {
                if (Exists(criteria))
                {
                    //Update(criteria, fieldArray, valueArray);
                }
                else
                {
                    Insert(fieldArray, valueArray);
                }
            }
            //string qs = "";
            //try
            //{
            //    if (ReadOnly)
            //    {
            //        logit($"The table {DisplayName} cannot be updated. It is marked as read only");
            //    }
            //    else
            //    {
            //        string fieldString = string.Join(",", fieldArray);
            //        string valueString = string.Join(",", valueArray);
            //        qs = $"INSERT INTO {PhysicalName} ";
            //        qs = qs + $"( {fieldString.Trim()} )";
            //        qs = qs + " VALUES ";
            //        qs = qs + $"( {valueString.Trim()} )";
            //        logit($"Insert {qs}");
            //        Query q = new Tools.Query("SqlTable.Insert()", qs, dtDataTable, Connection, false);
            //        q.Execute();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    logerror("SqlTable.Insert", ex);
            //    logit("SqlTable.Insert " + qs);
            //}
        }
        public void Save()
        {
            try
            {

            }
            catch (Exception ex)
            {
                logerror("Save", ex);
            }
        }
        public void Delete(string criteria, bool ShowQuery = false)
        {
            try
            {
                string qs = $"DELETE FROM {PhysicalName}";
                qs = qs + $" WHERE {criteria}";
                Query q = new Tools.Query("SqlTable.Delete(criteria,ShowQuery)", qs, dtVoid, Connection, false);
                q.Execute();
                if (ShowQuery)
                {
                    logit($"SqlTable.Delete(Delete,ShowQuery) => {qs}");
                    logit($"- Connection => {Connection.ConnectionString}");
                }
            }
            catch (Exception ex)
            {
                logerror("SqlTable.Delete()", ex);
            }
        }
        public void AddField(string inName, string inValue, System.Type inDatatype)
        {
            SqlColumn newField = new SqlColumn(inName, inValue, inDatatype);
            fieldList.Add(newField);
        }
        public void ClearFields()
        {
            fieldList.Clear();
        }
        private string UpdateClause(string[] updateColumns, string[] updateValues, string[] updateTypes)
        {
            StringBuilder resultString = new StringBuilder();
            string currentPair = "";
            string quote = "'";
            for (int index = 0; index < updateColumns.Length; index++)
            {
                if (updateTypes[index] == "int")
                {
                    quote = "";
                }
                currentPair = $"{updateColumns[index]} = {quote}{updateValues[index]}{quote}, ";
                resultString.Append(currentPair);
            }
            return resultString.ToString();
        }
        #endregion Methods
    }
    public class Strings
    {
        public static String Left(String Value, int Length)
        {
            if (Length <= Value.Length)
            {
                return Value.Substring(0, Length);
            }
            else
            {
                return "";
            }
        }
        public static double Cdbl(Object obj)
        {
            if (obj != null && obj.ToString() != "")
            {
                return Double.Parse(obj.ToString());
            }
            else
            {
                return 0.0;
            }
        }
        public static string RemoveLastChar(string OrigString, int removeCount)
        {
            string NewString;
            NewString = OrigString.Substring(0, OrigString.Length - removeCount);
            return NewString;
        }
        public static String DecimalFormat(String origString, int DecimalPlaces)
        {

            String returnString = "";
            Log log = new Tools.Log();
            //log.Write("DecimalFormat");
            //log.Write(origString);
            double n;

            if (double.TryParse(origString, out n) == true)
            {
                try
                {
                    //origString = origString.Trim() + ".0";
                    Decimal d = Convert.ToDecimal(origString);
                    Double tempDouble = double.Parse(origString + ".0");
                    //log.Write("df1: " + tempDouble.ToString());
                    //log.Write("df2: " + d.ToString());
                    returnString = tempDouble.ToString("D2");

                    //log.Write("df3: " + returnString);
                    return returnString;
                }
                catch (Exception ex)
                {
                    log.Error("DecimalFormat", ex, true);
                    return origString;
                }
            }
            return null;
        }
        public static String MakeString(object obj)
        {
            Log log = new Log();
            try
            {
                String returnString = "";
                string type = obj.GetType().ToString();
                //log.Write("MakeString " + type);
                switch (type)
                {
                    case "System.String":
                        returnString = obj.ToString().Trim();
                        break;
                    case "System.Decimal":
                        returnString = obj.ToString().Trim();
                        break;
                }
                return returnString;
            }
            catch (Exception ex)
            {
                log.Error("MakeString", ex, true);
                return null;
            }
        }
        public static string CSV(List<string> values)
        {
            StringBuilder builder = new StringBuilder();
            int listLength = values.Count;
            foreach (string str in values)
            {
                builder.Append(str);
                builder.Append(",");
            }
            return RemoveLastChar(builder.ToString(), 1);
        }
        public static string RemoveTrailingChars(string str, int charsToRemove)
        {
            int length = str.Length;
            return str.Substring(length - charsToRemove, charsToRemove);
        }
        public static int[] Split(string str, string delimiter)
        {
            Log log = new Tools.Log();
            try
            {
                int[] pair = { 0, 0 };
                int splitIndex = str.IndexOf(delimiter);
                pair[0] = int.Parse(str.Substring(0, splitIndex));
                pair[1] = int.Parse(str.Substring(splitIndex + 1));
                return pair;
            }
            catch (Exception ex)
            {
                log.Error("Split", ex, true);
                return null;
            }
        }
        public static string Underline(string Value, int length)
        {
            logit($"in Underline: {Value}, {length.ToString()}");
            string underline = "";
            for (int i = 1; i <= length; i++)
            {
                underline = underline + "-";
            }
            return underline;
        }
        public static string AddQuote(string s)
        {
            return $"'{s}'";
        }
        public static string AddBracket(string s)
        {
            return $"[{s}]";
        }
        
    }
    public static class StringTools
    {
        public static double ToDouble(this string origString)
        {
            double result;
            bool tryResult;
            tryResult = double.TryParse(origString, out result);
            if (tryResult)
            {

            }
            else
            {
                result = 0;
                logit("Error Parsing string to double");
            }
            return result;
        }
        public static int ToInt(this string origString)
        {
            int result;
            bool tryResult;
            tryResult = int.TryParse(origString, out result);
            if (tryResult)
            {

            }
            else
            {
                result = 0;
                logit("Error Parsing string to double");
            }
            return result;
        }
        public static string RemoveLastChar(this string origString, int removeCount)
        {
            if (origString.Length > 0)
            {
                string NewString;
                NewString = origString.Substring(0, origString.Length - removeCount);
                return NewString;
            }
            else
            {
                return "";
            }
        }
        public static string Underline(this string value, int length)
        {
            logit($"in Underline: {value}, {length.ToString()}");
            string underline = "";
            for (int i = 1; i <= length; i++)
            {
                underline = underline + "-";
            }
            return underline;
        }
        public static string LastChar(this string OrigString, int returnCount)
        {
            return OrigString.Substring(OrigString.Length - returnCount, returnCount);
        }
        public static string SubFromTo(this string OrigString, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex + 1;
            return OrigString.Substring(startIndex, length);
        }
        public static string Between(this string origString, string startBoundary, string endBoundary)
        {
            int firstPosition = origString.IndexOf(startBoundary) + 1;
            int lastPosition = origString.IndexOf(endBoundary) - 1;
            return origString.SubFromTo(firstPosition, lastPosition);
        }
    }
    public static class SystemTools
    {
        public static string ComputerName()
        {
            return Environment.MachineName.ToString();
        }
    }
    public static class TextFile
    {
        public static string LoadTextFile(string path)
        {
            string file = "";
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        file = sr.ReadToEnd();
                    }
                    return file;
                }
                else
                {
                    //Create();
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void WriteTextFile(string path, string file)
        {
            File.WriteAllText(path, file);
        }
    }
    public static class XmlTools
    {
        public static string RemoveNode(this string xmlDoc, string node)
        {
            node = node.Replace("<","").Replace(">","");
            string nodeHeader = $"<{node}>";
            string nodeFooter = $"</{node}>";
            int nodeStart = xmlDoc.IndexOf(nodeHeader);
            int nodeEnd = xmlDoc.IndexOf(nodeFooter) + nodeFooter.Length;
            string toRemove = xmlDoc.SubFromTo(nodeStart, nodeEnd);
            xmlDoc = xmlDoc.Replace(toRemove, null);
            return xmlDoc;
        }
        public static string RemoveNode(this string xmlDoc, string node, bool singleLine)
        {
            node = node.Replace("<", "").Replace(">", "");
            string nodeHeader = $"<{node}";
            string nodeFooter = ">";
            int nodeStart = xmlDoc.IndexOf(nodeHeader);
            int nodeEnd = xmlDoc.IndexOf(nodeFooter, nodeStart) + 1;
            string toRemove = xmlDoc.SubFromTo(nodeStart, nodeEnd);
            xmlDoc = xmlDoc.Replace(toRemove, "");
            return xmlDoc;
        }
    }
}
