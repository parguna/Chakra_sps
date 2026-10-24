using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class MetaData : DomainModelBase<MetaData, EntityModel.MetaData>
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string Type { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public static IList<MetaData> GetMetadata(string MetadataType)
        {
            IList<MetaData> lst = null;

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                if (MetadataType == "ARPR")
                {
                    lst = ORMapList(new MetaDataRepository(dataAccessTransaction).GetAccessRequestProgress());
                }
                else if (MetadataType == "ARSO")
                {
                    lst = ORMapList(new MetaDataRepository(dataAccessTransaction).GetAccessRequestSource());
                }
                else if (MetadataType == "ARST")
                {
                    lst = ORMapList(new MetaDataRepository(dataAccessTransaction).GetAccessRequestStatus());
                }

                else if (MetadataType == "ENST")
                {
                    lst = ORMapList(new MetaDataRepository(dataAccessTransaction).GetEndpointsystemtype());
                }
                return lst;
            }
        }

        public static void UpdateMetadata(MetaData presentationMetaData,string currentUser)
        {
            if (presentationMetaData.Type == "ARPR")
            {
                AccessRequestProgressTBD accessRequestProgressClass = new AccessRequestProgressTBD() { Id = (long)(DI.Chakra.Infrastructure.Enums.AccessRequestProgress)presentationMetaData.Id, Description = presentationMetaData.Description, IsActive = presentationMetaData.IsActive };
                accessRequestProgressClass.Save(currentUser);
            }
            if (presentationMetaData.Type == "ARSO")
            {
                AccessRequestSourceTBD accessRequestSourceClass = new AccessRequestSourceTBD() { Id = (long)(DI.Chakra.Infrastructure.Enums.AccessRequestSource)presentationMetaData.Id, Description = presentationMetaData.Description, IsActive = presentationMetaData.IsActive };
                accessRequestSourceClass.Save(currentUser);
            }
            if (presentationMetaData.Type == "ARST")
            {
                AccessRequestStatusTBD accessRequestStatusClass = new AccessRequestStatusTBD() { Id = (long)(DI.Chakra.Infrastructure.Enums.AccessRequestStatus)presentationMetaData.Id, Description = presentationMetaData.Description, IsActive = presentationMetaData.IsActive };
                accessRequestStatusClass.Save(currentUser);
            }
        }
    }
}
