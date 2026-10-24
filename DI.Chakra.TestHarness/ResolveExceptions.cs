using DI.Chakra.DomainModel;
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
    public partial class ResolveExceptions : Form
    {
        public ResolveExceptions()
        {
            InitializeComponent();
        }


        private void ResolveExceptions_Load(object sender, EventArgs e)
        {
            List<EndPoint> _endPoints = EndPoint.GetAllIncludingEndPointSystemType();
            comboBox1.DataSource = _endPoints;
            comboBox1.DisplayMember = "HostName";
            comboBox1.ValueMember = "EndpointSystemId";
            
        }

        private void btnResolveExceptions_Click(object sender, EventArgs e)
        {
            int EndpointSystemId = Convert.ToInt32(comboBox1.SelectedValue);

            EndPoint _endpoint = EndPoint.GetByIdIncludingOwner(EndpointSystemId);

            //EndPoint.ResolveAllExceptionsForFirstTimeDiscovery(_endpoint);
        }
    }
}
