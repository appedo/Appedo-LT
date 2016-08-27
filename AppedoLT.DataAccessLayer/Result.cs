using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using AppedoLT.Core;
using System.IO.Compression;
namespace AppedoLT.DataAccessLayer
{
    public class Result
    {
        private static Result _instance;
        public static Result GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Result();
            }
            return _instance;
        }
        private Result()
        {
        }

        public DataTable GetReportData(string reportName)
        {
            return new ReportMaster(reportName).GetReportData(reportName);
        }

        public DataTable GetReportNameList(string ReportName)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Report name", typeof(string));
            dt.Columns.Add("Timestamp", typeof(string));
            DirectoryInfo dicInfo = new DirectoryInfo(Constants.GetInstance().DataFolderPath);
            foreach (DirectoryInfo info in dicInfo.GetDirectories().OrderByDescending(p => p.CreationTime))
            {
                dt.Rows.Add(info.Name, info.CreationTime.ToShortDateString());                
            }
            return dt;
        }

        public DataTable GetCompareReportList(string scriptName = null)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Report name", typeof(string));
            dt.Columns.Add("Timestamp", typeof(string));
            string directoryPath = Constants.GetInstance().CompareReportsFolderPath;
            if (!string.IsNullOrEmpty(scriptName))
            {
                directoryPath += "\\" + scriptName;
            }
            DirectoryInfo dicInfo = new DirectoryInfo(directoryPath);
            foreach (DirectoryInfo info in dicInfo.GetDirectories().OrderByDescending(p => p.CreationTime))
            {
                if (info.Name != "Current")
                {
                    dt.Rows.Add(info.Name, info.CreationTime.ToShortDateString());
                }
            }
            return dt;
        }

        public DataTable GetChartSummary(string ReportName)
        {
            DataTable dt = new DataTable();
            try
            {
                Constants constants = Constants.GetInstance();
                string file = constants.DataFolderPath + "\\" + ReportName + "\\Report\\" + constants.ChartsSummaryFileName;
                if (File.Exists(file) == true)
                {
                    FileInfo fileInfo=new FileInfo(file);
                    DateTime databaseLaseModifiedTime = new ReportMaster(ReportName).GetRunEndTime();
                    DateTime reportLaseModifiedTime = fileInfo.LastAccessTime;
                    if (databaseLaseModifiedTime.Ticks > reportLaseModifiedTime.Ticks || fileInfo.Length<500)
                    {
                        File.Delete(file);
                        ReportMaster report = new ReportMaster(ReportName);
                        report.SetUserRunTime();
                        report.SetChartSummary();
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                    else
                    {
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                }
                else
                {
                    ReportMaster report = new ReportMaster(ReportName);
                    report.SetUserRunTime();
                    report.SetChartSummary();
                    dt = constants.GetDataTableFromCSVFile(file);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return dt;
        }
   
        public DataTable GetSummaryReport(string ReportName)
        {
            DataTable dt = new DataTable();
            try
            {
                Constants constants = Constants.GetInstance();
                string file = constants.DataFolderPath + "\\" + ReportName + "\\Report\\" + constants.ReportSummayReportFileName;
                if (File.Exists(file) == true)
                {
                    DateTime databaseLaseModifiedTime = new ReportMaster(ReportName).GetRunEndTime();
                    DateTime reportLaseModifiedTime = new FileInfo(file).LastAccessTime;
                    if (databaseLaseModifiedTime.Ticks > reportLaseModifiedTime.Ticks || new FileInfo(file).Length<1000)
                    {
                        File.Delete(file);
                        ReportMaster report = new ReportMaster(ReportName);
                        report.SetReportSummary();
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                    else
                    {
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                }
                else
                {

                    ReportMaster report = new ReportMaster(ReportName);
                    report.SetReportSummary();
                    dt = constants.GetDataTableFromCSVFile(file);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return dt;
        }

        public DataTable GetRequestSummaryReport(string ReportName)
        {
            DataTable dt = new DataTable();
            try
            {
                Constants constants = Constants.GetInstance();
                string file = constants.DataFolderPath + "\\" + ReportName + "\\Report\\" + constants.ReportRequestSummayReportFileName;
                if (File.Exists(file) == true)
                {
                    DateTime databaseLaseModifiedTime = new ReportMaster(ReportName).GetRunEndTime();
                    DateTime reportLaseModifiedTime = new FileInfo(file).LastAccessTime;
                    if (databaseLaseModifiedTime.Ticks > reportLaseModifiedTime.Ticks|| new FileInfo(file).Length<1000)
                    {
                        File.Delete(file);
                        ReportMaster report = new ReportMaster(ReportName);
                        report.SetRequestSummaryReport();
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                    else
                    {
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                }
                else
                {
                    ReportMaster report = new ReportMaster(ReportName);
                    report.SetRequestSummaryReport();
                    dt = constants.GetDataTableFromCSVFile(file);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return dt;

        }

        public DataTable GetPageSummaryReport(string ReportName)
        {
            DataTable dt = new DataTable();
            try
            {
                Constants constants = Constants.GetInstance();
                string file = constants.DataFolderPath + "\\" + ReportName + "\\Report\\" + constants.ReportPageSummayReportFileName;
                if (File.Exists(file) == true)
                {
                    DateTime databaseLaseModifiedTime = new ReportMaster(ReportName).GetRunEndTime();
                    DateTime reportLaseModifiedTime = new FileInfo(file).LastAccessTime;
                    if (databaseLaseModifiedTime.Ticks > reportLaseModifiedTime.Ticks || new FileInfo(file).Length < 1000)
                    {
                        File.Delete(file);
                        ReportMaster report = new ReportMaster(ReportName);
                        report.SetPageSummaryReport();
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                    else
                    {
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                }
                else
                {

                    ReportMaster report = new ReportMaster(ReportName);
                    report.SetPageSummaryReport();
                    dt = constants.GetDataTableFromCSVFile(file);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return dt;

        }

        public DataTable GetContainerSummaryReport(string ReportName)
        {
            DataTable dt = new DataTable();
            try
            {
                Constants constants = Constants.GetInstance();
                string file = constants.DataFolderPath + "\\" + ReportName + "\\Report\\" + constants.ReportContainerSummayReportFileName;
                if (File.Exists(file) == true)
                {
                    DateTime databaseLaseModifiedTime = new ReportMaster(ReportName).GetRunEndTime();
                    DateTime reportLaseModifiedTime = new FileInfo(file).LastAccessTime;
                    if (databaseLaseModifiedTime.Ticks > reportLaseModifiedTime.Ticks || new FileInfo(file).Length < 1000)
                    {
                        File.Delete(file);
                        ReportMaster report = new ReportMaster(ReportName);
                        report.SetContainerSummaryReport();
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                    else
                    {
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                }
                else
                {

                    ReportMaster report = new ReportMaster(ReportName);
                    report.SetContainerSummaryReport();
                    dt = constants.GetDataTableFromCSVFile(file);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return dt;

        }

        public DataTable GetTransactionReport(string ReportName)
        {
            DataTable dt = new DataTable();
            try
            {
                Constants constants = Constants.GetInstance();
                string file = constants.DataFolderPath + "\\" + ReportName + "\\Report\\" + constants.ReportTransactionSummayReportFileName;
                if (File.Exists(file) == true)
                {
                    DateTime databaseLaseModifiedTime = new ReportMaster(ReportName).GetRunEndTime();
                    DateTime reportLaseModifiedTime = new FileInfo(file).LastAccessTime;
                    if (databaseLaseModifiedTime.Ticks > reportLaseModifiedTime.Ticks || new FileInfo(file).Length < 1000)
                    {
                        File.Delete(file);
                        ReportMaster report = new ReportMaster(ReportName);
                        report.SetTransactionSummaryReport();
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                    else
                    {
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                }
                else
                {
                    ReportMaster report = new ReportMaster(ReportName);
                    report.SetTransactionSummaryReport();
                    dt = constants.GetDataTableFromCSVFile(file);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return dt;
        }

        public XmlNode GetSummaryReportByScript(string reportName, XmlNode runNode)
        {
            XmlNode result = null;
            try
            {
                string reportFolder = Constants.GetInstance().DataFolderPath + "\\" + reportName + "\\Report";
                string databasePath = Constants.GetInstance().DataFolderPath + "\\" + reportName;

                // Copy all js files
                Directory.CreateDirectory(reportFolder + "\\js");
                string sourceDirectory = Constants.GetInstance().ExecutingAssemblyLocation + "\\js";
                foreach (string fileName in Directory.GetFiles(sourceDirectory, "*.js"))
                {
                    File.Copy(fileName, reportFolder + "\\js\\" + Path.GetFileName(fileName));
                }

                // Copy all css files
                Directory.CreateDirectory(reportFolder + "\\css");
                sourceDirectory = Constants.GetInstance().ExecutingAssemblyLocation + "\\css";
                foreach (string fileName in Directory.GetFiles(sourceDirectory, "*.css"))
                {
                    File.Copy(fileName, reportFolder + "\\css\\" + Path.GetFileName(fileName));
                }

                using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + databasePath + "\\database.db;Version=3;New=True;Compress=True;"))
                {
                    try
                    {
                        _con.Open();
                        int index = 0;
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><report reportname=\"" + reportName + "\"></report>");
                        XmlNode report = doc.SelectSingleNode("//report");

                        SQLiteDataReader reader = null;

                        XmlNode summaryNode = doc.CreateElement("summaryreport");
                        reader = GetReader("select * from summaryreport", _con);
                        reader.Read();
                        while (reader.HasRows == true)
                        {
                            XmlNode val = summaryNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            summaryNode.AppendChild(val);
                            reader.Read();
                        }
                        report.AppendChild(summaryNode);

                        int count = 0;

                        XmlNode logNode = doc.CreateElement("log");
                        reader = GetReader("select * from log", _con);
                        reader.Read();

                        while (reader.HasRows == true)
                        {
                            if (count > 1000) break;
                            count++;
                            XmlNode val = logNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            logNode.AppendChild(val);
                            reader.Read();
                        }
                        report.AppendChild(logNode);

                        XmlNode errorNode = doc.CreateElement("error");
                        reader = GetReader("select loadgen,reportname,scenarioname,scriptname,containerid,containername, requestid,request,userid,iterationid,errorcode,message,time  from error order by time ", _con);
                        reader.Read();

                        while (reader.HasRows == true)
                        {
                            XmlNode val = errorNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            errorNode.AppendChild(val);
                            reader.Read();
                        }
                        reader.Close();
                        report.AppendChild(errorNode);


                        string[] tables = new string[] { "graphs", "errorgraph", "pageresponsegraph", "vuserrungraph" };
                        foreach (string table in tables)
                        {
                            XmlNode tableNode = doc.CreateElement(table);
                            reader = GetReader(string.Format("select * from {0}", table), _con);
                            reader.Read();
                            while (reader.HasRows == true)
                            {
                                XmlNode val = tableNode.OwnerDocument.CreateElement("val");
                                for (index = 0; index < reader.FieldCount; index++)
                                {
                                    try
                                    {
                                        if (reader[index].GetType() == typeof(DateTime))
                                        {
                                            val.Attributes.Append(GetAttribute(tableNode.OwnerDocument, reader.GetName(index), reader.GetDateTime(index).ToString("yyyy-MM-dd HH:mm:ss")));
                                        }
                                        else
                                        {
                                            val.Attributes.Append(GetAttribute(tableNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                    }
                                }
                                tableNode.AppendChild(val);
                                reader.Read();
                            }
                            report.AppendChild(tableNode);
                        }

                        tables = new string[] { "settings", "requests", "requestresponse", "containerresponse", "errorcount", "errorcode", "transactions" };
                        foreach (XmlNode script in runNode.SelectSingleNode("scripts").ChildNodes)
                        {
                            XmlNode scriptNode = doc.CreateElement("script");
                            scriptNode.Attributes.Append(GetAttribute(doc, "name", script.Attributes["name"].Value));
                            scriptNode.Attributes.Append(GetAttribute(doc, "id", script.Attributes["id"].Value));
                           
                            foreach (string table in tables)
                            {
                                XmlNode tableNode = doc.CreateElement(table);
                                reader = GetReader(string.Format("select * from {0}_{1}", table, script.Attributes["id"].Value), _con);
                                reader.Read();
                                while (reader.HasRows == true)
                                {
                                    XmlNode val = tableNode.OwnerDocument.CreateElement("val");
                                    for (index = 0; index < reader.FieldCount; index++)
                                    {
                                        try
                                        {
                                            if (reader[index].GetType() == typeof(DateTime))
                                            {
                                                val.Attributes.Append(GetAttribute(tableNode.OwnerDocument, reader.GetName(index), reader.GetDateTime(index).ToString("yyyy-MM-dd HH:mm:ss")));
                                            }
                                            else
                                            {
                                                val.Attributes.Append(GetAttribute(tableNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                        }
                                    }
                                    tableNode.AppendChild(val);
                                    reader.Read();
                                }
                                scriptNode.AppendChild(tableNode);
                            }

                            report.AppendChild(scriptNode);
                        }
                        _con.Close();
                        doc.Save(reportFolder + "\\summary.xml");
                        TransformXML(reportFolder);
                       // (new FileInfo(reportFolder + "\\summary.xml").OpenRead()).CopyTo(new GZipStream(File.Create(reportFolder + "\\summary.gz"), CompressionMode.Compress));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public XmlNode GetSummaryReportByScript(string reportName, DataTable runNode)
        {
            XmlNode result = null;
            try
            {
                string reportFolder = Constants.GetInstance().DataFolderPath + "\\" + reportName + "\\Report";
                string databasePath = Constants.GetInstance().DataFolderPath + "\\" + reportName;

                using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + databasePath + "\\database.db;Version=3;New=True;Compress=True;"))
                {
                    try
                    {
                        _con.Open();
                        int index = 0;
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><report reportname=\"" + reportName + "\"></report>");
                        XmlNode report = doc.SelectSingleNode("//report");

                        SQLiteDataReader reader = null;

                        XmlNode summaryNode = doc.CreateElement("summaryreport");
                        reader = GetReader("select * from summaryreport", _con);
                        reader.Read();
                        while (reader.HasRows == true)
                        {
                            XmlNode val = summaryNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            summaryNode.AppendChild(val);
                            reader.Read();
                        }
                        report.AppendChild(summaryNode);

                        XmlNode logNode = doc.CreateElement("log");
                        reader = GetReader("select * from log", _con);
                        reader.Read();
                        while (reader.HasRows == true)
                        {
                            XmlNode val = logNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            logNode.AppendChild(val);
                            reader.Read();
                        }
                        report.AppendChild(logNode);

                        XmlNode errorNode = doc.CreateElement("error");
                        reader = GetReader("select loadgen,reportname,scenarioname,scriptname,er.requestid as requestid, report.address as url ,userid,iterationid,errorcode,message,time  from error as er left join (select requestid,address from reportdata group by requestid,address) as report on er.requestid=report.requestid", _con);
                        reader.Read();
                        while (reader.HasRows == true)
                        {
                            XmlNode val = errorNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            errorNode.AppendChild(val);
                            reader.Read();
                        }
                        report.AppendChild(errorNode);


                        string[] tables = new string[] { "requests", "requestresponse", "requestresponse90", "containerresponse", "containerresponse90", "throughput", "hitcount",  "errorcount", "errorcode", };
                        foreach (DataRow script in runNode.Rows)
                        {
                            XmlNode scriptNode = doc.CreateElement("script");
                            scriptNode.Attributes.Append(GetAttribute(doc, "name", script["scriptname"].ToString()));
                            scriptNode.Attributes.Append(GetAttribute(doc, "id", script["id"].ToString()));

                            foreach (string table in tables)
                            {
                                XmlNode tableNode = doc.CreateElement(table);
                                reader = GetReader(string.Format("select * from {0}_{1}", table, script["id"].ToString()), _con);
                                reader.Read();
                                while (reader.HasRows == true)
                                {
                                    XmlNode val = tableNode.OwnerDocument.CreateElement("val");
                                    for (index = 0; index < reader.FieldCount; index++)
                                    {
                                        val.Attributes.Append(GetAttribute(tableNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                                    }
                                    tableNode.AppendChild(val);
                                    reader.Read();
                                }
                                scriptNode.AppendChild(tableNode);
                            }

                            report.AppendChild(scriptNode);
                        }
                        _con.Close();
                        doc.Save(reportFolder + "\\summary.xml");
                        TransformXMLJmeter(reportFolder);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public XmlNode GetSummaryReportJmeterByScript(string reportName, DataTable runNode)
        {
            XmlNode result = null;
            try
            {
                string reportFolder = Constants.GetInstance().DataFolderPath + "\\" + reportName + "\\Report";
                string databasePath = Constants.GetInstance().DataFolderPath + "\\" + reportName;

                using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + databasePath + "\\database.db;Version=3;New=True;Compress=True;"))
                {
                    try
                    {
                        _con.Open();
                        int index = 0;
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><report></report>");
                        XmlNode report = doc.SelectSingleNode("//report");

                        SQLiteDataReader reader = null;

                        XmlNode summaryNode = doc.CreateElement("summaryreport");
                        reader = GetReader("select * from summaryreport", _con);
                        reader.Read();
                        while (reader.HasRows == true)
                        {
                            XmlNode val = summaryNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            summaryNode.AppendChild(val);
                            reader.Read();
                        }
                        report.AppendChild(summaryNode);

                        //XmlNode logNode = doc.CreateElement("log");
                        //reader = GetReader("select * from log", _con);
                        //reader.Read();
                        //while (reader.HasRows == true)
                        //{
                        //    XmlNode val = logNode.OwnerDocument.CreateElement("val");
                        //    for (index = 0; index < reader.FieldCount; index++)
                        //    {
                        //        val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                        //    }
                        //    logNode.AppendChild(val);
                        //    reader.Read();
                        //}
                        //report.AppendChild(logNode);

                        XmlNode errorNode = doc.CreateElement("error");
                        reader = GetReader("select loadgen,epochtimestamp,address,responsecode,httpresponsemessage,threadgroupname,responsesize from jmeterdata where success='false' or success='FALSE'", _con);
                        reader.Read();
                        while (reader.HasRows == true)
                        {
                            XmlNode val = errorNode.OwnerDocument.CreateElement("val");
                            for (index = 0; index < reader.FieldCount; index++)
                            {
                                val.Attributes.Append(GetAttribute(summaryNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                            }
                            errorNode.AppendChild(val);
                            reader.Read();
                        }
                        report.AppendChild(errorNode);


                        string[] tables = new string[] { "requests", "requestresponse", "requestresponse90", "containerresponse", "containerresponse90", "throughput", "hitcount", "errorcount", "errorcode", };
                        foreach (DataRow script in runNode.Rows)
                        {
                            XmlNode scriptNode = doc.CreateElement("script");
                            scriptNode.Attributes.Append(GetAttribute(doc, "name", script["scriptname"].ToString()));
                            scriptNode.Attributes.Append(GetAttribute(doc, "id", script["id"].ToString()));

                            foreach (string table in tables)
                            {
                                XmlNode tableNode = doc.CreateElement(table);
                                reader = GetReader(string.Format("select * from {0}_{1}", table, script["id"].ToString()), _con);
                                reader.Read();
                                while (reader.HasRows == true)
                                {
                                    XmlNode val = tableNode.OwnerDocument.CreateElement("val");
                                    for (index = 0; index < reader.FieldCount; index++)
                                    {
                                        val.Attributes.Append(GetAttribute(tableNode.OwnerDocument, reader.GetName(index), reader[reader.GetName(index)].ToString()));
                                    }
                                    tableNode.AppendChild(val);
                                    reader.Read();
                                }
                                scriptNode.AppendChild(tableNode);
                            }

                            report.AppendChild(scriptNode);
                        }
                        _con.Close();
                        doc.Save(reportFolder + "\\summary.xml");
                        TransformXMLJmeter(reportFolder);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public XmlNode GenerateCompareReport(string scriptId, string scriptName, string report1, string report2, string report3)
        {
            XmlNode result = null;
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><comparereport></comparereport>");
                XmlNode report = doc.SelectSingleNode("//comparereport");
                report.Attributes.Append(GetAttribute(doc, "script", scriptName));

                XmlNode summaryNode = doc.CreateElement("summaryreport");
                report.AppendChild(summaryNode);

                XmlNode requestresponseNode = doc.CreateElement("requestresponse");
                report.AppendChild(requestresponseNode);

                XmlNode errorNode = doc.CreateElement("errorcount");
                report.AppendChild(errorNode);

                XmlNode containersNode = doc.CreateElement("containers");
                report.AppendChild(containersNode);

                XmlNode transactionsNode = doc.CreateElement("transactions");
                report.AppendChild(transactionsNode);

                for (int i = 0; i < 3; i++)
                {
                    string reportName = report1;
                    if (i == 1)
                    {
                        reportName = report2;
                    }
                    else if (i == 2)
                    {
                        reportName = report3;
                    }

                    if (string.IsNullOrEmpty(reportName))
                        continue;

                    string reportFolder = Constants.GetInstance().DataFolderPath + "\\" + reportName + "\\Report";
                    string databasePath = Constants.GetInstance().DataFolderPath + "\\" + reportName;

                    using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + databasePath + "\\database.db;Version=3;New=True;Compress=True;"))
                    {
                        try
                        {
                            _con.Open();
                            int index = 0;

                            SQLiteDataReader reader = null;
                            #region Summary Report
                            reader = GetReader("select * from summaryreport", _con);
                            reader.Read();
                            while (reader.HasRows == true)
                            {
                                string[] columns = new string[] { "name", "starttime", "endtime", "durationsec", "usercount", "totalhits", "avgresponse", "avghits", "totalthroughput", "avgthroughput", "totalerrors", "totalpage", "avgpageresponse","reponse200", "reponse300", "reponse400", "reponse500" };
                                string[] columnHeading = new string[] { "", "Start Time", "End Time", "Duration(sec)", "User Count", "Total Hits", "Average Request Response(sec)", "Average Hits/Sec", "Total Throughput(MB)", "Average Throughput(Mbps)", "Total Errors", "Total pages", "Average Page Response(sec)", "Response Code 200 Count", "Response Code 300 Count", "Response Code 400 Count", "Response Code 500 Count" };
                                for (index = 0; index < columns.Length; index++)
                                {
                                    try
                                    {
                                        string columnName = columns[index];

                                        XmlNode valNode = null;
                                        if (summaryNode.ChildNodes.Count > 0)
                                        {
                                            valNode = summaryNode.SelectSingleNode(string.Format("//attribute[@name='{0}']", columnName));
                                        }

                                        if (valNode == null)
                                        {
                                            valNode = doc.CreateElement("attribute");
                                            valNode.Attributes.Append(GetAttribute(doc, "name", columnName));
                                            valNode.Attributes.Append(GetAttribute(doc, "displayName", columnHeading[index]));
                                            summaryNode.AppendChild(valNode);
                                        }

                                        XmlNode rprtNode = doc.CreateElement("report");
                                        if (index == 0)
                                        {
                                            rprtNode.Attributes.Append(GetAttribute(doc, "val", reportName));
                                            rprtNode.Attributes.Append(GetAttribute(doc, "align", "center"));
                                        }
                                        else if (reader[columnName].GetType() == typeof(DateTime))
                                        {
                                            rprtNode.Attributes.Append(GetAttribute(doc, "val", ((DateTime)reader[columnName]).ToString("yyyy-MM-dd HH:mm:ss")));
                                            rprtNode.Attributes.Append(GetAttribute(doc, "align", "right"));
                                        }
                                        else if (reader[columnName].GetType() == typeof(Decimal))
                                        {
                                            rprtNode.Attributes.Append(GetAttribute(doc, "val", ((Decimal)reader[columnName]).ToString("#.000")));
                                            rprtNode.Attributes.Append(GetAttribute(doc, "align", "right"));
                                        }
                                        else
                                        {
                                            rprtNode.Attributes.Append(GetAttribute(doc, "val", reader[columnName].ToString()));
                                            rprtNode.Attributes.Append(GetAttribute(doc, "align", "right"));
                                        }

                                        valNode.AppendChild(rprtNode);
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                    }
                                }
                                reader.Read();
                            }
                            reader.Close();
                            #endregion

                            #region Request Response
                            reader = GetReader(string.Format("select * from requestresponse_{0}", scriptId), _con);
                            reader.Read();
                            while (reader.HasRows == true)
                            {
                                XmlNode requestNode = null;
                                if (requestresponseNode.ChildNodes.Count > 0)
                                {
                                    requestNode = requestresponseNode.SelectSingleNode(string.Format("//request[@id='{0}']", reader["requestid"].ToString()));
                                }

                                if(requestNode == null)
                                {
                                    requestNode = doc.CreateElement("request");
                                    requestNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, "id",  reader["requestid"].ToString()));
                                    requestNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, "address",  reader["address"].ToString()));
                                    requestNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, "containername", reader["containername"].ToString()));
                                    requestresponseNode.AppendChild(requestNode);
                                }

                                XmlNode reportNode = doc.CreateElement("report");
                                reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, "name", reportName));
                                requestNode.AppendChild(reportNode);

                                string[] columns = new string[] { "min", "max", "avg", "throughput", "hitcount", "minttfb", "maxttfb", "avgttfb" };                                
                                for (index = 0; index < columns.Length; index++)
                                {
                                    try
                                    {
                                        string columnName = columns[index];
                                        if (reader[columnName].GetType() == typeof(DateTime))
                                        {
                                            reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, columnName, ((DateTime)reader[columnName]).ToString("yyyy-MM-dd HH:mm:ss")));
                                        }
                                        if (reader[columnName].GetType() == typeof(Decimal))
                                        {
                                            reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, columnName, ((Decimal)reader[columnName]).ToString("#.000")));
                                        }
                                        else
                                        {
                                            reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, columnName, reader[columnName].ToString()));
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                    }
                                }
                                reader.Read();
                            }
                            reader.Close();
                            #endregion

                            #region Containers
                            reader = GetReader(string.Format("select * from containerresponse_{0}", scriptId), _con);
                            reader.Read();
                            while (reader.HasRows == true)
                            {
                                XmlNode containerNode = null;
                                if (containersNode.ChildNodes.Count > 0)
                                {
                                    containerNode = containersNode.SelectSingleNode(string.Format("//container[@id='{0}']", reader["containerid"].ToString()));
                                }

                                if (containerNode == null)
                                {
                                    containerNode = doc.CreateElement("container");
                                    containerNode.Attributes.Append(GetAttribute(doc, "id", reader["containerid"].ToString()));
                                    containerNode.Attributes.Append(GetAttribute(doc, "name", reader["containername"].ToString()));
                                    containersNode.AppendChild(containerNode);
                                }

                                XmlNode reportNode = doc.CreateElement("report");
                                reportNode.Attributes.Append(GetAttribute(doc, "name", reportName));
                                containerNode.AppendChild(reportNode);

                                string[] columns = new string[] { "min", "max", "avg" };
                                for (index = 0; index < columns.Length; index++)
                                {
                                    try
                                    {
                                        string columnName = columns[index];
                                        reportNode.Attributes.Append(GetAttribute(doc, columnName, ((double)reader[columnName]).ToString("#.000")));
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                    }
                                }
                                reader.Read();
                            }
                            reader.Close();
                            #endregion

                            #region Transactions
                            reader = GetReader(string.Format("select * from transactions_{0}", scriptId), _con);
                            reader.Read();
                            while (reader.HasRows == true)
                            {
                                XmlNode transactionNode = null;
                                if (transactionsNode.ChildNodes.Count > 0)
                                {
                                    transactionNode = transactionsNode.SelectSingleNode(string.Format("//transaction[@name='{0}']", reader["transactionname"].ToString()));
                                }

                                if (transactionNode == null)
                                {
                                    transactionNode = doc.CreateElement("transaction");
                                    transactionNode.Attributes.Append(GetAttribute(doc, "name", reader["transactionname"].ToString()));
                                    transactionsNode.AppendChild(transactionNode);
                                }

                                XmlNode reportNode = doc.CreateElement("report");
                                reportNode.Attributes.Append(GetAttribute(doc, "name", reportName));
                                transactionNode.AppendChild(reportNode);

                                string[] columns = new string[] { "min", "max", "avg" };
                                for (index = 0; index < columns.Length; index++)
                                {
                                    try
                                    {
                                        string columnName = columns[index];
                                        reportNode.Attributes.Append(GetAttribute(doc, columnName, ((double)reader[columnName]).ToString("#.000")));
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                    }
                                }
                                reader.Read();
                            }
                            reader.Close();
                            #endregion

                            #region Error
                            reader = GetReader(string.Format("select * from errorcode_{0}", scriptId), _con);
                            reader.Read();

                               // rprtNode.Attributes.Append(GetAttribute(doc, "val", reportName));
                               // rprtNode.Attributes.Append(GetAttribute(doc, "align", "center"));

                            while (reader.HasRows == true)
                            {
                                XmlNode errorcodeNode = null;
                                if (errorNode.ChildNodes.Count > 0)
                                {
                                    errorcodeNode = errorNode.SelectSingleNode(string.Format("//error[@code='{0}']", reader["errorcode"].ToString()));
                                }

                                if (errorcodeNode == null)
                                {
                                    errorcodeNode = doc.CreateElement("error");
                                    errorcodeNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, "message", reader["message"].ToString()));
                                    errorcodeNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, "code", reader["errorcode"].ToString()));
                                    errorNode.AppendChild(errorcodeNode);
                                }

                                XmlNode reportNode = doc.CreateElement("report");
                                reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, "name", reportName));
                                errorcodeNode.AppendChild(reportNode);

                                try
                                {
                                    string columnName = "count";
                                    if (reader[columnName].GetType() == typeof(DateTime))
                                    {
                                        reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, columnName, ((DateTime)reader[columnName]).ToString("yyyy-MM-dd HH:mm:ss")));
                                    }
                                    if (reader[columnName].GetType() == typeof(Decimal))
                                    {
                                        reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, columnName, ((Decimal)reader[columnName]).ToString("#.000")));
                                    }
                                    else
                                    {
                                        reportNode.Attributes.Append(GetAttribute(requestresponseNode.OwnerDocument, columnName, reader[columnName].ToString()));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                                }
                                reader.Read();
                            }
                            reader.Close();
                            #endregion

                            _con.Close();
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                        }
                    }
                }
                string compareReportFolder = Constants.GetInstance().CompareReportsFolderPath + "\\Current";
                if (!Directory.Exists(compareReportFolder))
                {
                    Directory.CreateDirectory(compareReportFolder);
                }
                doc.Save(compareReportFolder + "\\summary.xml");

                TransformCompareXML(compareReportFolder);
        
            return result;
        }

        public static void TransformXMLJmeter(string reportFolderPath)
        {
            // Create a resolver with default credentials.
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
            // transform the personnel.xml file to HTML
            XslTransform transform = new XslTransform();
            // load up the stylesheet
            transform.Load(Constants.GetInstance().ExecutingAssemblyLocation + "\\reportjmeter.xslt", resolver);
            // perform the transformation
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\summary.html", resolver);
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\summary.xls", resolver);

        }

        public static void TransformXML(string reportFolderPath)
        {
            // Create a resolver with default credentials.
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
            // transform the personnel.xml file to HTML
            XslTransform transform = new XslTransform();
            // load up the stylesheet
            transform.Load(Constants.GetInstance().ExecutingAssemblyLocation + "\\report.xslt", resolver);
            // perform the transformation
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\summary.html", resolver);
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\summary.xls", resolver);

            transform.Load(Constants.GetInstance().ExecutingAssemblyLocation + "\\log.xslt", resolver);
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\log.html", resolver);

            transform.Load(Constants.GetInstance().ExecutingAssemblyLocation + "\\error.xslt", resolver);
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\error.html", resolver);
        }

        public static void TransformCompareXML(string reportFolderPath)
        {
            // Create a resolver with default credentials.
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;
            // transform the personnel.xml file to HTML
            XslTransform transform = new XslTransform();
            // load up the stylesheet
            transform.Load(Constants.GetInstance().ExecutingAssemblyLocation + "\\comparereport.xslt", resolver);
            // perform the transformation
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\summary.html", resolver);
            transform.Transform(reportFolderPath + "\\summary.xml", reportFolderPath + "\\summary.xls", resolver);
        }

        public DataTable GetAvgUsers(string newReportName)
        {
            DataTable dt = new DataTable();
            try
            {
                Constants constants = Constants.GetInstance();
                string file = constants.DataFolderPath + "\\" + newReportName + "\\Report\\" + constants.ChartsAvgResponse;
                if (File.Exists(file) == true)
                {
                    DateTime databaseLaseModifiedTime = new FileInfo(constants.DataFolderPath + "\\" + newReportName + "\\database.db").LastWriteTime;
                    DateTime reportLaseModifiedTime = new FileInfo(file).LastAccessTime;
                    if (databaseLaseModifiedTime.Ticks > reportLaseModifiedTime.Ticks)
                    {
                        File.Delete(file);
                        ReportMaster report = new ReportMaster(newReportName);
                        report.SetUserAvgResponse();
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                    else
                    {
                        dt = constants.GetDataTableFromCSVFile(file);
                    }
                }
                else
                {
                    ReportMaster report = new ReportMaster(newReportName);
                    report.SetUserAvgResponse();
                    dt = constants.GetDataTableFromCSVFile(file);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            return dt;
        
        }
    
        private SQLiteDataReader GetReader(string query, SQLiteConnection _con)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, _con);
            return cmd.ExecuteReader();
        }

        private XmlAttribute GetAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
    }

    public class ResultLogMonitor
    {
        public Queue<string> monitorData = new Queue<string>();
        public StreamWriter monitorDataFile = null;
        private static ResultLogMonitor _instance;
        public static ResultLogMonitor GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ResultLogMonitor();
                _instance.InsertToDB();
                _instance.monitorDataFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\monitor.txt");
            }
            return _instance;
        }
        private ResultLogMonitor()
        {
          
        }
        public void LogResult(string counterid, string sample, string vallue)
        {
            try
            {
                lock (monitorData)
                {
                    monitorData.Enqueue(new StringBuilder().AppendFormat("{0},{1},{2}", counterid, sample, vallue).ToString()); ;
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        private void InsertToDB()
        {
            new Thread(() =>
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                do
                {
                    try
                    {
                        if (monitorDataFile != null )
                        {
                           
                            if (monitorData.Count > 0)
                            {
                                try
                                {
                                    int currentReportData = monitorData.Count;
                                    StringBuilder temp = new StringBuilder();
                                    for (int index = 0; index < currentReportData; index++)
                                    {
                                        temp.AppendLine(monitorData.Dequeue());
                                    }
                                    if (temp.Length > 0) monitorDataFile.Write(temp.ToString());
                                    monitorDataFile.Flush();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.Message);
                                }
                            }

                            if (timer.Elapsed.Seconds >= 5)
                            {
                                #region Write to database
                                if (monitorDataFile.BaseStream.Length > 0)
                                {
                                    long reportDataLength = monitorDataFile.BaseStream.Length;
                                    monitorDataFile.Close();
                                    try
                                    {
                                        Constants.GetInstance().ExecuteBat("execute_monitor.bat");
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    }
                                    finally
                                    {
                                        using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\monitor.txt", FileMode.Truncate)) { };
                                        monitorDataFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\monitor.txt");
                                    }

                                }
                                timer.Reset();
                                timer.Start();
                                #endregion
                            }
                        }
                        if (monitorData.Count == 0)
                        {
                            System.Threading.Thread.Sleep(5000);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                } while (true);
            }).Start();
        }
        public XmlAttribute GetAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            return att;
        }
        public DataTable GetReportName()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("reportname");
            DirectoryInfo dicInfo = new DirectoryInfo(Constants.GetInstance().DataFolderPathMonitor);
            foreach (DirectoryInfo info in dicInfo.GetDirectories().OrderByDescending(p => p.CreationTime))
            {
                dt.Rows.Add(info.Name);
            }

            DataRow drSelect = dt.NewRow();
            drSelect["reportname"] = "-Select-";
            dt.Rows.InsertAt(drSelect, 0);
            return dt;
        }
        public DataTable GetCounterData(string reportname,string counterid)
        {
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + Constants.GetInstance().DataFolderPathMonitor + "\\" + reportname + "\\databasemonitor.db;Version=3;New=True;Compress=True;"))
            {
                DataTable dt = new DataTable();
                try
                {
                    SQLiteDataAdapter data = new SQLiteDataAdapter("select sample,value from monitor where counterid=" + counterid, _con);
                    data.Fill(dt);

                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
                return dt;
            }
        }

    }
}
