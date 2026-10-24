using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.EntityModel
{
    public class IdBaseEqualityComparer<T> : EqualityComparer<T>
        where T : EntityBase
    {
        public override bool Equals(T tEntity1, T tEntity2)
        {
            if (tEntity1.Id == tEntity2.Id)
                return true;
            else
                return false;
        }

        public override int GetHashCode(T tEntity)
        {
            return tEntity.Id.GetHashCode();
        }
    }
}
