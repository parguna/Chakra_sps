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
    public partial class AdhocNpacStep2UC_B : UIBaseStep2UC_B
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public Dictionary<int, string> LevelData
        {
            get
            {
                if (ViewState["LevelData"] != null)
                {
                    return (Dictionary<int, string>)ViewState["LevelData"];
                }
                return null;
            }
            set
            {
                ViewState["LevelData"] = value;
            }
        }

        protected override void InitializeStep()
        {
            if (AdhocRequest != null)
            {
                BindLevels(AdhocRequest.BulkRequestDetails[0].UserId);
                BindRepeater();
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        OtherControls.Visible = true;
                    }
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    OtherControls.Visible = false;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    OtherControls.Visible = true;
                } 
            }
        }

        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocNPACRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (LevelRadComboBox.Items.FindItemByValue(Entitlement.Entitlement == null ? "--Select Level--" : Convert.ToString(Entitlement.Entitlement)) != null)
                    LevelRadComboBox.SelectedValue = Entitlement.Entitlement == null ? "--Select Level--" : Convert.ToString(Entitlement.Entitlement);
            }
        }
        void BindLevels(long RequesteeID)
        {
            LevelData  = DomainApiClient<Dto.NPACLevelInfo>.GetByAction<Dictionary<int, string>>(DomainApiAction.Endpoint.NPAC.GetNPACLevelInfoList,
                 new Dictionary<string, string> { { "RequesteeId", RequesteeID.ToString() } });
            bindLevelControls(LevelData);
        }

        void bindLevelControls(Dictionary<int, string> dic)
        {
            LevelRadComboBox.DataSource = dic;
            LevelRadComboBox.DataTextField = "value";
            LevelRadComboBox.DataValueField = "key";
            LevelRadComboBox.DataBind();
        }

        void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
                AccountRepeater.Visible = true;
                Accountpanel.Visible = true;
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
                Accountpanel.Visible = false;
            }
        }

        protected override void SetEntitlementDetail()
        {
            AdHocNPACRequest request = new AdHocNPACRequest
            {
                Entitlement = LevelRadComboBox.SelectedValue == "0" ? null : LevelRadComboBox.SelectedValue,
                EntitlementType = EntitlementType.Level
            };
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);

        }
        public override bool Validate()
        {
            if (OtherControls.Visible && (LevelRadComboBox.SelectedValue == "0"))
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please Select the Level.");
            } 
            return UpdateBulkReqAccountDetail();
        }

        private bool UpdateBulkReqAccountDetail()
        {
            if (AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
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
                            ShowAlertMessage (Infrastructure.AlertType.Error, "Please enter the Account Information."); 
                            return false;
                        }
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).EndpointUserId = Convert.ToInt32(endpointuserid);
                        AdhocRequest.BulkRequestDetails.FirstOrDefault(item => item.Name == userName).AccountName = SelectedAccount;
                    }
                }
                return true;
            }
            else
            {
                return true;
            }
        } 
         
    }
}