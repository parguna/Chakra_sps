using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SF.Objects
{
    // To Discover Section
    public  class SF_FODepartment : SF_Organisation
    {
        public SF_FODepartment(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer, _idmsLogger, _serviceUser)
        {
            scope = 5;
        }

        public override void GetOrganisation()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate Organisation [Section] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.FODepartment> fODepartments;

                totalCount = entityContainer.FODepartment.Where(c => c.endDate >= CurrentDay || c.endDate == MaxDate).Select(fODepartment =>
                    new SFOData.FODepartment()
                    {
                        externalCode = fODepartment.externalCode,
                        name = fODepartment.name,
                        cust_toBranchProp = fODepartment.cust_toBranchProp,
                        startDate = fODepartment.startDate,
                        endDate = fODepartment.endDate
                    }
                    )
                    .ToList().Count();

                while (pageSize * page < totalCount)
                {
                    fODepartments = entityContainer.FODepartment
                        .Where(c=> c.endDate >= CurrentDay || c.endDate == MaxDate)
                        .OrderBy(c => c.endDate)
                        .Skip(pageSize * page).Take(pageSize).Select(fODepartment => 
                    new SFOData.FODepartment()
                    {   externalCode= fODepartment.externalCode,
                        name = fODepartment.name,
                        cust_toBranchProp =fODepartment.cust_toBranchProp,
                        startDate = fODepartment.startDate,
                        endDate = fODepartment.endDate
                    }
                    )
                    .ToList();
                    foreach (var fODepartment in fODepartments)
                        saveOrganisations(fODepartment.externalCode,fODepartment.name, fODepartment.cust_toBranchProp);
                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"Organisation [Section] populated - {totalCount} in SF_FODepartment - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Section] Error Occured in populate SF_FODepartment", ex);
                throw ex;
            }

        }

        public override void UpdateOrgHead()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"SF_FODepartment:UpdateOrgHead - [Section] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.FODepartment> fODepartments;

                totalCount = entityContainer.FODepartment
                                     .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate))
                                     .Select(c => new SFOData.FODepartment()
                                     {
                                         externalCode = c.externalCode,
                                         headOfUnit = c.headOfUnit,
                                         endDate = c.endDate,
                                         startDate = c.startDate
                                     }).ToList().Count();
                while (pageSize * page < totalCount)
                {

                    var query = entityContainer.FODepartment
                                     .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate)).OrderBy(c => c.endDate)
                                     .Skip(pageSize * page)
                                     .Take(pageSize)
                                     .Select(c => new SFOData.FODepartment() { externalCode = c.externalCode, headOfUnit = c.headOfUnit, endDate = c.endDate, startDate = c.startDate });

                    fODepartments = query.ToList();

                    if (fODepartments == null || fODepartments.Count() == 0) return;

                    foreach (var fODepartment in fODepartments)
                    {
                        this.UpdateOrgHead(fODepartment.externalCode,fODepartment.headOfUnit);
                    }
                    page++;
                }

                idmsLogger.WriteLog(LogLevelType.Info, $"SF_FODepartment: Organisation Head population - {totalCount} for the scope - {scope}");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"SF_FODepartment: Error Occured in Organisation Head population for the scope - {scope}", ex);
                throw ex;
            }
            finally
            {
                idmsLogger.WriteLog(LogLevelType.Info, $"SF_FODepartment:UpdateOrgHead - [Section] Completed.");
            }
        }



    }
}
