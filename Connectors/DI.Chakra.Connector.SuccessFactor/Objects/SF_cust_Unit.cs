using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SF.Objects
{
    // To Discover Unit
    public class SF_cust_Unit : SF_Organisation
    {
        public SF_cust_Unit(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer, _idmsLogger, _serviceUser)
        {
            // scope for Unit
            scope = 6;
        }

        public override void GetOrganisation()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate Organisation [Unit] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.cust_Unit> cust_Units;

                totalCount = entityContainer.cust_Unit.Where(c => c.mdfSystemEffectiveEndDate >= CurrentDay || c.mdfSystemEffectiveEndDate == MaxDate).Select(_cust_Unit =>
                         new SFOData.cust_Unit()
                         {
                             mdfSystemEffectiveEndDate = _cust_Unit.mdfSystemEffectiveEndDate,
                             effectiveStartDate = _cust_Unit.effectiveStartDate,
                             externalCode = _cust_Unit.externalCode,
                             externalName_defaultValue = _cust_Unit.externalName_defaultValue,
                             cust_toSection =
                             new SFOData.FODepartment()
                             {
                                 externalCode = _cust_Unit.cust_toSection.externalCode,
                                 name = _cust_Unit.cust_toSection.name,
                                 cust_toBranchProp = _cust_Unit.cust_toSection.cust_toBranchProp
                             }
                         }).ToList().Count();

                while (pageSize * page < totalCount)
                {
                    cust_Units = entityContainer.cust_Unit
                        .Where(u=> u.mdfSystemEffectiveEndDate >= CurrentDay || u.mdfSystemEffectiveEndDate == MaxDate).OrderBy(c => c.mdfSystemEffectiveEndDate)
                        //.Expand(u=>u.cust_toSection)
                        .Skip(pageSize * page) 
                        .Take(pageSize)
                        .Select(_cust_Unit =>
                        new SFOData.cust_Unit()
                        {
                            mdfSystemEffectiveEndDate = _cust_Unit.mdfSystemEffectiveEndDate,
                            effectiveStartDate = _cust_Unit.effectiveStartDate,
                            externalCode = _cust_Unit.externalCode,
                            externalName_defaultValue = _cust_Unit.externalName_defaultValue,
                            cust_toSection =  
                            new SFOData.FODepartment()
                            {                                 
                                externalCode = _cust_Unit.cust_toSection.externalCode,
                                name = _cust_Unit.cust_toSection.name,
                                cust_toBranchProp = _cust_Unit.cust_toSection.cust_toBranchProp 
                            }  
                        }).ToList();

                    foreach (var _cust_Unit in cust_Units)
                        saveOrganisations(_cust_Unit.externalCode,_cust_Unit.externalName_defaultValue, (_cust_Unit.cust_toSection!=null? _cust_Unit.cust_toSection.externalCode : null));
                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"Organisation [Unit] populated - {totalCount} in SF_FODepartment - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Unit] Error Occured in populate SF_cust_Unit", ex);
                throw ex;
            }

        }

        public override void UpdateOrgHead()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"SF_cust_Unit: UpdateOrgHead - [Unit] Started.");
            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.cust_Unit> cust_Units;
                totalCount = entityContainer.cust_Unit
                                     .Where(c => (c.mdfSystemEffectiveEndDate >= CurrentDay || c.mdfSystemEffectiveEndDate == MaxDate))
                                     .Select(c => new SFOData.cust_Unit() { externalCode = c.externalCode, cust_HeadofUnit = c.cust_HeadofUnit, mdfSystemEffectiveEndDate = c.mdfSystemEffectiveEndDate, effectiveStartDate = c.effectiveStartDate })
                                     .ToList().Count();

                while (pageSize * page < totalCount)
                {
                    cust_Units = entityContainer.cust_Unit
                                     .Where(c =>(c.mdfSystemEffectiveEndDate >= CurrentDay || c.mdfSystemEffectiveEndDate == MaxDate)).OrderBy(c => c.mdfSystemEffectiveEndDate)
                                     .Skip(pageSize * page)
                                     .Take(pageSize).Select(c => new SFOData.cust_Unit() { externalCode = c.externalCode, cust_HeadofUnit = c.cust_HeadofUnit, mdfSystemEffectiveEndDate = c.mdfSystemEffectiveEndDate, effectiveStartDate = c.effectiveStartDate })
                                     
                                     .ToList();

                    if (cust_Units == null || cust_Units.Count() == 0) return;

                    foreach (var _cust_Unit in cust_Units)
                    {
                        this.UpdateOrgHead(_cust_Unit.externalCode, _cust_Unit.cust_HeadofUnit);
                    }
                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"SF_cust_Unit:Organisation Head population - {totalCount} for the scope - {scope}");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"SF_cust_Unit:Error Occured in Organisation Head population for the scope - {scope}", ex);
                throw ex;
            }
            finally { idmsLogger.WriteLog(LogLevelType.Info, $"SF_cust_Unit: UpdateOrgHead - [Unit] Completed."); }
        }
    }

 }
