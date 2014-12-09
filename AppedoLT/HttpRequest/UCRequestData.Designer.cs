namespace AppedoLT
{
    partial class UCRequestData
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
            this.radTabStrip1 = new Telerik.WinControls.UI.RadTabStrip();
            this.tabItem1 = new Telerik.WinControls.UI.TabItem();
            this.tapPostParam = new Telerik.WinControls.UI.TabItem();
            ((System.ComponentModel.ISupportInitialize)(this.radTabStrip1)).BeginInit();
            this.radTabStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radTabStrip1
            // 
            this.radTabStrip1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.radTabStrip1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.radTabStrip1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.tabItem1,
            this.tapPostParam});
            this.radTabStrip1.Location = new System.Drawing.Point(3, 3);
            this.radTabStrip1.Name = "radTabStrip1";
            this.radTabStrip1.ScrollOffsetStep = 5;
            this.radTabStrip1.Size = new System.Drawing.Size(1005, 429);
            this.radTabStrip1.TabIndex = 0;
            this.radTabStrip1.TabScrollButtonsPosition = Telerik.WinControls.UI.TabScrollButtonsPosition.RightBottom;
            this.radTabStrip1.Text = "radTabStrip1";
            this.radTabStrip1.ThemeName = "Telerik";
            // 
            // tabItem1
            // 
            this.tabItem1.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tabItem1.ContentPanel
            // 
            this.tabItem1.ContentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.tabItem1.ContentPanel.CausesValidation = true;
            this.tabItem1.ContentPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabItem1.ContentPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabItem1.ContentPanel.Location = new System.Drawing.Point(0, 24);
            this.tabItem1.ContentPanel.Size = new System.Drawing.Size(1005, 405);
            this.tabItem1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.tabItem1.IsSelected = true;
            this.tabItem1.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.tabItem1.Name = "tabItem1";
            this.tabItem1.StretchHorizontally = false;
            this.tabItem1.Text = "Query String Parameters";
            // 
            // tapPostParam
            // 
            this.tapPostParam.Alignment = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tapPostParam.ContentPanel
            // 
            this.tapPostParam.ContentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(248)))));
            this.tapPostParam.ContentPanel.CausesValidation = true;
            this.tapPostParam.ContentPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tapPostParam.ContentPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tapPostParam.ContentPanel.Location = new System.Drawing.Point(0, 24);
            this.tapPostParam.ContentPanel.Size = new System.Drawing.Size(1005, 405);
            this.tapPostParam.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.tapPostParam.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.tapPostParam.Name = "tapPostParam";
            this.tapPostParam.StretchHorizontally = false;
            this.tapPostParam.Text = "Post Parameters";
            // 
            // UCRequestData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radTabStrip1);
            this.Name = "UCRequestData";
            this.Size = new System.Drawing.Size(1011, 471);
            ((System.ComponentModel.ISupportInitialize)(this.radTabStrip1)).EndInit();
            this.radTabStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadTabStrip radTabStrip1;
        private Telerik.WinControls.UI.TabItem tabItem1;
        private Telerik.WinControls.UI.TabItem tapPostParam;

    }
}
