using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using DI.Chakra.Model.ReportData;

namespace DI.Chakra.Model
{
    [Table("DI_LNS_Users")]
    public class LotusUserInfo : IEndpointUserInfo, IAggregateRoot
    {

        public LotusUserInfo()
        {
            this.LotusGroupInfos = new List<LotusGroupInfo>();
            this.Groups = new List<string>();
        }

        [Key]
        public long Id { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string ListNameFullName { get; set; }
        public string FullName { get; set; }
        public string ListName { get; set; }
        public string Department { get; set; }
        public string CompanyName { get; set; }
        public string Owner { get; set; }
        public string JobTitle { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public string Members { get; set; }
        public string MailServer { get; set; }
        public string MailFile { get; set; }
        public string MailDomain { get; set; }
        public string MailAddress { get; set; }
        public string ShortName { get; set; }
        public string Location { get; set; }
        public string EncryptIncomingMail { get; set; }
        public string CalendarDomain { get; set; }
        public string NetUserName { get; set; }
        public string HTTPPassword { get; set; }
        public string CertThumbprint { get; set; }
        public string InternetAddress { get; set; }
        public string NewMailServer { get; set; }
        public string NewMailFile { get; set; }
        public string AltFullName { get; set; }
        public string MessageStorage { get; set; }
        public string ServerName { get; set; }
        public string AltFullNameLanguage { get; set; }
        public string QuickPlace { get; set; }
        public string SametimeServer { get; set; }
        public string Type { get; set; }
        public string MailSystem { get; set; }
        public string AdminpOldWebName { get; set; }
        public string HTTPPasswordChangeDate { get; set; }
        public string HTTPPasswordForceChange { get; set; }
        public string Policy { get; set; }
        public string LTPA_UsrNm { get; set; }
        public string DB2UserName { get; set; }
        public string OfficePhoneNumber { get; set; }
        public string OfficeFAXPhoneNumber { get; set; }
        public bool AvailableForDirSync { get; set; }
        public bool MailExternalAccess { get; set; }
        public bool MailChargeable { get; set; }
        public string Comment { get; set; }
        public bool IDMPolicy { get; set; }
        public string NRIC { get; set; }
        public int PwdResetCnt { get; set; }
        public string CryptoCap { get; set; }
        public long? DIUserId { get; set; }
        public string IfUnmappedMapTo { get; set; }
        public string MappingType { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public List<string> Groups;
        public string Password;
        public string CertifierIDFile;
        public string CertifierIDPwd;
        public string UserIDFile;
        public int MailQuota;
        public int MailQuotaThreshhold;
        public string TargetCertifier;
        public int Version { get; set; }
        public string[] ReplicaServer;

        public AccountType AccountType { get; set; }
        public LockScenario LockScenario { get; set; }
        public bool IsLocked { get; set; }
        
        public virtual IList<LotusGroupInfo> LotusGroupInfos { get; set; }
        public virtual EndPoint EndPointSystem { get; set; }

        public override string ToString()
        {
            //return this.name;
            return "";
        }

        public string AccountName
        {
            get { return this.InternetAddress; }
        }

        public string AccountPassword
        {
            get { return this.Password; }
        }
        public List<IEndpointEntitlementInfo> Entitlements
        {
            get
            {
                //Start:V5 Changes on 17.11.2015 by Yasila.H
                //return (List<IEndpointEntitlementInfo>)LotusGroupInfos;
                List<IEndpointEntitlementInfo> listOfEntitlement = new List<IEndpointEntitlementInfo>();
                foreach (var lnsGroupInfo in LotusGroupInfos)
                    listOfEntitlement.Add(lnsGroupInfo);
                return listOfEntitlement;
                //End:V5 Changes on 17.11.2015 by Yasila.H
            }
        }
        public string DisplayName
        {
            get
            {
                return this.FullName;
            }
        }
    }
}
