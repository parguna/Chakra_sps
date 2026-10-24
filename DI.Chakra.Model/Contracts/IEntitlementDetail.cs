using System;
namespace DI.Chakra.Model
{
    public interface IEntitlementDetail
    {
        string Entitlement { get; set; }

        string SharedAccountName { get; }
    }
}
