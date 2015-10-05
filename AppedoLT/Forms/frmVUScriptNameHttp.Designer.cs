namespace AppedoLT
{
    partial class frmVUScriptNameHttp
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
            this.txtName = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.btnOk = new Telerik.WinControls.UI.RadButton();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.rbtnFirefox = new Telerik.WinControls.UI.RadRadioButton();
            this.rbtnOthers = new Telerik.WinControls.UI.RadRadioButton();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.txtOpenurl = new Telerik.WinControls.UI.RadTextBox();
            this.erpRequired = new System.Windows.Forms.ErrorProvider(this.components);
            this.ddlParentContainer = new Telerik.WinControls.UI.RadComboBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.radComboBoxItem1 = new Telerik.WinControls.UI.RadComboBoxItem();
            this.radComboBoxItem2 = new Telerik.WinControls.UI.RadComboBoxItem();
            this.radComboBoxItem3 = new Telerik.WinControls.UI.RadComboBoxItem();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnFirefox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnOthers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenurl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.erpRequired)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlParentContainer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(112, 5);
            this.txtName.Multiline = true;
            this.txtName.Name = "txtName";
            // 
            // 
            // 
            this.txtName.RootElement.StretchVertically = true;
            this.txtName.Size = new System.Drawing.Size(272, 18);
            this.txtName.TabIndex = 0;
            this.txtName.TabStop = false;
            // 
            // radLabel1
            // 
            this.radLabel1.Location = new System.Drawing.Point(1, 5);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(105, 17);
            this.radLabel1.TabIndex = 1;
            this.radLabel1.Text = "VU Script Name :";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(112, 105);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(73, 24);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "&Ok";
            this.btnOk.ThemeName = "Telerik";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(193, 105);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.ThemeName = "Telerik";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // radLabel2
            // 
            this.radLabel2.Location = new System.Drawing.Point(1, 51);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(62, 17);
            this.radLabel2.TabIndex = 4;
            this.radLabel2.Text = "Browser :";
            // 
            // rbtnFirefox
            // 
            this.rbtnFirefox.Location = new System.Drawing.Point(112, 50);
            this.rbtnFirefox.Name = "rbtnFirefox";
            this.rbtnFirefox.Size = new System.Drawing.Size(110, 18);
            this.rbtnFirefox.TabIndex = 2;
            this.rbtnFirefox.Text = "&Firefox";
            this.rbtnFirefox.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            // 
            // rbtnOthers
            // 
            this.rbtnOthers.Location = new System.Drawing.Point(228, 50);
            this.rbtnOthers.Name = "rbtnOthers";
            this.rbtnOthers.Size = new System.Drawing.Size(110, 18);
            this.rbtnOthers.TabIndex = 3;
            this.rbtnOthers.Text = "&Other";
            // 
            // radLabel3
            // 
            this.radLabel3.Location = new System.Drawing.Point(1, 28);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(72, 17);
            this.radLabel3.TabIndex = 8;
            this.radLabel3.Text = "Open URL :";
            // 
            // txtOpenurl
            // 
            this.txtOpenurl.Location = new System.Drawing.Point(112, 28);
            this.txtOpenurl.Multiline = true;
            this.txtOpenurl.Name = "txtOpenurl";
            // 
            // 
            // 
            this.txtOpenurl.RootElement.StretchVertically = true;
            this.txtOpenurl.Size = new System.Drawing.Size(272, 18);
            this.txtOpenurl.TabIndex = 1;
            this.txtOpenurl.TabStop = false;
            this.txtOpenurl.Text = "http://";
            // 
            // erpRequired
            // 
            this.erpRequired.ContainerControl = this;
            // 
            // ddlParentContainer
            // 
            this.ddlParentContainer.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.ddlParentContainer.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radComboBoxItem1,
            this.radComboBoxItem2,
            this.radComboBoxItem3});
            this.ddlParentContainer.Location = new System.Drawing.Point(112, 74);
            this.ddlParentContainer.Name = "ddlParentContainer";
            // 
            // 
            // 
            this.ddlParentContainer.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.ddlParentContainer.Size = new System.Drawing.Size(119, 21);
            this.ddlParentContainer.TabIndex = 9;
            this.ddlParentContainer.TabStop = false;
            this.ddlParentContainer.ThemeName = "Telerik";
            // 
            // radLabel4
            // 
            this.radLabel4.Location = new System.Drawing.Point(1, 74);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(70, 17);
            this.radLabel4.TabIndex = 10;
            this.radLabel4.Text = "Container :";
            // 
            // radComboBoxItem1
            // 
            this.radComboBoxItem1.Name = "radComboBoxItem1";
            this.radComboBoxItem1.Text = "Initialize";
            // 
            // radComboBoxItem2
            // 
            this.radComboBoxItem2.Name = "radComboBoxItem2";
            this.radComboBoxItem2.Text = "Actions";
            // 
            // radComboBoxItem3
            // 
            this.radComboBoxItem3.Name = "radComboBoxItem3";
            this.radComboBoxItem3.Text = "End";
            // 
            // frmVUScriptNameHttp
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(393, 136);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.ddlParentContainer);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.txtOpenurl);
            this.Controls.Add(this.rbtnOthers);
            this.Controls.Add(this.rbtnFirefox);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.txtName);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmVUScriptNameHttp";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VUScript Name";
            this.ThemeName = "Office2010";
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnFirefox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnOthers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOpenurl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.erpRequired)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlParentContainer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtName;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadButton btnOk;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadRadioButton rbtnFirefox;
        private Telerik.WinControls.UI.RadRadioButton rbtnOthers;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadTextBox txtOpenurl;
        private System.Windows.Forms.ErrorProvider erpRequired;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        public Telerik.WinControls.UI.RadComboBox ddlParentContainer;
        private Telerik.WinControls.UI.RadComboBoxItem radComboBoxItem1;
        private Telerik.WinControls.UI.RadComboBoxItem radComboBoxItem2;
        private Telerik.WinControls.UI.RadComboBoxItem radComboBoxItem3;
    }
}

