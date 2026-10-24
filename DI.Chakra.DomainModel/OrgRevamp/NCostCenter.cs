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
    public partial class NCostCenter : DomainModelBase<NCostCenter, EntityModel.NCostCenter>
    {
        public string Code { get; set; }

        public long CompanyId { get; set; }
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public static NCostCenter GetByCode(string code)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new NCostCenterRepository(dataAccessTransaction).Get(a => a.Code == code, true).FirstOrDefault());
            }
        }

        public static NCostCenter GetByCodeAndCompany(string code, long companyId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new NCostCenterRepository(dataAccessTransaction).Get(a => a.Code == code && a.CompanyId == companyId).FirstOrDefault());
            }
        }

        public static Dictionary<long, string> GetNCostCenters(string searchString)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return new NCostCenterRepository(dataAccessTransaction).GetNCostCenters(CheckNull.AsteriskToString(searchString));
            }
        }
    }
}
