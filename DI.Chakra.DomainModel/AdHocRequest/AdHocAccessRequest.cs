using AutoMapper;
using DI.Chakra.Dto;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace DI.Chakra.DomainModel
{
    public partial class AdHocAccessRequest : DomainModelBase<AdHocAccessRequest, EntityModel.AdHocAccessRequest>
    {

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

        public string CreatedBy { get; set; }

        public static List<KeyValuePair<string, string>> GetEntitlementDetailByEntitlement(long endpointSystemId, string entitlement)
        {
            List<KeyValuePair<string, string>> entitlementDetail = new List<KeyValuePair<string, string>>();
            var endpoint = EndPoint.GetById(endpointSystemId);
            var tmp = entitlement.Split(',');
            // The stored query template's "@@" token sits where a comma-separated value list
            // belongs (e.g. "... IN (@@)"), so it is replaced with the equivalent list of SQL
            // parameter names ("@entitlementParam0,@entitlementParam1,...") instead of a
            // literal quoted list - the actual (tainted) entitlement values are bound as real
            // SqlParameters below, never concatenated into the SQL text.
            var sqlParameters = new System.Data.SqlClient.SqlParameter[tmp.Length];
            var paramNames = new string[tmp.Length];
            for (int i = 0; i < tmp.Length; i++)
            {
                paramNames[i] = "@entitlementParam" + i;
                sqlParameters[i] = new System.Data.SqlClient.SqlParameter(paramNames[i], tmp[i]);
            }
            var entitlementStringForQuery = string.Join(",", paramNames);
            var configKey = $"{endpoint.ApplicationCode}_ENTITLEMENT_INFO";
            if (endpoint.EndpointSystemTypeId.Value == (long)DI.Chakra.Infrastructure.Enums.EndpointType.SAPHANA)
            {
                configKey = $"SAPHANA_ENTITLEMENT_INFO";
            }
            var config = ConfigurationMaster.GetByConfigKey(configKey, Product.IACM);
            if (config != null)
            {
                var query = config.ConfigValue.Replace("@@", entitlementStringForQuery);
                if(endpoint.EndpointSystemTypeId.Value == (long)DI.Chakra.Infrastructure.Enums.EndpointType.SAPHANA)
                {
                    query = query.Replace("##EP##", endpoint.Id.ToString());
                }
                using (DataAccessTransaction transaction = new DataAccessTransaction("System"))
                {
                    var dataTable = new AdHocAccessRequestRepository(transaction).GetDataTableByQuery(query, sqlParameters);
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        entitlementDetail.Add(new KeyValuePair<string, string>(Convert.ToString(dataRow[0]), Convert.ToString(dataRow[1])));
                    }
                }
            }
            return entitlementDetail;
        }

        public System.DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public EndPoint EndPoint { get; set; }

        public bool IsAddAccess { get; set; }

        public bool IsÉxpiryMailSend { get; set; }

        public bool IsÉxpiryHandled { get; set; }

        public long InterimStoreId { get; set; }

        public ApplicationType ApplicationType { get; set; }
        public string SecurityQuestions { get; set; }
        public int? ActualSupervisor { get; set; }
        public int? SelectedSupervisor { get; set; }
        public bool SupervisorChanged
        {
            get
            {
                return SelectedSupervisor.HasValue;
            }
            set
            {
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
                if (this.EndpointSystemId.HasValue)
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
                        return UtilityMethods.Deserialize<AdHocSAPLocalRequest>(this.EntitlementDetail).Entitlement;
                    case ApplicationType.ADSingaporePowerLocal:
                        return UtilityMethods.Deserialize<AdHocADRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.Office365:
                        return string.Empty;

                    case ApplicationType.HRMS_FLINS:
                        AdHocSAPHRFLINSRequest adhocReq = UtilityMethods.Deserialize<AdHocSAPHRFLINSRequest>(this.EntitlementDetail);
                        return JoinedEntitlementAdHocSAPHRFLINSRequestDetail(adhocReq.AdHocSAPHRFLINSRequestDetails);

                    case ApplicationType.MSSL_ISU:
                    case ApplicationType.MSSL_CRM:
                        return UtilityMethods.Deserialize<AdHocMSSLRequest>(this.EntitlementDetail).Entitlement;

                    case ApplicationType.EBS_ISU:
                    case ApplicationType.EBS_CRM:
                        return UtilityMethods.Deserialize<AdHocEBSRequest>(this.EntitlementDetail).Entitlement;

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

                    case ApplicationType.SRM:
                    case ApplicationType.SAP_SUS:
                    case ApplicationType.SAP_SRP:
                        AdHocSAPSRMRequest srmReq = UtilityMethods.Deserialize<AdHocSAPSRMRequest>(this.EntitlementDetail);
                        return JoinedEntitlementAdHocSAPSRMRequestDetail(srmReq.AdHocSAPSRMRequestDetails);
                    case ApplicationType.SSIDS_GUI:
                    case ApplicationType.SSIDS_SMS:
                        return UtilityMethods.Deserialize<AdHocSSIDSRequest>(this.EntitlementDetail).Entitlement;
                    case ApplicationType.IDMS:
                        return UtilityMethods.Deserialize<AdHocIACMRequest>(this.EntitlementDetail).Entitlement;
                    case ApplicationType.EAM:
                        return UtilityMethods.Deserialize<AdHocNovaRequest>(this.EntitlementDetail).Entitlement;
                    case ApplicationType.MicrosoftTeams:
                        return UtilityMethods.Deserialize<AdhocSkypeForBusinessRequest>(this.EntitlementDetail).Entitlement;
                    case ApplicationType.OneCard:
                        return UtilityMethods.Deserialize<AdHocSMSRequest>(this.EntitlementDetail).Entitlement;
                    case ApplicationType.EBS:
                    case ApplicationType.MSSL:
                        return UtilityMethods.Deserialize<AdHocSAPHANARequest>(this.EntitlementDetail).Entitlement;
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
                        return UtilityMethods.Serialize((AdHocINHRequest)value);

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
                    case ApplicationType.SSIDS_SMS:
                    case ApplicationType.SSIDS_GUI:
                        return UtilityMethods.Serialize<AdHocSSIDSRequest>((AdHocSSIDSRequest)value);
                    case ApplicationType.IDMS:
                        return UtilityMethods.Serialize<AdHocIACMRequest>((AdHocIACMRequest)value);
                    case ApplicationType.EAM:
                        return UtilityMethods.Serialize<AdHocNovaRequest>((AdHocNovaRequest)value);
                    case ApplicationType.MicrosoftTeams:
                        return UtilityMethods.Serialize<AdhocSkypeForBusinessRequest>((AdhocSkypeForBusinessRequest)value);

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

        public string GetOffice365SerializedString(IEntitlementDetail value)
        {
            var data = Mapper.Map<Dto.AdHocO365Request>(value);
            return UtilityMethods.DataSerialize(data);
        }
        public List<Presentation.PresentationAccessRequestItem> RequestedItems
        {
            get
            {
                if (string.IsNullOrEmpty(EntitlementDetail))
                    return null;

                return (List<Presentation.PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<Presentation.PresentationAccessRequestItem>(EntitlementDetail);
            }
        }

        public string SharedAccountName
        {
            get
            {
                return string.Empty;
            }
        }

        public string DisplayName { get; set; }

        public List<Presentation.PresentationAccessRequestItem> GetAccessRequestDetail()
        {
            if (string.IsNullOrEmpty(EntitlementDetail))
                return null;

            return (List<Presentation.PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<Presentation.PresentationAccessRequestItem>(EntitlementDetail);
        }
        public List<Presentation.PresentationAccessRequestItem> SetRequestedItems
        {
            set
            {
                if (value != null && value.Count > 0)
                {
                    EntitlementDetail = UtilityMethods.SerializeCollection<Presentation.PresentationAccessRequestItem>(value);
                }

            }
        }
        //Switch case revamped
        //public IEntitlementDetail GetEntitlementDetail()
        //{
        //    if (!string.IsNullOrEmpty(EntitlementDetail))
        //        return EndPointFactory.GetEndPoint(EndpointSystemType).GetEntitlementDetail(EntitlementDetail);
        //    else
        //        return null;
        //}
        ////Switch case revamped
        //public static string GetSerializedString(IEntitlementDetail value, EndpointType endpointSystemType)
        //{
        //    if (value != null)
        //        return EndPointFactory.GetEndPoint(endpointSystemType).GetSerializedString(value);
        //    else
        //        return string.Empty;
        //}
        public static string GetAccessRequestItemString(IAccessRequestItem value)
        {
            if (value != null)
                return UtilityMethods.Serialize<Presentation.PresentationAccessRequestItem>((Presentation.PresentationAccessRequestItem)value);
            else
                return string.Empty;
        }



        public IAccessRequestItem GetAccessRequestItemObject()
        {
            if (!string.IsNullOrEmpty(EntitlementDetail))
                return UtilityMethods.Deserialize<Presentation.PresentationAccessRequestItem>(EntitlementDetail);
            else
                return null;
        }
        public static AdHocAccessRequest GetByRequesteeUserId(long requesteeUserId, string comments, long endpointSystemId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AdHocAccessRequest>(transaction).Get(adhocAccReq => adhocAccReq.RequesteeUserId == requesteeUserId && adhocAccReq.Comments == comments && adhocAccReq.EndpointSystemId == endpointSystemId).FirstOrDefault());
            }
        }

        public static string GetEntitlementDetail(long adhocAccessRequestId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var request = ORMap(new RepositoryBase<EntityModel.AdHocAccessRequest>(transaction).
                    Get(adhocAccReq => adhocAccReq.Id == adhocAccessRequestId).FirstOrDefault());

                if (request != null && !string.IsNullOrEmpty(request.EntitlementDetail))
                {
                    try
                    {
                        var entitlements = (List<Presentation.PresentationAccessRequestItem>)UtilityMethods.DeserializeCollection<Presentation.PresentationAccessRequestItem>(request.EntitlementDetail);
                        if (entitlements != null && entitlements.Count > 0)
                            return string.Join(",", entitlements.Select(a => a.ItemDisplayName));
                        else return string.Empty;
                    }
                    catch (Exception ex)
                    {
                        return string.Empty;
                    }
                }

                return string.Empty;
            }
        }

        public static AdHocAccessRequest GetByAdhcoIdsAndActionType(AdHocActionType actionType, long requesteeUserId, long endPointSysytemId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AdHocAccessRequest>(transaction).
                    Get(adhocAccReq => adhocAccReq.RequesteeUserId == requesteeUserId && adhocAccReq.EndpointSystemId == endPointSysytemId && adhocAccReq.AdhocActionType == actionType).OrderByDescending(i => i.Id).FirstOrDefault());
            }
        }

        public static List<AdHocAccessRequest> GetPendingByRequesteeUserId(long requesteeUserId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AdHocAccessRequestRepository(transaction)
                    .GetPendingByRequesteeUserId(requesteeUserId));
            }
        }
        public static List<AdHocAccessRequest> GetRequestsFromInterimStore(long userId)
        {
            var list = InterimStore.GetByUserId(userId, true, InterimStoreItemType.AccessRequest);

            var adhocList = new List<AdHocAccessRequest>();
            foreach (var item in list)
            {
                var adhocRequest = UtilityMethods.Deserialize<AdHocAccessRequest>(item.XmlData);
                adhocRequest.InterimStoreId = item.Id;
                adhocList.Add(adhocRequest);
            }
            //var adhocList =  list.Select
            //     (item => UtilityMethods.Deserialize<AdHocAccessRequest>(item.XmlData)).ToList();

            return adhocList;
        }

        public static AdHocAccessRequest GetRequestsFromInterimStoreForEndpoint(long epId)
        {
            var list = InterimStore.GetByType(InterimStoreItemType.AccessRequest).OrderByDescending(a => a.Id).FirstOrDefault();

            AdHocAccessRequest adhocList = null;
            if (list != null)
            {
                var adhocRequest = UtilityMethods.Deserialize<AdHocAccessRequest>(list.XmlData);
                if (adhocRequest != null && adhocRequest.EndpointSystemId.HasValue && adhocRequest.EndpointSystemId.Value == epId)
                {
                    adhocRequest.InterimStoreId = list.Id;
                    adhocList = adhocRequest;
                }
            }
            //var adhocList =  list.Select
            //     (item => UtilityMethods.Deserialize<AdHocAccessRequest>(item.XmlData)).ToList();

            return adhocList;
        }

        public static List<AdHocAccessRequest> GetRemoveRequestsFromInterimStore(long userId)
        {
            List<AdHocAccessRequest> lst = new List<AdHocAccessRequest>();
            var list = InterimStore.GetByUserId(userId, true, InterimStoreItemType.Task);
            foreach (InterimStore data in list)
            {
                var adhocList = UtilityMethods.Deserialize<AdHocAccessRequest>(data.XmlData);
                lst.Add(adhocList);
            }
            return lst;
        }

        public static AdHocAccessRequest GetByIdIncludeEndPoint(long id, bool deepLoad)
        {
                if (!deepLoad)
                    return AdHocAccessRequest.GetById(id, deepLoad);
            else
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
                {
                    return ORMap(new RepositoryBase<EntityModel.AdHocAccessRequest>(dataAccessTransaction)
                     .GetById(id, workflow => workflow.EndPoint));
                }
            }
        }

        //public static bool IsSecondaryAccount(AdHocAccessRequest adhocAccessRequestObj)
        //{
        //    if (adhocAccessRequestObj.EndpointSystemType == EndpointType.ActiveDirectory)
        //    {
        //        return ADSUserInfo.IsSecondaryAccount(adhocAccessRequestObj.RequesteeUserId);
        //    }
        //    else if (adhocAccessRequestObj.EndpointSystemType == EndpointType.LotusNotes)
        //    {
        //        return LotusUserInfo.IsSecondaryAccount(adhocAccessRequestObj.RequesteeUserId);
        //    }
        //    else if (adhocAccessRequestObj.EndpointSystemType == EndpointType.SAP)
        //    {
        //        return SAPUserinfo.IsSecondaryAccount(adhocAccessRequestObj.RequesteeUserId);
        //    }
        //    return false;
        //}

        public static string IsSameRequestPending(AdHocAccessRequest adhocAccessRequestObj, long requesterId, long accessRequestId)
        {
            var accountName = string.Empty;
            IEntitlementDetail entitlementDetail = adhocAccessRequestObj.GetEntitlementDetail();
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {

                var pendingRequestsList = new AdHocAccessRequestRepository(transaction).GetPendingRequests(adhocAccessRequestObj.ORUnMap());
                var entitlement = pendingRequestsList.Where(pendingRequest =>
                {
                    if (adhocAccessRequestObj.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        if (adhocAccessRequestObj.AccountType != AccountType.User &&
                               pendingRequest.adHocAccessRequests != null &&
                               pendingRequest.adHocAccessRequests.AccountType == adhocAccessRequestObj.AccountType)
                        {
                            var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                            if (adhoc != null &&
                                   adhoc.GetEntitlementDetail() != null && adhocAccessRequestObj.GetEntitlementDetail() != null &&
                                   adhoc.GetEntitlementDetail().SharedAccountDetail != null && adhocAccessRequestObj.GetEntitlementDetail().SharedAccountDetail != null &&
                                   adhoc.GetEntitlementDetail().SharedAccountDetail.AccountName.Trim().Equals(adhocAccessRequestObj.GetEntitlementDetail().SharedAccountDetail.AccountName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                            {
                                return true;
                            }
                            else
                                return false;
                        }
                        else if (adhocAccessRequestObj.AccountType == AccountType.User && adhocAccessRequestObj.ApplicationType == ApplicationType.SRM)
                        {
                            var pendingAdhoc = (AdHocSAPSRMRequest)(ORMap(pendingRequest.adHocAccessRequests).GetEntitlementDetail());
                            var adhoc = (AdHocSAPSRMRequest)adhocAccessRequestObj.GetEntitlementDetail();
                            if (pendingAdhoc != null && adhoc != null)
                            {
                                return adhoc.AccountName.Equals(pendingAdhoc.AccountName, StringComparison.InvariantCultureIgnoreCase);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return (pendingRequest.adHocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType);
                        }
                    }
                    else if (adhocAccessRequestObj.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                        var entitlementDetails = adhoc?.GetEntitlementDetail();

                        if (adhocAccessRequestObj.AccountType != AccountType.User &&
                               pendingRequest.adHocAccessRequests != null &&
                               pendingRequest.adHocAccessRequests.AccountType == adhocAccessRequestObj.AccountType)
                        {
                            var adhocAccessRequestObjEntitlementDetails = adhocAccessRequestObj?.GetEntitlementDetail();
                            if (adhoc != null &&
                                   entitlementDetails != null && adhocAccessRequestObjEntitlementDetails != null &&
                                   entitlementDetails.EndpointUserId > 0 && adhocAccessRequestObjEntitlementDetails.EndpointUserId > 0 &&
                                   entitlementDetails.EndpointUserId == adhocAccessRequestObjEntitlementDetails.EndpointUserId)
                            {
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                            return (pendingRequest.adHocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType && entitlementDetail.EndpointUserId == entitlementDetails.EndpointUserId);
                    }
                    else if (pendingRequest.adHocAccessRequests.EntitlementDetail != null && !string.IsNullOrWhiteSpace(adhocAccessRequestObj.Entitlement))
                    {
                        //var xmlDoc = new XmlDocument();

                        var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                        var aReqDetails = adhoc.GetEntitlementDetail();
                        if (aReqDetails.DisplayAccountName != null && entitlementDetail.DisplayAccountName != null && aReqDetails.DisplayAccountName.ToUpper() == entitlementDetail.DisplayAccountName.ToUpper())
                        {
                            //xmlDoc.LoadXml(pendingRequest.adHocAccessRequests.EntitlementDetail);

                            var existingRequestedEntitlements = aReqDetails.Entitlement?.Split(Constants.COMMA); //pendingRequest.adHocAccessRequests.EntitlementDetail xmlDoc.GetElementsByTagName("Entitlement")?.Item(0)?.InnerText?.Split(Constants.COMMA);
                            var newlyRequestedEntitlements = adhocAccessRequestObj.Entitlement?.Split(Constants.COMMA);
                            return existingRequestedEntitlements.Any(p => newlyRequestedEntitlements.Contains(p));
                        }
                        return false;
                        //return pendingRequest.adHocAccessRequests.EntitlementDetail.Contains(adhocAccessRequestObj.Entitlement);
                    }

                    else
                        return false;

                }).Select(pendingRequest =>
                {

                    //if (pendingRequest.adHocAccessRequests != null && pendingRequest.adHocAccessRequests.AccountType != AccountType.User)
                    //{
                    //    var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                    //    if (adhoc != null && adhoc.GetEntitlementDetail() != null && adhoc.GetEntitlementDetail().SharedAccountDetail != null)
                    //    {
                    //        accountName = adhoc.GetEntitlementDetail().SharedAccountDetail.AccountName;
                    //    }
                    //}

                    return new
                    {
                        AccessRequestId = pendingRequest.accessRequests.Id,
                        requesterid = pendingRequest.adHocAccessRequests.RequesterUserId
                    };
                }).ToList();


                if (entitlement != null && entitlement.Count > 0)
                {
                    accessRequestId = entitlement[0].AccessRequestId;
                    requesterId = entitlement[0].requesterid;
                }
                else
                {
                    accessRequestId = 0;
                    requesterId = 0;
                }
                if (entitlement == null || entitlement.Count == 0)
                    return string.Empty;
            }
            return requesterId.ToString() + ";" + accessRequestId.ToString();
        }
        public static string IsSameRemediationRequestPending(AdHocAccessRequest adhocAccessRequestObj, long? campaignId, long userId)
        {
            var accountName = string.Empty;
            IEntitlementDetail entitlementDetail = adhocAccessRequestObj.GetEntitlementDetail();
            long accessRequestId = 0;
            long requesterId = 0;
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var pendingRequestsList = new AdHocAccessRequestRepository(transaction).GetPendingRemediationRequests(adhocAccessRequestObj.ORUnMap(),campaignId,userId);
                var entitlement = pendingRequestsList.Where(pendingRequest =>
                {
                    if (adhocAccessRequestObj.AdhocActionType == AdHocActionType.DeleteAccount)
                    {
                        var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                        var entitlementDetails = adhoc?.GetEntitlementDetail();

                        if (adhocAccessRequestObj.AccountType != AccountType.User &&
                               pendingRequest.adHocAccessRequests != null &&
                               pendingRequest.adHocAccessRequests.AccountType == adhocAccessRequestObj.AccountType)
                        {
                            var adhocAccessRequestObjEntitlementDetails = adhocAccessRequestObj?.GetEntitlementDetail();
                            if (adhoc != null &&
                                   entitlementDetails != null && adhocAccessRequestObjEntitlementDetails != null &&
                                   entitlementDetails.EndpointUserId > 0 && adhocAccessRequestObjEntitlementDetails.EndpointUserId > 0 &&
                                   entitlementDetails.EndpointUserId == adhocAccessRequestObjEntitlementDetails.EndpointUserId)
                            {
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                            return (pendingRequest.adHocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType && entitlementDetail.EndpointUserId == entitlementDetails.EndpointUserId);
                    }
                    else if (pendingRequest.adHocAccessRequests.EntitlementDetail != null && !string.IsNullOrWhiteSpace(adhocAccessRequestObj.Entitlement))
                    {
                        //var xmlDoc = new XmlDocument();

                        var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                        var aReqDetails = adhoc.GetEntitlementDetail();
                        if (aReqDetails.DisplayAccountName != null && entitlementDetail.DisplayAccountName != null && aReqDetails.DisplayAccountName.ToUpper() == entitlementDetail.DisplayAccountName.ToUpper())
                        {
                            //xmlDoc.LoadXml(pendingRequest.adHocAccessRequests.EntitlementDetail);
                            if (adhoc.ApplicationType == adhocAccessRequestObj.ApplicationType && aReqDetails is AdHocVPNRequest)
                            {
                                var existingVPNRequest = aReqDetails as AdHocVPNRequest;
                                var newVPNRequest = adhocAccessRequestObj.GetEntitlementDetail() as AdHocVPNRequest;
                                if(existingVPNRequest != null && existingVPNRequest.AdHocVPNRequestDetails != null && existingVPNRequest.AdHocVPNRequestDetails.Count > 0
                                && newVPNRequest != null && newVPNRequest.AdHocVPNRequestDetails != null && newVPNRequest.AdHocVPNRequestDetails.Count > 0)
                                {
                                    var exReqData = existingVPNRequest.AdHocVPNRequestDetails.FirstOrDefault();
                                    var newReqData = newVPNRequest.AdHocVPNRequestDetails.FirstOrDefault();
                                    if(exReqData.ApplicationName == newReqData.ApplicationName &&
                                    exReqData.MatrixType == newReqData.MatrixType &&
                                    exReqData.DevelopmentADGroups == newReqData.DevelopmentADGroups &&
                                    exReqData.QAADgroups == newReqData.QAADgroups &&
                                    exReqData.ProductionADGroups == newReqData.ProductionADGroups)
                                    {
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                var existingRequestedEntitlements = aReqDetails.Entitlement?.Split(Constants.COMMA); //pendingRequest.adHocAccessRequests.EntitlementDetail xmlDoc.GetElementsByTagName("Entitlement")?.Item(0)?.InnerText?.Split(Constants.COMMA);
                                var newlyRequestedEntitlements = adhocAccessRequestObj.Entitlement?.Split(Constants.COMMA);
                                return existingRequestedEntitlements.Any(p => newlyRequestedEntitlements.Contains(p));
                            }
                        }
                        return false;
                        //return pendingRequest.adHocAccessRequests.EntitlementDetail.Contains(adhocAccessRequestObj.Entitlement);
                    }

                    else
                        return false;

                }).Select(pendingRequest =>
                {
                    return new
                    {
                        AccessRequestId = pendingRequest.accessRequests.Id,
                        requesterid = pendingRequest.adHocAccessRequests.RequesterUserId
                    };
                }).ToList();

                
                if (entitlement != null && entitlement.Count > 0)
                {
                    accessRequestId = entitlement[0].AccessRequestId;
                    requesterId = entitlement[0].requesterid;
                }
                else
                {
                    accessRequestId = 0;
                    requesterId = 0;
                }
                if (entitlement == null || entitlement.Count == 0)
                    return string.Empty;
            }
            return requesterId.ToString() + ";" + accessRequestId.ToString();
        }
        /// <summary>
        /// Used to check whether any delete request created for the account before we create AmendmentRemove Request for the same user and account
        /// </summary>
        /// <param name="adhocAccessRequestObj"></param>
        /// <param name="campaignId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public long? IsRemediationDeleteAccountRequestExists(AdHocAccessRequest adhocAccessRequestObj, long? campaignId, long userId)
        {
            long? accessRequestid = 0;
            try
            {
                IEntitlementDetail entitlementDetail = adhocAccessRequestObj.GetEntitlementDetail();
                using (DataAccessTransaction transaction = new DataAccessTransaction())
                {
                    var pendingRequestsList = new AdHocAccessRequestRepository(transaction).GetRemediationDeleteAccountRequests(adhocAccessRequestObj.ORUnMap(), campaignId, userId);
                    accessRequestid = pendingRequestsList.FirstOrDefault(pendingRequest =>
                    {
                        if (adhocAccessRequestObj.AdhocActionType == AdHocActionType.AmendmentRemove)
                        {
                            var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                            var entitlementDetails = adhoc?.GetEntitlementDetail();

                            if (adhocAccessRequestObj.AccountType != AccountType.User &&
                                   pendingRequest.adHocAccessRequests != null &&
                                   pendingRequest.adHocAccessRequests.AccountType == adhocAccessRequestObj.AccountType)
                            {
                                var adhocAccessRequestObjEntitlementDetails = adhocAccessRequestObj?.GetEntitlementDetail();
                                if (adhoc != null &&
                                       entitlementDetails != null && adhocAccessRequestObjEntitlementDetails != null &&
                                       entitlementDetails.EndpointUserId > 0 && adhocAccessRequestObjEntitlementDetails.EndpointUserId > 0 &&
                                       entitlementDetails.EndpointUserId == adhocAccessRequestObjEntitlementDetails.EndpointUserId)
                                {
                                    return true;
                                }
                                else
                                    return false;
                            }
                            else
                                return (pendingRequest.adHocAccessRequests.AdhocActionType == adhocAccessRequestObj.AdhocActionType && entitlementDetail.EndpointUserId == entitlementDetails.EndpointUserId);
                        }
                        else
                            return false;

                    })?.accessRequests?.Id;
                }
            }
            catch
            {
            }
            return accessRequestid;
        }
        public static bool IsDependentRequestPendingForEBSMSSL(AdHocAccessRequest adhocRequest, ApplicationType appType)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var pendingRequestsList = new AdHocAccessRequestRepository(transaction).GetPendingRequests(adhocRequest.ORUnMap(), appType);
                foreach (var pendingRequest in pendingRequestsList)
                {
                    if (adhocRequest.AdhocActionType == AdHocActionType.NewAccount)
                    {
                        if (adhocRequest.AccountType != AccountType.User &&
                               pendingRequest.adHocAccessRequests != null &&
                               pendingRequest.adHocAccessRequests.AccountType == adhocRequest.AccountType)
                        {
                            var adhoc = ORMap(pendingRequest.adHocAccessRequests);
                            if (adhoc != null &&
                                   adhoc.GetEntitlementDetail() != null && adhocRequest.GetEntitlementDetail() != null &&
                                   adhoc.GetEntitlementDetail().SharedAccountDetail != null && adhocRequest.GetEntitlementDetail().SharedAccountDetail != null &&
                                   adhoc.GetEntitlementDetail().SharedAccountDetail.AccountName.Trim().Equals(adhocRequest.GetEntitlementDetail().SharedAccountDetail.AccountName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                            {
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                            return (pendingRequest.adHocAccessRequests.AdhocActionType == adhocRequest.AdhocActionType);
                    }
                }
                return false;
            }
        }

        public static string IsSameRequestPendingSpecialACR(AdHocAccessRequest adhocAccessRequestObj, long requesterId, long accessRequestId)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var pendingRequestsSpecialACRList = new AdHocAccessRequestRepository(transaction).GetPendingRequestsSpecialACR(adhocAccessRequestObj.ORUnMap());
                var AdhocRequestList = ORMapList(pendingRequestsSpecialACRList.Select(l => l.adHocAccessRequests).ToList());

                var entitlement = (from pendingRequestSpecialACR in AdhocRequestList
                                   where
                                   (
                                   (adhocAccessRequestObj.AdhocActionType == AdHocActionType.NewAccount ? pendingRequestSpecialACR.SharedAccountName == adhocAccessRequestObj.SharedAccountName : pendingRequestSpecialACR.EntitlementDetail == adhocAccessRequestObj.EntitlementDetail))
                                   select new
                                   {
                                       AccessRequestId = (pendingRequestsSpecialACRList.Where(lq => lq.accessRequests.AdHocAccessRequestId == pendingRequestSpecialACR.Id).FirstOrDefault().accessRequests.Id),
                                       requesterid = pendingRequestSpecialACR.RequesterUserId,
                                   }).ToList();

                if (entitlement != null && entitlement.Count > 0)
                {
                    accessRequestId = entitlement[0].AccessRequestId;
                    requesterId = entitlement[0].requesterid;
                }
                else
                {
                    accessRequestId = 0;
                    requesterId = 0;
                }
                if (entitlement == null || entitlement.Count == 0)
                    return string.Empty;
            }
            return requesterId.ToString() + ";" + accessRequestId.ToString();
        }

        public static Dictionary<string, string> GetListofEntitlementByRequesteeUserId(int CurrentRequesteeUserId)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                List<AdHocAccessRequest> list = ORMapList(new AdHocAccessRequestRepository(transaction).GetListofEntitlementByRequesteeUserId(CurrentRequesteeUserId));
                foreach (AdHocAccessRequest details in list)
                {
                    AdHocSAPHRFLINSRequest flinsRequest = (AdHocSAPHRFLINSRequest)details.GetEntitlementDetail();
                    if (null != flinsRequest)
                    {
                        flinsRequest.AdHocSAPHRFLINSRequestDetails.ForEach(lq =>
                        {

                            lq.Entitlement.Split(',').ToList().ForEach(l =>
                            {
                                if (!keyValuePairs.ContainsKey(l))
                                {
                                    AccessRequest accessrequest = (DomainModel.AccessRequest.GetByAdHocAccessRequestId(details.Id));
                                    if (accessrequest != null)
                                        keyValuePairs.Add(l, "Requested" + "," + accessrequest.Id.ToString());
                                }
                            });
                        });
                    }
                }
            }
            return keyValuePairs;
        }

        public static bool DoesNewAccountExist(AdHocAccessRequest adhocAccessRequestObj)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                var newAccList = new AdHocAccessRequestRepository(transaction).GetNewAccount(adhocAccessRequestObj.ORUnMap());
                if (newAccList == null || newAccList.Count == 0)
                    return false;
            }
            return true;
        }

        public static List<AdHocAccessRequest> GetAdocRequestAboutToExpiry(int days)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AdHocAccessRequestRepository(transaction)
                    .GetAdocRequestAboutToExpiry(days));
            }
        }

        public static List<AdHocAccessRequest> GetExpiredAdocRequest()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMapList(new AdHocAccessRequestRepository(transaction)
                    .GetExpiredAdocRequest());
            }
        }

        public static bool GetExpiredRemoveAdhocRequest(AdHocAccessRequest adhocaccessReqeust)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new AdHocAccessRequestRepository(transaction).GetExpiredRemoveAdhocRequest(adhocaccessReqeust.ORUnMap());
            }
        }
        //public static AdHocAccessRequest GetRemoveAdocRequest(DateTime validTo, AdHocActionType adhocActionType, long userId)
        //{
        //    using (DataAccessTransaction transaction = new DataAccessTransaction())
        //    {
        //        return ORMap(new AdHocAccessRequestRepository(transaction)
        //                .GetRemoveAdocRequest(validTo, adhocActionType, userId));
        //    }
        //}

        public static List<string> GetHrFlinsTechnicalRolesByAdhocRequestObject(AdHocSAPHRFLINSRequestDetail requestDetail, bool isAmendmentRemove = false)
        {
            List<string> hrFlinsRoles = new List<string>();
            try
            {
                string costCenter = requestDetail.CostCenter;
                string companyCode = requestDetail.Company;
                if (string.IsNullOrWhiteSpace(requestDetail.Company))
                {
                    companyCode = string.Empty;
                }
                string businessRole = requestDetail.Entitlement;
                string purchasingGroup = requestDetail.PurchaseGroup;
                string Plant = requestDetail.Plant;
                string MRPController = requestDetail.MRPController;
                string StorageLocation = requestDetail.StorageLocation;
                bool RetainCompanyRoles = requestDetail.RetainCompanyRoles;

                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (!RetainCompanyRoles)
                    {
                        var mandatoryField = new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction)
                    .Get(item => item.BusinessRole == businessRole).FirstOrDefault();

                        if (mandatoryField == null)
                        {
                            hrFlinsRoles = new RepositoryBase<EntityModel.SAPHRFLINSRoleMatrix>(dataAccessTransaction)
                                .Get(item => item.BusinessRole == businessRole).Select(role => role.TechnicalRule).ToList();
                        }

                        //else if (mandatoryField.MandatoryField2 == string.Empty)
                        else if (string.IsNullOrEmpty(mandatoryField.MandatoryField2) || (!string.IsNullOrEmpty(mandatoryField.MandatoryField2) && mandatoryField.MandatoryField2.Trim().Length == 0))
                        {
                            var hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRole(businessRole, companyCode);

                            List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                            .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" && RoleMatrix.PurchasingOrg.Trim() == "")
                                            .Select(role => role.TechnicalRule)
                                            .ToList();

                            List<SAPHRFLINSRoleMatrix> roleMatrixes = hrFlinsRoleMatrix.ToList();

                            List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "CostCenter");
                            List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "PurchasingGroup");

                            hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                            hrFlinsRoles = hrFlinsRoles.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_CostCenter)
                        {
                            var hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRole(businessRole, companyCode);

                            List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                            .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" || RoleMatrix.CostCenter == null)
                                            .Select(role => role.TechnicalRule)
                                            .ToList();

                            List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "CostCenter");

                            hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_PurchaseGroup)
                        {
                            var hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRole(businessRole, companyCode);

                            List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                            .Where(RoleMatrix => RoleMatrix.PurchasingGrp.Trim() == "" || RoleMatrix.PurchasingGrp == null)
                                            .Select(role => role.TechnicalRule)
                                            .ToList();

                            List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "PurchaseGroup");

                            hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                        }

                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_Plant)
                        {
                            string[] lstPlants = Plant.Split(',');
                            var hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRole(businessRole, companyCode);

                            hrFlinsRoles = (from roleMatrix in hrFlinsRoleMatrix
                                            where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                            join plants in lstPlants
                                             on roleMatrix.Plant equals plants
                                            select roleMatrix.TechnicalRule).ToList();
                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_StoreLocation)
                        {
                            string[] lstStorageLocations = StorageLocation.Split(',');
                            var hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRole(businessRole, companyCode);

                            hrFlinsRoles = (from roleMatrix in hrFlinsRoleMatrix
                                            where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                            join storageLocation in lstStorageLocations
                                             on roleMatrix.StorageLocation equals storageLocation
                                            select roleMatrix.TechnicalRule).ToList();
                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_MRPController)
                        {
                            string[] lstMrpController = MRPController.Split(',');
                            var hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRole(businessRole, companyCode);

                            hrFlinsRoles = (from roleMatrix in hrFlinsRoleMatrix
                                            where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                            join mrpController in lstMrpController
                                             on roleMatrix.MRPControl equals mrpController
                                            select roleMatrix.TechnicalRule).ToList();
                        }
                    }
                    else
                    {
                        var hrFlinsRoleMatrix = new List<SAPHRFLINSRoleMatrix>();
                        var mandatoryField = new RepositoryBase<EntityModel.SAPRoleMandatoryField>(dataAccessTransaction)
                                            .Get(item => item.BusinessRole == businessRole).FirstOrDefault();


                        string SqlQuery = string.Empty;
                        if (mandatoryField == null)
                        {
                            hrFlinsRoles = SAPHRFLINSRoleMatrix.GetByBusinessRole(businessRole, true)
                               .Select(role => role.TechnicalRule).ToList();

                        }
                        //else if (mandatoryField.MandatoryField2 == string.Empty)
                        else if (string.IsNullOrEmpty(mandatoryField.MandatoryField2) || (!string.IsNullOrEmpty(mandatoryField.MandatoryField2) && mandatoryField.MandatoryField2.Trim().Length == 0))
                        {
                            hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRolesForRetain(businessRole, companyCode);

                            List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                            .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" && RoleMatrix.PurchasingOrg.Trim() == "")
                                            .Select(role => role.TechnicalRule)
                                            .ToList();

                            List<SAPHRFLINSRoleMatrix> roleMatrixes = hrFlinsRoleMatrix.ToList();

                            List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "CostCenter");
                            List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(roleMatrixes, companyCode, costCenter, purchasingGroup, "PurchasingGroup");

                            hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                            hrFlinsRoles = hrFlinsRoles.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_CostCenter)
                        {
                            hrFlinsRoleMatrix = new List<SAPHRFLINSRoleMatrix>();
                            hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRolesForRetain(businessRole, companyCode);

                            List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                            .Where(RoleMatrix => RoleMatrix.CostCenter.Trim() == "" || RoleMatrix.CostCenter == null)
                                            .Select(role => role.TechnicalRule)
                                            .ToList();

                            List<string> hrFlinsSupplementaryRolesCostCenter = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "CostCenter");

                            hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesCostCenter).ToList();
                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_PurchaseGroup)
                        {
                            hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRolesForRetain(businessRole, companyCode);

                            List<string> hrFlinsRolesWithoutSupplementary = hrFlinsRoleMatrix
                                            .Where(RoleMatrix => RoleMatrix.PurchasingGrp.Trim() == "" || RoleMatrix.PurchasingGrp == null)
                                            .Select(role => role.TechnicalRule)
                                            .ToList();

                            List<string> hrFlinsSupplementaryRolesPurchasingGroup = GetHrFlinsSupplementaryRoles(hrFlinsRoleMatrix.ToList(), companyCode, costCenter, purchasingGroup, "PurchaseGroup");

                            hrFlinsRoles = hrFlinsRolesWithoutSupplementary.Union(hrFlinsSupplementaryRolesPurchasingGroup).ToList();

                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_Plant)
                        {
                            string[] lstPlants = Plant.Split(',');
                            hrFlinsRoleMatrix = new List<SAPHRFLINSRoleMatrix>();
                            hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRolesForRetain(businessRole, companyCode);

                            hrFlinsRoles = (from roleMatrix in hrFlinsRoleMatrix
                                            where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                            join plants in lstPlants
                                             on roleMatrix.Plant equals plants
                                            select roleMatrix.TechnicalRule).ToList();

                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_StoreLocation)
                        {
                            string[] lstStorageLocations = StorageLocation.Split(',');
                            hrFlinsRoleMatrix = new List<SAPHRFLINSRoleMatrix>();
                            hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRolesForRetain(businessRole, companyCode);

                            hrFlinsRoles = (from roleMatrix in hrFlinsRoleMatrix
                                            where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                            join storageLocation in lstStorageLocations
                                             on roleMatrix.StorageLocation equals storageLocation
                                            select roleMatrix.TechnicalRule).ToList();
                        }
                        else if (mandatoryField.MandatoryField2 == Constants.MandatoryField_MRPController)
                        {
                            string[] lstMrpController = MRPController.Split(',');
                            hrFlinsRoleMatrix = SAPHRFLINSRoleMatrix.GetByBusinessRolesForRetain(businessRole, companyCode);

                            hrFlinsRoles = (from roleMatrix in hrFlinsRoleMatrix
                                            where (roleMatrix.BusinessRole == businessRole && roleMatrix.Company == companyCode)
                                            join mrpController in lstMrpController
                                             on roleMatrix.MRPControl equals mrpController
                                            select roleMatrix.TechnicalRule).ToList();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return hrFlinsRoles.Distinct().ToList();
        }

        static List<string> GetHrFlinsSupplementaryRoles(List<SAPHRFLINSRoleMatrix> roleMatrixes, string companyCode, string costCenter, string purchasingGroup,
                                                                                                   string MandatoryFieldName)
        {
            List<string> SupplementaryRoles = new List<string>();
            if (MandatoryFieldName == Constants.MandatoryField_CostCenter)
            {
                string[] lstCostCenter = costCenter?.Split(',');
                if (lstCostCenter == null) lstCostCenter = new string[] { };
                foreach (SAPHRFLINSRoleMatrix roleMatrix in roleMatrixes)
                {
                    if (roleMatrix.CostCenter != null && roleMatrix.CostCenter.Trim() == "X")
                    {
                        // company 
                        List<SAPHRFLINSSuppRoleMatrix> supplementaryRoles = SAPHRFLINSSuppRoleMatrix.GetByTechnicalRuleAndCompany(roleMatrix.TechnicalRule, roleMatrix.Company);

                        foreach (SAPHRFLINSSuppRoleMatrix supplementaryRole in supplementaryRoles)
                        {
                            if (supplementaryRole.CostCenter != null && lstCostCenter.FirstOrDefault(sRole => sRole.Trim() == supplementaryRole.CostCenter.Trim()) != null)
                            {
                                SupplementaryRoles.Add(supplementaryRole.Role);
                            }
                        }
                    }
                }
            }
            else if (MandatoryFieldName == Constants.MandatoryField_PurchaseGroup)
            {

                string[] lstPurchasingGroup = purchasingGroup.Split(',');

                foreach (SAPHRFLINSRoleMatrix roleMatrix in roleMatrixes)
                {
                    if (roleMatrix.PurchasingGrp != null && roleMatrix.PurchasingGrp.Trim() == "X")
                    {
                        List<SAPHRFLINSSuppRoleMatrix> supplementaryRoles = SAPHRFLINSSuppRoleMatrix.GetByTechnicalRule(roleMatrix.TechnicalRule);

                        foreach (SAPHRFLINSSuppRoleMatrix supplementaryRole in supplementaryRoles)
                        {
                            if (supplementaryRole.PurchasingGrp != null && lstPurchasingGroup.FirstOrDefault(sRole => sRole.Trim() == supplementaryRole.PurchasingGrp.Trim()) != null)
                            {
                                SupplementaryRoles.Add(supplementaryRole.Role);
                            }
                        }
                    }
                }
            }

            return SupplementaryRoles;
        }
        /// <summary>
        /// For Attestation remediation request generation it requires conveting to Domainmodel to Dto
        /// </summary>
        /// <param name="entitlementDetail"></param>
        /// <returns></returns>
        public static string GetSerializedStringForO365(IEntitlementDetail entitlementDetail)
        {
            var data = Mapper.Map<Dto.AdHocO365Request>(entitlementDetail);
            return UtilityMethods.DataSerialize(data);
        }

    }
}
