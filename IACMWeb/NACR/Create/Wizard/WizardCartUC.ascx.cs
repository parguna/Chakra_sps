using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.Web.NACR.Create.Wizard
{
    public partial class WizardCartUC : UIBaseUC
    {
        public event EventHandler EditCartClicked;
        private string AllowOnce
        {
            get
            {
                if (Session["AllowOnce"] == null) return null;
                return Session["AllowOnce"].ToString();
            }
            set
            {
                Session["AllowOnce"] = value;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AllowOnce = null;
                AdhocAccessUC.Initialize();

                if (Request.QueryString["AdhocRequestId"] != null)
                {
                    long requestId = !string.IsNullOrEmpty(Request.QueryString["AdhocRequestId"]) ? Convert.ToInt32(Request.QueryString["AdhocRequestId"]) : 0;
                    if (requestId > 0)
                    {
                        Dto.AdHocAccessRequest request = DomainApiClient<Dto.AdHocAccessRequest>.GetById(requestId, true);
                        AdhocAccessUC.BindAdhocRequest(request);
                        RadWizardStep2.Active = true;
                        ApplicationBind(RadWizard, new Telerik.Web.UI.WizardEventArgs(RadWizardStep1, RadWizardStep2));
                    }
                }
                else
                {
                    AccessUC.AccessRequest = this.AdhocAccessUC.AdHocAccessRequest;
                    AccessUC.Initialize();
                }

                Session["isRequestTypeChange"] = false;
            }
            if(CartInfo != null)
            {
                CartInfo.EditCartItemClicked += GotoStep3_Clicked;
            }
        }

        protected void GotoStep3_Clicked(object sender, EventArgs e)
        {
            if (RadWizard.ActiveStep == RadWizardStep3) return;
            var step = new Telerik.Web.UI.WizardEventArgs(this.RadWizard.ActiveStep,this.RadWizardStep3);
            RadWizardStep3.Active = true;
            CartItemUC.Initialize();
        }

        protected void ApplicationBind(object sender, Telerik.Web.UI.WizardEventArgs e)
        {

            Session["endpointSystemId"] = AccessUC.AccessRequest?.EndpointSystemId;

            if (e.NextStepIndex == 1)
            {
                AllowOnce = null;
                if (e.CurrentStepIndex < e.NextStepIndex)
                {
                    AdhocAccessUC.Initialize();

                    if (!AdhocAccessUC.Validate())
                    {
                        RadWizardStep1.Active = true;
                        return;
                    }

                    AccessUC.AccessRequest = this.AdhocAccessUC.AdHocAccessRequest;

                    AccessUC.Initialize();
                }
                else if (!AdhocAccessUC.Validate())
                {
                    RadWizardStep1.Active = true;
                    return;
                }
                if(e.CurrentStepIndex > e.NextStepIndex && e.CurrentStepIndex == 2)
                {
                    AllowOnce = null;
                    AccessUC.InitializeControlOnly();
                }
            }
            else if (e.NextStepIndex == 0 && e.CurrentStepIndex == 1)
            {
                AllowOnce = null;
                AccessUC.PersistEntitlementDetail();
            }
            else if (e.NextStepIndex == 2)
            {
                if ((AccessUC.AccessRequest == null || AccessUC.AccessRequest?.EndpointSystemId == 0) && CartItemUC.CartItemCount == 0)
                    RadWizardStep1.Active = true;
                else
                {
                    if (AllowOnce == null)
                    {
                        var addedToCart = AccessUC.AddToCart();
                        AccessUC.InitializeControlOnly();

                        if (!addedToCart)
                        {
                            RadWizardStep2.Active = true;

                            return;
                        }
                        else if (TotalItems == 0)
                        {
                            ShowAlertMessage(AlertType.Error, "Please Add at least one item into the cart");
                            RadWizardStep2.Active = true;
                            return;
                        }
                        AllowOnce = "Added";
                    }
                    else
                    {
                        AllowOnce = null;
                    }
                    AccessUC.InitializeControlOnly();
                }
                CartItemUC.Initialize();
            }
        }

        protected void RefreshCart()
        {
            CartInfo.Refresh();
            RadWizardStep3.Active = true;
            CartItemUC.Initialize();
            ///ApplicationBind(RadWizard, new Telerik.Web.UI.WizardEventArgs(RadWizardStep2, RadWizardStep3));
        }


        private bool IsValidToProcess()
        {
            //if (TotalItems == 0)
            //{
            //    RadWizardStep2.Active = true;
            //    ShowAlertMessage(AlertType.Warning, "Please Add at least one item into the cart");
            //    return false;
            //}
            //else
            if (this.AdhocAccessUC.AdHocAccessRequest != null && this.AdhocAccessUC.AdHocAccessRequest.ValidFrom != null && this.AdhocAccessUC.AdHocAccessRequest.ValidTo != null)
            {
                if (this.AdhocAccessUC.AdHocAccessRequest.ValidTo < this.AdhocAccessUC.AdHocAccessRequest.ValidFrom)
                {
                    ShowAlertMessage(AlertType.Warning, "Valid To date should not be less than Valid From date");
                    return false;
                }
            }

            return true;
        }
        protected void RadWizard_FinishButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            if (this.CartItemUC.CartItemCount == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please add at least one item.");
                return;
            }
            bool redirectToList = true;
            var requestBuilder = AccessRequestBuilderFactory.Get(AccessRequestBuilderType.Default);


            if (IsValidToProcess() == false)
                return;

            //bool removeAccess = !(this.AdhocAccessUC.AdHocAccessRequest.IsAddAccess);
            //if (CartItemUC.complianceReport.Count > 0 || this.AdhocAccessUC.AdHocAccessRequest.IsAddAccess)
            //{
            //    bool canAllowRequest = true;

            //    List<SODPolicyConfiguration> policyConfiguration = DomainApiClient<SODPolicyConfiguration>.GetAll().ToList();
            //    foreach (UserSODComplianceReport report in CartItemUC.complianceReport)
            //    {
            //        foreach (SODPolicyConfiguration config in policyConfiguration)
            //        {
            //            bool isScoresatisfied = true;
            //            Infrastructure.Enums.Operator operate = (Operator)Enum.Parse(typeof(Operator), config.Operator);
            //            Infrastructure.Enums.RiskScoreOperator riskoper = (RiskScoreOperator)Enum.Parse(typeof(RiskScoreOperator), config.RiskScoreOperator);
            //            SODRiskLevel riskLevel = DomainApiClient<SODRiskLevel>.GetById(config.RiskLevelId);
            //            if (Infrastructure.Enums.RiskScoreOperator.Greaterthan.Equals(riskoper))
            //            {
            //                isScoresatisfied = report.RiskScore > config.RiskScore;
            //            }
            //            else if (Infrastructure.Enums.RiskScoreOperator.Lessthan.Equals(riskoper))
            //            {
            //                isScoresatisfied = report.RiskScore < config.RiskScore;
            //            }
            //            else if (Infrastructure.Enums.RiskScoreOperator.Equalto.Equals(riskoper))
            //            {
            //                isScoresatisfied = report.RiskScore == config.RiskScore;
            //            }

            //            if (Infrastructure.Enums.Operator.AND.Equals(operate) && (!removeAccess))
            //            {
            //                if (riskLevel.RiskLevelCode.ToString().Equals(report.RiskLevel) && isScoresatisfied)
            //                {
            //                    canAllowRequest = false;
            //                    break;
            //                }
            //            }
            //            if (Infrastructure.Enums.Operator.OR.Equals(operate) && (!removeAccess))
            //            {
            //                if (config.RiskLevelId.ToString().Equals(report.RiskLevel) || isScoresatisfied)
            //                {
            //                    canAllowRequest = false;
            //                    break;
            //                }
            //            }
            //        }
            //        if (!canAllowRequest)
            //            break;
            //    }
            //    if (canAllowRequest && CartItemUC.complianceReport.Count > 0)
            //    {
            //        List<UserSODComplianceReport> report = CartItemUC.complianceReport;
            //        report.All(x => { x.UserId = this.AdhocAccessUC.AdHocAccessRequest.RequesteeUserId; x.isFromAccessRequest = true; return true; });
            //        List<UserSODComplianceReport> dbreport = DomainApiClient<UserSODComplianceReport>.GetAll();
            //        List<UserSODComplianceReport> reportTosave = report.ToList();
            //        foreach (UserSODComplianceReport rep in dbreport)
            //        {
            //            foreach (UserSODComplianceReport repo in report)
            //            {
            //                if (repo.CompliancePolicyType == rep.CompliancePolicyType)
            //                {
            //                    if (rep.Endpoint == repo.Endpoint && rep.ConflictEntitlement1 == repo.ConflictEntitlement1 && rep.ConflictEntitlement2 == repo.ConflictEntitlement2 && rep.UserId == repo.UserId && rep.isFromAccessRequest == repo.isFromAccessRequest)
            //                        reportTosave.Remove(repo);
            //                }
            //            }
            //        }
            //        DomainApiClient<Dto.UserSODComplianceReport>.SaveList(reportTosave);
            //    }

            //    if (canAllowRequest)
            //    {
            //        requestBuilder.CreateAccessRequests();
            //    }
            //    else
            //    {
            //        redirectToList = false;
            //        MasterPage.ShowAlert(AlertType.Error, "Current Request has Conflicts.");
            //    }
            //}
            //else
            var intrimStore = this.CartItemUC.GetSelectedItems();
            if (intrimStore.Count == 0)
            {
                ShowAlertMessage(AlertType.Warning, "Please add at least one item.");
                return;
            }
            bool checkedstatus = this.CartItemUC.checkBox2CheckedStatus();
            requestBuilder.AllowedItem = intrimStore;

            requestBuilder.IsCheckboxChecked = checkedstatus;
            string s = requestBuilder.CreateAccessRequests();
            //ShowAlertMessage(AlertType.Success, @"ADhoc Access Request Created with Id(s) - {s}");
            if (checkedstatus)
            {
                if (this.CartItemUC.IsCartItemStillExists())
                {
                    RefreshCart();
                }
                else
                {
                    Response.Redirect("~/Request/List.aspx");
                }
            }
            else
            {
                RefreshCart();
            }

        }

        protected void RadWizard_CancelButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            Response.Redirect("~/Home/Home.aspx");
        }
        public int TotalItems
        {
            get
            {
                return (int)CartInfo.TotalItems;
            }
        }

        bool IsUserHasSODConflictForCurrentRequest(List<UserSODComplianceReport> userSODComplianceReport, string requestedRole, out int riskScore, out string conflictPair, out string sensitiveRole)
        {
            riskScore = 0; conflictPair = ""; sensitiveRole = "";
            if (userSODComplianceReport != null && userSODComplianceReport.Count > 0)
            {
                var newConflicts = userSODComplianceReport.Where(a => a.Type == "New" && (a.ConflictEntitlement1 == requestedRole || a.ConflictEntitlement2 == requestedRole));
                if (newConflicts != null && newConflicts.Count() > 0)
                {
                    foreach (UserSODComplianceReport conflict in newConflicts)
                    {
                        riskScore = Math.Max(conflict.RiskScore, riskScore);
                        if (conflict.IsSensitiveRole)
                            sensitiveRole += string.Format("{0}, ", conflict.ConflictEntitlement1);
                        else
                            conflictPair += string.Format("{0}-{1}, ", conflict.ConflictEntitlement1, conflict.ConflictEntitlement2);
                    }
                    conflictPair = conflictPair.Trim().TrimEnd(',');
                    sensitiveRole = sensitiveRole.Trim().TrimEnd(',');
                    return true;
                }
            }
            return false;
        }
        
        protected void RadWizard_NavigationBarButtonClick(object sender, Telerik.Web.UI.WizardEventArgs e)
        {
            Session["endpointSystemId"] = AccessUC.AccessRequest?.EndpointSystemId;

            if (e.NextStepIndex == 1)
            {
                AllowOnce = null;
                if (e.CurrentStepIndex < e.NextStepIndex)
                {
                    AdhocAccessUC.Initialize();
                    if (!AdhocAccessUC.Validate())
                    {
                        RadWizardStep1.Active = true;
                        return;
                    }
                    AccessUC.AccessRequest = this.AdhocAccessUC.AdHocAccessRequest;
                    AccessUC.Initialize();
                }
                else if (!AdhocAccessUC.Validate())
                {
                    RadWizardStep1.Active = true;
                    return;
                }
                if (e.CurrentStepIndex > e.NextStepIndex && e.CurrentStepIndex == 2)
                {
                    AllowOnce = null;
                    AccessUC.InitializeControlOnly();
                }
            }
            else if (e.NextStepIndex == 0 && e.CurrentStepIndex == 1)
            {
                AllowOnce = null;
                AccessUC.PersistEntitlementDetail();
            }
            else if (e.NextStepIndex == 2 && e.CurrentStepIndex == 1)
            {
                AllowOnce = null;
            }
            else if (e.NextStepIndex == 2)
            {
                AllowOnce = null;
                CartItemUC.Initialize();
            }
        }
    }
}