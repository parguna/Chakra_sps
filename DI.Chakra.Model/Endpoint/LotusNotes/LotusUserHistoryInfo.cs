using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DI.Chakra.Infrastructure.Enums;
namespace DI.Chakra.Model
{
    [Table("DI_LNS_UsersHistory")]
    public class LotusUserHistoryInfo : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }

        public int UserId { get; set; }
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
        public int? DIUserId { get; set; }
        public string IfUnmappedMapTo { get; set; }
        public string MappingType { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }
        public int Version { get; set; }

        public AccountType AccountType { get; set; }
        public LockScenario LockScenario { get; set; }
        public bool IsLocked { get; set; }

        public string AuditUser { get; set; }
        public DateTime AuditDate { get; set; }

    }
}
