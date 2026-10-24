using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DI.Chakra.Web
{
    public class AccessRequestBuilderFactory
    {
        public static AccessRequestBuilderBase Get(AccessRequestBuilderType builderType)
        {
            switch (builderType)
            {
                case AccessRequestBuilderType.Default:
                    return new DefaultAccessRequestBuilder();
                case AccessRequestBuilderType.Custom:
                    return new CustomAccessRequestBuilder();
                default:
                    return new DefaultAccessRequestBuilder();
            }

        }
    }
}
