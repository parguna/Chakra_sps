using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DI.Chakra.Infrastructure;

namespace Application
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApplicationUtil
    {
        public static IQueryingService GetQueryingService()
        {
            return new QueryingService();
        }

    }
}