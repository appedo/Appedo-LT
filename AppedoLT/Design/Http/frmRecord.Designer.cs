namespace AppedoLT
{
    partial class frmRecord
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
            this.btnStop = new Telerik.WinControls.UI.RadButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.lblRequest = new System.Windows.Forms.Label();
            this.ddlParentContainer = new Telerik.WinControls.UI.RadComboBox();
            this.ddlInitialize = new Telerik.WinControls.UI.RadComboBoxItem();
            this.ddlActions = new Telerik.WinControls.UI.RadComboBoxItem();
            this.ddlEnd = new Telerik.WinControls.UI.RadComboBoxItem();
            this.txtContainer = new Telerik.WinControls.UI.RadTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnStop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlParentContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(3, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(39, 19);
            this.btnStop.TabIndex = 0;
            this.btnStop.Text = "Stop";
            this.btnStop.ThemeName = "Telerik";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(3, 27);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(116, 17);
            this.radLabel1.TabIndex = 2;
            this.radLabel1.Text = "Received Request :";
            // 
            // lblRequest
            // 
            this.lblRequest.AutoSize = true;
            this.lblRequest.Location = new System.Drawing.Point(121, 27);
            this.lblRequest.Name = "lblRequest";
            this.lblRequest.Size = new System.Drawing.Size(41, 13);
            this.lblRequest.TabIndex = 4;
            this.lblRequest.Text = "label1";
            // 
            // ddlParentContainer
            // 
            this.ddlParentContainer.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.ddlParentContainer.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.ddlInitialize,
            this.ddlActions,
            this.ddlEnd});
            this.ddlParentContainer.Location = new System.Drawing.Point(45, 4);
            this.ddlParentContainer.Name = "ddlParentContainer";
            // 
            // 
            // 
            this.ddlParentContainer.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.ddlParentContainer.Size = new System.Drawing.Size(119, 21);
            this.ddlParentContainer.TabIndex = 5;
            this.ddlParentContainer.TabStop = false;
            this.ddlParentContainer.ThemeName = "Telerik";
            // 
            // ddlInitialize
            // 
            this.ddlInitialize.Name = "ddlInitialize";
            this.ddlInitialize.Text = "Initialize";
            // 
            // ddlActions
            // 
            this.ddlActions.Name = "ddlActions";
            this.ddlActions.Text = "Actions";
            // 
            // ddlEnd
            // 
            this.ddlEnd.Name = "ddlEnd";
            this.ddlEnd.Text = "End";
            // 
            // txtContainer
            // 
            this.txtContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContainer.Location = new System.Drawing.Point(166, 4);
            this.txtContainer.Name = "txtContainer";
            this.txtContainer.Size = new System.Drawing.Size(329, 20);
            this.txtContainer.TabIndex = 6;
            this.txtContainer.TabStop = false;
            this.txtContainer.Text = "Container1";
            // 
            // frmRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(502, 43);
            this.Controls.Add(this.txtContainer);
            this.Controls.Add(this.ddlParentContainer);
            this.Controls.Add(this.lblRequest);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.btnStop);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "frmRecord";
            this.Opacity = 0.95;
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Record";
            this.ThemeName = "Vista";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRecord_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.btnStop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlParentContainer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtContainer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadButton btnStop;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.Label lblRequest;
        private Telerik.WinControls.UI.RadComboBox ddlParentContainer;
        private Telerik.WinControls.UI.RadComboBoxItem ddlInitialize;
        private Telerik.WinControls.UI.RadComboBoxItem ddlActions;
        private Telerik.WinControls.UI.RadComboBoxItem ddlEnd;
        private Telerik.WinControls.UI.RadTextBox txtContainer;
    }
}

