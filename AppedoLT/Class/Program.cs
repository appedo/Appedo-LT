using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using AppedoLT.Core;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AppedoLT.Forms;

namespace AppedoLT
{
    static class Program
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main()
        {
            try
            {
                bool createdNew = true;
                Process current = Process.GetCurrentProcess();
                using (Mutex mutex = new Mutex(true, current.ProcessName, out createdNew))
                {
                    if (createdNew)
                    {
                        RequestCountHandler._NetworkConnectedIP = System.Net.Dns.GetHostByName(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                        Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
                        Application.EnableVisualStyles();
                        Constants constants = AppedoLT.Core.Constants.GetInstance();
                        constants.ApplicationStartTime = DateTime.Now;
                        Application.Run(new Design());
                       //Application.Run(new frmUserCount());
                        //Application.Run(new NetworkForm());
                    }
                    else
                    {
                        foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                        {
                            if (process.Id != current.Id)
                            {
                                SetForegroundWindow(process.MainWindowHandle);
                                MessageBox.Show("Already AppedoLT is running");
                                break;
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
