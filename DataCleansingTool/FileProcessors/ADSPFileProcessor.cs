
using DeepIdentity.DataAccess;
using DeepIdentity.Facade;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Linq;
using System.Data;

namespace DataCleansingTool.FileProcessors
{
    public class ADSPFileProcessor : FileProcessorBase
    {

        UserFacade userFacade = new UserFacade();
        EndPointFacade endPointFacade = new EndPointFacade();
        ActiveDirectoryFacade adFacade = new ActiveDirectoryFacade();

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
                UserMappingXLData mappingXlData = this.GetMappingObjectFromData(dRow);
                if (mappingXlData != null)
                {
                    this.UploadLog += this.Process(mappingXlData);
                }
            }
        }

        private string Process(UserMappingXLData mappingXlData)
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
                var endpoint = endPointFacade.GetAllEndPointByType((int)EndpointType.ActiveDirectory).FirstOrDefault();
                //var endpoint = endPointFacade.GetEndPointByApplicationType(ApplicationType.IACMAD);
                var adUser = adFacade.GetADUserByCN(mappingXlData.EndpointKey, endpoint.EndpointSystemId);

                if (adUser != null)
                {
                    adUser.DIUserId = user.UserId;
                    adUser.IfUnmappedMapTo = "HOD";
                    adUser.MappingType = "Manual";
                    adUser.LastModifiedBy = "System";
                    adUser.LastModifiedDate = DateTime.Now;
                    adUser.AccountType = mappingXlData.AccountType;

                    adFacade.UpdateUser(adUser);
                }
                else
                {
                    log += string.Format("AD CN - {0} does not exists in AD.", mappingXlData.EndpointKey) + "\n";
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
