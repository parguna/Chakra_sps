using DI.Chakra.Dto;
using DI.Chakra.Dto.Core;
using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace DI.Chakra.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAttestationDataService" in both code and config file together.
    [ServiceContract]
    public interface IAttestationDataService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetAttestationUserComparisionData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        UserEntitlementComparisionResult GetAttestationUserComparisionData(long taskId, int draw, int start, int length, DataTableSearch search = null);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetTaskFilterData", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        List<FilterData> GetTaskFilterData(TaskFilter taskFilter);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "UpdateAttestationTaskActionByUser", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json)]
        bool UpdateAttestationTaskActionByUser(DTORequestParameter dTORequest);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetEmployeeInformation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json)]
        Dictionary<string, List<FilterData>> GetEmployeeInformation(long taskId, DataTableSearch search);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetTaskHasUnsavedChanges", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json)]
        bool GetTaskHasUnsavedChanges(long taskId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SavedChanges", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        bool SavedChanges(long taskId, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "RevertTaskInInterimStore", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        bool RevertTaskInInterimStore(long taskId, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetAttestationTaskInformation", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json)]
        AttestationTaskInformation GetAttestationTaskInformation(long taskId, string userName);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "SubmitAttestationTask", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        bool SubmitAttestationTask(long taskId, string userName, bool isSaved);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetAccountTypes", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json,
      RequestFormat = WebMessageFormat.Json)]
        List<AccountType> GetAccountTypes(long taskId);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "GetAccountStatusInformation", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json)]
        List<FilterData> GetAccountStatusInformation(long taskId, long userId, DataTableSearch search);
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "DownloadUserComparisionData", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json,
          RequestFormat = WebMessageFormat.Json)]
        byte[] DownloadUserComparisionData(long taskId);
    }

}
