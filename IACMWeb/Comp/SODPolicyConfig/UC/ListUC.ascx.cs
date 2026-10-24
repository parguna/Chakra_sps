using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Web.Comp.SODPolicyConfig.UC
{
    public partial class ListUC : UIBaseUC, IAccessControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.MasterPage.ActionClicked += ActionClicked;
        }

        public void ActionClicked(object sender, AccessActionEventArgs e)
        {
            if (e.ActionType == ACLAction.Delete)
            {
                delete();
            }
        }

        protected void gvRisklevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem g = gvRisklevel.SelectedItems[0] as GridDataItem;
            this.ContextObjectId = (long)g.GetDataKeyValue("Id");
        }

        private void delete()
        {
            if (!ContextObjectId.HasValue)
                return;
            DomainApiClient<Dto.SODPolicyConfiguration>.DeleteById(this.ContextObjectId.Value);
            this.ContextObjectId = null;
            MasterPage.ShowAlert(AlertType.Success, "SOD Policy Configuration deleted successfully");
            gvRisklevel.Rebind();
        }


        protected void gvRisklevel_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gvRisklevel.DataSource = DomainApiClient<Dto.SODPolicyConfiguration>.GetAll(true, gvRisklevel.PageSize, gvRisklevel.CurrentPageIndex);
        }

        protected void gvRisklevel_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;                           
                string Operator = item["Operator"].Text;
                DI.Chakra.Infrastructure.Enums.Operator riskOperator = (Operator)Enum.Parse(typeof(Operator), Operator);
                if(riskOperator == Infrastructure.Enums.Operator.AND)
                {
                    item["Operator"].Text = Infrastructure.Enums.Operator.AND.ToString();
                }
                else if(riskOperator == Infrastructure.Enums.Operator.OR)
                {
                    item["Operator"].Text = Infrastructure.Enums.Operator.OR.ToString();
                }
                string RiskScoreOperator = item["RiskScoreOperator"].Text;

                DI.Chakra.Infrastructure.Enums.RiskScoreOperator riskScoreOperator = (RiskScoreOperator)Enum.Parse(typeof(RiskScoreOperator), RiskScoreOperator);
                if(riskScoreOperator == Infrastructure.Enums.RiskScoreOperator.Equalto)
                {
                    item["RiskScoreOperator"].Text = Infrastructure.Enums.RiskScoreOperator.Equalto.ToString();
                }
                else if (riskScoreOperator == Infrastructure.Enums.RiskScoreOperator.Greaterthan)
                {
                    item["RiskScoreOperator"].Text = Infrastructure.Enums.RiskScoreOperator.Greaterthan.ToString();
                }
                else if (riskScoreOperator == Infrastructure.Enums.RiskScoreOperator.Lessthan)
                {
                    item["RiskScoreOperator"].Text = Infrastructure.Enums.RiskScoreOperator.Lessthan.ToString();
                }
               
            }
        }
    }
}