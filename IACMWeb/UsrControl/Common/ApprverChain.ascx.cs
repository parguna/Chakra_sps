using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Web.UI;

namespace DI.Chakra.Web.UsrControl.Common
{
    public partial class ApprverChain : DI.Chakra.Web.UIBaseUC
    {

        public Dto.AdHocAccessRequest AdhocAccessRequest { get; set; }
        public long BulkRequestId { get; set; }

        public long EmployeeTriggerId { get; set; }

        public List<PresentationApprovalChain> TaskApprovers
        {
            get
            {
                if (ViewState["TaskApprovers"] != null)
                {
                    return (List<PresentationApprovalChain>)ViewState["TaskApprovers"];
                }
                else
                {
                    return new List<PresentationApprovalChain>();
                }
            }
            set
            {
                ViewState["TaskApprovers"] = value;
            }
        }

        protected void grdApprover_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var approvers = this.TaskApprovers;

            if (approvers.Count == 0)
            {
                approvers = this.GetApprovers();
                this.TaskApprovers = approvers;
            }

            grdApprover.DataSource = approvers;
        }

        private List<PresentationApprovalChain> GetApprovers()
        {
            if (this.AdhocAccessRequest != null)
            {

                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("adhocAccessRequestId", Convert.ToString(AdhocAccessRequest.Id));
                Dto.AccessRequest accessRequest = DomainApiClient<Dto.AccessRequest>.GetByAction<Dto.AccessRequest>
                    (DomainApiAction.Provisioning.AccessRequest.GetByAdHocAccessRequestId, param);

                if (!accessRequest.BulkRequestId.HasValue)
                {
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("accessRequestId", accessRequest.Id.ToString());
                    return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByRequestId, parameter);

                }
                else
                {
                    if (AdhocAccessRequest.ApplicationType == ApplicationType.InhouseSystems)
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("accessRequestId", accessRequest.Id.ToString());
                        return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByRequestId, parameter);
                    }
                    else
                    {
                        Dictionary<string, string> parameter = new Dictionary<string, string>();
                        parameter.Add("bulkRequestId", accessRequest.BulkRequestId.ToString());
                        return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
                    }
                }
            }
            else if (this.BulkRequestId != 0)
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("bulkRequestId", this.BulkRequestId.ToString());
                return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByBulkRequestId, parameter);
            }
            else
            {
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("triggerId", this.EmployeeTriggerId.ToString());
                return DomainApiClient<Dto.Presentation.PresentationApprovalChain>.GetByAction<List<Dto.Presentation.PresentationApprovalChain>>(DomainApiAction.Provisioning.Presentation.PresentationApprovalChain.GetByEmployeeRequestId, parameter);
            }
        }

        protected void grdApprover_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                //item["DelegationComments"].Text = item["DelegationComments"].Text.Replace("\n", "<br/>");
                //item["Comments"].Text = item["Comments"].Text.Replace("\n", "<br/>");
            }
        }

        protected void grdApprover_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
            byte state = (byte)dataItem.GetDataKeyValue("ApprovalState");

            try
            {
                var approver = this.TaskApprovers.FirstOrDefault(item => item.ApprovalState == state && item.ApproverName == dataItem.DataItem.ToString());
                e.DetailTableView.DataSource = approver.ApproverDetails;
            }
            catch
            {
                var approver = this.TaskApprovers.FirstOrDefault(item => item.ApprovalState == state);
                e.DetailTableView.DataSource = approver.ApproverDetails;
            }
            
        }

    }
}