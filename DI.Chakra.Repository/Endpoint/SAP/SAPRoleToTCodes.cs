using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Transaction;

namespace DI.Chakra.Repository
{
    public class SAPRoleToTCodesRepository : RepositoryBase<EntityModel.SAPRoleToTCodes>
    {

        public SAPRoleToTCodesRepository(DataAccessTransaction transaction) : base(transaction)
        {
        }

        public void GenerateRoleToTCodesByEndpoint(long endpointSystemId)
        {

            IQueryable<EntityModel.SapRoleTCodeMapping> sapRoles = null;
            List<IGrouping<string, EntityModel.SapRoleTCodeMapping>> distinctRoles = new List<IGrouping<string, EntityModel.SapRoleTCodeMapping>>();
            
            try
            {
                sapRoles = CurrentContext.SapRoleTCodeMap.Where(i => i.EndPointSystemId == endpointSystemId);
                distinctRoles = sapRoles.Where(i=> i.RoleCode != null).GroupBy(i => i.RoleCode).ToList();

                foreach (var role in distinctRoles)
                {
                    try
                    {
                        var strRole = role.Key.Replace("'", "''");
                        var tCodes = string.Join(",", sapRoles.Where(i => i.RoleCode == role.Key).Select(i => i.TCode).ToList()).Replace("'", "''");
                        ExecuteQuery($"IF EXISTS(SELECT 1 FROM DBO.DI_SAP_RoleToTCodes WHERE EndPointSystemId = {endpointSystemId} AND RoleCode='{strRole}') BEGIN " +
                            $" UPDATE   DBO.DI_SAP_RoleToTCodes  SET TCODES = '{tCodes}', LASTMODIFIEDBY = 'System', LASTMODIFIEDDATE = GETDATE() WHERE EndPointSystemId = {endpointSystemId} AND EndPointSystemId={endpointSystemId} AND RoleCode='{strRole}'" +
                            $"    END ELSE BEGIN" +
                            $" INSERT INTO DBO.DI_SAP_RoleToTCodes(EndPointSystemId,ROLECODE,TCODES,CREATEDBY,CREATEDDATE) VALUES({endpointSystemId},'{strRole}','{tCodes}','System',GETDATE())" +
                            $" END");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sapRoles = null;
                distinctRoles?.Clear();
                distinctRoles = null;
            }
            
        }
    }
}
