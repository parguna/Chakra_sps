using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSComputerInfo : ADSComputerInfo
    {
        public string DisplayName { get; set; } 

        public new static List<PresentationADSComputerInfo> GetAll()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ADSComputerInfoRepository(dataAccessTransaction).GetAll());
            }
        }

        public static PresentationADSComputerInfo ORMap(EntityModel.Presentation.PresentationADSComputerInfo PresentationADSComputerInfoEntityModel)
        {
            if (PresentationADSComputerInfoEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSComputerInfo, PresentationADSComputerInfo>(PresentationADSComputerInfoEntityModel);
        }

        public static List<PresentationADSComputerInfo> ORMapList(IList<EntityModel.Presentation.PresentationADSComputerInfo> PresentationADSComputerInfoEntityModels)
        {
            return PresentationADSComputerInfoEntityModels.Select(ORMap).ToList();
        }
    }
}
