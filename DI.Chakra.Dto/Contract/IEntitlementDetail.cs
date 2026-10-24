namespace DI.Chakra.Dto
{
    public interface IEntitlementDetail
    {
        long EndpointUserId { get; set; }
        string Entitlement { get; }
        string DisplayAccountName { get; set; }
        ISharedAccountDetail SharedAccountDetail { get; }
    }

     
    public interface ISharedAccountDetail
    {
        string AccountName { get; set; } 
    }

    public interface ISharedAccountDisplay
    {
        string DisplayName { get; set; }
    }

    public interface ISharedAccountDescription
    {
        string Description { get; set; }
    }

}
