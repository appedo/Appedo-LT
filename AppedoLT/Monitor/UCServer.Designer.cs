namespace AppedoLT
{
    partial class UCServer
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
            this.components = new System.ComponentModel.Container();
            this.lblName = new Telerik.WinControls.UI.RadLabel();
            this.txtIPAddress = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.txtUserName = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.txtPassword = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.ddlType = new Telerik.WinControls.UI.RadComboBox();
            this.Machine = new Telerik.WinControls.UI.RadComboBoxItem();
            this.btnTest = new Telerik.WinControls.UI.RadButton();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.txtName = new Telerik.WinControls.UI.RadTextBox();
            this.btnShowCounters = new Telerik.WinControls.UI.RadButton();
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.tvAvailableCounters = new Telerik.WinControls.UI.RadTreeView();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.btnUnMap = new System.Windows.Forms.Button();
            this.btnMap = new System.Windows.Forms.Button();
            this.splitPanel3 = new Telerik.WinControls.UI.SplitPanel();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.tvMappedCounters = new Telerik.WinControls.UI.RadTreeView();
            this.radLabel7 = new Telerik.WinControls.UI.RadLabel();
            this.txtInterval = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.lblName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIPAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowCounters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvAvailableCounters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel3)).BeginInit();
            this.splitPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvMappedCounters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(9, 69);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(99, 17);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Machine Name :";
            // 
            // txtIPAddress
            // 
            this.txtIPAddress.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIPAddress.Location = new System.Drawing.Point(108, 69);
            this.txtIPAddress.Name = "txtIPAddress";
            this.txtIPAddress.Size = new System.Drawing.Size(308, 21);
            this.txtIPAddress.TabIndex = 5;
            this.txtIPAddress.TabStop = false;
            this.txtIPAddress.ThemeName = "Office2010";
            this.txtIPAddress.Validated += new System.EventHandler(this.txtIPAddress_Validated);
            // 
            // radLabel1
            // 
            this.radLabel1.Enabled = false;
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(9, 98);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(78, 17);
            this.radLabel1.TabIndex = 8;
            this.radLabel1.Text = "User Name :";
            // 
            // txtUserName
            // 
            this.txtUserName.Enabled = false;
            this.txtUserName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserName.Location = new System.Drawing.Point(108, 98);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(308, 21);
            this.txtUserName.TabIndex = 7;
            this.txtUserName.TabStop = false;
            this.txtUserName.ThemeName = "Office2010";
            this.txtUserName.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // radLabel2
            // 
            this.radLabel2.Enabled = false;
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(9, 127);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(69, 17);
            this.radLabel2.TabIndex = 10;
            this.radLabel2.Text = "Password :";
            // 
            // txtPassword
            // 
            this.txtPassword.Enabled = false;
            this.txtPassword.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(108, 127);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(308, 21);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.TabStop = false;
            this.txtPassword.ThemeName = "Office2010";
            this.txtPassword.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.Location = new System.Drawing.Point(9, 15);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(39, 17);
            this.radLabel3.TabIndex = 12;
            this.radLabel3.Text = "Type:";
            // 
            // ddlType
            // 
            this.ddlType.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.ddlType.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.ddlType.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.Machine});
            this.ddlType.Location = new System.Drawing.Point(108, 15);
            this.ddlType.Name = "ddlType";
            // 
            // 
            // 
            this.ddlType.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.ddlType.Size = new System.Drawing.Size(308, 19);
            this.ddlType.TabIndex = 13;
            this.ddlType.TabStop = false;
            this.ddlType.Text = "radComboBox1";
            // 
            // Machine
            // 
            this.Machine.Name = "Machine";
            this.Machine.Text = "Machine";
            // 
            // btnTest
            // 
            this.btnTest.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTest.Location = new System.Drawing.Point(336, 154);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(80, 24);
            this.btnTest.TabIndex = 14;
            this.btnTest.Text = "&Test";
            this.btnTest.ThemeName = "Telerik";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(9, 42);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(48, 17);
            this.radLabel4.TabIndex = 16;
            this.radLabel4.Text = "Name :";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(108, 41);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(308, 21);
            this.txtName.TabIndex = 15;
            this.txtName.TabStop = false;
            this.txtName.ThemeName = "Office2010";
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // btnShowCounters
            // 
            this.btnShowCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowCounters.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowCounters.Location = new System.Drawing.Point(266, 7);
            this.btnShowCounters.Name = "btnShowCounters";
            this.btnShowCounters.Size = new System.Drawing.Size(102, 24);
            this.btnShowCounters.TabIndex = 17;
            this.btnShowCounters.Text = "Show &Counters";
            this.btnShowCounters.ThemeName = "Telerik";
            this.btnShowCounters.Click += new System.EventHandler(this.btnShowCounters_Click);
            // 
            // radSplitContainer1
            // 
            this.radSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radSplitContainer1.BackColor = System.Drawing.Color.Transparent;
            this.radSplitContainer1.Controls.Add(this.splitPanel1);
            this.radSplitContainer1.Controls.Add(this.splitPanel2);
            this.radSplitContainer1.Controls.Add(this.splitPanel3);
            this.radSplitContainer1.Location = new System.Drawing.Point(9, 186);
            this.radSplitContainer1.Name = "radSplitContainer1";
            // 
            // 
            // 
            this.radSplitContainer1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.radSplitContainer1.Size = new System.Drawing.Size(791, 285);
            this.radSplitContainer1.TabIndex = 18;
            this.radSplitContainer1.TabStop = false;
            this.radSplitContainer1.Text = "radSplitContainer1";
            // 
            // splitPanel1
            // 
            this.splitPanel1.Controls.Add(this.radLabel5);
            this.splitPanel1.Controls.Add(this.btnShowCounters);
            this.splitPanel1.Controls.Add(this.tvAvailableCounters);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel1.Size = new System.Drawing.Size(372, 285);
            this.splitPanel1.SizeInfo.AutoSizeScale = new System.Drawing.SizeF(0.140552F, 0F);
            this.splitPanel1.SizeInfo.SplitterCorrection = new System.Drawing.Size(112, 0);
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            // 
            // radLabel5
            // 
            this.radLabel5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel5.Location = new System.Drawing.Point(5, 3);
            this.radLabel5.Name = "radLabel5";
            this.radLabel5.Size = new System.Drawing.Size(122, 17);
            this.radLabel5.TabIndex = 17;
            this.radLabel5.Text = "Available Counters :";
            // 
            // tvAvailableCounters
            // 
            this.tvAvailableCounters.AccessibleRole = System.Windows.Forms.AccessibleRole.ColumnHeader;
            this.tvAvailableCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvAvailableCounters.CheckBoxes = true;
            this.tvAvailableCounters.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvAvailableCounters.ForeColor = System.Drawing.Color.Black;
            this.tvAvailableCounters.Location = new System.Drawing.Point(5, 37);
            this.tvAvailableCounters.Name = "tvAvailableCounters";
            // 
            // 
            // 
            this.tvAvailableCounters.RootElement.ForeColor = System.Drawing.Color.Black;
            this.tvAvailableCounters.ShowLines = true;
            this.tvAvailableCounters.Size = new System.Drawing.Size(363, 242);
            this.tvAvailableCounters.TabIndex = 0;
            this.tvAvailableCounters.Text = "radTreeView1";
            this.tvAvailableCounters.ThemeName = "Telerik";
            this.tvAvailableCounters.TriStateMode = true;
            // 
            // splitPanel2
            // 
            this.splitPanel2.Controls.Add(this.btnUnMap);
            this.splitPanel2.Controls.Add(this.btnMap);
            this.splitPanel2.Location = new System.Drawing.Point(375, 0);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel2.Size = new System.Drawing.Size(36, 285);
            this.splitPanel2.SizeInfo.AutoSizeScale = new System.Drawing.SizeF(-0.2874735F, 0F);
            this.splitPanel2.SizeInfo.SplitterCorrection = new System.Drawing.Size(-231, 0);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            // 
            // btnUnMap
            // 
            this.btnUnMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnMap.Location = new System.Drawing.Point(2, 158);
            this.btnUnMap.Name = "btnUnMap";
            this.btnUnMap.Size = new System.Drawing.Size(29, 23);
            this.btnUnMap.TabIndex = 1;
            this.btnUnMap.Text = "<<";
            this.btnUnMap.UseVisualStyleBackColor = true;
            this.btnUnMap.Click += new System.EventHandler(this.btnUnMap_Click);
            // 
            // btnMap
            // 
            this.btnMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMap.Location = new System.Drawing.Point(3, 129);
            this.btnMap.Name = "btnMap";
            this.btnMap.Size = new System.Drawing.Size(29, 23);
            this.btnMap.TabIndex = 0;
            this.btnMap.Text = ">>";
            this.btnMap.UseVisualStyleBackColor = true;
            this.btnMap.Click += new System.EventHandler(this.btnMap_Click);
            // 
            // splitPanel3
            // 
            this.splitPanel3.Controls.Add(this.radLabel6);
            this.splitPanel3.Controls.Add(this.tvMappedCounters);
            this.splitPanel3.Location = new System.Drawing.Point(414, 0);
            this.splitPanel3.Name = "splitPanel3";
            // 
            // 
            // 
            this.splitPanel3.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel3.Size = new System.Drawing.Size(377, 285);
            this.splitPanel3.SizeInfo.AutoSizeScale = new System.Drawing.SizeF(0.1469214F, 0F);
            this.splitPanel3.SizeInfo.SplitterCorrection = new System.Drawing.Size(119, 0);
            this.splitPanel3.TabIndex = 2;
            this.splitPanel3.TabStop = false;
            this.splitPanel3.Text = "splitPanel3";
            // 
            // radLabel6
            // 
            this.radLabel6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel6.Location = new System.Drawing.Point(6, 3);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(115, 17);
            this.radLabel6.TabIndex = 18;
            this.radLabel6.Text = "Mapped Counters :";
            // 
            // tvMappedCounters
            // 
            this.tvMappedCounters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvMappedCounters.CheckBoxes = true;
            this.tvMappedCounters.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvMappedCounters.ForeColor = System.Drawing.Color.Black;
            this.tvMappedCounters.Location = new System.Drawing.Point(6, 37);
            this.tvMappedCounters.Name = "tvMappedCounters";
            // 
            // 
            // 
            this.tvMappedCounters.RootElement.ForeColor = System.Drawing.Color.Black;
            this.tvMappedCounters.ShowLines = true;
            this.tvMappedCounters.Size = new System.Drawing.Size(367, 242);
            this.tvMappedCounters.TabIndex = 1;
            this.tvMappedCounters.Text = "radTreeView2";
            this.tvMappedCounters.ThemeName = "Telerik";
            this.tvMappedCounters.TriStateMode = true;
            // 
            // radLabel7
            // 
            this.radLabel7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel7.Location = new System.Drawing.Point(429, 17);
            this.radLabel7.Name = "radLabel7";
            this.radLabel7.Size = new System.Drawing.Size(85, 17);
            this.radLabel7.TabIndex = 20;
            this.radLabel7.Text = "Interval(sec):";
            // 
            // txtInterval
            // 
            this.txtInterval.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInterval.Location = new System.Drawing.Point(520, 15);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(34, 21);
            this.txtInterval.TabIndex = 19;
            this.txtInterval.TabStop = false;
            this.txtInterval.Text = "5";
            this.txtInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtInterval.ThemeName = "Office2010";
            this.txtInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtInterval_KeyPress);
            this.txtInterval.Validated += new System.EventHandler(this.txtInterval_Validated);
            // 
            // UCServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.radLabel7);
            this.Controls.Add(this.txtInterval);
            this.Controls.Add(this.radSplitContainer1);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.ddlType);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtIPAddress);
            this.Name = "UCServer";
            this.Size = new System.Drawing.Size(809, 472);
            ((System.ComponentModel.ISupportInitialize)(this.lblName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtIPAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtUserName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnTest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowCounters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            this.splitPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvAvailableCounters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel3)).EndInit();
            this.splitPanel3.ResumeLayout(false);
            this.splitPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvMappedCounters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel lblName;
        private Telerik.WinControls.UI.RadTextBox txtIPAddress;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadTextBox txtUserName;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadTextBox txtPassword;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadComboBox ddlType;
        private Telerik.WinControls.UI.RadButton btnTest;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadTextBox txtName;
        private Telerik.WinControls.UI.RadComboBoxItem Machine;
        private Telerik.WinControls.UI.RadButton btnShowCounters;
        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private Telerik.WinControls.UI.SplitPanel splitPanel3;
        private Telerik.WinControls.UI.RadTreeView tvAvailableCounters;
        private Telerik.WinControls.UI.RadTreeView tvMappedCounters;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private System.Windows.Forms.Button btnUnMap;
        private System.Windows.Forms.Button btnMap;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadLabel radLabel7;
        private Telerik.WinControls.UI.RadTextBox txtInterval;
    }
}
