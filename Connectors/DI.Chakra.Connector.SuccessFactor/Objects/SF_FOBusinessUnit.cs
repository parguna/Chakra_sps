using DI.Chakra.Logger; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SF.Objects
{
    public class SF_FOBusinessUnit : SF_Organisation
    {

        // To Discover Department
        public SF_FOBusinessUnit(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer, _idmsLogger, _serviceUser)
        {
            scope = 2;
        }

        public override void GetOrganisation()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate  Organisation [Department] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.FOBusinessUnit> fOBusinessUnits;

                totalCount = entityContainer.FOBusinessUnit.Where(d => d.endDate >= CurrentDay || d.endDate == MaxDate).Select(foBusinessUnit =>
                        new SFOData.FOBusinessUnit()
                        {
                            startDate = foBusinessUnit.startDate,
                            endDate = foBusinessUnit.endDate,
                            externalCode = foBusinessUnit.externalCode,
                            name = foBusinessUnit.name,
                            cust_companyOrg = foBusinessUnit.cust_companyOrg
                        })
                          .ToList().Count();

                while (pageSize * page < totalCount)
                {
                    fOBusinessUnits = entityContainer.FOBusinessUnit
                        .Where(d => d.endDate >= CurrentDay || d.endDate == MaxDate).OrderBy(c => c.endDate)
                        .Skip(pageSize * page)
                        .Take(pageSize)
                        .Select(foBusinessUnit =>
                        new SFOData.FOBusinessUnit()
                        {
                            startDate = foBusinessUnit.startDate,
                            endDate = foBusinessUnit.endDate,
                            externalCode = foBusinessUnit.externalCode,
                            name = foBusinessUnit.name,
                            cust_companyOrg = foBusinessUnit.cust_companyOrg
                        })
                          .ToList();
                    foreach (var foBusinessUnit in fOBusinessUnits)
                        saveOrganisations(foBusinessUnit.externalCode,foBusinessUnit.name, foBusinessUnit.cust_companyOrg);
                    page++;
                }  
                    idmsLogger.WriteLog(LogLevelType.Info, $"Organisation [Department] populated - {totalCount} in SF_FOBusinessUnit - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Department] Error Occured in populate FOBusinessUnit", ex);
                throw ex;
            }

        }

        public override void UpdateOrgHead()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"SF_FOBusinessUnit:Populate  Organisation [Department] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.FOBusinessUnit> FOBusinessUnits;

                totalCount = entityContainer.FOBusinessUnit
                        .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate)).Select(c => new SFOData.FOBusinessUnit()
                        {
                            startDate = c.startDate,
                            endDate = c.endDate,
                            externalCode = c.externalCode,
                            headOfUnit = c.headOfUnit
                        }).ToList().Count();
                while (pageSize * page < totalCount)
                {
                    FOBusinessUnits = entityContainer.FOBusinessUnit
                                        .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate) ).OrderBy(c => c.endDate)
                                        .Skip(pageSize * page)
                                        .Take(pageSize)
                                        .Select(c => new SFOData.FOBusinessUnit()
                                        {
                                            startDate = c.startDate,
                                            endDate = c.endDate,
                                            externalCode = c.externalCode,
                                            headOfUnit = c.headOfUnit
                                        }).ToList();

                    if (FOBusinessUnits == null || FOBusinessUnits.Count() == 0) return;

                    foreach (var fOBusinessUnit in FOBusinessUnits)
                    {
                        this.UpdateOrgHead(fOBusinessUnit.externalCode, fOBusinessUnit.headOfUnit);
                    }
                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"SF_FOBusinessUnit:Organisation Head population - {totalCount} for the scope - {scope}");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"SF_FOBusinessUnit:Error Occured in Organisation Head population for the scope - {scope}", ex);
                throw ex;
            }
            finally
            {
                idmsLogger.WriteLog(LogLevelType.Info, $"SF_FOBusinessUnit:Populate  Organisation [Department] Completed.");
            }
        }



    }
}
