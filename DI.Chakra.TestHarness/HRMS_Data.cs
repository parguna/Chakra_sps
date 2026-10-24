using DI.Chakra.Connector;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestHarness
{
    public partial class HRMS_Data : Form
    {
        readonly SAPHRMSConnector connector;
        public HRMS_Data()
        {
            InitializeComponent();
            var endpoint = EndPoint.GetByApplicationType(DI.Chakra.Infrastructure.Enums.ApplicationType.HRMS_FLINS, true);
            connector = new SAPHRMSConnector(endpoint, new DiscoveryLogger(0, endpoint.Id, EndpointType.None, string.Empty) );
            cmbTableNames.SelectedIndex = 0;
            cmbEventNames.SelectedIndex = 0;
        }

        private void cmbEventNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cmb = sender as ComboBox;
            dgvEventCodeAndReason.DataSource = null;
            switch (cmb.SelectedItem)
            {
                case "On Board" : dgvEventCodeAndReason.DataSource = CreateEventDataTable(MasterConfigHelper.OnBoardTriggers);
                    break;
                case "Off Board":
                    dgvEventCodeAndReason.DataSource = CreateEventDataTable(MasterConfigHelper.OffBoardTriggers);
                    break;
                case "Transfer":
                    dgvEventCodeAndReason.DataSource = CreateEventDataTable(MasterConfigHelper.TranferTriggers);
                    break;
                case "Re-Org":
                    dgvEventCodeAndReason.DataSource = CreateEventDataTable(MasterConfigHelper.ReOrgTriggers);
                    break;
                default:
                    dgvEventCodeAndReason.DataSource = CreateEventDataTable(new List<EventTypeReason>());
                    break;
            }
        }



        DataTable CreateEventDataTable(List<EventTypeReason> eventTypeReasons)

        {
            DataTable dt = new DataTable();
            DataColumn dc = null;

            dc = new DataColumn();
            dc.ColumnName = "EventType";
            dc.DataType = typeof(string);
            dt.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "EventReason";
            dc.DataType = typeof(string);
            dt.Columns.Add(dc);

            foreach (var evt in eventTypeReasons)
            {
                var dr = dt.NewRow();
                dr["EventType"] = evt.EventType;
                dr["EventReason"] = evt.EventReason;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pernr = txtPERNR.Text;

            if (string.IsNullOrWhiteSpace(pernr))
            {
                MessageBox.Show("Enter PERNR");
                return;
            }

            if (cmbTableNames.SelectedIndex == 0)
            {
                MessageBox.Show("Select the Table Name.");
                return;
            } 

            switch (cmbTableNames.SelectedItem)
            {
                case "PA0000": dgvTableDetails.DataSource = connector.GetTablePA0000(pernr);
                    break;
                case "PA0001":
                    dgvTableDetails.DataSource = connector.GetTablePA0001(pernr);
                    break;
                case "PA0002":
                    dgvTableDetails.DataSource = connector.GetTablePA0002(pernr);
                    break;
                case "PA0302":
                    dgvTableDetails.DataSource = connector.GetTablePA0302(pernr);
                    break;
            }

        }
    }
}
