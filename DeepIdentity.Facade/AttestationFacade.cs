using DeepIdentity.DataAccess;
using DeepIdentity.Infrastructure;
using DeepIdentity.Model;
using DeepIdentity.Model.ReportData;
using System;
using System.Collections.Generic;
using DeepIdentity.Model.GridData;
using DeepIdentity.Infrastructure.Enums;

namespace DeepIdentity.Facade
{

    public class AttestationFacade
    {
        public AttestationDA _attestationDA { get; set; }
        AttestationProcessDA _attestationProcessDA = null;

        public AttestationFacade()
        {
            _attestationDA = new AttestationDA();
            _attestationProcessDA = new AttestationProcessDA();
        }

        public bool IsAttestationNameExists(string attestationName, int attestationid)
        {
            return _attestationDA.IsAttestationNameExists(attestationName, attestationid);
        }

        public AttestationProcess GetAttestationProcessById(int _processId)
        {
            try
            {
                return _attestationProcessDA.GetAttestationProcessById(_processId);
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }


        public Attestation GetAttestation(int _id, bool deepLoad)
        {
            try
            {
                Attestation attestation = _attestationDA.GetAttestation(_id, deepLoad);

                return attestation;
            }
            catch (DomainException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DomainException(DomainException.TEXT_DOMAIN_EXCEPTION, ex);
            }
        }

        public IList<Attestation> GetAllAttestations()
        {
            try
            {
                IList<Attestation> attestations = _attestationDA.GetAllAttestations();

                return attestations;
            }
            catch
            {
                throw;
            }

        }

        public IList<Attestation> GetAllAttestationsOfEndPointType(EndpointType paramEPType)
        {
            try
            {
                IList<Attestation> attestations = _attestationDA.GetAllAttestationsOfEndPointType(paramEPType);

                return attestations;
            }
            catch
            {
                throw;
            }

        }

        public IList<ListAttestationGridData> GetAllAttestationsForActionUsers(int paramActionUserId)
        {
            try
            {
                return _attestationDA.GetAllAttestationsForActionUsers(paramActionUserId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<AttestationReviewEndpointSystemGridData> GetUsersofEndpointForReviewer(long _taskId, int _userId)
        {
            try
            {
                return _attestationDA.GetUsersofEndpointForReviewer(_taskId, _userId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateAttestationProcess(AttestationProcess _attProcess)
        {
            try
            {
                _attestationProcessDA.UpdateAttestationProcess(_attProcess);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void UpdateProcess(Process _Process)
        {
            try
            {
                _attestationProcessDA.UpdateProcess(_Process);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void AddAttestation(Attestation _attestation)
        {
            try
            {
                _attestationDA.AddAttestation(_attestation);
            }
            catch
            {

                throw;
            }
        }

        public List<ADEXRevokeRequestReportData> GetAllADEXRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllADEXRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<ADEXAccountRequests> GetAllADEXAccountRequests()
        {
            try
            {
                return _attestationDA.GetAllADEXAccountRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<LNSAccountRequests> GetAllLNSAccountRequests()
        {
            try
            {
                return _attestationDA.GetAllLNSAccountRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<LNSRevokeRequestReportData> GetAllLNSRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllLNSRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<LNSGroupRevokeRequestReportData> GetAllLNSGroupRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllLNSGroupRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<LNSGroupRequestReportData> GetAllLNSGroupRequests()
        {
            try
            {
                return _attestationDA.GetAllLNSGroupRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<ADEXGroupRevokeRequestReportData> GetAllADEXGroupRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllADEXGroupRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<ADEXGroupRequestReportData> GetAllADEXGroupRequests()
        {
            try
            {
                return _attestationDA.GetAllADEXGroupRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseAccountRequests> GetAllInhouseAccountRequests()
        {
            try
            {
                return _attestationDA.GetAllInhouseAccountRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseGroupRevokeRequestReportData> GetAllInhouseGroupRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllInhouseGroupRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseGroupRequestReportData> GetAllInhouseGroupRequests()
        {
            try
            {
                return _attestationDA.GetAllInhouseGroupRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<InhouseRevokeRequestReportData> GetAllInhouseRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllInhouseRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public void AddAttestationProcessHistory(AttestationProcess _attestationprocess)
        {
            try
            {
                AttestationProcessHistory history = new AttestationProcessHistory()
                {
                    AttestationId = _attestationprocess.AttestationId,
                    AttestationProcessId = _attestationprocess.AttestationProcessId,
                    CreatedBy = _attestationprocess.CreatedBy,
                    CreatedDate = _attestationprocess.CreatedDate,
                    HistoryId = _attestationprocess.HistoryId,
                    LastModifiedBy = _attestationprocess.LastModifiedBy,
                    LastModifiedDate = _attestationprocess.LastModifiedDate,
                    //Message = _attestationprocess.Message,
                    ProcessEndAt = _attestationprocess.ProcessEndAt,
                    ProcessStartAt = _attestationprocess.ProcessStartAt,
                    //Status = _attestationprocess.Status,
                    WorkflowStatus = _attestationprocess.WorkflowStatus,
                };

                _attestationProcessDA.AddAttestationProcessHistory(history);
            }
            catch
            {
                throw;
            }
        }

        public List<User> GetUsersForReviewer(long _taskId)
        {
            try
            {
                return _attestationDA.GetUsersForReviewer(_taskId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IList<AttestationEndpoint> GetAttestationEndpoint(int AttestationId)
        {
            try
            {
                return _attestationDA.GetAttestationEndpoint(AttestationId);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IList<ListAttestationGridData> GetAllAttestationsWithDetails(int ApplicationType)
        {
            try
            {
                IList<ListAttestationGridData> attestations = _attestationDA.GetAllAttestationsWithDetails(ApplicationType);

                return attestations;
            }
            catch
            {
                throw;
            }

        }

        public List<SAPEPRoleRevokeRequestReportData> GetAllSAPEPRoleRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllSAPEPRoleRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<SAPEPRevokeRequestReportData> GetAllSAPEPRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllSAPEPRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<SAPEPRoleRequestReportData> GetAllSAPEPRoleRequests()
        {
            try
            {
                return _attestationDA.GetAllSAPEPRoleRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<SAPEPAccountRequestsReportData> GetAllSAPEPAccountRequests()
        {
            try
            {
                return _attestationDA.GetAllSAPEPAccountRequests();
            }
            catch
            {
                throw;
            }
        }

        public IList<Attestation_ListOfAttestation> getAllAttestationProcessForReport(EndpointType paramEPType)
        {
            try
            {
                return _attestationDA.getAllAttestationProcessForReport(paramEPType);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<Attestation_ListOfUsersByAttestationStatusReportData> getAllUsersAttestationProcessStatusForReport(int attestationId, string attestationStatus)
        {
            try
            {
                return _attestationDA.getAllUsersAttestationProcessStatusForReport(attestationId, attestationStatus);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationActionReportData> getAttestationActionReportData(int paramAttestationId)
        {
            try
            {
                return _attestationDA.getAttestationActionReportData(paramAttestationId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationsByGroupAndRoleLevelReportData> getAttestationsByGroupAndRoleLevel(int _attestationId, int _actionId)
        {
            try
            {
                return _attestationDA.getAttestationsByGroupAndRoleLevel(_attestationId, _actionId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationReviewPageSAP_Level1GridData> getSAPLevel1InfosForAttestationReviewGrid(long paramL2DetailId, int paramEndPointSystemId)
        {
            try
            {
                return _attestationDA.getSAPLevel1InfosForAttestationReviewGrid(paramL2DetailId, paramEndPointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IList<AttestationReviewPageSAP_Level1GridData> getSAPLevel2InfosForAttestationReviewGrid(string paramSingleRoleCode, int paramEndPointSystemId)
        {
            try
            {
                return _attestationDA.getSAPLevel2InfosForAttestationReviewGrid(paramSingleRoleCode, paramEndPointSystemId);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public IList<AttestationReviewPageSAP_Level1GridData> getLevel2DetailsForAuthorization(string paramSingleRoleCode, int paramEndPointSystemId)
        {
            try
            {
                return _attestationDA.getAuthorizationDataForAttestationReviewGrid(paramSingleRoleCode, paramEndPointSystemId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string getRoleDescription(long paramL2DetailId)
        {
            try
            {
                return _attestationDA.getRoleDescription(paramL2DetailId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<SAPRoleRequestReportData> GetAllSAPRoleRequests()
        {
            try
            {
                return _attestationDA.GetAllSAPRoleRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<SAPRoleRequestReportData> GetAllSapEXRoleRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllSapEXRoleRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public List<SAPRevokeRequestReportData> GetAllSAPRevokeRequests()
        {
            try
            {
                return _attestationDA.GetAllSAPRevokeRequests();
            }
            catch
            {
                throw;
            }
        }

        public object GetAllSAPAccountRequests()
        {
            try
            {
                return _attestationDA.GetAllSAPAccountRequests();
            }
            catch
            {
                throw;
            }
        }

        public void Save(Attestation _attestation)
        {
            _attestationDA.Save(_attestation);
        }

        public void DeleteAttestation(int AttestationId)
        {
            try
            {
                _attestationDA.DeleteAttestation(AttestationId);
            }
            catch
            {
                throw;
            }
        }
        public bool IsAttestationExistInTask(int AttestationId)
        {
            try
            {
                return _attestationDA.IsAttestationExistInTask(AttestationId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        #region Added by Yasila.H for V5 Changes on 02.11.2015
        public List<Attestation> GetAttestationName()
        {
            try
            {
                return _attestationDA.GetAttestationName();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public AttestationProcess GetPrcoessByAttestationId(int attestationId)
        {
            try
            {
                return _attestationProcessDA.GetPrcoessByAttestationId(attestationId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsProcessNameExisits(string processName)
        {
            return _attestationDA.IsProcessNameExists(processName);
        }
        #endregion
    }

}