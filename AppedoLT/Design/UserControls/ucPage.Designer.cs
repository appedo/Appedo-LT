namespace AppedoLT
{
    partial class ucPage
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
            this.lblPage = new Telerik.WinControls.UI.RadLabel();
            this.txtPageDelay = new Telerik.WinControls.UI.RadTextBox();
            this.lblDelay = new Telerik.WinControls.UI.RadLabel();
            this.lblPagename = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.lblPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPageDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPagename)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPage
            // 
            this.lblPage.Location = new System.Drawing.Point(100, 27);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(48, 17);
            this.lblPage.TabIndex = 19;
            this.lblPage.Text = "Method";
            // 
            // txtPageDelay
            // 
            this.txtPageDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtPageDelay.Location = new System.Drawing.Point(100, 50);
            this.txtPageDelay.Name = "txtPageDelay";
            this.txtPageDelay.Size = new System.Drawing.Size(175, 18);
            this.txtPageDelay.TabIndex = 18;
            this.txtPageDelay.TabStop = false;
            this.txtPageDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPageDelay.ThemeName = "Office2010";
            this.txtPageDelay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDelay_KeyPress);
            this.txtPageDelay.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // lblDelay
            // 
            this.lblDelay.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblDelay.Location = new System.Drawing.Point(3, 52);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(50, 17);
            this.lblDelay.TabIndex = 17;
            this.lblDelay.Text = "Delay :";
            this.lblDelay.Click += new System.EventHandler(this.lblDelay_Click);
            // 
            // lblPagename
            // 
            this.lblPagename.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPagename.Location = new System.Drawing.Point(3, 26);
            this.lblPagename.Name = "lblPagename";
            this.lblPagename.Size = new System.Drawing.Size(86, 17);
            this.lblPagename.TabIndex = 16;
            this.lblPagename.Text = "Page Name :";
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.radLabel1.Location = new System.Drawing.Point(277, 51);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(25, 17);
            this.radLabel1.TabIndex = 20;
            this.radLabel1.Text = "ms";
            // 
            // ucPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.lblPage);
            this.Controls.Add(this.txtPageDelay);
            this.Controls.Add(this.lblDelay);
            this.Controls.Add(this.lblPagename);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ucPage";
            this.Size = new System.Drawing.Size(586, 101);
            this.Load += new System.EventHandler(this.ucPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.lblPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPageDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPagename)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel lblPage;
        private Telerik.WinControls.UI.RadTextBox txtPageDelay;
        private Telerik.WinControls.UI.RadLabel lblDelay;
        private Telerik.WinControls.UI.RadLabel lblPagename;
        private Telerik.WinControls.UI.RadLabel radLabel1;
    }
}
