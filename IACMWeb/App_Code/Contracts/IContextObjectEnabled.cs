
namespace DI.Chakra.Web
{
    public interface IContextObjectEnabled
    {
        long? ContextObjectId { get; set; }
        string TypeofRequest { get; set; }
    }
}