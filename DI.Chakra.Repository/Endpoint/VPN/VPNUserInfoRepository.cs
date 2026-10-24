using DI.Chakra.EntityModel;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Repository
{
    public class VPNUserInfoRepository : RepositoryBase<VPNUserInfo>
    {
        public VPNUserInfoRepository(DataAccessTransaction transaction) : base(transaction) { }
        public List<PresentationVPNUserInfo> GetByEndPointSystemId(long endPointSystemId)
        {
            return (from vpnUser in CurrentContext.VPNUserInfo
                    join user in CurrentContext.User on vpnUser.UserId equals user.Id
                    where vpnUser.EndPointSystemId == endPointSystemId && user.IsActive == true
                    select new PresentationVPNUserInfo
                    {
                        EndpointUserId = vpnUser.Id,
                        UserId = user.Id,
                        OneKeyTokenId = user.OneKeyTokenId,
                        UserName = user.EmployeeId,
                        DisplayName = user.DisplayName,
                        EmployeeType =(int)user.EmployeeType,
                    }
                    ).ToList();
        }
        public void InsertVPNOneKeyTokenMapping(long endPointSystemId, long userId, long oneKeyId)
        {
            string cmd = string.Format("INSERT INTO [dbo].[VPNUserInfoVPNOneKeyTokenInfo] ([VPNUserInfo_Id],[VPNOneKeyTokenInfo_Id],[EndpointSystemId]) VALUES({0}, {1},{2})", userId, oneKeyId, endPointSystemId);
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public void DeleteOneKeyToken(long userid, long oneKeyId, long endpointsystemid)
        {
            string cmd = $"DELETE FROM [VPNUserInfoVPNOneKeyTokenInfo] WHERE EndpointSystemId = {endpointsystemid } AND  VPNUserInfo_Id = {userid } AND  VPNOneKeyTokenInfo_Id = {oneKeyId}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public void ReomveAllUseroneyKeyToken()
        {
            string cmd = $"TRUNCATE TABLE [VPNUserInfoVPNOneKeyTokenInfo]";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public void DeleteByUserId(long userid)
        {
            string cmd = $"DELETE FROM [dbo.VPNUserApplicationEnviornment] WHERE EndpointUserId = {userid}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public void DeleteUserApplicationEnviornmentByMatrixId(long matrixId,long endPointUserId)
        {
            string cmd = $"DELETE FROM [dbo.VPNUserApplicationEnviornment] WHERE EndpointUserId = {endPointUserId} && MatrixId={matrixId}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }
        public void DeleteUserApplicationEnviornmentByEnviornment(long endPointUserId, long matrixId,int envType)
        {
            string cmd = $"DELETE FROM [dbo.VPNUserApplicationEnviornment] WHERE EndpointUserId = {endPointUserId} AND EnviornmentType={envType} AND MatrixId={matrixId}";
            this.CurrentContext.Database.ExecuteSqlCommand(cmd);
        }

        public List<VPNMatrix> GetVPNADGroups(string adGroupName)
        {
            var matrix = new List<VPNMatrix>();
            var appln = CurrentContext.EndPoint.FirstOrDefault(i => i.ApplicationType == Infrastructure.Enums.ApplicationType.VPN);
            if (appln != null && appln.IsActive)
            {

                matrix = (from VPNMatrix in CurrentContext.VPNMatrix

                        where ((VPNMatrix.ProductionADgroups != null && VPNMatrix.ProductionADgroups.Contains(adGroupName))
                        ||
                        (VPNMatrix.QAADGroups != null && VPNMatrix.QAADGroups.Contains(adGroupName))
                        || (VPNMatrix.DevelpomentADGroups != null && VPNMatrix.DevelpomentADGroups.Contains(adGroupName))
                        || (VPNMatrix.ADGroups != null && VPNMatrix.ADGroups.Contains(adGroupName))
                        )
                        select VPNMatrix).ToList().Select(i => new VPNMatrix()
                        {
                            Name = appln.DisplayName,
                            ADGroups = $"{i.ProductionADgroups ?? ""},{i.QAADGroups ?? ""},{i.DevelpomentADGroups ?? ""},{i.ADGroups ?? ""}"
                        }).ToList();
            }
            return matrix;
        }       
    }
}
