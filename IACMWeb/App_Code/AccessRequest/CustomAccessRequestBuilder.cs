using DI.Chakra.Dto.Presentation;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.Web
{
    public class CustomAccessRequestBuilder : AccessRequestBuilderBase
    {
        public override string CreateAccessRequests()
        {
            return string.Empty;
        }
        public override string CreateRemoveAccessRequests(List<PresentationAttestationEntitlementReviewDetail> userDetail)
        {
            return string.Empty;
        }
    }
}

