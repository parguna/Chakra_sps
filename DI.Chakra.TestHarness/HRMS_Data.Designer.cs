namespace TestHarness
{
    partial class HRMS_Data
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTableNames = new System.Windows.Forms.ComboBox();
            this.cmbEventNames = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvTableDetails = new System.Windows.Forms.DataGridView();
            this.dgvEventCodeAndReason = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPERNR = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableDetails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEventCodeAndReason)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Table Name:";
            // 
            // cmbTableNames
            // 
            this.cmbTableNames.FormattingEnabled = true;
            this.cmbTableNames.Items.AddRange(new object[] {
            "Select",
            "PA0000",
            "PA0001",
            "PA0002",
            "PA0302"});
            this.cmbTableNames.Location = new System.Drawing.Point(155, 57);
            this.cmbTableNames.Name = "cmbTableNames";
            this.cmbTableNames.Size = new System.Drawing.Size(272, 24);
            this.cmbTableNames.TabIndex = 1;
            // 
            // cmbEventNames
            // 
            this.cmbEventNames.FormattingEnabled = true;
            this.cmbEventNames.Items.AddRange(new object[] {
            "Select",
            "On Board",
            "Off Board",
            "Transfer",
            "Re-Org"});
            this.cmbEventNames.Location = new System.Drawing.Point(113, 11);
            this.cmbEventNames.Name = "cmbEventNames";
            this.cmbEventNames.Size = new System.Drawing.Size(201, 24);
            this.cmbEventNames.TabIndex = 2;
            this.cmbEventNames.SelectedIndexChanged += new System.EventHandler(this.cmbEventNames_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.txtPERNR);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.dgvTableDetails);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cmbTableNames);
            this.panel1.Location = new System.Drawing.Point(12, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1083, 713);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvEventCodeAndReason);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cmbEventNames);
            this.panel2.Location = new System.Drawing.Point(1101, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(326, 713);
            this.panel2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Event :";
            // 
            // dgvTableDetails
            // 
            this.dgvTableDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableDetails.Location = new System.Drawing.Point(19, 101);
            this.dgvTableDetails.Name = "dgvTableDetails";
            this.dgvTableDetails.RowTemplate.Height = 24;
            this.dgvTableDetails.Size = new System.Drawing.Size(1043, 583);
            this.dgvTableDetails.TabIndex = 2;
            // 
            // dgvEventCodeAndReason
            // 
            this.dgvEventCodeAndReason.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEventCodeAndReason.Location = new System.Drawing.Point(17, 101);
            this.dgvEventCodeAndReason.Name = "dgvEventCodeAndReason";
            this.dgvEventCodeAndReason.ReadOnly = true;
            this.dgvEventCodeAndReason.RowTemplate.Height = 24;
            this.dgvEventCodeAndReason.Size = new System.Drawing.Size(296, 582);
            this.dgvEventCodeAndReason.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "PERNR:";
            // 
            // txtPERNR
            // 
            this.txtPERNR.Location = new System.Drawing.Point(155, 15);
            this.txtPERNR.MaxLength = 8;
            this.txtPERNR.Name = "txtPERNR";
            this.txtPERNR.Size = new System.Drawing.Size(270, 22);
            this.txtPERNR.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(497, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(151, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "Get Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // HRMS_Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1439, 729);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "HRMS_Data";
            this.Text = "HRMS_Data";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableDetails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEventCodeAndReason)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTableNames;
        private System.Windows.Forms.ComboBox cmbEventNames;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPERNR;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvTableDetails;
        private System.Windows.Forms.DataGridView dgvEventCodeAndReason;
        private System.Windows.Forms.Button button1;
    }
}