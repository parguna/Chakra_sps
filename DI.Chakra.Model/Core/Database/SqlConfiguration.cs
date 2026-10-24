using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    [Table("DI_M_SqlConfiguration")]
    public class SqlConfiguration : DatabaseConfiguration, IEndpointConfiguration
    {
        [Key]
        public long Id { get; set; }
        public string ServerName { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SqlAuthentication { get; set; }
        public Nullable<bool> MapAll { get; set; }
        public Nullable<long> EndPointSystemId { get; set; }
        [ForeignKey("EndPointSystemId")]
        public virtual EndPoint EndpointSystem { get; set; }
    }
}
