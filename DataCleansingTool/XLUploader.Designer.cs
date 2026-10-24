namespace DataCleansingTool
{
    partial class XLUploader
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
            this.MapUserRadioButton = new System.Windows.Forms.RadioButton();
            this.CreateUserRadioButton = new System.Windows.Forms.RadioButton();
            this.ApplicationsComboBox = new System.Windows.Forms.ComboBox();
            this.ImportFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FileNameTextBox = new System.Windows.Forms.TextBox();
            this.BrowseButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ApplicationPanel = new System.Windows.Forms.Panel();
            this.SelectFilePanel = new System.Windows.Forms.Panel();
            this.ImportButton = new System.Windows.Forms.Button();
            this.LogTextBox = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ResolveAllButton = new System.Windows.Forms.Button();
            this.EndpointsComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ApplicationPanel.SuspendLayout();
            this.SelectFilePanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MapUserRadioButton
            // 
            this.MapUserRadioButton.AutoSize = true;
            this.MapUserRadioButton.Location = new System.Drawing.Point(161, 29);
            this.MapUserRadioButton.Name = "MapUserRadioButton";
            this.MapUserRadioButton.Size = new System.Drawing.Size(97, 21);
            this.MapUserRadioButton.TabIndex = 0;
            this.MapUserRadioButton.Text = "Map Users";
            this.MapUserRadioButton.UseVisualStyleBackColor = true;
            this.MapUserRadioButton.CheckedChanged += new System.EventHandler(this.MapUserRadioButton_CheckedChanged);
            // 
            // CreateUserRadioButton
            // 
            this.CreateUserRadioButton.AutoSize = true;
            this.CreateUserRadioButton.Checked = true;
            this.CreateUserRadioButton.Location = new System.Drawing.Point(12, 29);
            this.CreateUserRadioButton.Name = "CreateUserRadioButton";
            this.CreateUserRadioButton.Size = new System.Drawing.Size(143, 21);
            this.CreateUserRadioButton.TabIndex = 1;
            this.CreateUserRadioButton.TabStop = true;
            this.CreateUserRadioButton.Text = "Bulk Create Users";
            this.CreateUserRadioButton.UseVisualStyleBackColor = true;
            this.CreateUserRadioButton.CheckedChanged += new System.EventHandler(this.CreateUserRadioButton_CheckedChanged);
            // 
            // ApplicationsComboBox
            // 
            this.ApplicationsComboBox.FormattingEnabled = true;
            this.ApplicationsComboBox.Location = new System.Drawing.Point(153, 3);
            this.ApplicationsComboBox.Name = "ApplicationsComboBox";
            this.ApplicationsComboBox.Size = new System.Drawing.Size(374, 24);
            this.ApplicationsComboBox.TabIndex = 2;
            // 
            // ImportFileDialog
            // 
            this.ImportFileDialog.FileName = "openFileDialog1";
            this.ImportFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.ImportFileDialog_FileOk);
            // 
            // FileNameTextBox
            // 
            this.FileNameTextBox.Location = new System.Drawing.Point(149, 10);
            this.FileNameTextBox.Name = "FileNameTextBox";
            this.FileNameTextBox.ReadOnly = true;
            this.FileNameTextBox.Size = new System.Drawing.Size(279, 22);
            this.FileNameTextBox.TabIndex = 3;
            // 
            // BrowseButton
            // 
            this.BrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.BrowseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BrowseButton.Location = new System.Drawing.Point(429, 10);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(25, 25);
            this.BrowseButton.TabIndex = 4;
            this.BrowseButton.Text = "...";
            this.BrowseButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select File to Import";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Selection Application";
            // 
            // ApplicationPanel
            // 
            this.ApplicationPanel.Controls.Add(this.ApplicationsComboBox);
            this.ApplicationPanel.Controls.Add(this.label2);
            this.ApplicationPanel.Location = new System.Drawing.Point(17, 59);
            this.ApplicationPanel.Name = "ApplicationPanel";
            this.ApplicationPanel.Size = new System.Drawing.Size(533, 30);
            this.ApplicationPanel.TabIndex = 7;
            // 
            // SelectFilePanel
            // 
            this.SelectFilePanel.Controls.Add(this.ImportButton);
            this.SelectFilePanel.Controls.Add(this.FileNameTextBox);
            this.SelectFilePanel.Controls.Add(this.BrowseButton);
            this.SelectFilePanel.Controls.Add(this.label1);
            this.SelectFilePanel.Location = new System.Drawing.Point(17, 103);
            this.SelectFilePanel.Name = "SelectFilePanel";
            this.SelectFilePanel.Size = new System.Drawing.Size(548, 46);
            this.SelectFilePanel.TabIndex = 8;
            // 
            // ImportButton
            // 
            this.ImportButton.Location = new System.Drawing.Point(460, 9);
            this.ImportButton.Name = "ImportButton";
            this.ImportButton.Size = new System.Drawing.Size(66, 30);
            this.ImportButton.TabIndex = 6;
            this.ImportButton.Text = "Import";
            this.ImportButton.UseVisualStyleBackColor = true;
            this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // LogTextBox
            // 
            this.LogTextBox.Location = new System.Drawing.Point(17, 209);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.LogTextBox.Size = new System.Drawing.Size(1284, 488);
            this.LogTextBox.TabIndex = 9;
            this.LogTextBox.Text = "";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Import Log";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ResolveAllButton);
            this.groupBox1.Controls.Add(this.EndpointsComboBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(571, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(711, 69);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resolve All Exceptions for First Time";
            // 
            // ResolveAllButton
            // 
            this.ResolveAllButton.Location = new System.Drawing.Point(558, 27);
            this.ResolveAllButton.Name = "ResolveAllButton";
            this.ResolveAllButton.Size = new System.Drawing.Size(116, 30);
            this.ResolveAllButton.TabIndex = 9;
            this.ResolveAllButton.Text = "Resolve All";
            this.ResolveAllButton.UseVisualStyleBackColor = true;
            this.ResolveAllButton.Click += new System.EventHandler(this.ResolveAllButton_Click);
            // 
            // EndpointsComboBox
            // 
            this.EndpointsComboBox.FormattingEnabled = true;
            this.EndpointsComboBox.Location = new System.Drawing.Point(162, 29);
            this.EndpointsComboBox.Name = "EndpointsComboBox";
            this.EndpointsComboBox.Size = new System.Drawing.Size(374, 24);
            this.EndpointsComboBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Selection Application";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Location = new System.Drawing.Point(624, 121);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(639, 46);
            this.panel1.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(554, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 30);
            this.button1.TabIndex = 6;
            this.button1.Text = "Import";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(149, 10);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(374, 22);
            this.textBox1.TabIndex = 3;
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(523, 9);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(25, 25);
            this.button2.TabIndex = 4;
            this.button2.Text = "...";
            this.button2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 17);
            this.label5.TabIndex = 5;
            this.label5.Text = "Select File to Import";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(621, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(187, 17);
            this.label6.TabIndex = 7;
            this.label6.Text = "Approver Exceptions Upload";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // XLUploader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 709);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.SelectFilePanel);
            this.Controls.Add(this.ApplicationPanel);
            this.Controls.Add(this.CreateUserRadioButton);
            this.Controls.Add(this.MapUserRadioButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XLUploader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.XLUploader_Load);
            this.ApplicationPanel.ResumeLayout(false);
            this.ApplicationPanel.PerformLayout();
            this.SelectFilePanel.ResumeLayout(false);
            this.SelectFilePanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton MapUserRadioButton;
        private System.Windows.Forms.RadioButton CreateUserRadioButton;
        private System.Windows.Forms.ComboBox ApplicationsComboBox;
        private System.Windows.Forms.OpenFileDialog ImportFileDialog;
        private System.Windows.Forms.TextBox FileNameTextBox;
        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel ApplicationPanel;
        private System.Windows.Forms.Panel SelectFilePanel;
        private System.Windows.Forms.RichTextBox LogTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ImportButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ResolveAllButton;
        private System.Windows.Forms.ComboBox EndpointsComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

