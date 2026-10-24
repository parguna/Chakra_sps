
namespace DI.Chakra.Web
{
    public interface IContextObjectRequired
    {
        long? ContextObjectId { get; set; }
        string TypeofRequest { get; set; }
    }
}
