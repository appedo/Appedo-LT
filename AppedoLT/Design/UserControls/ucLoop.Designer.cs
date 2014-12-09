namespace AppedoLT
{
    partial class ucLoop
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
            this.txtLoopCount = new Telerik.WinControls.UI.RadTextBox();
            this.Count = new Telerik.WinControls.UI.RadLabel();
            this.txtName = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoopCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Count)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtLoopCount
            // 
            this.txtLoopCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.txtLoopCount.Location = new System.Drawing.Point(83, 37);
            this.txtLoopCount.Name = "txtLoopCount";
            this.txtLoopCount.Size = new System.Drawing.Size(214, 20);
            this.txtLoopCount.TabIndex = 1;
            this.txtLoopCount.TabStop = false;
            this.txtLoopCount.ThemeName = "Office2010";
            this.txtLoopCount.Validated += new System.EventHandler(this.txt_Validated);
            this.txtLoopCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDelay_KeyPress);
            // 
            // Count
            // 
            this.Count.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.Count.Location = new System.Drawing.Point(25, 39);
            this.Count.Name = "Count";
            this.Count.Size = new System.Drawing.Size(52, 17);
            this.Count.TabIndex = 15;
            this.Count.Text = "Count :";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(83, 11);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(214, 22);
            this.txtName.TabIndex = 0;
            this.txtName.TabStop = false;
            this.txtName.ThemeName = "Office2010";
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold);
            this.radLabel1.Location = new System.Drawing.Point(25, 13);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(51, 17);
            this.radLabel1.TabIndex = 17;
            this.radLabel1.Text = "Name :";
            // 
            // ucLoop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.txtLoopCount);
            this.Controls.Add(this.Count);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ucLoop";
            this.Size = new System.Drawing.Size(555, 70);
            ((System.ComponentModel.ISupportInitialize)(this.txtLoopCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Count)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtLoopCount;
        private Telerik.WinControls.UI.RadLabel Count;
        private Telerik.WinControls.UI.RadTextBox txtName;
        private Telerik.WinControls.UI.RadLabel radLabel1;
    }
}
