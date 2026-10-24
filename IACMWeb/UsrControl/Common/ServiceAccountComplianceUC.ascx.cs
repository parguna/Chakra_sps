using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class ServiceAccountComplianceUC : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //public string SecurityResponseString
        //{
        //    get; set;
        //}

        public List<Dto.SecurityResponseGridData> SecurityResponse
        {
            get; set;
        }

        protected void RequestGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {  
            RequestGrid.DataSource = SecurityResponse;
            //RequestGrid.DataBind();
        }
    }
}