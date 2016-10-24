using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppedoLT.Core
{
    public static class AppedoLogger
    {
        public static bool IsLogEnabled { get; set; }
        private static Queue<LogMessage> logMessages = new Queue<LogMessage>();
        private static string fileName;

        static AppedoLogger()
        {
            IsLogEnabled = false;
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["EnableLoggin"]))
            {
                IsLogEnabled = bool.Parse(ConfigurationManager.AppSettings["EnableLoggin"]);
            }
            try
            {
                fileName = Constants.GetInstance().ExecutingAssemblyLocation + "\\log.csv";
                using (StreamWriter logFile = new StreamWriter(fileName, false))
                {
                    logFile.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", "Thread ID", "Total Thread", "Active Threads", "User ID", "Iteration Number", "Timestamp", "Status", "Request"));
                    logFile.Flush();
                    logFile.Close();
                }
            }
            catch { }

            if (IsLogEnabled)
            {
                StartLogging();
            }
        }

        public static void Log(LogMessage message)
        {
            if (IsLogEnabled)
            {
                logMessages.Enqueue(message);
            }
        }

        private static void StartLogging()
        {
            new Thread(() =>
               {
                   Stopwatch timer = new Stopwatch();
                   timer.Start();
                   StringBuilder temp = new StringBuilder();

                   while (true)
                   {
                       try
                       {
                           if (logMessages.Count == 0)
                           {
                               Thread.Sleep(2000);
                               continue;
                           }

                           int logLength = logMessages.Count;
                            for (int index = 0; index < logLength; index++)
                            {
                                object data = logMessages.Dequeue();
                                if (data != null)
                                {
                                    temp.AppendLine(data.ToString());
                                }
                                data = null;
                            }

                           if (timer.Elapsed.Seconds >= 10)
                           {
                               if (temp.Length > 0)
                               {
                                   using (StreamWriter logFile = new StreamWriter(fileName, true))
                                   {
                                       logFile.Write(temp.ToString());
                                       logFile.Flush();
                                       logFile.Close();
                                   }
                                   temp.Remove(0, temp.Length);
                               }

                               timer.Reset();
                               timer.Start();
                           }
                       }
                       catch (Exception ex)
                       {
                           ExceptionHandler.WritetoEventLog(ex.Message);
                           Thread.Sleep(5000);
                       }
                   }
               }).Start();
        }
    }

    public class LogMessage
    {
        public int ThreadID { get; set; }
        public int TotalThread { get; set; }
        public int ActiveThreads { get; set; }
        public int UserID { get; set; }
        public int IterationNumber { get; set; }
        public int LoopID { get; set; }
        public int RequestID { get; set; }
        public int ResponseID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", this.ThreadID, this.TotalThread, this.ActiveThreads, this.UserID, this.IterationNumber, this.Timestamp, this.Status, this.Request);
        }
    }
}
