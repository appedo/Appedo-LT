using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Data.SQLite;
using System.Data;
using System.Configuration;

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

        public string ReportPath { get; set; }
        public string ReportName { get; set; }

        private bool isLogEnabled;
        public int threadCount;

        private static DataServer _instance;

        private DataServer()
        {
            isLogEnabled = false;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableLoggin"]))
            {
                isLogEnabled = bool.Parse(ConfigurationManager.AppSettings["EnableLoggin"]);
            }
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

        public const int MaxEntryCount = 20000;
        private void Start()
        {
            using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\reportdata.txt", FileMode.Create))
            {
            };
            using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\error.txt", FileMode.Create))
            {
            };
            using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\transactions.txt", FileMode.Create))
            {
            };
            using (FileStream stream = new FileStream(Constants.GetInstance().ExecutingAssemblyLocation + "\\logs.txt", FileMode.Create))
            { };

            new Thread(() =>
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                do
                {
                    try
                    {
                        if (reportDT.Count > MaxEntryCount || errors.Count > MaxEntryCount || logs.Count > MaxEntryCount || transcations.Count > MaxEntryCount)
                        {
                            LogToDatabase();
                        }

                        if (timer.Elapsed.Seconds >= 5)
                        {
                            if (reportDT.Count > 0 || errors.Count > 0 || logs.Count > 0 || transcations.Count > 0)
                            {
                                LogToDatabase();
                            }

                            timer.Reset();
                            timer.Start();
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

        #region Database Logging
        public void LogToDatabase()
        {
            new Thread(() =>
            {
                Interlocked.Increment(ref threadCount);

                try
                {
                    Queue<ReportData> reportDataCopy = null;
                    lock (reportDT)
                    {
                        reportDataCopy = new Queue<ReportData>(reportDT);
                        reportDT.Clear();
                    }

                    Queue<RequestException> errorCopy = null;
                    lock (errors)
                    {
                        errorCopy = new Queue<RequestException>(errors);
                        errors.Clear();
                    }

                    Queue<Log> logsCopy = null;
                    lock (logs)
                    {
                        logsCopy = new Queue<Log>(logs);
                        logs.Clear();
                    }

                    Queue<TransactionRunTimeDetail> transactionsCopy = null;
                    lock (transcations)
                    {
                        transactionsCopy = new Queue<TransactionRunTimeDetail>(transcations);
                        transcations.Clear();
                    }
                    
                    StringBuilder queryBuilder = new StringBuilder();
                    Dictionary<string, string> fileList = new Dictionary<string, string>();
                    
                    if (reportDataCopy.Count > 0)
                    {
                        fileList.Add("reportdata", PrepareFile(reportDataCopy));
                    }

                    if (errorCopy.Count > 0)
                    {
                        fileList.Add("error", PrepareFile(errorCopy));
                    }

                    if (logsCopy.Count > 0)
                    {
                        fileList.Add("log", PrepareFile(logsCopy));
                    }

                    if (transactionsCopy.Count > 0)
                    {
                        fileList.Add("transactions", PrepareFile(transactionsCopy));
                    }
                    
                    string commandsFile = Constants.GetInstance().ExecutingAssemblyLocation + "\\commands_" + DateTime.Now.ToString("hhmmssfffffff") + ".txt";
                    using (StreamWriter writer = new StreamWriter(commandsFile))
                    {
                        writer.WriteLine(".separator ,");
                        foreach (KeyValuePair<string, string> file in fileList)
                        {
                            writer.WriteLine(string.Format(".import {0} {1}", Path.GetFileName(file.Value), file.Key));
                        }
                    }
                    if (Import(commandsFile))
                    {
                        foreach (KeyValuePair<string, string> file in fileList)
                        {
                            File.Delete(file.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                }
                finally
                {
                    Interlocked.Decrement(ref threadCount);
                }
            }).Start();
        }

        //private object logSynchObject = new object();
        private string PrepareFile<T>(Queue<T> queue)
        {
            string entityName = typeof(T).Name;
            StringBuilder queryBuilder = new StringBuilder();
            int count = queue.Count;
            //for (var i = 0; i < count; i++)
            //{
            //    queryBuilder.AppendLine(queue.Dequeue().ToString());
            //}

            //if (isLogEnabled)
            //{
            //    lock (logSynchObject)
            //    {
            //        using (StreamWriter reportDataFile = new StreamWriter(Constants.GetInstance().ExecutingAssemblyLocation + "\\" + entityName + ".txt", true))
            //        {
            //            reportDataFile.WriteLine(queryBuilder.ToString());
            //            reportDataFile.Close();
            //        }
            //    }
            //}

            string fileName = string.Format("{0}\\{1}.csv", Constants.GetInstance().ExecutingAssemblyLocation, Guid.NewGuid().ToString());
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                for (var i = 0; i < count; i++)
                {
                    writer.WriteLine(queue.Dequeue().ToString());
                }
                //writer.Write(queryBuilder.ToString());
            }
            return fileName;
        }

        private bool Import(string commandFile)
        {
            lock (DataBaseLock)
            {
                try
                {
                    string fileName = Constants.GetInstance().ExecutingAssemblyLocation + "\\execute.bat";
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        writer.WriteLine(string.Format("sqlite3 \"{0}\\Data\\{1}\\database.db\" < \"{2}\"", Constants.GetInstance().ExecutingAssemblyLocation, ReportName, commandFile));
                    }

                    ProcessStartInfo startInfo = new ProcessStartInfo(fileName);
                    startInfo.WorkingDirectory = Constants.GetInstance().ExecutingAssemblyLocation;
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;

                    Process process = Process.Start(startInfo);

                    process.WaitForExit();

                    int exitCode = process.ExitCode;
                    process.Close();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + ex.StackTrace);
                    return false;
                }
                finally
                {
                    File.Delete(commandFile);
                }
            }

            return true;
        }
        #endregion

        public void LogResult(ReportData rd)
        {
            try
            {
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
