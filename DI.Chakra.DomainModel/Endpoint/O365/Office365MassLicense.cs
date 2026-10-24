
using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class Office365MassLicense : DomainModelBase<DomainModel.Office365MassLicense, EntityModel.Office365MassLicense>
    {
        public bool IsMassAdd { get; set; }
        public bool IsCompleted { get; set; }
        public string Licenses { get; set; }
        public long EndPointSystemId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        
        public ProvMassLicenseInfo LicenseInfoObject
        {
            get
            {
                return UtilityMethods.Deserialize<ProvMassLicenseInfo>(this.Licenses);
            }
        }
        public static Office365MassLicense GetMassLicensePropagationRequest()
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.Office365MassLicense>(transaction).Get(item => !item.IsCompleted).FirstOrDefault());
            }
        }
    }

    [Serializable]
    public class ProvMassLicenseInfo
    {
        public List<ProvMassLicenseInfoDetail> LicenseDetails { get; set; }
    }

    [Serializable]
    public class ProvMassLicenseInfoDetail
    {
        public string License { get; set; }
        public string ServiceName { get; set; }
        public bool IsRemoveLicense { get; set; }
    }
}
