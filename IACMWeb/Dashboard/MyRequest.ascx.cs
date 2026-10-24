using Application;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DI.Chakra.Web.Dashboard
{
    public partial class MyRequest : UIBaseUC
    {
        private SessionStatusImpl _m_session = new SessionStatusImpl();
        protected void Page_Load(object sender, EventArgs e)
        {
            DashBoardHeaderUC.IsChartType = false;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("actionUserId", Convert.ToString(_m_session.User.Id));

            //List< Dto.Presentation.PresentationAdHocAccessRequest> requestsList = DomainApiClient<Dto.Presentation.PresentationAdHocAccessRequest>.GetByAction<List<Dto.Presentation.PresentationAdHocAccessRequest>>(DomainApiAction.PresentationAdHocAccessRequest.GetRequestsByActionUserId,parameter);
            var requestsList = GetAllData();
            if (requestsList != null && requestsList.Data != null && requestsList.Data.Count > 0)
            {
                MyRequestRepeater.DataSource = requestsList.Data.OrderByDescending(l => l.RequestedDate);
                MyRequestRepeater.DataBind();
                if (requestsList.TotalCount < 10)
                    DashBoardHeaderUC.DashBoardRowCount = "0" + requestsList.TotalCount;
                else
                    DashBoardHeaderUC.DashBoardRowCount = requestsList.TotalCount.ToString();
            }
            else
                DashBoardHeaderUC.DashBoardRowCount = "0";
        }

        Dto.Core.PresentationGridPagedData<PresentationMyRequestData> GetAllData()
        {
            Dictionary<string, string> paramtr = new Dictionary<string, string>();
            paramtr.Add("actionUserId", _m_session.User.Id.ToString());

            PresentationGridPagedDataParameter param = new PresentationGridPagedDataParameter
            {
                StartRowIndex = 0,
                MaximumRows = 10
            };

            var myRequests =
                DomainApiClient<Dto.Presentation.PresentationMyRequestData>.
                GetByAction<Dto.Core.PresentationGridPagedData<PresentationMyRequestData>>
                (DomainApiAction.Attestation.Presentation.PresentationMyRequest.GetPagedByActionUserId, paramtr, param);

            if (myRequests.Data != null && myRequests.Data.Count > 0)
            {
                myRequests.Data = myRequests.Data.Where(p => p.RequestSource == Constants.AccessRequest || p.RequestSource == Constants.BulkRequest || p.RequestSource == Constants.NonEmployee).ToList();
                return myRequests;
            }
            return null;
        }

        protected void lnkTaskDetail_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = sender as LinkButton;
                Label lblAdhocRequest = (Label)btn.Controls[3];
                HiddenField lblAccessRequestId = (HiddenField)btn.Controls[5];
                HiddenField lblBulkRequestId = (HiddenField)btn.Controls[6];
                HiddenField hdnRequestSource = (HiddenField)btn.Controls[7];
                this.ContextObjectId = (!string.IsNullOrWhiteSpace(lblBulkRequestId.Value) && long.Parse(lblBulkRequestId.Value) > 0) ? Convert.ToInt64(lblBulkRequestId.Value) : Convert.ToInt64(lblAccessRequestId.Value);

                SessionStatusImpl _m_session = new SessionStatusImpl();
                _m_session.RedirectFrom = "Home";
                if (string.IsNullOrEmpty(((HiddenField)btn.Controls[5]).Value))
                {
                    Response.Redirect("~/Request/List.aspx");
                }
                else
                {
                    Response.Redirect($"~/Request/Detail.aspx?contextId={this.ContextObjectId}&TypeOfRequest={hdnRequestSource.Value}");
                }
            }
            catch (Exception ex)
            {
                //supress the exception
            }
        }

        protected void MyRequestRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (PresentationMyRequestData)e.Item.DataItem;
                Label reqFor = e.Item.FindControl("lblReqFor") as Label;

                if (item != null && !string.IsNullOrWhiteSpace(item.RequestSource) && item.RequestSource.ToLower().Contains("bulk"))
                {
                    reqFor.Visible = false;
                }
                else
                {
                    reqFor.Visible = true;
                }
            }
        }
    }
}