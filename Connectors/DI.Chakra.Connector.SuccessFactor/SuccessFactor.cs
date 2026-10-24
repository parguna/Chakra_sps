using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;

using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DI.Chakra.Connector.SF.Objects;
using System.Data;
using Newtonsoft.Json.Linq;
using System.IO;
using DI.Chakra.Services.EmailingProcessor;

namespace DI.Chakra.Connector
{
    public delegate void SendmailDelegate(DomainModel.EndPoint endPoint, IdmsLogger logger, string message, string employeeId);
    public class SuccessFactorConnector : ConnectorBase
    {
        string today = DateTime.Today.ToString(SF_Helper.DateFormat);
        readonly string _userName;
        readonly string _decrpytedPassword;
        readonly string _url;
        readonly string _base64Creds;
        readonly SFOData.EntityContainer _entityContainer;
        readonly DI.Chakra.DomainModel.EndPoint _endPoint;
        readonly string MasterConfigHelperServiceUser;
        readonly EndpointConfiguration _endpointConfiguration;
        readonly List<string> typesConsideredFromDateParamter;
        public readonly string TransferTriggerNotes;
        #region Objects

        SF_Organisation sF_Organisation;
        readonly SF_User sF_User;

        #endregion Objects 
        public SuccessFactorConnector(DI.Chakra.DomainModel.EndPoint endpoint, IdmsLogger logger) : base(logger)
        {
            this.TransferTriggerNotes = MasterConfigHelper.TranferTriggersNotes;

            MasterConfigHelperServiceUser = MasterConfigHelper.ServiceUser;

            typesConsideredFromDateParamter = MasterConfigHelper.TypesConsideredFromDateParamter;

            _endPoint = this.Endpoint = endpoint;

            _endpointConfiguration = EndpointConfiguration.GetByEndpointSystemId(endpoint.Id);

            _userName = getUserName();

            _decrpytedPassword = getPassword();

            _url = getUrl();


            _base64Creds = getBaseCredential();

            _entityContainer = new SFOData.EntityContainer(new Uri(_url));
            _entityContainer.Timeout = MasterConfigHelper.SFAPITimeoutDuration;
            _entityContainer.BuildingRequest += (sender, e) =>
            {

                System.Net.ServicePointManager.Expect100Continue = true;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls
                       | System.Net.SecurityProtocolType.Tls11
                       | System.Net.SecurityProtocolType.Tls12
                       | System.Net.SecurityProtocolType.Ssl3;
            };


            _entityContainer.SendingRequest2 += (sender, e) =>
            {
                //_idmsLogger.WriteLog(LogLevelType.DebugStep, e.RequestMessage.Url.Query);
                e.RequestMessage.SetHeader("Authorization", $"Basic {_base64Creds}");
                e.RequestMessage.SetHeader("Pragma", $"no-cache");
            };

            _entityContainer.ReceivingResponse += (sender, e) =>
              {

              };

            _entityContainer.WritingEntity += (sender, e) =>
              {
              };

            // fromDate parameter is required to fetch future dated events and data. Else SF would respond as of today
            _entityContainer.BuildingRequest += (sender, e) =>
             {
                 if (typesConsideredFromDateParamter.Any(t=> e.RequestUri.LocalPath.Contains(t.Trim())))
                 {
                     e.RequestUri = new Uri($"{e.RequestUri.AbsoluteUri}{getFromDate()}");
                 }
             };

            sF_User = new SF_User(_entityContainer, logger, MasterConfigHelperServiceUser,this.Endpoint, fnEmailSend);
        }

        string fromDate = string.Empty;

        string getFromDate()
        {
            if (string.IsNullOrWhiteSpace(fromDate))
            {
                var dt = DateTime.MinValue;// DeltaDate;
                fromDate = $"&fromDate={dt.Year}-{dt.Month.ToString().PadWithZeros(2)}-{dt.Day.ToString().PadWithZeros(2)}";
            }
            return fromDate;
        }

        #region Container

        internal SFOData.EntityContainer GetContainer()
        {
            return _entityContainer;
        }

        #endregion Container

        #region Connection 
        string getUserName()
        {
            return $"{SF_Helper.Config(EndpointKey.UserName, _endpointConfiguration)}@{SF_Helper.Config(EndpointKey.SF_Company, _endpointConfiguration)}";
        }

        string getPassword()
        {
            return RSAHelper.DecryptData(SF_Helper.Config(EndpointKey.Password, _endpointConfiguration));
        }

        string getUrl()
        {
            return SF_Helper.Config(EndpointKey.WSDLURL, _endpointConfiguration);
        }

        string getBaseCredential()
        {
            string creds = $"{_userName}:{_decrpytedPassword}";
            byte[] bcreds = Encoding.ASCII.GetBytes(creds);
            return Convert.ToBase64String(bcreds);
        }


        #endregion Connection

        #region Common Connector Methods

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        public override bool CreateAccount(IEndpointUserInfo _userInfo, ProvisioningParamCollection fields)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool LockAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool UnLockAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ChangeContainer(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool ExtendValidity(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool DisableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        public override bool EnableAccount(IEndpointUserInfo _userInfo)
        {
            throw new NotImplementedException();
        }

        #endregion Common Connector Methods

        public override bool Connect()
        {
            List<SFOData.FOCompany> companies = null;
            try
            {
                companies = GetContainer().FOCompany.Take(1).Select(c => new SFOData.FOCompany() { name = c.name }).ToList();
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException.Message.Contains("Authentication failed"))
                {
                    throw new Exception(e.InnerException.Message);
                }
                _idmsLogger.WriteLog(LogLevelType.Exception,
                    $"Could not estabish the connection to SuccessFactor. The Url is {_url}"// and BaseCredential :{_base64Creds}"
                    , e);
                throw e;
            }
            return companies != null && companies.Count() > 0;
        }

        public override void DiscoverSpecificUserFromHR(string pernr)
        {
            try
            {
                getOrganisation();
                getCostCenter();
                getDeltaPositions(this.DeltaDate);

                sF_User.Initialize();
                sF_User.discoverUserByPERNR(pernr.PadWithZeros(SF_Helper.EmployeeIdMaxSize), this.DeltaDate);
                getOrganisationHead();
                // pernr is attacker-influenced, so it is bound as a real SqlParameter ("{0}" is
                // an EF token, not a string interpolation substitution) instead of being
                // concatenated into the SQL text.
                User.ExecuteQuery(@" UPDATE A Set A.Supervisor = B.DisplayName FROM DI_M_User A inner join DI_M_User B on A.ROEmployeeId = B.EmployeeId WHERE  A.EmployeeId={0} ", pernr);
            }
            catch (Exception ee)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Exception at DiscoverSpecificUserFromHR", ee);
                throw;
            }
        }

        public override void GetData()
        {
            try
            {
                getOrganisation();
                getCostCenter();


                if (!this.IsFirstTimeDiscovery)
                {
                    getDeltaPositions(this.DeltaDate);
                    deltaDiscovery(this.DeltaDate);
                }
                else
                {
                    getPositions();
                    completeDiscovery();
                }
                getOrganisationHead();

                User.ExecuteQuery(@" UPDATE A Set A.Supervisor = B.DisplayName FROM DI_M_User A inner join DI_M_User B on A.ROEmployeeId = B.EmployeeId WHERE  A.IsActive=1");

                User.ExecuteQuery(@" UPDATE A Set A.Supervisor = '' FROM DI_M_User A WHERE  A.IsActive=1 AND  ISNULL(A.ROEmployeeId,'')= ''   ");
            }
            catch (Exception ee)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception in Discovery for the date :{DateTime.Now}", ee);
                throw ee;
            }

        }

        public DateTime DeltaDate
        {
            get
            {
                if (!this.IsFirstTimeDiscovery)
                {
                    var lastRunOn = this.Process.LastRunOn;
                    if (lastRunOn.HasValue)
                    {
                        return new DateTime(lastRunOn.Value.Year, lastRunOn.Value.Month, lastRunOn.Value.Day);
                    }
                }
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            }
        }

        public override bool ChangePassword(string UserName, string Password, long DIUserId, bool isReactivation)
        {
            throw new NotImplementedException();
        }

        #region Organisation 

        void getOrganisation()
        {
            sF_Organisation = new SF_FOCompany(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.GetOrganisations();

            sF_Organisation = new SF_FOBusinessUnit(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.GetOrganisations();

            sF_Organisation = new SF_FODivision(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.GetOrganisations();

            sF_Organisation = new SF_FODepartment(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.GetOrganisations();

            sF_Organisation = new SF_cust_Unit(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.GetOrganisations();
        }

        void getCostCenter()
        {
            var sf_FOCostCener = new SF_FOCostCenter(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);
            sf_FOCostCener.GetCostCenter();
        }

        void getPositions()
        {
            var sf_Position = new SF_Position(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);
            sf_Position.GetPosition();
        }

        void getDeltaPositions(DateTime? Dt)
        {
            if (Dt == null)
            {
                Dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day - 2);
            }
            var sf_Position = new SF_Position(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);
            sf_Position.GetPosition(Dt.Value);
        }
        void getOrganisationHead()
        {
            SF_Organisation sF_Organisation = new SF_FOCompany(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.UpdateOrgHead();

            sF_Organisation = new SF_FOBusinessUnit(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.UpdateOrgHead();

            sF_Organisation = new SF_FODivision(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.UpdateOrgHead();

            sF_Organisation = new SF_FODepartment(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.UpdateOrgHead();

            sF_Organisation = new SF_cust_Unit(this.GetContainer(), _idmsLogger, MasterConfigHelperServiceUser);

            sF_Organisation.UpdateOrgHead();
        }

        #endregion Organisation 

        #region Discovery Methods

        void completeDiscovery()
        {
            sF_User.Initialize();
            sF_User.FullDiscovery();
        }
        void deltaDiscovery(DateTime? deltaDate)
        {
            _idmsLogger.WriteLog(LogLevelType.Info, $"Delta Discovery Starts");

            if (!deltaDate.HasValue)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"Delta Discovery : Delta Date is empty or Invalid. Hence IDMS cannot discover.");
                return;
            }
            _idmsLogger.WriteLog(LogLevelType.Info, $"Delta Discovery Starts for the date :{deltaDate.Value.ToString(SF_Helper.DateFormat)}");
            sF_User.Initialize();
            sF_User.DeltaDiscovery(new DateTime(deltaDate.Value.Year, deltaDate.Value.Month, deltaDate.Value.Day));
        }


        public override DateTime GetEffectiveDateByPernr(string pernr, EmployeeEvents eventType, DateTime effectiveStartDate, out bool isTransferWithRetain, EmployeeEventTriggerComment eventTriggerComment = null)
        {
            _idmsLogger.WriteLog(LogLevelType.Info, $"SF:GetEffectiveDateByPernr Starts - PERNR:[{pernr}], EventType:[{eventType.ToString()}], EffectiveStartDate:[{effectiveStartDate}]");
            SFOData.EmpJob empJob = null;
            try
            {
                isTransferWithRetain = false;
                var dtEffectiveDate = new DateTime(effectiveStartDate.Year, effectiveStartDate.Month, effectiveStartDate.Day);

                List<EventTypeReason> triggerConfigValues = new List<EventTypeReason>();

                if (eventTriggerComment == null)
                {
                    switch (eventType)
                    {
                        case EmployeeEvents.NewJoinee:
                            triggerConfigValues = MasterConfigHelper.OnBoardTriggers;
                            break;
                        case EmployeeEvents.Rehire:
                            triggerConfigValues = MasterConfigHelper.RejoinTriggers;
                            break;
                        case EmployeeEvents.OUChange:
                            triggerConfigValues = MasterConfigHelper.TranferTriggers;
                            break;
                        case EmployeeEvents.Resignation:
                            triggerConfigValues = MasterConfigHelper.OffBoardTriggers;
                            break;
                        case EmployeeEvents.ReOrganisation:
                            triggerConfigValues = MasterConfigHelper.ReOrgTriggers;
                            break;
                        case EmployeeEvents.Promotion:
                            triggerConfigValues = MasterConfigHelper.PromotionTriggers;
                            break;
                        case EmployeeEvents.Propagation:
                            triggerConfigValues = MasterConfigHelper.PropagationTriggers;
                            break;
                    }
                }
                else
                {
                    triggerConfigValues.Add(new EventTypeReason() { EventReason = eventTriggerComment.EventReason, EventType = eventTriggerComment.EventCode, RetainAccessForTransfer = eventTriggerComment.RetainAccessForTransfer });
                }


                var query = this._entityContainer.EmpJob
                        .Where(e => e.userId == pernr.TrimLeadingSeros())
                        .OrderByDescending(e => e.startDate)
                        .Select(e => new SFOData.EmpJob()
                        {
                            userId = e.userId,
                            startDate = e.startDate,
                            endDate = e.endDate,
                            position = e.position,
                            costCenter = e.costCenter,
                            company = e.company,
                            businessUnit = e.businessUnit,
                            division = e.division,
                            department = e.department,
                            customString3 = e.customString3,
                            eventReason = e.eventReason,
                            customDate2 = e.customDate2,
                            customString24Nav = e.customString24Nav != null ? new SFOData.PicklistOption() { id = e.customString24Nav.id, externalCode = e.customString24Nav.externalCode }: null,
                            eventNav = new SFOData.PicklistOption() { id = e.eventNav.id, externalCode = e.eventNav.externalCode }
                        });

                var jobEvents = query.ToList();

                empJob = jobEvents.FirstOrDefault(e =>
                {
                    var flag = e.eventNav != null &&
                    triggerConfigValues.Any(trigger => trigger.EventType == e.eventNav.externalCode && trigger.EventReason == e.eventReason) &&
                    e.endDate >= dtEffectiveDate;

                    if (eventType == EmployeeEvents.NewJoinee || eventType == EmployeeEvents.Rehire || eventType == EmployeeEvents.Resignation)
                    {
                        flag = flag && e.endDate >= DateTime.Today;
                    }

                    if (eventType != EmployeeEvents.NewJoinee && eventType != EmployeeEvents.Rehire)
                    {
                        flag = flag && e.startDate == dtEffectiveDate;
                    }

                    return flag;
                });


                if (empJob != null)
                {
                    if (eventType == EmployeeEvents.OUChange)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"Validate Trigger in {this.Endpoint.DisplayName}: Starts - PERNR:[{pernr}], EventType:[{eventType.ToString()}], EffectiveStartDate:[{effectiveStartDate}], RetainAccessForTransfer:{empJob?.customString24Nav?.externalCode}");
                        if (empJob.customString24Nav != null && !string.IsNullOrWhiteSpace(empJob.customString24Nav.externalCode) && !string.IsNullOrWhiteSpace(this.TransferTriggerNotes) && empJob.customString24Nav.externalCode.Trim().ToUpper() == this.TransferTriggerNotes.ToUpper())
                        {
                            isTransferWithRetain = true;
                        }
                    }
                    return empJob.startDate;
                }
                else
                    return new DateTime();
            }
            catch(Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Warning, $"Error occured - GetEffectiveDateByPernr End - PERNR:[{pernr}], EventType:[{eventType.ToString()}], EffectiveStartDate:[{effectiveStartDate}]",ex);
                throw ex;
            }
            finally
            {
                this._entityContainer.ClearPendingChanges();
                _idmsLogger.WriteLog(LogLevelType.Info, $"SF:GetEffectiveDateByPernr End - PERNR:[{pernr}], EventType:[{eventType.ToString()}], EffectiveStartDate:[{effectiveStartDate}]");
            }
        }

        public override void SyncTelephoneDirectory()
        {
            sF_User.Initialize();
            sF_User.SyncTelephoneDirectory();
        }

        public override int DownloadAllUsersFromHR(DateTime scanDate)
        {
            var recordCount = 0;
            try
            {
                var dt = new DateTime(scanDate.Year, scanDate.Month, scanDate.Day);
                getOrganisation();
                getCostCenter();
                getDeltaPositions(dt);
                _idmsLogger.WriteLog(LogLevelType.Info, $"DownloadAllUsersFromHR Starts");
                sF_User.Initialize();
                recordCount = sF_User.DeltaDiscovery(dt);

                _idmsLogger.WriteLog(LogLevelType.Info, string.Format("{0} Users found in Trusted Source", recordCount.ToString()));
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Exception at DownloadAllUsersFromHR", ex));
                recordCount = -1;
            }
            return recordCount;
        }

        public override void UpdateAllUserFromHRMS(List<string> perNos)
        {
            if (perNos != null && perNos.Count > 0)
            {
                try
                {
                    var CurrentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    sF_User.Initialize();


                    if (!this.IsFirstTimeDiscovery)
                    {
                        var lastRunOn = this.Process.LastRunOn;
                        if (lastRunOn.HasValue)
                        {
                            CurrentDate = new DateTime(lastRunOn.Value.Year, lastRunOn.Value.Month, lastRunOn.Value.Day);
                        }
                    }
                    var notify = MasterConfigHelper.EnableSFFailedNotification;
                    foreach (var perNo in perNos)
                    {
                        try
                        {
                            var sfDiscoveryStatus = sF_User.discoverUserByPERNR(perNo.PadWithZeros(SF_Helper.EmployeeIdMaxSize), CurrentDate);
                            if (sfDiscoveryStatus != null && sfDiscoveryStatus.SFDiscoveryStatusType == SFDiscoveryStatusType.Failure)
                            {
                                try
                                {
                                    _idmsLogger.WriteLog(LogLevelType.Warning, $"Disocvery failed for User:{perNo} and message:{sfDiscoveryStatus?.Message}");
                                    if (notify)
                                        fnEmailSend.Invoke(this.Endpoint, this._idmsLogger, sfDiscoveryStatus.Message, perNo);
                                }
                                catch
                                {
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception at DownloadAllUserFromHR PerNo:{perNo} ex:{ex.Message}", ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Exception at DownloadAllUserFromHR", ex));
                }
            }
            else
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, $"perNos empty");
            }
        }

        public override void DownloadSpecificUserFromHR(string pernr)
        {
            try
            {
                getOrganisation();
                getCostCenter();
                getDeltaPositions(this.DeltaDate);
                sF_User.Initialize();
                sF_User.GetUserByPernr(pernr);
                // pernr is attacker-influenced, so it is bound as a real SqlParameter ("{0}" is
                // an EF token, not a string interpolation substitution) instead of being
                // concatenated into the SQL text.
                User.ExecuteQuery(@" UPDATE A Set A.Supervisor = B.DisplayName FROM DI_M_User A inner join DI_M_User B on A.ROEmployeeId = B.EmployeeId WHERE  A.EmployeeId={0} ", pernr.PadWithZeros(SF_Helper.EmployeeIdMaxSize));
            }
            catch (Exception ex)
            {
                WriteDiscoveryLog(LogLevelType.Exception, string.Format("Exception at DownloadSpecificUserFromHR", ex));
                throw;
            }
        }

        #endregion Discovery Methods

        public DataTable GetTableInfo(string detail, string PERNR)
        {
            return sF_User.GetDataTableInfo(detail, PERNR);
        }

        #region Update & Delimit Methods

        HttpWebRequest GetHttpPostMethod(JObject rss, bool isPurgeTypeFull = false)
        {
            var uri = new Uri(this._entityContainer.BaseUri, (isPurgeTypeFull ? "upsert?purgeType=full" : "upsert"));
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers.Set("Authorization", $"Basic {_base64Creds}");

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(rss.ToString());
            }

            return request;
        }

        HttpWebRequest GetHttpPostMethod(JArray rss, bool isPurgeTypeFull = false)
        {
            var uri = new Uri(this._entityContainer.BaseUri, (isPurgeTypeFull ? "upsert?purgeType=full" : "upsert"));
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers.Set("Authorization", $"Basic {_base64Creds}");

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(rss.ToString());
            }

            return request;
        }

        public override bool AddFLINSLogonIdInHR(string employeeId, string sapLogonId, DateTime beginDate)
        {
            bool resultflag = false;
            try
            {
                var status = this.sF_User.GetUserStatusByPernr(employeeId);
                if (string.IsNullOrWhiteSpace(status))
                {
                    WriteProvisioningLog(LogLevelType.Warning, $"Cannot find the Employee Status(PERNO:{employeeId}) in HRMS");
                    return false;
                }
                JObject rss =
                 new JObject(
                        new JProperty("__metadata",
                              new JObject(new JObject(new JProperty("uri", $@"{_url}/User('{employeeId}')"),
                                                      new JProperty("type", "SFOData.User")
                                                      )
                                                      )),
                        new JProperty("username", sapLogonId.Trim().ToUpper()),
                        new JProperty("status", status)
                            );

                HttpWebRequest request = GetHttpPostMethod(rss);

                var httpResponse = (HttpWebResponse)request.GetResponse();
                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                if (httpResponse.StatusCode == HttpStatusCode.OK && result.GetUpsertStatus() == "OK")
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Successfully updated the FLINS LoginId for the PERNR:{employeeId}");

                    resultflag = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(result) && !result.Contains("The user is an inactive user", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception($"Failed in updating the FLINS LoginId for the PERNR:{employeeId}. The reason is, [{result}]");
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"The user is an inactive user. PERNR:{employeeId}");
                        resultflag = true;
                    }
                }
            }
            catch (Exception e)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, e.Message, e);

            }
            return resultflag;
        }

        public override bool AddEmailIdInHR(string employeeId, string emailId, DateTime beginDate)
        {
            bool resultflag = false;
            try
            {
                
                SFOData.PickListValueV2 emailType = sF_User.GetEmailType();

                if (emailType == null || emailType.optionId.HasValue == false)
                {
                    throw new Exception("Could not find EmailType for Business Email.");
                }

                JObject rss =
                new JObject(
                       new JProperty("__metadata",
                             new JObject(new JObject(new JProperty("uri", $@"{_url}/PerEmail(emailType='{emailType.optionId}',personIdExternal='{employeeId.TrimLeadingSeros()}')"),
                                                     new JProperty("type", "SFOData.PerEmail")
                                                     )
                                                     )),
                       new JProperty("emailAddress", emailId),
                       new JProperty("isPrimary", true)
                           );

                HttpWebRequest request = GetHttpPostMethod(rss, true);

                var httpResponse = (HttpWebResponse)request.GetResponse();

                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }


                if (httpResponse.StatusCode == HttpStatusCode.OK && result.GetUpsertStatus() == "OK")
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Successfully updated the Email Id for the PERNR:{employeeId}");

                    resultflag = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(result) && !result.Contains("The user is an inactive user", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception($"Failed in create/update the Email Id for the PERNR:{employeeId}. The reason is, [{result}]");
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"The user is an inactive user. PERNR:{employeeId}");
                        resultflag = true;
                    }
                }
            }
            catch (Exception e)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, e.Message, e);

            }
            return resultflag;
        }

        public override bool AddTelephoneInHR(string employeeId, string telephone, string extn, DateTime beginDate)
        {
            bool resultflag = false;
            try
            {
                if (telephone == null)
                {
                    telephone = string.Empty;
                }

                //TODO: Ashok Below 2 lines copied from SAP HRMS -  Need to confirm
                extn = string.Empty;
                string maskedtelPhone = telephone.Trim(); //.PadRight(30, ' ') + extn?.PadRight(10, ' ');

                var phonetype = sF_User.PhoneTypeForTelephone;
                SFOData.PickListValueV2 off_phoneType = sF_User.GetOfficePhoneType();

                if (off_phoneType == null || off_phoneType.optionId.HasValue == false)
                {
                    throw new Exception("Could not find Office honeType.");
                }
                else
                {
                    phonetype = off_phoneType.optionId.Value.ToString();
                }

                var phoneList = this._entityContainer.PerPhone.Where(p => p.personIdExternal == employeeId.TrimLeadingSeros() && p.phoneType != phonetype)
                                          .Select(p => new SFOData.PerPhone() { personIdExternal = p.personIdExternal, isPrimary = p.isPrimary, phoneType = p.phoneType, phoneNumber = p.phoneNumber }).ToList();

                JArray rss = new JArray();
                foreach (var phone in phoneList)
                {
                    rss.Add(new JObject(new JProperty("__metadata",
                                new JObject(new JObject(new JProperty("uri", $@"{_url}/PerPhone(phoneType='{phone.phoneType}',personIdExternal='{phone.personIdExternal}')"),
                                                        new JProperty("type", "SFOData.PerPhone")
                                                        )
                                                        )),
                      new JProperty("phoneNumber", phone.phoneNumber),
                      new JProperty("isPrimary", phone.isPrimary),
                      new JProperty("phoneType", phone.phoneType)));
                }

                bool hasPrimaryPhones = phoneList.Any(p => p.isPrimary == true);

                rss.Add(
                  new JObject(
                      new JProperty("__metadata",
                            new JObject(new JObject(new JProperty("uri", $@"{_url}/PerPhone(phoneType='{phonetype}',personIdExternal='{employeeId.TrimLeadingSeros()}')"),
                                                    new JProperty("type", "SFOData.PerPhone")
                                                    )
                                                    )),
                      new JProperty("phoneNumber", maskedtelPhone),
                      new JProperty("isPrimary", !hasPrimaryPhones),
                      new JProperty("phoneType", phonetype)
                          ));

                HttpWebRequest request = GetHttpPostMethod(rss, true);

                var httpResponse = (HttpWebResponse)request.GetResponse();

                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                if (httpResponse.StatusCode == HttpStatusCode.OK && result.GetUpsertStatus() == "OK")
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Successfully updated the Phone:{maskedtelPhone} for the PERNR:{employeeId}");

                    resultflag = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(result) && !result.Contains("The user is an inactive user", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new Exception($"Failed in create/update the phone :{maskedtelPhone} for the PERNR:{employeeId}. The reason is, [{result}]");
                    }
                    else
                    {
                        _idmsLogger.WriteLog(LogLevelType.Info, $"The user is an inactive user. PERNR:{employeeId}");
                        resultflag = true;
                    }
                }

            }
            catch (Exception e)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, e.Message, e);

            }
            return resultflag;
        }

        public override bool DelimitTelephoneInHR(string employeeId, string telephone, string extn, DateTime beginDate)
        {
            bool resultflag = false;
            try
            {
                var phonetype = sF_User.PhoneTypeForTelephone;
                SFOData.PickListValueV2 off_phoneType = sF_User.GetOfficePhoneType();

                if (off_phoneType == null || off_phoneType.optionId.HasValue == false)
                {
                    throw new Exception("Could not find Office honeType.");
                }
                else
                {
                    phonetype = off_phoneType.optionId.Value.ToString();
                }

                var phoneList = this._entityContainer.PerPhone.Where(p => p.personIdExternal == employeeId && p.phoneType != phonetype)
                                          .Select(p => new SFOData.PerPhone() { personIdExternal = p.personIdExternal, isPrimary = p.isPrimary, phoneType = p.phoneType, phoneNumber = p.phoneNumber }).ToList();

                if (phoneList == null || phoneList.Count() == 0)
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"No phone details found to update for the PERNR:{employeeId}");
                    return true;
                }

                bool hasPrimaryPhones = phoneList.Any(p => p.isPrimary == true);

                if (!hasPrimaryPhones)
                {
                    phoneList.First().isPrimary = true;
                }

                JArray rss = new JArray();

                foreach (var phone in phoneList)
                {
                    rss.Add(new JObject(new JProperty("__metadata",
                                new JObject(new JObject(new JProperty("uri", $@"{_url}/PerPhone(phoneType='{phone.phoneType}',personIdExternal='{phone.personIdExternal}')"),
                                                        new JProperty("type", "SFOData.PerPhone")
                                                        )
                                                        )),
                      new JProperty("phoneNumber", phone.phoneNumber),
                      new JProperty("isPrimary", phone.isPrimary.HasValue ? phone.isPrimary.Value : false),
                      new JProperty("phoneType", phone.phoneType)));
                }

                HttpWebRequest request = GetHttpPostMethod(rss, true);

                var httpResponse = (HttpWebResponse)request.GetResponse();

                string result = string.Empty;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                if (httpResponse.StatusCode == HttpStatusCode.OK && result.GetUpsertStatus() == "OK")
                {
                    _idmsLogger.WriteLog(LogLevelType.Info, $"Successfully removed the Business Phones for the PERNR:{employeeId}");

                    resultflag = true;
                }
                else
                {
                    throw new Exception($"Failed in removing the business phones for the PERNR:{employeeId}. The reason is, [{result}]");
                }
            }
            catch (Exception e)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, e.Message, e);

            }
            return resultflag;
        }

        #endregion Update & Delimit Methods 

        #region Email Send 

       readonly SendmailDelegate fnEmailSend = (endpoint, idmsLogger, message, employeeId) =>
      {
          string strMessage = string.Empty;
          StringBuilder strBldr = new StringBuilder();
          DomainModel.MailTemplate template = null;
          StringBuilder subject = null;
          try
          {
              var pName = "Successfactor Discovery Process";
              EmailInfo emailInfo = new EmailInfo();
              template = MailTemplate.GetByTemplateName(Constants.DiscoverSpecificUserFailed);
              subject = new StringBuilder(template.MailSubject);
              subject.Replace("@@ENDPOINTNAME@@", endpoint.DisplayName);
              subject.Replace("@@PROCESSNAME@@", pName);
              subject.Replace("@@EMPLOYEEID@@", employeeId);

              StringBuilder body = new StringBuilder(template.MailContent);
              body.Replace("@@ERRORMESSAGE@@", message);
              body.Replace("@@ENDPOINTTYPE@@", endpoint.EndPointSystemType.DisplayName);
              body.Replace("@@ENDPOINTNAME@@", endpoint.DisplayName);
              body.Replace("@@PROCESSNAME@@", pName);
              body.Replace("@@EMPLOYEEID@@", employeeId);
              body.Replace("@@REGARDS@@", MasterConfigHelper.EmailRegards);

              emailInfo.Body = body.ToString();
              emailInfo.Subject = subject.ToString();
              emailInfo.To = string.Format("{0}", string.Join(",", MasterConfigHelper.FailureNotificationReceivers));
              emailInfo.From = template.MailFrom;
              if (!string.IsNullOrWhiteSpace(template.MailCC))
                  emailInfo.Cc = template.MailCC;
              if (!string.IsNullOrWhiteSpace(template.MailBcc))
                  emailInfo.Bcc = template.MailBcc;
              emailInfo.ActivityType = EmailActivityType.Discovery.ToString();
              EmailHandler.Send(emailInfo);
          }
          catch (Exception ex)
          {
              idmsLogger.WriteLog(LogLevelType.Exception, "Failed in composing mail from template:DiscoverSpecificUserFailed", ex);
          }
        };

        #endregion
    }
}
