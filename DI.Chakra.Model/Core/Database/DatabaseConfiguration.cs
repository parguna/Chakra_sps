using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Infrastructure.Enums;

namespace DI.Chakra.Model
{
    public abstract class DatabaseConfiguration
    {
        public DatabaseTypes DatabaseType { get; set; }
    }
}
