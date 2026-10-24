using DI.Chakra.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Dto
{
    [Serializable]
    public class AdHocNovaRequest : IEntitlementDetail
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public List<AdHocNovaRequestDetails> RequestDetails { get; set; } = new List<AdHocNovaRequestDetails>();
        public string Entitlement
        {
            get
            {
                return string.Join(",", RequestDetails.Select(i=>i.RoleName));
            }
        } 
        public AdHocSAPSharedAccount AdHocSAPSharedAccountDetail { get; set; }

        public long FlinsAccountEndpointUserId { get; set; }

        [Obsolete]
        public ISharedAccountDetail SharedAccountDetail
        {
            get { return AdHocSAPSharedAccountDetail; }
        }
    }
    [Serializable]
    public class AdHocNovaRequestDetails
    {
        public long Id { get; set; }
        public string RoleName { get; set; }
        public NovaMatrixType NovaMatrixType { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string SectionCode { get; set; }
        public string SectionName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string JobDescription { get; set; }
    }
}
