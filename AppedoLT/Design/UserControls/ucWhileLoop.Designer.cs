namespace AppedoLT
{
    partial class ucWhileLoop
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
            this.txtWhileLoop = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel8 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtWhileLoop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).BeginInit();
            this.SuspendLayout();
            // 
            // txtWhileLoop
            // 
            this.txtWhileLoop.Location = new System.Drawing.Point(81, 29);
            this.txtWhileLoop.Name = "txtWhileLoop";
            this.txtWhileLoop.Size = new System.Drawing.Size(321, 20);
            this.txtWhileLoop.TabIndex = 17;
            this.txtWhileLoop.TabStop = false;
            this.txtWhileLoop.ThemeName = "Office2010";
            this.txtWhileLoop.Validated += new System.EventHandler(this.txt_Validated);
            // 
            // radLabel8
            // 
            this.radLabel8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel8.Location = new System.Drawing.Point(3, 31);
            this.radLabel8.Name = "radLabel8";
            this.radLabel8.Size = new System.Drawing.Size(75, 18);
            this.radLabel8.TabIndex = 16;
            this.radLabel8.Text = "Condition :";
            // 
            // ucWhileLoop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtWhileLoop);
            this.Controls.Add(this.radLabel8);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ucWhileLoop";
            this.Size = new System.Drawing.Size(474, 76);
            ((System.ComponentModel.ISupportInitialize)(this.txtWhileLoop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel8)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtWhileLoop;
        private Telerik.WinControls.UI.RadLabel radLabel8;
    }
}
