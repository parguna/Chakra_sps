namespace TestHarness
{
    partial class SAP_AccountCreation
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
            this.cmbEndpoint = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAccountname = new System.Windows.Forms.TextBox();
            this.cmbAccountType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.cmbSAPUserType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnReactivate = new System.Windows.Forms.Button();
            this.btnDeactivate = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmployeeId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dtValidTo = new System.Windows.Forms.DateTimePicker();
            this.chkmaxDate = new System.Windows.Forms.CheckBox();
            this.gvResult = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbEndpoint
            // 
            this.cmbEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEndpoint.FormattingEnabled = true;
            this.cmbEndpoint.Location = new System.Drawing.Point(198, 49);
            this.cmbEndpoint.Name = "cmbEndpoint";
            this.cmbEndpoint.Size = new System.Drawing.Size(259, 24);
            this.cmbEndpoint.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(82, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Endpoint";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 17);
            this.label4.TabIndex = 15;
            this.label4.Text = "Account Name";
            // 
            // txtAccountname
            // 
            this.txtAccountname.Location = new System.Drawing.Point(198, 136);
            this.txtAccountname.Name = "txtAccountname";
            this.txtAccountname.Size = new System.Drawing.Size(259, 22);
            this.txtAccountname.TabIndex = 14;
            // 
            // cmbAccountType
            // 
            this.cmbAccountType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAccountType.FormattingEnabled = true;
            this.cmbAccountType.Items.AddRange(new object[] {
            "Profile",
            "Group",
            "Role",
            "User",
            "Parameter",
            "TCode",
            "Field",
            "AuthObjects",
            "Position",
            "Section"});
            this.cmbAccountType.Location = new System.Drawing.Point(198, 91);
            this.cmbAccountType.Name = "cmbAccountType";
            this.cmbAccountType.Size = new System.Drawing.Size(259, 24);
            this.cmbAccountType.TabIndex = 17;
            this.cmbAccountType.SelectedIndexChanged += new System.EventHandler(this.cmbAccountType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 17);
            this.label2.TabIndex = 16;
            this.label2.Text = "Account Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 181);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 17);
            this.label3.TabIndex = 19;
            this.label3.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(198, 181);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(259, 22);
            this.txtDescription.TabIndex = 18;
            // 
            // cmbSAPUserType
            // 
            this.cmbSAPUserType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSAPUserType.FormattingEnabled = true;
            this.cmbSAPUserType.Items.AddRange(new object[] {
            "Profile",
            "Group",
            "Role",
            "User",
            "Parameter",
            "TCode",
            "Field",
            "AuthObjects",
            "Position",
            "Section"});
            this.cmbSAPUserType.Location = new System.Drawing.Point(198, 225);
            this.cmbSAPUserType.Name = "cmbSAPUserType";
            this.cmbSAPUserType.Size = new System.Drawing.Size(259, 24);
            this.cmbSAPUserType.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 17);
            this.label5.TabIndex = 20;
            this.label5.Text = "SAP User Type";
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(539, 7);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(124, 28);
            this.btnCreate.TabIndex = 32;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnReactivate
            // 
            this.btnReactivate.Location = new System.Drawing.Point(539, 75);
            this.btnReactivate.Name = "btnReactivate";
            this.btnReactivate.Size = new System.Drawing.Size(124, 28);
            this.btnReactivate.TabIndex = 31;
            this.btnReactivate.Text = "Reactivate";
            this.btnReactivate.UseVisualStyleBackColor = true;
            this.btnReactivate.Click += new System.EventHandler(this.btnReactivate_Click);
            // 
            // btnDeactivate
            // 
            this.btnDeactivate.Location = new System.Drawing.Point(539, 41);
            this.btnDeactivate.Name = "btnDeactivate";
            this.btnDeactivate.Size = new System.Drawing.Size(124, 28);
            this.btnDeactivate.TabIndex = 30;
            this.btnDeactivate.Text = "Deactivate";
            this.btnDeactivate.UseVisualStyleBackColor = true;
            this.btnDeactivate.Click += new System.EventHandler(this.btnDeactivate_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 17);
            this.label6.TabIndex = 34;
            this.label6.Text = "User Employee Id";
            // 
            // txtEmployeeId
            // 
            this.txtEmployeeId.Location = new System.Drawing.Point(198, 9);
            this.txtEmployeeId.Name = "txtEmployeeId";
            this.txtEmployeeId.Size = new System.Drawing.Size(259, 22);
            this.txtEmployeeId.TabIndex = 35;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(86, 285);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 17);
            this.label7.TabIndex = 36;
            this.label7.Text = "Valid To";
            // 
            // dtValidTo
            // 
            this.dtValidTo.Location = new System.Drawing.Point(198, 280);
            this.dtValidTo.Name = "dtValidTo";
            this.dtValidTo.Size = new System.Drawing.Size(259, 22);
            this.dtValidTo.TabIndex = 37;
            // 
            // chkmaxDate
            // 
            this.chkmaxDate.AutoSize = true;
            this.chkmaxDate.Location = new System.Drawing.Point(198, 325);
            this.chkmaxDate.Name = "chkmaxDate";
            this.chkmaxDate.Size = new System.Drawing.Size(95, 21);
            this.chkmaxDate.TabIndex = 38;
            this.chkmaxDate.Text = "IsMaxDate";
            this.chkmaxDate.UseVisualStyleBackColor = true;
            // 
            // gvResult
            // 
            this.gvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvResult.Location = new System.Drawing.Point(30, 363);
            this.gvResult.Name = "gvResult";
            this.gvResult.RowTemplate.Height = 24;
            this.gvResult.Size = new System.Drawing.Size(1245, 340);
            this.gvResult.TabIndex = 39;
            // 
            // SAP_AccountCreation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 732);
            this.Controls.Add(this.gvResult);
            this.Controls.Add(this.chkmaxDate);
            this.Controls.Add(this.dtValidTo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtEmployeeId);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnReactivate);
            this.Controls.Add(this.btnDeactivate);
            this.Controls.Add(this.cmbSAPUserType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.cmbAccountType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAccountname);
            this.Controls.Add(this.cmbEndpoint);
            this.Controls.Add(this.label1);
            this.Name = "SAP_AccountCreation";
            this.Text = "SAP_AccountCreation";
            this.Load += new System.EventHandler(this.SAP_AccountCreation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbEndpoint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAccountname;
        private System.Windows.Forms.ComboBox cmbAccountType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.ComboBox cmbSAPUserType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnReactivate;
        private System.Windows.Forms.Button btnDeactivate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEmployeeId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtValidTo;
        private System.Windows.Forms.CheckBox chkmaxDate;
        private System.Windows.Forms.DataGridView gvResult;
    }
}