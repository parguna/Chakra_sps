using DI.Chakra.Infrastructure;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Text;
using System.Windows.Forms;
using DI.Chakra.DomainModel;
using DI.Chakra.DBConfigHelper;

namespace DI.Chakra.RuleEngine.CodeEditor
{
    public partial class CodeSnippetEditor : Form
    {
        public CodeSnippetEditor()
        {
            InitializeComponent();
        }

        private long SnippetId
        {
            get
            {
                if (this.SnippetsComboBox.SelectedIndex != -1)
                    return Convert.ToInt64(this.SnippetsComboBox.SelectedValue);
                else
                    return 0;
            }
        }

        private void CodeSnippetEditor_Load(object sender, EventArgs e)
        {
            this.BindSnippets();
        }

        private void SnippetsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindDetailsToView();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (this.IsValid())
            {
                this.SaveCodeSnippet();
                this.BindSnippets();
            }
        }

        private void BindSnippets()
        {
            //var list = snippetFacade.GetSnippetList();
            var runTimeCodeSnippetList = RunTimeCodeSnippet.GetAll();
            this.SnippetsComboBox.ValueMember = "Key";
            this.SnippetsComboBox.DisplayMember = "Value";
            this.SnippetsComboBox.DataSource = new BindingSource(runTimeCodeSnippetList, null);
        }

        private void BindDetailsToView()
        {
            RunTimeCodeSnippet snippet = null;

            if (this.SnippetId != 0)
            {
                snippet = RunTimeCodeSnippet.GetById(this.SnippetId);

                this.SnippetNameTextBox.Text = snippet.CodeSnippetName;
                this.SnippetRichTextBox.Text = UtilityMethods.GetStringFromByteArray(snippet.CodeSnippet);
                this.IsActiveCheckBox.Checked = snippet.Active;
            }
        }

        private RunTimeCodeSnippet GetDetailView()
        {

            RunTimeCodeSnippet runTimeCodeSnippet = new RunTimeCodeSnippet();
            runTimeCodeSnippet.Id = this.SnippetId;

            runTimeCodeSnippet.CodeSnippetName = this.SnippetNameTextBox.Text.Trim();
            runTimeCodeSnippet.CodeSnippet = UtilityMethods.GetByteArrayFromString(this.SnippetRichTextBox.Text.Trim());
            runTimeCodeSnippet.Active = this.IsActiveCheckBox.Checked;

            return runTimeCodeSnippet;
        }

        private void SaveCodeSnippet()
        {
            var snippet = this.GetDetailView();
            snippet.Save(MasterConfigHelper.ServiceUser);
        }

        private void ClearControls()
        {
            this.SnippetsComboBox.SelectedIndex = -1;
            this.SnippetNameTextBox.Clear();
            this.SnippetRichTextBox.Clear();
            this.IsActiveCheckBox.Checked = false;
        }

        private bool IsValid()
        {
            if (string.IsNullOrEmpty(this.SnippetNameTextBox.Text.Trim()))
            {
                MessageBox.Show("Please enter code snippet name.", "Code Snippet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.SnippetNameTextBox.Focus();
                return false;
            }

            if (this.SnippetId == 0)
            {
                var snippet = RunTimeCodeSnippet.GetByName(this.SnippetNameTextBox.Text.Trim());

                if (snippet != null)
                {
                    MessageBox.Show("Code snippet name already exists. Please enter a different name.", "Code Snippet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.SnippetNameTextBox.Focus();
                    return false;
                }
            }

            if (string.IsNullOrEmpty(this.SnippetRichTextBox.Text.Trim()))
            {
                MessageBox.Show("Please enter code snippet.", "Code Snippet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.SnippetRichTextBox.Focus();
                return false;
            }
            if (!this.IsCodeCompiling())
            {
                return false;
            }
            return true;
        }

        private bool IsCodeCompiling()
        {
            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();

                // Reference to System.Drawing library
                parameters.ReferencedAssemblies.Add("DI.Chakra.Infrastructure.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("DI.Chakra.EntityModel.dll");
                parameters.ReferencedAssemblies.Add("DI.Chakra.DomainModel.dll");
                // True - memory generation, false - external file generation
                parameters.GenerateInMemory = true;
                // True - exe file generation, false - dll file generation
                parameters.GenerateExecutable = false;

                string codeSnippet = this.SnippetRichTextBox.Text.Trim();

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeSnippet);

                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }

                    MessageBox.Show(sb.ToString(), "Code Compile Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Exception occurred while compiling the code. Message - {0}", ex.Message), "Code Compile Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }

        private void NewSnippetButton_Click(object sender, EventArgs e)
        {
            this.ClearControls();
        }


    }
}
