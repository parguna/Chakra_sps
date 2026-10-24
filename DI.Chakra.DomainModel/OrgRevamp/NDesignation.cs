using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public partial class NDesignation : DomainModelBase<NDesignation, EntityModel.NDesignation>
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static NDesignation GetByCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new NDesignationRepository(dataAccessTransaction).Get(a => a.Code == code, true).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetNDesignations(string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NDesignationRepository(dataAccessTransaction).GetNDesignations(CheckNull.AsteriskToString(searchString));
            }
        }
    }
}
