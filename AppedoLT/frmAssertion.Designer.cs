namespace AppedoLT
{
    partial class frmAssertion
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
            this.rbtnText = new Telerik.WinControls.UI.RadRadioButton();
            this.rbtnPattern = new Telerik.WinControls.UI.RadRadioButton();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.ddlCondition = new Telerik.WinControls.UI.RadComboBox();
            this.radComboBoxItem1 = new Telerik.WinControls.UI.RadComboBoxItem();
            this.radComboBoxItem2 = new Telerik.WinControls.UI.RadComboBoxItem();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.txtText = new Telerik.WinControls.UI.RadTextBox();
            this.btnSave = new Telerik.WinControls.UI.RadButton();
            this.txtName = new Telerik.WinControls.UI.RadTextBox();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.errAssertion = new System.Windows.Forms.ErrorProvider(this.components);
            this.chkFailedResponse = new Telerik.WinControls.UI.RadCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnPattern)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlCondition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errAssertion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFailedResponse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // rbtnText
            // 
            this.rbtnText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnText.Location = new System.Drawing.Point(87, 69);
            this.rbtnText.Name = "rbtnText";
            this.rbtnText.Size = new System.Drawing.Size(50, 18);
            this.rbtnText.TabIndex = 21;
            this.rbtnText.Text = "Text";
            this.rbtnText.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            // 
            // rbtnPattern
            // 
            this.rbtnPattern.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnPattern.Location = new System.Drawing.Point(164, 72);
            this.rbtnPattern.Name = "rbtnPattern";
            this.rbtnPattern.Size = new System.Drawing.Size(113, 15);
            this.rbtnPattern.TabIndex = 22;
            this.rbtnPattern.Text = "Pattern(Regex)";
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(12, 41);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(60, 17);
            this.radLabel1.TabIndex = 30;
            this.radLabel1.Text = "Condition";
            // 
            // ddlCondition
            // 
            this.ddlCondition.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.ddlCondition.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlCondition.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radComboBoxItem1,
            this.radComboBoxItem2});
            this.ddlCondition.Location = new System.Drawing.Point(87, 41);
            this.ddlCondition.Name = "ddlCondition";
            // 
            // 
            // 
            this.ddlCondition.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.ddlCondition.Size = new System.Drawing.Size(326, 21);
            this.ddlCondition.TabIndex = 31;
            this.ddlCondition.TabStop = false;
            this.ddlCondition.ThemeName = "Telerik";
            // 
            // radComboBoxItem1
            // 
            this.radComboBoxItem1.Name = "radComboBoxItem1";
            this.radComboBoxItem1.Text = "Response contain";
            // 
            // radComboBoxItem2
            // 
            this.radComboBoxItem2.Name = "radComboBoxItem2";
            this.radComboBoxItem2.Text = "Response not contain";
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.Location = new System.Drawing.Point(12, 95);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(31, 17);
            this.radLabel3.TabIndex = 31;
            this.radLabel3.Text = "Text";
            // 
            // txtText
            // 
            this.txtText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtText.Location = new System.Drawing.Point(87, 95);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(326, 19);
            this.txtText.TabIndex = 32;
            this.txtText.TabStop = false;
            this.txtText.ThemeName = "Office2010";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(124, 146);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(83, 24);
            this.btnSave.TabIndex = 33;
            this.btnSave.Text = "&Save";
            this.btnSave.ThemeName = "Telerik";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.Location = new System.Drawing.Point(87, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(326, 19);
            this.txtName.TabIndex = 35;
            this.txtName.TabStop = false;
            this.txtName.ThemeName = "Office2010";
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(12, 12);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(39, 17);
            this.radLabel2.TabIndex = 34;
            this.radLabel2.Text = "Name";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(215, 146);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 24);
            this.btnCancel.TabIndex = 34;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.ThemeName = "Telerik";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(12, 69);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(34, 17);
            this.radLabel4.TabIndex = 36;
            this.radLabel4.Text = "Type";
            // 
            // errAssertion
            // 
            this.errAssertion.ContainerControl = this;
            // 
            // chkFailedResponse
            // 
            this.chkFailedResponse.Location = new System.Drawing.Point(87, 122);
            this.chkFailedResponse.Name = "chkFailedResponse";
            this.chkFailedResponse.Size = new System.Drawing.Size(167, 18);
            this.chkFailedResponse.TabIndex = 37;
            this.chkFailedResponse.Text = "Store assertion failed respose";
            this.chkFailedResponse.Visible = false;
            // 
            // frmAssertion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(432, 179);
            this.Controls.Add(this.chkFailedResponse);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtText);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.rbtnPattern);
            this.Controls.Add(this.ddlCondition);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.rbtnText);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAssertion";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Assertion";
            this.ThemeName = "Vista";
            ((System.ComponentModel.ISupportInitialize)(this.rbtnText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnPattern)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlCondition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errAssertion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkFailedResponse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadRadioButton rbtnText;
        private Telerik.WinControls.UI.RadRadioButton rbtnPattern;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadComboBox ddlCondition;
        private Telerik.WinControls.UI.RadComboBoxItem radComboBoxItem1;
        private Telerik.WinControls.UI.RadComboBoxItem radComboBoxItem2;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private Telerik.WinControls.UI.RadTextBox txtText;
        private Telerik.WinControls.UI.RadButton btnSave;
        private Telerik.WinControls.UI.RadTextBox txtName;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private System.Windows.Forms.ErrorProvider errAssertion;
        private Telerik.WinControls.UI.RadCheckBox chkFailedResponse;
    }
}

