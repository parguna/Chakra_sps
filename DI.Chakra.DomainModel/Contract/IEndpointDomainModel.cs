namespace DI.Chakra.DomainModel
{
    public interface IEndpointDomainModel
    {
        IEndpointUserInfo GetEndpointUserInfo(long EndpointUserId);
    }
}
