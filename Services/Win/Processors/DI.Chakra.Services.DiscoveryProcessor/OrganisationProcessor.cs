using DI.Chakra.DBConfigHelper;
using DI.Chakra.DomainModel;
using System;
using System.Linq;

namespace DI.Chakra.Services.DiscoveryProcessor
{
    public class OrganisationProcessor
    {
        private long m_epId;
        private Process _process;
        public OrganisationProcessor(long endpointSystemId, Process process)
        {
            m_epId = endpointSystemId;
            _process = process;
        }

        public void Start()
        {
            NOrganisation.FlushHeadInformation();

            ProcessOrganisation();

            ProcessOrganisationHead();

            ProcessOrganisationScope();

            ProcessDesignation();

        }

        public void ProcessOrganisationScope()
        {

            var list = NHRMSOrganisation.GetAll();

            var listScopes = NOrganisationScope.GetAll();

            foreach (NHRMSOrganisation hrmsOrg in list)
            {
                var scopeId = listScopes.Where(t => t.Scope == hrmsOrg.Scope).Select(l => l.Id).FirstOrDefault();

                if (scopeId > 0)
                {
                    var org = NOrganisation.GetByOrgCode(hrmsOrg.Code);

                    // update if value changed
                    if (org.ScopeId != scopeId)
                    {
                        org.ScopeId = scopeId;

                        org.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
        }

        public void ProcessOrganisationHead()
        {
            var hrmsOrgsWithHead = NHRMSOrganisation.GetBySystemId(m_epId).Where(t => t.Head != null && !string.IsNullOrEmpty(t.Head.Trim())).ToList();

            foreach (var item in hrmsOrgsWithHead)
            {
                var user = User.GetByEmployeeId(item.Head);
                var section = NOrganisation.GetByOrgCode(item.Code);
                if (user != null && section != null)
                {

                    var nOrgHead = NOrganisationHead.GetByOrganisationId(section.Id, false);

                    if (nOrgHead != null)
                    {
                        // Only if head id has changed, update
                        if (nOrgHead.HeadId != user.Id)
                        {
                            nOrgHead.HeadId = user.Id; // set new value
                            nOrgHead.CoordinatorId = user.Id;
                            nOrgHead.Save(MasterConfigHelper.ServiceUser);
                        }
                    }
                    else
                    {
                        NOrganisationHead head = new NOrganisationHead();
                        head.OrganisationId = section.Id;
                        head.HeadId = user.Id;
                        head.CoordinatorId = user.Id;
                        head.Save(MasterConfigHelper.ServiceUser);
                    }
                }
            }
        }
        public void ProcessOrganisation()
        {
            var hrmsOrgs = NHRMSOrganisation.GetBySystemId(m_epId);


            foreach (var hrmsOrg in hrmsOrgs)
            {
                var nOrg = NOrganisation.GetByOrgCode(hrmsOrg.Code);
                //var nParentOrg = NOrganisation.GetByOrgCode(hrmsOrg.ParentOrgCode);

                if (nOrg == null)
                {
                    NOrganisation org = new NOrganisation()
                    {
                        IsActive = true,
                        Code = hrmsOrg.Code,
                        Name = hrmsOrg.Name,
                        Description = hrmsOrg.Description,
                        ParentOrgCode = hrmsOrg.ParentOrgCode,
                        //ParentOrgCode = nParentOrg != null ? nParentOrg.Code : null,
                        //ParentOrgId = nParentOrg != null ? nParentOrg.Id : (long?)null,

                    };
                    org.Save(MasterConfigHelper.ServiceUser);
                }
                else
                {
                    nOrg.Name = hrmsOrg.Name;
                    nOrg.Description = hrmsOrg.Description;
                    nOrg.ParentOrgCode = hrmsOrg.ParentOrgCode;
                    //nOrg.ParentOrgCode = nParentOrg != null ? nParentOrg.Code : null;
                    //nOrg.ParentOrgId = nParentOrg != null ? nParentOrg.Id : (long?)null;
                    nOrg.Save(MasterConfigHelper.ServiceUser);
                }
            }

            NOrganisation.UpdateParentOrgId();
        }

        public void ProcessDesignation()
        {
            var hrmsDesignationList = NHRMSDesignation.GetByEndpointSystemId(m_epId);
            var currentUser = MasterConfigHelper.ServiceUser;
            var allNDesignations = NDesignation.GetAll();

            foreach (var hrmsDesignation in hrmsDesignationList)
            {
                var nDesignation = allNDesignations.Where(x => x.Code == hrmsDesignation.Code).FirstOrDefault();

                if (nDesignation == null)
                {
                    nDesignation = new NDesignation()
                    {
                        IsActive = hrmsDesignation.IsActive,
                        Code = hrmsDesignation.Code,
                        Description = hrmsDesignation.Description,
                        CreatedDate = DateTime.Now,
                        CreatedBy = currentUser
                    };
                    nDesignation.Save(currentUser);
                }
                else
                {
                    nDesignation.IsActive = hrmsDesignation.IsActive;
                    nDesignation.Description = hrmsDesignation.Description;
                    nDesignation.CreatedDate = hrmsDesignation.CreatedDate;
                    nDesignation.CreatedBy = hrmsDesignation.CreatedBy;
                    nDesignation.LastModifiedDate = hrmsDesignation.LastModifiedDate;
                    nDesignation.LastModifiedBy = hrmsDesignation.LastModifiedBy;
                    nDesignation.Save(currentUser);
                }

            }

            allNDesignations.Clear();
            allNDesignations = null;
        }
    }
}
