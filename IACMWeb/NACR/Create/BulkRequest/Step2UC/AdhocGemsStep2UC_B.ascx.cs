using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
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
    public partial class AdhocGemsStep2UC_B : UIBaseStep2UC_B
    {
        const string V_REQUEST = "V_REQUEST";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        List<string> EntitlementRequest
        {
            get
            {
                if (ViewState[V_REQUEST] != null)
                {
                    return (List<string>)ViewState[V_REQUEST];
                }
                return new List<string>();
            }
            set
            {
                ViewState[V_REQUEST] = value;
            }
        }

        protected void RequestGrid_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var requests = this.EntitlementRequest;
                requests.RemoveAt(e.Item.ItemIndex);
                this.EntitlementRequest = requests;
                FillRequestGrid();
            }
        }
        protected override void InitializeStep()
        {
            if (AdhocRequest != null)
            {

                SetMandatory( RoleAstreix, true);
                EntitlementRequest.Clear();
                Session["EndpointUserId"] = null;
                RolesAutoCompleteBox.Entries.Clear();
                FillRequestGrid();
                BindRepeater();
                if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove || AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    if (AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        GroupPanelGEMS.Visible = true;
                    }
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.DeleteAccount)
                {
                    this.AddRequestButton.Visible = false;
                    GroupPanelGEMS.Visible = false;
                }
                if (AdhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    GroupPanelGEMS.Visible = true;
                    SetMandatory( RoleAstreix, false);
                }
            } 
        }

        protected override void BindAdhocRequest()
        {
            var Entitlement = (AdHocGEMSRequest)AdhocRequest.GetEntitlementDetail();
            if (Entitlement != null)
            {
                if (!string.IsNullOrEmpty(Entitlement.Entitlement))
                    EntitlementRequest = Entitlement.Entitlement.Split(',').Select(l => l).ToList();
                FillRequestGrid();
            }
        }

        protected void AddRequestButton_Click(object sender, EventArgs e)
        {
            if (RolesAutoCompleteBox.Entries.Count != 0)
            {
                var msg = string.Empty;
                if ((AdhocRequest.AdhocActionType == AdHocActionType.NewAccount || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd) && !ValidGemsRole())
                {
                    msg = "Cannot raise request for this entitlement as it needs AD account.";
                    
                }
                if (string.IsNullOrWhiteSpace(msg))
                {
                    List<string> requests = new List<string>();
                    string newRequest = RolesAutoCompleteBox.Text.Replace(";", "").Trim();
                    requests = this.EntitlementRequest;
                    List<string> listofOldEntiltement = this.EntitlementRequest.Select(entitlement => entitlement).ToList();
                    if (!EntitlementRequest.Contains(newRequest) && !string.IsNullOrEmpty(newRequest))
                    {
                        requests.Add(newRequest);
                        this.EntitlementRequest = requests;
                        FillRequestGrid();
                        RolesAutoCompleteBox.Entries.Clear();
                    }
                    else
                    {
                        ShowAlertMessage(Infrastructure.AlertType.Error, "The role is already in the request list.");
                    }
                }
                else
                {
                    ShowAlertMessage(Infrastructure.AlertType.Warning, msg);
                }
            }
            else
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please select the role name.");
            }
        }

        void FillRequestGrid()
        {
            if (this.EntitlementRequest == null)
            {
                EntitlementRequest = new List<string>();
            }
            RequestGrid.DataSource = this.EntitlementRequest;
            trGrdid.Visible = this.EntitlementRequest.Count == 0 ? false : true;
            RequestGrid.DataBind();
        }
        bool UpdateBulkReqAccountDetail()
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
                            ShowAlertMessage(Infrastructure.AlertType.Error, "Please enter the Account Information."); 
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

        void BindRepeater()
        {
            if (this.AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
            {
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
                            userNames.Add(i.EndpointUserId, i.AccountName);
                        }
                        repeaterData.Add(item.Name, userNames);
                    }
                });

                AccountRepeater.DataSource = repeaterData;
                AccountRepeater.DataBind();

            }
            else
            {
                Accountpanel.Visible = false;
            }
        }

        public override bool Validate()
        {

            if (AdhocRequest.AdhocActionType != AdHocActionType.DeleteAccount && this.EntitlementRequest.Count == 0)
            {
                ShowAlertMessage(Infrastructure.AlertType.Error, "Please add Role(s)");
                return false;
            }
            if ((GroupPanelGEMS.Visible && EntitlementRequest.Count <= 0))
            {
                string Message = "Please Select the ";
                string ControlMessage = "";
                if (GroupPanelGEMS.Visible && EntitlementRequest.Count <= 0 && AdhocRequest.AdhocActionType != AdHocActionType.NewAccount)
                {
                    ControlMessage = string.IsNullOrEmpty(ControlMessage) ? " Role" : ControlMessage + " and Role";
                }
                if (ControlMessage != "")
                {
                    Message = Message + ControlMessage + (ControlMessage.IndexOf(",") > 0 ? ";" : ".");
                    ShowAlertMessage(Infrastructure.AlertType.Error, Message + ".");
                    return false;
                }                
                return UpdateBulkReqAccountDetail() ;
            }
            if ((AdhocRequest.AdhocActionType == AdHocActionType.AmendmentAdd || AdhocRequest.AdhocActionType == AdHocActionType.AmendmentRemove) && !UpdateBulkReqAccountDetail())
            {
                return false;
            }
            return UpdateBulkReqAccountDetail();
        }

        protected override void SetEntitlementDetail()
        {
            AdHocGEMSRequest request = new AdHocGEMSRequest
            {
                Entitlement = string.Join(",", EntitlementRequest),
                EntitlementType = EntitlementType.Role
            };
             
            AdhocRequest.EntitlementDetail = AdhocRequest.GetSerializedString(request);
                      
        }

        protected override void afterSave()
        {
            EntitlementRequest.Clear();
            FillRequestGrid();
        }
        bool ValidGemsRole()
        {
            var gemsMatrixId = Convert.ToInt64(RolesAutoCompleteBox.Entries[0].Value);
            var matrix = DomainApiClient<Dto.GEMSRoleMatrix>.GetById(gemsMatrixId);
            if (matrix != null && !string.IsNullOrWhiteSpace(matrix.ADGroups))
            {
                var adParam = new Dictionary<string, string>();
                adParam.Add("applicationType", ((int)ApplicationType.ADSingaporePowerLocal).ToString());
                var endPoint = DomainApiClient<Dto.EndPoint>.GetByAction<EndPoint>(DomainApiAction.Endpoint.EndPoint.GetEndPointByApplicationType, adParam);
                foreach (var item in AdhocRequest.BulkRequestDetails)
                {
                    var param = new Dictionary<string, string>
                    {
                        { "userId", item.UserId.ToString()},
                        { "endpointSystemId", endPoint.Id.ToString()},
                        { "accountType", AccountType.User.ToString()}
                    };

                    List<AccountReconcilation> accounts = DomainApiClient<Dto.AccountReconcilation>.
                                                                    GetByAction<List<Dto.AccountReconcilation>>(
                                                                                    DomainApiAction.Provisioning.AccountReconcilation.GetByAccountTypeAndEndpointSystemId, param);

                    if (accounts == null || accounts.Count == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


    }

}