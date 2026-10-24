
namespace DI.Chakra.Model
{
    public class AdHocInhouseSystemRequest : IEntitlementDetail
    {
        public long EndPointUserID { get; set; }
        public string Entitlement { get; set; }
        public string SharedAccountName
        {
            get
            {
                return "";
            }
        }
    }
}
