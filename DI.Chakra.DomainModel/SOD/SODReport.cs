using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class SODReport : DomainModelBase<SODReport, EntityModel.SODReport>
    {
        public Nullable<long> SODPolicyID { get; set; }

        public Nullable<long> FunctionId1 { get; set; }

        public string Role1 { get; set; }

        public Nullable<long> FunctionItem1 { get; set; }

        public string FunctionType1 { get; set; }

        public Nullable<long> FunctionId2 { get; set; }

        public string Role2 { get; set; }

        public Nullable<long> FunctionItem2 { get; set; }

        public string FunctionType2 { get; set; }

        public string AuthorizationCode1 { get; set; }

        public string AuthorizationCode2 { get; set; }

        public string ProfileCode1 { get; set; }

        public string ProfileCode2 { get; set; }

        public string RoleCode1 { get; set; }

        public string RoleCode2 { get; set; }

        public Nullable<long> ConflictId { get; set; }

        public string ConflictCode { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string RunCode { get; set; }

        public bool PassedObjectLevelLeft { get; set; }

        public bool PassedObjectLevelRight { get; set; }

        public int TotalMatching { get; set; }

        public string MitigationCode { get; set; }

        public Nullable<long> SODRunPolicyId { get; set; }

        public long? ExceptionId { get; set; }

        public SODFunction Function1 { get; set; }

        public SODFunction Function2 { get; set; }

        public SODFunctionItem FunctionObjItem1 { get; set; }

        public SODFunctionItem FunctionObjItem2 { get; set; }

        public SODConflictFunctions Conflict { get; set; }

        public SODException exception { get; set; }

        public SODRunPolicy RunPolicy { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static List<SODReport> GetBySODRunPolicyIdIncludeSodFunctionItem(long sODRunPolicyId)
        {
            using (DataAccessTransaction tranaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.SODReport>(tranaction)
                    .Get(rept => rept.SODRunPolicyId == sODRunPolicyId, 
                    rept => rept.Function1,
                    rept => rept.Function2, 
                    rept => rept.FunctionObjItem1, 
                    rept => rept.FunctionObjItem1.EndpointType, 
                    rept => rept.FunctionObjItem1.endpoint, 
                    rept => rept.FunctionObjItem2, 
                    rept => rept.FunctionObjItem2.EndpointType,
                    rept=>rept.FunctionObjItem2.endpoint,
                    rept=>rept.Conflict,
                    rept=>rept.Conflict.SodRiskCode,
                    rept=>rept.exception,
                    rept=>rept.RunPolicy,
                    rept=>rept.RunPolicy.compliancepolicy).ToList());
            }
        }
    }
}
