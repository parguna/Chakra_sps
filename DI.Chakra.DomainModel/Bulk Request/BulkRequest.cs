using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public partial class BulkRequest : DomainModelBase<BulkRequest, EntityModel.BulkRequest>
    {

        public long RequestedBy { get; set; }
        public EndpointType EndpointType { get; set; }
        public long EndpointSystemId { get; set; }
        public string Description { get; set; }
        public AdHocActionType AdhocActionType { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string EntitlementDetail { get; set; }

        public bool IsEmployee { get; set; }

        public long? ActualSupervisor { get; set; }
        public long? SelectedSupervisor { get; set; }
        public bool SupervisorChanged { get; set; }

        public AccessRequestStatus RequestStatus { get; set; }
        public AccessRequestProgress ProgressStatus { get; set; }
        public AccessRequestSource RequestSource { get; set; }

        public Nullable<DateTime> ActivationDate { get; set; }
        public Nullable<DateTime> DeactivationDate { get; set; }
        public Nullable<DateTime> ExecutionTime { get; set; }

        public string Remarks { get; set; }
        public string AdditionalInfo { get; set; }
        public string Comments { get; set; }
        public bool IsCurrentLevelApproved { get; set; }
        public bool IsRequestedToRemove { get; set; }
        public string ApproverType { get; set; }

        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<DateTime> LastModifiedDate { get; set; }

        public EndPoint EndPoint { get; set; }

        public string ConflictsRemarks { get; set; }
        public List<BulkRequestDetail> BulkRequestDetails { get; set; }

        public string RequestTypeString
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>((AdHocActionType)this.AdhocActionType);
            }
        }

        public string AccessName
        {
            get
            {
                if (this.EndPoint != null) return this.EndPoint.Description;
                else return string.Empty;
            }
        }

        public bool HasMoreDetails
        {
            get
            {
                if (string.IsNullOrEmpty(this.EntitlementDetail))
                {
                    return false;
                }
                else
                {
                    if (this.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        return false;
                    }
                    if (this.ApplicationType == ApplicationType.VPN)
                    {
                        var adHocRequest = (AdHocVPNRequest)GetEntitlementDetail();
                        if (adHocRequest.AdHocVPNRequestDetails.Count == 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public string Entitlement
        {
            get
            {
                string Entitle = DeserializeEntitlement();
                if (!string.IsNullOrEmpty(Entitle))
                    return Entitle.Replace(",", ", ").Replace("  ", " ").Trim();
                else
                    return "";
            }
        }

        public string DeserializeEntitlement()
        {
            try
            {

                switch (this.ApplicationType)
                {
                    case ApplicationType.ADSingaporePowerLocal:
                        return UtilityMethods.Deserialize<AdHocADRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.Office365:
                        return string.Empty;

                    case ApplicationType.HRMS_FLINS:
                        AdHocSAPHRFLINSRequest adhocReq = UtilityMethods.Deserialize<AdHocSAPHRFLINSRequest>(this.EntitlementDetail);
                        return JoinedEntitlementAdHocSAPHRFLINSRequestDetail(adhocReq.AdHocSAPHRFLINSRequestDetails);

                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:

                        var msslDetail = UtilityMethods.Deserialize<AdHocMSSLRequest>(this.EntitlementDetail);
                        return string.IsNullOrEmpty(msslDetail.Position) ? msslDetail.Roles : string.IsNullOrEmpty(msslDetail.Roles) ? msslDetail.Position : msslDetail.Position + "," + msslDetail.Roles;


                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        var ebsDetail = UtilityMethods.Deserialize<AdHocEBSRequest>(this.EntitlementDetail);
                        return string.IsNullOrEmpty(ebsDetail.Position) ? ebsDetail.Roles : string.IsNullOrEmpty(ebsDetail.Roles) ? ebsDetail.Position : ebsDetail.Position + "," + ebsDetail.Roles;

                    case ApplicationType.SAP_PI:
                        return UtilityMethods.Deserialize<AdHocSAPPIRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.SAP_GRC:
                        return UtilityMethods.Deserialize<AdHocSAPGRCRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.FLINS_BI:
                        return UtilityMethods.Deserialize<AdHocSAPFLINSBIRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.VPN:
                        var adhocReqVPN = UtilityMethods.Deserialize<AdHocVPNRequest>(this.EntitlementDetail);
                        return JoinedEntitlementAdHocVPNRequestDetail(adhocReqVPN.AdHocVPNRequestDetails);

                    case ApplicationType.InhouseSystems:
                        return UtilityMethods.Deserialize<AdHocINHRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.NPAC:
                        return UtilityMethods.Deserialize<AdHocNPACRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.GEMS:
                        return UtilityMethods.Deserialize<AdHocGEMSRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        return UtilityMethods.Deserialize<AdhocSAPEPRequest>(this.EntitlementDetail).AdHocSAPEPSharedAccount.AccountName;

                    case ApplicationType.OneCard:
                        return UtilityMethods.Deserialize<AdHocSMSRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.SRM:
                        AdHocSAPSRMRequest srmReq = UtilityMethods.Deserialize<AdHocSAPSRMRequest>(this.EntitlementDetail);
                        return JoinedEntitlementAdHocSAPSRMRequestDetail(srmReq.AdHocSAPSRMRequestDetails);
                    case ApplicationType.MicrosoftTeams:
                        return UtilityMethods.Deserialize<AdhocSkypeForBusinessRequest>(this.EntitlementDetail)?.Entitlement;
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        return UtilityMethods.Deserialize<AdHocSAPHANARequest>(this.EntitlementDetail)?.Entitlement; 
                    default:
                        return string.Empty;
                }

            }
            catch { return ""; }

        }

        private string JoinedEntitlementAdHocSAPSRMRequestDetail(List<AdHocSAPSRMRequestDetail> AdhocReqDetails)
        {
            string strItems = "";
            foreach (var item in AdhocReqDetails)
            {
                if (string.IsNullOrEmpty(item.PurchaseOrg))
                {
                    strItems += (strItems == "") ? item.Entitlement : "," + item.Entitlement;
                }
                else
                {
                    strItems += (strItems == "") ? string.Format("{0} ({1})", item.Entitlement, item.PurchaseOrg) : "," + string.Format("{0} ({1})", item.Entitlement, item.PurchaseOrg);
                }
            }
            return strItems;
        }

        private string JoinedEntitlementAdHocSAPHRFLINSRequestDetail(List<AdHocSAPHRFLINSRequestDetail> AdhocReqDetails)
        {
            string strItems = "";
            foreach (var item in AdhocReqDetails)
            {
                if (string.IsNullOrEmpty(item.Company))
                {
                    strItems += (strItems == "") ? item.Entitlement : "," + item.Entitlement;
                }
                else
                {
                    strItems += (strItems == "") ? string.Format("{0} ({1})", item.Entitlement, item.Company) : "," + string.Format("{0} ({1})", item.Entitlement, item.Company);
                }
            }
            return strItems;
        }

        private string JoinedEntitlementAdHocVPNRequestDetail(List<AdHocVPNRequestDetail> AdhocReqDetails)
        {
            string strItems = "";
            foreach (var items in AdhocReqDetails)
            {
                strItems += (strItems == "") ? items.ApplicationName : "," + items.ApplicationName;
            }
            return strItems;
        }

        public IEntitlementDetail GetEntitlementDetail()
        {
            if (!string.IsNullOrEmpty(this.EntitlementDetail))
            {
                switch (this.ApplicationType)
                {
                    case ApplicationType.ADSingaporePowerLocal:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocADRequest>(this.EntitlementDetail);

                    case ApplicationType.Office365:
                        return (IEntitlementDetail)AutoMapper.Mapper.Map<DomainModel.AdHocO365Request>(UtilityMethods.DataDeserialize(this.EntitlementDetail, typeof(Dto.AdHocO365Request)));

                    case ApplicationType.HRMS_FLINS:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPHRFLINSRequest>(this.EntitlementDetail);

                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:
                        return UtilityMethods.Deserialize<AdHocMSSLRequest>(this.EntitlementDetail);

                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        return UtilityMethods.Deserialize<AdHocEBSRequest>(this.EntitlementDetail);

                    case ApplicationType.SAP_PI:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPPIRequest>(this.EntitlementDetail);

                    case ApplicationType.SAP_GRC:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPGRCRequest>(this.EntitlementDetail);

                    case ApplicationType.FLINS_BI:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPFLINSBIRequest>(this.EntitlementDetail);

                    case ApplicationType.VPN:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocVPNRequest>(this.EntitlementDetail);

                    case ApplicationType.InhouseSystems:
                        return UtilityMethods.Deserialize<AdHocINHRequest>(this.EntitlementDetail);

                    case ApplicationType.NPAC:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocNPACRequest>(this.EntitlementDetail);

                    case ApplicationType.GEMS:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocGEMSRequest>(this.EntitlementDetail);

                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdhocSAPEPRequest>(this.EntitlementDetail);

                    case ApplicationType.OneCard:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSMSRequest>(this.EntitlementDetail);

                    case ApplicationType.SRM:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPSRMRequest>(this.EntitlementDetail);
                    case ApplicationType.MicrosoftTeams:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdhocSkypeForBusinessRequest>(this.EntitlementDetail);
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPHANARequest>(this.EntitlementDetail);
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string GetSerializedString(IEntitlementDetail value)
        {
            if (value != null)
            {
                switch (this.ApplicationType)
                {
                    case ApplicationType.ADSingaporePowerLocal:
                        return UtilityMethods.Serialize<AdHocADRequest>((AdHocADRequest)value);

                    case ApplicationType.Office365:
                        return UtilityMethods.DataSerialize((AdHocO365Request)value);

                    case ApplicationType.HRMS_FLINS:
                        return UtilityMethods.Serialize<AdHocSAPHRFLINSRequest>((AdHocSAPHRFLINSRequest)value);

                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:
                        return UtilityMethods.Serialize<AdHocMSSLRequest>((AdHocMSSLRequest)value);

                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        return UtilityMethods.Serialize<AdHocEBSRequest>((AdHocEBSRequest)value);

                    case ApplicationType.SAP_PI:
                        return UtilityMethods.Serialize<AdHocSAPPIRequest>((AdHocSAPPIRequest)value);

                    case ApplicationType.SAP_GRC:
                        return UtilityMethods.Serialize<AdHocSAPGRCRequest>((AdHocSAPGRCRequest)value);

                    case ApplicationType.FLINS_BI:
                        return UtilityMethods.Serialize<AdHocSAPFLINSBIRequest>((AdHocSAPFLINSBIRequest)value);

                    case ApplicationType.VPN:
                        return UtilityMethods.Serialize<AdHocVPNRequest>((AdHocVPNRequest)value);

                    case ApplicationType.InhouseSystems:
                        return UtilityMethods.Serialize<AdHocINHRequest>((AdHocINHRequest)value);

                    case ApplicationType.NPAC:
                        return UtilityMethods.Serialize<AdHocNPACRequest>((AdHocNPACRequest)value);

                    case ApplicationType.GEMS:
                        return UtilityMethods.Serialize<AdHocGEMSRequest>((AdHocGEMSRequest)value);

                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        return UtilityMethods.Serialize<AdhocSAPEPRequest>((AdhocSAPEPRequest)value);

                    case ApplicationType.OneCard:
                        return UtilityMethods.Serialize<AdHocSMSRequest>((AdHocSMSRequest)value);

                    case ApplicationType.SRM:
                        return UtilityMethods.Serialize<AdHocSAPSRMRequest>((AdHocSAPSRMRequest)value);
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        return UtilityMethods.Serialize<AdHocSAPHANARequest>((AdHocSAPHANARequest)value);
                    default:
                        return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static BulkRequest GetRequestsToExecute()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMap(new BulkRequestRepository(transaction).GetRequestsToExecute());
            }
        }

        public static List<BulkRequest> GetSubmittedBulkRequests()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(false))
            {
                return ORMapList(new BulkRequestRepository(transaction).GetSubmittedBulkRequests());
            }
        }

        public override long Save(string currentUser)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                if (this.Id == 0)
                {
                    this.Id = new BulkRequestRepository(transaction).Insert(ORUnMap()); 
                }
                else
                {
                    new BulkRequestRepository(transaction).Update(ORUnMap());
                }

                transaction.Commit();
            }
            return 0;
        }

        public static List<BulkRequest> GetAllBulkRequestsToExecute()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new BulkRequestRepository(transaction).GetAllBulkRequestsToExecute());

            }

        }

        public static bool UpdateStatusWhenResolved(long bulkRequestId, string currentUser)
        {
            bool markAsCompleted = false;
            var bulkRequest = BulkRequest.GetById(bulkRequestId);
            var childRequests = AccessRequest.GetChildRequestByBulkRequestId(bulkRequestId);
            markAsCompleted = childRequests.All(x => x.ProgressStatus == AccessRequestProgress.Completed || x.ProgressStatus == AccessRequestProgress.ManuallyResolved);
            if (markAsCompleted)
            {
                bulkRequest.ProgressStatus = AccessRequestProgress.Completed;
                bulkRequest.LastModifiedBy = currentUser;
                bulkRequest.LastModifiedDate = DateTime.Now;
                bulkRequest.Save(currentUser);
            }
            return markAsCompleted;
        }

    }
}