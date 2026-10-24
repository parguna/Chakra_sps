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
    public class AdminModeItem : DomainModelBase<AdminModeItem,EntityModel.AdminModeItem>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long TabMenuId { get; set; }

        public int Number { get; set; }

        public string Icon { get; set; }

        public string Information { get; set; }

        public AdminModeItemType Type { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public string TableName { get; set; }

        public TabMenu TabMenu { get; set; }

        public static bool DoesExists(string name)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new RepositoryBase<EntityModel.AdminModeItem>(dataAccessTransaction).Any(ai => ai.Name == name);
            }
        }

        public static IList<AdminModeItem> GetAllIncludingTabMenu()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AdminModeItem>(dataAccessTransaction).GetAll(ai=> ai.TabMenu).ToList());
            }
        }

        public static IList<AdminModeItem> GetAllIncludingPageUrl()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.AdminModeItem>(dataAccessTransaction).GetAll(ai => ai.TabMenu.WebPage.PageUrl).ToList());
            }
        }

    }
}
