using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using DI.Chakra.Infrastructure.;
using DI.Chakra.Model;
using DI.Chakra.Infrastructure;
using DI.Chakra.DataAccess;
using DI.Chakra.Infrastructure.Enums;


namespace DI.Chakra.Facade
{
    public class SchedulerFacade// : ISchedulerFacade
    {
        private SchedulerDA SchedulerDA;
        private DiscoveryProcessDA discoveryDA;
        private AttestationProcessDA attestationProcessDA;

        public SchedulerFacade()
        {
            SchedulerDA = new SchedulerDA();
            discoveryDA = new DiscoveryProcessDA();
            attestationProcessDA = new AttestationProcessDA();
        }
        #region ISchedulerFacade Members

        
        public void UpdateScheduler(Scheduler _scheduler)
        {
            SchedulerDA.UpdateScheduler(_scheduler);
        }
        public Scheduler GetScheduler(string processName, int processId)
        {
            throw new NotImplementedException();
        }

        public IList<Scheduler> GetAllSchedulers()
        {
            try
            {
                IList<Scheduler> schedulerList = this.GetAll();

                return schedulerList;

            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Scheduler GetSchedulerById(int id)
        {
            try
            {
                return SchedulerDA.GetSchedulerById(id);

            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IList<Scheduler> GetCurrentJob()
        {
            try
            {
                IList<Scheduler> schedulerList = SchedulerDA.GetCurrentJob();
                return schedulerList;

            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public IList<DiscoveryProcessHistory> GetDiscoveryHistory(int _EndPointId)
        {
            try
            {
                return discoveryDA.GetDiscoveryHistory(_EndPointId);

            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region IAssembler<Scheduler,Scheduler> Members

        
        

        public Scheduler GetByKey(object _key)
        {
            if (null == _key)
                return null;

            try
            {
                var obj = from scheduler in this.GetAll()
                          where scheduler.Id == Convert.ToInt32(_key)
                          select scheduler;

                return (Scheduler)obj;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public IList<Scheduler> GetAll()
        {
            try
            {
                SchedulerDA = new SchedulerDA();

                return SchedulerDA.GetAllScheduler();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IList<Scheduler> GetAllSchedulersByProcess(IList<string> _processName)
        {
            IList<Scheduler> retList = new List<Scheduler>();

            IList<Scheduler> allList = new List<Scheduler>();

            try
            {
                var list = this.GetAll();

                foreach (string str in _processName)
                {

                    retList = list.Where(scheduler => scheduler.ProcessName == str).ToList();

                    allList = (IList<Scheduler>)allList.Union(retList).ToList();

                }

                // return list.Where(scheduler => scheduler.ProcessName == _processName).ToList();

                return allList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        public List<T> GetCurrentJobs<T>(ProcessType jobType)
        {
            try
            {
                return SchedulerDA.GetCurrentJobs<T>(jobType);
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
