using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Report.Company
{
    public partial class Company : UIBasePage
    {
        private static List<Dto.NOrganisation> companyList;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Excel || e.ActionType == ACLAction.CSV)
            {
                grdCompany.ExportSettings.FileName = "Company";
                grdCompany.Export(grdCompany, e.ActionType);
            }
            else if (e.ActionType == ACLAction.ExcelAll || e.ActionType == ACLAction.CSVAll)
            {
                grdCompany.Export<List<Dto.NOrganisation>>(companyList, e.ActionType, "Company");
            }
        }
        protected void grdCompany_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();         
            parameters.Add("FromDate", DatePickerFromDate.SelectedDate.HasValue ? DatePickerFromDate.SelectedDate.Value.ToString("dd-MM-yyyy 00:00:00.001") : string.Empty);
            parameters.Add("ToDate", DatePickerFromDate.SelectedDate.HasValue ? DatePickerFromDate.SelectedDate.Value.ToString("dd-MM-yyyy 23:59:59.999") : string.Empty);
            parameters.Add("Internal", chkIsInternalCompany.Checked.ToString());
            companyList = DomainApiClient<Dto.NOrganisation>.GetByAction<List<Dto.NOrganisation>>("GetCompaniesByFilter", parameters);
            grdCompany.DataSource = companyList;
        }

        protected void search_Click(object sender, EventArgs e)
        {        
            grdCompany.Rebind();
        }
    }
}