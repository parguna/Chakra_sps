using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Model
{
    [Table("DI_M_MySqlConfiguration")]
    public class MySqlConfiguration : DatabaseConfiguration
    {
        [Key]
        public long Id { get; set; }
        public string ServerName { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual EndPoint EndpointSystem { get; set; }
    }
}
