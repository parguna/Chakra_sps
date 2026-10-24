using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.DomainModel;
using DI.Chakra.Logger;
using DI.Chakra.Connector.SF.Base;

namespace DI.Chakra.Connector.SF.Objects
{
    public abstract class SF_Organisation : FO_Base
    {  
        protected long scope;

        public SF_Organisation(SFOData.EntityContainer _entityContainer, IdmsLogger _idmsLogger, string _serviceUser) : base(_entityContainer, _idmsLogger, _serviceUser)
        { 
            scope = 0;
        }

        public abstract void GetOrganisation();
        public abstract void UpdateOrgHead();
        protected virtual void UpdateOrgHead(string orgCode, string employeeId)
        {
            if (string.IsNullOrWhiteSpace(orgCode)) return;
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                //No Organisation Head, Delete orgHead;
                var currentOrgHeads = NOrganisationHead.GeByOrgCode(orgCode,this.scope);
                if (currentOrgHeads != null)
                {
                    try
                    {
                        currentOrgHeads.Delete();
                    }
                    catch
                    {
                    }
                }
                return;
            }
            orgCode = orgCode.PadWithZeros(SF_Helper.OrgCode_MaxSize);
            employeeId = employeeId.PadWithZeros(SF_Helper.EmployeeIdMaxSize);
            try
            {
                NOrganisation org = NOrganisation.GetByOrgCodeAndScopeId(orgCode, scope);
                if (org != null)
                {
                    User usr = User.GetByEmployeeId(employeeId.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
                    if (usr != null)
                    {
                        List<NOrganisationHead> heads = NOrganisationHead.GetAllByOrganisationCode(orgCode);

                        if (heads != null || heads.Count()> 0)
                        {
                            foreach (var h in heads.Where(c => !c.HeadId.HasValue || c.HeadId.Value != usr.Id))
                                h.Delete();
                        }

                        heads = NOrganisationHead.GetAllByOrganisationCode(orgCode);

                        NOrganisationHead nOrganisationHead = null;

                        nOrganisationHead = heads.FirstOrDefault(c => c.HeadId.HasValue && c.HeadId.Value == usr.Id); 

                        if (nOrganisationHead == null)
                        {
                            nOrganisationHead = new NOrganisationHead()
                            {
                                CoordinatorId = usr.Id,
                                HeadId = usr.Id,
                                OrganisationId = org.Id,
                                CreatedBy = "System",
                                CreatedDate = DateTime.Now

                            }; 
                        }
                        else
                        {
                            nOrganisationHead.HeadId = usr.Id;
                            nOrganisationHead.CoordinatorId = usr.Id;
                            nOrganisationHead.OrganisationId = org.Id; 
                        }
                        nOrganisationHead.Save(serviceUser);

                    }
                }
            }
            catch (Exception e)
            {
                idmsLogger.WriteLog(LogLevelType.Exception, $"Exception occured in UpdateORgHead for the orgCode - {orgCode} and EmployeeId - {employeeId}", e);
            }
        }


        protected void saveOrganisations(string orgCode,string name, string parentCode)
        {
            if (!string.IsNullOrWhiteSpace(orgCode))
            {
                orgCode = orgCode.PadWithZeros(SF_Helper.OrgCode_MaxSize);
                var org = NOrganisation.GetByOrgCodeAndScopeId(orgCode, scope);
                if (org == null)
                {
                    org = new NOrganisation()
                    {
                        CreatedBy = "System",
                        CreatedDate = DateTime.Now,
                        Code = orgCode,
                        ScopeId = scope,
                        ParentOrgCode = string.IsNullOrWhiteSpace(parentCode) ? null : parentCode.PadWithZeros(SF_Helper.OrgCode_MaxSize),
                        IsActive = true,
                        Name = name,
                    };
                    org.Save(serviceUser);
                }
                else
                {
                    org.LastModifiedBy = "System";
                    org.LastModifiedDate = DateTime.Now;
                    org.Code = orgCode;
                    org.ScopeId = scope;
                    org.ParentOrgCode = string.IsNullOrWhiteSpace(parentCode) ? null : parentCode.PadWithZeros(SF_Helper.OrgCode_MaxSize);
                    org.IsActive = true;
                    org.Name = name;
                    org.Save(serviceUser);
                }
            }
        } 

        public void GetOrganisations()
        {
            byte retry = 0;
            while (retry < 2)
            {
                try
                {
                    GetOrganisation();
                    return;
                }
                catch (Exception ex)
                {
                    retry++;
                    idmsLogger.WriteLog(LogLevelType.Exception, $"GetOrganisations failed. Retry:{retry} and Type:{this.GetType()}",ex);
                    if(retry >= 2)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
