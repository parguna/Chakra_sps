using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;

namespace DI.Chakra.Connector.SF.Objects
{
    public class SF_FOCompany : SF_Organisation
    { 
        public SF_FOCompany(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer,_idmsLogger,_serviceUser)
        {
            scope = 1;
        }


        public  override  void GetOrganisation()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"Populate Organisation [Company] Started.");

            long totalCount = 0; 
            int page = 0;
            try
            {
                List<SFOData.FOCompany> companies;

                totalCount = entityContainer.FOCompany.Where(c => c.endDate >= CurrentDay || c.endDate == MaxDate).Select(c => new SFOData.FOCompany()
                {
                    cust_companyOrg = c.cust_companyOrg,
                    endDate = c.endDate,
                    externalCode = c.externalCode,
                    name = c.name
                }).ToList().Count();


                while (pageSize * page < totalCount)
                {
                    companies = entityContainer.FOCompany.Where(c => c.endDate >= CurrentDay || c.endDate == MaxDate).OrderBy(c => c.endDate).Skip(pageSize * page).Take(pageSize).Select(c=> new SFOData.FOCompany() { cust_companyOrg = c.cust_companyOrg, endDate = c.endDate, externalCode = c.externalCode, name = c.name }  ).ToList();
                    saveCompanies(companies, serviceUser);
                    page++;
                }
                 
                    idmsLogger.WriteLog(LogLevelType.Info, $"Organisation [Company] populated - {totalCount} in SF_FOCompany - Completed");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, "[Company] Error Occured in populate companies", ex);
                throw ex;
            }

        }

        public override void UpdateOrgHead()
        {
            idmsLogger.WriteLog(LogLevelType.Info, $"SF_FOCompany:UpdateOrgHead - [Company] Started.");

            long totalCount = 0;
            int page = 0;
            try
            {
                List<SFOData.FOCompany> companies;
                totalCount = entityContainer.FOCompany
                     .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate)).Select(c => new SFOData.FOCompany()
                     {
                         cust_companyOrg = c.cust_companyOrg,
                         cust_headOfUnit = c.cust_headOfUnit,
                         endDate = c.endDate,
                         startDate = c.startDate
                     })
                     .ToList().Count();
                while (pageSize * page < totalCount)
                {
                    companies = entityContainer.FOCompany
                                     .Where(c => (c.endDate >= CurrentDay || c.endDate == MaxDate)).OrderBy(c => c.endDate)
                                     .Skip(pageSize * page)
                                     .Take(pageSize)
                                     .Select(c => new SFOData.FOCompany()
                                     { cust_companyOrg = c.cust_companyOrg, cust_headOfUnit = c.cust_headOfUnit,
                                       endDate = c.endDate, startDate = c.startDate
                                     }) 
                                     .ToList();

                    if (companies == null || companies.Count() == 0) return;

                    foreach (var company in companies)
                    {
                        this.UpdateOrgHead(company.cust_companyOrg, company.cust_headOfUnit);
                    }

                    page++;
                }
                idmsLogger.WriteLog(LogLevelType.Info, $"SF_FOCompany:Organisation Head population - {totalCount} for the scope - {scope}");
            }
            catch (Exception ex)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"SF_FOCompany:Error Occured in Organisation Head population for the scope - {scope}", ex);
                throw ex;
            }
            finally
            {
                idmsLogger.WriteLog(LogLevelType.Info, $"SF_FOCompany:UpdateOrgHead - [Company] Completed.");
            }
        }
         
        void saveCompanies(List<SFOData.FOCompany> companies, string serviceUser)
        {
            if (companies == null) return;
            foreach (var company in companies)
            {
                var comp = Company.GetByCode(company.externalCode);
                if (comp == null)
                {
                    Company cmpany = new Company()
                    {
                        CreatedBy = "System",
                        CreatedDate = DateTime.Now,
                        Code = company.externalCode,
                        Text = company.name,
                        IsActive = true,
                    };

                    cmpany.Save(serviceUser);
                }
                else
                {
                    comp.Text = company.name;
                    comp.LastModifiedBy = "System";
                    comp.LastModifiedDate = DateTime.Now;

                    comp.Save(serviceUser);
                }


                if (!string.IsNullOrWhiteSpace(company.cust_companyOrg))
                {
                    var orgCode = company.cust_companyOrg.PadWithZeros(SF_Helper.OrgCode_MaxSize);
                    var org = NOrganisation.GetByOrgCodeAndScopeId(orgCode, 1);
                    if (org == null)
                    {
                        org = new NOrganisation()
                        {
                            CreatedBy = "System",
                            CreatedDate = DateTime.Now,
                            Code = orgCode,
                            ScopeId = 1,
                            ParentOrgCode = null,
                            IsActive = true,
                            Name = company.name
                        };
                        org.Save(serviceUser);
                    }
                    else
                    {
                        org.LastModifiedBy = "System";
                        org.LastModifiedDate = DateTime.Now;
                        org.Code = orgCode;
                        org.ScopeId = 1;
                        org.ParentOrgCode = null;
                        org.IsActive = true;
                        org.Name = company.name;
                        org.Save(serviceUser);
                    }
                }
                else
                {
                    idmsLogger.WriteLog(LogLevelType.Warning, $"SF_FOCompany:company :{company?.name} Property cust_companyOrg empty. Unable to create NOrganisation entry");
                }
            }

        }
         
         

    }
}
