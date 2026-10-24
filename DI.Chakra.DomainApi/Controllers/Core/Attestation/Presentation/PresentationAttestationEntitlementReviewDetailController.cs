using AutoMapper;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace DI.Chakra.DomainApi.Controllers
{
    [RoutePrefix("PresentationAttestationEntitlementReviewDetail")]
    public class PresentationAttestationEntitlementReviewDetailController : ApiController
    {
        [HttpGet]
        [Route("GetUserForAttestationTaskReview")]
        public IList<PresentationAttestationEntitlementReviewDetail> GetUserForAttestationTaskReview(long taskId, string currentUser, long level, bool isInitialLoad = false)
        {
            var attestationTaskReview = DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.GetUsersForAttestationTaskReview(taskId, currentUser, level, isInitialLoad).ToList();
            return DtoMapList(attestationTaskReview);
        }

        [HttpGet]
        [Route("GetTaskHasUnsavedChanges")]
        public bool GetTaskHasUnsavedChanges(long taskId, string currentUser)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.GetTaskHasUnsavedChanges(taskId, currentUser);
        }

        [HttpGet]
        [Route("GetAttestationTypeForTask")]
        public AttestationType GetAttestationTypeForTask(long taskId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.GetAttestationTypeForTask(taskId);
        }

        [HttpGet]
        [Route("GetEndpointsForAttestationTaskReview")]
        public IList<PresentationAttestationEntitlementReviewDetail> GetEndpointsForAttestationTaskReview(long taskId, string currentUser, long level, bool isInitialLoad = false)
        {
            var test = DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.GetEndpointsForAttestationTaskReview(taskId, currentUser, level, isInitialLoad).ToList();
            return ConverToDtoPresentationAttestationEntitlementReviewDetail(test);
        }

        [HttpGet]
        [Route("GetDetailsForAttestationTaskReviewByUser")]
        public IList<PresentationAttestationEntitlementReviewDetail> GetDetailsForAttestationTaskReviewByUser(long taskId, string currentUser, long level, long bindingLevel, string entity, long endpointSystemId, string accountName, bool isInitialLoad = false)
        {
            var attestationTaskReview = DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.GetDetailsForAttestationTaskReviewByUser(taskId, currentUser, level, bindingLevel, entity, endpointSystemId, accountName, isInitialLoad).ToList();
            return ConverToDtoPresentationAttestationEntitlementReviewDetail(attestationTaskReview);
        }

        [HttpGet]
        [Route("GetDetailsForAttestationTaskReviewByRole")]
        public IList<PresentationAttestationEntitlementReviewDetail> GetDetailsForAttestationTaskReviewByRole(long taskId, long keyId, string currentUser, long level, long bindingLevel, string entity, long endpointSystemId, long displayHierarchyStartLevel, bool isInitialLoad = false)
        {
            var attestationTaskReview = DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.GetDetailsForAttestationTaskReviewByRole(taskId, keyId, currentUser, level, bindingLevel, entity, endpointSystemId, displayHierarchyStartLevel, isInitialLoad).ToList();
            return ConverToDtoPresentationAttestationEntitlementReviewDetail(attestationTaskReview);
        }

        [HttpGet]
        [Route("UpdateAttestationReviewAction")]
        public bool UpdateAttestationReviewAction(long snapshotActionId, bool value, string accessType, long taskId, string currentUser, long taskActionUserId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.UpdateAttestationReviewAction(snapshotActionId, value, accessType, taskId, currentUser, taskActionUserId);
        }

        [HttpGet]
        [Route("UpdateAttestationReviewAll")]
        public bool UpdateAttestationReviewAll(bool value, string accessType, long taskId, string currentUser, long taskActionUserId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.UpdateAttestationReviewAll(value, accessType, taskId, currentUser, taskActionUserId);
        }

        [HttpGet]
        [Route("UpdateAttestationReviewReservedColumn")]
        public bool UpdateAttestationReviewReservedColumn(long snapshotActionId, string value, string accessType, long taskId, string currentUser, long taskActionUserId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.UpdateAttestationReviewReservedColumn(snapshotActionId, value, accessType, taskId, currentUser, taskActionUserId);
        }

        [HttpGet]
        [Route("UpdateAttestationReviewComments")]
        public bool UpdateAttestationReviewComments(long snapshotActionId, string comments, string category, long taskId, string currentUser, long taskActionUserId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.UpdateAttestationReviewComments(snapshotActionId, comments, category, taskId, currentUser, taskActionUserId);
        }

        [HttpGet]
        [Route("SaveAttestationReview")]
        public bool SaveAttestationReview(long taskId, string currentUser)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.SaveAttestationReview(taskId, currentUser);
        }

        [HttpGet]
        [Route("RevertAttestationReview")]
        public bool RevertAttestationReview(long taskId, string currentUser)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.RevertAttestationReview(taskId, currentUser);
        }

        [HttpGet]
        [Route("CanSubmitTask")]
        public bool CanSubmitTask(long taskId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.CanSubmitTask(taskId);
        }


        [HttpGet]
        [Route("UpdateAttestationComment")]
        public bool UpdateAttestationComment(long campaignId, string currentUser, string comment)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.UpdateAttestationComment(campaignId, currentUser, comment);
        }

        public PresentationAttestationEntitlementReviewDetail DtoMap(DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail tModel)
        {
            if (tModel == null)
                return null;
            return Mapper.Map<DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail, PresentationAttestationEntitlementReviewDetail>(tModel);
        }

        public List<PresentationAttestationEntitlementReviewDetail> DtoMapList(IList<DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail> tModelList)
        {
            return tModelList.Select(DtoMap).ToList();
        }
        [HttpGet]
        [Route("UpdateAllAttestationReviewEffectiveDate")]
        public string UpdateAllAttestationReviewEffectiveDate(string entity, DateTime? effectivedate, string category, long taskId, string currentUser, long taskActionUserId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.UpdateAllAttestationReviewEffectiveDate(entity, effectivedate, category, taskId, currentUser, taskActionUserId);
        }
        [HttpGet]
        [Route("UpdateAttestationReviewEffectiveDate")]
        public string UpdateAttestationReviewEffectiveDate(long snapshotActionId, [FromUri]DateTime? effectivedate, string category, long taskId, string currentUser, long taskActionUserId)
        {
            return DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail.UpdateAttestationReviewEffectiveDate(snapshotActionId, effectivedate, category, taskId, currentUser, taskActionUserId);
        }

        private static IList<PresentationAttestationEntitlementReviewDetail> ConverToDtoPresentationAttestationEntitlementReviewDetail(IList<DomainModel.Presentation.PresentationAttestationEntitlementReviewDetail> dPresentationAttestationEntitlementReviewDetails)
        {
            List<PresentationAttestationEntitlementReviewDetail> presentationAttestationEntitlementReviewDetails = new List<PresentationAttestationEntitlementReviewDetail>();
            foreach (var dReview in dPresentationAttestationEntitlementReviewDetails)
            {
                presentationAttestationEntitlementReviewDetails.Add(new PresentationAttestationEntitlementReviewDetail()
                {
                    AccountName = dReview.AccountName,
                    AccountStatus = dReview.AccountStatus,
                    AccountType = dReview.AccountType,
                    ActionReserved1 = dReview.ActionReserved1,
                    ActionReserved2 = dReview.ActionReserved2,
                    ActionReserved3 = dReview.ActionReserved3,
                    AdhocActionType = dReview.AdhocActionType,
                    Approved = dReview.Approved,
                    BindingLevel = dReview.BindingLevel,
                    Comments = dReview.Comments,
                    DisplayName = dReview.DisplayName,
                    DIUserName = dReview.DIUserName,
                    EffectiveDate = dReview.EffectiveDate,
                    EndpointSystemId = dReview.EndpointSystemId,
                    EndpointSystemName = dReview.EndpointSystemName,
                    EndpointSystemType = dReview.EndpointSystemType,
                    EndpointType = dReview.EndpointType,
                    EntitlementId = dReview.EntitlementId,
                    Entity = dReview.Entity,
                    IsActionable = dReview.IsActionable,
                    IsActionExtendable = dReview.IsActionExtendable,
                    IsChanged = dReview.IsChanged,
                    IsChangedInPreviousLevel = dReview.IsChangedInPreviousLevel,
                    IsDelta = dReview.IsDelta,
                    IsNextLevelAvailable = dReview.IsNextLevelAvailable,
                    IsSensitiveAccess = dReview.IsSensitiveAccess,
                    IsSODConflict = dReview.IsSODConflict,
                    KeyId = dReview.KeyId,
                    LastLogon = dReview.LastLogon,
                    Level = dReview.Level,
                    Name = dReview.Name,
                    ParentBindingLevel = dReview.ParentBindingLevel,
                    ParentEntity = dReview.ParentEntity,
                    Rejected = dReview.Rejected,
                    RequesteeUserId = dReview.RequesteeUserId,
                    RequesterUserId = dReview.RequesterUserId,
                    Reserved1 = dReview.Reserved1,
                    Reserved10 = dReview.Reserved10,
                    Reserved2 = dReview.Reserved2,
                    Reserved3 = dReview.Reserved3,
                    Reserved4 = dReview.Reserved4,
                    Reserved5 = dReview.Reserved5,
                    Reserved6 = dReview.Reserved6,
                    Reserved7 = dReview.Reserved7,
                    Reserved8 = dReview.Reserved8,
                    Reserved9 = dReview.Reserved9,
                    ReviewCategory = dReview.ReviewCategory,
                    SnapshotActionId = dReview.SnapshotActionId,
                    Type = dReview.Type
                });
            }
            return presentationAttestationEntitlementReviewDetails;
        }
    }
}