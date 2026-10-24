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

namespace DI.Chakra.Web.NACR.Create.Common
{
    public partial class AdhocMoreDetails : System.Web.UI.UserControl
    {
        public event EventHandler Close;
        private string RequesterText
        {
            get
            {
                if (ViewState["RequesterText"] != null)
                {
                    return (string)ViewState["RequesterText"];
                }
                return "";
            }
            set
            {
                ViewState["RequesterText"] = value;
            }
        }

        AccountReconcilation getAccountReconcilation(long endpointUserId, long endpointSystemId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        {"endpointUserId",endpointUserId.ToString() },
                        {"endpointSystemId" , endpointSystemId.ToString() }
                    };
            AccountReconcilation adusr = DomainApiClient<Dto.AccountReconcilation>.GetByAction<AccountReconcilation>(DomainApiAction.Provisioning.AccountReconcilation.GetByUser, param);// new ActiveDirectoryFacade().GetADUserByUserId(adhocAD.EndpointUserId);
            return adusr;
        }
        public void RaiseModelPopup(AdHocAccessRequest adhocaccessrequest)
        {
            //AdHocAccessRequest adhocaccessrequest = new AdHocAccessRequest();
            //AdHocRequestFacade adhocfacad = new AdHocRequestFacade();
            //InterimStoreFacade interimstorefacad = new InterimStoreFacade();
            //AccessRequestFacade accessrequestfacad = new AccessRequestFacade();
            string str = "";
            string[] arr;
            ApplicationGrid.Visible = false;
            //var listinterim = interimstorefacad.GetFromInterimStoreByInterimStoreId(Interimstoreid);
            //adhocaccessrequest = UtilityMethods.Deserialize<AdHocAccessRequest>(listinterim.XmlData);
            IEntitlementDetail ientitlement = adhocaccessrequest.GetEntitlementDetail();
            trHRMSFLINS.Visible = false;
            BussinessRoleRow.Visible = false;
            #region IDMS
            if (adhocaccessrequest.ApplicationType == ApplicationType.IDMS)
            {
                this.RadGridPopup.Visible = true;
                UserControlVisibility(PanelofSSIDS.ID);
                AdHocIACMRequest adHocIACMRequest = (AdHocIACMRequest)ientitlement;
                if (adHocIACMRequest != null)
                {
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation adusr = getAccountReconcilation(adHocIACMRequest.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);
                        lblUserName.Text = adusr?.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }

                    if (adHocIACMRequest.AdHocIACMRequestDetails != null)
                    {
                        RadGridSSIDS.DataSource = adHocIACMRequest.AdHocIACMRequestDetails.Select(item => new { EntitlementType = UtilityMethods.GetEnumAttribute<Description>(item.EntitlementType), Entitlement = item.Entitlement });

                        RadGridSSIDS.DataBind();
                        RadGridSSIDS.Visible = true;
                    }
                }
            }
            #endregion

            #region EAM NOVA
            if (adhocaccessrequest.ApplicationType == ApplicationType.EAM)
            {
               
                AdHocNovaRequest adhocNOVA = (AdHocNovaRequest)ientitlement;
                if (adhocNOVA != null)
                {
                    UserControlVisibility(EAM_Panel.ID);
                    IList<AdHocSAPSharedAccount> listhrmshocadshared = new List<AdHocSAPSharedAccount>();
                    if (adhocNOVA.AdHocSAPSharedAccountDetail != null)
                    {
                        listhrmshocadshared.Add(adhocNOVA.AdHocSAPSharedAccountDetail);
                        eam_AccountGrid.DataSource = listhrmshocadshared.Count > 0 ? listhrmshocadshared : null;
                        eam_AccountGrid.DataBind();
                        eam_AccountGrid.Visible = listhrmshocadshared.Count > 0 ? true : false;
                        EAM_Entitlement_Panel.Visible = listhrmshocadshared.Count > 0 ? true : false;
                    }

                    var visible = adhocNOVA.RequestDetails.Count > 0 ? true : false;

                    NovaRoleGrid.DataSource = adhocNOVA.RequestDetails.Select(c => c);
                    NovaRoleGrid.DataBind();
                    NovaRoleGrid.Visible = visible;
                    NovaRoleGrid.Visible = visible; 
                }
            }
            #endregion EAM NOVA
            #region SSIDS


            else if (adhocaccessrequest.ApplicationType == ApplicationType.SSIDS_GUI || adhocaccessrequest.ApplicationType == ApplicationType.SSIDS_SMS)
            {
                this.RadGridPopup.Visible = true;
                UserControlVisibility(PanelofSSIDS.ID);
                //pnlSSIDSEntitlement.Visible = true;
                AdHocSSIDSRequest adhocSSIDS = (AdHocSSIDSRequest)ientitlement;
                if (adhocSSIDS != null)
                {
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation adusr = getAccountReconcilation(adhocSSIDS.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);
                        lblUserName.Text = adusr.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }

                    var type = UtilityMethods.GetEnumAttribute<Description>(adhocSSIDS.EntitlementType);
                    var entitlement = adhocSSIDS.Entitlement;
                    var list = new List<AdHocSSIDSRequest> { adhocSSIDS };
                    RadGridSSIDS.DataSource = list.Select(item => new { EntitlementType = UtilityMethods.GetEnumAttribute<Description>(item.EntitlementType), Entitlement = item.Entitlement });

                    RadGridSSIDS.DataBind();
                    RadGridSSIDS.Visible = true;
                    //divET.Visible = true;
                    //lblssidsET.Text = UtilityMethods.GetEnumAttribute<Description>(adhocSSIDS.EntitlementType);
                    //if (adhocSSIDS.EntitlementType == EntitlementType.HandPhone)
                    //{
                    //    divHP.Visible = true;
                    //    lblssidsHP.Text = adhocSSIDS.Entitlement;
                    //}
                    //else if (adhocSSIDS.EntitlementType == EntitlementType.Role)
                    //{
                    //    divRole.Visible = true;
                    //    lblssidsRL.Text = adhocSSIDS.Entitlement;
                    //} 
                }
            }
            #endregion

            #region ADS
            else if (adhocaccessrequest.ApplicationType == ApplicationType.ADSingaporePowerLocal)
            {
                this.RadGridPopup.Visible = true;
                UserControlVisibility(PanelofAD.ID);
                AdHocADRequest adhocAD = (AdHocADRequest)ientitlement;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    AccountReconcilation adusr = getAccountReconcilation(adhocAD.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);
                    lblUserName.Text = adusr.AccountName;
                    trUsername.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                }

                if (adhocAD.RequesterComment != null)
                    adhocAD.RequesterComment = adhocAD.RequesterComment.Replace("\n", "<br/>");
                RequesterText = adhocAD.RequesterComment;
                str = adhocAD.Entitlement;
                arr = str.Split(',');
                IList<AdHocADRequest> listadhocad = new List<AdHocADRequest>();
                for (int i = 0; i < arr.Length; i++)
                {
                    AdHocADRequest adhocAD1 = new AdHocADRequest();
                    adhocAD1.Entitlement = arr[i];
                    listadhocad.Add(adhocAD1);
                }
                RadGridPopup.MasterTableView.GetColumn("GrouporRequestorText").HeaderText = "Request Details";

                RadGridPopup.DataSource = listadhocad;
                RadGridPopup.DataBind();
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                {
                    IList<AdHocADSharedAccount> listadhocadshared = new List<AdHocADSharedAccount>();
                    if (adhocAD.AdHocADSharedAccount != null)
                    {
                        listadhocadshared.Add(adhocAD.AdHocADSharedAccount);
                    }
                    this.SharedDetailsGridAD.Visible = true;
                    SharedDetailsGridAD.DataSource = listadhocadshared.Count > 0 ? listadhocadshared : null;
                    SharedDetailsGridAD.MasterTableView.GetColumn("EmailAddress").Display = false;
                    SharedDetailsGridAD.MasterTableView.GetColumn("DisplayName").Display = true;
                    SharedDetailsGridAD.MasterTableView.GetColumn("Description").Display = true;
                    SharedDetailsGridAD.DataBind();
                    SharedAccountDetailPanel.Visible = listadhocadshared.Count > 0 ? true : false;
                }
                else
                {
                    SharedAccountDetailPanel.Visible = false;
                }

            }
            #endregion

            #region SAP Local
            else if (adhocaccessrequest.ApplicationType == ApplicationType.SAPLocal)
            {
                this.RadGridPopup.Visible = true;
                UserControlVisibility(PanelofAD.ID);
                AdHocSAPLocalRequest adhocAD = (AdHocSAPLocalRequest)ientitlement;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    AccountReconcilation adusr = getAccountReconcilation(adhocAD.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);
                    lblUserName.Text = adusr.AccountName;
                    trUsername.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                }
                if (adhocAD.RequesterComment != null)
                    adhocAD.RequesterComment = adhocAD.RequesterComment.Replace("\n", "<br/>");
                RequesterText = adhocAD.RequesterComment;
                str = adhocAD.Entitlement;
                arr = str.Split(',');
                IList<AdHocSAPLocalRequest> listadhocad = new List<AdHocSAPLocalRequest>();
                for (int i = 0; i < arr.Length; i++)
                {
                    AdHocSAPLocalRequest adhocAD1 = new AdHocSAPLocalRequest();
                    adhocAD1.Entitlement = arr[i];
                    listadhocad.Add(adhocAD1);
                }
                RadGridPopup.MasterTableView.GetColumn("GrouporRequestorText").HeaderText = "Request Details";

                RadGridPopup.DataSource = listadhocad;
                RadGridPopup.DataBind();
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount)
                {
                    IList<AdHocSAPLocalRequest> listadhocadshared = new List<AdHocSAPLocalRequest>();
                    if (adhocAD != null)
                    {
                        listadhocadshared.Add(adhocAD);
                    }
                    this.SharedDetailsGridAD.Visible = true;
                    SharedDetailsGridAD.DataSource = listadhocadshared.Count > 0 ? listadhocadshared : null;
                    SharedDetailsGridAD.MasterTableView.GetColumn("EmailAddress").Display = false;
                    SharedDetailsGridAD.MasterTableView.GetColumn("DisplayName").Display = true;
                    SharedDetailsGridAD.MasterTableView.GetColumn("Description").Display = true;
                    SharedDetailsGridAD.DataBind();
                    SharedAccountDetailPanel.Visible = listadhocadshared.Count > 0 ? true : false;
                }
                else
                {
                    SharedAccountDetailPanel.Visible = false;
                }

            }
            #endregion

            #region VPN
            else if (adhocaccessrequest.ApplicationType == ApplicationType.VPN)
            {
                try
                {
                    this.ApplicationGrid.Visible = true;
                    UserControlVisibility(VPNDetailsPanel.ID);
                    AdHocVPNRequest adhocVPNDetails = (AdHocVPNRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation adusr = getAccountReconcilation(adhocVPNDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);
                        lblUserName.Text = adusr.AccountName;
                        lblUserName.Visible = true;
                        trUsername.Visible = true;
                    }

                    if (!string.IsNullOrWhiteSpace(adhocVPNDetails.OneKey))
                        lblOneKey.Text = "Token Number " + adhocVPNDetails.OneKey;
                    if (adhocVPNDetails.UpdateOneKeyToken)
                    {
                        lblOneKey.Text = "Update Token Number: Yes";
                    }
                    ApplicationGrid.DataSource = adhocVPNDetails.AdHocVPNRequestDetails.AsEnumerable().Select(c => c);
                    ApplicationGrid.DataBind();

                    if (ApplicationGrid.Items.Count < 1)
                    {
                        ApplicationGrid.Visible = false;
                    }

                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region HRMS

            else if (adhocaccessrequest.ApplicationType == ApplicationType.HRMS_FLINS)
            {
                AdHocSAPHRFLINSRequest adhocsap = (AdHocSAPHRFLINSRequest)ientitlement;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    AccountReconcilation sapusr = getAccountReconcilation(adhocsap.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                    lblUserName.Text = sapusr.AccountName;
                    trUsername.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                }
                UserControlVisibility(MoreDetailPanel.ID);
                var detail = adhocsap.AdHocSAPHRFLINSRequestDetails;
                BussinessRoleRow.Visible = false;
                trHRMSFLINS.Visible = HRMSGRID.Visible = detail.Count > 0 ? true : false;
                HRMSGRID.DataSource = detail;
                HRMSGRID.DataBind();
                HRMSGRID.MasterTableView.GetColumn("IsCompanyRetain").Visible = adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                {
                    IList<AdHocSAPSharedAccount> listhrmshocadshared = new List<AdHocSAPSharedAccount>();
                    if (adhocsap.AdHocSAPHRFLINSSharedAccount != null)
                    {
                        listhrmshocadshared.Add(adhocsap.AdHocSAPHRFLINSSharedAccount);
                    }
                    this.SharedHRMSPanel.Visible = true;
                    SharedHRMSGrid.DataSource = listhrmshocadshared.Count > 0 ? listhrmshocadshared : null;
                    SharedHRMSGrid.DataBind();
                    SharedHRMSGrid.Visible = listhrmshocadshared.Count > 0 ? true : false;
                }
                else
                {
                    SharedHRMSPanel.Visible = false;
                }

            }
            #endregion
            #region FLINSBI
            else if (adhocaccessrequest.ApplicationType == ApplicationType.FLINS_BI)
            {
                AdHocSAPFLINSBIRequest adhocsap = (AdHocSAPFLINSBIRequest)ientitlement;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    AccountReconcilation sapusr = getAccountReconcilation(adhocsap.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                    lblUserName.Text = sapusr.AccountName;
                    trUsername.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                }
                UserControlVisibility(MoreDetailPanel.ID);


                var list = adhocsap.Entitlement.Split(',').Select(e => new { Entitlement = e }).ToList();
                radGrdbusinessrole.DataSource = list;
                radGrdbusinessrole.DataBind();
                BussinessRoleRow.Visible = list != null && list.Count > 0 ? true : false;

                if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                {
                    IList<AdHocSAPSharedAccount> listflinsbihocadshared = new List<AdHocSAPSharedAccount>();
                    if (adhocsap.AdHocFLINSBISharedAccount != null)
                    {
                        listflinsbihocadshared.Add(adhocsap.AdHocFLINSBISharedAccount);
                    }
                    this.SharedHRMSPanel.Visible = true;
                    SharedHRMSGrid.DataSource = listflinsbihocadshared.Count > 0 ? listflinsbihocadshared : null;
                    SharedHRMSGrid.DataBind();
                    SharedHRMSGrid.Visible = listflinsbihocadshared.Count > 0 ? true : false;
                }
                else
                {
                    SharedHRMSPanel.Visible = false;
                }

            }
            #endregion
            #region SAPGRC
            else if (adhocaccessrequest.ApplicationType == ApplicationType.SAP_GRC)
            {
                AdHocSAPGRCRequest adhocsap = (AdHocSAPGRCRequest)ientitlement;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    AccountReconcilation sapusr = getAccountReconcilation(adhocsap.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                    lblUserName.Text = sapusr.AccountName;
                    trUsername.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                }
                UserControlVisibility(MoreDetailPanel.ID);
                var list = adhocsap.Entitlement.Split(',').Select(e => new { Entitlement = e }).ToList();
                radGrdbusinessrole.DataSource = list;
                radGrdbusinessrole.DataBind();
                BussinessRoleRow.Visible = list != null && list.Count > 0 ? true : false;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                {
                    IList<AdHocSAPSharedAccount> listsaphocadshared = new List<AdHocSAPSharedAccount>();
                    if (adhocsap.AdHocSAPGRCSharedAccount != null)
                    {
                        listsaphocadshared.Add(adhocsap.AdHocSAPGRCSharedAccount);
                    }
                    this.SharedHRMSPanel.Visible = true;
                    SharedHRMSGrid.DataSource = listsaphocadshared.Count > 0 ? listsaphocadshared : null;
                    SharedHRMSGrid.DataBind();
                    SharedHRMSGrid.Visible = listsaphocadshared.Count > 0 ? true : false;
                }
                else
                {
                    SharedHRMSPanel.Visible = false;
                }

            }
            #endregion
            #region SAPPI
            else if (adhocaccessrequest.ApplicationType == ApplicationType.SAP_PI)
            {
                AdHocSAPPIRequest adhocsap = (AdHocSAPPIRequest)ientitlement;

                if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    AccountReconcilation sapusr = getAccountReconcilation(adhocsap.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                    lblUserName.Text = sapusr.AccountName;
                    trUsername.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                }
                UserControlVisibility(MoreDetailPanel.ID);
                var list = adhocsap.Entitlement.Split(',').Select(e => new { Entitlement = e }).ToList();
                radGrdbusinessrole.DataSource = list;
                radGrdbusinessrole.DataBind();
                BussinessRoleRow.Visible = list != null && list.Count > 0 ? true : false;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                {
                    IList<AdHocSAPSharedAccount> listsaphocadshared = new List<AdHocSAPSharedAccount>();
                    if (adhocsap.AdHocSAPPISharedAccount != null)
                    {
                        listsaphocadshared.Add(adhocsap.AdHocSAPPISharedAccount);
                    }
                    this.SharedHRMSPanel.Visible = true;
                    SharedHRMSGrid.DataSource = listsaphocadshared.Count > 0 ? listsaphocadshared : null;
                    SharedHRMSGrid.DataBind();
                    SharedHRMSGrid.Visible = listsaphocadshared.Count > 0 ? true : false;
                }
                else
                {
                    SharedHRMSPanel.Visible = false;
                }

            }
            #endregion
            #region ERecuritment

            #endregion
            #region InhouseSystem
            else if (adhocaccessrequest.ApplicationType == ApplicationType.InhouseSystems)
            {
                this.RadGridPopup.Visible = true;
                AdHocINHRequest adhocinhouse = (AdHocINHRequest)ientitlement;
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                {
                    AccountReconcilation INHusr = getAccountReconcilation(adhocinhouse.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                    lblUserName.Text = INHusr.AccountName;
                    trUsername.Visible = true;
                }
                else
                {
                    trUsername.Visible = false;
                }
                UserControlVisibility(pnlINH.ID);
                str = adhocinhouse.Entitlement;

                //arr = str.Split(',');
                //IList<AdHocINHRequest> listadhocinhouse = new List<AdHocINHRequest>();
                //for (int i = 0; i < arr.Length; i++)
                //{
                //    AdHocINHRequest adhocinhouse1 = new AdHocINHRequest();
                //    adhocinhouse1.Entitlement = arr[i];
                //    listadhocinhouse.Add(adhocinhouse1);
                //}
                Dictionary<string, string> param = new Dictionary<string, string>
                    {
                        {"endpointsystemId",adhocaccessrequest.EndpointSystemId.ToString() },
                        {"entitlement" , adhocinhouse.Entitlement }
                    };
                var result = DomainApiClient<Dto.InhousePermission>.GetByAction<List<InhouserEntitlementCustom>>("GetPermissionAndFunctionWithDescription", param);
                if (result != null && result.FirstOrDefault() != null && string.IsNullOrWhiteSpace(result.FirstOrDefault().Function))
                {
                    radGridINH.MasterTableView.GetColumn("Function").Visible = false;
                    radGridINH.MasterTableView.GetColumn("FunctionDescription").Visible = false;
                }
                else
                {
                    radGridINH.MasterTableView.GetColumn("Function").Visible = true;
                    radGridINH.MasterTableView.GetColumn("FunctionDescription").Visible = true;
                }
                //radGridINH.MasterTableView.GetColumn("GrouporRequestorText").HeaderText = "Inhouse Entitlement Details";
                radGridINH.DataSource = result;
                radGridINH.DataBind();
                if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                {
                    IList<AdHocINHSharedAccount> listadhocadshared = new List<AdHocINHSharedAccount>();
                    if (adhocinhouse.AdHocINHSharedAccount != null)
                    {
                        listadhocadshared.Add(adhocinhouse.AdHocINHSharedAccount);
                    }
                    this.SharedDetailsGridAD.Visible = true;
                    SharedDetailsGridAD.DataSource = listadhocadshared.Count > 0 ? listadhocadshared : null;
                    SharedDetailsGridAD.MasterTableView.GetColumn("EmailAddress").Display = false;
                    SharedDetailsGridAD.MasterTableView.GetColumn("DisplayName").Display = true;
                    SharedDetailsGridAD.MasterTableView.GetColumn("Description").Display = true;
                    SharedDetailsGridAD.DataBind();
                    SharedAccountDetailPanel.Visible = listadhocadshared.Count > 0 ? true : false;
                }
                else
                {
                    SharedAccountDetailPanel.Visible = false;
                }

            }
            #endregion

            #region GEMS
            else if (adhocaccessrequest.ApplicationType == ApplicationType.GEMS)
            {
                try
                {
                    //this.GEMSGrid.Visible = true;
                    //UserControlVisibility(GEMSDetailPanel.ID);
                    this.RadGridPopup.Visible = true;
                    AdHocGEMSRequest adhocGEMSSDetails = (AdHocGEMSRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation gtssusr = getAccountReconcilation(adhocGEMSSDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = gtssusr.AccountName;
                        trUsername.Visible = true;
                    }
                    UserControlVisibility(PanelofAD.ID);
                    str = adhocGEMSSDetails.Entitlement;

                    arr = str.Split(',');
                    IList<AdHocGEMSRequest> listadhocinhouse = new List<AdHocGEMSRequest>();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        AdHocGEMSRequest entitlement = new AdHocGEMSRequest();
                        entitlement.Entitlement = arr[i];
                        listadhocinhouse.Add(entitlement);
                    }
                    RadGridPopup.MasterTableView.GetColumn("GrouporRequestorText").HeaderText = "Groups";
                    RadGridPopup.DataSource = listadhocinhouse;
                    RadGridPopup.DataBind();


                    //List<AdHocGEMSRequest> dataSource = new List<AdHocGEMSRequest>();
                    //dataSource.Add(adhocGEMSSDetails);
                    //GEMSGrid.DataSource = dataSource;
                    //GEMSGrid.DataBind();
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                    {
                        IList<AdHocGEMSSharedAccount> listadhocadshared = new List<AdHocGEMSSharedAccount>();
                        if (adhocGEMSSDetails.AdHocGEMSSharedAccount != null)
                        {
                            listadhocadshared.Add(adhocGEMSSDetails.AdHocGEMSSharedAccount);
                        }
                        this.SharedDetailsGridAD.Visible = true;
                        SharedDetailsGridAD.DataSource = listadhocadshared.Count > 0 ? listadhocadshared : null;
                        SharedDetailsGridAD.MasterTableView.GetColumn("EmailAddress").Display = true;
                        SharedDetailsGridAD.MasterTableView.GetColumn("DisplayName").Display = true;
                        SharedDetailsGridAD.MasterTableView.GetColumn("Description").Display = true;
                        SharedDetailsGridAD.DataBind();
                        SharedAccountDetailPanel.Visible = listadhocadshared.Count > 0 ? true : false;
                    }
                    else
                    {
                        SharedAccountDetailPanel.Visible = false;
                    }

                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region O365
            else if (adhocaccessrequest.ApplicationType == ApplicationType.Office365)
            {
                try
                {
                    this.RadGridPopup.Visible = true;
                    AdHocO365Request adhocGEMSSDetails = (AdHocO365Request)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation o365User = getAccountReconcilation(adhocGEMSSDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = o365User.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }
                    UserControlVisibility(O365Panel.ID);

                    AdHocO365Request entilement = (AdHocO365Request)UtilityMethods.DataDeserialize(adhocaccessrequest.EntitlementDetail, typeof(Dto.AdHocO365Request));
                    if (adhocaccessrequest.AccountType == AccountType.Shared && adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        O365sharedNewAccPanel.Visible = true;
                        O365sharedAmmendmentPanel.Visible = false;
                        O365UsrAccAmendment.Visible = false;
                        IList<AdHocO365SharedAccount> listadhocadshared = new List<AdHocO365SharedAccount>();
                        listadhocadshared.Add(entilement.AdHocO365SharedAccount);
                        SharedNewAccGrid.DataSource = listadhocadshared;
                        SharedNewAccGrid.DataBind();
                    }
                    if (adhocaccessrequest.AccountType == AccountType.Shared && adhocaccessrequest.AdhocActionType != AdHocActionType.NewAccount)
                    {
                        O365sharedNewAccPanel.Visible = false;
                        O365sharedAmmendmentPanel.Visible = true;
                        O365UsrAccAmendment.Visible = false;
                        IList<AdHocO365Request> entilementlist = new List<AdHocO365Request>();
                        entilementlist.Add(entilement);
                        SharedAmendmentgrid.DataSource = entilementlist;
                        SharedAmendmentgrid.DataBind();
                    }

                    if (adhocaccessrequest.AccountType == AccountType.User)
                    {
                        O365sharedNewAccPanel.Visible = false;
                        O365sharedAmmendmentPanel.Visible = false;
                        O365UsrAccAmendment.Visible = true;
                        IList<AdHocO365Request> entilementlist = new List<AdHocO365Request>();
                        entilementlist.Add(entilement);
                        UsrAccAmendmntGrid.DataSource = entilementlist;
                        UsrAccAmendmntGrid.DataBind();

                    }
                    if (entilement.Members != null && entilement.Members.Count > 0)
                    {
                        MembersPanel.Visible = true;
                        gridMembers.Visible = true;
                        gridMembers.DataSource = entilement.Members;
                        gridMembers.DataBind();
                    }
                    else
                    {
                        MembersPanel.Visible = false;
                    }
                    if (entilement.LicenseInfo != null && entilement.LicenseInfo.Count > 0)
                    {
                        LicensePanel.Visible = true;
                        gridLicense.Visible = true;
                        gridLicense.DataSource = entilement.LicenseInfo;
                        gridLicense.DataBind();
                    }
                    else
                    {
                        LicensePanel.Visible = false;
                    }

                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region NPAC
            else if (adhocaccessrequest.ApplicationType == ApplicationType.NPAC)
            {
                try
                {
                    this.NPACGrid.Visible = true;
                    UserControlVisibility(NPACDetailPanel.ID);
                    AdHocNPACRequest adhocNPACDetails = (AdHocNPACRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation npacusr = getAccountReconcilation(adhocNPACDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = npacusr.AccountName;
                        trUsername.Visible = true;
                    }

                    if (!string.IsNullOrWhiteSpace(adhocNPACDetails.Section))
                    {
                        var parameters = new Dictionary<string, string>();
                        parameters.Add("sectionCode", adhocNPACDetails.Section);
                        var npacSection = DomainApiClient<Dto.NPACSectionInfo>.GetByAction<Dto.NPACSectionInfo>(DomainApiAction.Matrix.NPAC.GetNPACSectionByCode, parameters);
                        if (npacSection != null && !string.IsNullOrWhiteSpace(npacSection.HRSectionName))
                        {
                            adhocNPACDetails.Section = $"{adhocNPACDetails.Section} - {npacSection.HRSectionName}";
                        }
                    }
                    List<AdHocNPACRequest> dataSource = new List<AdHocNPACRequest>();
                    dataSource.Add(adhocNPACDetails);
                    NPACGrid.DataSource = dataSource;
                    NPACGrid.DataBind();

                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region SAPEP
            else if (adhocaccessrequest.ApplicationType == ApplicationType.SAP_EP || adhocaccessrequest.ApplicationType == ApplicationType.CE)
            {
                try
                {
                    this.SAPEPGrid.Visible = true;
                    UserControlVisibility(SAPEPPanel.ID);
                    AdhocSAPEPRequest adhocSAPEPDetails = (AdhocSAPEPRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation sapcusr = getAccountReconcilation(adhocSAPEPDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = sapcusr.AccountName;
                        trUsername.Visible = true;
                    }
                    List<AdhocSAPEPRequest> dataSource = new List<AdhocSAPEPRequest>();
                    dataSource.Add(adhocSAPEPDetails);
                    SAPEPGrid.DataSource = dataSource;
                    SAPEPGrid.DataBind();
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                    {
                        IList<AdHocSAPEPSharedAccount> listsapepahocadshared = new List<AdHocSAPEPSharedAccount>();
                        if (adhocSAPEPDetails.AdHocSAPEPSharedAccount != null)
                        {
                            listsapepahocadshared.Add(adhocSAPEPDetails.AdHocSAPEPSharedAccount);
                        }
                        this.SAPEPSharedPanel.Visible = true;
                        SAPEPSharedGrid.DataSource = listsapepahocadshared.Count > 0 ? listsapepahocadshared : null;
                        SAPEPSharedGrid.DataBind();
                        SAPEPSharedGrid.Visible = listsapepahocadshared.Count > 0 ? true : false;
                    }
                    else
                    {
                        SAPEPSharedPanel.Visible = false;
                    }

                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region MSSL
            else if (adhocaccessrequest.ApplicationType == ApplicationType.MSSL_ISU || adhocaccessrequest.ApplicationType == ApplicationType.MSSL_CRM)
            {
                try
                {
                    this.SAPMSSLGrid.Visible = true;
                    AdHocMSSLRequest adHocMSSLCDetails = (AdHocMSSLRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation sapusr = getAccountReconcilation(adHocMSSLCDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = sapusr.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }
                    UserControlVisibility(PanelofMSSL.ID);
                    List<AdHocMSSLRequest> dataSource = new List<AdHocMSSLRequest>();
                    dataSource.Add(adHocMSSLCDetails);
                    SAPMSSLGrid.DataSource = dataSource;
                    SAPMSSLGrid.DataBind();
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                    {
                        IList<AdHocSAPSharedAccount> listsaphocadshared = new List<AdHocSAPSharedAccount>();
                        if (adHocMSSLCDetails.AdHocSAPMSSLSharedAccount != null)
                        {
                            listsaphocadshared.Add(adHocMSSLCDetails.AdHocSAPMSSLSharedAccount);
                        }
                        this.SharedAccountMSSLPanel.Visible = true;
                        SharedMSSLGrid.DataSource = listsaphocadshared.Count > 0 ? listsaphocadshared : null;
                        SharedMSSLGrid.DataBind();
                        SharedMSSLGrid.Visible = listsaphocadshared.Count > 0 ? true : false;
                    }
                    else
                    {
                        this.SharedAccountMSSLPanel.Visible = false;
                        SharedHRMSPanel.Visible = false;
                    }

                    if (adHocMSSLCDetails != null && adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocMSSLCDetails.IsReallocate)
                    {
                        divReAllocate.Visible = true;
                        lblIsReAllocate.Text = adHocMSSLCDetails.IsReallocate.GetBoolToString();
                    }
                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region EBS
            else if (adhocaccessrequest.ApplicationType == ApplicationType.EBS_CRM || adhocaccessrequest.ApplicationType == ApplicationType.EBS_ISU)
            {
                try
                {
                    this.SAPMSSLGrid.Visible = true;
                    AdHocEBSRequest adHocEBSDetails = (AdHocEBSRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation sapusr = getAccountReconcilation(adHocEBSDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = sapusr.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }
                    UserControlVisibility(PanelofMSSL.ID);
                    List<AdHocEBSRequest> dataSource = new List<AdHocEBSRequest>();
                    dataSource.Add(adHocEBSDetails);
                    SAPMSSLGrid.DataSource = dataSource;
                    SAPMSSLGrid.DataBind();
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                    {
                        IList<AdHocSAPSharedAccount> listsaphocadshared = new List<AdHocSAPSharedAccount>();
                        if (adHocEBSDetails.AdHocSAPEBSSharedAccount != null)
                        {
                            listsaphocadshared.Add(adHocEBSDetails.AdHocSAPEBSSharedAccount);
                        }
                        this.SharedAccountMSSLPanel.Visible = true;
                        SharedMSSLGrid.DataSource = listsaphocadshared.Count > 0 ? listsaphocadshared : null;
                        SharedMSSLGrid.DataBind();
                        SharedMSSLGrid.Visible = listsaphocadshared.Count > 0 ? true : false;
                    }
                    else
                    {
                        this.SharedAccountMSSLPanel.Visible = false;
                        SharedHRMSPanel.Visible = false;
                    }
                    if (adHocEBSDetails != null && adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocEBSDetails.IsReallocate)
                    {
                        divReAllocate.Visible = true;
                        lblIsReAllocate.Text = adHocEBSDetails.IsReallocate.GetBoolToString();
                    }

                }
                catch (Exception)
                {
                }
            }
            #endregion

            #region SMS
            else if (adhocaccessrequest.ApplicationType == ApplicationType.OneCard)
            {
                try
                {
                    this.RadGridPopup.Visible = true;
                    AdHocSMSRequest adhocSMSDetails = (AdHocSMSRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation smsUser = getAccountReconcilation(adhocSMSDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = smsUser.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }
                    UserControlVisibility(SMSPanel.ID);

                    // AdHocSMSRequest entilement = (AdHocSMSRequest)UtilityMethods.Deserialize<AdHocSMSRequest>(;
                    SMSGrid.DataSource = adhocSMSDetails.GridInfo;
                    SMSGrid.DataBind();


                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region SRM
            else if (adhocaccessrequest.ApplicationType == ApplicationType.SRM)
            {
                try
                {
                    this.RadGridPopup.Visible = true;
                    AdHocSAPSRMRequest adhocSMSDetails = (AdHocSAPSRMRequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType != AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType != AdHocActionType.AmendmentRemove)
                    {

                        lblUserName.Text = adhocSMSDetails.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }
                    UserControlVisibility(SRMPanel.ID);

                    // AdHocSMSRequest entilement = (AdHocSMSRequest)UtilityMethods.Deserialize<AdHocSMSRequest>(;
                    SRMGrid.DataSource = adhocSMSDetails.AdHocSAPSRMRequestDetails;
                    SRMGrid.DataBind();


                }
                catch (Exception)
                {
                }
            }
            #endregion
            #region Skype for business
            else if (adhocaccessrequest.ApplicationType == ApplicationType.MicrosoftTeams)
            {
                try
                {
                    pnlResponseGroup.Visible = false;
                    pnlDCCExtension.Visible = false;
                    UserControlVisibility(pnlSkypeForBusiness.ID);
                    var adhocSFBDetails = (AdhocSkypeForBusinessRequest)ientitlement;
                    lblType.Text = adhocSFBDetails.EntitlementType.ToString();
                    switch (adhocaccessrequest.AdhocActionType)
                    {
                        case AdHocActionType.None:
                            break;
                        case AdHocActionType.NewAccount:
                            if (adhocSFBDetails.EntitlementType == EntitlementType.ResponseGroup)
                            {
                                ShowResponseGroup(adhocSFBDetails);
                            }
                            break;
                        case AdHocActionType.AmendmentAdd:
                            lblLineURI.Text = adhocSFBDetails.DisplayCurrentLineURI;
                            if (adhocSFBDetails.EntitlementType == EntitlementType.ResponseGroup)
                            {
                                ShowResponseGroup(adhocSFBDetails);
                            }
                            else if (adhocSFBDetails.EntitlementType == EntitlementType.Move)
                            {
                                if (adhocSFBDetails.MoveToOnline.HasValue && adhocSFBDetails.MoveToOnline.Value)
                                {
                                    lblType.Text = $"{adhocSFBDetails.EntitlementType.ToString()} To Online";
                                }
                                else
                                {
                                    lblType.Text = $"Move To {adhocSFBDetails.RegistrarPoolName}";
                                }
                            }
                            else if (adhocSFBDetails.EntitlementType == EntitlementType.Default)
                            {
                                lblType.Text = $"Reserved Number";
                            }
                            else if (adhocSFBDetails.EntitlementType == EntitlementType.DCC)
                            {

                                pnlDCCExtension.Visible = true;
                                lblDCCExtension.Text = adhocSFBDetails.Extension;
                                lblDCCExtension.Visible = true;
                            }

                            break;
                        case AdHocActionType.AmendmentRemove:
                            lblLineURI.Text = adhocSFBDetails.DisplayCurrentLineURI;
                            if (adhocSFBDetails.EntitlementType == EntitlementType.ResponseGroup)
                            {
                                ShowResponseGroup(adhocSFBDetails);
                            }
                            else
                            {
                                lblLineURI.Text = adhocSFBDetails.DisplayCurrentLineURI;
                            }
                            break;
                        case AdHocActionType.DeleteAccount:
                            lblLineURI.Text = adhocSFBDetails.DisplayCurrentLineURI;
                            lblType.Text = "Delete Account";
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            #endregion
            #region EBS
            else if (adhocaccessrequest.ApplicationType == ApplicationType.EBS || adhocaccessrequest.ApplicationType == ApplicationType.MSSL)
            {
                try
                {
                    this.SAPHANAGrid.Visible = true;
                    AdHocSAPHANARequest adHocEBSDetails = (AdHocSAPHANARequest)ientitlement;
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd || adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentRemove)
                    {
                        AccountReconcilation sapusr = getAccountReconcilation(adHocEBSDetails.EndpointUserId, adhocaccessrequest.EndpointSystemId.HasValue ? adhocaccessrequest.EndpointSystemId.Value : 0);

                        lblUserName.Text = sapusr.AccountName;
                        trUsername.Visible = true;
                    }
                    else
                    {
                        trUsername.Visible = false;
                    }
                    UserControlVisibility(PanelofSAPHANA.ID);
                    List<AdHocSAPHANARequest> dataSource = new List<AdHocSAPHANARequest>();
                    dataSource.Add(adHocEBSDetails);
                    SAPHANAGrid.DataSource = dataSource;
                    SAPHANAGrid.DataBind();
                    if (adhocaccessrequest.AdhocActionType == AdHocActionType.NewAccount && adhocaccessrequest.AccountType != AccountType.User)
                    {
                        IList<AdHocSAPSharedAccount> listsaphocadshared = new List<AdHocSAPSharedAccount>();
                        if (adHocEBSDetails.AdHocSAPHANASharedAccount != null)
                        {
                            listsaphocadshared.Add(adHocEBSDetails.AdHocSAPHANASharedAccount);
                        }
                        this.SharedAccountSAPHANAPanel.Visible = true;
                        SharedSAPHANAGrid.DataSource = listsaphocadshared.Count > 0 ? listsaphocadshared : null;
                        SharedSAPHANAGrid.DataBind();
                        SharedSAPHANAGrid.Visible = listsaphocadshared.Count > 0 ? true : false;
                    }
                    else
                    {
                        this.SharedAccountSAPHANAPanel.Visible = false;
                        SharedHRMSPanel.Visible = false;
                    }
                    if (adHocEBSDetails != null && adhocaccessrequest.AdhocActionType == AdHocActionType.AmendmentAdd && adHocEBSDetails.IsReallocate)
                    {
                        divSAPHANAReAllocate.Visible = true;
                        lblIsSAPHANAReAllocate.Text = adHocEBSDetails.IsReallocate.GetBoolToString();
                    }

                }
                catch (Exception)
                {
                }
            }
            #endregion
        }
        private void ShowResponseGroup(AdhocSkypeForBusinessRequest adhocSFBDetails)
        {
            pnlResponseGroup.Visible = true;
            ResponseGroupGrid.Visible = true;
            ResponseGroupGrid.DataSource = adhocSFBDetails.SkypeForBusinessResponseGroups;
            ResponseGroupGrid.DataBind();
        }
        protected void NPACGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            try
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem item = e.Item as GridDataItem;
                    Label label = (Label)item.FindControl("EntitlementLabel");
                    string entitlement = item.GetDataKeyValue("Entitlement").ToString();
                    string[] splitText = entitlement.Split(',');
                    string concadinateText = "";
                    foreach (string text in splitText)
                    {
                        concadinateText = concadinateText == "" ? "LEVEL " + text : concadinateText + ", LEVEL " + text;
                    }
                    label.Text = concadinateText;
                }
            }
            catch
            {
            }
        }

        private void UserControlVisibility(object Control)
        {
            PanelofAD.Visible = Control == PanelofAD.ID ? true : false;
            MoreDetailPanel.Visible = Control == MoreDetailPanel.ID ? true : false;
            SharedHRMSPanel.Visible = Control == SharedHRMSPanel.ID ? true : false;
            VPNDetailsPanel.Visible = Control == VPNDetailsPanel.ID ? true : false;
            GEMSDetailPanel.Visible = Control == GEMSDetailPanel.ID ? true : false;
            NPACDetailPanel.Visible = Control == NPACDetailPanel.ID ? true : false;
            PanelofMSSL.Visible = Control == PanelofMSSL.ID ? true : false;
            SAPEPPanel.Visible = Control == SAPEPPanel.ID ? true : false;
            O365Panel.Visible = Control == O365Panel.ID ? true : false;
            SMSPanel.Visible = Control == SMSPanel.ID ? true : false;
            SRMPanel.Visible = Control == SRMPanel.ID ? true : false;
            PanelofSSIDS.Visible = Control == PanelofSSIDS.ID ? true : false;
            pnlSkypeForBusiness.Visible = Control == pnlSkypeForBusiness.ID ? true : false;
            pnlINH.Visible = Control == pnlINH.ID;
            EAM_Panel.Visible = Control == EAM_Panel.ID ? true : false;
            PanelofSAPHANA.Visible = Control == PanelofSAPHANA.ID ? true : false;
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void RadGridPopup_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label label = (Label)item.FindControl("EntitlementLabelAD");
                if (!string.IsNullOrEmpty(RequesterText))
                {
                    label.Text = RequesterText;
                    RequesterText = "";
                }
            }
        }
        protected void HRMSGRID_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = e.Item as GridDataItem;
                Label VarianceLabel = (Label)item.FindControl("VarianceLabel");
                string CostCenter = Convert.ToString(item.GetDataKeyValue("CostCenter"));
                string PurchaseOrg = Convert.ToString(item.GetDataKeyValue("PurchaseOrg"));
                string PurchaseGroup = Convert.ToString(item.GetDataKeyValue("PurchaseGroup"));
                string Plant = Convert.ToString(item.GetDataKeyValue("Plant"));
                string Company = Convert.ToString(item.GetDataKeyValue(""));
                string StorageLocation = Convert.ToString(item.GetDataKeyValue("StorageLocation"));
                string MRPController = Convert.ToString(item.GetDataKeyValue("MRPController"));
                if (VarianceLabel != null)
                {
                    if (!string.IsNullOrEmpty(CostCenter))
                        VarianceLabel.Text = "Cost Center : " + CostCenter;
                    else if (!string.IsNullOrEmpty(MRPController))
                        VarianceLabel.Text = "MRP Controller : " + MRPController;
                    else if (!string.IsNullOrEmpty(Plant))
                        VarianceLabel.Text = "Plant : " + Plant;
                    else if (!string.IsNullOrEmpty(PurchaseGroup))
                        VarianceLabel.Text = "Purchase Group : " + PurchaseGroup;
                    else if (!string.IsNullOrEmpty(PurchaseOrg))
                        VarianceLabel.Text = "Purchase Org : " + PurchaseOrg;
                    else if (!string.IsNullOrEmpty(StorageLocation))
                        VarianceLabel.Text = "Storage Location : " + StorageLocation;
                }
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            if (Close != null)
            {
                Close(this, new EventArgs());
            }
        }

        protected void radGridINH_ItemDataBound(object sender, GridItemEventArgs e)
        {

        }
    }
}