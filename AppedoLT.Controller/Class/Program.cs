using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net.Config;

namespace AppedoLTController
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew = true;
           
            Process current = Process.GetCurrentProcess();
            using (Mutex mutex = new Mutex(true, current.ProcessName, out createdNew))
            {
                if (createdNew)
                {
                    using (frmController frm = new frmController())
                    {
                        XmlConfigurator.Configure();

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
                            using (frmController frm = new frmController())
                            {
                                XmlConfigurator.Configure();

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
