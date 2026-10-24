using DeepIdentity.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCleansingTool.FileProcessors
{
    public class FileProcessorFactory
    {
        public static FileProcessorBase GetFileProcessor()
        {
            FileProcessorBase fileProcessor = new ApproverExceptionUploadFileProcessor();
            return fileProcessor;
        }

        public static FileProcessorBase GetFileProcessor(EndpointType endpointType)
        {
            FileProcessorBase fileProcessor = null;
            switch (endpointType)
            {

                case EndpointType.None:
                    fileProcessor = new UserUploadFileProcessor();
                    break;

                case EndpointType.ActiveDirectory:
                    fileProcessor = new ADSPFileProcessor();
                    break;

                case EndpointType.LotusNotes:
                    fileProcessor = new LNSFileProcessor();
                    break;

                case EndpointType.SAP:
                    fileProcessor = new GRCFileProcessor();
                    break;

                case EndpointType.InhouseSystems:
                    fileProcessor = new InhouseSystemFileProcessor();
                    break;

                default:
                    break;
            }

            return fileProcessor;
        }
    }
}
