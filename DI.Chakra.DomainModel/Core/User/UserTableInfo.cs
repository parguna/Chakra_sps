namespace DI.Chakra.DomainModel
{
    public class UserTableInfo: DomainModelBase<UserTableInfo, EntityModel.UserTableInfo>
    {
        public string TableName { get; set; }
        public string Columns { get; set; }
        public string Caption { get; set; }
        public string ColumnType { get; set; }
    }
}
