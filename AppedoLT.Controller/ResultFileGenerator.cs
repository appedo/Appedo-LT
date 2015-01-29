using AppedoLT.Core;
using AppedoLT.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;

namespace AppedoLTController
{
    class ResultFileGenerator
    {
        public static object _lockObj = new object();
        string _runid = string.Empty;
        string _sourceIp = string.Empty;
        Constants _constants = Constants.GetInstance();
        int _index = 0;
        public ResultFileGenerator(string runtid)
        {
            _runid = runtid;
            
           _sourceIp= ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + _runid + "']").Attributes["sourceip"].Value;
        }
        public void Genarate()
        {
            ReceiveAllLoadGenDatafiles();
            if (ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + _runid + "']").SelectNodes("loadgen[@resultfilereceived='False' and @runstarted='True']").Count == 0)
            {
                File.Copy(Constants.GetInstance().ExecutingAssemblyLocation + "\\database.db",Constants.GetInstance().ExecutingAssemblyLocation +"\\data\\"+ _runid + "\\database.db", true);
                CreateSummaryReport(_runid);
                ExceptionHandler.LogRunDetail(_runid, "Summary report genetated");
                ReportMaster reportMaster = new ReportMaster(_runid);
                reportMaster.SetUserRunTime();
                reportMaster.GenerateReports();
                ExceptionHandler.LogRunDetail(_runid, "Summary chart genetated");
                ReceiveAllChartData(_runid);
                SendResultFile(_runid);
            }
        }
        private void ReceiveAllLoadGenDatafiles()
        {
            try
            {
                string ipAddress = string.Empty;
                string filePath;
                int readCount = 0;

                for (_index = 0; _index < 20; _index++)
                {
                    XmlNodeList result = ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + _runid + "']").SelectNodes("loadgen[@resultfilereceived='False' and @runstarted='True']");
                    if (result.Count == 0) break;
                    else Thread.Sleep(2000);
                    foreach (XmlNode loadGen in result)
                    {
                        ipAddress = loadGen.Attributes["ipaddress"].Value;
                        filePath = _constants.ExecutingAssemblyLocation + "\\Data\\" + _runid + "\\database_" + ipAddress.Replace('.', '_') + ".db";
                        readCount = 0;
                        try
                        {
                            TcpClient clt = new TcpClient();
                            clt.Connect(IPAddress.Parse(ipAddress), 8889);
                            clt.Client.Send(Encoding.ASCII.GetBytes("resultfile: 0\r\nreportname= " + _runid + "\r\n\r\n"));
                            NetworkStream stream = clt.GetStream();
                          
                            string header = _constants.ReadHeader(stream);
                            string operation = new Regex("(.*): ([0-9]*)").Match(header).Groups[1].Value;
                            string streamlengthStr = new Regex("(.*): ([0-9]*)").Match(header).Groups[2].Value;
                            long streamLength = Convert.ToInt64(streamlengthStr);
                            byte[] buffer = new byte[8192];
                            if (File.Exists(filePath)) File.Delete(filePath);
                            using (FileStream file = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite))
                            {
                                while (streamLength > 0)
                                {

                                    if (streamLength < buffer.Length)
                                    {
                                        readCount = stream.Read(buffer, 0,Convert.ToInt32(streamLength));
                                    }
                                    else
                                    {
                                        readCount = stream.Read(buffer, 0, buffer.Length);
                                    }
                                    streamLength = streamLength - readCount;
                                    file.Write(buffer, 0, readCount);
                                }
                            }
                            if (streamLength == 0)
                            {
                                loadGen.Attributes["resultfilereceived"].Value = true.ToString();
                                ControllerXml.GetInstance().Save();
                                ExceptionHandler.LogRunDetail(_runid, "Received data file from " + ipAddress);
                                clt.Client.Send(Encoding.ASCII.GetBytes("ok: 0\r\nreceivedsize= " + streamlengthStr + "\r\n\r\n"));
                            }
                            clt.Client.Close();
                            buffer = null;
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void CreateSummaryReport(string reportName)
        {
            try
            {
                ReportMaster mas = new ReportMaster(reportName);
                mas.Executequery(reportName, Constants.GetInstance().GetQuery(reportName, ControllerXml.GetInstance().doc));
                XmlNode runNode = ControllerXml.GetInstance().doc.SelectSingleNode("//run[@reportname='" + reportName + "']");
                Result.GetInstance().GetSummaryReportByScript(reportName, runNode);
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }
      
        void SendResultFile(string runid)
        {
            Trasport server = null;
            string chartSummaryFile = _constants.ExecutingAssemblyLocation + "\\Data\\" + runid + "\\Report\\chart_ summary.csv";
            string reportSummaryFile = _constants.ExecutingAssemblyLocation + "\\Data\\" + runid + "\\Report\\summary.xml";
            TrasportData response = null;
            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("runid", runid);

            if (File.Exists(reportSummaryFile) == true )
            {
                server = new Trasport(_sourceIp, Constants.GetInstance().AppedoPort);
                for (_index = 0; _index < 20; _index++)
                {
                    try
                    {
                        response = new TrasportData("summaryreport", header, reportSummaryFile);
                        server.Send(response);
                        response = server.Receive();
                        ExceptionHandler.LogRunDetail(_runid, "Summaryreport sent successfully.");
                        break;
                    }
                    catch { }
                }
                if (File.Exists(chartSummaryFile) == true)
                {
                    for (_index = 0; _index < 20; _index++)
                    {
                        try
                        {
                            response = new TrasportData("result", header, chartSummaryFile);
                            server = new Trasport(_sourceIp, Constants.GetInstance().AppedoPort);
                            server.Send(response);
                            response = server.Receive();
                            ExceptionHandler.LogRunDetail(_runid, "result sent successfully.");
                            break;
                        }
                        catch { }
                    }
                    if (File.Exists(reportSummaryFile) == true)
                    {
                        if (ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + runid + "']") != null)
                        {
                            ControllerXml.GetInstance().doc.SelectSingleNode("//runs").RemoveChild(ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + runid + "']"));
                            ControllerXml.GetInstance().Save();
                            ExceptionHandler.LogRunDetail(_runid, "node deleted successfully.");
                        }
                        DeleteReportFolder(runid);
                    }
                    for (_index = 0; _index < 20; _index++)
                    {
                        try
                        {
                            response = new TrasportData("log", ExceptionHandler.GetLog(_runid),header);
                            server = new Trasport(_sourceIp, Constants.GetInstance().AppedoPort);
                            server.Send(response);
                            response = server.Receive();
                            ExceptionHandler.RunDetaillog.Remove(_runid);
                            break;
                        }
                        catch { }
                    }
                }
            }
            else
            {
                server = new Trasport(_sourceIp, Constants.GetInstance().AppedoPort);
                response = new TrasportData("error", "Unable to get report", header);
                server.Send(response);
                
            }
        }
        private string DeleteReportFolder(string reportname)
        {
            string folderPath = Constants.GetInstance().ExecutingAssemblyLocation + "\\Data\\" + reportname;
            try
            {
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                    ExceptionHandler.LogRunDetail(_runid, "Directory deleted successfully.");
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            return folderPath;
        }
        private void UpdateReportStatus()
        {
            try
            {
                double totalPer = ReportMaster.Status.Count * 100;
                double competedPer = 0;
                while (totalPer > competedPer)
                {
                    competedPer = 0;
                    foreach (string key in ReportMaster.Status.Keys)
                    {
                        competedPer += ReportMaster.Status[key].Percentage;
                    }
                    try
                    {
                        //lblStatus.Text = string.Format("Report Generating. {0}% Completed.", Convert.ToInt32((competedPer / totalPer) * 100));
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                }
                string filepath = _constants.ExecutingAssemblyLocation + "\\Data\\" + _runid + "\\Report\\chart_ summary.csv";
                FileInfo info = new FileInfo(filepath);

                int count = 10;
                while (count > 0)
                {
                    if (info.Length < 10)
                    {
                        Thread.Sleep(1000);
                        count--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            finally
            {
                Thread.Sleep(10000);
                ReceiveAllChartData(_runid);

            }

        }
        private void ReceiveAllChartData(string reportName)
        {
            try
            {
                string filePath;
                Dictionary<string, string> header = new Dictionary<string, string>();
                header.Add("reportname", reportName);
                filePath = _constants.ExecutingAssemblyLocation + "\\Data\\" + reportName + "\\Report\\chart_ summary.csv";
                if (File.Exists(filePath) == false) File.Create(filePath);
                using (FileStream stream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    for (_index = 0; _index < 10; _index++)
                    {
                        XmlNodeList result = ControllerXml.GetInstance().doc.SelectSingleNode("//runs/run[@reportname='" + reportName + "']").SelectNodes("loadgen[@chartresultfilereceived='False']");
                        if (result.Count == 0) break;
                        else Thread.Sleep(2000);
                        foreach (XmlNode loadGen in result)
                        {
                            if (loadGen.Attributes["chartresultfilereceived"].Value == "False")
                            {
                                try
                                {
                                    Trasport trasport = new Trasport(loadGen.Attributes["ipaddress"].Value, "8889", 60000);
                                    trasport.Send(new TrasportData("chartsummary", string.Empty, header));
                                    TrasportData data = trasport.Receive();
                                    if (data.Operation == "file")
                                    {
                                        stream.Write(data.DataStream.ToArray(), 0, Convert.ToInt32(data.DataStream.Length));
                                    }
                                    loadGen.Attributes["chartresultfilereceived"].Value = true.ToString();
                                    ExceptionHandler.LogRunDetail(_runid, "Chart file is received from " + loadGen.Attributes["ipaddress"].Value);
                                    ControllerXml.GetInstance().Save();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
    }
}
