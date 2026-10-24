using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DI.Chakra.EntityModel
{
    [Table("DI_M_MySqlConfiguration")]
    public class MySqlConfiguration : EntityBase, ICreatable, IUpdatable
    {
        public string ServerName { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public EndPoint EndpointSystem { get; set; }

        public DatabaseTypes DatabaseType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

    }
}
