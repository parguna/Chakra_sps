using DI.Chakra.Infrastructure.Enums;
using System;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class OrgChangeTrack : IOrgChangeTrack, IEmployeeEventReason
    {
        public OrgChangeTrack(EmployeeEvents employeeEvent, string EventReason, string EventCode, DateTime EffectiveDate, DateTime? endDate = null,int? eventSequence=null, string retainAccessForTransfer = "")
        {
            Event = employeeEvent;
            this.EventReason = EventReason;
            this.EventCode = EventCode;
            this.EffectiveDate = EffectiveDate;
            this.EndDate = endDate;
            EventSequence = eventSequence ?? 1;
            this.RetainAccessForTransfer = retainAccessForTransfer;
        }

        public DateTime? EndDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string KnownAs { get; set; }
        public string EmailId { get; set; }         
        public string HomephoneNumber { get; set; }        
        public string TelephoneNumber { get; set; }        
        public string MobileNumber { get; set; }
        public string Fax { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
        public string Ethinicity { get; set; }
        public string Country { get; set; }
        public EmployeeType EmployeeType { get; set; } 
        public Nullable<DateTime> JoinDate { get; set; }
        public Nullable<DateTime> RehireDate { get; set; }
        public Nullable<DateTime> TerminationDate { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
        public string CostCenter { get; set; }
        public string CostCenterText { get; set; }

        public string EmployeeId { get; set; }
        public string ROEmployeeId { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public Nullable<long> DepartmentId { get; set; }
        public Nullable<long> DivisionId { get; set; }
        public Nullable<long> OrganisationId { get; set; }
        public string CompanyCode { get; set; }
        public string DepartmentCode { get; set; }
        public string DivisionCode { get; set; }
        public string SectionCode { get; set; }
        public string CompanyName { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionName { get; set; }
        public string SectionName { get; set; }
        public string OrgCompany { get; set; }
        public string OrgDepartment { get; set; }
        public string OrgDivision { get; set; }
        public string OrgBranch { get; set; }
        public string OrgSection { get; set; }
        public string OrgUnit { get; set; }
        public string BranchCode { get; set; }
        public string UnitCode { get; set; }
        public string OrganisationCode { get; set; }


        public string PositionName { get; set; }
        public string PositionCode { get; set; }

        public DateTime EffectiveDate { get; private set; }
        public EmployeeEvents Event { get; private set; }


        public string EventCode { get; private set; }
        public string EventReason { get; private set; }

        public string EEGroup { get; set; }
        public string EmployeeStatus { get; set; }
        public bool IsExecutiveEmployee { get; set; }
        public int EventSequence { get; set; }
        public override string ToString()
        {
            return string.Format(@"  CostCenter : {0};

         CompanyId : {1};
         DepartmentId : {2};
         DivisionId : {3};
         OrganisationId : {4};

         CompanyCode : {5};
         DepartmentCode : {6};
         DivisionCode : {7};
         SectionCode : {8};

         CompanyName : {9};
         DepartmentName : {10};
         DivisionName : {11};
         SectionName : {12};

         OrgCompany : {13};
         OrgDepartment : {14};
         OrgDivision : {15};
         OrgBranch : {16};
         OrgSection : {17};
         OrgUnit : {18};
         PositionName : {19};

         EffectiveDate : {20};
         Event : {21};
         BranchCode: {22};
         UnitCode : {23};
         OrganisationCode : {24} ", CostCenter,

          CompanyId,
         DepartmentId,
        DivisionId,
         OrganisationId,

        CompanyCode,
        DepartmentCode,
        DivisionCode,
        SectionCode,
        CompanyName,
        DepartmentName,
        DivisionName,
        SectionName,
        OrgCompany,
        OrgDepartment,
        OrgDivision,
        OrgBranch,
        OrgSection,
        OrgUnit,
        PositionName,
          EffectiveDate,
          Event.ToString(), BranchCode, UnitCode, OrganisationCode);
        }
        /// <summary>
        /// Used to track Notes for transfer scenario. If notes contains Retain then it should create as Reorg trigger
        /// </summary>
        public string RetainAccessForTransfer { get; set; }
        public User GetNewUser()
        {
            var user = User.GetByEmployeeId(this.EmployeeId);

            user.CompanyId = CompanyId;
            user.DepartmentId = DepartmentId;
            user.DivisionId = DivisionId;
            user.CompanyCode = CompanyCode;
            user.DepartmentCode = DepartmentCode;
            user.DivisionCode = DivisionCode;
            user.SectionCode = SectionCode;
            user.CompanyName = CompanyName;
            user.DepartmentName = DepartmentName;
            user.DivisionName = DivisionName;
            user.SectionName = SectionName;
            user.OrgCompany = OrgCompany;
            user.OrgDepartment = OrgDepartment;
            user.OrgDivision = OrgDivision;
            user.OrgBranch = OrgBranch;
            user.OrgSection = OrgSection;
            user.OrgUnit = OrgUnit;
            user.BranchCode = BranchCode;
            user.UnitCode = UnitCode;
            user.OrganisationCode = OrganisationCode;
            user.EEGroup = EEGroup;
            user.IsExecutiveEmployee = IsExecutiveEmployee;
            user.EmployeeStatus = EmployeeStatus;
            user.IsActive = IsActive;
            user.CostCenter = CostCenter;
            user.CostCenterText = CostCenterText;
            user.PositionCode = PositionCode;
            user.PositionName = PositionName;
            user.JoinDate = JoinDate;
            user.RehireDate = RehireDate;
            user.TerminationDate = TerminationDate;
            user.ROEmployeeId = ROEmployeeId;
            user.FirstName = FirstName;
            user.MiddleName = MiddleName;
            user.LastName = LastName;
            user.DisplayName = DisplayName;
            user.KnownAs = KnownAs;
            user.EmailId = EmailId;
            user.HomephoneNumber = HomephoneNumber;
            user.TelephoneNumber = TelephoneNumber;
            user.MobileNumber = MobileNumber;
            user.Fax = Fax;
            user.PostalCode = PostalCode;
            user.PostalAddress = PostalAddress;
            user.Ethinicity = Ethinicity;
            user.Country = Country;
            user.EmployeeType = EmployeeType;

            return user;
        }


        public User GetOldUser()
        {
            return User.GetByEmployeeId(this.EmployeeId); 
        }

        public string GetComment()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(
                new EmployeeEventTriggerComment()
                {
                    EventCode = EventCode,
                    EventReason = EventReason,
                    EventType = Event.ToString(),
                    EmployeeId = EmployeeId,
                    EffectiveDate = EffectiveDate,
                    RetainAccessForTransfer = RetainAccessForTransfer
                });
        }
    }
}
