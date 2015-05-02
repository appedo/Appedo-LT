using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using System.Threading;
=======
using System.Text;
using System.Threading;

>>>>>>> dev_master
namespace AppedoLT.Core
{
    /// <summary>
    /// Write exceptions to log file
    /// </summary>
    public static class ExceptionHandler
    {

        #region The static varialbles and methods

        private static bool isErrorLogRunning = false;

        public static Queue<string> errorLogs = new Queue<string>();
<<<<<<< HEAD
=======

        public static Dictionary<string, StringBuilder> RunDetaillog = new Dictionary<string, StringBuilder>();
>>>>>>> dev_master


        public static void WritetoEventLog(string strMessage)
        {
            try
            {
                if (strMessage.ToLower().Contains("Thread was being aborted".ToLower()) == false)
                {
                    errorLogs.Enqueue(strMessage);
                }
                if (isErrorLogRunning == false)
                {
                    isErrorLogRunning = true;
                    LogErrors();
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
                                    writer.WriteLine("{0} {1} ", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
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
<<<<<<< HEAD
=======

        public static void LogRunDetail(string runid, string msg)
        {
            try
            {
                if (RunDetaillog.ContainsKey(runid) == false)
                {
                    RunDetaillog.Add(runid, new StringBuilder());
                }
                else
                {
                    if (RunDetaillog[runid] != null) RunDetaillog[runid].Append(DateTime.Now.ToString()).Append(": ").AppendLine(msg);
                }
            }
            catch { }
        }

>>>>>>> dev_master
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
<<<<<<< HEAD
        
=======

        public static string GetLog()
        {
            StringBuilder logDetail = new StringBuilder();
            try
            {
                foreach (string key in RunDetaillog.Keys)
                {
                    logDetail.Append(key).Append(":").AppendLine().AppendLine(RunDetaillog[key].ToString()).AppendLine();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            return logDetail.ToString();
        }

        public static string GetLog(string runid)
        {
            StringBuilder logDetail = new StringBuilder();
            try
            {
               logDetail.AppendLine(RunDetaillog[runid].ToString());
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
            return logDetail.ToString();
        }

        #endregion
>>>>>>> dev_master
    }
}
