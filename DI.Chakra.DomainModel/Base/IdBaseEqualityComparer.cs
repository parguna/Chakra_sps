using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class IdBaseEqualityComparer<T> : EqualityComparer<T>
        where T : IdBase
    {
        public override bool Equals(T tModel1, T tModel2)
        {
            if (tModel1.Id == tModel2.Id)
                return true;
            else
                return false;
        }

        public override int GetHashCode(T tModel)
        {
            return tModel.Id.GetHashCode();
        }
    }
}
