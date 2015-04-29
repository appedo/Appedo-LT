using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
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
                try
                {
                    try
                    {
                        System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                        if (dirInfo != null)
                        {
                            DirectorySecurity security = dirInfo.GetAccessControl();
                            security.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow));
                            security.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                            dirInfo.SetAccessControl(security);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\APPEDO_LT.exe");
                }
                catch
                {
                    // Do nothing... 
                }
                Process.Start(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\APPEDO_LT_LOAD_GENERATOR.exe");
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
