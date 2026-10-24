using DI.Chakra.DBConfigHelper;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Admin.House_Keeping.UC
{
    public partial class DetailUC : UIBaseUC
    {
        public delegate void Add_SubmitButton(object sender, EventArgs e);
        public event Add_SubmitButton Add_SubmitButtonClicked;
        public delegate void Add_CancelButton(object sender, EventArgs e);
        public event Add_CancelButton Add_CancelButtonClicked;

        public string TableName
        {
            get { return TableNames.SelectedItem.Text; }
            set
            {
                TableNames.ClearSelection();
                if (TableNames.Items.FindByText(value.ToString()) != null)
                    TableNames.Items.FindByText(value.ToString()).Selected = true;
            }
        }
        public string Description
        {
            get { return txtDescription.Text; }
            set { txtDescription.Text = value; }
        }
        public int DaysToKeep
        {
            get { return String.IsNullOrEmpty(txtDaysToKeep.Text) ? 0 : Convert.ToInt32(txtDaysToKeep.Text); }
            set
            {
                txtDaysToKeep.Text = value.ToString();
            }
        }
        public int PurgeModeConfig
        {
            get { return ddlPurgeMode.SelectedIndex; }
            set { ddlPurgeMode.SelectedIndex = ddlPurgeMode.Items.IndexOf(ddlPurgeMode.Items.FindByValue(value.ToString())); }
        }
        public string update_btn
        {
            get { return btnSubmit.Text; }
            set { btnSubmit.Text = value; }
        }
        public string header
        {
            get { return H1.InnerText; }
            set { H1.InnerText = value; }
        }
        public void BindPurgeMode()
        {
            ddlPurgeMode.Items.Insert(0, new ListItem("--Select--", "-1"));
            try
            {
                foreach (PurgeMode item in Enum.GetValues(typeof(PurgeMode)))
                {
                    var description = UtilityMethods.GetEnumAttribute<Description>(item);
                    ListItem listItem = new ListItem(description, ((byte)item).ToString());
                    ddlPurgeMode.Items.Add(listItem);
                }
            }
            catch
            {
                throw;
            }
        }

        public void BindTableNames()
        {
            string[] tables = MasterConfigHelper.PurgeTables.Split(',');

            List<string> tNames = new List<string>();
            foreach (string tName in tables)
            {
                tNames.Add(tName);
            }

            TableNames.Items.Clear();
            TableNames.DataSource = tNames;
            TableNames.DataBind();
            TableNames.Items.Insert(0, "--Select--");

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindPurgeMode();
                BindTableNames();
            }
        }

        public void Clearvalue()
        {
            TableNames.ClearSelection();
            txtDescription.Text = string.Empty;
            txtDaysToKeep.Text = "365";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Add_CancelButtonClicked != null)
            {
                Add_CancelButtonClicked(sender, e);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Add_SubmitButtonClicked != null)
            {
                Add_SubmitButtonClicked(sender, e);
            }
        }
    }
}