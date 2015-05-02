namespace AppedoLT
{
    partial class frmScriptNameList
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
            this.btnDownload = new Telerik.WinControls.UI.RadButton();
            this.tvScriptName = new Telerik.WinControls.UI.RadTreeView();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnDownload)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvScriptName)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDownload
            // 
            this.btnDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDownload.BackColor = System.Drawing.SystemColors.Control;
            this.btnDownload.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnDownload.Location = new System.Drawing.Point(86, 366);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(86, 24);
            this.btnDownload.TabIndex = 40;
            this.btnDownload.Text = "&Download";
            this.btnDownload.ThemeName = "Telerik";
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // tvScriptName
            // 
            this.tvScriptName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvScriptName.CheckBoxes = true;
            this.tvScriptName.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvScriptName.Location = new System.Drawing.Point(5, 29);
            this.tvScriptName.Name = "tvScriptName";
            this.tvScriptName.Size = new System.Drawing.Size(273, 331);
            this.tvScriptName.TabIndex = 41;
            this.tvScriptName.Text = "radTreeView1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = "Please select script(s):";
            // 
            // frmScriptNameList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 397);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tvScriptName);
            this.Controls.Add(this.btnDownload);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmScriptNameList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Script Name List";
            ((System.ComponentModel.ISupportInitialize)(this.btnDownload)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvScriptName)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnDownload;
        private Telerik.WinControls.UI.RadTreeView tvScriptName;
        private System.Windows.Forms.Label label1;
        //private Telerik.WinControls.UI.RadTreeNode tvScriptName;
    }
}