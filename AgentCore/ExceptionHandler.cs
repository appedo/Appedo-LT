using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
namespace AgentCore
{
    /// <summary>
    /// Write exceptions to log file
    /// </summary>
    public static class ExceptionHandler
    {
        private static bool isErrorLogRunning = false;

        public static Queue<string> errorLogs = new Queue<string>();

        public static void WritetoEventLog(string strMessage)
        {
            try
            {
                if (strMessage.EndsWith("Thread was being aborted.") == false)
                {
                    errorLogs.Enqueue(strMessage);
                    if (isErrorLogRunning == false)
                    {
                        isErrorLogRunning = true;
                        LogErrors();
                    }
                }
            }
            catch { }
        }
        private static void LogErrors()
        {
            try
            {
                new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            if (errorLogs.Count > 0)
                            {
                                string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"AppedoLT.log";
                                FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                                StreamWriter writer = new StreamWriter(stream);
                                while (errorLogs.Count > 0)
                                {
                                    writer.BaseStream.Seek(0L, SeekOrigin.End);
                                    writer.Write("Log Entry : \r\n");
                                    writer.Write("{0} {1} \r\n", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                                    writer.WriteLine("--Log entry goes here--");
                                    writer.WriteLine(errorLogs.Dequeue());
                                    writer.WriteLine("---------------------------------------");
                                }
                                writer.Flush();
                                writer.Close();
                                stream.Close();
                            }
                            else
                            {
                                Thread.Sleep(5000);
                            }

                        }
                    }
                    catch
                    {

                    }

                }).Start();
            }
            catch
            {

            }
        }
        public static void WriteResponse(string filename, string strMessage)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Response\\";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string path = directoryPath + filename.Replace("/", "");
                FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                writer.BaseStream.Seek(0L, SeekOrigin.End);
                writer.WriteLine(strMessage);
                writer.Flush();
                writer.Close();
                stream.Close();
            }
            catch { }
        }
        public static void WriteResponseImage(string filename, System.Drawing.Image image)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Response\\";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                string path = directoryPath + filename.Replace("/", "");
                image.Save(path);
            }
            catch { }
        }
        public static void WriteRequest(string filename, string strMessage)
        {
            try
            {
                string directoryPath =Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location) + "\\Request\\";
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string path = directoryPath + filename.Replace("/", "");
                FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                writer.BaseStream.Seek(0L, SeekOrigin.End);
                writer.WriteLine(strMessage);
                writer.Flush();
                writer.Close();
                stream.Close();
            }
            catch { }
        }
        public static void WriteRepository(string strMessage)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string path = directoryPath + "\\Repository.xml";
                if (File.Exists(path)) File.Delete(path);
                FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                writer.BaseStream.Seek(0L, SeekOrigin.End);
                writer.WriteLine(strMessage);
                writer.Flush();
                writer.Close();
                stream.Close();
            }
            catch { }
        }
        public static void WriteRunTimeException(string strMessage)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string path = directoryPath + "\\RunTimeException_"+DateTime.Now.ToString("dd_MMM_yyy_hh_mm_ss")+".xml";
                if (File.Exists(path)) File.Delete(path);
                FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter writer = new StreamWriter(stream);
                writer.BaseStream.Seek(0L, SeekOrigin.End);
                writer.WriteLine(strMessage);
                writer.Flush();
                writer.Close();
                stream.Close();
            }
            catch { }
        }
        
    }
}
