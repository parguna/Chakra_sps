using DI.Chakra.Infrastructure.Enums;
using System;
using System.ComponentModel;


namespace DI.Chakra.Dto
{
    public class SqlConfiguration : DtoBase, IDatabaseConfiguration, IEndpointConfiguration
    {
        public string ServerName { get; set; }

        public string Database { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool SqlAuthentication { get; set; }

        public Nullable<bool> MapAll { get; set; }

        public Nullable<long> EndPointSystemId { get; set; }

        public EndPoint EndpointSystem { get; set; }

        public DatabaseTypes DatabaseType { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }
    }
}
