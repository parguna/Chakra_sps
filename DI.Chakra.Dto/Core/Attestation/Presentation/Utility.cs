using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Collections.Generic;

namespace DI.Chakra.Dto
{
    public class Utility
    {

        static string JoinedEntitlementAdHocSAPHRFLINSRequestDetail(List<AdHocSAPHRFLINSRequestDetail> AdhocReqDetails)
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

        static string JoinedEntitlementAdHocSRMRequestDetail(List<AdHocSAPSRMRequestDetail> AdhocReqDetails)
        {
            string strItems = string.Empty;
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

        static string JoinedEntitlementAdHocVPNRequestDetail(List<AdHocVPNRequestDetail> AdhocReqDetails)
        {
            string strItems = string.Empty;
            foreach (var items in AdhocReqDetails)
            {
                strItems += (strItems == "") ? items.ApplicationName : "," + items.ApplicationName;
            }
            return strItems;
        }

        public static string DeserializeEntitlement(string Entitlement, ApplicationType applicationType)
        {
            try
            {
                if (string.IsNullOrEmpty(Entitlement))
                {
                    return string.Empty;
                }
                 
                string entitlementDetail = Entitlement;
                switch (applicationType)
                {
                    case ApplicationType.ADSingaporePowerLocal:
                        //case ApplicationType.ADSDCLocal:
                        return UtilityMethods.Deserialize<AdHocADRequest>(entitlementDetail).Entitlement;
                    case ApplicationType.SAPLocal:
                        return UtilityMethods.Deserialize<AdHocSAPLocalRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.Office365:
                        return string.Empty;

                    //case ApplicationType.LotusNotes:
                    //    return UtilityMethods.Deserialize<AdHocLNSRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.HRMS_FLINS:
                        AdHocSAPHRFLINSRequest adhocReq = UtilityMethods.Deserialize<AdHocSAPHRFLINSRequest>(entitlementDetail);
                        return JoinedEntitlementAdHocSAPHRFLINSRequestDetail(adhocReq.AdHocSAPHRFLINSRequestDetails);

                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:
                        if (entitlementDetail.Contains("AdHocMSSLRequestNew"))
                        {
                            var detail = UtilityMethods.Deserialize<AdHocMSSLRequest>(entitlementDetail);
                            return string.IsNullOrEmpty(detail.Position) ? detail.Roles : string.IsNullOrEmpty(detail.Roles) ? detail.Position : detail.Position + "," + detail.Roles;
                        }
                        else
                        {
                            return UtilityMethods.Deserialize<AdHocMSSLRequest>(entitlementDetail).Entitlement;
                        }

                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        if (entitlementDetail.Contains("AdHocEBSRequestNew"))
                        {
                            var detail = UtilityMethods.Deserialize<AdHocEBSRequest>(entitlementDetail);
                            return string.IsNullOrEmpty(detail.Position) ? detail.Roles : string.IsNullOrEmpty(detail.Roles) ? detail.Position : detail.Position + "," + detail.Roles;
                        }
                        else
                        {
                            return UtilityMethods.Deserialize<AdHocEBSRequest>(entitlementDetail).Entitlement;
                        }

                    //case ApplicationType.eLearning:
                    //    return UtilityMethods.Deserialize<AdHocPLRELearningRequest>(entitlementDetail).Entitlement;

                    //case ApplicationType.eRecruitment:
                    //    return UtilityMethods.Deserialize<AdHocPERERecruitmentRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.SAP_PI:
                        return UtilityMethods.Deserialize<AdHocSAPPIRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.SAP_GRC:
                        return UtilityMethods.Deserialize<AdHocSAPGRCRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.FLINS_BI:
                        return UtilityMethods.Deserialize<AdHocSAPFLINSBIRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.VPN:
                        var adhocReqVPN = UtilityMethods.Deserialize<AdHocVPNRequest>(entitlementDetail);
                        return JoinedEntitlementAdHocVPNRequestDetail(adhocReqVPN.AdHocVPNRequestDetails);

                    case ApplicationType.InhouseSystems:
                        return UtilityMethods.Deserialize<AdHocINHRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.NPAC:
                        return UtilityMethods.Deserialize<AdHocNPACRequest>(entitlementDetail).Entitlement;

                    case ApplicationType.GEMS:
                        return UtilityMethods.Deserialize<AdHocGEMSRequest>(entitlementDetail).Entitlement;

                    // case ApplicationType.Miscellaneous:
                    //return UtilityMethods.Deserialize<AdHocMiscRequest>(entitlementDetail).enti;
                    //  return "";
                    case ApplicationType.SAP_EP:
                    case ApplicationType.CE:
                        return UtilityMethods.Deserialize<AdhocSAPEPRequest>(entitlementDetail).Entitlement;
                    case ApplicationType.SSIDS_GUI:
                    case ApplicationType.SSIDS_SMS:
                        return UtilityMethods.Deserialize<AdHocSSIDSRequest>(entitlementDetail).Entitlement;
                    case ApplicationType.SRM:
                        AdHocSAPSRMRequest srmadhocReq = UtilityMethods.Deserialize<AdHocSAPSRMRequest>(entitlementDetail);
                        return JoinedEntitlementAdHocSRMRequestDetail(srmadhocReq.AdHocSAPSRMRequestDetails);
                    case ApplicationType.EAM:
                        return UtilityMethods.Deserialize<AdHocNovaRequest>(entitlementDetail).Entitlement;
                    case ApplicationType.OneCard:
                        return UtilityMethods.Deserialize<AdHocSMSRequest>(entitlementDetail).Entitlement;
                    case ApplicationType.MicrosoftTeams:
                        return UtilityMethods.Deserialize<AdhocSkypeForBusinessRequest>(entitlementDetail).Entitlement;
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        if (entitlementDetail.Contains("AdHocSAPHANARequest"))
                        {
                            var detail = UtilityMethods.Deserialize<AdHocSAPHANARequest>(entitlementDetail);
                            return string.IsNullOrEmpty(detail.Position) ? detail.Roles : string.IsNullOrEmpty(detail.Roles) ? detail.Position : detail.Position + "," + detail.Roles;
                        }
                        else
                        {
                            return UtilityMethods.Deserialize<AdHocSAPHANARequest>(entitlementDetail).Entitlement;
                        }
                    default:
                        return string.Empty;
                }

            }
            catch { return ""; }

        }
    }
}
