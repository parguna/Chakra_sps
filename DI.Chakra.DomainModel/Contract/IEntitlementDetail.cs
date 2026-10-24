using DI.Chakra.Infrastructure.Enums;
using System.Runtime.Serialization;

namespace DI.Chakra.DomainModel
{
    public interface IEntitlementDetail
    {
        long EndpointUserId { get; set; }
        string DisplayAccountName { get; set; }
        string Entitlement { get; }
        ISharedAccountDetail SharedAccountDetail { get; }
    }


    public interface ISharedAccountDetail
    {
        string AccountName { get; set; }
    }

    public interface ISAPUserType
    {
        SAPUserType SAPUserType { get; set; }
         string Description { get; set; }
    }

   
}
