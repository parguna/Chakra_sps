using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class PresentationAttestationScopeRepository : RepositoryBase<AttestationScope>
    {
        public PresentationAttestationScopeRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public PresentationAttestationScope GetOrganisationScope(long id, long organisationScopeId)
        {
            if ((organisationScopeId == 0) || (organisationScopeId == null))
            {
                return (from att in CurrentContext.Attestations
                        join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                        where attScope.Id == id
                        select new PresentationAttestationScope
                        {
                            Id = attScope.Id,
                            AttestationId = att.Id,
                            OrganisationScopeId = 0,
                            OrganisationScopeName = string.Empty,
                            AttestationScopeName = attScope.AttestationScopeName,
                            AttestationScopeValue = attScope.AttestationScopeValue == null ? 0 : attScope.AttestationScopeValue,
                            AttestationScopeValueDesc = "ALL"

                        }).FirstOrDefault();

            }
            return (from att in CurrentContext.Attestations
                    join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                    join org in CurrentContext.NOrganisations on attScope.OrganisationScopeId equals org.Id
                    join orgScope in CurrentContext.NOrganisationScopes on attScope.AttestationScopeValue equals orgScope.Id
                    where attScope.Id == id && attScope.OrganisationScopeId == organisationScopeId
                    select new PresentationAttestationScope
                    {
                        Id = attScope.Id,
                        AttestationId = att.Id,
                        OrganisationScopeId = (long)attScope.OrganisationScopeId,
                        OrganisationScopeName = org.Code+" - "+ org.Name,
                        AttestationScopeName = attScope.AttestationScopeName,
                        AttestationScopeValue = attScope.AttestationScopeValue == null ? 0 : attScope.AttestationScopeValue,
                        AttestationScopeValueDesc = org.Code + " - " + org.Name

                    }).FirstOrDefault();
        }

        public PresentationAttestationScope GetIACMGroupScope(long id, long attestationScopeValue)
        {
            if ((attestationScopeValue == 0) || (attestationScopeValue == null))
            {
                return (from att in CurrentContext.Attestations
                        join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                        where attScope.Id == id
                        select new PresentationAttestationScope
                        {
                            Id = attScope.Id,
                            AttestationId = att.Id,
                            OrganisationScopeName = string.Empty,
                            AttestationScopeName = attScope.AttestationScopeName,
                            AttestationScopeValue = null,
                            AttestationScopeValueDesc = "ALL"

                        }).FirstOrDefault();
            }

            return (from att in CurrentContext.Attestations
                    join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                    join grp in CurrentContext.Group on attScope.AttestationScopeValue equals grp.Id
                    where attScope.Id == id
                    select new PresentationAttestationScope
                    {
                        Id = attScope.Id,
                        AttestationId = att.Id,
                        OrganisationScopeName = string.Empty,
                        AttestationScopeName = attScope.AttestationScopeName,
                        AttestationScopeValue = attScope.AttestationScopeValue,
                        AttestationScopeValueDesc = grp.GroupName

                    }).FirstOrDefault();
        }

        public PresentationAttestationScope GetUserScope(long id, long attestationScopeValue)
        {
            if ((attestationScopeValue == 0) || (attestationScopeValue == null))
            {
                return (from att in CurrentContext.Attestations
                        join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                        where attScope.Id == id
                        select new PresentationAttestationScope
                        {
                            Id = attScope.Id,
                            AttestationId = att.Id,
                            OrganisationScopeName = string.Empty,
                            AttestationScopeName = attScope.AttestationScopeName,
                            AttestationScopeValue = null,
                            AttestationScopeValueDesc = "ALL"

                        }).FirstOrDefault();
            }

            return (from att in CurrentContext.Attestations
                    join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                    join usr in CurrentContext.User on attScope.AttestationScopeValue equals usr.Id
                    where attScope.Id == id
                    select new PresentationAttestationScope
                    {
                        Id = attScope.Id,
                        AttestationId = att.Id,
                        OrganisationScopeName = string.Empty,
                        AttestationScopeName = attScope.AttestationScopeName,
                        AttestationScopeValue = attScope.AttestationScopeValue,
                        AttestationScopeValueDesc = usr.DisplayName + " [" + usr.UserName + "]"
                    }).FirstOrDefault();
        }

        public PresentationAttestationScope GetManagerScope(long id, long attestationScopeValue)
        {
            if ((attestationScopeValue == 0) || (attestationScopeValue == null))
            {
                return (from att in CurrentContext.Attestations
                        join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                        where attScope.Id == id
                        select new PresentationAttestationScope
                        {
                            Id = attScope.Id,
                            AttestationId = att.Id,
                            OrganisationScopeName = string.Empty,
                            AttestationScopeName = attScope.AttestationScopeName,
                            AttestationScopeValue = null,
                            AttestationScopeValueDesc = "ALL"

                        }).FirstOrDefault();
            }

            return (from att in CurrentContext.Attestations
                    join attScope in CurrentContext.AttestationScopes on att.Id equals attScope.AttestationId
                    join mgr in CurrentContext.User on attScope.AttestationScopeValue equals mgr.Id
                    where attScope.Id == id
                    select new PresentationAttestationScope
                    {
                        Id = attScope.Id,
                        AttestationId = att.Id,
                        OrganisationScopeName = string.Empty,
                        AttestationScopeName = attScope.AttestationScopeName,
                        AttestationScopeValue = attScope.AttestationScopeValue,
                        AttestationScopeValueDesc = mgr.DisplayName + " [" + mgr.UserName + "]"
                    }).FirstOrDefault();
        }

        public List<PresentationAttestationScope> GetOrganisationHeadList(long attestationId) 
        { 
              return (from orgHeards in CurrentContext.NOrganisationHeads 
                        join usr in CurrentContext.User on orgHeards.HeadId equals usr.Id
                        join org in CurrentContext.NOrganisations on orgHeards.OrganisationId equals org.Id
                        where usr.IsActive == true && org.IsActive == true
                        select new PresentationAttestationScope
                        {
                            Id = orgHeards.Id,
                            AttestationId = attestationId,
                            OrganisationScopeId = org.Id,
                            OrganisationScopeName = org.Name,
                            AttestationScopeName = string.Empty,
                            AttestationScopeValue = 0,
                            AttestationScopeValueDesc = "ALL"

                        }).ToList();
            }
        }
    }
