using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DI.Chakra.Model;
using System.Data;
using System.Data.SqlClient;

using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using DI.Chakra.Logger;

namespace DI.Chakra.DataAccess
{
    public class PasswordSynchronizationDA
    {
        
        public EndPoint GetEndPointSystemByKey(int EndPointSystemId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                        return _context.EndpointSystems.Include("EndPointSystemType").Where(ep => ep.Id == EndPointSystemId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Getting the EndpointSystem by Id", ex);
                throw ex;
            }            
        }
        //public MailTemplate GetMailTemplate(string TemplateName)
        //{
        //    try
        //    {
        //        using (IACMDBContext oIACMDBContext = new IACMDBContext())
        //        {
        //            return oIACMDBContext.MailTemplate.Where(temp => temp.TemplateName == TemplateName).FirstOrDefault();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DILog.GeneralLogger.WriteLog(LogLevelType.Info, "Error Occured while Fetching Get MailTemplete", ex);
        //        throw ex;
        //    }
        //}
        public EndpointType GetEndPointTypeName(int EndpointTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    EndPointSystemType eType = new EndPointSystemType();
                    eType = _context.EndpointSystemTypes.Find(EndpointTypeId);
                    return eType.EndpointSystemTypeName;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
      
    }
}
