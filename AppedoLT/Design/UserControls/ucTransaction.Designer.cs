namespace AppedoLT
{
    partial class ucTransaction
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
            this.txtStartTransactionName = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTransactionName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStartTransactionName
            // 
            this.txtStartTransactionName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStartTransactionName.Location = new System.Drawing.Point(134, 9);
            this.txtStartTransactionName.Name = "txtStartTransactionName";
            this.txtStartTransactionName.Size = new System.Drawing.Size(293, 21);
            this.txtStartTransactionName.TabIndex = 17;
            this.txtStartTransactionName.TabStop = false;
            this.txtStartTransactionName.ThemeName = "Office2010";
            this.txtStartTransactionName.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(3, 9);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(129, 18);
            this.radLabel2.TabIndex = 16;
            this.radLabel2.Text = "Transaction Name :";
            this.radLabel2.ThemeName = "ControlDefault";
            // 
            // ucTransaction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtStartTransactionName);
            this.Controls.Add(this.radLabel2);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ucTransaction";
            this.Size = new System.Drawing.Size(464, 42);
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTransactionName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtStartTransactionName;
        private Telerik.WinControls.UI.RadLabel radLabel2;
    }
}
