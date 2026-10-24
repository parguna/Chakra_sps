
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
    public class SAPTCodeInfoRepository : RepositoryBase<SAPTCodeInfo>
    {
        public SAPTCodeInfoRepository(DataAccessTransaction transaction) : base(transaction) { }

        public  Dictionary<long, string> GetAllSapTCodes(long endPointId)
        {
            return (from objInfo in CurrentContext.SAPTCodeInfo
                    where objInfo.EndPointSystemId == endPointId orderby objInfo.TCODE_Code
                    select new { objInfo.Id, objInfo.TCODE_Code }).Distinct().Take(100)
                         .ToDictionary(c => c.Id, c => c.TCODE_Code);
                      
        }

        public List<SAPTCodeInfo> GetSapRolesToTCodeData(string roleCode, long endpointSystemId)
        {
            return (from RoleToAuth in CurrentContext.SapRoleToAuthMap
                    join TCodes in CurrentContext.SAPTCodeInfo on RoleToAuth.FromValue equals TCodes.TCODE_Code
                    where (RoleToAuth.RoleName == roleCode && RoleToAuth.ObjectCode == "S_TCODE"
                        && RoleToAuth.FieldCode == "TCD" && TCodes.EndPointSystemId == endpointSystemId
                        && RoleToAuth.EndPointSystemId == endpointSystemId)
                    select TCodes
                  ).ToList();
        }

        public List<SAPTCodeInfo> GetSapRolesToTCodeDataWithRoles(string roleCode, long endpointSystemId)
        {
            return (from RoleToAuth in CurrentContext.SapRoleToAuthMap
                    join TCodes in CurrentContext.SAPTCodeInfo on RoleToAuth.FromValue equals TCodes.TCODE_Code
                    where (RoleToAuth.RoleName == roleCode && RoleToAuth.ObjectCode == "S_TCODE"
                 && RoleToAuth.FieldCode == "TCD" && TCodes.EndPointSystemId == endpointSystemId)
             select TCodes
                   ).ToList();
        }


        public List<PresentationSAPTCodeReport> GetAllSapTCodesReportData(long endpointSystemId)
        {
            return (from sapTcode in CurrentContext.SAPTCodeInfo
                    where sapTcode.EndPointSystemId == endpointSystemId
                    select new PresentationSAPTCodeReport

                    {
                        TCODE_Id = (int)sapTcode.Id,
                        TCODE_Code = sapTcode.TCODE_Code,
                        TCODE_Desc = sapTcode.TCODE_Desc

                    }
                   )
                   .ToList();
        }

        public Dictionary<long, string> GetAllAssignedTCodes(long endPointSystemId,string objectCode,string fieldCode)
        {
            return (from authObject in CurrentContext.SapRoleToAuthMap
                    where authObject.EndPointSystemId == endPointSystemId && authObject.ObjectCode == "S_TCODE" && authObject.FieldCode == "TCD"
                    join tCodes in CurrentContext.SAPTCodeInfo on authObject.FromValue equals tCodes.TCODE_Code
                    select tCodes)
                        .Select(tCode => new { key = tCode.Id, value = tCode.TCODE_Code })
                        .Take(100)
                        .Distinct().ToDictionary(dictionary => dictionary.key, dictionary => dictionary.value);
        }
        public Dictionary<long, string> GetSAPTCodeBySearchString(long endpointId, string searchString)
        {
            Dictionary < long, string> sapTCodes = (from sapTcode in CurrentContext.SAPTCodeInfo
                    where sapTcode.TCODE_Code.Contains(searchString) && sapTcode.EndPointSystemId == endpointId
                    select new { sapTcode.Id, sapTcode.TCODE_Code }).Distinct().Take(100).OrderBy(s => s.TCODE_Code)
                       .ToDictionary(c => c.Id, c => c.TCODE_Code);
            return sapTCodes;
        }

        public List<PresentationSAPTCodeReport> GetTcodeInfo(long endpointSystemId, string roleCode)
        {
            List<PresentationSAPTCodeReport> data = null;
            var query = $@"select B.Id as TCODE_Id,B.TCODE_Code,TCODE_Desc,T.EndPointSystemId,NULL as Institution,NULL as HostName from DI_SAP_ROLETCODEMAPPING T inner join DI_SAP_TCODEINFO B on T.TCode = B.TCODE_Code where T.EndPointSystemId={endpointSystemId} and b.EndPointSystemId={endpointSystemId} AND T.RoleCode='{roleCode.Replace("'","''")}' ORDER BY B.TCODE_Code";
            data = CurrentContext.Database.SqlQuery<PresentationSAPTCodeReport>(query).ToList();
            return data;
        }

        public List<PresentationSAPRoleTCode> GetRoleTCodeForReport(long endpointId)
        {
            var roleInfos = CurrentContext.SAPRoleInfos.Where(i=> i.EndPointSystemId == endpointId);
            var enPoints = CurrentContext.EndPoint.Where(i => i.Id == endpointId);
            var sapTcodes = CurrentContext.SapRoleToAuthMap.Where(i => i.EndPointSystemId == endpointId && i.ObjectCode == "S_TCODE");
            var tcodes = CurrentContext.SAPTCodeInfo.Where(i => i.EndPointSystemId == endpointId);
            
            CurrentContext.Database.CommandTimeout = 0;
            List<PresentationSAPRoleTCode> presentationSAPRoleTCodes = new List<PresentationSAPRoleTCode>();

            var query = (from sapTcode in sapTcodes
                         join enPoint in enPoints on sapTcode.EndPointSystemId equals enPoint.Id
                         join roleInfo in roleInfos on sapTcode.RoleName equals roleInfo.ROLE_CODE
                         join tcode in tcodes on sapTcode.FromValue equals tcode.TCODE_Code into tCodes
                         from tcode in tCodes.DefaultIfEmpty()
                         select new PresentationSAPRoleTCode
                         {
                             EndpointName = enPoint.DisplayName,
                             RoleName = roleInfo.ROLE_CODE,
                             RoleDescription = roleInfo.ROLE_DESC,
                             TCODE_Code =  (sapTcode.FromValue + ((sapTcode.ToValue != null && sapTcode.ToValue.Length > 0) ? " - "+sapTcode.ToValue : "")),
                             TCODE_Desc = (sapTcode.FromValue.Contains("*") || (sapTcode.ToValue != null && sapTcode.ToValue.Length > 0)) ? "Multiple TCode Values" : tcode.TCODE_Desc
                                        }).Distinct().OrderBy(i=> i.RoleName).ThenBy(i=> i.TCODE_Code);

            var count = query.Count();
            int pageSize = 10000;
            var totalPage = (int)Math.Ceiling((double)count / pageSize);
            for (int page = 0; page < totalPage; page++)
            {
                presentationSAPRoleTCodes.AddRange(query.Skip(page * pageSize).Take(pageSize).ToList());
            }
            return presentationSAPRoleTCodes.Distinct().ToList();
        }
    }
}
