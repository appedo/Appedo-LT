using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AppedoLT.Core
{
    // Taken from:http://msdn2.microsoft.com/en-us/library/
    // system.configuration.configurationmanager.aspx
    // Set 'RunInstaller' attribute to true.

    [RunInstaller(true)]
    public class InstallerClass : System.Configuration.Install.Installer
    {
        public InstallerClass()
            : base()
        {
            this.Committed += new InstallEventHandler(MyInstaller_Committed);
            this.Committing += new InstallEventHandler(MyInstaller_Committing);
        }

        private void MyInstaller_Committing(object sender, InstallEventArgs e)
        {
           
        }

        private void MyInstaller_Committed(object sender, InstallEventArgs e)
        {
            try
            {
               
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                if (!Directory.Exists(".\\Data")) Directory.CreateDirectory(".\\Data");
                if (!Directory.Exists(".\\DataMonitor")) Directory.CreateDirectory(".\\DataMonitor");
                if (!Directory.Exists(".\\Exported Charts")) Directory.CreateDirectory(".\\Exported Charts");
                if (!Directory.Exists(".\\Exported Reports")) Directory.CreateDirectory(".\\Exported Reports");
                if (!Directory.Exists(".\\MonitorData")) Directory.CreateDirectory(".\\MonitorData");
                if (!Directory.Exists(".\\Request")) Directory.CreateDirectory(".\\Request");
                if (!Directory.Exists(".\\Response")) Directory.CreateDirectory(".\\Response");
                if (!Directory.Exists(".\\Upload")) Directory.CreateDirectory(".\\Upload");
                if (!Directory.Exists(".\\Variables")) Directory.CreateDirectory(".\\Variables");
                Process.Start(Path.GetDirectoryName(
                  Assembly.GetExecutingAssembly().Location) + "\\AppedoLT.exe");
            }
            catch
            {
                // Do nothing... 
            }
        }

        // Override the 'Install' method.
        public override void Install(IDictionary savedState)
        {
            base.Install(savedState);
        }

        // Override the 'Commit' method.
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
        }

        // Override the 'Rollback' method.
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }
    }
}
