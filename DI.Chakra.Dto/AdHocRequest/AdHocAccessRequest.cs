using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    [Serializable]

    public class AdHocAccessRequest : DtoBase
    {
        public AdHocAccessRequest()
        {
            AccountType = AccountType.None; 
        }
        public long RequesterUserId { get; set; }

        public long RequesteeUserId { get; set; }

        public EndpointType EndpointSystemType { get; set; }

        public long? EndpointSystemId { get; set; }

        public AdHocActionType AdhocActionType { get; set; }

        public AccountType AccountType { get; set; }

        public bool IsSelfRequest { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string Comments { get; set; }
        public string EntitlementDetail { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string SecurityQuestions { get; set; }
        public int? ActualSupervisor { get; set; }
        public int? SelectedSupervisor { get; set; }
        public bool SupervisorChanged { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public EndPoint EndPoint { get; set; }
        public bool IsAddAccess { get; set; }        
        public string SharedAccountName { get; set; }
        public string DisplayName { get; set; }
        public bool Is…xpiryMailSend { get; set; }

        public bool Is…xpiryHandled { get; set; }

        public long InterimStoreId { get; set; }

        public string AccountTypeString
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>((AccountType)this.AccountType);
            }
        }

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
                if (this.EndpointSystemId.HasValue && this.EndPoint!=null)
                {
                    return this.EndPoint.DisplayName;
                }
                else if (this.EndpointSystemType == EndpointType.SRM)
                {
                    return "SRP";
                }
                return string.Empty;
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
                    case ApplicationType.SAPLocal:
                        return UtilityMethods.Deserialize<AdHocSAPLocalRequest>(this.EntitlementDetail)?.Entitlement;
                    case ApplicationType.ADSingaporePowerLocal:
                        return UtilityMethods.Deserialize<AdHocADRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.Office365:
                        return (UtilityMethods.DataDeserialize(this.EntitlementDetail, typeof(AdHocO365Request)) as AdHocO365Request)?.Entitlement;
                        //return UtilityMethods.Deserialize<AdHocO365Request>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.HRMS_FLINS:
                        return UtilityMethods.Deserialize<AdHocSAPHRFLINSRequest>(this.EntitlementDetail)?.Entitlement;
                        //return JoinedEntitlementAdHocSAPHRFLINSRequestDetail(adhocReq.AdHocSAPHRFLINSRequestDetails);

                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:
                        return UtilityMethods.Deserialize<AdHocMSSLRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        return UtilityMethods.Deserialize<AdHocEBSRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.SAP_PI:
                        return UtilityMethods.Deserialize<AdHocSAPPIRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.SAP_GRC:
                        return UtilityMethods.Deserialize<AdHocSAPGRCRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.FLINS_BI:
                        return UtilityMethods.Deserialize<AdHocSAPFLINSBIRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.VPN:
                        return UtilityMethods.Deserialize<AdHocVPNRequest>(this.EntitlementDetail)?.Entitlement;
                        //return JoinedEntitlementAdHocVPNRequestDetail(adhocReqVPN.AdHocVPNRequestDetails);

                    case ApplicationType.InhouseSystems:
                        return UtilityMethods.Deserialize<AdHocINHRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.NPAC:
                        return UtilityMethods.Deserialize<AdHocNPACRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.GEMS:
                        return UtilityMethods.Deserialize<AdHocGEMSRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        return UtilityMethods.Deserialize<AdhocSAPEPRequest>(this.EntitlementDetail)?.Entitlement;

                    case ApplicationType.SSIDS_SMS:
                    case ApplicationType.SSIDS_GUI: 
                            return UtilityMethods.Deserialize<AdHocSSIDSRequest>(this.EntitlementDetail)?.Entitlement; 
                    case ApplicationType.SRM:
                    case ApplicationType.SAP_SUS:
                    case ApplicationType.SAP_SRP:
                        return UtilityMethods.Deserialize<AdHocSAPSRMRequest>(this.EntitlementDetail)?.Entitlement;
                    case ApplicationType.IDMS:
                        return UtilityMethods.Deserialize<AdHocIACMRequest>(this.EntitlementDetail)?.Entitlement;
                    //return JoinedEntitlementAdHocSAPSRMRequestDetail(srmReq.AdHocSAPSRMRequestDetails);
                    case ApplicationType.MicrosoftTeams:
                        return UtilityMethods.Deserialize<AdhocSkypeForBusinessRequest>(this.EntitlementDetail)?.Entitlement;
                    case ApplicationType.EAM:
                        return UtilityMethods.Deserialize<AdHocNovaRequest>(this.EntitlementDetail)?.Entitlement;
                    case ApplicationType.OneCard:
                        return UtilityMethods.Deserialize<AdHocSMSRequest>(this.EntitlementDetail)?.Entitlement;
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        return UtilityMethods.Deserialize<AdHocSAPHANARequest>(this.EntitlementDetail)?.Entitlement;
                    default:
                        return string.Empty;
                }

            }
            catch(Exception ex) { return ""; }

        }

        //private string JoinedEntitlementAdHocSAPSRMRequestDetail(List<AdHocSAPSRMRequestDetail> AdhocReqDetails)
        //{
        //    string strItems = "";
        //    foreach (var item in AdhocReqDetails)
        //    {
        //        if (string.IsNullOrEmpty(item.PurchaseOrg))
        //        {
        //            strItems += (strItems == "") ? item.Entitlement : "," + item.Entitlement;
        //        }
        //        else
        //        {
        //            strItems += (strItems == "") ? string.Format("{0} ({1})", item.Entitlement, item.PurchaseOrg) : "," + string.Format("{0} ({1})", item.Entitlement, item.PurchaseOrg);
        //        }
        //    }
        //    return strItems;
        //}


        //private string JoinedEntitlementAdHocSAPHRFLINSRequestDetail(List<AdHocSAPHRFLINSRequestDetail> AdhocReqDetails)
        //{
        //    string strItems = "";
        //    foreach (var item in AdhocReqDetails)
        //    {
        //        if (string.IsNullOrEmpty(item.Company))
        //        {
        //            strItems += (strItems == "") ? item.Entitlement : "," + item.Entitlement;
        //        }
        //        else
        //        {
        //            strItems += (strItems == "") ? string.Format("{0} ({1})", item.Entitlement, item.Company) : "," + string.Format("{0} ({1})", item.Entitlement, item.Company);
        //        }
        //    }
        //    return strItems;
        //}

        //private string JoinedEntitlementAdHocVPNRequestDetail(List<AdHocVPNRequestDetail> AdhocReqDetails)
        //{
        //    string strItems = "";
        //    foreach (var items in AdhocReqDetails)
        //    {
        //        strItems += (strItems == "") ? items.ApplicationName : "," + items.ApplicationName;
        //    }
        //    return strItems;
        //}

        public IEntitlementDetail GetEntitlementDetail()
        {
            if (!string.IsNullOrEmpty(this.EntitlementDetail))
            {
                switch (this.ApplicationType)
                {
                    case ApplicationType.ADSingaporePowerLocal:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocADRequest>(this.EntitlementDetail);


                    case ApplicationType.Office365:
                        return (IEntitlementDetail)UtilityMethods.DataDeserialize(this.EntitlementDetail, typeof(Dto.AdHocO365Request));

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
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocINHRequest>(this.EntitlementDetail);

                    case ApplicationType.NPAC:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocNPACRequest>(this.EntitlementDetail);

                    case ApplicationType.GEMS:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocGEMSRequest>(this.EntitlementDetail);

                    case ApplicationType.OneCard:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSMSRequest>(this.EntitlementDetail);

                    case ApplicationType.SRM:
                    case ApplicationType.SAP_SRP:
                    case ApplicationType.SAP_SUS:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPSRMRequest>(this.EntitlementDetail);

                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdhocSAPEPRequest>(this.EntitlementDetail);
                    case ApplicationType.SAPLocal:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPLocalRequest>(this.EntitlementDetail);
                    case ApplicationType.SSIDS_GUI:
                    case ApplicationType.SSIDS_SMS:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSSIDSRequest>(this.EntitlementDetail);
                    case ApplicationType.IDMS:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocIACMRequest>(this.EntitlementDetail);
                    case ApplicationType.MicrosoftTeams:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdhocSkypeForBusinessRequest>(this.EntitlementDetail);
                    case ApplicationType.EAM:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocNovaRequest>(this.EntitlementDetail);
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

                    case ApplicationType.OneCard:
                        return UtilityMethods.Serialize<AdHocSMSRequest>((AdHocSMSRequest)value);

                    case ApplicationType.SRM:
                    case ApplicationType.SAP_SRP:
                    case ApplicationType.SAP_SUS:
                        return UtilityMethods.Serialize<AdHocSAPSRMRequest>((AdHocSAPSRMRequest)value);

                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        return UtilityMethods.Serialize<AdhocSAPEPRequest>((AdhocSAPEPRequest)value);
                    case ApplicationType.SAPLocal:
                        return UtilityMethods.Serialize<AdHocSAPLocalRequest>((AdHocSAPLocalRequest)value);
                    case ApplicationType.SSIDS_GUI:
                    case ApplicationType.SSIDS_SMS:
                        return UtilityMethods.Serialize<AdHocSSIDSRequest>((AdHocSSIDSRequest)value);
                    case ApplicationType.IDMS:
                        return UtilityMethods.Serialize<AdHocIACMRequest>((AdHocIACMRequest)value);
                    case ApplicationType.MicrosoftTeams:
                        return UtilityMethods.Serialize<AdhocSkypeForBusinessRequest>((AdhocSkypeForBusinessRequest)value);
                    case ApplicationType.EAM:
                        return UtilityMethods.Serialize<AdHocNovaRequest>((AdHocNovaRequest)value);
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

        public List<PresentationAccessRequestItem> RequestedItems
        {
            get
            {
                if (string.IsNullOrEmpty(EntitlementDetail))
                    return new List<PresentationAccessRequestItem>();

                return (List<PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<PresentationAccessRequestItem>(EntitlementDetail);
            }
        }
        public List<PresentationAccessRequestItem> SetRequestedItems
        {
            set
            {
                if (value != null)
                {
                    EntitlementDetail = UtilityMethods.SerializeCollection<PresentationAccessRequestItem>(value);
                }
                
            }
        }
        public List<PresentationAccessRequestItem> GetAccessRequestDetail()
        {
            if (string.IsNullOrEmpty(EntitlementDetail))
                return null;

            try
            {
                return (List<PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<PresentationAccessRequestItem>(EntitlementDetail);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public SecurityResponse SetSecurityQuestions
        {
            set
            {
                if (value != null)
                {
                    this.SecurityQuestions = UtilityMethods.Serialize<SecurityResponse>(value);
                }
            }
        }

        public SecurityResponse GetSecurityQuestions
        {
            get
            {
                if (!string.IsNullOrEmpty(this.SecurityQuestions))
                {
                    return UtilityMethods.Deserialize<SecurityResponse>(this.SecurityQuestions);
                }
                return null;
            }
        }
    }
}
