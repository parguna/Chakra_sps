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
    [Table("DI_M_User")]
    public class User : IAggregateRoot, IComparer<User>
    {

        public User()
        {
            this.Groups = new List<Group>();
        }

        [Key]
        public long Id { get; set; }
        public string NRIC { get; set; }
        public string FIN { get; set; }
        public string PassportNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TenantId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string KnownAs { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<bool> FullTimeEmployee { get; set; }
        

        public string EmailId { get; set; }
        public string TelephoneNumber { get; set; }
        public string HomephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Fax { get; set; }

        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }

        public Nullable<System.DateTime> JoinDate { get; set; }
        public Nullable<System.DateTime> RehireDate { get; set; }
        public Nullable<System.DateTime> TerminationDate { get; set; }

        public Nullable<bool> IsActive { get; set; }
        public string Ethinicity { get; set; }
        public string Country { get; set; }
        public EmployeeType EmployeeType { get; set; }

        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<long> OrganisationId { get; set; }
        public Nullable<long> BusinessRoleId { get; set; }
        public Nullable<long> ManagerId { get; set; }
        public string AuthenticationMode { get; set; }
       
        public string Reserved1 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved7 { get; set; }
        public string Reserved8 { get; set; }
        public string Reserved9 { get; set; }
        public string Reserved10 { get; set; }

        
        public Nullable<DateTime> LastLogonDate { get; set; }
        public int SortDirection;

        [XmlIgnore]
        public virtual List<Group> Groups { get; set; }
        public virtual List<Role> Roles { get; set; }

        //[ForeignKey("CompanyId")]
        //public virtual Company Company { get; set; }
        //[ForeignKey("DepartmentId")]
        //public virtual Department Department { get; set; }
        //[ForeignKey("DivisionId")]
        //public virtual Division Division { get; set; }
        //public bool NeedEmailUpdateToHR { get; set; }
        //public bool NeedFLINSLoginIdUpdateToHR { get; set; }
        //public string CostCenter { get; set; }
        //public string CostCenterText { get; set; }
        //public string CostDistribution { get; set; }
        //public string CompanyCode { get; set; }
        //public string CompanyName { get; set; }
        //public string DepartmentCode { get; set; }
        //public string DepartmentName { get; set; }
        //public string DivisionCode { get; set; }
        //public string DivisionName { get; set; }
        //public string SectionCode { get; set; }
        //public string SectionName { get; set; }
        //public string PositionCode { get; set; }
        //public string PositionName { get; set; }
        //public string Supervisor { get; set; }
        //public string OrgCompany { get; set; }
        //public string OrgDepartment { get; set; }
        //public string OrgDivision { get; set; }
        //public string OrgBranch { get; set; }
        //public string OrgSection { get; set; }
        //public string OneKeyTokenId { get; set; }
        //public string FLINSLoginId { get; set; }
        //public string EEGroup { get; set; }
        //public string EmployeeStatus { get; set; }
        //public bool IsExecutiveEmployee { get; set; }
        //public Nullable<long> CompanyId { get; set; }
        //public Nullable<long> DepartmentId { get; set; }
        //public Nullable<long> DivisionId { get; set; }
        

        [ForeignKey("OrganisationId")]
        public virtual Organisation Organisation { get; set; }

        public virtual Designation BusinessRole { get; set; }

        public string UserExtension { get; set; }

        public UserExtension UserExtensionObj
        {
            get
            {
                if (string.IsNullOrEmpty(UserExtension))
                    return null;

                return UtilityMethods.Deserialize<UserExtension>(UserExtension);
            }
            
        }

        public string RulesetOutput;

        public override string ToString()
        {
            return this.FirstName + this.LastName;
        }

        public static User CreateUser()
        {
            User user = new User();

            user.JoinDate = null;
            user.RehireDate = null;
            user.TerminationDate = null;
            user.CreatedDate = DateTime.Now;
            user.LastModifiedDate = DateTime.Now;
            return user;
        }

        /// <summary>
        /// Checks if the given instance is equals to the current one.
        /// </summary>
        /// <param name="obj">The object instance to check.</param>
        /// <returns>True if they equals, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is User)
            {
                User other = (User)obj;
                return this.Id.Equals(other.Id);
            }
            else
                return false;
        }

        /// <summary>
        /// Gets the hash code of the category.
        /// </summary>
        /// <returns>The category hash code.</returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.DisplayName.GetHashCode();
        }

        #region IComparer<Role> Members

        public int Compare(User x, User y)
        {
            int cmprValue = -1;
            if (x != null && y != null)
            {
                if (SortDirection == 0)
                {
                    cmprValue = string.Compare(x.UserName, y.UserName);

                }
                else
                {
                    cmprValue = string.Compare(y.UserName, x.UserName);
                }
            }

            return cmprValue;
        }

        #endregion

    }
}

