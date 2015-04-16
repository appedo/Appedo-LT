using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using AppedoLT.DataAccessLayer;
using System;
using System.Text;
using System.Xml;
using AppedoLT.BusinessLogic;
using System.IO;
using AppedoLT.LoadGenerator.Properties;
using System.Diagnostics;
namespace AppedoLTLoadGenerator
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        //[STAThread]
        static void Main()
        {
            bool createdNew = true;
            Process current = Process.GetCurrentProcess();
            using (Mutex mutex = new Mutex(true, current.ProcessName, out createdNew))
            {
                if (createdNew)
                {
                    using (LoadGenerator frm = new LoadGenerator())
                    {
                        Application.Run();
                    }
                }
                else
                {
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            process.Kill();
                            using (LoadGenerator frm = new LoadGenerator())
                            {
                                Application.Run();
                            }
                            break;
                        }
                    }
                }
            }
        }

    }
}