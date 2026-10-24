using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace DataCleansingTool
{
    public partial class XLUploader : Form
    {
        public XLUploader()
        {
            InitializeComponent();
        }

        private void XLUploader_Load(object sender, EventArgs e)
        {
            this.ChangeControlVisibility();
            this.LoadApplications();
            this.LoadEndpoints();

            this.ImportFileDialog.Filter = "XLS|*.xls|XLSX|*.xlsx";
        }

        private void CreateUserRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.ChangeControlVisibility();
        }

        private void MapUserRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.ChangeControlVisibility();
        }

        private void ChangeControlVisibility()
        {
            this.SelectFilePanel.Visible = true;
            if (this.CreateUserRadioButton.Checked)
            {
                this.ApplicationPanel.Visible = false;
            }
            else
            {
                this.ApplicationPanel.Visible = true;
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            //string fileName = this.FileNameTextBox.Text.Trim();
            //if (string.IsNullOrEmpty(fileName))
            //{
            //    MessageBox.Show(this, "Please select a file to upload.", "Import File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //if (this.MapUserRadioButton.Checked && this.ApplicationsComboBox.SelectedIndex == 0)
            //{
            //    MessageBox.Show(this, "Please select the target application for upload.", "Import File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //this.LogTextBox.Clear();

            //EndpointType applicationType = (EndpointType)((byte)this.ApplicationsComboBox.SelectedValue);

            //FileProcessorBase fileProcessor = FileProcessorFactory.GetFileProcessor(applicationType);
            //fileProcessor.EndpointTypes = applicationType;
            //fileProcessor.UploadData(fileName);

            //this.LogTextBox.Text = fileProcessor.UploadLog;

            //this.FileNameTextBox.Clear();

            //this.ApplicationsComboBox.SelectedIndex = 0;

            //MessageBox.Show(this, "File upload completed.", "Import File", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            this.ImportFileDialog.ShowDialog();
        }

        private void ImportFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            this.FileNameTextBox.Text = this.ImportFileDialog.FileName;
        }

        private void LoadApplications()
        {
            try
            {
                Dictionary<byte, string> list = new Dictionary<byte, string>();

                this.ApplicationsComboBox.Items.Clear();
                foreach (EndpointType item in Enum.GetValues(typeof(EndpointType)))
                {
                    string description = UtilityMethods.GetEnumAttribute<Description>(item);
                    byte id = (byte)item;
                    if (!list.ContainsKey(id))
                    {
                        list.Add(id, description);
                    }
                }

                this.ApplicationsComboBox.DisplayMember = "Value";
                this.ApplicationsComboBox.ValueMember = "Key";
                this.ApplicationsComboBox.DataSource = new BindingSource(list, null);
            }
            catch
            {
                MessageBox.Show(this, "Error while populating Application list.", "Form Load", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEndpoints()
        {
            //List<EndPoint> _endPoints = new EndPointFacade().GetAllEndpoint();

            //this.EndpointsComboBox.DataSource = _endPoints;
            //this.EndpointsComboBox.DisplayMember = "HostName";
            //this.EndpointsComboBox.ValueMember = "EndpointSystemId";
        }

        private void ResolveAllButton_Click(object sender, EventArgs e)
        {
            //if (this.EndpointsComboBox.SelectedIndex == 0)
            //{
            //    MessageBox.Show(this, "Please select the target application to resolve exception.", "Resolve Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //this.LogTextBox.Clear();

            //int EndpointSystemId = Convert.ToInt32(this.EndpointsComboBox.SelectedValue);

            //EndPoint _endpoint = new EndPointFacade().GetEndPointSystemByKey(EndpointSystemId);

            //new ReconcilationFacade().ResolveAllExceptionsForFirstTimeDiscovery(_endpoint);

            //MessageBox.Show(this, "Resolved all exceptions for the selected endpoint.", "Resolve Exception", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string fileName = this.textBox1.Text.Trim();
            //if (string.IsNullOrEmpty(fileName))
            //{
            //    MessageBox.Show(this, "Please select a file to upload.", "Import File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            //this.LogTextBox.Clear();

            //FileProcessorBase fileProcessor = FileProcessorFactory.GetFileProcessor();
            //fileProcessor.IsApproverException = true;
            //fileProcessor.UploadData(fileName);

            //this.LogTextBox.Text = fileProcessor.UploadLog;

            //this.FileNameTextBox.Clear();

            //this.ApplicationsComboBox.SelectedIndex = 0;

            //MessageBox.Show(this, "File upload completed.", "Import File", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text = this.openFileDialog1.FileName;
        }

    }
}
