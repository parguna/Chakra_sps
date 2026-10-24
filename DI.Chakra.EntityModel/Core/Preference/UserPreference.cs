namespace DI.Chakra.EntityModel
{

    public class UserPreference:EntityBase
    {
        public long TabMenuId { get; set; }
        public string MenuName { get; set; }
        public int Position { get; set; }
        public string TargetUrl { get; set; }
    }
}
