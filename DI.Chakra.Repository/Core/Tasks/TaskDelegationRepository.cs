using DI.Chakra.EntityModel;
using DI.Chakra.EntityModel.Presentation;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
namespace DI.Chakra.Repository
{
    public class TaskDelegationRepository : RepositoryBase<TaskDelegation>
    {
        public TaskDelegationRepository(DataAccessTransaction transaction) : base(transaction) { }
        
        public PresentationGridPagedData<PresentationTaskdelegation> GetPagedAllDelegationByActionUser(long userId, bool isAdmin,bool includeInActive ,string sortExpression, string filterExpression, int startIndex, int maximumRow)
        {
            PresentationGridPagedData<PresentationTaskdelegation> result = new PresentationGridPagedData<PresentationTaskdelegation>();

            var query = from item in CurrentContext.TaskDelegations
                        join user in CurrentContext.User
                            on item.DelegatedTo equals user.Id
                        join user1 in CurrentContext.User
                            on item.ActionUser equals user1.Id
                        join cuser in CurrentContext.User
                        on item.CreatedBy equals cuser.UserName into cusers
                        from cuser in cusers.DefaultIfEmpty()
                        join muser in CurrentContext.User
                            on item.LastModifiedBy equals muser.UserName into musers
                        from muser in musers.DefaultIfEmpty()
                        where (item.EndDate >= DateTime.Today || includeInActive) && item.CreatedBy != null && item.LastModifiedBy != null &&
                        (isAdmin ? true : item.ActionUser == userId) && (includeInActive || item.IsActive)
                       select new PresentationTaskdelegation
                       {
                           Id = item.Id,
                           ActionUser = item.ActionUser,
                           ActionUserName = user1.DisplayName,
                           CreatedDate = item.CreatedDate,
                           DelegateAttestationTask = item.DelegateAttestationTask,
                           DelegatedToUserId = item.DelegatedTo,
                           DelegatedToUserName = user.DisplayName,
                           DelegateProvisioningTask = item.DelegateProvisioningTask,
                           DelegateSODTask = item.DelegateSODTask,
                           DelegationId = item.Id,
                           EndDate = item.EndDate,
                           IsActive = item.IsActive,
                           StartDate = item.StartDate,
                           DelegationComments = item.DelegationComments,
                           CreatedBy = cuser != null ? cuser.DisplayName : item.CreatedBy,
                           LastModifiedBy = muser != null ? muser.DisplayName : item.LastModifiedBy,
                           LastModifiedDate = item.LastModifiedDate
                       };
            var newwlyAdded = from item in CurrentContext.TaskDelegations
                              join user in CurrentContext.User
                                  on item.DelegatedTo equals user.Id
                              join user1 in CurrentContext.User
                                  on item.ActionUser equals user1.Id
                              join cuser in CurrentContext.User
                                    on item.CreatedBy equals cuser.UserName into cusers
                              from cuser in cusers.DefaultIfEmpty()

                              where (item.EndDate >= DateTime.Today || includeInActive) && item.CreatedBy != null && item.LastModifiedBy == null &&
                              (isAdmin ? true : item.ActionUser == userId) && (includeInActive || item.IsActive)
                              select new PresentationTaskdelegation
                              {
                                  Id = item.Id,
                                  ActionUser = item.ActionUser,
                                  ActionUserName = user1.DisplayName,
                                  CreatedDate = item.CreatedDate,
                                  DelegateAttestationTask = item.DelegateAttestationTask,
                                  DelegatedToUserId = item.DelegatedTo,
                                  DelegatedToUserName = user.DisplayName,
                                  DelegateProvisioningTask = item.DelegateProvisioningTask,
                                  DelegateSODTask = item.DelegateSODTask,
                                  DelegationId = item.Id,
                                  EndDate = item.EndDate,
                                  IsActive = item.IsActive,
                                  StartDate = item.StartDate,
                                  DelegationComments = item.DelegationComments,
                                  CreatedBy = cuser != null ? cuser.DisplayName : item.CreatedBy,
                                  LastModifiedBy = null,
                                  LastModifiedDate = item.LastModifiedDate
                              };
            query = query.Union(newwlyAdded).Distinct();
            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                query = query.Where(filterExpression);
            }
            result.TotalCount = query.Count();
            if (!string.IsNullOrWhiteSpace(sortExpression))
            {
                query = query.OrderBy(sortExpression);
            }
            else
            {
                query = query.OrderByDescending(i => i.IsActive);
            }
            if(maximumRow > 0)
            {
                query = query.Skip(startIndex).Take(maximumRow);
            }
            result.Data = query.Distinct().ToList();
            return result;
        }

        /*
        public PresentationGridPagedData<PresentationTaskdelegation> GetPagedAllDelegationByActionUser(long userId, bool isAdmin, string sortExpression, string filterExpression, int startIndex, int maximumRow)
        {
            return GetPagedAllDelegationByActionUser(userId, isAdmin, false, sortExpression, filterExpression, startIndex, maximumRow);
        }
        */
        public List<PresentationTaskdelegation> GetDelegationsByAdmin()
        {
            return (from item in CurrentContext.TaskDelegations
                    join user in CurrentContext.User
                        on item.DelegatedTo equals user.Id
                    join user1 in CurrentContext.User
                        on item.ActionUser equals user1.Id
                    join cuser in CurrentContext.User
                        on item.CreatedBy equals cuser.UserName into cusers
                    from cuser in cusers.DefaultIfEmpty()
                    join muser in CurrentContext.User
                        on item.LastModifiedBy equals muser.UserName into musers
                    from muser in musers.DefaultIfEmpty()
                    where item.EndDate >= DateTime.Today
                    orderby item.IsActive descending
                    select new PresentationTaskdelegation
                    {
                        Id = item.Id,
                        ActionUser = item.ActionUser,
                        ActionUserName = user1.DisplayName,
                        CreatedDate = item.CreatedDate,
                        DelegateAttestationTask = item.DelegateAttestationTask,
                        DelegatedToUserId = item.DelegatedTo,
                        DelegatedToUserName = user.DisplayName,
                        DelegateProvisioningTask = item.DelegateProvisioningTask,
                        DelegateSODTask = item.DelegateSODTask,
                        DelegationId = item.Id,
                        EndDate = item.EndDate,
                        IsActive = item.IsActive,
                        StartDate = item.StartDate,
                        DelegationComments = item.DelegationComments,
                        CreatedBy = cuser.DisplayName,
                        LastModifiedBy = muser.DisplayName,
                        LastModifiedDate = item.LastModifiedDate
                    }).ToList();
        }
        public List<PresentationTaskdelegation> GetDelegationsByUserId(long userId)
        {
            return (from item in CurrentContext.TaskDelegations
                    join user in CurrentContext.User
                        on item.DelegatedTo equals user.Id
                    join user1 in CurrentContext.User
                        on item.ActionUser equals user1.Id
                    join cuser in CurrentContext.User
                        on item.CreatedBy equals cuser.UserName into cusers
                    from cuser in cusers.DefaultIfEmpty()
                    join muser in CurrentContext.User
                        on item.LastModifiedBy equals muser.UserName into musers
                    from muser in musers.DefaultIfEmpty()
                    where item.ActionUser == userId && item.EndDate >= DateTime.Today
                    orderby item.IsActive descending
                    select new PresentationTaskdelegation
                    {
                        Id = item.Id,
                        ActionUser = item.ActionUser,
                        ActionUserName = user1.DisplayName,
                        CreatedDate = item.CreatedDate,
                        DelegateAttestationTask = item.DelegateAttestationTask,
                        DelegatedToUserId = item.DelegatedTo,
                        DelegatedToUserName = user.DisplayName,
                        DelegateProvisioningTask = item.DelegateProvisioningTask,
                        DelegateSODTask = item.DelegateSODTask,
                        DelegationId = item.Id,
                        EndDate = item.EndDate,
                        IsActive = item.IsActive,
                        StartDate = item.StartDate,
                        DelegationComments = item.DelegationComments,
                        CreatedBy = cuser.DisplayName,
                        LastModifiedBy = muser.DisplayName,
                        LastModifiedDate = item.LastModifiedDate
                    }).ToList();
        }
        public List<PresentationTaskDelegationReport> GetTaskDelegationsForReport()
        {
            return (from delegation in CurrentContext.TaskDelegations
                    join actionUser in CurrentContext.User
                            on delegation.ActionUser equals actionUser.Id
                    select new PresentationTaskDelegationReport
                    {
                        Id = delegation.Id,
                        ActionUserName = actionUser.UserName,
                        DelegatedToUserName = delegation.DelegatedToUser.UserName,
                        Active = delegation.IsActive,
                        DelegateAttestationTasks = delegation.DelegateAttestationTask ? "Yes" : "No",
                        DelegateProvisioningTasks = delegation.DelegateProvisioningTask ? "Yes" : "No",
                        DelegationId = delegation.Id,
                        StartDate = delegation.StartDate,
                        EndDate = delegation.EndDate,
                        DelegationDate = delegation.CreatedDate,
                        DelegationCreatedBy = delegation.CreatedBy,
                        DelegationComments = string.IsNullOrEmpty(delegation.DelegationComments) ? string.Empty : delegation.DelegationComments.Replace("\n", "<br>")
                    }).ToList();

        }

        public TaskDelegation GetByUserId(long userId, RequestType requestType)
        {
            try
            {
                DateTime today = DateTime.Now.Date;

                if (requestType == RequestType.Attestation)
                {
                    return CurrentContext.TaskDelegations.FirstOrDefault(item => item.ActionUser == userId && item.IsActive == true && item.StartDate <= today &&
                    item.EndDate >= today && item.DelegateAttestationTask == true);
                }
                else if (requestType == RequestType.Provisioning)
                {
                    return CurrentContext.TaskDelegations.FirstOrDefault(item => item.ActionUser == userId && item.IsActive == true && item.StartDate <= today &&
                    item.EndDate >= today && item.DelegateProvisioningTask == true);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }


    }
}
