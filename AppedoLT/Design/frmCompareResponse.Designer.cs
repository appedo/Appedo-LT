namespace AppedoLT
{
    partial class frmCompareResponse
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
            this.radSplitContainer1 = new Telerik.WinControls.UI.RadSplitContainer();
            this.splitPanel1 = new Telerik.WinControls.UI.SplitPanel();
            this.txtRecordedResponse = new System.Windows.Forms.RichTextBox();
            this.splitPanel2 = new Telerik.WinControls.UI.SplitPanel();
            this.txtValidatedResponse = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).BeginInit();
            this.radSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).BeginInit();
            this.splitPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).BeginInit();
            this.splitPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
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
            this.radSplitContainer1.Size = new System.Drawing.Size(923, 477);
            this.radSplitContainer1.TabIndex = 0;
            this.radSplitContainer1.TabStop = false;
            this.radSplitContainer1.Text = "radSplitContainer1";
            // 
            // splitPanel1
            // 
            this.splitPanel1.Controls.Add(this.txtRecordedResponse);
            this.splitPanel1.Location = new System.Drawing.Point(0, 0);
            this.splitPanel1.Name = "splitPanel1";
            // 
            // 
            // 
            this.splitPanel1.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel1.Size = new System.Drawing.Size(460, 477);
            this.splitPanel1.TabIndex = 0;
            this.splitPanel1.TabStop = false;
            this.splitPanel1.Text = "splitPanel1";
            // 
            // txtRecordedResponse
            // 
            this.txtRecordedResponse.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRecordedResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRecordedResponse.Location = new System.Drawing.Point(0, 0);
            this.txtRecordedResponse.Name = "txtRecordedResponse";
            this.txtRecordedResponse.Size = new System.Drawing.Size(460, 477);
            this.txtRecordedResponse.TabIndex = 0;
            this.txtRecordedResponse.Text = "";
            this.txtRecordedResponse.WordWrap = false;
            // 
            // splitPanel2
            // 
            this.splitPanel2.Controls.Add(this.txtValidatedResponse);
            this.splitPanel2.Location = new System.Drawing.Point(463, 0);
            this.splitPanel2.Name = "splitPanel2";
            // 
            // 
            // 
            this.splitPanel2.RootElement.MinSize = new System.Drawing.Size(25, 25);
            this.splitPanel2.Size = new System.Drawing.Size(460, 477);
            this.splitPanel2.TabIndex = 1;
            this.splitPanel2.TabStop = false;
            this.splitPanel2.Text = "splitPanel2";
            // 
            // txtValidatedResponse
            // 
            this.txtValidatedResponse.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtValidatedResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtValidatedResponse.Location = new System.Drawing.Point(0, 0);
            this.txtValidatedResponse.Name = "txtValidatedResponse";
            this.txtValidatedResponse.Size = new System.Drawing.Size(460, 477);
            this.txtValidatedResponse.TabIndex = 1;
            this.txtValidatedResponse.Text = "";
            this.txtValidatedResponse.WordWrap = false;
            // 
            // frmCompareResponse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 477);
            this.Controls.Add(this.radSplitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Name = "frmCompareResponse";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "Recorded Response  Vs  Validated Response";
            this.ThemeName = "ControlDefault";
            ((System.ComponentModel.ISupportInitialize)(this.radSplitContainer1)).EndInit();
            this.radSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel1)).EndInit();
            this.splitPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanel2)).EndInit();
            this.splitPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadSplitContainer radSplitContainer1;
        private Telerik.WinControls.UI.SplitPanel splitPanel1;
        private Telerik.WinControls.UI.SplitPanel splitPanel2;
        private System.Windows.Forms.RichTextBox txtRecordedResponse;
        private System.Windows.Forms.RichTextBox txtValidatedResponse;


    }
}

