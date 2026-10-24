using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_AdHocAccessRequest")]
    public partial class AdHocAccessRequest
    {
        [Key]
        public long Id { get; set; }

        public string TenantId { get; set; }
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
        public System.DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        [XmlIgnore]
        [ForeignKey("EndpointSystemId")]
        public virtual EndPoint EndPoint { get; set; }

        public string AccountTypeString
        {
            get { return UtilityMethods.GetEnumAttribute<Description>((AccountType)this.AccountType); }
        }

        public string RequestTypeString
        {
            get { return UtilityMethods.GetEnumAttribute<Description>((AdHocActionType)this.AdhocActionType); }
        }

        public string AccessName
        {
            get { return this.EndPoint.DisplayName; }
        }
        [NotMapped]
        public bool IsAddAccess { get; set; }
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
                    if (this.EndpointSystemType == EndpointType.VPN)
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
                var detail = GetEntitlementDetail();
                return detail == null ? null : detail.Entitlement;
            }
        }

        public string SharedAccountName
        {
            get
            {
                var detail = GetEntitlementDetail();
                return detail == null ? "" : detail.SharedAccountName;
            }
        }
    }

    public partial class AdHocAccessRequest
    {
        public IEntitlementDetail GetEntitlementDetail()
        {
            if (!string.IsNullOrEmpty(EntitlementDetail))
            {
                switch (EndpointSystemType)
                {
                    case EndpointType.ActiveDirectory:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocADRequest>(EntitlementDetail);

                    case EndpointType.LotusNotes:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocLNSRequest>(EntitlementDetail);

                    case EndpointType.SAP:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocSAPRequest>(EntitlementDetail);

                    case EndpointType.InhouseSystems:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocInhouseSystemRequest>(EntitlementDetail);

                    case EndpointType.OracleEBS:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocOracleEBSRequest>(EntitlementDetail);

                    case EndpointType.Linux:
                        return (IEntitlementDetail)UtilityMethods.Deserialize<AdHocLINUXRequest>(EntitlementDetail);


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
                switch (EndpointSystemType)
                {
                    case EndpointType.ActiveDirectory:
                        return UtilityMethods.Serialize<AdHocADRequest>((AdHocADRequest)value);

                    case EndpointType.LotusNotes:
                        return UtilityMethods.Serialize<AdHocLNSRequest>((AdHocLNSRequest)value);

                    case EndpointType.SAP:
                        return UtilityMethods.Serialize<AdHocSAPRequest>((AdHocSAPRequest)value);

                    case EndpointType.InhouseSystems:
                        return UtilityMethods.Serialize<AdHocInhouseSystemRequest>((AdHocInhouseSystemRequest)value);

                    case EndpointType.OracleEBS:
                        return UtilityMethods.Serialize<AdHocOracleEBSRequest>((AdHocOracleEBSRequest)value);

                    case EndpointType.Linux:
                        return UtilityMethods.Serialize<AdHocLINUXRequest>((AdHocLINUXRequest)value);
                    case EndpointType.ORCLUM:
                        return UtilityMethods.Serialize<AdHocOracleUMRequest>((AdHocOracleUMRequest)value);

                    default:
                        return null;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
