namespace AppedoLT
{
    partial class frmDynamicRequestParameter
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
            this.txtParameterName = new Telerik.WinControls.UI.RadTextBox();
            this.lblName = new Telerik.WinControls.UI.RadLabel();
            this.lblValue = new Telerik.WinControls.UI.RadLabel();
            this.txtParameterValue = new Telerik.WinControls.UI.RadTextBox();
            this.radioPOST = new Telerik.WinControls.UI.RadRadioButton();
            this.radioGET = new Telerik.WinControls.UI.RadRadioButton();
            this.radioMultipart = new Telerik.WinControls.UI.RadRadioButton();
            this.btnSave = new Telerik.WinControls.UI.RadButton();
            this.btnClose = new Telerik.WinControls.UI.RadButton();
            this.lblMethod = new Telerik.WinControls.UI.RadLabel();
            this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            ((System.ComponentModel.ISupportInitialize)(this.txtParameterName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParameterValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioPOST)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGET)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioMultipart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMethod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // txtParameterName
            // 
            this.txtParameterName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParameterName.Location = new System.Drawing.Point(79, 12);
            this.txtParameterName.Name = "txtParameterName";
            this.txtParameterName.Size = new System.Drawing.Size(270, 19);
            this.txtParameterName.TabIndex = 0;
            this.txtParameterName.TabStop = false;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(13, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(52, 17);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name : ";
            // 
            // lblValue
            // 
            this.lblValue.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValue.Location = new System.Drawing.Point(13, 40);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(51, 17);
            this.lblValue.TabIndex = 3;
            this.lblValue.Text = "Value : ";
            // 
            // txtParameterValue
            // 
            this.txtParameterValue.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParameterValue.Location = new System.Drawing.Point(79, 40);
            this.txtParameterValue.Name = "txtParameterValue";
            this.txtParameterValue.Size = new System.Drawing.Size(270, 19);
            this.txtParameterValue.TabIndex = 2;
            this.txtParameterValue.TabStop = false;
            // 
            // radioPOST
            // 
            this.radioPOST.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioPOST.Location = new System.Drawing.Point(79, 68);
            this.radioPOST.Name = "radioPOST";
            this.radioPOST.Size = new System.Drawing.Size(65, 18);
            this.radioPOST.TabIndex = 7;
            this.radioPOST.Text = "POST";
            this.radioPOST.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
            // 
            // radioGET
            // 
            this.radioGET.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioGET.Location = new System.Drawing.Point(141, 68);
            this.radioGET.Name = "radioGET";
            this.radioGET.Size = new System.Drawing.Size(65, 18);
            this.radioGET.TabIndex = 8;
            this.radioGET.Text = "GET";
            // 
            // radioMultipart
            // 
            this.radioMultipart.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioMultipart.Location = new System.Drawing.Point(212, 68);
            this.radioMultipart.Name = "radioMultipart";
            this.radioMultipart.Size = new System.Drawing.Size(137, 18);
            this.radioMultipart.TabIndex = 9;
            this.radioMultipart.Text = "Multipart/form Data";
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(116, 92);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 24);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Save";
            this.btnSave.ThemeName = "Telerik";
          //  this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(214, 92);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 24);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.ThemeName = "Telerik";
            // 
            // lblMethod
            // 
            this.lblMethod.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMethod.Location = new System.Drawing.Point(13, 68);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(61, 17);
            this.lblMethod.TabIndex = 12;
            this.lblMethod.Text = "Method : ";
            // 
            // radLabel1
            // 
            this.radLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel1.ForeColor = System.Drawing.Color.Red;
            this.radLabel1.Location = new System.Drawing.Point(0, 12);
            this.radLabel1.Name = "radLabel1";
            // 
            // 
            // 
            this.radLabel1.RootElement.ForeColor = System.Drawing.Color.Red;
            this.radLabel1.Size = new System.Drawing.Size(13, 17);
            this.radLabel1.TabIndex = 13;
            this.radLabel1.Text = "*";
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.ForeColor = System.Drawing.Color.Red;
            this.radLabel2.Location = new System.Drawing.Point(0, 40);
            this.radLabel2.Name = "radLabel2";
            // 
            // 
            // 
            this.radLabel2.RootElement.ForeColor = System.Drawing.Color.Red;
            this.radLabel2.Size = new System.Drawing.Size(13, 17);
            this.radLabel2.TabIndex = 14;
            this.radLabel2.Text = "*";
            // 
            // frmDynamicRequestParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(361, 130);
            this.Controls.Add(this.radLabel2);
            this.Controls.Add(this.radLabel1);
            this.Controls.Add(this.lblMethod);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.radioMultipart);
            this.Controls.Add(this.radioGET);
            this.Controls.Add(this.radioPOST);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.txtParameterValue);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtParameterName);
            this.ForeColor = System.Drawing.Color.Black;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDynamicRequestParameter";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.RootElement.ForeColor = System.Drawing.Color.Black;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Dynamic Request Parameters";
            this.ThemeName = "Vista";
            ((System.ComponentModel.ISupportInitialize)(this.txtParameterName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtParameterValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioPOST)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioGET)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioMultipart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblMethod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadTextBox txtParameterName;
        private Telerik.WinControls.UI.RadLabel lblName;
        private Telerik.WinControls.UI.RadLabel lblValue;
        private Telerik.WinControls.UI.RadTextBox txtParameterValue;
        private Telerik.WinControls.UI.RadRadioButton radioPOST;
        private Telerik.WinControls.UI.RadRadioButton radioGET;
        private Telerik.WinControls.UI.RadRadioButton radioMultipart;
        private Telerik.WinControls.UI.RadButton btnSave;
        private Telerik.WinControls.UI.RadButton btnClose;
        private Telerik.WinControls.UI.RadLabel lblMethod;
        private Telerik.WinControls.UI.RadLabel radLabel1;
        private Telerik.WinControls.UI.RadLabel radLabel2;
    }
}

