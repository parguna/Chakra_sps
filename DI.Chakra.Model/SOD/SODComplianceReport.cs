using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Serializable]
    [Table("DI_M_SODReport")]
    public class SODReport
    {
        [Key]
        public long Id { get; set; }

        public Nullable<int> SODPolicyID { get; set; }
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

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string RunCode { get; set; }

        public bool PassedObjectLevelLeft { get; set; }
        public bool PassedObjectLevelRight { get; set; }

        public int TotalMatching { get; set; }
        public string MitigationCode { get; set; }
        public Nullable<long> SODRunPolicyId { get; set; }
        public long? ExceptionId { get; set; }

        [ForeignKey("FunctionId1")]
        public virtual SodFunction Function1 { get; set; }
        [ForeignKey("FunctionId2")]
        public virtual SodFunction Function2 { get; set; }

        [ForeignKey("FunctionItem1")]
        public virtual SodFunctionItem FunctionObjItem1 { get; set; }
        [ForeignKey("FunctionItem2")]
        public virtual SodFunctionItem FunctionObjItem2 { get; set; }

        [ForeignKey("ConflictId")]
        public virtual SODConflictFunction Conflict { get; set; }

        [ForeignKey("ExceptionId")]
        public virtual SODException exception { get; set; }
        public virtual SODRunPolicy RunPolicy { get; set; }


        public SODReport()
        {
            AuthorizationCode1 = string.Empty;
            AuthorizationCode2 = string.Empty;
        }

       

        public SODReport(SODReport report)
        {
            this.AuthorizationCode1 = report.AuthorizationCode1;
            this.AuthorizationCode2 = report.AuthorizationCode2;
            this.Conflict = report.Conflict;
            this.ConflictId = report.ConflictId;
            this.Function1 = report.Function1;
            this.Function2 = report.Function2;
            this.FunctionId1 = report.FunctionId1;
            this.FunctionId2 = report.FunctionId2;
            this.FunctionItem1 = report.FunctionItem1;
            this.FunctionItem2 = report.FunctionItem2;
            this.FunctionObjItem1 = report.FunctionObjItem1;
            this.FunctionObjItem2 = report.FunctionObjItem2;
            this.FunctionType1 = report.FunctionType1;
            this.FunctionType2 = report.FunctionType2;
            this.PassedObjectLevelLeft = report.PassedObjectLevelLeft;
            this.PassedObjectLevelRight = report.PassedObjectLevelRight;
            this.Role1 = report.Role1;
            this.Role2 = report.Role2;
            this.Id = report.Id;
            this.UserId = report.UserId;
            this.UserName = report.UserName;
        }


    }

}
