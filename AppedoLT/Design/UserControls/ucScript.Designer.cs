namespace AppedoLT
{
    partial class ucScript
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnAutoCorrelate = new Telerik.WinControls.UI.RadButton();
            this.chkDynamicReqEnable = new Telerik.WinControls.UI.RadCheckBox();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.txtScriptname = new Telerik.WinControls.UI.RadTextBox();
            this.txtExclutionFileTypes = new Telerik.WinControls.UI.RadTextBox();
            this.lblScriptName = new Telerik.WinControls.UI.RadLabel();
            this.lblFileTypes = new Telerik.WinControls.UI.RadLabel();
            this.btnValidate = new Telerik.WinControls.UI.RadButton();
            this.btnUpload = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.btnAutoCorrelate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDynamicReqEnable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScriptname)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExclutionFileTypes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblScriptName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFileTypes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUpload)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAutoCorrelate
            // 
            this.btnAutoCorrelate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnAutoCorrelate.Location = new System.Drawing.Point(234, 6);
            this.btnAutoCorrelate.Name = "btnAutoCorrelate";
            this.btnAutoCorrelate.Size = new System.Drawing.Size(175, 24);
            this.btnAutoCorrelate.TabIndex = 32;
            this.btnAutoCorrelate.Text = "Predefined Correlation";
            this.btnAutoCorrelate.ThemeName = "Telerik";
            this.btnAutoCorrelate.Visible = false;
            // 
            // chkDynamicReqEnable
            // 
            this.chkDynamicReqEnable.Location = new System.Drawing.Point(253, 105);
            this.chkDynamicReqEnable.Name = "chkDynamicReqEnable";
            this.chkDynamicReqEnable.Size = new System.Drawing.Size(17, 17);
            this.chkDynamicReqEnable.TabIndex = 31;
            this.chkDynamicReqEnable.ThemeName = "Telerik";
            this.chkDynamicReqEnable.Validated += new System.EventHandler(this.chkDynamicReqEnable_Leave);
            // 
            // radLabel6
            // 
            this.radLabel6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.radLabel6.Location = new System.Drawing.Point(3, 104);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(249, 17);
            this.radLabel6.TabIndex = 30;
            this.radLabel6.Text = "Enable Dynamic Secondary Requests :";
            // 
            // txtScriptname
            // 
            this.txtScriptname.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScriptname.Location = new System.Drawing.Point(253, 45);
            this.txtScriptname.Name = "txtScriptname";
            this.txtScriptname.Size = new System.Drawing.Size(252, 19);
            this.txtScriptname.TabIndex = 27;
            this.txtScriptname.TabStop = false;
            this.txtScriptname.ThemeName = "Office2010";
            this.txtScriptname.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // txtExclutionFileTypes
            // 
            this.txtExclutionFileTypes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExclutionFileTypes.Location = new System.Drawing.Point(253, 72);
            this.txtExclutionFileTypes.Name = "txtExclutionFileTypes";
            this.txtExclutionFileTypes.Size = new System.Drawing.Size(252, 18);
            this.txtExclutionFileTypes.TabIndex = 29;
            this.txtExclutionFileTypes.TabStop = false;
            this.txtExclutionFileTypes.ThemeName = "Office2010";
            this.txtExclutionFileTypes.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // lblScriptName
            // 
            this.lblScriptName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblScriptName.Location = new System.Drawing.Point(3, 45);
            this.lblScriptName.Name = "lblScriptName";
            this.lblScriptName.Size = new System.Drawing.Size(92, 17);
            this.lblScriptName.TabIndex = 26;
            this.lblScriptName.Text = "Script Name :";
            // 
            // lblFileTypes
            // 
            this.lblFileTypes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblFileTypes.Location = new System.Drawing.Point(3, 72);
            this.lblFileTypes.Name = "lblFileTypes";
            this.lblFileTypes.Size = new System.Drawing.Size(143, 17);
            this.lblFileTypes.TabIndex = 28;
            this.lblFileTypes.Text = "Exclusion File Types :";
            // 
            // btnValidate
            // 
            this.btnValidate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnValidate.ImageKey = "validate.gif";
            this.btnValidate.Location = new System.Drawing.Point(7, 6);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(96, 24);
            this.btnValidate.TabIndex = 25;
            this.btnValidate.Text = "Validate";
            this.btnValidate.ThemeName = "Telerik";
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnUpload.ImageKey = "validate.gif";
            this.btnUpload.Location = new System.Drawing.Point(109, 6);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(96, 24);
            this.btnUpload.TabIndex = 33;
            this.btnUpload.Text = "&Upload";
            this.btnUpload.ThemeName = "Telerik";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // ucScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnAutoCorrelate);
            this.Controls.Add(this.chkDynamicReqEnable);
            this.Controls.Add(this.radLabel6);
            this.Controls.Add(this.txtScriptname);
            this.Controls.Add(this.txtExclutionFileTypes);
            this.Controls.Add(this.lblScriptName);
            this.Controls.Add(this.lblFileTypes);
            this.Controls.Add(this.btnValidate);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ucScript";
            this.Size = new System.Drawing.Size(555, 171);
            ((System.ComponentModel.ISupportInitialize)(this.btnAutoCorrelate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkDynamicReqEnable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScriptname)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExclutionFileTypes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblScriptName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblFileTypes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnValidate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnUpload)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnAutoCorrelate;
        private Telerik.WinControls.UI.RadCheckBox chkDynamicReqEnable;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadTextBox txtScriptname;
        private Telerik.WinControls.UI.RadTextBox txtExclutionFileTypes;
        private Telerik.WinControls.UI.RadLabel lblScriptName;
        private Telerik.WinControls.UI.RadLabel lblFileTypes;
        private Telerik.WinControls.UI.RadButton btnValidate;
        private Telerik.WinControls.UI.RadButton btnUpload;
    }
}
