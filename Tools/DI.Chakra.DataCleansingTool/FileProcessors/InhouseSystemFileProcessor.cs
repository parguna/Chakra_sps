
using DI.Chakra.DataAccess;
using DI.Chakra.Facade;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Linq;
using System.Data;

namespace DataCleansingTool.FileProcessors
{
    public class InhouseSystemFileProcessor : FileProcessorBase
    {

        UserFacade userFacade = new UserFacade();
        EndPointFacade endPointFacade = new EndPointFacade();
        InhouseSystemFacade inhouseSystemFacade = new InhouseSystemFacade();

        public override void UploadData(string fileName)
        {
            string validationMsg = string.Empty;

            if (!this.IsValidFile(fileName, ref validationMsg))
            {
                this.UploadLog = validationMsg;
                return;
            }

            DataTable dt = new ExcelConnect().ReadExcel(fileName);

            foreach (DataRow dRow in dt.Rows)
            {
                InhouseUserMappingXLData mappingXlData = this.GetInhouseMappingObjectFromData(dRow);
                if (mappingXlData != null)
                {
                    this.UploadLog += this.Process(mappingXlData);
                }
            }
        }

        private string Process(InhouseUserMappingXLData mappingXlData)
        {
            string log = string.Empty;

            log += string.Format("Started processing user - {0}.", mappingXlData.OwnerPernr) + "\n";

            string validPernr = mappingXlData.OwnerPernr.PadLeft(8, '0');

            //get user by pernr
            User user = userFacade.GetUserByEmployeeId(validPernr);

            if (user != null)
            {
                //get endpoint account by endpoint key
                //TEMP FIX
                var endpoint = endPointFacade.GetAllEndPointByType((int)EndpointType.InhouseSystems).FirstOrDefault();
                //var endpoint = endPointFacade.GetEndPointByApplicationType(ApplicationType.InhouseSystems);
                var inhouseUser = inhouseSystemFacade.GetInhouseInfoByUserNameAndSystemCode(mappingXlData.EndpointKey, mappingXlData.SystemCode);

                if (inhouseUser != null)
                {
                    inhouseUser.DIUserId = user.Id;
                    //inhouseUser.IfUnmappedMapTo = "HOD";
                    inhouseUser.MappingType = "Manual";
                    inhouseUser.LastModifiedBy = "System";
                    inhouseUser.LastModifiedDate = DateTime.Now;
                    inhouseUser.AccountType = mappingXlData.AccountType;

                    inhouseSystemFacade.UpdateInhouseUserInfo(inhouseUser);
                }
                else
                {
                    log += string.Format("NPAC User - {0} does not exists in NPAC.", mappingXlData.EndpointKey) + "\n";
                }
            }
            else
            {
                log += string.Format("Pernr - {0} does not exists in master user.", mappingXlData.OwnerPernr) + "\n";
            }

            log += string.Format("Completed processing pernr - {0}.", mappingXlData.OwnerPernr) + "\n";

            return log;
        }

    }
}
