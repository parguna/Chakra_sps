
using DI.Chakra.DataAccess;
using DI.Chakra.Facade;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Model;
using System;
using System.Linq;
using System.Data;

namespace DataCleansingTool.FileProcessors
{
    public class GRCFileProcessor : FileProcessorBase
    {

        UserFacade userFacade = new UserFacade();
        EndPointFacade endPointFacade = new EndPointFacade();
        SAPFacade sapFacade = new SAPFacade();

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
                var endpoint = endPointFacade.GetAllEndPointByType((int)EndpointType.SAP).FirstOrDefault();
                //var endpoint = endPointFacade.GetEndPointByApplicationType(ApplicationType.SAP);
                var sapUser = sapFacade.GetSAPUserByUserName(mappingXlData.EndpointKey, endpoint.Id);

                if (sapUser != null)
                {
                    sapUser.DIUserId = user.Id;
                    sapUser.IfUnmappedMapTo = "HOD";
                    sapUser.MappingType = "Manual";
                    sapUser.LastModifiedBy = "System";
                    sapUser.LastModifiedDate = DateTime.Now;
                    sapUser.AccountType = mappingXlData.AccountType;

                    sapFacade.UpdateSAPUser(sapUser);
                }
                else
                {
                    log += string.Format("SAP Login Name - {0} does not exists in {1}.", mappingXlData.EndpointKey, endpoint.DisplayName) + "\n";
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
