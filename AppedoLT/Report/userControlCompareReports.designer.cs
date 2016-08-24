using System.Drawing;
namespace AppedoLT
{
    partial class userControlCompareReports
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
            this.cntmSave = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.radPanel2 = new Telerik.WinControls.UI.RadPanel();
            this.radGridReport = new Telerik.WinControls.UI.RadGridView();
            this.gridViewTemplate1 = new Telerik.WinControls.UI.GridViewTemplate();
            this.panel1 = new Telerik.WinControls.UI.RadPanel();
            this.btnCompare = new Telerik.WinControls.UI.RadButton();
            this.cmbScript = new System.Windows.Forms.ComboBox();
            this.cmbReport3 = new System.Windows.Forms.ComboBox();
            this.lblReport3 = new System.Windows.Forms.Label();
            this.cmbReport2 = new System.Windows.Forms.ComboBox();
            this.lblReport2 = new System.Windows.Forms.Label();
            this.cmbReport1 = new System.Windows.Forms.ComboBox();
            this.lblReport1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.brwReportView = new System.Windows.Forms.WebBrowser();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSave = new Telerik.WinControls.UI.RadButton();
            this.cntmSave.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).BeginInit();
            this.radPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radGridReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridReport.MasterGridViewTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTemplate1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel1)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCompare)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            this.SuspendLayout();
            // 
            // cntmSave
            // 
            this.cntmSave.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cntmSave.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cntmSave.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.cntmSave.Name = "cntmUVScript";
            this.cntmSave.Size = new System.Drawing.Size(106, 26);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.AutoToolTip = true;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
            this.deleteToolStripMenuItem.Text = "&Save";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // radSplitContainer1
            // 
            this.radSplitContainer1.Controls.Add(this.splitPanel1);
            this.radSplitContainer1.Controls.Add(this.splitPanel2);
            this.radSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.radSplitContainer1.Name = "radSplitContainer1";
            // 
            // 
            // 
            this.radSplitContainer1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.radSplitContainer1.Size = new System.Drawing.Size(825, 704);
            this.radSplitContainer1.TabIndex = 47;
            this.radSplitContainer1.TabStop = false;
            this.radSplitContainer1.Text = "radSplitContainer1";
            // 
            // splitPanel1
            // 
            this.splitPanel1.Controls.Add(this.radPanel2);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel1.Size = new System.Drawing.Size(300, 704);
            this.splitPanel1.SizeInfo.AbsoluteSize = new System.Drawing.Size(300, 150);
            this.splitPanel1.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Absolute;
            this.splitPanel1.SizeInfo.SplitterCorrection = new System.Drawing.Size(49, 0);
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            // 
            // radPanel2
            // 
            this.radPanel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.radPanel2.Controls.Add(this.radGridReport);
            this.radPanel2.Controls.Add(this.panel1);
            this.radPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPanel2.Location = new System.Drawing.Point(0, 0);
            this.radPanel2.Name = "radPanel2";
            this.radPanel2.Size = new System.Drawing.Size(300, 704);
            this.radPanel2.TabIndex = 2;
            this.radPanel2.Text = "radPanel2";
            // 
            // radGridReport
            // 
            this.radGridReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radGridReport.ForeColor = System.Drawing.Color.Black;
            this.radGridReport.Location = new System.Drawing.Point(0, 242);
            // 
            // 
            // 
            this.radGridReport.MasterGridViewTemplate.AllowAddNewRow = false;
            this.radGridReport.MasterGridViewTemplate.AllowDeleteRow = false;
            this.radGridReport.MasterGridViewTemplate.AllowEditRow = false;
            this.radGridReport.MasterGridViewTemplate.ChildGridViewTemplates.AddRange(new Telerik.WinControls.UI.GridViewTemplate[] {
            this.gridViewTemplate1});
            this.radGridReport.MasterGridViewTemplate.EnableFiltering = true;
            this.radGridReport.Name = "radGridReport";
            this.radGridReport.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // 
            // 
            this.radGridReport.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radGridReport.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.radGridReport.Size = new System.Drawing.Size(300, 462);
            this.radGridReport.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.btnCompare);
            this.panel1.Controls.Add(this.cmbScript);
            this.panel1.Controls.Add(this.cmbReport3);
            this.panel1.Controls.Add(this.lblReport3);
            this.panel1.Controls.Add(this.cmbReport2);
            this.panel1.Controls.Add(this.lblReport2);
            this.panel1.Controls.Add(this.cmbReport1);
            this.panel1.Controls.Add(this.lblReport1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 242);
            this.panel1.TabIndex = 3;
            // 
            // btnCompare
            // 
            this.btnCompare.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCompare.Location = new System.Drawing.Point(168, 212);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(93, 24);
            this.btnCompare.TabIndex = 51;
            this.btnCompare.Text = "Compare";
            this.btnCompare.ThemeName = "Telerik";
            this.btnCompare.Visible = false;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // cmbScript
            // 
            this.cmbScript.DisplayMember = "Name";
            this.cmbScript.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScript.FormattingEnabled = true;
            this.cmbScript.Location = new System.Drawing.Point(16, 33);
            this.cmbScript.Name = "cmbScript";
            this.cmbScript.Size = new System.Drawing.Size(248, 21);
            this.cmbScript.TabIndex = 50;
            this.cmbScript.ValueMember = "Id";
            this.cmbScript.SelectedIndexChanged += new System.EventHandler(this.cmbScript_SelectedIndexChanged);
            // 
            // cmbReport3
            // 
            this.cmbReport3.DisplayMember = "Key";
            this.cmbReport3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReport3.FormattingEnabled = true;
            this.cmbReport3.Location = new System.Drawing.Point(13, 176);
            this.cmbReport3.Name = "cmbReport3";
            this.cmbReport3.Size = new System.Drawing.Size(248, 21);
            this.cmbReport3.TabIndex = 49;
            this.cmbReport3.ValueMember = "Value";
            this.cmbReport3.Visible = false;
            // 
            // lblReport3
            // 
            this.lblReport3.AutoSize = true;
            this.lblReport3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReport3.Location = new System.Drawing.Point(13, 155);
            this.lblReport3.Name = "lblReport3";
            this.lblReport3.Size = new System.Drawing.Size(66, 14);
            this.lblReport3.TabIndex = 48;
            this.lblReport3.Text = "Report 3:";
            this.lblReport3.Visible = false;
            // 
            // cmbReport2
            // 
            this.cmbReport2.DisplayMember = "Key";
            this.cmbReport2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReport2.FormattingEnabled = true;
            this.cmbReport2.Location = new System.Drawing.Point(13, 127);
            this.cmbReport2.Name = "cmbReport2";
            this.cmbReport2.Size = new System.Drawing.Size(248, 21);
            this.cmbReport2.TabIndex = 47;
            this.cmbReport2.ValueMember = "Value";
            this.cmbReport2.Visible = false;
            this.cmbReport2.SelectedIndexChanged += new System.EventHandler(this.cmbReport2_SelectedIndexChanged);
            // 
            // lblReport2
            // 
            this.lblReport2.AutoSize = true;
            this.lblReport2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReport2.Location = new System.Drawing.Point(13, 106);
            this.lblReport2.Name = "lblReport2";
            this.lblReport2.Size = new System.Drawing.Size(66, 14);
            this.lblReport2.TabIndex = 46;
            this.lblReport2.Text = "Report 2:";
            this.lblReport2.Visible = false;
            // 
            // cmbReport1
            // 
            this.cmbReport1.DisplayMember = "Key";
            this.cmbReport1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReport1.FormattingEnabled = true;
            this.cmbReport1.Location = new System.Drawing.Point(13, 78);
            this.cmbReport1.Name = "cmbReport1";
            this.cmbReport1.Size = new System.Drawing.Size(248, 21);
            this.cmbReport1.TabIndex = 45;
            this.cmbReport1.ValueMember = "Value";
            this.cmbReport1.Visible = false;
            this.cmbReport1.SelectedIndexChanged += new System.EventHandler(this.cmbReport1_SelectedIndexChanged);
            // 
            // lblReport1
            // 
            this.lblReport1.AutoSize = true;
            this.lblReport1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReport1.Location = new System.Drawing.Point(13, 57);
            this.lblReport1.Name = "lblReport1";
            this.lblReport1.Size = new System.Drawing.Size(66, 14);
            this.lblReport1.TabIndex = 21;
            this.lblReport1.Text = "Report 1:";
            this.lblReport1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 14);
            this.label1.TabIndex = 20;
            this.label1.Text = "Select Script";
            // 
            // splitPanel2
            // 
            this.splitPanel2.Controls.Add(this.brwReportView);
            this.splitPanel2.Controls.Add(this.panel2);
            this.splitPanel2.Location = new System.Drawing.Point(303, 0);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel2.Size = new System.Drawing.Size(522, 704);
            this.splitPanel2.SizeInfo.AutoSizeScale = new System.Drawing.SizeF(-0.05352798F, 0F);
            this.splitPanel2.SizeInfo.SplitterCorrection = new System.Drawing.Size(-49, 0);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            this.splitPanel2.Text = "splitPanel2";
            // 
            // brwReportView
            // 
            this.brwReportView.ContextMenuStrip = this.cntmSave;
            this.brwReportView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brwReportView.Location = new System.Drawing.Point(0, 36);
            this.brwReportView.MinimumSize = new System.Drawing.Size(20, 20);
            this.brwReportView.Name = "brwReportView";
            this.brwReportView.ScriptErrorsSuppressed = true;
            this.brwReportView.Size = new System.Drawing.Size(522, 668);
            this.brwReportView.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(522, 36);
            this.panel2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 14);
            this.label2.TabIndex = 21;
            this.label2.Text = "Compare Reports";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(411, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(93, 24);
            this.btnSave.TabIndex = 52;
            this.btnSave.Text = "Save Report";
            this.btnSave.ThemeName = "Telerik";
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // userControlCompareReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radSplitContainer1);
            this.Name = "userControlCompareReports";
            this.Size = new System.Drawing.Size(825, 704);
            this.Load += new System.EventHandler(this.userControlCompareReports_Load);
            this.cntmSave.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radPanel2)).EndInit();
            this.radPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radGridReport.MasterGridViewTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewTemplate1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panel1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnCompare)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private Telerik.WinControls.UI.RadPanel radPanel2;
        private System.Windows.Forms.ContextMenuStrip cntmSave;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private Telerik.WinControls.UI.RadGridView radGridReport;
        private Telerik.WinControls.UI.GridViewTemplate gridViewTemplate1;
        private System.Windows.Forms.WebBrowser brwReportView;
        private Telerik.WinControls.UI.RadPanel panel1;
        private System.Windows.Forms.Label lblReport1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbScript;
        private System.Windows.Forms.ComboBox cmbReport3;
        private System.Windows.Forms.Label lblReport3;
        private System.Windows.Forms.ComboBox cmbReport2;
        private System.Windows.Forms.Label lblReport2;
        private System.Windows.Forms.ComboBox cmbReport1;
        private Telerik.WinControls.UI.RadButton btnCompare;
        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private System.Windows.Forms.Panel panel2;
        private Telerik.WinControls.UI.RadButton btnSave;
        private System.Windows.Forms.Label label2;
    }
}
