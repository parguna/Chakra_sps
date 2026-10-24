
using DeepIdentity.DataAccess;
using DeepIdentity.Facade;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Linq;
using System.Data;

namespace DataCleansingTool.FileProcessors
{
    public class LNSFileProcessor : FileProcessorBase
    {

        UserFacade userFacade = new UserFacade();
        EndPointFacade endPointFacade = new EndPointFacade();
        LotusNotesFacade lnFacade = new LotusNotesFacade();

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
                //var endpoint = endPointFacade.GetEndPointByApplicationType(ApplicationType.LotusNotes);
                var endpoint = endPointFacade.GetAllEndPointByType((int)EndpointType.LotusNotes).FirstOrDefault();
                var lnUser = lnFacade.GetLNSUserByShortName(mappingXlData.EndpointKey);

                if (lnUser != null)
                {
                    lnUser.DIUserId = user.UserId;
                    lnUser.IfUnmappedMapTo = "HOD";
                    lnUser.MappingType = "Manual";
                    lnUser.LastModifiedBy = "System";
                    lnUser.LastModifiedDate = DateTime.Now;
                    lnUser.AccountType = mappingXlData.AccountType;

                    lnFacade.UpdateUser(lnUser);
                }
                else
                {
                    log += string.Format("Lotus Notes short name - {0} does not exists in Lotus notes.", mappingXlData.EndpointKey) + "\n";
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
