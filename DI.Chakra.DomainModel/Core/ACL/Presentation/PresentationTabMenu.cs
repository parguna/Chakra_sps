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
    public class PresentationTabMenu : TabMenu
    {
        public string ParentMenuName { get; private set; }
        public string PageName { get; private set; }

        public new static List<PresentationTabMenu> GetAll()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TabMenuRepository(dataAccessTransaction).GetAll());
            }
        }

        public static PresentationTabMenu ORMap(EntityModel.Presentation.PresentationTabMenu presentationTabMenuEntityModel)
        {
            if (presentationTabMenuEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationTabMenu, PresentationTabMenu>(presentationTabMenuEntityModel);
        }

        public static List<PresentationTabMenu> ORMapList(IList<EntityModel.Presentation.PresentationTabMenu> presentationTabMenuEntityModels)
        {
            return presentationTabMenuEntityModels.Select(ORMap).ToList();
        }
    }
}
