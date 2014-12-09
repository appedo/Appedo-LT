namespace AppedoLT
{
    partial class ListBoxMove
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
            this.lstLeft = new Telerik.WinControls.UI.RadListBox();
            this.lstRight = new Telerik.WinControls.UI.RadListBox();
            this.btnMoveRightSingle = new Telerik.WinControls.UI.RadButton();
            this.btnMoveLeftSingle = new Telerik.WinControls.UI.RadButton();
            this.btnMoveRightAll = new Telerik.WinControls.UI.RadButton();
            this.btnMoveLeftAll = new Telerik.WinControls.UI.RadButton();
            this.lblProjectName = new Telerik.WinControls.UI.RadLabel();
            this.lblRight = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.lstLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveRightSingle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveLeftSingle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveRightAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveLeftAll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblProjectName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblRight)).BeginInit();
            this.SuspendLayout();
            // 
            // lstLeft
            // 
            this.lstLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLeft.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstLeft.Location = new System.Drawing.Point(3, 24);
            this.lstLeft.Name = "lstLeft";
            this.lstLeft.Size = new System.Drawing.Size(153, 132);
            this.lstLeft.TabIndex = 0;
            this.lstLeft.ThemeName = "Telerik";
            // 
            // lstRight
            // 
            this.lstRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstRight.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstRight.Location = new System.Drawing.Point(218, 24);
            this.lstRight.Name = "lstRight";
            this.lstRight.Size = new System.Drawing.Size(153, 132);
            this.lstRight.TabIndex = 5;
            this.lstRight.ThemeName = "Telerik";
            // 
            // btnMoveRightSingle
            // 
            this.btnMoveRightSingle.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveRightSingle.Location = new System.Drawing.Point(166, 64);
            this.btnMoveRightSingle.Name = "btnMoveRightSingle";
            this.btnMoveRightSingle.Size = new System.Drawing.Size(43, 24);
            this.btnMoveRightSingle.TabIndex = 2;
            this.btnMoveRightSingle.Text = ">";
            this.btnMoveRightSingle.ThemeName = "Telerik";
            this.btnMoveRightSingle.Click += new System.EventHandler(this.btnMoveRightSingle_Click);
            // 
            // btnMoveLeftSingle
            // 
            this.btnMoveLeftSingle.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveLeftSingle.Location = new System.Drawing.Point(166, 34);
            this.btnMoveLeftSingle.Name = "btnMoveLeftSingle";
            this.btnMoveLeftSingle.Size = new System.Drawing.Size(43, 24);
            this.btnMoveLeftSingle.TabIndex = 1;
            this.btnMoveLeftSingle.Text = "<";
            this.btnMoveLeftSingle.ThemeName = "Telerik";
            this.btnMoveLeftSingle.Click += new System.EventHandler(this.btnMoveLeftSingle_Click);
            // 
            // btnMoveRightAll
            // 
            this.btnMoveRightAll.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveRightAll.Location = new System.Drawing.Point(166, 124);
            this.btnMoveRightAll.Name = "btnMoveRightAll";
            this.btnMoveRightAll.Size = new System.Drawing.Size(43, 24);
            this.btnMoveRightAll.TabIndex = 4;
            this.btnMoveRightAll.Text = ">>";
            this.btnMoveRightAll.ThemeName = "Telerik";
            this.btnMoveRightAll.Click += new System.EventHandler(this.btnMoveRightAll_Click);
            // 
            // btnMoveLeftAll
            // 
            this.btnMoveLeftAll.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveLeftAll.Location = new System.Drawing.Point(166, 94);
            this.btnMoveLeftAll.Name = "btnMoveLeftAll";
            this.btnMoveLeftAll.Size = new System.Drawing.Size(43, 24);
            this.btnMoveLeftAll.TabIndex = 3;
            this.btnMoveLeftAll.Text = "<<";
            this.btnMoveLeftAll.ThemeName = "Telerik";
            this.btnMoveLeftAll.Click += new System.EventHandler(this.btnMoveLeftAll_Click);
            // 
            // lblProjectName
            // 
            this.lblProjectName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProjectName.Location = new System.Drawing.Point(6, 2);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(93, 17);
            this.lblProjectName.TabIndex = 94;
            this.lblProjectName.Text = "Mapped Scripts";
            // 
            // lblRight
            // 
            this.lblRight.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRight.Location = new System.Drawing.Point(218, 2);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(100, 17);
            this.lblRight.TabIndex = 94;
            this.lblRight.Text = "Available Scripts";
            // 
            // ListBoxMove
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblProjectName);
            this.Controls.Add(this.btnMoveLeftSingle);
            this.Controls.Add(this.btnMoveLeftAll);
            this.Controls.Add(this.btnMoveRightAll);
            this.Controls.Add(this.btnMoveRightSingle);
            this.Controls.Add(this.lstRight);
            this.Controls.Add(this.lstLeft);
            this.Name = "ListBoxMove";
            this.Size = new System.Drawing.Size(375, 160);
            this.EnabledChanged += new System.EventHandler(this.ListBoxMove_EnabledChanged);
            ((System.ComponentModel.ISupportInitialize)(this.lstLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveRightSingle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveLeftSingle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveRightAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMoveLeftAll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblProjectName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private  Telerik.WinControls.UI.RadListBox lstLeft;
        private  Telerik.WinControls.UI.RadListBox lstRight;
        private Telerik.WinControls.UI.RadButton btnMoveRightSingle;
        private Telerik.WinControls.UI.RadButton btnMoveLeftSingle;
        private Telerik.WinControls.UI.RadButton btnMoveRightAll;
        private Telerik.WinControls.UI.RadButton btnMoveLeftAll;
        private Telerik.WinControls.UI.RadLabel lblProjectName;
        private Telerik.WinControls.UI.RadLabel lblRight;
    }
}
