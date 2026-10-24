
using DeepIdentity.DataAccess;
using DeepIdentity.Facade;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
namespace DataCleansingTool.FileProcessors
{
    public class ApproverExceptionUploadFileProcessor : FileProcessorBase
    {

        OrganisationFacade orgFacade = new OrganisationFacade();

        public override void UploadData(string fileName)
        {
            string validationMsg = string.Empty;

            if (!this.IsValidFile(fileName, ref validationMsg))
            {
                this.UploadLog = validationMsg;
                return;
            }

            DataTable dt = new ExcelConnect().ReadExcel(fileName);

            if (dt.Rows.Count != 0)
            {
                orgFacade.FlushApproverExceptions();
            }

            foreach (DataRow dRow in dt.Rows)
            {
                ApproverExceptionData exceptionXlData = this.GetApproverExceptionObjectFromData(dRow);
                if (exceptionXlData != null)
                {
                    this.UploadLog += this.Process(exceptionXlData);
                }
            }
        }

        private string Process(ApproverExceptionData exceptionXlData)
        {
            string log = string.Empty;

            try
            {
                log += string.Format("Started saving Approver exception saved successfully for Person - {0}, Approver - {1}, Level - {2}",
                    exceptionXlData.Person, exceptionXlData.Approver, exceptionXlData.ApprovalLevel) + "\n";
                
                ApproverException exception = new ApproverException();

                exception.Person = exceptionXlData.Person;
                exception.Approver = exceptionXlData.Approver;
                exception.ApprovalLevel = exceptionXlData.ApprovalLevel;

                orgFacade.AddApproverException(exception);

                log += string.Format("Approver exception saved successfully for Person - {0}, Approver - {1}, Level - {2}",
                    exceptionXlData.Person, exceptionXlData.Approver, exceptionXlData.ApprovalLevel) + "\n";

            }
            catch (Exception ex)
            {
                log += string.Format("Error while saving approver exception for pernr - {0}, level - {1}, Error info - {2} - {3}", exceptionXlData.Person, exceptionXlData.ApprovalLevel, ex.Source, ex.Message) + "\n";
                log += string.Format("Stack Trace - {0}", ex.StackTrace) + "\n";
            }

            return log;
        }

    }
}
