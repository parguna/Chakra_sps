using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DI.Chakra.Model.CustomEntities
{
    public class BusinessRole : IComparable<BusinessRole>
    {
        public static readonly BusinessRole Empty = new BusinessRole("General");
                
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// asdasdsa
        public BusinessRole()
        {
            Name = Empty.Name;
        }

        public BusinessRole(string _name)
        {
            Name = _name;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }



        #region IComparable<BusinessRole> Members

        public int CompareTo(BusinessRole other)
        {
            return this.CompareTo(other);
        }

        #endregion
    }
}
