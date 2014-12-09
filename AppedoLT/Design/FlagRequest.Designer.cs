namespace AppedoLT
{
    partial class FlagRequest
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
            this.txtText = new Telerik.WinControls.UI.RadTextBox();
            this.telerikTheme1 = new Telerik.WinControls.Themes.TelerikTheme();
            this.rbtnRequestHeader = new Telerik.WinControls.UI.RadRadioButton();
            this.rbtnRequestBody = new Telerik.WinControls.UI.RadRadioButton();
            this.rbtnResponseBody = new Telerik.WinControls.UI.RadRadioButton();
            this.rbtnResponseHeader = new Telerik.WinControls.UI.RadRadioButton();
            this.ddlCondition = new Telerik.WinControls.UI.RadComboBox();
            this.radComboBoxItem1 = new Telerik.WinControls.UI.RadComboBoxItem();
            this.radComboBoxItem2 = new Telerik.WinControls.UI.RadComboBoxItem();
            this.rbtnNone = new Telerik.WinControls.UI.RadRadioButton();
            this.rbtnHasVariable = new Telerik.WinControls.UI.RadRadioButton();
            this.rbtnHasError = new Telerik.WinControls.UI.RadRadioButton();
            this.txtHasVariable = new Telerik.WinControls.UI.RadTextBox();
            this.rbtnDisabled = new Telerik.WinControls.UI.RadRadioButton();
            this.btnOk = new Telerik.WinControls.UI.RadButton();
            this.btnCancel = new Telerik.WinControls.UI.RadButton();
            this.btnVariableSelector = new Telerik.WinControls.UI.RadButton();
            ((System.ComponentModel.ISupportInitialize)(this.txtText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnRequestHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnRequestBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnResponseBody)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnResponseHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlCondition)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnNone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnHasVariable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnHasError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHasVariable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnDisabled)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVariableSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // txtText
            // 
            this.txtText.Enabled = false;
            this.txtText.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtText.Location = new System.Drawing.Point(124, 55);
            this.txtText.Name = "txtText";
            this.txtText.Size = new System.Drawing.Size(358, 21);
            this.txtText.TabIndex = 3;
            this.txtText.TabStop = false;
            this.txtText.ThemeName = "Office2010";
            // 
            // rbtnRequestHeader
            // 
            this.rbtnRequestHeader.Location = new System.Drawing.Point(8, 31);
            this.rbtnRequestHeader.Name = "rbtnRequestHeader";
            this.rbtnRequestHeader.Size = new System.Drawing.Size(110, 18);
            this.rbtnRequestHeader.TabIndex = 5;
            this.rbtnRequestHeader.Text = "Request header";
            this.rbtnRequestHeader.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // rbtnRequestBody
            // 
            this.rbtnRequestBody.Location = new System.Drawing.Point(121, 31);
            this.rbtnRequestBody.Name = "rbtnRequestBody";
            this.rbtnRequestBody.Size = new System.Drawing.Size(122, 18);
            this.rbtnRequestBody.TabIndex = 6;
            this.rbtnRequestBody.Text = "Request parameter";
            this.rbtnRequestBody.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // rbtnResponseBody
            // 
            this.rbtnResponseBody.Location = new System.Drawing.Point(381, 31);
            this.rbtnResponseBody.Name = "rbtnResponseBody";
            this.rbtnResponseBody.Size = new System.Drawing.Size(101, 18);
            this.rbtnResponseBody.TabIndex = 7;
            this.rbtnResponseBody.Text = "Response";
            this.rbtnResponseBody.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // rbtnResponseHeader
            // 
            this.rbtnResponseHeader.Location = new System.Drawing.Point(249, 31);
            this.rbtnResponseHeader.Name = "rbtnResponseHeader";
            this.rbtnResponseHeader.Size = new System.Drawing.Size(110, 18);
            this.rbtnResponseHeader.TabIndex = 7;
            this.rbtnResponseHeader.Text = "Response header";
            this.rbtnResponseHeader.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // ddlCondition
            // 
            this.ddlCondition.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
            this.ddlCondition.Enabled = false;
            this.ddlCondition.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radComboBoxItem1,
            this.radComboBoxItem2});
            this.ddlCondition.Location = new System.Drawing.Point(12, 55);
            this.ddlCondition.Name = "ddlCondition";
            // 
            // 
            // 
            this.ddlCondition.RootElement.AutoSizeMode = Telerik.WinControls.RadAutoSizeMode.WrapAroundChildren;
            this.ddlCondition.Size = new System.Drawing.Size(106, 21);
            this.ddlCondition.TabIndex = 9;
            this.ddlCondition.TabStop = false;
            this.ddlCondition.ThemeName = "Telerik";
            // 
            // radComboBoxItem1
            // 
            this.radComboBoxItem1.Name = "radComboBoxItem1";
            this.radComboBoxItem1.Text = "Contain";
            // 
            // radComboBoxItem2
            // 
            this.radComboBoxItem2.Name = "radComboBoxItem2";
            this.radComboBoxItem2.Text = "Not Contain";
            // 
            // rbtnNone
            // 
            this.rbtnNone.Location = new System.Drawing.Point(8, 5);
            this.rbtnNone.Name = "rbtnNone";
            this.rbtnNone.Size = new System.Drawing.Size(110, 18);
            this.rbtnNone.TabIndex = 10;
            this.rbtnNone.Text = "None";
            this.rbtnNone.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            this.rbtnNone.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // rbtnHasVariable
            // 
            this.rbtnHasVariable.Location = new System.Drawing.Point(8, 80);
            this.rbtnHasVariable.Name = "rbtnHasVariable";
            this.rbtnHasVariable.Size = new System.Drawing.Size(101, 18);
            this.rbtnHasVariable.TabIndex = 17;
            this.rbtnHasVariable.Text = "Has variable";
            this.rbtnHasVariable.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // rbtnHasError
            // 
            this.rbtnHasError.Location = new System.Drawing.Point(8, 106);
            this.rbtnHasError.Name = "rbtnHasError";
            this.rbtnHasError.Size = new System.Drawing.Size(101, 18);
            this.rbtnHasError.TabIndex = 18;
            this.rbtnHasError.Text = "Has Error";
            this.rbtnHasError.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // txtHasVariable
            // 
            this.txtHasVariable.Enabled = false;
            this.txtHasVariable.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHasVariable.Location = new System.Drawing.Point(124, 80);
            this.txtHasVariable.Name = "txtHasVariable";
            this.txtHasVariable.Size = new System.Drawing.Size(358, 21);
            this.txtHasVariable.TabIndex = 19;
            this.txtHasVariable.TabStop = false;
            this.txtHasVariable.ThemeName = "Office2010";
            // 
            // rbtnDisabled
            // 
            this.rbtnDisabled.Location = new System.Drawing.Point(8, 130);
            this.rbtnDisabled.Name = "rbtnDisabled";
            this.rbtnDisabled.Size = new System.Drawing.Size(101, 18);
            this.rbtnDisabled.TabIndex = 20;
            this.rbtnDisabled.Text = "Disabled";
            this.rbtnDisabled.ToggleStateChanged += new Telerik.WinControls.UI.StateChangedEventHandler(this.SelectionChange);
            // 
            // btnOk
            // 
            this.btnOk.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(182, 159);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(61, 24);
            this.btnOk.TabIndex = 22;
            this.btnOk.Text = "&Ok";
            this.btnOk.ThemeName = "Telerik";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(249, 159);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(74, 24);
            this.btnCancel.TabIndex = 23;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.ThemeName = "Telerik";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnVariableSelector
            // 
            this.btnVariableSelector.Enabled = false;
            this.btnVariableSelector.Location = new System.Drawing.Point(338, 164);
            this.btnVariableSelector.Name = "btnVariableSelector";
            this.btnVariableSelector.Size = new System.Drawing.Size(21, 19);
            this.btnVariableSelector.TabIndex = 21;
            this.btnVariableSelector.Text = "...";
            this.btnVariableSelector.Visible = false;
            // 
            // FlagRequest
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(490, 194);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnVariableSelector);
            this.Controls.Add(this.rbtnDisabled);
            this.Controls.Add(this.txtHasVariable);
            this.Controls.Add(this.rbtnHasError);
            this.Controls.Add(this.rbtnHasVariable);
            this.Controls.Add(this.rbtnRequestHeader);
            this.Controls.Add(this.rbtnNone);
            this.Controls.Add(this.ddlCondition);
            this.Controls.Add(this.rbtnResponseHeader);
            this.Controls.Add(this.rbtnResponseBody);
            this.Controls.Add(this.rbtnRequestBody);
            this.Controls.Add(this.txtText);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.Name = "FlagRequest";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Flag Request";
            this.ThemeName = "Vista";
            ((System.ComponentModel.ISupportInitialize)(this.txtText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnRequestHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnRequestBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnResponseBody)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnResponseHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ddlCondition)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnNone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnHasVariable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnHasError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHasVariable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rbtnDisabled)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnVariableSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtText;
        private Telerik.WinControls.Themes.TelerikTheme telerikTheme1;
        private Telerik.WinControls.UI.RadRadioButton rbtnRequestHeader;
        private Telerik.WinControls.UI.RadRadioButton rbtnRequestBody;
        private Telerik.WinControls.UI.RadRadioButton rbtnResponseBody;
        private Telerik.WinControls.UI.RadRadioButton rbtnResponseHeader;
        private Telerik.WinControls.UI.RadComboBox ddlCondition;
        private Telerik.WinControls.UI.RadRadioButton rbtnNone;
        private Telerik.WinControls.UI.RadRadioButton rbtnHasVariable;
        private Telerik.WinControls.UI.RadRadioButton rbtnHasError;
        private Telerik.WinControls.UI.RadTextBox txtHasVariable;
        private Telerik.WinControls.UI.RadRadioButton rbtnDisabled;
        private Telerik.WinControls.UI.RadComboBoxItem radComboBoxItem1;
        private Telerik.WinControls.UI.RadComboBoxItem radComboBoxItem2;
        private Telerik.WinControls.UI.RadButton btnOk;
        private Telerik.WinControls.UI.RadButton btnCancel;
        private Telerik.WinControls.UI.RadButton btnVariableSelector;
    }
}

