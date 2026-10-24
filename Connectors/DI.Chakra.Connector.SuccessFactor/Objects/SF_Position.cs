using DI.Chakra.Connector.SF.Base;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SF.Objects
{
   public class SF_Position : FO_Base 
    { 
        public SF_Position(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer, _idmsLogger, _serviceUser)
        {

        } 
        public void GetPosition()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate Position Started.");

            long totalCount = 0;
            int page = 0;
            List<SFOData.Position> FOPositions = new List<SFOData.Position>();

            try
            {

                totalCount = this.entityContainer.Position.Where(p => p.effectiveEndDate.Value >= CurrentDay).Select(p => new SFOData.Position()
                {
                    code = p.code,
                    jobTitle = p.jobTitle,
                    externalName_defaultValue = p.externalName_defaultValue,
                    effectiveStartDate = p.effectiveStartDate,
                    effectiveEndDate = p.effectiveEndDate
                }).ToList().Count();

                while (pageSize * page < totalCount)
                {
                    FOPositions = entityContainer.Position.Where(p => p.effectiveEndDate.Value >= CurrentDay).Skip(pageSize * page)
                       .Select(p => new SFOData.Position()
                       {
                           code = p.code,
                           jobTitle = p.jobTitle,
                           externalName_defaultValue = p.externalName_defaultValue,
                           effectiveStartDate = p.effectiveStartDate,
                           effectiveEndDate = p.effectiveEndDate
                       }).Take(pageSize).ToList();
                    foreach (var cc in FOPositions)
                    {
                        var position = DomainModel.NDesignation.GetByCode(cc.code);
                        if (position == null)
                        {
                            position = new DomainModel.NDesignation() { Code = cc.code, Description = cc.externalName_defaultValue, IsActive = true, CreatedBy = serviceUser, CreatedDate = DateTime.Now };
                            position.Save(serviceUser);
                        }
                        else
                        {
                            if (position.Description != cc.externalName_defaultValue || position.IsActive == false)
                            {
                                position.IsActive = true;
                                position.LastModifiedDate = DateTime.Now;
                                position.LastModifiedBy = serviceUser;
                                position.Description = cc.externalName_defaultValue;
                                position.Save(serviceUser);
                            }
                        }
                        
                    }
                    page++;
                }

                idmsLogger.WriteLog(LogLevelType.Info, $"Position populated - {totalCount} in SF_Position - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Position] Error Occured in populate Positions", ex);
                throw ex;
            }
            finally
            {
                FOPositions?.Clear();
                FOPositions = null;
            }
        }



        public void GetPosition(DateTime deltaDate)
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate Position Started.");
            
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.Position> FOPositions;

                totalCount = this.entityContainer.Position.Where(p => (p.createdDateTime >= deltaDate || p.lastModifiedDateTime >= deltaDate)
                && p.effectiveEndDate.Value == MaxDate).Select(p => new SFOData.Position()
                {
                    code = p.code,
                    jobTitle = p.jobTitle,
                    externalName_defaultValue = p.externalName_defaultValue,
                    effectiveStartDate = p.effectiveStartDate,
                    effectiveEndDate = p.effectiveEndDate
                }).ToList().Count();

                while (pageSize * page < totalCount)
                {
                    var query = entityContainer.Position.Where(p => (p.createdDateTime >= deltaDate || p.lastModifiedDateTime >= deltaDate) && p.effectiveEndDate.Value == MaxDate).Skip(pageSize * page)
                       .Select(p => new SFOData.Position()
                       {
                           code = p.code,
                           jobTitle = p.jobTitle,
                           externalName_defaultValue = p.externalName_defaultValue, 
                           effectiveStartDate = p.effectiveStartDate,
                           effectiveEndDate = p.effectiveEndDate
                       });
                    
                    FOPositions = query.Take(pageSize).ToList();

                    foreach (var cc in FOPositions)
                    {
                        var position = DomainModel.NDesignation.GetByCode(cc.code);
                        if (position == null)
                        {
                            position = new DomainModel.NDesignation() { Code = cc.code, Description = cc.externalName_defaultValue, IsActive = true, CreatedBy = serviceUser, CreatedDate = DateTime.Now };
                            position.Save(serviceUser);
                        }
                        else
                        {
                            if (position.Description != cc.externalName_defaultValue || position.IsActive == false)
                            {
                                position.IsActive = true;
                                position.LastModifiedDate = DateTime.Now;
                                position.LastModifiedBy = serviceUser;
                                position.Description = cc.externalName_defaultValue;
                                position.Save(serviceUser);
                            }
                        }

                    }
                    page++;
                }

                idmsLogger.WriteLog(LogLevelType.Info, $"Position populated - {totalCount} in SF_Position - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Position] Error Occured in populate Positions", ex);
                throw ex;
            }
        }

    }
}
