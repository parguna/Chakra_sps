using DI.Chakra.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Linq;
namespace DI.Chakra.Dto.Presentation
{
    [Serializable]
    public class PresentationOUChangeSnapshots:IIDMSReportPropertyExtractor
    {
        public long SnapshotId { get; set; }
        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
        public string EmailID { get; set; }
        public long EndpointSystemTypeID { get; set; }
        public string EndpointSystemType { get; set; }
        public string EndpointsystemName { get; set; }
        public long EndpointUserID { get; set; }
        public string AccountName { get; set; }
        public EntitlementType EntitlementType { get; set; }
        public string Entitlement { get; set; }
        public DateTime CreatedDate { get; set; }
        public EmployeeEvents EmployeeEventType { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string Entitlements
        {
            get
            {
                var entitlements = this.Entitlement;
                if (!string.IsNullOrWhiteSpace(entitlements))
                {
                    try
                    {
                        if (ApplicationType.IDMS == ApplicationType || ApplicationType.MicrosoftTeams == ApplicationType || ApplicationType.VPN == ApplicationType)
                        {
                            var snapshot = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(entitlements);
                            if (snapshot != null)
                            {
                                var grpEnt = snapshot.Entitlements.GroupBy(g => g.EntitlementType).Select(e => new { e.Key, entitlements = string.Join(",", e.Select(i => i.Entitlement)) });
                                entitlements = string.Join(Environment.NewLine, grpEnt.Select(i => $"{(DI.Chakra.Infrastructure.Enums.EntitlementType)i.Key} : {i.entitlements}"));
                            }
                        }
                        else if (ApplicationType.EAM == ApplicationType)
                        {
                            var snapshot = JsonConvert.DeserializeObject<SnapshotUserDetails<SnapshotUser>>(entitlements);
                            if (snapshot != null)
                            {
                                var grpEnt = snapshot.Entitlements.GroupBy(g => g.EntitlementType).Select(e => new {
                                    e.Key,
                                    entitlements =
                                    string.Join(",", e.Select(i => (i != null && !string.IsNullOrWhiteSpace(i.Entitlement)) ? JsonConvert.DeserializeObject<SAPEamNovaUserRoleInfo>(i.Entitlement)?.RoleName : ""))
                                });
                                entitlements = string.Join(Environment.NewLine, grpEnt.Select(i => $"{(DI.Chakra.Infrastructure.Enums.EntitlementType)i.Key} : {i.entitlements}"));
                            }
                        }
                        else if (ApplicationType.SRM == ApplicationType)
                        {
                            var snapshot = JsonConvert.DeserializeObject<SRMSnapShot>(entitlements);
                            if (snapshot != null && snapshot.SRMUserRoles != null)
                            {
                                var grp = snapshot.SRMUserRoles.GroupBy(j => new { j.BusinessRole, j.POrg }).Select(k => $"{k.Key.BusinessRole} - {k.Key.POrg}" + " : " + string.Join(" , ", k.Select(l => l.PGrp)));
                                entitlements = string.Join("<br/>", grp);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return entitlements;
            }
        }

        public string GetReportPropertyValue(string propertyName, string format)
        {
            var returnValue = string.Empty;
            switch (propertyName)
            {
                case "CreatedDate":
                    returnValue = string.IsNullOrWhiteSpace(format) ? CreatedDate.ToString("dd-MMM-yyyy hh:mm:tt"):CreatedDate.ToString(format);
                    break;
                case "EmployeeEventType":
                    returnValue = EmployeeEventType.ToString() ?? "";
                    break;
                case "UserName":
                    returnValue = UserName?.ToString() ?? "";
                    break;
                case "EmployeeNo":
                    returnValue = EmployeeNo?.ToString() ?? "";
                    break;
                case "EmailID":
                    returnValue = EmailID?.ToString() ?? "";
                    break;
                case "EndpointSystemType":
                    returnValue = EndpointSystemType?.ToString() ?? "";
                    break;
                case "EndpointsystemName":
                    returnValue = EndpointsystemName?.ToString() ?? "";
                    break;
                case "AccountName":
                    returnValue = AccountName ?? "";
                    break;
                case "AccountType":
                    returnValue = AccountType.User.ToString();
                    break;
                case "Entitlements":
                    returnValue = Entitlements?.Replace("<br/>", Environment.NewLine).ToString() ?? "";
                    break;
                case "Entitlement":
                    returnValue = Entitlement?.ToString() ?? "";
                    break;
                case "EntitlementType":
                    returnValue = EntitlementType.ToString() ?? "";
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
