using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AdHocSAPHANARequest : IEntitlementDetail
    {
        public string DisplayAccountName { get; set; }
        public long EndpointUserId { get; set; }
        public string ApplicationName { get; set; }
        public string PositionPrefix { get; set; }
        public string Module { get; set; }
        public string SubModule { get; set; }
        public string Position { get; set; }
        public string PositionValue { get; set; }
        public string Roles { get; set; }
        public bool IsReallocate { get; set; }
        public AdHocSAPSharedAccount AdHocSAPHANASharedAccount { get; set; }

        public string Entitlement
        {
            get
            {
                return string.IsNullOrEmpty(this.Position) ? this.Roles : string.IsNullOrEmpty(this.Roles) ? this.Position : this.Position + "," + this.Roles;
            }
        }

        public ISharedAccountDetail SharedAccountDetail
        {
            get
            {
                return AdHocSAPHANASharedAccount;
            }
        }
    }

}
