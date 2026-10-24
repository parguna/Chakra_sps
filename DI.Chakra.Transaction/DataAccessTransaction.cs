using DI.Chakra.DBClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Transaction
{
    public class DataAccessTransaction : IDisposable
    {
        private IACMDBContext context;
        private DbContextTransaction transaction;
        public DateTime transactionDateTime;
        public string User;

        public DataAccessTransaction(bool isTransaction)
        {
            this.context = new IACMDBContext();
            //if (isTransaction)
            //{
            //    transaction = this.context.Database.BeginTransaction();
                  transactionDateTime = DateTime.Now;
            //}
        }
        public DataAccessTransaction()
        {
            this.context = new IACMDBContext();
            //transaction = this.context.Database.BeginTransaction();
            transactionDateTime = DateTime.Now;
        }
        public DataAccessTransaction(string currentUser)
        {
            this.context = new IACMDBContext();
            //transaction = this.context.Database.BeginTransaction();
            transactionDateTime = DateTime.Now;
            User = currentUser;
        }


        public IACMDBContext Context
        {
            get
            {
                return this.context;
            }
        }

        public void Commit()
        {
            //if(transaction != null)
            //    transaction.Commit();
        }

        public void dbContextRefresh()
        {
            IACMDBContext oDBContext = new IACMDBContext();
            oDBContext.DataAudit();
        }

        public void Rollback()
        {
            //if(transaction != null)
            //    transaction.Rollback();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //if(transaction != null)
                    //{
                    //    transaction.Dispose();
                    //}
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
