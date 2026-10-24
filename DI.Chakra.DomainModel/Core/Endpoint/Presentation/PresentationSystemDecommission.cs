using DI.Chakra.Logger;
using DI.Chakra.EntityModel;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class PresentationSystemDecommission
    {
        public long Id { get; set; }
        public long EndpointSystemId { get; set; }

        public DateTime DecommissionedDate;

        public string DecommissionedBy { get; set; }

        public string Comments { get; set; }

        public bool Status { get; set; }

        public static void StartDecommisson()
        {
            ProvisioningLogger idmsLogger  = null;
            EndPoint endPoint = null;
            DataTable decommissionList = null;
            try
            {
                decommissionList = EndPoint.GetDataTableByQuery("SELECT Id,EndpointSystemId,DecommissionedBy FROM DI_M_DECOMMISSION WHERE STATUS = 1 ");
                using (DataAccessTransaction transaction = new DataAccessTransaction())
                {
                    if (decommissionList != null && decommissionList.Rows.Count > 0)
                    {
                        
                        string decommissionedBy = string.Empty;
                        foreach (DataRow systemList in decommissionList.Rows)
                        {
                            var Id = Convert.ToInt64(systemList["Id"].ToString());
                            bool isSuccess = false;
                            try
                            {
                                
                                var epId = Convert.ToInt64(systemList["EndpointSystemId"].ToString());
                                decommissionedBy = Convert.ToString(systemList["DecommissionedBy"])??"System"; 
                                endPoint = EndPoint.GetById(epId);
                                if (endPoint != null && endPoint.IsActive)
                                {
                                    idmsLogger = new ProvisioningLogger(null, epId, endPoint.EndpointSystemTypeId, endPoint.DisplayName);

                                    EndPoint.ExecuteQueryReturnValue(string.Format("UPDATE DI_M_AccountReconcilation SET IsDeleted = 1,Islocked = 1,DeletedDate=GETDATE(),LastModifiedDate=GETDATE(),LastModifiedBy='{1}',Comments='System Decommission' WHERE EndpointSystemId={0} AND IsDeleted = 0;", epId, decommissionedBy));
                                    EndPoint.ExecuteQueryReturnValue(string.Format("UPDATE DI_M_EntitlementReconcilation SET IsDeleted=1,DeletedDate=GETDATE(),LastModifiedDate=GETDATE(),LastModifiedBy='{1}',Comments='System Decommission' WHERE EndpointSystemId={0} AND IsDeleted = 0", epId, decommissionedBy));
                                    EndPoint.ExecuteQueryReturnValue(string.Format("Update DI_M_Task Set Action = 6, ActionDate = GETDATE(), Remarks = 'System Decommission',LastModifiedDate=GETDATE(),LastModifiedBy='{1}'  WHERE AccessRequestId IN(select Id from DI_M_AccessRequest where EndpointSystemId = {0} AND Progressstatus IN(1,3,9,11,12,13)) and IsCompleted = 0 and IsActive = 1 and [Action] = 1", epId, decommissionedBy));
                                    EndPoint.ExecuteQueryReturnValue(string.Format("Update DI_M_AccessRequest Set RequestStatus=5, Progressstatus=8,Comments='System Decommission',LastModifiedDate=GETDATE(),LastModifiedBy='{1}' WHERE EndpointSystemId = {0} AND Progressstatus IN(1,4,6,7,3,9,11,12,13)", epId, decommissionedBy));
                                    EndPoint.ExecuteQueryReturnValue($"Update DI_M_Scheduler Set [occurence]=5,LastModifiedDate=GETDATE(),LastModifiedBy='{decommissionedBy}' where Id in(select Scheduler_SchedulerId from DI_M_Process where ProcessTypeKeyId in(select Id from DI_M_DiscoveryProcess where Endpointsystemid={epId}))");
                                    EndPoint.ExecuteQueryReturnValue(string.Format("Update DI_M_EndpointSystem Set IncludeInAttestation = 0,IsActive=0,LastModifiedDate=GETDATE(),LastModifiedBy='{1}' where Id={0}", epId, decommissionedBy));
                                    EndPoint.ExecuteQueryReturnValue(string.Format("Update DI_M_WorkflowConfiguration Set IsActive=0,LastModifiedDate=GETDATE(),LastModifiedBy='{1}' where EndpointSystemId={0}", epId, decommissionedBy));
                                    if(endPoint.ApplicationType == Infrastructure.Enums.ApplicationType.InhouseSystems)
                                    {
                                        EndPoint.ExecuteQueryReturnValue($"DELETE FROM DI_INH_UserPermissionFunction WHERE  EndpointSystemId = {epId}");
                                        EndPoint.ExecuteQueryReturnValue($"DELETE FROM DI_INH_Permission WHERE EndPointSystemId={epId}");
                                        EndPoint.ExecuteQueryReturnValue($"Update DI_INH_UserInfo Set IsDeleted = 1, IsDisabled=1,IsLocked = 1, LastModifiedDate=GETDATE(),LastModifiedBy='{decommissionedBy}' WHERE EndPointSystemId = {epId}");
                                    }
                                }
                                idmsLogger?.WriteLog(LogLevelType.Warning, $"System - {endPoint?.DisplayName} decommissioned successfully and initiated by {decommissionedBy}");
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                idmsLogger?.WriteLog(LogLevelType.Exception, $"Error while StartDecommisson System :{endPoint?.DisplayName}  {ex.Message}.", ex);
                                isSuccess = false;
                            }
                            finally
                            {
                                if (isSuccess && Id > 0)
                                {
                                    EndPoint.ExecuteQueryReturnValue($"Update DI_M_DECOMMISSION Set STATUS = 0 where Id = {Id}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                idmsLogger?.WriteLog(LogLevelType.Exception, "Error while StartDecommisson System.", ex);
            }
            finally
            {
                decommissionList = null;
                idmsLogger = null;
                endPoint = null;
            }
        }


        public static string InsertDecommission(PresentationSystemDecommission  Decommissionvalues)
        {
            using (DataAccessTransaction transaction = new DataAccessTransaction())
            {
                return new PresentationSystemDecommissionRepository(transaction).InsertDecommission(new EntityModel.PresentationSystemDecommission() {Comments = Decommissionvalues.Comments, DecommissionedBy= Decommissionvalues.DecommissionedBy,DecommissionedDate = Decommissionvalues.DecommissionedDate, EndpointSystemId = Decommissionvalues.EndpointSystemId, Status  = Decommissionvalues.Status });
            }
        }

       
    }
}
