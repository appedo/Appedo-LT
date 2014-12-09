namespace AppedoLT
{
    partial class ucLog
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
            this.txtLogName = new Telerik.WinControls.UI.RadTextBox();
            this.lblDelay = new Telerik.WinControls.UI.RadLabel();
            this.lblLogname = new Telerik.WinControls.UI.RadLabel();
            this.txtMessage = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtLogName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblLogname)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLogName
            // 
            this.txtLogName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLogName.Location = new System.Drawing.Point(112, 15);
            this.txtLogName.Name = "txtLogName";
            this.txtLogName.Size = new System.Drawing.Size(392, 21);
            this.txtLogName.TabIndex = 18;
            this.txtLogName.TabStop = false;
            this.txtLogName.ThemeName = "Office2010";
            this.txtLogName.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // lblDelay
            // 
            this.lblDelay.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDelay.Location = new System.Drawing.Point(22, 41);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(69, 17);
            this.lblDelay.TabIndex = 17;
            this.lblDelay.Text = "Message :";
            this.lblDelay.Click += new System.EventHandler(this.lblDelay_Click);
            // 
            // lblLogname
            // 
            this.lblLogname.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblLogname.Location = new System.Drawing.Point(22, 15);
            this.lblLogname.Name = "lblLogname";
            this.lblLogname.Size = new System.Drawing.Size(78, 17);
            this.lblLogname.TabIndex = 16;
            this.lblLogname.Text = "Log Name :";
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(112, 42);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(392, 21);
            this.txtMessage.TabIndex = 19;
            this.txtMessage.TabStop = false;
            this.txtMessage.ThemeName = "Office2010";
            this.txtMessage.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // ucLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtLogName);
            this.Controls.Add(this.lblDelay);
            this.Controls.Add(this.lblLogname);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ucLog";
            this.Size = new System.Drawing.Size(518, 72);
            ((System.ComponentModel.ISupportInitialize)(this.txtLogName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblLogname)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMessage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtLogName;
        private Telerik.WinControls.UI.RadLabel lblDelay;
        private Telerik.WinControls.UI.RadLabel lblLogname;
        private Telerik.WinControls.UI.RadTextBox txtMessage;
    }
}
