using DI.Chakra.Dto;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace DI.Chakra.Web.NACR.Create.BulkRequest.Step2UC
{
    public partial class AdhocADStep2UC_B : UIBaseStep2UC_B
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
   
        protected override void InitializeStep()
        {   
            txtRequesterComments.Text = "";
            if (AdhocRequest != null)
            {
                lblComments.CssClass = "control-label";
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    lblComments.CssClass = "control-label required";
                }
                GroupPanelAD.Visible = true;
                BindRepeater();
                LoadControl();
                //if (string.IsNullOrEmpty(AdhocRequest.CanInvokeRequest))
                //    BindAdhocRequest();
                //else
                //    AdhocRequest.CanInvokeRequest = string.Empty;
            }

        }

        private void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                AccountRepeater.Visible = true;
                Panel1.Visible = true;
                Dictionary<string, Dictionary<long, string>> repeaterData = new Dictionary<string, Dictionary<long, string>>();
                AdhocRequest.BulkRequestDetails.ForEach(item =>
                { 
                    var param = new Dictionary<string, string>
                    {
                        { "userId",item.UserId.ToString()},
                        { "endpointSystemId", AdhocRequest.EndpointSystemId.ToString()},
                        { "accountType", AccountType.User.ToString()}
                    };
                     
                    List<AccountReconcilation> userAccounts = DomainApiClient<Dto.AccountReconcilation>.
                                                                    GetByAction<List<Dto.AccountReconcilation>>(
                                                                                    DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemId, param);


                    if (userAccounts != null && userAccounts.Count > 0)
                    {
                        var userNames = new Dictionary<long, string>();

                        var items = userAccounts.Distinct();
                        if (items.Count() > 1)
                        {
                            userNames.Add(0, "--Please Select Account--");
                        }
                        foreach (var i in items)
                        {
                            userNames.Add(i.EndpointUserId , i.AccountName);
                        }
                        repeaterData.Add(item.Name, userNames);
                    }
                });

                AccountRepeater.DataSource = repeaterData;
                AccountRepeater.DataBind();

            }
            else
            {
                AccountRepeater.Visible = false;
                Panel1.Visible = false;
            }
        } 

        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocADRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                txtRequesterComments.Text = Entitlement.RequesterComment;
                if (Entitlement.AdHocADSharedAccount == null)
                    return;
            }
        }
        public override bool Validate()
        {
            bool result = true;
             
            if (
                (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                &&  string.IsNullOrWhiteSpace(txtRequesterComments.Text)
                )
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter Comments for Group Add/Remove");
                return false;
            } 

            if(UpdateBulkReqAccountDetail() == false)  
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter the Account Information.");
                result = false ;
            }
            return result;
        }

        private bool ValidateAccountInformation()
        {
            return false;

        }
        private void LoadControl()
        {
            if (AdhocRequest != null)
            {
                this.tdGroupAdd.Visible = AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove;
                SetMandatory(tdGroupAdd,!(AdhocRequest.AdhocActionType == AdHocActionType.NewAccount));
            }
        }



        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveBulkRequest();
        }

        protected override void SetEntitlementDetail()
        {     
            AdHocADRequest adhocAD = new AdHocADRequest();
            if (AdhocRequest != null)
            {

                //if (string.IsNullOrEmpty(adhocAD.RequesterComment) && AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                //{
                //    AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(adhocAD);
                //    //AdhocRequest.Id = 0;
                //    return;
                //}
                adhocAD.EntitlementType = EntitlementType.Group;
                adhocAD.Entitlement = "";
                adhocAD.RequesterComment = txtRequesterComments.Text;
                if (adhocAD.RequesterComment != null)
                {
                    adhocAD.RequesterComment = adhocAD.RequesterComment.Replace("\n", "<br>");
                }

                AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(adhocAD);

            } 
        }

        protected override void afterSave()
        {
            base.afterSave();
            txtRequesterComments.Text = "";
        }

        private bool UpdateBulkReqAccountDetail()
        {
            foreach (GridDataItem dataItem in AccountRepeater.Items)
            {

                if (dataItem.ItemType == GridItemType.AlternatingItem || dataItem.ItemType == GridItemType.Item)
                {
                    var userName = ((Label)dataItem.FindControl("lblAcountName")).Text;
                    var SelectedAccount = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Text;
                    var endpointuserid = ((RadComboBox)dataItem.FindControl("ddlAccounts")).SelectedItem.Value;
                    if (Convert.ToInt32(endpointuserid) == 0)
                    {
                        return false;
                    }
                    AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).EndpointUserId = Convert.ToInt32(endpointuserid);
                    AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).AccountName = SelectedAccount;
                }
            }
            return true;
        } 
    }
}