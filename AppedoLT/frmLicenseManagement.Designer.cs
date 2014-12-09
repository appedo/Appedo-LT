namespace AppedoLT
{
    partial class frmLicenseManagement
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.ofLicense = new System.Windows.Forms.OpenFileDialog();
            this.txtFilename = new Telerik.WinControls.UI.RadTextBox();
            this.btnBrowse = new Telerik.WinControls.UI.RadButton();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.lblStatus = new Telerik.WinControls.UI.RadLabel();
            this.btnGetLicense = new Telerik.WinControls.UI.RadButton();
            this.telerikTheme1 = new Telerik.WinControls.Themes.TelerikTheme();
            this.vistaTheme1 = new Telerik.WinControls.Themes.VistaTheme();
            this.btnActivate = new Telerik.WinControls.UI.RadButton();
            this.radButton3 = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilename)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBrowse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGetLicense)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnActivate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(2, 44);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(69, 17);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "License file";
            // 
            // ofLicense
            // 
            this.ofLicense.FileName = "openFileDialog1";
            this.ofLicense.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            // 
            // txtFilename
            // 
            this.txtFilename.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilename.Location = new System.Drawing.Point(81, 44);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(263, 19);
            this.txtFilename.TabIndex = 1;
            this.txtFilename.TabStop = false;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowse.Location = new System.Drawing.Point(345, 44);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(27, 20);
            this.btnBrowse.TabIndex = 0;
            this.btnBrowse.Text = "...";
            this.btnBrowse.ThemeName = "Telerik";
            this.btnBrowse.Click += new System.EventHandler(this.radButton1_Click);
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(2, 12);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(43, 17);
            this.radLabel2.TabIndex = 3;
            this.radLabel2.Text = "Status";
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(81, 12);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(64, 17);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "No license";
            // 
            // btnGetLicense
            // 
            this.btnGetLicense.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetLicense.Location = new System.Drawing.Point(180, 79);
            this.btnGetLicense.Name = "btnGetLicense";
            this.btnGetLicense.Size = new System.Drawing.Size(93, 24);
            this.btnGetLicense.TabIndex = 2;
            this.btnGetLicense.Text = "Get license";
            this.btnGetLicense.ThemeName = "Telerik";
            this.btnGetLicense.Click += new System.EventHandler(this.btnGetLicense_Click);
            // 
            // btnActivate
            // 
            this.btnActivate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnActivate.Location = new System.Drawing.Point(81, 79);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(93, 24);
            this.btnActivate.TabIndex = 1;
            this.btnActivate.Text = "&Activate";
            this.btnActivate.ThemeName = "Telerik";
            this.btnActivate.Click += new System.EventHandler(this.btnActivate_Click);
            // 
            // radButton3
            // 
            this.radButton3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radButton3.Location = new System.Drawing.Point(279, 79);
            this.radButton3.Name = "radButton3";
            this.radButton3.Size = new System.Drawing.Size(93, 24);
            this.radButton3.TabIndex = 3;
            this.radButton3.Text = "&Cancel";
            this.radButton3.ThemeName = "Telerik";
            this.radButton3.Click += new System.EventHandler(this.radButton3_Click);
            // 
            // frmLicenseManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(389, 114);
            this.Controls.Add(this.radButton3);
            this.Controls.Add(this.btnActivate);
            this.Controls.Add(this.btnGetLicense);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.radLabel1);
            this.Name = "frmLicenseManagement";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LicenseManagement";
            this.ThemeName = "Vista";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilename)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnBrowse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnGetLicense)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnActivate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButton3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.OpenFileDialog ofLicense;
        private Telerik.WinControls.UI.RadTextBox txtFilename;
        private Telerik.WinControls.UI.RadButton btnBrowse;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadLabel lblStatus;
        private Telerik.WinControls.UI.RadButton btnGetLicense;
        private Telerik.WinControls.Themes.TelerikTheme telerikTheme1;
        private Telerik.WinControls.Themes.VistaTheme vistaTheme1;
        private Telerik.WinControls.UI.RadButton btnActivate;
        private Telerik.WinControls.UI.RadButton radButton3;
    }
}

