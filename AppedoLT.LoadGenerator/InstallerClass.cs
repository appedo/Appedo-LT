using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace AppedoLTLoadGenerator
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
                if (!Directory.Exists(".\\Upload")) Directory.CreateDirectory(".\\Upload");
                if (!Directory.Exists(".\\Variables")) Directory.CreateDirectory(".\\Variables");
                Process.Start(Path.GetDirectoryName(
                  Assembly.GetExecutingAssembly().Location) + "\\AppedoLTLoadGenerator.exe");
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
