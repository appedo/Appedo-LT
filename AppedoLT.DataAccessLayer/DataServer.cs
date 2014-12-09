using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace AppedoLT.DataAccessLayer
{
    public class DataServer
    {

        public object DataBaseLock = new object();

        public Queue<ReportData> reportDT = new Queue<ReportData>();
        public Queue<TransactionRunTimeDetail> transcations = new Queue<TransactionRunTimeDetail>();
        public Queue<RequestException> errors = new Queue<RequestException>();
        public Queue<Log> logs = new Queue<Log>();
        public ExecutionReport Status = ExecutionReport.GetInstance();

        public StreamWriter reportDataFile = null;
        public StreamWriter errorFile = null;
        public StreamWriter transactionFile = null;
        public StreamWriter logsFile = null;

        private static DataServer _instance;

        private DataServer()
        {
            reportDataFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\out.txt");
            errorFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\error.txt");
            transactionFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\transaction.txt");
            logsFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\log.txt");
        }
        public static DataServer GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DataServer();
                _instance.Start();
            }
            return _instance;
        }
        private void Start()
        {
            new Thread(() =>
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                do
                {
                    try
                    {
                        if (reportDataFile != null && errorFile != null && transactionFile != null)
                        {
                            #region Enque
                            if (reportDT.Count > 0)
                            {
                                try
                                {
                                    int currentReportData = reportDT.Count;
                                    StringBuilder temp = new StringBuilder();
                                    for (int index = 0; index < currentReportData; index++)
                                    {
                                        ReportData rt = reportDT.Dequeue();
                                        if (rt != null && rt.starttime.Year != 1 && rt.endtime.Year != 1)
                                        {
                                            temp.AppendLine(rt.ToString());
                                        }
                                        rt = null;
                                    }
                                    if (temp.Length > 0) reportDataFile.Write(temp.ToString());
                                    reportDataFile.Flush();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.Message);
                                }
                            }

                            if (errors.Count > 0)
                            {
                                try
                                {
                                    int errorsData = errors.Count;
                                    StringBuilder temp = new StringBuilder();

                                    for (int index = 0; index < errorsData; index++)
                                    {
                                        RequestException runException = errors.Dequeue();
                                        if (runException != null)
                                        {
                                            temp.AppendLine(runException.ToString());
                                        }

                                        runException = null;
                                    }
                                    if (temp.Length > 0) errorFile.Write(temp.ToString());
                                    errorFile.Flush();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.Message);
                                }
                            }

                            if (logs.Count > 0)
                            {
                                try
                                {
                                    int logData = logs.Count;
                                    StringBuilder temp = new StringBuilder();
                                    for (int index = 0; index < logData; index++)
                                    {
                                        Log logObj = logs.Dequeue();
                                        if (logObj != null)
                                        {
                                            temp.AppendLine(logObj.ToString());
                                        }
                                        logObj = null;
                                    }
                                    if (temp.Length > 0) logsFile.Write(temp.ToString());
                                    logsFile.Flush();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.Message);
                                }
                            }

                            if (transcations.Count > 0)
                            {
                                try
                                {
                                    int transcationsData = transcations.Count;
                                    StringBuilder temp = new StringBuilder();
                                    for (int index = 0; index < transcationsData; index++)
                                    {
                                        TransactionRunTimeDetail transcation = transcations.Dequeue();
                                        if (transcation != null)
                                        {
                                            temp.AppendLine(transcation.ToString());
                                        }
                                        transcation = null;
                                    }
                                    if (temp.Length > 0) transactionFile.Write(temp.ToString());
                                    transactionFile.Flush();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.Message);
                                }
                            }
                            #endregion

                            if (timer.Elapsed.Seconds >= 5)
                            {
                                #region Write to database
                                if (reportDataFile.BaseStream.Length > 0 || errorFile.BaseStream.Length > 0 || transactionFile.BaseStream.Length > 0 || logsFile.BaseStream.Length > 0)
                                {

                                    long reportDataLength = reportDataFile.BaseStream.Length;
                                    long errorFileLength = errorFile.BaseStream.Length;
                                    long transactionFileLength = transactionFile.BaseStream.Length;
                                    long logFileLength = logsFile.BaseStream.Length;

                                    reportDataFile.Close();
                                    errorFile.Close();
                                    transactionFile.Close();
                                    logsFile.Close();

                                    try
                                    {
                                        lock (DataBaseLock)
                                        {
                                            Constants.GetInstance().ExecuteBat(Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                                    }
                                    finally
                                    {
                                        using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\out.txt", FileMode.Truncate)) 
                                        {
                                        };
                                        using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\error.txt", FileMode.Truncate)) 
                                        {
                                        };
                                        using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\transaction.txt", FileMode.Truncate)) 
                                        {
                                        };
                                        using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\log.txt", FileMode.Truncate)) 
                                        { };
                                        reportDataFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\out.txt");
                                        errorFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\error.txt");
                                        transactionFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\transaction.txt");
                                        logsFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\log.txt");
                                    }
                                }
                                timer.Reset();
                                timer.Start();
                                #endregion
                            }
                        }
                        if (reportDT.Count == 0 && errors.Count == 0 && transcations.Count == 0 && logs.Count == 0)
                        {
                            System.Threading.Thread.Sleep(4000);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                } while (true);
            }).Start();
        }
        public void LogResult(string loadGen, string sourceip, string reportName, string sceanrioName, string scriptid, string containerid, string containername, string pageid, string requestid, string address, int userid, int iteration, DateTime start, DateTime end, double diff, long responseSize, string reponseCode)
        {
            try
            {
                ReportData rd = new ReportData();
                rd.loadgen = loadGen;
                rd.sourceip = sourceip;
                rd.scenarioname = sceanrioName;
                rd.scriptid = scriptid;
                rd.containerid = containerid;
                rd.containername = containername;
                rd.pageid = pageid;
                rd.requestid = requestid;
                rd.address = address;
                rd.userid = userid;
                rd.iterationid = iteration;
                rd.starttime = start;
                rd.endtime = end;
                rd.diff = diff;
                rd.responsesize = responseSize;
                rd.reponseCode = reponseCode;
                lock (reportDT)
                {
                    reportDT.Enqueue(rd);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        public void ClearData()
        {
            try
            {
                reportDT.Clear();
                transcations.Clear();
                errors.Clear();
                logs.Clear();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
