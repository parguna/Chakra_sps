namespace DI.Chakra.RuleEngine.CodeEditor
{
    partial class CodeSnippetEditor
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
            this.SnippetNameTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.IsActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SnippetsComboBox = new System.Windows.Forms.ComboBox();
            this.SnippetRichTextBox = new System.Windows.Forms.RichTextBox();
            this.NewSnippetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(651, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Code Snippet Name";
            // 
            // SnippetNameTextBox
            // 
            this.SnippetNameTextBox.Location = new System.Drawing.Point(795, 17);
            this.SnippetNameTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SnippetNameTextBox.Name = "SnippetNameTextBox";
            this.SnippetNameTextBox.Size = new System.Drawing.Size(369, 22);
            this.SnippetNameTextBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1183, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Is Active";
            // 
            // IsActiveCheckBox
            // 
            this.IsActiveCheckBox.AutoSize = true;
            this.IsActiveCheckBox.Location = new System.Drawing.Point(1248, 17);
            this.IsActiveCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.IsActiveCheckBox.Name = "IsActiveCheckBox";
            this.IsActiveCheckBox.Size = new System.Drawing.Size(18, 17);
            this.IsActiveCheckBox.TabIndex = 4;
            this.IsActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(1309, 12);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 32);
            this.SaveButton.TabIndex = 6;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Existing Snippets";
            // 
            // SnippetsComboBox
            // 
            this.SnippetsComboBox.FormattingEnabled = true;
            this.SnippetsComboBox.Location = new System.Drawing.Point(133, 17);
            this.SnippetsComboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SnippetsComboBox.Name = "SnippetsComboBox";
            this.SnippetsComboBox.Size = new System.Drawing.Size(407, 24);
            this.SnippetsComboBox.TabIndex = 8;
            this.SnippetsComboBox.SelectedIndexChanged += new System.EventHandler(this.SnippetsComboBox_SelectedIndexChanged);
            // 
            // SnippetRichTextBox
            // 
            this.SnippetRichTextBox.Location = new System.Drawing.Point(12, 54);
            this.SnippetRichTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SnippetRichTextBox.Name = "SnippetRichTextBox";
            this.SnippetRichTextBox.Size = new System.Drawing.Size(1373, 645);
            this.SnippetRichTextBox.TabIndex = 5;
            this.SnippetRichTextBox.Text = "";
            // 
            // NewSnippetButton
            // 
            this.NewSnippetButton.Location = new System.Drawing.Point(558, 12);
            this.NewSnippetButton.Name = "NewSnippetButton";
            this.NewSnippetButton.Size = new System.Drawing.Size(75, 32);
            this.NewSnippetButton.TabIndex = 9;
            this.NewSnippetButton.Text = "New";
            this.NewSnippetButton.UseVisualStyleBackColor = true;
            this.NewSnippetButton.Click += new System.EventHandler(this.NewSnippetButton_Click);
            // 
            // CodeSnippetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1411, 711);
            this.Controls.Add(this.NewSnippetButton);
            this.Controls.Add(this.SnippetsComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.SnippetRichTextBox);
            this.Controls.Add(this.IsActiveCheckBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SnippetNameTextBox);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CodeSnippetEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Code Snippet Editor";
            this.Load += new System.EventHandler(this.CodeSnippetEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SnippetNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox IsActiveCheckBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox SnippetsComboBox;
        private System.Windows.Forms.RichTextBox SnippetRichTextBox;
        private System.Windows.Forms.Button NewSnippetButton;
    }
}

