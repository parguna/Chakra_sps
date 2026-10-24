using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure
{
    public class ProvisioningParamCollection : IEnumerable<ProvisioningParam>
    {
        List<ProvisioningParam> mylist = new List<ProvisioningParam>();

        public void AddAll(List<ProvisioningParam> parameters)
        {
            mylist = parameters;
        }

        public ProvisioningParam this[int index]
        {
            get { return mylist[index]; }
            set { mylist.Insert(index, value); }
        }
        public ProvisioningParam this[string Name]
        {
            get { return mylist.Find(t => t.Name == Name); }
        }

        public IEnumerator<ProvisioningParam> GetEnumerator()
        {
            return mylist.GetEnumerator();

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
