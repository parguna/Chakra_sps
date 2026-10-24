using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_UserTableInfo")]
    public class UserTableInfo: EntityBase
    {
        public string TableName { get; set; }
        public string Columns { get; set; }
        public string Caption { get; set; }
        public string ColumnType { get; set; }
    }
}
