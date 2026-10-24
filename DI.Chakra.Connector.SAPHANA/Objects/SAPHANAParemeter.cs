using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace DI.Chakra.Connector.SAPHANA
{
    public class SAPHANAParemeter
    {
        string ServiceUser;
        public SAPHANAParemeter(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public SAPHANAProcessorConfig ProcessorConfig { get; set; }


        public void StartProcess()
        {
            ProcessParameters();
        }
        private IList<SAPHANAParameterInfo> ProcessParameters()
        {
            var lstParameterInfo = new List<SAPHANAParameterInfo>();

            ProcessorConfig.sapUtil.ResetPaging();

            try
            {
                DataTable dtParameters = null;
                do
                {

                    dtParameters = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("TPARA", null, null, null, ProcessorConfig.sapUtil.IsPaginationEnabled);
                    foreach (DataRow row in dtParameters.Rows)
                    {
                        var SAPParameter = new SAPHANAParameterInfo();
                        SAPParameter.PARAM_CODE = row["PARAMID"].ToString().Trim();
                        SAPParameter.PARAM_DESC = row["PARTEXT"].ToString().Trim();
                        SAPParameter.Version = Convert.ToInt16(VersionNo.Available);
                        SAPParameter.EndPointSystemId = (int)ProcessorConfig.EndpointId;
                        SAPParameter.TargetSysId = null;
                        lstParameterInfo.Add(SAPParameter);
                    }

                    if (lstParameterInfo.Count > 0)
                    {
                        DI.Chakra.DomainModel.SAPHANAParameterInfo.SaveParameterInfos(lstParameterInfo, ProcessorConfig.EndpointId, ServiceUser);
                        lstParameterInfo.Clear();
                    }
                    if(ProcessorConfig.sapUtil.IsPaginationEnabled)
                        ProcessorConfig.sapUtil.NextPage();
                } while (dtParameters.Rows.Count > 0 && ProcessorConfig.sapUtil.IsPaginationEnabled);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstParameterInfo;
        }

    }
}
