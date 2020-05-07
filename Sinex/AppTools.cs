using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;
using static Tools.Log;
using static Tools.StringTools;
using static Tools.Query.DataType;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace Sinex
{
    public class AppTools
    {
        static string applicationPath = @"C:\Sinex\";
        static string configFile = "Application.config";
        public class AppConfigFile : ConfigFile
        {
            public AppConfigFile() : base()
            {
                try
                {
                    string[] keyArray = { "GPSERVER", "GPDATABASE", "GPUSERNAME", "GPPASSWORD", "MODSERVER", "MODDATABASE", "MODUSERNAME", "MODPASSWORD" };
                    this.Path = applicationPath + configFile;
                    this.Delimeter = '=';
                    this.Comment = "#";
                    this.keyArray = keyArray;
                    int loadStatus = Load();
                    if (loadStatus == 1)
                    {
                        logit("ERROR: The config file did not exist. Creating a new one.");
                    }
                }
                catch (Exception ex)
                {
                    logerror("AppConfigFile()", ex, true);
                }
            }
            public static string GetValue(string Key, Boolean ToLower = false)
            {
                try
                {
                    ConfigFile config = new Tools.ConfigFile();
                    string[] keyArray = { "SERVER", "DATABASE", "USERNAME", "PASSWORD" };
                    config.Path = applicationPath + "Application.config";
                    config.Delimeter = '=';
                    config.Comment = "#";
                    config.keyArray = keyArray;
                    config.Load();
                    if (ToLower == true)
                    {
                        return config.Item(Key).ToLower();
                    }
                    else
                    {
                        return config.Item(Key);
                    }
                }
                catch (Exception ex)
                {
                    logerror("AppConfigFile.GetValue()", ex, true);
                    return "";
                }
            }
        }
        public class AppLog : Log
        {

            #region Variables
            static string logName = "Sinex.log";
            static bool includeTime = true;
            static bool useSql = true;
            #endregion Variables

            #region Properties
            #endregion Properties

            #region Constructors
            public AppLog() : base(AppTools.AppConnection.GetLogConnection())
            {
                includeTime = true;
                appVersion = AppVersion();
            }
            public AppLog(bool useSql, string logPath = @"C:\Temp\Sinex.log") : base()
            {
                includeTime = true;
                appVersion = AppVersion();
                if (useSql)
                {
                    type = 1;
                    connection = AppConnection.GetLogConnection();
                }
                else
                {
                    type = 0;
                    Path = logPath;
                }
            }
            #endregion Constructors

            #region Methods
            public void ClearLog()
            {
                Log log = new Tools.Log(applicationPath + logName);
                log.Clear();
            }
            public static new void logit(string message, string source = "")
            {
                if (message != "")
                {
                    Log SystemLog = new Log();
                    Log log = new Tools.Log(applicationPath + logName);
                    log.IncludeTime = includeTime;
                    log.useSql = true;
                    log.connection = AppTools.AppConnection.GetLogConnection();
                    log.appVersion = AppVersion();
                    //log.appUser = AppUser();
                    switch (message)
                    {
                        case "DIV":
                        case "DIVIDER":
                            log.WriteDivider();
                            break;
                        case "BLANK":
                        case "SPACE":
                            log.WriteBlank();
                            break;
                        case "CLEAR":
                        case "NEW":
                            log.Clear();
                            break;
                        default:
                            log.Write(source, message);
                            break;
                    }
                }
            }
            public static void logit(string message, int indentSpaces, bool writeLog = true)
            {
                if (message != "")
                {
                    Log log = new Tools.Log(applicationPath + logName);
                    log.Write(message, indentSpaces, writeLog);
                }
            }
            public static void logdebug(string message, string source = "")
            {
                logit(message, source);
            }
            public static void logdebug(string message, string lineNumber, bool writeLog = true)
            {
                logdebug($"{message} ({lineNumber})");
            }
            public static new void logerror(string Source, Exception ex, bool ShowDetails = true)
            {
                Log log = new Tools.Log(applicationPath + logName);
                logerror(Source, ex, ShowDetails);
            }
            public static string LogPath()
            {
                return applicationPath + logName;
            }
            public static void logArray(string source, string[] messages, bool writeLog = true)
            {

                logit("-");
                logit("-");
                logit("Log Array = " + source.Trim());
                //logit(Underline(source.Trim(), source.Trim().Length));
                foreach (string s in messages)
                {
                    logit($"- {s}");
                }
                logit("");
            }
            #endregion Methods
        }
        public class AppConnection : ServerConnection
        {
            public static ServerConnection GetGPConnection()
            {
                AppConfigFile config = new AppConfigFile();
                ServerConnection connection = new ServerConnection();
                connection.Server = config.Item("GPSERVER");
                connection.Database = config.Item("GPDATABASE");
                connection.UserId = config.Item("GPUSERNAME");
                connection.Password = config.Item("GPPASSWORD");
                int connectionTest = connection.Test();
                if (connectionTest == 0)
                {
                    return connection;
                }
                else
                {
                    return null;
                }
            }
            public static ServerConnection GetModConnection()
            {
                AppConfigFile config = new AppConfigFile();
                ServerConnection connection = new ServerConnection();
                connection.Server = config.Item("MODSERVER");
                connection.Database = config.Item("MODDATABASE");
                connection.UserId = config.Item("MODUSERNAME");
                connection.Password = config.Item("MODPASSWORD");
                int connectionTest = connection.Test();
                //logdebug($"GetModConnection() Test= {connectionTest}, string= {connection.ConnectionString}");
                if (connectionTest == 0)
                {
                    return connection;
                }
                else
                {
                    logit($"ERROR: Connection Test Failed. Server={connection.Server}");
                    return null;
                }
            }
            public static ServerConnection GetLogConnection()
            {
                AppConfigFile config = new AppConfigFile();
                ServerConnection connection = new ServerConnection();
                connection.Server = config.Item("MODSERVER");
                connection.Database = config.Item("MODDATABASE");
                connection.UserId = config.Item("MODUSERNAME");
                connection.Password = config.Item("MODPASSWORD");
                int connectionTest = connection.Test();
                if (connectionTest == 0)
                {
                    return connection;
                }
                else
                {
                    logit($"ERROR: Connection Test Failed. Server={connection.Server}");
                    return null;
                }
            }
        }
        public static DataTable GetDataTable(string table, string selectClause, string whereClause, string orderByClause)
        {
            Log log = new Log();
            log.Write("in GetDataTable()");
            //String CompanyId = Dynamics.Globals.IntercompanyId.Value.ToString();
            ServerConnection Connection = AppConnection.GetGPConnection();
            try
            {
                string qs = selectClause + " FROM " + table;
                if (whereClause != "")
                {
                    qs = qs + " WHERE " + whereClause;
                }
                if (orderByClause != "")
                {
                    qs = qs + " ORDER BY " + orderByClause;
                }
                log.Write("GetDataTable():" + qs);
                Query q = new Query("GetDataTable " + table, qs, dtDataTable, Connection, false);
                q.Execute();
                return q.dataTableResult;
            }
            catch (Exception ex)
            {
                logerror("GetDataTable", ex, true);
                return null;
            }
        }
        public static string AppVersion()
        {
            return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        }
    }
    public static class IndexSearchCheckbox
    {
        public static CheckBoxVisualSwitch CreateVisualSwitch(CheckBox checkBox)
        {
            string checkedValue = "Results Returned";
            string uncheckedValue = "Index Search";
            CheckBoxVisualSwitch cbvs = new CheckBoxVisualSwitch(checkBox, checkedValue, uncheckedValue);
            cbvs.FontColor = System.Drawing.Color.Black;
            cbvs.UncheckedColor = System.Drawing.Color.LightPink;
            cbvs.CheckedColor = System.Drawing.Color.LightGreen;
            return cbvs;
        }
        public static void Switch(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).Switch();
        }
        public static void SetChecked(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).SetChecked();
        }
        public static void SetUnchecked(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).SetUnchecked();
        }
    }
    public static class TableSearchCheckbox
    {
        public static CheckBoxVisualSwitch CreateVisualSwitch(CheckBox checkBox)
        {
            string checkedValue = "Results Returned";
            string uncheckedValue = "Table Search";
            CheckBoxVisualSwitch cbvs = new CheckBoxVisualSwitch(checkBox, checkedValue, uncheckedValue);
            cbvs.FontColor = System.Drawing.Color.Black;
            cbvs.UncheckedColor = System.Drawing.Color.LightPink;
            cbvs.CheckedColor = System.Drawing.Color.LightGreen;
            return cbvs;
        }
        public static void Switch(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).Switch();
        }
        public static void SetChecked(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).SetChecked();
        }
        public static void SetUnchecked(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).SetUnchecked();
        }
    }
    public static class AndOrSearchCheckbox
    {
        public static CheckBoxVisualSwitch CreateVisualSwitch(CheckBox checkBox)
        {
            string checkedValue = "or";
            string uncheckedValue = "and";
            CheckBoxVisualSwitch cbvs = new CheckBoxVisualSwitch(checkBox, checkedValue, uncheckedValue);
            cbvs.FontColor = System.Drawing.Color.Black;
            cbvs.UncheckedColor = System.Drawing.Color.Silver;
            cbvs.CheckedColor = System.Drawing.Color.Silver;
            return cbvs;
        }
        public static void Switch(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).Switch();
        }
        public static void SetChecked(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).SetChecked();
        }
        public static void SetUnchecked(CheckBox checkBox)
        {
            CreateVisualSwitch(checkBox).SetUnchecked();
        }
    }
    //public class BusinessObject
    //{
    //    #region Variables
    //    string name;
    //    SQLView view;
    //    #endregion Variables

    //    #region Properties
    //    public BusinessObject()
    //    {

    //    }
    //    #endregion Properties

    //    #region Constructors
    //    #endregion Constructors

    //    #region Methods
    //    #endregion Methods

    //}
    //public class boFed : BusinessObject
    //{

    //}

}

