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
    public class PresentationInhouseSystem
    {
        public long? DIUserID { get; private set; }
        public string UserName { get; private set; }
        public string SystemCode { get; private set; }
        public string SystemName { get; private set; }
        public string Permission { get; private set; }

        //public new static List<PresentationInhouseSystem> GetByDIUserId(long diuserid)
        //{
        //    using (var dataAccessTransaction = new DataAccessTransaction())
        //    {
        //        return ORMapList(new InhouseSystemsRepository(dataAccessTransaction).GetByDIUserID(diuserid));
        //    }
        //}

        public static PresentationInhouseSystem ORMap(EntityModel.Presentation.PresentationInhouseSystem presentationTabMenuEntityModel)
        {
            if (presentationTabMenuEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationInhouseSystem, PresentationInhouseSystem>(presentationTabMenuEntityModel);
        }

        public static List<PresentationInhouseSystem> ORMapList(IList<EntityModel.Presentation.PresentationInhouseSystem> presentationInhouseGridEntityModels)
        {
            return presentationInhouseGridEntityModels.Select(ORMap).ToList();
        }
    }
}
