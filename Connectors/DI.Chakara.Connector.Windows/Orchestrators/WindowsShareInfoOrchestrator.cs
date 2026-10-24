using DI.Chakra.EntityModel;
using System;

namespace DeepIdentity.Connector
{
    public class WindowsShareInfoOrchestrator
    {
        private const string PROCESSNAME = "DiscoveryProcess";
        public WindowsShareInfoOrchestrator()
        {
        }
        public int EndpointSystemId { get; set; }
        

        public WindowsShareInfo PopulateShare(Int64 AutoIncrementValue, string objectSid, string permission, string Resourcename, string ResourceCompletePath, string AccessType)
        {
            WindowsShareInfo ShareList = new WindowsShareInfo();

            try
            {
                ShareList.EndPointSystemId = EndpointSystemId;
                ShareList.ResourceID = AutoIncrementValue;
                ShareList.ResouceName = Resourcename;
                ShareList.ResourceCompletePath = ResourceCompletePath;
                ShareList.objectSid = objectSid;

                if (permission == "2032127")
                    ShareList.Access_Rights = "FullControl";
                else if (permission == "1245631")
                    ShareList.Access_Rights = "Change";
                else if (permission == "1179817")
                    ShareList.Access_Rights = "ReadOnly";

                if (AccessType == "0")
                    ShareList.AccessType = "Allow";
                else
                    ShareList.AccessType = "Deny";

            }
            catch (Exception)
            {
                throw;
            }
            return ShareList;
        }
    }
}
