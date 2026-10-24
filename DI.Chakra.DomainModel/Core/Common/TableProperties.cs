using AutoMapper;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class TableProperties
    {
        public string ColumnName { get; set; }

        public string DataType { get; set; }

        public static IList<TableProperties> GetByTableName(string tableName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new TablePropertiesRepository(dataAccessTransaction).GetByTableName(tableName));
            }
        }
        public static List<string> GetPropertyList(string tableName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return TableProperties.GetByTableName(tableName).Select(x => x.ColumnName).ToList();
            }
        }
        public static IList<string> GetTableList()
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return new TablePropertiesRepository(dataAccessTransaction).GetTableList();
            }
        }

        public static TableProperties ORMap(EntityModel.TableProperties tablePropertiesEntityModel)
        {
            if (tablePropertiesEntityModel == null)
                return null;
            return Mapper.Map<EntityModel.TableProperties, TableProperties>(tablePropertiesEntityModel);
        }

        public static List<TableProperties> ORMapList(IList<EntityModel.TableProperties> tablePropertiesEntityModel)
        {
            return tablePropertiesEntityModel.Select(ORMap).ToList();
        }

        public static int GetMaxId(string tableName, string columnName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return (new TablePropertiesRepository(dataAccessTransaction).GetMaxId(tableName, columnName));
            }
        }
    }
}
