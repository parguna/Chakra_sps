
using DI.Chakra.DomainModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DI.Chakra.Connector.SAPV3
{
    public class Position
    {
        string ServiceUser;
        public Position(string _serviceUser)
        {
            ServiceUser = _serviceUser;
        }
        public ProcessorConfig ProcessorConfig { get; set; }

        public void StartProcess()
        {
            try
            {
                PopulatePositionRoleMapping();
                PopulatePositionOrgMapping();
                PopulateOrgMapping();
                PopulateOrgRoleMapping();
                PopulatePositionPersonMapping();

                List<SAPHRP1001> deleteLst = SAPHRP1001.GetByModifiedDate(ProcessorConfig.Process.ProcessStartAt, ProcessorConfig.EndpointId).ToList();
                SAPHRP1001.DeleteList(deleteLst);
            }
            catch
            {
                throw;
            }
        }

        public void StartPositionProcess()
        {
            try
            {
                PopulatePositionPersonMapping();
            }
            catch
            {
                throw;
            }
        }

        void PopulatePositionRoleMapping()
        {
            try
            {
                //As per SAP it should be ENDA must be greater than equal to current date confirmed by Dominick. 04 Aug 2022
                string filter = "OTYPE = 'S' AND SUBTY = 'B007' AND SCLAS = 'AG' AND ENDDA >= '" + DateTime.Today.ToString("yyyyMMdd") + "' ";
                Dump(filter, "S", "AG");
            }
            catch
            {
                throw;
            }
        }

        void PopulatePositionOrgMapping()
        {
            try
            {
                string filter = "SUBTY = 'A003' AND SCLAS = 'O' AND OTYPE = 'S' AND ENDDA >= '" + DateTime.Today.ToString("yyyyMMdd") + "' ";
                Dump(filter, "S", "O");
            }
            catch
            {
                throw;
            }
        }

        void PopulateOrgMapping()
        {
            try
            {
                string filter = "SUBTY = 'B002' AND SCLAS = 'O' AND OTYPE = 'O' AND ENDDA >= '" + DateTime.Today.ToString("yyyyMMdd") + "' ";
                Dump(filter, "O", "O");
            }
            catch
            {
                throw;
            }
        }

        void PopulateOrgRoleMapping()
        {
            try
            {
                string filter = "SUBTY = 'B007' AND SCLAS = 'AG' AND OTYPE = 'O' AND ENDDA >= '" + DateTime.Today.ToString("yyyyMMdd") + "' ";
                Dump(filter, "O", "AG");
            }
            catch
            {
                throw;
            }
        }

        void PopulatePositionPersonMapping()
        {
            try
            {
                string filter = "SCLAS = 'US' AND OTYPE = 'S' AND ENDDA > '" + DateTime.Today.ToString("yyyyMMdd") + "' ";
                Dump(filter, "S", "US");

            }
            catch
            {
                throw;
            }
        }

        void Dump(string filter, string actualObjectType, string relatedObjectType)
        {

            try
            {

                ProcessorConfig.sapUtil.ResetPaging();

                DataTable m_dtHRP1001 = null;

                do
                {
                    m_dtHRP1001 = null;
                    m_dtHRP1001 = ProcessorConfig.sapUtil.ReadRFCTblAsFunction("HRP1001", null, ConnectorFields.POSITION_FIELDS, filter);
                    foreach (DataRow row in m_dtHRP1001.Rows)
                    {
                        SAPHRP1001 hr1001 = new SAPHRP1001()
                        {
                            EndpointSystemId = ProcessorConfig.EndpointId,
                            ActualObjectCode = row["OBJID"].ToString().Trim(),
                            RelatedObjectCode = row["SOBID"].ToString().Trim(),
                            BeginDate = row["BEGDA"].ToString().Trim(),
                            EndDate = row["ENDDA"].ToString().Trim(),
                            RelatedObjectType = relatedObjectType,
                            ActualObjectType = actualObjectType
                        };

                        var mapping = SAPHRP1001.GetByActualObjectCode(hr1001);

                        if (mapping == null)
                        {
                            hr1001.Save(ServiceUser);
                        }
                        else
                        {
                            mapping.BeginDate = hr1001.BeginDate;
                            mapping.EndDate = hr1001.EndDate;
                            mapping.Save(ServiceUser);
                        }
                    }

                    ProcessorConfig.sapUtil.NextPage();

                } while (m_dtHRP1001.Rows.Count > 0);

            }
            catch
            {
                throw;
            }
        }


    }
}
