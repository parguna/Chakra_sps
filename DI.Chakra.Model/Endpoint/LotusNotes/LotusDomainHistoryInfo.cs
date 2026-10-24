using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Infrastructure;

namespace DI.Chakra.Model
{
    public class LotusDomainHistoryInfo : IAggregateRoot
    {
        public int Id { get; set; }
        public int HistoryId { get; set; }
        public string TenantId { get; set; }
        public long EndPointSystemId { get; set; }
        public string Domain { get; set; }
        public string NextDomain { get; set; }
        public string MailServer { get; set; }
        public string MailFile { get; set; }
        public string Allow { get; set; }
        public string Deny { get; set; }
        public string Country { get; set; }
        public string ADMD { get; set; }
        public string PRMD { get; set; }
        public string Org { get; set; }
        public string OrgUnit1 { get; set; }
        public string OrgUnit2 { get; set; }
        public string OrgUnit3 { get; set; }
        public string OrgUnit4 { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string CalendarServer { get; set; }
        public string CalendarSystem { get; set; }
        public string DefaultGlobalDomain { get; set; }
        public string LocalPrimaryDomain { get; set; }
        public string LocalDomainAliases { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? Version { get; set; }

        public static LotusDomainHistoryInfo CreateLotusDomainHistoryInfo()
        {
            LotusDomainHistoryInfo lotusDomainHistoryInfo = new LotusDomainHistoryInfo();

            return lotusDomainHistoryInfo;
        }
    }
}
