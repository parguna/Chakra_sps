using DI.Chakra.DBClient;
using DI.Chakra.EntityModel;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.Repository
{
    public class PresentationSystemDecommissionRepository
    {
        public PresentationSystemDecommissionRepository(DataAccessTransaction transaction) { }
        IACMDBContext CurrentContext = new IACMDBContext();
        public string InsertDecommission(PresentationSystemDecommission Decommissionvalues)
        {
            var status = string.Empty;
            try
            {
                //string query = $"if not exists(select 1  from  [dbo].[DI_M_Decommission] where EndpointSystemId = @EndpointId and Status = '{Decommissionvalues.Status}') " +
                    //$"INSERT INTO[dbo].[DI_M_Decommission] (EndpointSystemId, DecommissionedBy,Comments,Status,DecommissionedDate) VALUES(@EndpointId{Decommissionvalues.EndpointSystemId},'{Decommissionvalues.DecommissionedBy}','{Decommissionvalues.Comments}','{Decommissionvalues.Status}','{Decommissionvalues.DecommissionedDate}')";

                string query = $"if not exists(select 1  from  [dbo].[DI_M_Decommission] where EndpointSystemId = @EndpointId and Status = @Status) " +
                    $"INSERT INTO[dbo].[DI_M_Decommission] (EndpointSystemId, DecommissionedBy,Comments,Status,DecommissionedDate) VALUES(@EndpointId,@DecommissionedBy,@Comments,@Status,GETDATE())";

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter() { ParameterName= "@EndpointId", SqlDbType= System.Data.SqlDbType.BigInt, Value = Decommissionvalues.EndpointSystemId });
                parameters.Add(new SqlParameter() { ParameterName = "@Status", SqlDbType = System.Data.SqlDbType.Bit, Value = Decommissionvalues.Status });
                parameters.Add(new SqlParameter() { ParameterName = "@DecommissionedBy", SqlDbType = System.Data.SqlDbType.NVarChar, Size=300, Value = Decommissionvalues.DecommissionedBy });
                parameters.Add(new SqlParameter() { ParameterName = "@Comments", SqlDbType = System.Data.SqlDbType.NVarChar, Size = 200, Value = Decommissionvalues.Comments });
                var isSuccess = CurrentContext.Database.ExecuteSqlCommand(query, parameters.ToArray());
                if(isSuccess > 0)
                {
                    status = isSuccess.ToString();
                }
                else
                {
                    status = "Endpoint already exists in Decommission List";
                }
            }
            catch (Exception ex)
            {
                status = "Failed to create Decommission Entry";
            }
            return status;
        }
    }
}
