using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
   
    [Table("DI_M_SODPolicyReport")]
    public class SODPolicyReport : EntityBase,ICreatable,IUpdatable
    {

        public Nullable<long> PolicyExecutionDefinitionId { get; set; }

        public string UniqueRunCode { get; set; }

        public DateTime RunDate { get; set; }
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

        [ForeignKey("FunctionId1")]
        public  SODFunction Function1 { get; set; }

        [ForeignKey("FunctionId2")]
        public  SODFunction Function2 { get; set; }

        [ForeignKey("FunctionItem1")]
        public  SODFunctionItem FunctionObjItem1 { get; set; }

        [ForeignKey("FunctionItem2")]
        public  SODFunctionItem FunctionObjItem2 { get; set; }

        [ForeignKey("ConflictId")]
        public  SODConflictFunctions Conflict { get; set; }

        [ForeignKey("ExceptionId")]
        public  SODException exception { get; set; }

        [ForeignKey("PolicyExecutionDefinitionId")]
        public  SODPolicyExecutionDefinition PolicyDefinition { get; set; }

        public SODRunPolicy RunPolicy { get; set; }


        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }


    }
}
