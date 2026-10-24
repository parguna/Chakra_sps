using DeepIdentity.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIdentity.Facade
{
    public class HouseKeepingFacade
    {
        HouseKeepingDA _housekeepingDA;

        public HouseKeepingFacade()
        {
            _housekeepingDA = new HouseKeepingDA();
        }

        public int PurgeAllTasksData(int NumberOfDays)
        {
            try
            {
                return _housekeepingDA.PurgeAllTasksData(NumberOfDays);
            }

            catch (Exception)
            {
                throw;
            }
        }

        public int PurgeAllAttestationData(int NumberOfDays)
        {
            try
            {
                return _housekeepingDA.PurgeAllAttestationData(NumberOfDays);
            }

            catch (Exception)
            {
                throw;
            }
        }

        public int PurgeAllLogsData(int NumberOfDays)
        {
            try
            {
                return _housekeepingDA.PurgeAllLogsData(NumberOfDays);
            }

            catch (Exception)
            {
                throw;
            }
        }
    }
}
