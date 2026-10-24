
namespace DI.Chakra.Infrastructure.Enums
{
    public enum PasswordSource : byte
    {
        [Description("None")]
        None = 0,

        [Description("Expression")]
        Expression = 1,

        [Description("Policy")]
        Policy = 2,
       
    }
}
