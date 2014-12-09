namespace AppedoLT
{
    partial class frmAutoCorrelation
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
            this.btnOk = new Telerik.WinControls.UI.RadButton();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.grdAutoCorrelation = new Telerik.WinControls.UI.RadGridView();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAutoCorrelation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAutoCorrelation.MasterGridViewTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnOk.Location = new System.Drawing.Point(253, 288);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 24);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "&Save";
           // this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Location = new System.Drawing.Point(345, 288);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
           // this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grdAutoCorrelation
            // 
            this.grdAutoCorrelation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAutoCorrelation.HideSelection = true;
            this.grdAutoCorrelation.Location = new System.Drawing.Point(12, 3);
            // 
            // 
            // 
            this.grdAutoCorrelation.MasterGridViewTemplate.AllowDragToGroup = false;
            this.grdAutoCorrelation.MasterGridViewTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            this.grdAutoCorrelation.MasterGridViewTemplate.EnableGrouping = false;
            this.grdAutoCorrelation.Name = "grdAutoCorrelation";
            this.grdAutoCorrelation.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // 
            // 
            this.grdAutoCorrelation.RootElement.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.grdAutoCorrelation.ShowGroupPanel = false;
            this.grdAutoCorrelation.Size = new System.Drawing.Size(667, 279);
            this.grdAutoCorrelation.TabIndex = 38;
            //this.grdAutoCorrelation.CellEndEdit += new Telerik.WinControls.UI.GridViewCellEventHandler(this.grdAutoCorrelation_CellEndEdit);
            // 
            // frmAutoCorrelation
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 324);
            this.Controls.Add(this.grdAutoCorrelation);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAutoCorrelation";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Predefined Correlation";
            this.ThemeName = "ControlDefault";
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAutoCorrelation.MasterGridViewTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdAutoCorrelation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnOk;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.UI.RadGridView grdAutoCorrelation;
    }
}

