using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class SAPHANAXLPositionsMatrix: DomainModelBase<DomainModel.SAPHANAXLPositionsMatrix, EntityModel.SAPHANAXLPositionsMatrix>, IXLData, IApprover
    {
        public long EndpointSystemId { get; set; }
        public string Application { get; set; }
        public string Remarks { get; set; }
        public string PositionText { get; set; }
        public string Position { get; set; }
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
            get; set;
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
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static SAPHANAXLPositionsMatrix GetSAPHANAXLPositionsMatrixbyObject(SAPHANAXLPositionsMatrix obj)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    return ORMap(new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction)
                        .Get(cnfg => cnfg.EndpointSystemId == obj.EndpointSystemId &&  cnfg.PositionText == obj.PositionText).FirstOrDefault());
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
                    return new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction)
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
        public static List<SAPHANAXLPositionsMatrix> GetAllSAPHANAXLPositionMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                    {
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
                var matrix = new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).GetById(ID);
                if (matrix == null) return string.Empty;
                return string.Format("{0}", string.IsNullOrEmpty(matrix.Position) ? string.Empty : matrix.Position);
            }
        }

        public static SAPHANAXLPositionsMatrix GetSAPHANAPositionMatrixByPosition(long endpointSystemId, string position)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap( new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).Get(item => item.EndpointSystemId == endpointSystemId && item.PositionText.Equals(position, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
                
            }
        }
        public static SAPHANAXLPositionsMatrix GetSAPHANAPositionMatrixByPositionCode(string positionCode,long endpointSystemId)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).Get(item => item.EndpointSystemId == endpointSystemId && item.Position.Equals(positionCode, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault());
            }
        }
        public static string GetPrefixSAPHANA(long ID)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var matrix = new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).GetById(ID);
                return string.Format("{0},{1}", string.IsNullOrEmpty(matrix.Prefix_Employees) ? string.Empty : matrix.Prefix_Employees,
                    string.IsNullOrEmpty(matrix.Prefix_TempStaff) ? string.Empty : matrix.Prefix_TempStaff);
            }
        }

        public static IList<SAPHANAXLPositionsMatrix> BindPositionsSAPHANA(long endpointSystemId, string prefix, EmployeeType employeeType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                IList<SAPHANAXLPositionsMatrix> data = new List<SAPHANAXLPositionsMatrix>();
                IEnumerable<EntityModel.SAPHANAXLPositionsMatrix> result;
                result = new RepositoryBase<EntityModel.SAPHANAXLPositionsMatrix>(dataAccessTransaction).Get(source => source.EndpointSystemId == endpointSystemId &&!string.IsNullOrEmpty(source.Position))
                        .OrderBy(source => source.PositionText).ToList();

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
