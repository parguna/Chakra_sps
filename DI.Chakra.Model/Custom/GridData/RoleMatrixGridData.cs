using System;

namespace DI.Chakra.Model
{
    public class RoleMatrixGridData
    {
        public long RoleMatrixId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyCode { get; set; }
        public string Company { get; set; }
        public string CostCenter { get; set; }
        public int CostCenterId { get; set; }
        public int DepartmentId { get; set; }
        public string Department { get; set; }
        public int DivisionId { get; set; }
        public string Division { get; set; }
        public int OrganisationId { get; set; }
        public string Organisation { get; set; }
        public string SectionName { get; set; }
        public int BusinessRoleId { get; set; }
        public string BusinessRole { get; set; }
        public string EmployeeTerm { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
