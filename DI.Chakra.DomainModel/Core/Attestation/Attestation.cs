using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.DomainModel
{
    public class Attestation : DomainModelBase<Attestation, EntityModel.Attestation>
    {
        public string AttestationName { get; set; }

        public string AttestationDescription { get; set; }
        public string CampaignNameFormat { get; set; }

        public bool RequiresAttestationRemediation { get; set; }

        public long WorkflowConfigurationId { get; set; }
        public bool IncludeUnmappedAccount { get; set; }
        public AttestationType AttestationType { get; set; }
        public AttestationScopeEnum SplitCampaignUsing { get; set; }
        public bool IncludeDormantAccount { get; set; }
        public long DormantAccountDays { get; set; }
        public bool CreateTaskOnNoReviewer { get; set; }
        public Nullable<long> TaskActionUserId { get; set; }
        public bool IsGroupTask { get; set; }
        public bool SendEmailOnNoReviewer { get; set; }
        public Nullable<long> EmailEntityId { get; set; }
        public bool IsGroupMail { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public Nullable<System.DateTime> LastModifiedDate { get; set; }

        public WorkflowConfiguration WorkflowConfiguration { get; set; }

        public List<AttestationProcess> AttestationProcesses { get; set; }

        public List<AttestationEndpoint> AttestationEndpoint { get; set; }

        public List<AttestationScope> AttestationScopes { get; set; } 

        public List<AttestationHierarchy> AttestationHierarchy { get; set; }

        public AttestationDefaultReviewStatus DefaultReviewStatus { get; set; }

        public static Attestation GetByIdIncludingAttestationProcesses(long id)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Attestation>(dataAccessTransaction)
                    .GetById(id,
                    attn => attn.AttestationProcesses,
                    attn => attn.AttestationProcesses.Select(x => x.process),
                    attn => attn.AttestationEndpoint,
                    attn => attn.AttestationEndpoint.Select(x => x.EndPoint),
                    attn => attn.AttestationEndpoint.Select(x => x.EndPoint.EndPointSystemType),
                    attn => attn.WorkflowConfiguration,
                    attn => attn.AttestationScopes,
                    attn => attn.AttestationHierarchy,
                    attn => attn.AttestationHierarchy.Select(x => x.EndpointHierarchy),
                    attn => attn.WorkflowConfiguration.WorkflowConfigurationDetails));
            }
                
        }

        public static IList<Attestation> GetByEndpointType(EndpointType endpointType)
        {
            using (DataAccessTransaction dataAccessTransaction = new DataAccessTransaction(false))
            {
                return ORMapList(new AttestationRepository(dataAccessTransaction).GetByEndpointSystemType(endpointType).ToList());
            }
        }

        public override long Save(string currentUser)
        {
            try
            {
                bool isNew = this.IsNew;

                using (var trans = new DataAccessTransaction(currentUser))
                {

                    base.Save(trans);

                    if (!isNew && this.AttestationEndpoint != null)
                    {
                        List<AttestationEndpoint> dbattestationDetails = DomainModel.AttestationEndpoint.GetByAttestationId(this.Id).ToList();
                        List<AttestationEndpoint> removedattestationDetails = dbattestationDetails
                            .Except(this.AttestationEndpoint, new IdBaseEqualityComparer<AttestationEndpoint>()).ToList();
                        removedattestationDetails.ForEach(att => att.Delete());
                        this.AttestationEndpoint.ForEach(att => att.Save(trans));

                        var dbattestationHierarchy = DomainModel.AttestationHierarchy.GetByAttestationId(this.Id).ToList();
                        List<AttestationHierarchy> removedattestationHierarchy = dbattestationHierarchy
                            .Except(this.AttestationHierarchy, new IdBaseEqualityComparer<AttestationHierarchy>()).ToList();
                        removedattestationHierarchy.ForEach(att => att.Delete());
                        
                        this.AttestationHierarchy.ForEach(att => att.AttestationId = this.Id);
                        this.AttestationHierarchy.ForEach(att => att.Save(trans));

                        List<AttestationScope> dbattestationDetailsforScope = DomainModel.AttestationScope.GetByAttestationId(this.Id).ToList();
                        List<AttestationScope> removedattestationDetailsforScope = dbattestationDetailsforScope
                           .Except(this.AttestationScopes, new IdBaseEqualityComparer<AttestationScope>()).ToList();
                        removedattestationDetailsforScope.ForEach(att => att.Delete());
                        this.AttestationScopes.ForEach(att => att.Save(trans));

                    }
                    trans.Commit();
                }

            }
            catch(Exception ex)
            {
                DI.Chakra.Logger.DILog.GeneralLogger.WriteLog(null, Logger.LogLevelType.Exception, ex.Message, ex);
            }
            return 0;
        }

        public static bool DoesExists(string attestationName, long attestationid)
        {
            using(var tran = new DataAccessTransaction(false))
            {
                if (attestationid != 0)

                    return new RepositoryBase<EntityModel.Attestation>(tran).Any(attes => attes.Id == attestationid && attes.AttestationName == attestationName);
                else
                    return new RepositoryBase<EntityModel.Attestation>(tran).Any(atts => atts.AttestationName == attestationName);
            }
        }

        public static bool DeleteById(long id)
        {
                AttestationProcess dbattestationProcess = DomainModel.AttestationProcess.GetByAttestationId(id);
                if (dbattestationProcess == null)
                {
                    IList<AttestationEndpoint> dbattestationEndpoint = DomainModel.AttestationEndpoint.GetByAttestationId(id);
                    dbattestationEndpoint.ToList().ForEach(l => l.Delete());
                    IList<AttestationScope> dbattestationScope = DomainModel.AttestationScope.GetByAttestationId(id);
                    dbattestationScope.ToList().ForEach(s => s.Delete());
                    IList<AttestationHierarchy> dbattestationHierarchy = DomainModel.AttestationHierarchy.GetByAttestationId(id);
                    dbattestationHierarchy.ToList().ForEach(s => s.Delete());
                    Delete(id);
                    return true;
                }
                else
                {
                    return false;
                }
        }

        public static bool IsLinkedWithAtestation(long workflowid)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new RepositoryBase<EntityModel.Attestation>(tran).Any(attes => attes.WorkflowConfigurationId == workflowid);
            }
        }
        public static Attestation GetByAttestationName(string attestationName)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Attestation>(tran).Get(t => t.AttestationName.ToUpper() == attestationName.ToUpper()).FirstOrDefault());
            }
        }
        public static string GetFlinsAttestationMessage(long id)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                if(new AttestationRepository(tran).GetFlinsAttestationMessage(id))
                {
                    return MasterConfigHelper.AttestationFlinsWarningMessage;
                }
                return null;
            }
                
        }
        public static Attestation GetByIdWithDependent(long id)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return ORMap(new RepositoryBase<EntityModel.Attestation>(tran)
                .GetById(id,
                attn => attn.AttestationProcesses,
                attn => attn.AttestationProcesses.Select(x => x.process),
                attn => attn.AttestationEndpoint,
                attn => attn.AttestationEndpoint.Select(x => x.EndPoint)));
            }
        }
        public static Dictionary<int, string> GetSAPApplicationTypeFromAttestation(long id)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationRepository(tran).GetSAPApplicationTypeFromAttestation(id);
            }
        }

        public static IDMSHttpResponse<byte[]> DownloadTCodeReport(ApplicationType applicationType)
        {
            IDMSHttpResponse<byte[]> result = new IDMSHttpResponse<byte[]>();
            var ep = EndPoint.GetByApplicationType(applicationType);
            var filePath = string.Format("{0}\\{1}_RoleToTCode.xlsx", MasterConfigHelper.TCodeReportFilePath, ep.DisplayName.GetValidFileName().ToString());

            if (!System.IO.File.Exists(filePath))
            {
                result.Message = SAPTCodeInfo.GenerateRoleTCodeForReport(ep.Id);
            }

            using (var fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var objExcelPackage = new ExcelPackage(fStream))
                {
                    result.Data = objExcelPackage.GetAsByteArray();
                }
            }
            return result;
        }

        public static bool DoesExistsOtherSystemEndpoint(long taskId)
        {
            using (var tran = new DataAccessTransaction(false))
            {
                return new AttestationRepository(tran).DoesExistsOtherSystemEndpoint(taskId);
            }
        }
    }
}

