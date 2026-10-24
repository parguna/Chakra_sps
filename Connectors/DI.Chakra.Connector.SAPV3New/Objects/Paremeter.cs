using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.Connector.SAPV3
{
    public class Parameter
    {
        string ServiceUser;
        public Parameter(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public ProcessorConfig ProcessorConfig { get; set; }


        public void StartProcess()
        {
            ProcessParameters();
        }
        private IList<SAPParameterInfo> ProcessParameters()
        {
            IList<SAPParameterInfo> lstParameterInfo = new List<SAPParameterInfo>();

            ProcessorConfig.sapUtil.ResetPaging();

            try
            {
                DataTable dtParameters = null;
                do
                {

                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TPARA", null, null, null);
                    foreach (DataRow row in dtParameters.Rows)
                    {
                        var SAPParameter = new SAPParameterInfo();
                        SAPParameter.PARAM_CODE = row["PARAMID"].ToString().Trim();
                        SAPParameter.PARAM_DESC = row["PARTEXT"].ToString().Trim();
                        SAPParameter.Version = Convert.ToInt16(VersionNo.Available);
                        SAPParameter.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SAPParameter.TargetSysId = null;
                        lstParameterInfo.Add(SAPParameter);
                    }

                    if (lstParameterInfo.Count > 0)
                    {
                        DI.Chakra.DomainModel.SAPParameterInfo.SaveParameterInfos(lstParameterInfo, ProcessorConfig.EndpointId, ServiceUser);
                        lstParameterInfo.Clear();
                    }

                    ProcessorConfig.sapUtil.NextPage();
                } while (dtParameters.Rows.Count > 0);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstParameterInfo;
        }

    }
}
