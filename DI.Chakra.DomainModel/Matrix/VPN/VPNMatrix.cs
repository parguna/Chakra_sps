using DI.Chakra.DBClient;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;

namespace DI.Chakra.DomainModel
{
    public class VPNMatrix : DomainModelBase<VPNMatrix, EntityModel.VPNMatrix>, IXLData,IEndpointEntitlementInfo, IApprover
    {
        [Required]
        public string Name { get; set; }
        public string ProductionADgroups { get; set; }
        public string QAADGroups { get; set; }
        public string DevelpomentADGroups { get; set; }
        public string ADGroups { get; set; }
        public VPNMatrixType VPNMatrixType { get; set; }
        public string Level1Approver { get; set; }
        public ApprovalUserType Level1ApproverType { get; set; }
        public string Level2Approver { get; set; }
        public ApprovalUserType Level2ApproverType { get; set; }
        public string Level3Approver { get; set; }
        public ApprovalUserType Level3ApproverType { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string ValidationMessage { get; set; }
        public bool IsActive {
            get
            {
                return IsValid;
            }
            set
            {
                IsValid = value;
            }
        }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public long EndPointSystemId { get; set; }
        public string BusinessRole
        {
            get { return this.Name; }
        }

        public string SystemName
        {
            get { return ApplicationType.VPN.GetDescription(); }
        }
        public string EntitlementName
        {   get
            {
                return this.Name;
            }
            set
            {
                Name = value;
            }
        }

        public string EntitlementDesc
        {
            get
            {
                return this.Name;
            }
            set
            {
                Name = value;
            }
        }

        public EntitlementType EntitlementType
        {
            get;
            set;
        }

        public static VPNMatrix GetByName(string name,VPNMatrixType type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(Item => Item.Name == name && Item.VPNMatrixType == type &&
                Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).FirstOrDefault());
            }
        }
        public static VPNMatrix GetByIdAndType(long id, VPNMatrixType type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(Item => Item.Id == id && Item.VPNMatrixType == type &&
                Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).FirstOrDefault());
            }
        }
        public static List<VPNMatrix> GetActiveValidApplication()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(Item => Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).ToList());
            }
        }
        public static string DeleteByMatrixId(long id)
        {
            
            string strMsg = string.Empty;
            if (id == 0)
            {
                strMsg = "Invalid Id. Matrix cannot be removed";
            }
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var vpnMatrix = VPNMatrix.GetById(id);
                var vpnUser = new RepositoryBase<EntityModel.VPNUserApplicationEnviornment>(dataAccessTransaction).Get(i => i.MatrixId == id);
                if(vpnUser != null && vpnUser.Count() == 0)
                {
                    var vpnApplication = VPNApplicationEnvironment.GetByMatrixId(id);
                    if(vpnApplication.Count > 0)
                    {
                        VPNApplicationEnvironment.DeleteList(vpnApplication);
                    }
                    VPNMatrix.Delete(id);
                    strMsg = $"Selected matrix:{vpnMatrix?.Name} removed successfully";
                }
                else
                {
                    strMsg = $"Selected Matrix mapped to the user. Matrix:{vpnMatrix?.Name} cannot be removed.";
                }
            }
            return strMsg;
        }
        public static VPNMatrix GetApplication(string name, VPNMatrixType type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(Item => Item.Name == name && Item.VPNMatrixType == type && Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).FirstOrDefault());
            }
        }
        public static VPNMatrix GetByType(VPNMatrixType type)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMap(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(Item => Item.VPNMatrixType == type &&
                Item.ValidFrom <= DateTime.Now && Item.ValidTo >= DateTime.Now && Item.IsActive).FirstOrDefault());
            }
        }
        public static List<VPNMatrix> GetByIds(List<long> ids)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(Item => ids.Contains(Item.Id)).ToList());
            }
        }
        public static string GetWhoUpdatedData()
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            {
                var lastModifiedDate = new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.LastModifiedDate).FirstOrDefault();
                var lastCreatedDate = new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).GetAll().OrderByDescending(source => source.CreatedDate).FirstOrDefault();
                if (lastModifiedDate != null && lastModifiedDate.LastModifiedDate != null && lastModifiedDate.LastModifiedDate > lastCreatedDate?.CreatedDate)
                {
                    return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastModifiedDate.LastModifiedBy, lastModifiedDate.LastModifiedDate);
                    //string userDisplayName = User.GetByUserName(lastModifiedDate.LastModifiedBy)?.DisplayName;
                    //return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, userDisplayName, lastModifiedDate.LastModifiedDate);
                }
                else
                {
                    if (lastCreatedDate != null && lastCreatedDate.CreatedDate != null)
                        return string.Format(Constants.MATRIX_WHO_UPDATED_MESSAGE, lastCreatedDate.CreatedBy, lastCreatedDate.CreatedDate);
                    else
                        return string.Empty;
                }
            }
        }
        public static bool TruncateTable()
        {
            bool returnValue = true;
            //try
            //{
            //    using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
            //    {
            //        new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).ExecuteQuery("Truncate table DI_VPN_Matrix");
            //        dataAccessTransaction.Commit();
            //        returnValue = true;
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            return returnValue;
        }
        public static List<VPNMatrix> GetAllVPNMatrix(bool isValidRecords, bool isAllRecords)
        {
            try
            {
                using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction())
                {
                    if (isAllRecords)
                        return ORMapList(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).GetAll().ToList());
                    else
                    {
                        if (isValidRecords)
                            return ORMapList(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(item => item.ValidTo.HasValue && item.ValidTo.Value.Year == DateTime.MaxValue.Year).ToList());
                        else
                            return ORMapList(new RepositoryBase<EntityModel.VPNMatrix>(dataAccessTransaction).Get(item => item.ValidTo != DateTime.MaxValue).ToList());
                    }
                }
            }
            catch { throw; }
        }
        public static List<VPNMatrix> GetVPNADGroups(string adGroupName)
        {
            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                return ORMapList(new VPNUserInfoRepository(dataAccessTransaction).GetVPNADGroups(adGroupName));
            }
        }
    }
}
