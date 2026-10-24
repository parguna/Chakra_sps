using DI.Chakra.Infrastructure;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class ApproverExceptionInfo : DomainModelBase<DomainModel.ApproverExceptionInfo, EntityModel.ApproverExceptionInfo>, IXLData
    {
        public string Person { get; set; }
        public string Approver { get; set; }
        public int ApprovalLevel { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
        public bool IsEditMode
        {
            get
            {
                return Id > 0;
            }
        }

        public static bool CheckPesronHasDiffrentapproversForSameLevel(ApproverExceptionInfo obj)
        {
            if (obj.IsEditMode)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var exitingobj = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.Person == obj.Person && item.ApprovalLevel == obj.ApprovalLevel && item.Id != obj.Id).FirstOrDefault();
                    if (exitingobj != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var exitingobj = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.Person == obj.Person && item.ApprovalLevel == obj.ApprovalLevel).FirstOrDefault();
                    if (exitingobj != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public static bool CheckPesronHasSameapproversForSameLevel(ApproverExceptionInfo obj)
        {
            if (obj.IsEditMode)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var exitingobj = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.Person == obj.Person && item.ApprovalLevel == obj.ApprovalLevel && item.Approver == obj.Approver && item.Id != obj.Id).FirstOrDefault();
                    if (exitingobj != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var exitingobj = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.Person == obj.Person && item.ApprovalLevel == obj.ApprovalLevel && item.Approver == obj.Approver).FirstOrDefault();
                    if (exitingobj != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).ExecuteQuery("Truncate table DI_M_XLApproverExceptionInfo");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        public static ApproverExceptionInfo GetApproverExceptionInfobyObject(ApproverExceptionInfo obj)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var exitingobj = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.Person == obj.Person && item.Approver == obj.Approver && item.ApprovalLevel == obj.ApprovalLevel).FirstOrDefault();
                return ORMap(exitingobj);
            }
        }

        public static bool CheckPesronHasSameApproverDiffentLevel(ApproverExceptionInfo obj)
        {
            if (obj.IsEditMode)
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var exitingobj = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.Person == obj.Person && item.Approver == obj.Approver && item.Id != obj.Id).FirstOrDefault();
                    if (exitingobj != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            else
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    var exitingobj = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.Person == obj.Person && item.Approver == obj.Approver).FirstOrDefault();
                    if (exitingobj != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }

        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
                        //string userDisplayName = User.GetByUserName(lastModifiedDate.CreatedBy)?.DisplayName;
                        //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastCreatedDate.CreatedDate);
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    }
                    else
                        return string.Empty;
                }
            }
        }
        public static List<ApproverExceptionInfo> GetApproverException(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.ApproverExceptionInfo>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
    }
}
