
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using DI.Chakra.DomainModel;

namespace DI.Chakra.Services.AttestationProcessor
{
   public class LinuxAttestation : EndpointAttestation
    {
       public IList<AttestationReviewEndpointSystemL2Detail> GetGroupReviews(int userId, long reviewEndpointId)
       {
           IList<AttestationReviewEndpointSystemL2Detail> list = new List<AttestationReviewEndpointSystemL2Detail>();

           try
           {
               AttestationReviewEndpointSystemL2Detail reviewRole = null;

                IList<LinuxGroupInfo> listGroups = LinuxGroupInfo.GetByIdIncludingLinuxGroupInfo(userId);

               if (listGroups == null)
                   return null;

               if (listGroups.Count <= 0)
                   return null;

               foreach (LinuxGroupInfo oraUser in listGroups)
               {
                   reviewRole = new AttestationReviewEndpointSystemL2Detail();
                   reviewRole.ReviewEndpointId = reviewEndpointId;
                   reviewRole.AccessName = oraUser.GroupName;
                   reviewRole.Comments = string.Empty;
                   reviewRole.Approve = true;
                   reviewRole.PreviousAccess = Constants.APPROVE;
                   list.Add(reviewRole);

               }
               return list;
           }
           catch (Exception)
           {

               throw;
           }
       }
    }
}
