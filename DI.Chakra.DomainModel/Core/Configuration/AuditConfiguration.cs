using System;
using System.Linq;
using System.Collections.Generic;
using DI.Chakra.Transaction;
using DI.Chakra.Repository;
using DI.Chakra.DBClient;
using TrackerEnabledDbContext.Common.Models;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;
using System.Data;
using System.Reflection;

namespace DI.Chakra.DomainModel
{
    [Serializable]
    public class AuditConfiguration : DomainModelBase<AuditConfiguration, EntityModel.AuditConfiguration>
    {
        public string TableName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<DateTime> LastModifiedDate { get; set; }

        public List<AuditConfigurationDetail> AuditConfigurationDetails { get; set; }


        #region AuditLog
        //As AuditLog model is created by Third party dll by default, that can't be updated, 
        //Since AuditLog related methods are updated below

        private const string entityPrefix = "DI.Chakra.EntityModel";

        private static IACMDBContext _db;

        protected static IACMDBContext DBContext
        {
            get { return _db ?? (_db = new IACMDBContext()); }
        }
        protected static TrackerEnabledDbContext.TrackerContext TrackerDBContext
        {
            get { return _db ?? (_db = new IACMDBContext()); }
        }
        public static IList<string> GetAllTableList()
        {
            IList<string> tableList = new List<string>();
            List<AuditLog> auditDetailList = TrackerDBContext.AuditLog.ToList();
            foreach(AuditLog data in auditDetailList){
                tableList.Add(data.TypeFullName.Split('.').Last());
            }
            return tableList.Distinct().ToList();
        }
        public static List<AuditLogDetail> GetAuditLogDetail(long auditLogId)
        {
            List<AuditLogDetail> auditDetailList = TrackerDBContext.LogDetails.Where(AD => AD.AuditLogId == auditLogId).ToList();
            return auditDetailList;
        }
        public static IList<string> GetTableList(string tableName, DateTime? filterDate, string userName)
        {
            IList<string> getTableList = new List<string>();
            IList<AuditLog> auditLogList = TrackerDBContext.AuditLog.ToList();

            if (!String.IsNullOrEmpty(tableName))
            {
                auditLogList = auditLogList.Where(x => x.TypeFullName.Split('.').Last().IndexOf(tableName, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            if (filterDate != null)
            {
                DateTime toFilterDate = (DateTime)filterDate;
                auditLogList = auditLogList.Where(x => x.EventDateUTC.Date == toFilterDate.Date).ToList();
            }
            if (!string.IsNullOrEmpty(userName))
            {
                auditLogList = auditLogList.Where(x => x.UserName.ToLower().Contains(userName.ToLower())).ToList();
            }

            getTableList = auditLogList.Select(x => x.TypeFullName).Distinct().ToList();
            if (getTableList.Count > 0)
            {
                getTableList = getTableList.Select(x => x.Split('.').Last()).ToList();
            }
            return getTableList;
        }
        public static IList<AuditLog> GetLogs(string entityName, DateTime? filterDate, string userName)
        {
            IList<AuditLog> auditLogList = DBContext.GetLogs(entityName).ToList();
            if (filterDate != null)
            {
                DateTime toFilterDate = (DateTime)filterDate;
                auditLogList = auditLogList.Where(x => x.EventDateUTC.Date == toFilterDate.Date).ToList();
            }
            if (!string.IsNullOrEmpty(userName))
            {
                auditLogList = auditLogList.Where(x => x.UserName.ToLower().Contains(userName.ToLower())).ToList();
            }
            return auditLogList;
        }

        public static Dictionary<string, List<string>> GetEntitiesToAudit()
        {
            Dictionary<string, List<string>> entitiesToAudit = new Dictionary<string, List<string>>();
            using (var transaction = new DataAccessTransaction())
            {
                List<AuditConfiguration> configurationList = ORMapList(new RepositoryBase<EntityModel.AuditConfiguration>(transaction)
                .GetAll(true).ToList());
                foreach (AuditConfiguration data in configurationList)
                {
                    List<string> propertyList = data.AuditConfigurationDetails.Select(ad => ad.PropertyName).ToList();
                    entitiesToAudit.Add(data.TableName, propertyList);
                }
                return entitiesToAudit;
            }
        }
        #endregion

        public static AuditConfiguration GetByTableName(string tableName)
        {
            using (var transaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.AuditConfiguration>(transaction)
                    .Get(query => query.TableName == tableName, true).FirstOrDefault());
            }
        }

        public override long Save(string currentUser)
        {
            bool isNew = this.IsNew;
            using (DataAccessTransaction transaction = new DataAccessTransaction(currentUser))
            {
                base.Save(transaction);
                if (!isNew && (this.AuditConfigurationDetails != null))
                {
                    List<AuditConfigurationDetail> dbConfigDetail = AuditConfiguration.GetById(this.Id, true).AuditConfigurationDetails;

                    List<AuditConfigurationDetail> newConfigDetail = dbConfigDetail.Except(this.AuditConfigurationDetails, new IdBaseEqualityComparer<AuditConfigurationDetail>()).ToList();

                    newConfigDetail.ForEach(CD => CD.Delete(transaction));

                    this.AuditConfigurationDetails.ForEach(CD => CD.Save(transaction));

                    if (this.AuditConfigurationDetails.Count == 0)
                    {
                        base.Delete(transaction);
                    }
                }
                transaction.Commit();
                transaction.dbContextRefresh();
            }
            return 0;
        }

        public static DataTable GetAuditLogRecordDetail(long auditLogId)
        {
            AuditLog auditLog = TrackerDBContext.AuditLog.Where(x => x.AuditLogId == auditLogId).FirstOrDefault();
            if (auditLog != null)
            {
                var t = Assembly
                              .GetExecutingAssembly()
                              .GetReferencedAssemblies()
                              .Select(x => Assembly.Load(x))
                              .SelectMany(x => x.GetTypes()).First(x => x.FullName == auditLog.TypeFullName);

                var tableName = t.CustomAttributes.FirstOrDefault().ConstructorArguments.FirstOrDefault().Value;
                string query = string.Format("Exec DI_SP_GetAuditTableWithouFile @tableName= {0}, @id={1} ", tableName, auditLog.RecordId);
                using(var tran = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.AuditConfiguration>(tran).GetDataTableByQuery(query);
                }
            }

            return new DataTable();
        }

    }
}
