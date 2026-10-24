namespace TestHarness
{
    partial class SAPUtilityForm
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
            this.cmbEndpoint = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnGetData = new System.Windows.Forms.Button();
            this.cmbObject = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.gvResult = new System.Windows.Forms.DataGridView();
            this.txtObjectValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnCheckUsername = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPositionCode = new System.Windows.Forms.TextBox();
            this.btnCheckPosition = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.btnBAPIGetAll = new System.Windows.Forms.Button();
            this.cmbSAPTables = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtTableName = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtFields = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.btnExtension = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.txtEmployeeId = new System.Windows.Forms.TextBox();
            this.btnResetPassword = new System.Windows.Forms.Button();
            this.btnSRMReverse = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Endpoint";
            // 
            // cmbEndpoint
            // 
            this.cmbEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEndpoint.FormattingEnabled = true;
            this.cmbEndpoint.Location = new System.Drawing.Point(143, 23);
            this.cmbEndpoint.Name = "cmbEndpoint";
            this.cmbEndpoint.Size = new System.Drawing.Size(259, 24);
            this.cmbEndpoint.TabIndex = 1;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(436, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(124, 28);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(609, 29);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 17);
            this.lblStatus.TabIndex = 3;
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(436, 109);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(124, 28);
            this.btnGetData.TabIndex = 6;
            this.btnGetData.Text = "Get";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // cmbObject
            // 
            this.cmbObject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbObject.FormattingEnabled = true;
            this.cmbObject.Items.AddRange(new object[] {
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
            this.cmbObject.Location = new System.Drawing.Point(143, 65);
            this.cmbObject.Name = "cmbObject";
            this.cmbObject.Size = new System.Drawing.Size(259, 24);
            this.cmbObject.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Object Type";
            // 
            // gvResult
            // 
            this.gvResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvResult.Location = new System.Drawing.Point(15, 308);
            this.gvResult.Name = "gvResult";
            this.gvResult.RowTemplate.Height = 24;
            this.gvResult.Size = new System.Drawing.Size(1245, 436);
            this.gvResult.TabIndex = 7;
            // 
            // txtObjectValue
            // 
            this.txtObjectValue.Location = new System.Drawing.Point(143, 112);
            this.txtObjectValue.Name = "txtObjectValue";
            this.txtObjectValue.Size = new System.Drawing.Size(259, 22);
            this.txtObjectValue.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Object Value";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(835, 84);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(259, 22);
            this.txtUsername.TabIndex = 12;
            // 
            // btnCheckUsername
            // 
            this.btnCheckUsername.Location = new System.Drawing.Point(1133, 81);
            this.btnCheckUsername.Name = "btnCheckUsername";
            this.btnCheckUsername.Size = new System.Drawing.Size(124, 28);
            this.btnCheckUsername.TabIndex = 11;
            this.btnCheckUsername.Text = "Check";
            this.btnCheckUsername.UseVisualStyleBackColor = true;
            this.btnCheckUsername.Click += new System.EventHandler(this.btnCheckUsername_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(676, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Username";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(676, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 17);
            this.label5.TabIndex = 16;
            this.label5.Text = "Position Code";
            // 
            // txtPositionCode
            // 
            this.txtPositionCode.Location = new System.Drawing.Point(835, 152);
            this.txtPositionCode.Name = "txtPositionCode";
            this.txtPositionCode.Size = new System.Drawing.Size(259, 22);
            this.txtPositionCode.TabIndex = 15;
            // 
            // btnCheckPosition
            // 
            this.btnCheckPosition.Location = new System.Drawing.Point(1133, 149);
            this.btnCheckPosition.Name = "btnCheckPosition";
            this.btnCheckPosition.Size = new System.Drawing.Size(124, 28);
            this.btnCheckPosition.TabIndex = 14;
            this.btnCheckPosition.Text = "Check Position";
            this.btnCheckPosition.UseVisualStyleBackColor = true;
            this.btnCheckPosition.Click += new System.EventHandler(this.btnCheckPosition_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(676, 118);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 17);
            this.label6.TabIndex = 19;
            this.label6.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(835, 118);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(259, 22);
            this.txtPassword.TabIndex = 18;
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Location = new System.Drawing.Point(1133, 115);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(124, 28);
            this.btnChangePassword.TabIndex = 17;
            this.btnChangePassword.Text = "Change";
            this.btnChangePassword.UseVisualStyleBackColor = true;
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnBAPIGetAll
            // 
            this.btnBAPIGetAll.Location = new System.Drawing.Point(436, 252);
            this.btnBAPIGetAll.Name = "btnBAPIGetAll";
            this.btnBAPIGetAll.Size = new System.Drawing.Size(124, 28);
            this.btnBAPIGetAll.TabIndex = 22;
            this.btnBAPIGetAll.Text = "Get All";
            this.btnBAPIGetAll.UseVisualStyleBackColor = true;
            this.btnBAPIGetAll.Click += new System.EventHandler(this.btnBAPIGetAll_Click);
            // 
            // cmbSAPTables
            // 
            this.cmbSAPTables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSAPTables.FormattingEnabled = true;
            this.cmbSAPTables.Items.AddRange(new object[] {
            "ADDRESS",
            "ISLOCKED",
            "DEFAULTS",
            "LOGONDATA",
            "ALIAS"});
            this.cmbSAPTables.Location = new System.Drawing.Point(143, 255);
            this.cmbSAPTables.Name = "cmbSAPTables";
            this.cmbSAPTables.Size = new System.Drawing.Size(259, 24);
            this.cmbSAPTables.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 17);
            this.label7.TabIndex = 20;
            this.label7.Text = "SAP Table";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 17);
            this.label8.TabIndex = 24;
            this.label8.Text = "TableName";
            // 
            // txtTableName
            // 
            this.txtTableName.Location = new System.Drawing.Point(143, 152);
            this.txtTableName.Name = "txtTableName";
            this.txtTableName.Size = new System.Drawing.Size(259, 22);
            this.txtTableName.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(29, 180);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 17);
            this.label9.TabIndex = 26;
            this.label9.Text = "Fields";
            // 
            // txtFields
            // 
            this.txtFields.Location = new System.Drawing.Point(143, 180);
            this.txtFields.Name = "txtFields";
            this.txtFields.Size = new System.Drawing.Size(1114, 22);
            this.txtFields.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(29, 224);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 17);
            this.label10.TabIndex = 28;
            this.label10.Text = "Filter";
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(143, 224);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(1114, 22);
            this.txtFilter.TabIndex = 27;
            // 
            // btnExtension
            // 
            this.btnExtension.Location = new System.Drawing.Point(1133, 47);
            this.btnExtension.Name = "btnExtension";
            this.btnExtension.Size = new System.Drawing.Size(124, 28);
            this.btnExtension.TabIndex = 29;
            this.btnExtension.Text = "Extend";
            this.btnExtension.UseVisualStyleBackColor = true;
            this.btnExtension.Click += new System.EventHandler(this.btnExtension_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(676, 47);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 17);
            this.label11.TabIndex = 31;
            this.label11.Text = "EmployeeId";
            // 
            // txtEmployeeId
            // 
            this.txtEmployeeId.Location = new System.Drawing.Point(835, 47);
            this.txtEmployeeId.Name = "txtEmployeeId";
            this.txtEmployeeId.Size = new System.Drawing.Size(259, 22);
            this.txtEmployeeId.TabIndex = 30;
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.Location = new System.Drawing.Point(1263, 115);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(102, 28);
            this.btnResetPassword.TabIndex = 32;
            this.btnResetPassword.Text = "Reset";
            this.btnResetPassword.UseVisualStyleBackColor = true;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // btnSRMReverse
            // 
            this.btnSRMReverse.Location = new System.Drawing.Point(835, 258);
            this.btnSRMReverse.Name = "btnSRMReverse";
            this.btnSRMReverse.Size = new System.Drawing.Size(422, 28);
            this.btnSRMReverse.TabIndex = 33;
            this.btnSRMReverse.Text = "Process Reverse Engineering SRM";
            this.btnSRMReverse.UseVisualStyleBackColor = true;
            this.btnSRMReverse.Click += new System.EventHandler(this.btnSRMReverse_Click);
            // 
            // SAPUtilityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1422, 756);
            this.Controls.Add(this.btnSRMReverse);
            this.Controls.Add(this.btnResetPassword);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtEmployeeId);
            this.Controls.Add(this.btnExtension);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtFields);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtTableName);
            this.Controls.Add(this.btnBAPIGetAll);
            this.Controls.Add(this.cmbSAPTables);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnChangePassword);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtPositionCode);
            this.Controls.Add(this.btnCheckPosition);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.btnCheckUsername);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtObjectValue);
            this.Controls.Add(this.gvResult);
            this.Controls.Add(this.btnGetData);
            this.Controls.Add(this.cmbObject);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbEndpoint);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SAPUtilityForm";
            this.Text = "SAPUtilityForm";
            this.Load += new System.EventHandler(this.SAPUtilityForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbEndpoint;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.ComboBox cmbObject;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gvResult;
        private System.Windows.Forms.TextBox txtObjectValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Button btnCheckUsername;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtPositionCode;
        private System.Windows.Forms.Button btnCheckPosition;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnChangePassword;
        private System.Windows.Forms.Button btnBAPIGetAll;
        private System.Windows.Forms.ComboBox cmbSAPTables;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTableName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtFields;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Button btnExtension;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtEmployeeId;
        private System.Windows.Forms.Button btnResetPassword;
        private System.Windows.Forms.Button btnSRMReverse;
    }
}