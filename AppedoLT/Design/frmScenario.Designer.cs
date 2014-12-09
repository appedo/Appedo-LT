namespace AppedoLT
{
    partial class frmScenario
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
            this.components = new System.ComponentModel.Container();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.txtScenarioName = new Telerik.WinControls.UI.RadTextBox();
            this.btnSave = new Telerik.WinControls.UI.RadButton();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.telerikTheme1 = new Telerik.WinControls.Themes.TelerikTheme();
            this.vistaTheme1 = new Telerik.WinControls.Themes.VistaTheme();
            this.listBoxMove1 = new AppedoLT.ListBoxMove();
            this.chkIPSpoofing = new Telerik.WinControls.UI.RadCheckBox();
            this.erpRequired = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScenarioName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIPSpoofing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.erpRequired)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(9, 9);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(102, 17);
            this.radLabel1.TabIndex = 0;
            this.radLabel1.Text = "Scenario Name :";
            // 
            // txtScenarioName
            // 
            this.txtScenarioName.Location = new System.Drawing.Point(117, 9);
            this.txtScenarioName.Name = "txtScenarioName";
            this.txtScenarioName.Size = new System.Drawing.Size(269, 20);
            this.txtScenarioName.TabIndex = 0;
            this.txtScenarioName.TabStop = false;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(109, 229);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 24);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "&Save";
            this.btnSave.ThemeName = "Telerik";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(201, 229);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.ThemeName = "Telerik";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // listBoxMove1
            // 
            this.listBoxMove1.Location = new System.Drawing.Point(9, 59);
            this.listBoxMove1.Name = "listBoxMove1";
            this.listBoxMove1.Size = new System.Drawing.Size(375, 160);
            this.listBoxMove1.TabIndex = 1;
            // 
            // chkIPSpoofing
            // 
            this.chkIPSpoofing.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkIPSpoofing.ForeColor = System.Drawing.SystemColors.ControlText;
            this.chkIPSpoofing.Location = new System.Drawing.Point(117, 36);
            this.chkIPSpoofing.Name = "chkIPSpoofing";
            this.chkIPSpoofing.Size = new System.Drawing.Size(128, 17);
            this.chkIPSpoofing.TabIndex = 5;
            this.chkIPSpoofing.Text = "Enable IP Spoofing";
            // 
            // erpRequired
            // 
            this.erpRequired.ContainerControl = this;
            // 
            // frmScenario
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(403, 261);
            this.Controls.Add(this.chkIPSpoofing);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.listBoxMove1);
            this.Controls.Add(this.txtScenarioName);
            this.Controls.Add(this.radLabel1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmScenario";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Scenario";
            this.ThemeName = "Vista";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScenarioName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkIPSpoofing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.erpRequired)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox txtScenarioName;
        private ListBoxMove listBoxMove1;
        private Telerik.WinControls.UI.RadButton btnSave;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.Themes.TelerikTheme telerikTheme1;
        private Telerik.WinControls.Themes.VistaTheme vistaTheme1;
        private Telerik.WinControls.UI.RadCheckBox chkIPSpoofing;
        private System.Windows.Forms.ErrorProvider erpRequired;
    }
}

