namespace AppedoLT
{
    partial class UCFormPost
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvParam = new System.Windows.Forms.DataGridView();
            this.headername = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.headerValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.headernade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParam)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvParam
            // 
            this.dgvParam.AllowUserToAddRows = false;
            this.dgvParam.AllowUserToOrderColumns = true;
            this.dgvParam.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvParam.BackgroundColor = System.Drawing.Color.White;
            this.dgvParam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.headername,
            this.headerValue,
            this.headernade});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(45)))), ((int)(((byte)(52)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvParam.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParam.GridColor = System.Drawing.Color.Black;
            this.dgvParam.Location = new System.Drawing.Point(0, 0);
            this.dgvParam.MultiSelect = false;
            this.dgvParam.Name = "dgvParam";
            this.dgvParam.ReadOnly = true;
            this.dgvParam.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvParam.Size = new System.Drawing.Size(594, 317);
            this.dgvParam.TabIndex = 24;
            this.dgvParam.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgvParam_MouseDoubleClick);
            // 
            // headername
            // 
            this.headername.DataPropertyName = "name";
            this.headername.HeaderText = "Name";
            this.headername.Name = "headername";
            this.headername.ReadOnly = true;
            this.headername.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // headerValue
            // 
            this.headerValue.DataPropertyName = "value";
            this.headerValue.HeaderText = "Value";
            this.headerValue.Name = "headerValue";
            this.headerValue.ReadOnly = true;
            this.headerValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // headernade
            // 
            this.headernade.DataPropertyName = "node";
            this.headernade.HeaderText = "node";
            this.headernade.Name = "headernade";
            this.headernade.ReadOnly = true;
            this.headernade.Visible = false;
            // 
            // UCFormPost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgvParam);
            this.Name = "UCFormPost";
            this.Size = new System.Drawing.Size(594, 317);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvParam;
        private System.Windows.Forms.DataGridViewTextBoxColumn headername;
        private System.Windows.Forms.DataGridViewTextBoxColumn headerValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn headernade;
    }
}
