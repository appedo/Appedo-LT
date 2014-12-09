namespace AppedoLT
{
    partial class ucJavaScript
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
            this.txtJavaScript = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtJavaScript
            // 
            this.txtJavaScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJavaScript.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtJavaScript.Location = new System.Drawing.Point(3, 3);
            this.txtJavaScript.Name = "txtJavaScript";
            this.txtJavaScript.Size = new System.Drawing.Size(648, 144);
            this.txtJavaScript.TabIndex = 1;
            this.txtJavaScript.Text = "";
            this.txtJavaScript.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // ucJavaScript
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtJavaScript);
            this.Name = "ucJavaScript";
            this.Size = new System.Drawing.Size(654, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtJavaScript;
    }
}
