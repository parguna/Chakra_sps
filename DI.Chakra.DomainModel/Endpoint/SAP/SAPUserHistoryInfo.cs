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
    public class SAPUserHistoryInfo : DomainModelBase<SAPUserHistoryInfo, EntityModel.SAPUserHistoryInfo>
    {
        public int UserId { get; set; }

        

        public long EndPointSystemId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Initials { get; set; }

        public string Department { get; set; }

        public string E_Mail { get; set; }

        public string Pers_No { get; set; }

        public string Tel1_Numbr { get; set; }

        public string Function0 { get; set; }

        public string Title { get; set; }

        public string City { get; set; }

        public string Namcountry { get; set; }

        public string Pboxcit_No { get; set; }

        public string Postl_Cod1 { get; set; }

        public string Street { get; set; }

        public string Start_Menu { get; set; }

        public string ActivityGroup { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string UserLogonName { get; set; }

        public string AccntLockNoPwd { get; set; }

        public string AccntLocDurPwd { get; set; }

        public string LOCKEDBYINTRUDER { get; set; }

        public string EMPLOYEEID { get; set; }

        public string EMPLOYEETYPE { get; set; }

        public DateTime? PASSWORDEXPTIME { get; set; }

        public string LOGINDISABLED { get; set; }

        public bool IsActive { get; set; }

        public string DomainName { get; set; }

        public int TargetSysId { get; set; }

        public int PwdResetCnt { get; set; }

        public int? DIUserId { get; set; }

        public bool SpecialAccount { get; set; }

        public DateTime? IACMLastPasswordChange { get; set; }

        public string Position { get; set; }

        public string CostCenter { get; set; }

        public DateTime? validFromDate { get; set; }

        public DateTime? validToDate { get; set; }

        public string SapUserType { get; set; }

        public bool IsUserLocked { get; set; }

        public string MappingType { get; set; }

        public string IfUnmappedMapTo { get; set; }

        public int Version { get; set; }

        public bool IsLocked { get; set; }

        public AccountType AccountType { get; set; }

        public LockScenario LockScenario { get; set; }

        public bool NeedEPGroupAssignment { get; set; }

        public string AuditUser { get; set; }

        public DateTime AuditDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


        public static IList<SAPUserHistoryInfo> GetByEndpointId(long endPointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SAPUserHistoryInfo>(dataAccessTransaction).Get(historyInfo => historyInfo.EndPointSystemId == endPointSystemId).ToList());
            }
        }
    }
}
