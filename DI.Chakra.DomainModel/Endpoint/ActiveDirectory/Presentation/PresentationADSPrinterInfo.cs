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
    public class PresentationADSPrinterInfo : ADSPrinterInfo
    {
        public string DisplayName { get; set; }

        public new static List<PresentationADSPrinterInfo> GetAll()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new ADSPrinterInfoRepository(dataAccessTransaction).GetAll());
            }
        }

        public static PresentationADSPrinterInfo ORMap(EntityModel.Presentation.PresentationADSPrinterInfo PresentationADSPrinterInfoEntityModel)
        {
            if (PresentationADSPrinterInfoEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationADSPrinterInfo, PresentationADSPrinterInfo>(PresentationADSPrinterInfoEntityModel);
        }

        public static List<PresentationADSPrinterInfo> ORMapList(IList<EntityModel.Presentation.PresentationADSPrinterInfo> PresentationADSPrinterInfoEntityModels)
        {
            return PresentationADSPrinterInfoEntityModels.Select(ORMap).ToList();
        }
    }
}
