using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public interface IConfigurationMaster
    {
        long Id { get; set; }

        string ConfigKey { get; set; }

        string ConfigValue { get; set; }

        string Description { get; set; }

        string CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }

        string LastModifiedBy { get; set; }

        Nullable<DateTime> LastModifiedDate { get; set; }

        bool IsUiDisplay { get; set; }
    }
}
