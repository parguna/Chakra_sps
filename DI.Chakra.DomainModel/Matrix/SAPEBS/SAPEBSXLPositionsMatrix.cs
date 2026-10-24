using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPEBSXLPositionsMatrix: DomainModelBase<DomainModel.SAPEBSXLPositionsMatrix, EntityModel.SAPEBSXLPositionsMatrix>, IXLData, IApprover
    {
        public string Remarks { get; set; }
        public string PositionText { get; set; }
        public string Position_ISP { get; set; }
        public string Position_CRP { get; set; }
        public string Prefix_Employees { get; set; }
        public string Prefix_TempStaff { get; set; }
        public string User_Group { get; set; }
        public string Department_Field { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }

        public string BusinessRole
        {
            get { return this.PositionText; }
        }
        public string SystemName
        {
            get
            {
                return UtilityMethods.GetEnumAttribute<Description>(ApplicationType.None);
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public static bool TruncateTable()
        {
            bool returnValue = false;
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_SAP_EBS_XL_PositionsMatrix");
                    dataAccessTransaction.Commit();
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPEBSXLPositionsMatrix GetSAPEBSXLPositionsMatrixbyObject(SAPEBSXLPositionsMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.PositionText == obj.PositionText).FirstOrDefault());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static bool IsEmployeeConfiguredAsApprover(string employeeId)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction)
                        .Any(sap => sap.Level1Approver == employeeId
                       || sap.Level2Approver == employeeId
                       || sap.Level3Approver == employeeId);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static List<SAPEBSXLPositionsMatrix> GetAllSAPEBSXLPositionMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
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

        public static string GetPositionCode(long ID)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var matrix = new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).GetById(ID);
                if (matrix == null) return string.Empty;
                return string.Format("{0},{1}", string.IsNullOrEmpty(matrix.Position_ISP) ? string.Empty : matrix.Position_ISP,
                    string.IsNullOrEmpty(matrix.Position_CRP) ? string.Empty : matrix.Position_CRP);
            }
        }

        public static SAPEBSXLPositionsMatrix GetEBSPositionMatrixByPosition(string position)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap( new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).Get(item => item.PositionText.Equals(position, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
                
            }
        }
        public static SAPEBSXLPositionsMatrix GetEBSPositionMatrixByPositionCode(string positionCode,bool isCRP)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                if(isCRP)
                    return ORMap(new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).Get(item => item.Position_CRP.Equals(positionCode, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
                else
                    return ORMap(new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).Get(item => item.Position_ISP.Equals(positionCode, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
            }
        }
        public static string GetPrefixEBS(long ID)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var matrix = new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).GetById(ID);
                return string.Format("{0},{1}", string.IsNullOrEmpty(matrix.Prefix_Employees) ? string.Empty : matrix.Prefix_Employees,
                    string.IsNullOrEmpty(matrix.Prefix_TempStaff) ? string.Empty : matrix.Prefix_TempStaff);
            }
        }

        public static IList<SAPEBSXLPositionsMatrix> BindPositionsEBS(ApplicationType applicationType, string prefix, EmployeeType employeeType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPEBSXLPositionsMatrix> data = new List<SAPEBSXLPositionsMatrix>();
                IEnumerable<EntityModel.SAPEBSXLPositionsMatrix> result;
                if (applicationType == ApplicationType.EBS_ISU)
                {
                    result = new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).Get(source => !string.IsNullOrEmpty(source.Position_ISP))
                        .OrderBy(source => source.PositionText).ToList();
                }
                else
                {
                    result = new RepositoryBase<EntityModel.SAPEBSXLPositionsMatrix>(dataAccessTransaction).Get(source => !string.IsNullOrEmpty(source.Position_CRP))
                       .OrderBy(source => source.PositionText).ToList();
                }
                
                foreach (var item in result)
                {
                    if (data.Any(v => v.PositionText == item.PositionText))
                        continue;

                    if (!string.IsNullOrWhiteSpace(prefix))
                    {
                        if ((employeeType == EmployeeType.Permanent && item.Prefix_Employees == prefix) || (employeeType != EmployeeType.Permanent && item.Prefix_TempStaff == prefix))
                            data.Add(ORMap(item));
                    }
                    else
                        data.Add(ORMap(item));
                }

                return data;
            }
        }

    }
}
