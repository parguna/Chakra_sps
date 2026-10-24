using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;

namespace TestHarness
{
    public partial class SAP_AccountCreation : Form
    {
        const string MasterConfigHelperServiceUser = "System";
        List<DI.Chakra.DomainModel.EndPoint> LstEndpoints = new List<DI.Chakra.DomainModel.EndPoint>();
        List<AccessRequest> accessRequests = new List<AccessRequest>();
        public SAP_AccountCreation()
        {
            InitializeComponent();
        }

        void InitializeControls()
        { 
            LstEndpoints = DI.Chakra.DomainModel.EndPoint.GetAll().Where(a => a.EndpointSystemTypeId == 3 || a.EndpointSystemTypeId == 13 ).ToList();

            cmbEndpoint.DataSource = LstEndpoints;
            cmbEndpoint.DisplayMember = "DisplayName";
            cmbEndpoint.ValueMember = "Id";

            cmbAccountType.DataSource = Enum.GetValues(typeof(DI.Chakra.Infrastructure.Enums.AccountType)) ;

        }

        private void SAP_AccountCreation_Load(object sender, EventArgs e)
        {
            InitializeControls();

        }

        private void cmbAccountType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbSAPUserType.DataSource = null;
            DI.Chakra.Infrastructure.Enums.AccountType accountType = (DI.Chakra.Infrastructure.Enums.AccountType) ((ComboBox)sender).SelectedItem ;

            if (accountType == DI.Chakra.Infrastructure.Enums.AccountType.Service)
            {
                cmbSAPUserType.DataSource = new DI.Chakra.Infrastructure.Enums.SAPUserType[] { DI.Chakra.Infrastructure.Enums.SAPUserType.Service, DI.Chakra.Infrastructure.Enums.SAPUserType.System };
            }
            else
            {
                cmbSAPUserType.DataSource = new DI.Chakra.Infrastructure.Enums.SAPUserType[] { DI.Chakra.Infrastructure.Enums.SAPUserType.Dialog  };
            }

        }

        void CreateAccessRequest(bool isDeleteAccount)
        {
            var accessRequest = new AccessRequest();
            var adhocrequest = CreateadHocAccessRequest(isDeleteAccount);

            accessRequest.AccountType = adhocrequest.AccountType;
            accessRequest.ActivationDate = DateTime.Now;
            accessRequest.DeactivationDate = adhocrequest.ValidTo;
            accessRequest.EndpointSystemId = adhocrequest.EndpointSystemId;
            accessRequest.ApplicationType = adhocrequest.ApplicationType;
            accessRequest.UserId = adhocrequest.RequesteeUserId;
            accessRequest.RequesterId = adhocrequest.RequesteeUserId;
            accessRequest.AdHocAccessRequest = adhocrequest;
            accessRequest.RequestSource = DI.Chakra.Infrastructure.Enums.AccessRequestSource.AdhocRequest;
            accessRequest.ProgressStatus = DI.Chakra.Infrastructure.Enums.AccessRequestProgress.ReadytoExecute;
            accessRequest.RequestStatus = DI.Chakra.Infrastructure.Enums.AccessRequestStatus.Approved;

           var longs =  AccessRequest.SaveAccessRequestList(new List<AccessRequest> { accessRequest });
            foreach (var l in longs)
                accessRequests.Add(AccessRequest.GetById(l));

            gvResult.DataSource = accessRequests;
        }


        AdHocAccessRequest CreateadHocAccessRequest(bool isDeleteAccount)
        {
            User usr = User.GetByEmployeeId(txtEmployeeId.Text);

            AdHocAccessRequest adHocAccessRequest = new AdHocAccessRequest();

            adHocAccessRequest.AccountType = (DI.Chakra.Infrastructure.Enums.AccountType)cmbAccountType.SelectedItem   ;


            adHocAccessRequest.AdhocActionType = isDeleteAccount ? DI.Chakra.Infrastructure.Enums.AdHocActionType.DeleteAccount : DI.Chakra.Infrastructure.Enums.AdHocActionType.NewAccount;


            adHocAccessRequest.EndPoint =  (EndPoint) cmbEndpoint.SelectedItem;

            adHocAccessRequest.ApplicationType = adHocAccessRequest.EndPoint.ApplicationType;

            adHocAccessRequest.EndpointSystemId = adHocAccessRequest.EndPoint.Id;

            adHocAccessRequest.RequesteeUserId = usr.Id;

            adHocAccessRequest.RequesterUserId = usr.Id;

            adHocAccessRequest.EndpointSystemType = DI.Chakra.Infrastructure.Enums.EndpointType.SAP;

            adHocAccessRequest.ValidFrom = DateTime.Now;

            adHocAccessRequest.ValidTo = isDeleteAccount ? DateTime.Now : (chkmaxDate.Checked ? DateTime.MaxValue : dtValidTo.Value);

            adHocAccessRequest.EntitlementDetail = adHocAccessRequest.GetSerializedString(CreateEntitlementDetail(adHocAccessRequest.ApplicationType, isDeleteAccount, adHocAccessRequest.EndPoint));

            return adHocAccessRequest;

        }


        IEntitlementDetail CreateEntitlementDetail(ApplicationType applicationType, bool isDelete, EndPoint endPoint)
        {
            IEntitlementDetail entitlementDetail = null;

            string accountname = txtAccountname.Text;

            string desc = txtDescription.Text;

            SAPUserType type = (SAPUserType) cmbSAPUserType.SelectedItem ;

            long endpointUserId = 0;

            if (isDelete)
            {
              var accountREcon =    AccountReconcilation.GetByAccountName(accountname, endPoint.Id);
                if (accountREcon != null)
                {
                    endpointUserId = accountREcon.EndpointUserId;
                }
            }

            switch (applicationType)
            {
                case ApplicationType.HRMS_FLINS:
                    {
                        return new AdHocSAPHRFLINSRequest()
                        {
                             AdHocSAPHRFLINSSharedAccount  = new AdHocSAPSharedAccount() { AccountName = accountname, Description = desc , SAPUserType = type }, DisplayAccountName = accountname, EndpointUserId = endpointUserId  
 
                        };
                    }

                case ApplicationType.FLINS_BI:
                    {
                        return new AdHocSAPFLINSBIRequest()
                        {
                             AdHocFLINSBISharedAccount = new AdHocSAPSharedAccount() { AccountName = accountname, Description = desc, SAPUserType = type },
                            DisplayAccountName = accountname,
                            EndpointUserId = endpointUserId, EntitlementType = EntitlementType.Role
                        };
                    }
                case ApplicationType.SAP_GRC:
                    {
                        return new AdHocSAPGRCRequest()
                        {
                            AdHocSAPGRCSharedAccount = new AdHocSAPSharedAccount() { AccountName = accountname, Description = desc, SAPUserType = type },
                            DisplayAccountName = accountname,
                            EndpointUserId = endpointUserId
                        };
                    }

                case ApplicationType.SRM:
                    {
                        return new AdHocSAPSRMRequest()
                        {
                            AdHocSAPSRMSharedAccount = new AdHocSAPSharedAccount() { AccountName = accountname, Description = desc, SAPUserType = type },
                            DisplayAccountName = accountname,
                            EndpointUserId = endpointUserId,
                            AdHocSAPSRMRequestDetails = new List<AdHocSAPSRMRequestDetail>() {
                                   { new AdHocSAPSRMRequestDetail() { Entitlement= "[Creators] Contract Owner", PurchaseGroup = "CD1-CD2-GS1-IPG", PurchaseOrg= "B,C,N" , EntitlementType = EntitlementType.Role, RetainMainRole = false } } }
                        };
                    }

                case ApplicationType.EBS_CRM:
                case ApplicationType.EBS_ISU:
                    {
                        return new AdHocEBSRequest()
                        {
                            AdHocSAPEBSSharedAccount = new AdHocSAPSharedAccount() { AccountName = accountname, Description = desc, SAPUserType = type },
                            DisplayAccountName = accountname,
                            EndpointUserId = endpointUserId,
                            Position = "ITD/DBM - DBM Manager(XISD-50000654)",
                            PositionValue = "50000654",
                            PositionPrefix = "XISD",
                            ApplicationName = applicationType == ApplicationType.EBS_ISU ? Constants.EBSISP : (applicationType == ApplicationType.EBS_CRM ? Constants.EBSCRP : Constants.EBSBI)
                        };
                    }

                case ApplicationType.MSSL_CRM:
                case ApplicationType.MSSL_ISU:
                    {
                        return new AdHocEBSRequest()
                        {
                            AdHocSAPEBSSharedAccount = new AdHocSAPSharedAccount() { AccountName = accountname, Description = desc, SAPUserType = type },
                            DisplayAccountName = accountname,
                            EndpointUserId = endpointUserId,
                            Position = "ITD/DC Operation - Assistant Director(XISD-50000141/50000252)",
                            PositionValue = applicationType == ApplicationType.MSSL_ISU?   "50000141" : "50002879",
                            PositionPrefix = "XISD",
                            ApplicationName = applicationType == ApplicationType.MSSL_ISU ? Constants.MSSLMSP : (applicationType == ApplicationType.MSSL_CRM ? Constants.MSSLMCP : Constants.MSSLBI)
                        };
                    }


            }


            return entitlementDetail;
        }




        private void btnCreate_Click(object sender, EventArgs e)
        {
            CreateAccessRequest(false);

        }

        private void btnReactivate_Click(object sender, EventArgs e)
        {
            CreateAccessRequest(false);
        }

        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            CreateAccessRequest(true);
        }
    } 

}
