using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    [Table("DI_SAP_UserInfo")]
    public class SAPUserinfo : EntityBase,ICreatable,IUpdatable
    {

        public SAPUserinfo()
        {
            this.SAPGroupInfo = new List<SAPGroupInfo>();
            this.SAPProfileInfos = new List<SAPProfileInfo>();
            this.SAPParameterInfo = new List<SAPParameterInfo>();
            this.SAPRoleInfo = new List<SAPRoleInfo>();
        }
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
        public DateTime? IACMLastPasswordChange { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public string Position { get; set; }
        public string CostCenter { get; set; }

        public DateTime? validFromDate { get; set; }
        public DateTime? validToDate { get; set; }
        public string SapUserType { get; set; }
        public bool IsUserLocked { get; set; }

        public bool NeedEPGroupAssignment { get; set; }

        public Dictionary<string, string> Parameters;
        public SAPUserType UserType;
        public string Password;
        public string OuputDevice;
        public string UserValidFrom;
        public string UserValidTo;
        public List<string> Roles;
        public List<string> Profiles;
        public List<string> Positions;
        public List<string> Groups;
        public DateTime StartDate;
        public DateTime EndDate;

        public EndPoint EndPointSystem { get; set; }
        public List<SAPGroupInfo> SAPGroupInfo { get; set; }
        public List<SAPProfileInfo> SAPProfileInfos { get; set; }
        public List<SAPParameterInfo> SAPParameterInfo { get; set; }
        public List<SAPRoleInfo> SAPRoleInfo { get; set; }
        public string SAPUserSNC { get; set; }
        //public List<IEndpointEntitlementInfo> Entitlements
        //{
        //    get
        //    {
        //        List<IEndpointEntitlementInfo> li = new List<IEndpointEntitlementInfo>();

        //        foreach (var item in SAPRoleInfo)
        //            li.Add(item);

        //        return li;
        //    }
        //}



        //public SAPUserinfo()
        //{
        //    this.SAPGroupInfo = new List<SAPGroupInfo>();
        //    this.SAPProfileInfos = new List<SAPProfileInfo>();
        //    this.SAPParameterInfo = new List<SAPParameterInfo>();
        //    this.SAPRoleInfo = new List<SAPRoleInfo>();

        //    this.Groups = new List<string>();
        //    this.Roles = new List<string>();
        //    this.Profiles = new List<string>();
        //    this.Positions = new List<string>();
        //    this.Parameters = new Dictionary<string, string>();
        //}

        //public override string ToString()
        //{
        //    //return this.name;
        //    return "";
        //}

        //public static SAPUserinfo CreateSAPUserinfo()
        //{
        //    SAPUserinfo SAPUserinfo = new SAPUserinfo();

        //    return SAPUserinfo;
        //}


        //public string AccountName
        //{
        //    get
        //    {
        //        return this.UserLogonName;
        //    }
        //}

        //public string DisplayName
        //{
        //    get
        //    {
        //        return (this.Firstname + " " + this.Lastname).Trim();
        //    }
        //}

        //public string AccountPassword
        //{
        //    get { return this.Password; }
        //}
    }
}
