namespace AppedoLT
{
    partial class frmReplaceServer
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
            this.tvHostList = new Telerik.WinControls.UI.RadTreeView();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.txtPort = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.txtHost = new Telerik.WinControls.UI.RadTextBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblSchema = new Telerik.WinControls.UI.RadLabel();
            this.txtSchema = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.tvHostList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSchema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSchema)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            this.SuspendLayout();
            // 
            // tvHostList
            // 
            this.tvHostList.AllowDragDrop = true;
            this.tvHostList.AllowMultiselect = true;
            this.tvHostList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvHostList.BackColor = System.Drawing.SystemColors.Window;
            this.tvHostList.Cursor = System.Windows.Forms.Cursors.Default;
            this.tvHostList.EnableKeyMap = true;
            this.tvHostList.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvHostList.ForeColor = System.Drawing.Color.Black;
            this.tvHostList.FullRowSelect = true;
            this.tvHostList.ItemHeight = 20;
            this.tvHostList.Location = new System.Drawing.Point(3, 117);
            this.tvHostList.Name = "tvHostList";
            this.tvHostList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // 
            // 
            this.tvHostList.RootElement.ForeColor = System.Drawing.Color.Black;
            this.tvHostList.Size = new System.Drawing.Size(364, 295);
            this.tvHostList.TabIndex = 39;
            this.tvHostList.Text = "radTreeView1";
            this.tvHostList.ThemeName = "Telerik";
            this.tvHostList.TreeIndent = 30;
            this.tvHostList.SelectedNodeChanged += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(this.tvHostList_SelectedNodeChanged);
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.ForeColor = System.Drawing.Color.Black;
            this.radLabel2.Location = new System.Drawing.Point(3, 66);
            this.radLabel2.Name = "radLabel2";
            // 
            // 
            // 
            this.radLabel2.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel2.Size = new System.Drawing.Size(63, 17);
            this.radLabel2.TabIndex = 43;
            this.radLabel2.Text = "New Port:";
            // 
            // txtPort
            // 
            this.txtPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPort.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPort.Location = new System.Drawing.Point(93, 66);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(77, 19);
            this.txtPort.TabIndex = 42;
            this.txtPort.TabStop = false;
            this.txtPort.ThemeName = "Office2010";
            this.txtPort.Leave += new System.EventHandler(this.txtPort_Validated);
            this.txtPort.Validated += new System.EventHandler(this.txtPort_Validated);
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.ForeColor = System.Drawing.Color.Black;
            this.radLabel1.Location = new System.Drawing.Point(3, 39);
            this.radLabel1.Name = "radLabel1";
            // 
            // 
            // 
            this.radLabel1.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel1.Size = new System.Drawing.Size(66, 17);
            this.radLabel1.TabIndex = 41;
            this.radLabel1.Text = "New Host:";
            // 
            // txtHost
            // 
            this.txtHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHost.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHost.Location = new System.Drawing.Point(93, 39);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(274, 19);
            this.txtHost.TabIndex = 40;
            this.txtHost.TabStop = false;
            this.txtHost.ThemeName = "Office2010";
            this.txtHost.Leave += new System.EventHandler(this.txtPort_Validated);
            this.txtHost.Validated += new System.EventHandler(this.txtPort_Validated);
            // 
            // btnReplace
            // 
            this.btnReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReplace.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnReplace.Location = new System.Drawing.Point(116, 418);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplace.TabIndex = 44;
            this.btnReplace.Text = "&Save";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(197, 418);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 45;
            this.button2.Text = "&Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // lblSchema
            // 
            this.lblSchema.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSchema.ForeColor = System.Drawing.Color.Black;
            this.lblSchema.Location = new System.Drawing.Point(3, 12);
            this.lblSchema.Name = "lblSchema";
            // 
            // 
            // 
            this.lblSchema.RootElement.ForeColor = System.Drawing.Color.Black;
            this.lblSchema.Size = new System.Drawing.Size(85, 17);
            this.lblSchema.TabIndex = 47;
            this.lblSchema.Text = "New Schema:";
            // 
            // txtSchema
            // 
            this.txtSchema.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSchema.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSchema.Location = new System.Drawing.Point(93, 12);
            this.txtSchema.Name = "txtSchema";
            this.txtSchema.Size = new System.Drawing.Size(77, 19);
            this.txtSchema.TabIndex = 46;
            this.txtSchema.TabStop = false;
            this.txtSchema.ThemeName = "Office2010";
            this.txtSchema.Leave += new System.EventHandler(this.txtPort_Validated);
            this.txtSchema.Validated += new System.EventHandler(this.txtPort_Validated);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.ForeColor = System.Drawing.Color.Black;
            this.radLabel4.Location = new System.Drawing.Point(3, 94);
            this.radLabel4.Name = "radLabel4";
            // 
            // 
            // 
            this.radLabel4.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel4.Size = new System.Drawing.Size(58, 17);
            this.radLabel4.TabIndex = 48;
            this.radLabel4.Text = "Current :";
            // 
            // frmReplaceServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 444);
            this.ControlBox = false;
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.lblSchema);
            this.Controls.Add(this.txtSchema);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.txtHost);
            this.Controls.Add(this.tvHostList);
            this.Name = "frmReplaceServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Replace";
            ((System.ComponentModel.ISupportInitialize)(this.tvHostList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblSchema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSchema)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public Telerik.WinControls.UI.RadTreeView tvHostList;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadTextBox txtPort;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox txtHost;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button button2;
        private Telerik.WinControls.UI.RadLabel lblSchema;
        private Telerik.WinControls.UI.RadTextBox txtSchema;
        private Telerik.WinControls.UI.RadLabel radLabel4;
    }
}