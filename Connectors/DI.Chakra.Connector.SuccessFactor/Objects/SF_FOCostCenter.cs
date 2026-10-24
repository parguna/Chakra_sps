using DI.Chakra.Connector.SF.Base;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Connector.SF.Objects
{
    public class SF_FOCostCenter : FO_Base
    { 

        
        public SF_FOCostCenter(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer, _idmsLogger, _serviceUser)
        {
             
        }


        public void GetCostCenter()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate Cost Center started");
            long totalCount = 0;
            int page = 0;
            List<SFOData.FOCostCenter> FOCostCenters = new List<SFOData.FOCostCenter>();
            try
            {

                totalCount = entityContainer.FOCostCenter.Where(c => c.endDate >= CurrentDay || c.endDate == MaxDate).Select(cc => new SFOData.FOCostCenter()
                {
                    startDate = cc.startDate,
                    endDate = cc.endDate,
                    externalCode = cc.externalCode,
                    description = cc.description,
                    cust_toCompany = cc.cust_toCompany
                }).ToList().Count();

                while (pageSize * page < totalCount)
                {
                    FOCostCenters = entityContainer.FOCostCenter
                        .Where(c=> c.endDate >= CurrentDay || c.endDate == MaxDate ).OrderBy(c => c.endDate)
                        .Skip(pageSize * page)
                        .Select(cc=> new SFOData.FOCostCenter() {
                            startDate = cc.startDate,
                            endDate = cc.endDate,
                            externalCode = cc.externalCode,
                            description = cc.description, 
                            cust_toCompany = cc.cust_toCompany
                        }).Take(pageSize)
                        
                        .ToList();
                    foreach (var cc in FOCostCenters)
                    {
                       var costCenter = DomainModel.NCostCenter.GetByCode(cc.externalCode);

                        if (costCenter == null)
                        {
                            costCenter = new DomainModel.NCostCenter();
                            costCenter.Code = cc.externalCode;
                            costCenter.CreatedBy = "System";
                            costCenter.CreatedDate = DateTime.Now;
                            costCenter.Description = cc.description;

                        }
                        else
                        {
                            costCenter.LastModifiedBy = "System";
                            costCenter.LastModifiedDate = DateTime.Now;
                            costCenter.Description = cc.description;
                        }
                        costCenter.IsActive = cc.endDate >= DateTime.Today ;
                        if (!string.IsNullOrWhiteSpace(cc.cust_toCompany))
                        {
                            var company = DomainModel.Company.GetByCode(cc.cust_toCompany);
                            if (company != null)
                            {
                                costCenter.CompanyId = company.Id;
                            }
                        }

                        costCenter.Save(serviceUser);

                    }
                    page++;
                }

                idmsLogger.WriteLog(LogLevelType.Info, $"[Cost Center] populated - {totalCount} in SF_FOCostCenter - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Cost Center] Error Occured in populate Cost center", ex);
                throw ex;
            }
            finally
            {
                FOCostCenters?.Clear();
                FOCostCenters = null;
            }
        }


    }
}
