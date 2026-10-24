using DI.Chakra.DataAccess;
//using DI.Chakra.Model.Core.House_Keeping;
using DI.Chakra.Model.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Facade
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

        //public List<DI.Chakra.Model.Core.House_Keeping.PurgeConfig> GetAllPurgeConfigData()
        //{
        //    try
        //    {
        //        return _housekeepingDA.GetAllPurgeConfigData();
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public PurgeConfig GetPurgeConfigDatabyid(int id)
        //{
        //    try
        //    {
        //        return _housekeepingDA.GetPurgeConfigDatabyid(id);
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //public int SavePurgemode(PurgeConfig purgconfig)
        //{
        //    try
        //    {
        //       return  _housekeepingDA.SavePurgemode(purgconfig);
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        // public bool DeletePurgeConfig(int ID)
        // {
        //    try
        //    {
        //        return _housekeepingDA.DeletePurgeConfig(ID);
        //    }

        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        // public bool IsAlreadyExists(string _tablename)
        // {
        //     return _housekeepingDA.IsAlreadyExists(_tablename);
        // }
    }
}
