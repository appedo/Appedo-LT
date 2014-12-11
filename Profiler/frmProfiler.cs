using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Profiler
{
    public partial class frmProfiler : Form
    {
        private string logDirectory;
        private NamedManualResetEvent loggingActiveEvent;
        private NamedManualResetEvent forceGcEvent;
        private NamedManualResetEvent loggingActiveCompletedEvent;
        private NamedManualResetEvent forceGcCompletedEvent;
        private NamedManualResetEvent callGraphActiveEvent;
        private NamedManualResetEvent callGraphActiveCompletedEvent;
        private NamedManualResetEvent detachEvent;
        private Process profiledProcess;
        internal bool noUI = true;
        private SafeFileHandle handshakingPipeHandle;
        private SafeFileHandle loggingPipeHandle;
        private FileStream handshakingPipe;
        private FileStream loggingPipe;
        private Dictionary<string, Stack<TraceLogInfo>> TraceLog = new Dictionary<string, Stack<TraceLogInfo>>();
        private Queue<TraceLogInfo> Trace = new Queue<TraceLogInfo>();

        NotifyIcon ni = new NotifyIcon();
        Thread DoWorkThread = null;

        string trayText = "Appedo Profile Agent.";
        string trayTipText = "Appedo Profile.";
        MessageQueue myQueue;
        ProfilerXml xml = ProfilerXml.GetInstance();
        string uid = string.Empty;
        string dataSendUrl;

        object lockobj = new object();
        public long TransactionID
        {
            get
            {
                lock (lockobj)
                {
                    Thread.Sleep(1);
                    return DateTime.Now.Ticks;
                }
            }
        }
        public frmProfiler()
        {
            InitializeComponent();
            try
            {
                if (ConfigurationSettings.AppSettings["uid"] == string.Empty)
                {
                    MessageBox.Show("UID is missing. Please download again.");
                    Environment.Exit(1);
                }
                else
                {
                    uid = ConfigurationSettings.AppSettings["uid"];
                    dataSendUrl = GetPath() + "/collectProfilerStack";
                    DoWorkThread = new Thread(new ThreadStart(DoWork));
                    DoWorkThread.Start();
                    ni.Icon = new Form().Icon;
                    ni.Text = trayText;
                    ni.Visible = true;
                    ni.ContextMenuStrip = contextMenuStrip1;
                    ni.BalloonTipText = trayTipText;
                    ni.ShowBalloonTip(1000);
                    ni.ContextMenuStrip = contextMenuStrip1;
                    myQueue = CreateMSMQ();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void DoWork()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        //  Thread.Sleep(30000);
                        if (Trace.Count > 0)
                        {
                            // GetProfilerData();
                            GetPageContent(dataSendUrl, GetProfilerData());
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (targetv2DesktopCLR())
            {
                RegisterDLL.Register();  // Register profilerOBJ.dll for v2 CLR, which doesn't support registry free activation
            }
            StopIIS();
            string logDir = GetLogDir();
            string[] profilerEnvironment = CreateProfilerEnvironment(logDir);

            string[] baseEnvironment = GetServicesEnvironment();
            baseEnvironment = ReplaceTempDir(baseEnvironment, GetLogDir());
            string[] combinedEnvironment = CombineEnvironmentVariables(baseEnvironment, profilerEnvironment);
            SetEnvironmentVariables("IISADMIN", combinedEnvironment);
            SetEnvironmentVariables("W3SVC", combinedEnvironment);
            SetEnvironmentVariables("WAS", combinedEnvironment);
        }
        private bool targetv2DesktopCLR()
        {
            return false;
            // return (noUI && targetCLRVersion == CLRSKU.V2DesktopCLR) || (!noUI && targetCLRVersioncomboBox.SelectedIndex == 2);
        }
        private void StopIIS()
        {
            // stop IIS
            //  Text = "Stopping IIS ";
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (Environment.OSVersion.Version.Major >= 6/*Vista*/)
                processStartInfo.Arguments = "/c net stop was /y";
            else
                processStartInfo.Arguments = "/c net stop iisadmin /y";
            Process process = Process.Start(processStartInfo);
            while (!process.HasExited)
            {
                //Text += ".";
                Thread.Sleep(100);
                //  Application.DoEvents();
            }
            if (process.ExitCode != 0)
            {
                //  Text += string.Format(" Error {0} occurred", process.ExitCode);
            }
            else
            {

            }
            // Text = "IIS stopped";
        }
        private string GetLogDir()
        {
            // if (logDirectory != null)
            // return logDirectory;

            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string tempDir = null;
            //string winDir = Environment.GetEnvironmentVariable("WINDIR");
            //if (winDir != null)
            //{
            //    tempDir = winDir + @"\TEMP";
            //    if (!Directory.Exists(tempDir))
            //        tempDir = null;
            //}
            //if (tempDir == null)
            //{
            //    tempDir = Environment.GetEnvironmentVariable("TEMP");
            //    if (tempDir == null)
            //    {
            //        tempDir = Environment.GetEnvironmentVariable("TMP");
            //        if (tempDir == null)
            //            tempDir = @"C:\TEMP";
            //    }
            //}
            //return tempDir;
        }
        private string[] CreateProfilerEnvironment(string tempDir)
        {
            return new string[]
            { 
                "Cor_Enable_Profiling=0x1",
                "COR_PROFILER={8C29BC4E-1F57-461a-9B51-1200C32E6F1F}",
                "COR_PROFILER_PATH=" + getProfilerFullPath(),
                "OMV_SKIP=0",
                "OMV_FORMAT=v2",
                "OMV_STACK=0",
                "OMV_DynamicObjectTracking=0x1",
                "OMV_PATH=" + tempDir,
                "OMV_USAGE=" + CreateUsageString(),
                "OMV_FORCE_GC_ON_COMMENT=1",
                "OMV_INITIAL_SETTING=" + CreateInitialString(),
                "OMV_TargetCLRVersion="+ConfigurationSettings.AppSettings["clrversion"],
                "OMV_WindowsStoreApp=0"
            };
        }
        private static unsafe int wcslen(char* s)
        {
            char* e;
            for (e = s; *e != '\0'; e++)
                ;
            return (int)(e - s);
        }
        private string[] GetServicesEnvironment()
        {
            Process[] servicesProcesses = Process.GetProcessesByName("services");
            if (servicesProcesses == null || servicesProcesses.Length != 1)
            {
                servicesProcesses = Process.GetProcessesByName("services.exe");
                if (servicesProcesses == null || servicesProcesses.Length != 1)
                    return new string[0];
            }
            Process servicesProcess = servicesProcesses[0];
            IntPtr processHandle = OpenProcess(0x20400, false, servicesProcess.Id);
            if (processHandle == IntPtr.Zero)
                return new string[0];
            IntPtr tokenHandle = IntPtr.Zero;
            if (!OpenProcessToken(processHandle, 0x20008, ref tokenHandle))
                return new string[0];
            IntPtr environmentPtr = IntPtr.Zero;
            if (!CreateEnvironmentBlock(out environmentPtr, tokenHandle, false))
                return new String[0];
            unsafe
            {
                string[] envStrings = null;
                // rather than duplicate the code that walks over the environment, 
                // we have this funny loop where the first iteration just counts the strings,
                // and the second iteration fills in the strings
                for (int i = 0; i < 2; i++)
                {
                    char* env = (char*)environmentPtr.ToPointer();
                    int count = 0;
                    while (true)
                    {
                        int len = wcslen(env);
                        if (len == 0)
                            break;
                        if (envStrings != null)
                            envStrings[count] = new String(env);
                        count++;
                        env += len + 1;
                    }
                    if (envStrings == null)
                        envStrings = new string[count];
                }
                return envStrings;
            }
        }

        #region Kernel32
        struct SECURITY_ATTRIBUTES
        {
            public uint nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        };
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        private static extern SafeFileHandle CreateNamedPipe(
            string lpName,         // pointer to pipe name
            uint dwOpenMode,       // pipe open mode
            uint dwPipeMode,       // pipe-specific modes
            uint nMaxInstances,    // maximum number of instances
            uint nOutBufferSize,   // output buffer size, in bytes
            uint nInBufferSize,    // input buffer size, in bytes
            uint nDefaultTimeOut,  // time-out time, in milliseconds
            ref SECURITY_ATTRIBUTES lpSecurityAttributes  // pointer to security attributes
            );

        [DllImport("Kernel32.dll")]
        private static extern IntPtr OpenProcess(
            uint dwDesiredAccess,  // access flag
            bool bInheritHandle,    // handle inheritance option
            int dwProcessId       // process identifier
            );

        [DllImport("Advapi32.dll")]
        private static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            uint DesiredAccess,
            ref IntPtr TokenHandle
            );

        [DllImport("UserEnv.dll")]
        private static extern bool CreateEnvironmentBlock(
                out IntPtr lpEnvironment,
                IntPtr hToken,
                bool bInherit);

        [DllImport("UserEnv.dll")]
        private static extern bool DestroyEnvironmentBlock(
                IntPtr lpEnvironment);

        [DllImport("Advapi32.dll")]
        private static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(
            string StringSecurityDescriptor,
            uint StringSDRevision,
            out IntPtr SecurityDescriptor,
            IntPtr SecurityDescriptorSize
            );

        [DllImport("Kernel32.dll")]
        private static extern bool LocalFree(IntPtr ptr);

        [DllImport("Advapi32.dll")]
        private static extern bool ConvertSidToStringSidW(byte[] sid, out IntPtr stringSid);

        [DllImport("Advapi32.dll")]
        private static extern bool LookupAccountName(string machineName, string accountName, byte[] sid,
                                 ref int sidLen, StringBuilder domainName, ref int domainNameLen, out int peUse);

        [DllImport("Kernel32.dll")]
        private static extern bool ConnectNamedPipe(
            SafeFileHandle hNamedPipe,  // handle to named pipe to connect
            IntPtr lpOverlapped         // pointer to overlapped structure
            );

        [DllImport("Kernel32.dll")]
        private static extern bool DisconnectNamedPipe(
            SafeFileHandle hNamedPipe   // handle to named pipe
            );

        [DllImport("Kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("Kernel32.dll")]
        private static extern bool ReadFile(
            IntPtr hFile,                // handle of file to read
            byte[] lpBuffer,             // pointer to buffer that receives data
            uint nNumberOfBytesToRead,  // number of bytes to read
            out uint lpNumberOfBytesRead, // pointer to number of bytes read
            IntPtr lpOverlapped    // pointer to structure for data
            );

        [DllImport("Kernel32.dll")]
        private static extern int IsWow64Process(IntPtr process, out int wow64Process);
        #endregion

        private bool CreatePipe(string pipeName, bool blockingPipe, ref SafeFileHandle pipeHandle, ref FileStream pipe)
        {
            SECURITY_ATTRIBUTES sa;
            sa.nLength = 12;
            sa.bInheritHandle = 0;
            if (!ConvertStringSecurityDescriptorToSecurityDescriptor("D: (A;OICI;GRGW;;;AU)", 1, out sa.lpSecurityDescriptor, IntPtr.Zero))
                return false;
            uint flags = 4 | 2 | 0;

            if (!blockingPipe)
                flags |= 1;
            pipeHandle = CreateNamedPipe(pipeName, 3, flags, 1, 512, 512, 1000, ref sa);
            LocalFree(sa.lpSecurityDescriptor);
            if (pipeHandle.IsInvalid)
                return false;
            pipe = new FileStream(pipeHandle, FileAccess.ReadWrite, 512, false);
            return true;
        }

        private void ClosePipe(ref SafeFileHandle pipeHandle, ref FileStream pipe)
        {
            pipe.Close();
            pipe = null;
            pipeHandle = null;
        }

        private void InitWindowsStoreAppLogDirectory(string acFolderPath)
        {
            // Profiler will need to write under the AppContainer directory that
            // the Windows Store app is given access to.  Create a CLRProfiler subdirectory
            // under there for the profiler to use.
            logDirectory = acFolderPath + "\\CLRProfiler";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        private NamedManualResetEvent CreateEvent(string baseName, int pid, bool createEventHandle)
        {
            string eventName = string.Format("{0}{1}_{2:x8}", "Global\\", baseName, pid);
            return new NamedManualResetEvent(eventName, false, createEventHandle);
        }

        private void CreateEvents(int pid)
        {
            try
            {
                loggingActiveEvent = CreateEvent("OMV_TriggerObjects", pid, true);
                loggingActiveCompletedEvent = CreateEvent("OMV_TriggerObjects_Completed", pid, true);
                forceGcEvent = CreateEvent("OMV_ForceGC", pid, true);
                forceGcCompletedEvent = CreateEvent("OMV_ForceGC_Completed", pid, true);
                callGraphActiveEvent = CreateEvent("OMV_Callgraph", pid, true);
                callGraphActiveCompletedEvent = CreateEvent("OMV_Callgraph_Completed", pid, true);
                detachEvent = CreateEvent("OMV_Detach", pid, true);
            }
            catch
            {
                MessageBox.Show("Could not create events - in case you are profiling a service, " +
                    "start the profiler BEFORE starting the service");
                throw;
            }
        }

        private void ClearEvents()
        {
            loggingActiveEvent.Dispose();
            loggingActiveEvent = null;
            loggingActiveCompletedEvent.Dispose();
            loggingActiveCompletedEvent = null;
            forceGcEvent.Dispose();
            forceGcEvent = null;
            forceGcCompletedEvent.Dispose();
            forceGcCompletedEvent = null;
            callGraphActiveEvent.Dispose();
            callGraphActiveEvent = null;
            callGraphActiveCompletedEvent.Dispose();
            callGraphActiveCompletedEvent = null;
            detachEvent.Dispose();
            detachEvent = null;
        }


        private string GetLogFullPath(int pid)
        {
            return GetLogDir() + "\\" + getLogFileName(pid);
        }

        private void ClearProfiledProcessInfo()
        {
            profiledProcess = null;

        }

        // Checks if bitness of CLRProfiler.exe matches bitness of specified process.
        // If we can conclusively prove the bitnesses are different, then display an
        // error message and return false.  If the bitnesses are the same (or we failed
        // trying to determine that), just optimistically return true.
        private bool VerifyCorrectBitness(Process process)
        {
            if (process == null)
                return true;

            if (!Environment.Is64BitOperatingSystem)
            {
                // On 32-bit OS's everyone has the same bitness
                return true;
            }

            int areYouWow;
            if (IsWow64Process(process.Handle, out areYouWow) == 0)
                return true;
            bool areYou64 = (areYouWow == 0);

            bool amI64 = Environment.Is64BitProcess;

            if (amI64 == areYou64)
                return true;

            ShowErrorMessage(
                string.Format(
                    "You are trying to profile process ID {0} which is running as {1} bits, but CLRProfiler is currently running as {2} bits.  Please rerun the {1} bit version of CLRProfiler and try again.",
                    process.Id,
                    areYou64 ? "64" : "32",
                    amI64 ? "64" : "32"));

            return false;
        }

        private void ShowErrorMessage(string message)
        {
            if (!noUI)
            {
                MessageBox.Show(message, "CLRProfiler");
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        private int WaitForProcessToConnect(string tempDir, string text, bool attachMode = false, uint result = 0)
        {
            bool fProfiledProcessInitialized = profiledProcess != null;
            if (fProfiledProcessInitialized)
            {
                if (!VerifyCorrectBitness(profiledProcess))
                    return -1;
            }


            ConnectNamedPipe(handshakingPipeHandle, IntPtr.Zero);
            ConnectNamedPipe(loggingPipeHandle, IntPtr.Zero);

            int pid = 0;
            byte[] handshakingBuffer = new byte[9];
            int handshakingReadBytes = 0;

            // IMPORTANT: maxloggingBufferSize must match bufferSize defined in ProfilerCallback.cpp.
            const int maxloggingBufferSize = 512;
            byte[] loggingBuffer = new byte[maxloggingBufferSize];
            int loggingReadBytes = 0;
            WaitingForConnectionForm waitingForConnectionForm = null;
            int beginTickCount = Environment.TickCount;

            //Do not show the text in attachmode 
            if (attachMode == false)
            {
                if (noUI)
                {
                    Console.WriteLine(text);
                }
                else
                {
                    //    if (waitingForConnectionForm == null)
                    //        waitingForConnectionForm = new WaitingForConnectionForm();
                    //    waitingForConnectionForm.setMessage(text);
                    //    waitingForConnectionForm.Visible = true;
                }
            }


            // loop reading two pipes,
            // until   
            //  (1)successfully connected 
            //  (2)User canceled
            //  (3)attach failed
            //  (4)target process exited
            while (true)
            {
                #region handshaking
                //(1)succeeded
                try
                {
                    handshakingReadBytes += handshakingPipe.Read(handshakingBuffer, handshakingReadBytes, 9 - handshakingReadBytes);
                }
                catch (System.IO.IOException)
                {
                }

                //Read 9 bytes from handshaking pipe
                //means the profielr was initialized successfully
                if (handshakingReadBytes == 9)
                    break;

                Application.DoEvents();
                //  (2)User canceled
                //if (!noUI)
                //{
                //    if (waitingForConnectionForm != null && waitingForConnectionForm.DialogResult == DialogResult.Cancel)
                //    {
                //        pid = -1;
                //        break;
                //    }
                //}
                #endregion handshaking
                #region logging
                //  (3)attach failed
                //  (3.1) read logging message
                //  (3.2) break if attach failed.

                //  (3.1) read logging message
                try
                {
                    loggingReadBytes += loggingPipe.Read(loggingBuffer, loggingReadBytes, maxloggingBufferSize - loggingReadBytes);
                }
                catch (System.IO.IOException)
                {
                }

                if (loggingReadBytes == maxloggingBufferSize)
                {
                    char[] charBuffer = new char[loggingReadBytes];
                    for (int i = 0; i < loggingReadBytes; i++)
                        charBuffer[i] = Convert.ToChar(loggingBuffer[i]);

                    string message = new String(charBuffer, 0, loggingReadBytes);

                    if (attachMode == false && noUI == false)
                    {
                        //  waitingForConnectionForm.addMessage(message);
                    }
                    else
                    {
                        // ShowErrorMessage(message);
                    }

                    loggingReadBytes = 0;

                    while (true)
                    {
                        try
                        {
                            if (loggingPipe.Read(loggingBuffer, 0, 1) == 0)
                            {
                                DisconnectNamedPipe(loggingPipeHandle);
                                ConnectNamedPipe(loggingPipeHandle, IntPtr.Zero);
                                break;
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            DisconnectNamedPipe(loggingPipeHandle);
                            ConnectNamedPipe(loggingPipeHandle, IntPtr.Zero);
                            break;
                        }
                    }
                }
                //  (3.2) break if attach failed.
                if (attachMode == true && result != 0)
                {
                    pid = -1;
                    break;
                }
                #endregion logging
                //  (4)target process exited
                if ((fProfiledProcessInitialized && profiledProcess == null) || (profiledProcess != null))// && ProfiledProcessHasExited()))
                {
                    pid = -1;
                    break;
                }
                Thread.Sleep(100);
            }

            if (waitingForConnectionForm != null) ;
            // waitingForConnectionForm.Visible = false;

            if (pid == -1)
                return pid;
            if (handshakingReadBytes == 9)
            {
                char[] charBuffer = new char[9];
                for (int i = 0; i < handshakingBuffer.Length; i++)
                    charBuffer[i] = Convert.ToChar(handshakingBuffer[i]);
                pid = Int32.Parse(new String(charBuffer, 0, 8), NumberStyles.HexNumber);

                CreateEvents(pid);

                string fileName = getLogFileName(pid);
                byte[] fileNameBuffer = new Byte[fileName.Length + 1];
                for (int i = 0; i < fileName.Length; i++)
                    fileNameBuffer[i] = (byte)fileName[i];

                fileNameBuffer[fileName.Length] = 0;
                handshakingPipe.Write(fileNameBuffer, 0, fileNameBuffer.Length);
                handshakingPipe.Flush();

                //  log = new ReadNewLog(logFileName);
                //  lastLogResult = null;
                //   ObjectGraph.cachedGraph = null;
                while (true)
                {
                    try
                    {
                        if (handshakingPipe.Read(handshakingBuffer, 0, 1) == 0) // && GetLastError() == 109/*ERROR_BROKEN_PIPE*/)
                        {
                            DisconnectNamedPipe(handshakingPipeHandle);
                            ConnectNamedPipe(handshakingPipeHandle, IntPtr.Zero);
                            break;
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        DisconnectNamedPipe(handshakingPipeHandle);
                        ConnectNamedPipe(handshakingPipeHandle, IntPtr.Zero);
                        break;
                    }
                }
            }
            else
            {
                string error = string.Format("Error {0} occurred", GetLastError());
                ShowErrorMessage(error);
            }

            if (noUI)
            {
                Console.WriteLine("CLRProfiler is loaded in the target process.");
            }
            else
            {
                //EnableDisableViewMenuItems();
                //EnableDisableLaunchControls(false);

                //if (!allocationsCheckBox.Checked && callsCheckBox.Checked)
                //    showHeapButton.Enabled = false;
                //else
                //    showHeapButton.Enabled = true;

                //killApplicationButton.Enabled = true;
                //detachProcessMenuItem.Enabled = false;
            }

            //profilerConnected = true;

            return pid;
        }

        private string[] CombineEnvironmentVariables(string[] a, string[] b)
        {
            string[] c = new string[a.Length + b.Length];
            int i = 0;
            foreach (string s in a)
                c[i++] = s;
            foreach (string s in b)
                c[i++] = s;
            return c;
        }

        private Microsoft.Win32.RegistryKey GetServiceKey(string serviceName)
        {
            Microsoft.Win32.RegistryKey localMachine = Microsoft.Win32.Registry.LocalMachine;
            Microsoft.Win32.RegistryKey key = localMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\" + serviceName, true);
            return key;
        }

        private void SetEnvironmentVariables(string serviceName, string[] environment)
        {
            Microsoft.Win32.RegistryKey key = GetServiceKey(serviceName);
            if (key != null)
                key.SetValue("Environment", environment);
        }

        private void DeleteEnvironmentVariables(string serviceName)
        {
            Microsoft.Win32.RegistryKey key = GetServiceKey(serviceName);
            if (key != null)
                key.DeleteValue("Environment");
        }

        private string EnvKey(string envVariable)
        {
            int index = envVariable.IndexOf('=');
            Debug.Assert(index >= 0);
            return envVariable.Substring(0, index);
        }

        private string EnvValue(string envVariable)
        {
            int index = envVariable.IndexOf('=');
            Debug.Assert(index >= 0);
            return envVariable.Substring(index + 1);
        }

        private Microsoft.Win32.RegistryKey GetAccountEnvironmentKey(string serviceAccountSid)
        {
            Microsoft.Win32.RegistryKey users = Microsoft.Win32.Registry.Users;
            return users.OpenSubKey(serviceAccountSid + @"\Environment", true);
        }

        private void SetAccountEnvironment(string serviceAccountSid, string[] profilerEnvironment)
        {
            Microsoft.Win32.RegistryKey key = GetAccountEnvironmentKey(serviceAccountSid);
            if (key != null)
            {
                foreach (string envVariable in profilerEnvironment)
                {
                    key.SetValue(EnvKey(envVariable), EnvValue(envVariable));
                }
            }
        }

        private void ResetAccountEnvironment(string serviceAccountSid, string[] profilerEnvironment)
        {
            Microsoft.Win32.RegistryKey key = GetAccountEnvironmentKey(serviceAccountSid);
            if (key != null)
            {
                foreach (string envVariable in profilerEnvironment)
                {
                    key.DeleteValue(EnvKey(envVariable));
                }
            }
        }

        private string CreateUsageString()
        {
            int index = 0;
            string[] usageStrings = new string[] { "none", "objects", "trace", "both" };
            return usageStrings[index];
        }

        private string CreateInitialString()
        {
            int flags = 0;
            return flags.ToString();
        }

        private string GetServiceAccountName(string serviceName)
        {
            Microsoft.Win32.RegistryKey key = GetServiceKey(serviceName);
            if (key != null)
                return key.GetValue("ObjectName") as string;
            return null;
        }

        private string LookupAccountSid(string accountName)
        {
            int sidLen = 0;
            byte[] sid = new byte[sidLen];
            int domainNameLen = 0;
            int peUse;
            StringBuilder domainName = new StringBuilder();
            LookupAccountName(Environment.MachineName, accountName, sid, ref sidLen, domainName, ref domainNameLen, out peUse);

            sid = new byte[sidLen];
            domainName = new StringBuilder(domainNameLen);
            string stringSid = null;
            if (LookupAccountName(Environment.MachineName, accountName, sid, ref sidLen, domainName, ref domainNameLen, out peUse))
            {
                IntPtr stringSidPtr;
                if (ConvertSidToStringSidW(sid, out stringSidPtr))
                {
                    try
                    {
                        stringSid = Marshal.PtrToStringUni(stringSidPtr);
                    }
                    finally
                    {
                        LocalFree(stringSidPtr);
                    }
                }
            }
            return stringSid;
        }

        private string[] ReplaceTempDir(string[] env, string newTempDir)
        {
            for (int i = 0; i < env.Length; i++)
            {
                if (env[i].StartsWith("TEMP="))
                    env[i] = "TEMP=" + newTempDir;
                else if (env[i].StartsWith("TMP="))
                    env[i] = "TMP=" + newTempDir;
            }
            return env;
        }

        private string getProfilerFullPath()
        {
            return Path.GetDirectoryName(Application.ExecutablePath) + "\\ProfilerOBJ.dll";
        }

        private string getLogFileName(int pid)
        {
            return string.Format("pipe_{0}.log", pid);
        }

        private int getPID(string[] arguments)
        {
            if (arguments.Length == 1)
            {
                Console.WriteLine("Please specify the process ID");
                return 0;
            }
            int pid = 0;
            try
            {
                pid = Int32.Parse(arguments[1]);
                Process.GetProcessById(pid);
            }
            catch (Exception e)
            {
                ShowErrorMessage(string.Format("The process ID ({0}) is not valid : {1}", arguments[1], e.Message));
                pid = 0;
            }

            return pid;
        }

        private bool isProfilerLoaded(int pid)
        {
            NamedManualResetEvent forceGCEvent = CreateEvent("OMV_ForceGC", pid, false);
            bool result = forceGCEvent.IsValid();
            forceGCEvent.Dispose();
            return result;
        }


        private bool StartIIS()
        {
            // Text = "Starting IIS ";
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.Arguments = "/c net start w3svc";
            Process process = Process.Start(processStartInfo);
            while (!process.HasExited)
            {
                // Text += ".";
                Thread.Sleep(100);
                Application.DoEvents();
            }
            if (process.ExitCode != 0)
            {
                //Text += string.Format(" Error {0} occurred", process.ExitCode);
                return false;
            }
            // Text = "IIS running";
            return true;
        }

        private string GetASP_NETaccountName()
        {
            try
            {
                XmlDocument machineConfig = new XmlDocument();
                string runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
                string configPath = Path.Combine(runtimePath, @"CONFIG\machine.config");
                machineConfig.Load(configPath);
                XmlNodeList elemList = machineConfig.GetElementsByTagName("processModel");
                for (int i = 0; i < elemList.Count; i++)
                {
                    XmlAttributeCollection attributes = elemList[i].Attributes;
                    XmlAttribute userNameAttribute = attributes["userName"];
                    if (userNameAttribute != null)
                    {
                        string userName = userNameAttribute.InnerText;
                        if (userName == "machine")
                            return "ASPNET";
                        else if (userName == "SYSTEM")
                            return null;
                        else
                            return userName;
                    }
                }
            }
            catch
            {
                // swallow all exceptions here
            }
            return "ASPNET";
        }

        private void Start()
        {
            // if (targetv2DesktopCLR())
            {
                RegisterDLL.Register();  // Register profilerOBJ.dll for v2 CLR, which doesn't support registry free activation
            }
            StopIIS();

            // set environment variables

            string logDir = GetLogDir();
            string[] profilerEnvironment = CreateProfilerEnvironment(logDir);

            string[] baseEnvironment = GetServicesEnvironment();
            baseEnvironment = ReplaceTempDir(baseEnvironment, GetLogDir());
            string[] combinedEnvironment = CombineEnvironmentVariables(baseEnvironment, profilerEnvironment);
            SetEnvironmentVariables("IISADMIN", combinedEnvironment);
            SetEnvironmentVariables("W3SVC", combinedEnvironment);
            SetEnvironmentVariables("WAS", combinedEnvironment);

            string asp_netAccountName = GetASP_NETaccountName();
            string asp_netAccountSid = null;
            if (asp_netAccountName != null)
            {
                asp_netAccountSid = LookupAccountSid(asp_netAccountName);
                if (asp_netAccountSid != null)
                    SetAccountEnvironment(asp_netAccountSid, profilerEnvironment);
            }

            if (StartIIS())
            {
                // wait for worker process to start up and connect
                //  Text = "Waiting for ASP.NET worker process to start up";

                Thread.Sleep(1000);
                int pid = WaitForProcessToConnect(logDir, "Waiting for ASP.NET to start common language runtime - this is the time to load your test page");
                if (pid > 0)
                {
                    profiledProcess = Process.GetProcessById(pid);
                    ni.BalloonTipText = "Appedo Profile Agent started successfully.";
                    ni.ShowBalloonTip(2000);
                    //  trayTipText = 
                    // startApplicationButton.Text = "Start ASP.NET";
                    //killApplicationButton.Text = "Kill ASP.NET";

                }
            }

            /* Delete the environment variables as early as possible, so that even if CLRProfiler crashes, the user's machine
             * won't be screwed up.
             * */
            DeleteEnvironmentVariables("IISADMIN");
            DeleteEnvironmentVariables("W3SVC");
            DeleteEnvironmentVariables("WAS");

            if (asp_netAccountSid != null)
                ResetAccountEnvironment(asp_netAccountSid, profilerEnvironment);

            Receive();
        }

        private void Receive()
        {
            Regex regex = new Regex(@"([\d]+) ([\d]+) ([\d]+) '(.*)' (.*)");
            Match mat;
            new Thread(() =>
            {
                System.Messaging.Message msg;
                byte[] data = new byte[256];
                string receivedStr = string.Empty;
                long transactionidlocal;
                while (true)
                {
                    msg = myQueue.Receive();
                    data = new byte[msg.BodyStream.Length];
                    msg.BodyStream.Read(data, 0, Convert.ToInt32(msg.BodyStream.Length));
                    receivedStr = Encoding.Unicode.GetString(data, 0, Convert.ToInt32(msg.BodyStream.Length));

                    switch (msg.Label)
                    {
                        case "E":
                            mat = regex.Match(receivedStr);
                            if (mat.Success)
                            {
                                if (TraceLog.ContainsKey(mat.Groups[1].Value) == false)
                                {
                                    TraceLog.Add(mat.Groups[1].Value, new Stack<TraceLogInfo>());
                                }
                                if (TraceLog[mat.Groups[1].Value].Count == 0)
                                {
                                    transactionidlocal = this.TransactionID;

                                    TraceLog[mat.Groups[1].Value].Push(new TraceLogInfo(
                                        "-1",
                                        transactionidlocal.ToString(),
                                        mat.Groups[2].Value,
                                        "1",
                                         mat.Groups[4].Value,
                                         mat.Groups[5].Value,
                                         1));
                                }
                                else
                                {

                                    TraceLog[mat.Groups[1].Value].Push(new TraceLogInfo(
                                        TraceLog[mat.Groups[1].Value].Peek().MethodId.ToString(),
                                        TraceLog[mat.Groups[1].Value].Peek().ThreadId.ToString(),
                                        mat.Groups[2].Value,
                                        (Convert.ToInt32(TraceLog[mat.Groups[1].Value].Peek().LastCreatedId) + 1).ToString(),
                                        mat.Groups[4].Value,
                                        mat.Groups[5].Value,
                                        Convert.ToInt32(TraceLog[mat.Groups[1].Value].Peek().LastCreatedId) + 1));
                                }
                            }
                            break;

                        case "L":
                            mat = regex.Match(receivedStr);
                            if (mat.Success)
                            {
                                if (TraceLog.ContainsKey(mat.Groups[1].Value) == true)
                                {
                                    TraceLogInfo info = TraceLog[mat.Groups[1].Value].Pop();
                                    info.EndTime = Convert.ToInt32(mat.Groups[2].Value);
                                    if (TraceLog[mat.Groups[1].Value].Count > 0) TraceLog[mat.Groups[1].Value].Peek().LastCreatedId = info.LastCreatedId;
                                    Trace.Enqueue(info);
                                }
                            }
                            break;

                    }
                }

            }).Start();

        }
        public string GetPath()
        {
            return string.Format("{0}://{1}:{2}/{3}", ConfigurationSettings.AppSettings["protocol"], ConfigurationSettings.AppSettings["server"], ConfigurationSettings.AppSettings["port"], ConfigurationSettings.AppSettings["path"]);
        }
        public string GetPageContent(string Url, string data)
        {
            string PageContent = string.Empty;
            HttpWebRequest WebRequestObject = null;
            try
            {
                WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(Url);
                WebRequestObject.Method = "POST";
                WebRequestObject.ContentLength = data.Length;
                using (Stream stream = WebRequestObject.GetRequestStream())
                {
                    stream.Write(ASCIIEncoding.ASCII.GetBytes(data), 0, data.Length);
                }
                WebResponse Response = WebRequestObject.GetResponse();
                Stream WebStream = Response.GetResponseStream();
                StreamReader objReader = new StreamReader(WebStream);
                PageContent = objReader.ReadToEnd();
                objReader.Close();
                WebStream.Close();
                Response.Close();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }
            finally
            {
                WebRequestObject = null;
            }
            return PageContent;
        }
        public string GetProfilerData()
        {
            //thread_id - 1
            //start_time - 3
            //duration_ms - 4
            //class_name - 51
            //method_name - 52
            //method_signature - 53
            //caller_method_id - 54
            //callee_method_id - 55
            int count = Trace.Count;
            StringBuilder dataStr = new StringBuilder();
            dataStr.AppendFormat("uid={0}&agent_type=DOTNET_PROFILER&profiler_array_json=[", uid);
            for (int index = 0; index < count; index++)
            {
                TraceLogInfo info = Trace.Dequeue();
                dataStr.AppendFormat("{{1={0},3=\"{1}\",4={2},51=\"{3}\",52=\"{4}\",53=\"\",54={5},55={6},7=\"{7}\"}}", info.ThreadId, info.LogStartime, info.ResponseTime, info.ClassName, info.MethodName, info.ParentMethodId, info.MethodId, info.URI);
                if (index < count - 1)
                {
                    dataStr.Append(",");
                }
            }
            dataStr.Append("]");
            return dataStr.ToString();
        }
        private MessageQueue CreateMSMQ()
        {
            string queueName = ".\\private$\\appedoprofile";
            if (MessageQueue.Exists(queueName) == true)
            {
                MessageQueue.Delete(queueName);
            }
            return MessageQueue.Create(queueName, false);

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            CreatePipe(@"\\.\pipe\OMV_PIPE", false, ref handshakingPipeHandle, ref handshakingPipe);
            CreatePipe(@"\\.\pipe\OMV_LOGGING_PIPE", false, ref loggingPipeHandle, ref loggingPipe);
            Start();
        }
        private void testToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            StopIIS();
            StartIIS();
            System.Environment.Exit(1);
        }
        private void frmProfiler_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

    }
    class TraceLogInfo
    {

        public string ParentMethodId { set; get; }
        public string ThreadId { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public string MethodId { get; set; }
        public string Path
        {

            set
            {
                string[] path = value.Split(new string[] { "::" }, StringSplitOptions.None);
                ClassName = path[0];
                if (path.Length > 1)
                    MethodName = path[1];
            }
        }
        public string MethodName { set; get; }
        public string ClassName { set; get; }
        public int ResponseTime { get { return this.EndTime - this.StartTime; } }
        public string LogStartime = string.Empty;
        public int LastCreatedId = 0;
        public string URI
        {
            get
            {
                if (ParentMethodId == "-1")
                    return ClassName;
                else return string.Empty;
            }
        }
        public TraceLogInfo(string parentmethodid, string threadid, string starttime, string methodid, string logstarttime, string methodname, int lastcreatedid)
        {
            ParentMethodId = parentmethodid;
            ThreadId = threadid;
            StartTime = Convert.ToInt32(starttime);
            MethodId = methodid;
            Path = methodname;
            LogStartime = Convert.ToDateTime(logstarttime).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
            LastCreatedId = lastcreatedid;
            // test = Convert.ToDateTime(logstarttime);
        }

    }
}
