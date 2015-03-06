namespace AppedoLT
{
    partial class RequestParameter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RequestParameter));
            this.imglParameter = new System.Windows.Forms.ImageList(this.components);
            this.btnVariableManager = new Telerik.WinControls.UI.RadButton();
            this.btnOk = new Telerik.WinControls.UI.RadButton();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tabsRequestParameter = new Telerik.WinControls.UI.RadTabStrip();
            this.tabiValue = new Telerik.WinControls.UI.TabItem();
            this.txtValue = new Telerik.WinControls.UI.RadTextBox();
            this.tvVariables = new Telerik.WinControls.UI.RadTreeView();
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.office2007SilverTheme1 = new Telerik.WinControls.Themes.Office2007SilverTheme();
            ((System.ComponentModel.ISupportInitialize)(this.btnVariableManager)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabsRequestParameter)).BeginInit();
            this.tabsRequestParameter.SuspendLayout();
            this.tabiValue.ContentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvVariables)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // imglParameter
            // 
            this.imglParameter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglParameter.ImageStream")));
            this.imglParameter.TransparentColor = System.Drawing.Color.Transparent;
            this.imglParameter.Images.SetKeyName(0, "variable.gif");
            // 
            // btnVariableManager
            // 
            this.btnVariableManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnVariableManager.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVariableManager.Location = new System.Drawing.Point(87, 414);
            this.btnVariableManager.Name = "btnVariableManager";
            this.btnVariableManager.Size = new System.Drawing.Size(135, 24);
            this.btnVariableManager.TabIndex = 1;
            this.btnVariableManager.Text = "&Variable Manager";
            this.btnVariableManager.ThemeName = "Telerik";
            this.btnVariableManager.Click += new System.EventHandler(this.btnVariableManager_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Location = new System.Drawing.Point(87, 444);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(61, 24);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "&Ok";
            this.btnOk.ThemeName = "Telerik";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(161, 445);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.ThemeName = "Telerik";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(7, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Available Variable";
            // 
            // tabsRequestParameter
            // 
            this.tabsRequestParameter.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.tabsRequestParameter.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.tabsRequestParameter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.tabsRequestParameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsRequestParameter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabsRequestParameter.ForeColor = System.Drawing.Color.Black;
            this.tabsRequestParameter.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.tabiValue});
            this.tabsRequestParameter.Location = new System.Drawing.Point(0, 0);
            this.tabsRequestParameter.Name = "tabsRequestParameter";
            // 
            // 
            // 
            this.tabsRequestParameter.RootElement.ForeColor = System.Drawing.Color.Black;
            this.tabsRequestParameter.ScrollOffsetStep = 5;
            this.tabsRequestParameter.Size = new System.Drawing.Size(316, 147);
            this.tabsRequestParameter.TabIndex = 7;
            this.tabsRequestParameter.TabScrollButtonsPosition = Telerik.WinControls.UI.TabScrollButtonsPosition.RightBottom;
            this.tabsRequestParameter.Text = "radTabStrip1";
            this.tabsRequestParameter.ThemeName = "Telerik";
            // 
            // tabiValue
            // 
            this.tabiValue.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tabiValue.ContentPanel
            // 
            this.tabiValue.ContentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.tabiValue.ContentPanel.CausesValidation = true;
            this.tabiValue.ContentPanel.Controls.Add(this.txtValue);
            this.tabiValue.ContentPanel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabiValue.ContentPanel.ForeColor = System.Drawing.Color.Black;
            this.tabiValue.ContentPanel.Location = new System.Drawing.Point(0, 24);
            this.tabiValue.ContentPanel.Size = new System.Drawing.Size(316, 123);
            this.tabiValue.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabiValue.IsSelected = true;
            this.tabiValue.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.tabiValue.Name = "tabiValue";
            this.tabiValue.StretchHorizontally = false;
            this.tabiValue.Text = "Value";
            // 
            // txtValue
            // 
            this.txtValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtValue.Location = new System.Drawing.Point(0, 0);
            this.txtValue.Multiline = true;
            this.txtValue.Name = "txtValue";
            // 
            // 
            // 
            this.txtValue.RootElement.StretchVertically = true;
            this.txtValue.Size = new System.Drawing.Size(316, 123);
            this.txtValue.TabIndex = 0;
            this.txtValue.TabStop = false;
            // 
            // tvVariables
            // 
            this.tvVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvVariables.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvVariables.ForeColor = System.Drawing.Color.Black;
            this.tvVariables.ImageList = this.imglParameter;
            this.tvVariables.Location = new System.Drawing.Point(2, 28);
            this.tvVariables.Name = "tvVariables";
            // 
            // 
            // 
            this.tvVariables.RootElement.ForeColor = System.Drawing.Color.Black;
            this.tvVariables.Size = new System.Drawing.Size(311, 226);
            this.tvVariables.TabIndex = 3;
            this.tvVariables.Text = "radTreeView1";
            this.tvVariables.ThemeName = "Telerik";
            this.tvVariables.SelectedNodeChanged += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(this.tvVariables_SelectedNodeChanged);
            // 
            // radSplitContainer1
            // 
            this.radSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radSplitContainer1.Controls.Add(this.splitPanel1);
            this.radSplitContainer1.Controls.Add(this.splitPanel2);
            this.radSplitContainer1.Location = new System.Drawing.Point(0, 2);
            this.radSplitContainer1.Name = "radSplitContainer1";
            this.radSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // 
            // 
            this.radSplitContainer1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.radSplitContainer1.Size = new System.Drawing.Size(316, 405);
            this.radSplitContainer1.TabIndex = 8;
            this.radSplitContainer1.TabStop = false;
            this.radSplitContainer1.Text = "radSplitContainer1";
            this.radSplitContainer1.ThemeName = "Office2010";
            // 
            // splitPanel1
            // 
            this.splitPanel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitPanel1.Controls.Add(this.tabsRequestParameter);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel1.Size = new System.Drawing.Size(316, 147);
            this.splitPanel1.SizeInfo.AutoSizeScale = new System.Drawing.SizeF(0F, -0.1340852F);
            this.splitPanel1.SizeInfo.SplitterCorrection = new System.Drawing.Size(0, -93);
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            this.splitPanel1.ThemeName = "Office2010";
            // 
            // splitPanel2
            // 
            this.splitPanel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.splitPanel2.Controls.Add(this.label2);
            this.splitPanel2.Controls.Add(this.tvVariables);
            this.splitPanel2.Location = new System.Drawing.Point(0, 150);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel2.Size = new System.Drawing.Size(316, 255);
            this.splitPanel2.SizeInfo.AutoSizeScale = new System.Drawing.SizeF(0F, 0.1340852F);
            this.splitPanel2.SizeInfo.SplitterCorrection = new System.Drawing.Size(0, 93);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            this.splitPanel2.Text = "splitPanel2";
            this.splitPanel2.ThemeName = "Office2010";
            // 
            // RequestParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(315, 470);
            this.Controls.Add(this.radSplitContainer1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnVariableManager);
            this.Name = "RequestParameter";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Request Parameter";
            this.ThemeName = "Vista";
            ((System.ComponentModel.ISupportInitialize)(this.btnVariableManager)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabsRequestParameter)).EndInit();
            this.tabsRequestParameter.ResumeLayout(false);
            this.tabiValue.ContentPanel.ResumeLayout(false);
            this.tabiValue.ContentPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvVariables)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            this.splitPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnVariableManager;
        private Telerik.WinControls.UI.RadButton btnOk;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private System.Windows.Forms.Label label2;
        private Telerik.WinControls.UI.RadTabStrip tabsRequestParameter;
        private System.Windows.Forms.ImageList imglParameter;
        private Telerik.WinControls.UI.RadTreeView tvVariables;
        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private Telerik.WinControls.Themes.Office2007SilverTheme office2007SilverTheme1;
        private Telerik.WinControls.UI.TabItem tabiValue;
        private Telerik.WinControls.UI.RadTextBox txtValue;
    }
}

