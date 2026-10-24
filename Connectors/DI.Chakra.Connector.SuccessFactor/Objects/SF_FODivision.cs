using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SF.Objects
{

    // To Discover branch
    public class SF_FODivision : SF_Organisation
    {
        public SF_FODivision(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer, _idmsLogger, _serviceUser)
        {
            scope = 4;
        }

        public override void GetOrganisation()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate Organisation [Branch] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.FODivision> fODivisions;

                totalCount = entityContainer.FODivision.Where(c => c.endDate >= CurrentDay || c.endDate == MaxDate).Select(fODivision => new SFOData.FODivision()
                {
                    startDate = fODivision.startDate,
                    endDate = fODivision.endDate,
                    externalCode = fODivision.externalCode,
                    name = fODivision.name,
                    cust_toDepartmentProp = fODivision.cust_toDepartmentProp
                }).ToList().Count();

                while ( pageSize*page < totalCount)
                {
                    fODivisions = entityContainer.FODivision
                        .Where(c=> c.endDate >= CurrentDay || c.endDate == MaxDate).OrderBy(c => c.endDate)
                        .Skip(pageSize * page).Take(pageSize).Select(fODivision=> new SFOData.FODivision()
                    {
                        startDate = fODivision.startDate,
                        endDate = fODivision.endDate,
                        externalCode = fODivision.externalCode,
                        name = fODivision.name,
                        cust_toDepartmentProp = fODivision.cust_toDepartmentProp
                    })
                    
                    .ToList();
                    foreach (var fODivision in fODivisions)
                        saveOrganisations(fODivision.externalCode,fODivision.name, fODivision.cust_toDepartmentProp);
                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"Organisation [Branch] populated - {totalCount} in SF_FODivision - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Branch] Error Occured in populate FOBusinessUnit", ex);
                throw ex;
            }

        }

        public override void UpdateOrgHead()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"SF_FODivision: UpdateOrgHead - [Branch] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.FODivision> fODivisions;
                totalCount = entityContainer.FODivision
                                     .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate)).Select(c => new SFOData.FODivision()
                                     { externalCode = c.externalCode, headOfUnit = c.headOfUnit, endDate = c.endDate, startDate = c.startDate })
                                    .ToList().Count();

                while (pageSize * page < totalCount)
                {
                    fODivisions = entityContainer.FODivision
                                    .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate)).OrderBy(c => c.endDate)
                                    .Skip(pageSize * page)
                                    .Take(pageSize)
                                    .Select(c => new SFOData.FODivision()
                                    { externalCode = c.externalCode, headOfUnit = c.headOfUnit, endDate = c.endDate, startDate = c.startDate })
                                     
                                    .ToList();

                    if (fODivisions == null || fODivisions.Count() == 0) return;

                    foreach (var fODivision in fODivisions)
                    {
                        this.UpdateOrgHead(fODivision.externalCode
                            , fODivision.headOfUnit);
                    }
                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"SF_FODivision:Organisation Head population - {totalCount} for the scope - {scope}");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"SF_FODivision:Error Occured in Organisation Head population for the scope - {scope}", ex);
                throw ex;
            }
            finally { idmsLogger.WriteLog(LogLevelType.Info, $"SF_FODivision: UpdateOrgHead - [Branch] Completed."); }
        }

    }
}
