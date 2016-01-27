using AppedoLT.Core;
namespace AppedoLT
{
    partial class UCScriptSetting
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCScriptSetting));
            this.imglSettings = new System.Windows.Forms.ImageList(this.components);
            this.lblScriptName = new Telerik.WinControls.UI.RadLabel();
            this.chkBrowseCache = new System.Windows.Forms.CheckBox();
            this.radLabel3 = new Telerik.WinControls.UI.RadLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblDurationIteration = new Telerik.WinControls.UI.RadLabel();
            this.txtIteration = new AppedoLT.IntTextBox();
            this.rbtnDuraion = new System.Windows.Forms.RadioButton();
            this.rbtnIteration = new System.Windows.Forms.RadioButton();
            this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.txtMaxuser = new AppedoLT.IntTextBox();
            this.radLabel4 = new Telerik.WinControls.UI.RadLabel();
            this.radLabel5 = new Telerik.WinControls.UI.RadLabel();
            this.lblTime = new Telerik.WinControls.UI.RadLabel();
            this.txtIncrementUser = new AppedoLT.IntTextBox();
            this.ucIncrementTime = new AppedoLT.UCTime();
            this.ucDurationTime = new AppedoLT.UCTime();
            this.lblUserCount = new Telerik.WinControls.UI.RadLabel();
            this.radReplyThinkTime = new Telerik.WinControls.UI.RadCheckBox();
            this.txtStatUserCount = new AppedoLT.IntTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.lblScriptName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblDurationIteration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblUserCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radReplyThinkTime)).BeginInit();
            this.SuspendLayout();
            // 
            // imglSettings
            // 
            this.imglSettings.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglSettings.ImageStream")));
            this.imglSettings.TransparentColor = System.Drawing.Color.Transparent;
            this.imglSettings.Images.SetKeyName(0, "settings.gif");
            // 
            // lblScriptName
            // 
            this.lblScriptName.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScriptName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.lblScriptName.Location = new System.Drawing.Point(8, 1);
            this.lblScriptName.Name = "lblScriptName";
            // 
            // 
            // 
            this.lblScriptName.RootElement.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.lblScriptName.Size = new System.Drawing.Size(105, 19);
            this.lblScriptName.TabIndex = 49;
            this.lblScriptName.Text = "Script Name : ";
            // 
            // chkBrowseCache
            // 
            this.chkBrowseCache.AutoSize = true;
            this.chkBrowseCache.Location = new System.Drawing.Point(133, 27);
            this.chkBrowseCache.Name = "chkBrowseCache";
            this.chkBrowseCache.Size = new System.Drawing.Size(15, 14);
            this.chkBrowseCache.TabIndex = 2;
            this.chkBrowseCache.UseVisualStyleBackColor = true;
            // 
            // radLabel3
            // 
            this.radLabel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel3.ForeColor = System.Drawing.Color.Black;
            this.radLabel3.Location = new System.Drawing.Point(8, 26);
            this.radLabel3.Name = "radLabel3";
            // 
            // 
            // 
            this.radLabel3.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel3.Size = new System.Drawing.Size(110, 17);
            this.radLabel3.TabIndex = 48;
            this.radLabel3.Text = "Browser Cache :";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblDurationIteration);
            this.panel1.Controls.Add(this.txtIteration);
            this.panel1.Controls.Add(this.rbtnDuraion);
            this.panel1.Controls.Add(this.rbtnIteration);
            this.panel1.Controls.Add(this.radLabel2);
            this.panel1.Controls.Add(this.radLabel6);
            this.panel1.Controls.Add(this.txtMaxuser);
            this.panel1.Controls.Add(this.radLabel4);
            this.panel1.Controls.Add(this.radLabel5);
            this.panel1.Controls.Add(this.lblTime);
            this.panel1.Controls.Add(this.txtIncrementUser);
            this.panel1.Controls.Add(this.ucIncrementTime);
            this.panel1.Controls.Add(this.ucDurationTime);
            this.panel1.Location = new System.Drawing.Point(2, 75);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(595, 147);
            this.panel1.TabIndex = 46;
            // 
            // lblDurationIteration
            // 
            this.lblDurationIteration.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDurationIteration.ForeColor = System.Drawing.Color.Black;
            this.lblDurationIteration.Location = new System.Drawing.Point(6, 123);
            this.lblDurationIteration.Name = "lblDurationIteration";
            // 
            // 
            // 
            this.lblDurationIteration.RootElement.ForeColor = System.Drawing.Color.Black;
            this.lblDurationIteration.Size = new System.Drawing.Size(72, 17);
            this.lblDurationIteration.TabIndex = 52;
            this.lblDurationIteration.Text = "Iteration :";
            this.lblDurationIteration.Visible = false;
            // 
            // txtIteration
            // 
            this.txtIteration.BackColor = System.Drawing.Color.White;
            this.txtIteration.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIteration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIteration.Location = new System.Drawing.Point(131, 123);
            this.txtIteration.Name = "txtIteration";
            this.txtIteration.Size = new System.Drawing.Size(90, 19);
            this.txtIteration.TabIndex = 51;
            this.txtIteration.Text = "1";
            this.txtIteration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtIteration.Visible = false;
            // 
            // rbtnDuraion
            // 
            this.rbtnDuraion.AutoSize = true;
            this.rbtnDuraion.Checked = true;
            this.rbtnDuraion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnDuraion.ForeColor = System.Drawing.Color.Black;
            this.rbtnDuraion.Location = new System.Drawing.Point(131, 30);
            this.rbtnDuraion.Name = "rbtnDuraion";
            this.rbtnDuraion.Size = new System.Drawing.Size(81, 17);
            this.rbtnDuraion.TabIndex = 48;
            this.rbtnDuraion.TabStop = true;
            this.rbtnDuraion.Text = "Duration";
            this.rbtnDuraion.UseVisualStyleBackColor = true;
            this.rbtnDuraion.CheckedChanged += new System.EventHandler(this.tbtnRampUpDuration_CheckedChanged);
            // 
            // rbtnIteration
            // 
            this.rbtnIteration.AutoSize = true;
            this.rbtnIteration.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnIteration.ForeColor = System.Drawing.Color.Black;
            this.rbtnIteration.Location = new System.Drawing.Point(251, 31);
            this.rbtnIteration.Name = "rbtnIteration";
            this.rbtnIteration.Size = new System.Drawing.Size(83, 17);
            this.rbtnIteration.TabIndex = 49;
            this.rbtnIteration.Text = "Iteration";
            this.rbtnIteration.UseVisualStyleBackColor = true;
            // 
            // radLabel2
            // 
            this.radLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel2.ForeColor = System.Drawing.Color.Black;
            this.radLabel2.Location = new System.Drawing.Point(6, 31);
            this.radLabel2.Name = "radLabel2";
            // 
            // 
            // 
            this.radLabel2.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel2.Size = new System.Drawing.Size(49, 17);
            this.radLabel2.TabIndex = 50;
            this.radLabel2.Text = "Mode :";
            // 
            // radLabel6
            // 
            this.radLabel6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel6.ForeColor = System.Drawing.Color.Black;
            this.radLabel6.Location = new System.Drawing.Point(6, 1);
            this.radLabel6.Name = "radLabel6";
            // 
            // 
            // 
            this.radLabel6.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel6.Size = new System.Drawing.Size(73, 17);
            this.radLabel6.TabIndex = 20;
            this.radLabel6.Text = "Max User :";
            // 
            // txtMaxuser
            // 
            this.txtMaxuser.BackColor = System.Drawing.Color.White;
            this.txtMaxuser.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMaxuser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxuser.Location = new System.Drawing.Point(131, 1);
            this.txtMaxuser.Name = "txtMaxuser";
            this.txtMaxuser.Size = new System.Drawing.Size(90, 19);
            this.txtMaxuser.TabIndex = 0;
            this.txtMaxuser.Text = "1";
            this.txtMaxuser.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMaxuser.Validated += new System.EventHandler(this.txtMaxuser_Validated);
            // 
            // radLabel4
            // 
            this.radLabel4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel4.ForeColor = System.Drawing.Color.Black;
            this.radLabel4.Location = new System.Drawing.Point(6, 90);
            this.radLabel4.Name = "radLabel4";
            // 
            // 
            // 
            this.radLabel4.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel4.Size = new System.Drawing.Size(113, 17);
            this.radLabel4.TabIndex = 5;
            this.radLabel4.Text = "Increment user :";
            // 
            // radLabel5
            // 
            this.radLabel5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radLabel5.ForeColor = System.Drawing.Color.Black;
            this.radLabel5.Location = new System.Drawing.Point(199, 90);
            this.radLabel5.Name = "radLabel5";
            // 
            // 
            // 
            this.radLabel5.RootElement.ForeColor = System.Drawing.Color.Black;
            this.radLabel5.Size = new System.Drawing.Size(75, 17);
            this.radLabel5.TabIndex = 2;
            this.radLabel5.Text = "For Every :";
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.Black;
            this.lblTime.Location = new System.Drawing.Point(6, 60);
            this.lblTime.Name = "lblTime";
            // 
            // 
            // 
            this.lblTime.RootElement.ForeColor = System.Drawing.Color.Black;
            this.lblTime.Size = new System.Drawing.Size(46, 17);
            this.lblTime.TabIndex = 18;
            this.lblTime.Text = "Time :";
            // 
            // txtIncrementUser
            // 
            this.txtIncrementUser.BackColor = System.Drawing.Color.White;
            this.txtIncrementUser.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIncrementUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtIncrementUser.Location = new System.Drawing.Point(131, 90);
            this.txtIncrementUser.Name = "txtIncrementUser";
            this.txtIncrementUser.Size = new System.Drawing.Size(62, 19);
            this.txtIncrementUser.TabIndex = 2;
            this.txtIncrementUser.Text = "1";
            this.txtIncrementUser.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtIncrementUser.TextChanged += new System.EventHandler(this.txtIncrementUser_TextChanged);
            // 
            // ucIncrementTime
            // 
            this.ucIncrementTime.ForeColor = System.Drawing.Color.Black;
            this.ucIncrementTime.Location = new System.Drawing.Point(273, 90);
            this.ucIncrementTime.Name = "ucIncrementTime";
            this.ucIncrementTime.Size = new System.Drawing.Size(319, 20);
            this.ucIncrementTime.TabIndex = 3;
            this.ucIncrementTime.Time = System.TimeSpan.Parse("00:00:00");
            // 
            // ucDurationTime
            // 
            this.ucDurationTime.ForeColor = System.Drawing.Color.Black;
            this.ucDurationTime.Location = new System.Drawing.Point(130, 61);
            this.ucDurationTime.Name = "ucDurationTime";
            this.ucDurationTime.Size = new System.Drawing.Size(320, 20);
            this.ucDurationTime.TabIndex = 1;
            this.ucDurationTime.Time = System.TimeSpan.Parse("00:00:00");
            // 
            // lblUserCount
            // 
            this.lblUserCount.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserCount.ForeColor = System.Drawing.Color.Black;
            this.lblUserCount.Location = new System.Drawing.Point(8, 48);
            this.lblUserCount.Name = "lblUserCount";
            // 
            // 
            // 
            this.lblUserCount.RootElement.ForeColor = System.Drawing.Color.Black;
            this.lblUserCount.Size = new System.Drawing.Size(120, 17);
            this.lblUserCount.TabIndex = 45;
            this.lblUserCount.Text = "Start User Count :";
            // 
            // radReplyThinkTime
            // 
            this.radReplyThinkTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radReplyThinkTime.Location = new System.Drawing.Point(172, 26);
            this.radReplyThinkTime.Name = "radReplyThinkTime";
            this.radReplyThinkTime.Size = new System.Drawing.Size(130, 17);
            this.radReplyThinkTime.TabIndex = 50;
            this.radReplyThinkTime.Text = "Reply Think Time";
            // 
            // txtStatUserCount
            // 
            this.txtStatUserCount.BackColor = System.Drawing.Color.White;
            this.txtStatUserCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtStatUserCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStatUserCount.Location = new System.Drawing.Point(133, 48);
            this.txtStatUserCount.Name = "txtStatUserCount";
            this.txtStatUserCount.Size = new System.Drawing.Size(90, 19);
            this.txtStatUserCount.TabIndex = 3;
            this.txtStatUserCount.Text = "1";
            this.txtStatUserCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStatUserCount.Validated += new System.EventHandler(this.txtStatUserCount_Validated);
            // 
            // UCScriptSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radReplyThinkTime);
            this.Controls.Add(this.lblScriptName);
            this.Controls.Add(this.chkBrowseCache);
            this.Controls.Add(this.radLabel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblUserCount);
            this.Controls.Add(this.txtStatUserCount);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UCScriptSetting";
            this.Size = new System.Drawing.Size(599, 226);
            ((System.ComponentModel.ISupportInitialize)(this.lblScriptName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel3)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lblDurationIteration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblUserCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radReplyThinkTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imglSettings;
        private Telerik.WinControls.UI.RadLabel lblScriptName;
        private System.Windows.Forms.CheckBox chkBrowseCache;
        private Telerik.WinControls.UI.RadLabel radLabel3;
        private System.Windows.Forms.Panel panel1;
        private Telerik.WinControls.UI.RadLabel radLabel4;
        private Telerik.WinControls.UI.RadLabel radLabel5;
        private Telerik.WinControls.UI.RadLabel lblTime;
        private IntTextBox txtIncrementUser;
        private UCTime ucIncrementTime;
        private UCTime ucDurationTime;
        private Telerik.WinControls.UI.RadLabel lblUserCount;
        private IntTextBox txtStatUserCount;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private IntTextBox txtMaxuser;
        private Telerik.WinControls.UI.RadLabel lblDurationIteration;
        private IntTextBox txtIteration;
        private System.Windows.Forms.RadioButton rbtnDuraion;
        private System.Windows.Forms.RadioButton rbtnIteration;
        private Telerik.WinControls.UI.RadLabel radLabel2;
        private Telerik.WinControls.UI.RadCheckBox radReplyThinkTime;
    }
}
