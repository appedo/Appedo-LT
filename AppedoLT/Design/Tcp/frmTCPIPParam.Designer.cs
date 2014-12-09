namespace AppedoLT
{
    partial class frmTCPIPParam
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
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.txtStartPosition = new System.Windows.Forms.TextBox();
            this.txtParamName = new System.Windows.Forms.TextBox();
            this.lblPort = new Telerik.WinControls.UI.RadLabel();
            this.lblServerIP = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.txtPaddingChar = new System.Windows.Forms.TextBox();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ddlPaddingType = new System.Windows.Forms.ComboBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.btnVariable = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblServerIP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radLabel6
            // 
            this.radLabel6.AutoSize = false;
            this.radLabel6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel6.Location = new System.Drawing.Point(13, 91);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(116, 18);
            this.radLabel6.TabIndex = 25;
            this.radLabel6.Text = "Padding Type";
            // 
            // txtLength
            // 
            this.txtLength.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLength.Location = new System.Drawing.Point(135, 62);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(164, 22);
            this.txtLength.TabIndex = 2;
            this.txtLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtLength_KeyPress);
            // 
            // radLabel3
            // 
            this.radLabel3.AutoSize = false;
            this.radLabel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.Location = new System.Drawing.Point(13, 66);
            this.radLabel3.Name = "radLabel3";
            this.radLabel3.Size = new System.Drawing.Size(116, 18);
            this.radLabel3.TabIndex = 23;
            this.radLabel3.Text = "Length";
            // 
            // txtStartPosition
            // 
            this.txtStartPosition.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStartPosition.Location = new System.Drawing.Point(134, 37);
            this.txtStartPosition.Name = "txtStartPosition";
            this.txtStartPosition.Size = new System.Drawing.Size(164, 22);
            this.txtStartPosition.TabIndex = 1;
            this.txtStartPosition.TextChanged += new System.EventHandler(this.txtStartPosition_TextChanged);
            this.txtStartPosition.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStartPosition_KeyPress);
            // 
            // txtParamName
            // 
            this.txtParamName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParamName.Location = new System.Drawing.Point(134, 12);
            this.txtParamName.Name = "txtParamName";
            this.txtParamName.Size = new System.Drawing.Size(164, 22);
            this.txtParamName.TabIndex = 0;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = false;
            this.lblPort.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPort.Location = new System.Drawing.Point(12, 41);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(99, 18);
            this.lblPort.TabIndex = 20;
            this.lblPort.Text = "Start Position";
            // 
            // lblServerIP
            // 
            this.lblServerIP.AutoSize = false;
            this.lblServerIP.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerIP.Location = new System.Drawing.Point(12, 12);
            this.lblServerIP.Name = "lblServerIP";
            this.lblServerIP.Size = new System.Drawing.Size(117, 18);
            this.lblServerIP.TabIndex = 18;
            this.lblServerIP.Text = "Param Name";
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.Location = new System.Drawing.Point(12, 12);
            this.radLabel2.Name = "radLabel2";
            this.radLabel2.Size = new System.Drawing.Size(69, 17);
            this.radLabel2.TabIndex = 19;
            this.radLabel2.Text = "radLabel2";
            // 
            // txtPaddingChar
            // 
            this.txtPaddingChar.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPaddingChar.Location = new System.Drawing.Point(135, 113);
            this.txtPaddingChar.Name = "txtPaddingChar";
            this.txtPaddingChar.Size = new System.Drawing.Size(164, 22);
            this.txtPaddingChar.TabIndex = 4;
            // 
            // radLabel1
            // 
            this.radLabel1.AutoSize = false;
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.Location = new System.Drawing.Point(13, 117);
            this.radLabel1.Name = "radLabel1";
            this.radLabel1.Size = new System.Drawing.Size(116, 18);
            this.radLabel1.TabIndex = 27;
            this.radLabel1.Text = "Padding Char";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(139, 169);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(220, 169);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ddlPaddingType
            // 
            this.ddlPaddingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlPaddingType.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlPaddingType.FormattingEnabled = true;
            this.ddlPaddingType.Items.AddRange(new object[] {
            "L",
            "R"});
            this.ddlPaddingType.Location = new System.Drawing.Point(134, 88);
            this.ddlPaddingType.Name = "ddlPaddingType";
            this.ddlPaddingType.Size = new System.Drawing.Size(80, 22);
            this.ddlPaddingType.TabIndex = 3;
            this.ddlPaddingType.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // txtValue
            // 
            this.txtValue.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValue.Location = new System.Drawing.Point(135, 141);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(126, 22);
            this.txtValue.TabIndex = 5;
            // 
            // radLabel4
            // 
            this.radLabel4.AutoSize = false;
            this.radLabel4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.Location = new System.Drawing.Point(13, 143);
            this.radLabel4.Name = "radLabel4";
            this.radLabel4.Size = new System.Drawing.Size(116, 18);
            this.radLabel4.TabIndex = 34;
            this.radLabel4.Text = "Value";
            // 
            // btnVariable
            // 
            this.btnVariable.Location = new System.Drawing.Point(262, 141);
            this.btnVariable.Name = "btnVariable";
            this.btnVariable.Size = new System.Drawing.Size(37, 23);
            this.btnVariable.TabIndex = 6;
            this.btnVariable.Text = "..";
            this.btnVariable.UseVisualStyleBackColor = true;
            this.btnVariable.Click += new System.EventHandler(this.btnVariable_Click);
            // 
            // frmTCPIPParam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(316, 198);
            this.Controls.Add(this.btnVariable);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.radLabel4);
            this.Controls.Add(this.ddlPaddingType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtPaddingChar);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.radLabel6);
            this.Controls.Add(this.txtLength);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.txtStartPosition);
            this.Controls.Add(this.txtParamName);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblServerIP);
            this.Controls.Add(this.radLabel2);
            this.MaximizeBox = false;
            this.Name = "frmTCPIPParam";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TCPIP Param";
            this.ThemeName = "Vista";
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblServerIP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadLabel radLabel6;
        private System.Windows.Forms.TextBox txtLength;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private System.Windows.Forms.TextBox txtStartPosition;
        private System.Windows.Forms.TextBox txtParamName;
        private Telerik.WinControls.UI.RadLabel lblPort;
        private Telerik.WinControls.UI.RadLabel lblServerIP;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private System.Windows.Forms.TextBox txtPaddingChar;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox ddlPaddingType;
        private System.Windows.Forms.TextBox txtValue;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private System.Windows.Forms.Button btnVariable;

    }
}

