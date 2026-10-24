using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace DI.Chakra.DomainModel
{
    public class EamNovaHelper
    {
        readonly EndPoint _flinsEndpoint;
        readonly EndPoint _flinsBiEndpoint;
        readonly EndPoint _adEndpoint;
        readonly NovaMatrixResponse _novaMatrixResponse;
        readonly EndPoint NovaEndpoint;
        readonly IdmsLogger _idmsLogger;
        readonly string _currentuser;
        Process _process;

        public  readonly List<string> FlinsExculsiveEAM = new List<string>();
        public readonly List<string> FlinsBIExculsiveEAM = new List<string>();
        public readonly List<string> ADGroupExculsiveEAM = new List<string>();

        readonly List<string> FlinsCommonRole = new List<string>();
        readonly List<string> FlinsBiCommonRole = new List<string>();

        volatile Dictionary<long,SAPEamNovaUserInfo> _allNovaUser = new Dictionary<long,SAPEamNovaUserInfo>();
        Dictionary<long, string> dicUsers = new Dictionary<long, string>();
        public EamNovaHelper(Infrastructure.NovaMatrixType novaMatrixType, EndPoint endpoint, Process process, IdmsLogger idmsLogger)
        {
            _flinsBiEndpoint = loadEndpoint(ApplicationType.FLINS_BI);
            _flinsEndpoint = loadEndpoint(ApplicationType.HRMS_FLINS);
            _adEndpoint = loadEndpoint(ApplicationType.ADSingaporePowerLocal);
            _novaMatrixResponse = loadMatrixData(new NovaMatrixParmeter() { novaMatrixType = novaMatrixType });
            NovaEndpoint = endpoint;
            _idmsLogger = idmsLogger;
            _currentuser = MasterConfigHelper.ServiceUser;
            _process = process;

            _novaMatrixResponse.NovaMatrixResponseDetails.ForEach(s =>
            {
                FlinsExculsiveEAM.AddRange(s.FlinsTCode);
                FlinsBIExculsiveEAM.AddRange(s.FlinsBiTCode);
                ADGroupExculsiveEAM.AddRange(s.ADGroup);
            });


            var Novafields = NProvisioningField.GetByEndpointSystemId(NovaEndpoint.Id);
            if (Novafields != null && Novafields.Count() > 0)
            {
                var value = Novafields.FirstOrDefault(i => i.FieldName.Equals("CommonRoles", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(i.Value));
                if (value != null)
                {
                    FlinsExculsiveEAM.AddRange(value.Value.Split(',').ToList());
                    FlinsCommonRole.AddRange(value.Value.Split(',').ToList());
                }

                  value = Novafields.FirstOrDefault(i => i.FieldName.Equals("BWRoles", StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrWhiteSpace(i.Value));
                if (value != null)
                {
                    FlinsBIExculsiveEAM.AddRange(value.Value.Split(',').ToList());
                    FlinsBiCommonRole.AddRange(value.Value.Split(',').ToList());
                }
            }

        }

        public void SetProcess(Process process)
        {
            _process = process;
        }
        public EndPoint FlinsEndpoint => _flinsEndpoint;

        public EndPoint FlinsBiEndpoint => _flinsBiEndpoint;

        public EndPoint ADEndpoint => _adEndpoint;

        public NovaMatrixResponse NovaMatrixResponseData => _novaMatrixResponse;

        public string Currentuser => _currentuser;

        EndPoint loadEndpoint(ApplicationType type)
        {
            return DomainModel.EndPoint.GetByApplicationType(type);
        }

        NovaMatrixResponse loadMatrixData(NovaMatrixParmeter parameter)
        {
            return DomainModel.NOVAUserRoleMatrix.GetNovaMatrix(parameter);
        }

        public  Dictionary<long, string> DiscoverUsers()
        {
            dicUsers = new Dictionary<long, string>();
            try
            {
                _idmsLogger.WriteLog(LogLevelType.Info, $"NOVA Discover started");

                try {
                    //_idmsLogger.WriteLog(LogLevelType.Info, $"Start Truncating the table DI_M_EAMNovaHangingRoleInfo");
                    EAMNovaHangingRoleInfo.ExecuteQueryWithCommandTimeout(" TRUNCATE TABLE DI_M_EAMNovaHangingRoleInfo ", 0);
                    //_idmsLogger.WriteLog(LogLevelType.Info, $"End Truncating the table DI_M_EAMNovaHangingRoleInfo");
                }
                catch (Exception e)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, $"Exception occured whild truncating the table DI_M_EAMNovaHangingRoleInfo.",e);
                }

                var accountTypes = new[] { AccountType.User, AccountType.Shared, AccountType.Service, AccountType.Secondary };
                foreach (var accountType in accountTypes)
                {
                    var accountTypeString = UtilityMethods.GetEnumAttribute<Description>(accountType);

                    //_idmsLogger.WriteLog(LogLevelType.Info, $"Started NOVA Discovering {accountTypeString}s");
                    Dictionary<long,SAPEamNovaUserInfo> listofUsers = new Dictionary<long, SAPEamNovaUserInfo>();
                    _allNovaUser.Clear();
                    listofUsers = this.GetUsers(accountType);
                    SAPEamNovaUserInfo.SaveUsers(listofUsers.Values.ToList(), NovaEndpoint.Id, _currentuser, accountType);
                    listofUsers?.Clear();
                    listofUsers = null;
                    //_idmsLogger.WriteLog(LogLevelType.Info, $"Completed NOVA {accountTypeString}s" );
                }
                IList<SAPEamNovaUserInfo> list   = SAPEamNovaUserInfo.GetDeletedUsers(NovaEndpoint.Id, _process);
                SAPEamNovaUserInfo.RemoveNovaUser(list.ToList(), _currentuser);
                list?.Clear();
                list = null;
                AccountReconcilation.ProcessDeletedAccounts(NovaEndpoint.Id, _process, _currentuser);
                dicUsers = SAPEamNovaUserInfo.GetAllByEndPointSystemId(NovaEndpoint.Id);
                _idmsLogger.WriteLog(LogLevelType.Info, $"NOVA Discover end");

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Error while Discovery NOVA Users.", ex);
                throw ex;
            }
            return dicUsers;
        }

        public bool GetSpecificUsers(long DIUserId)
        {
            Dictionary<long, string> keyValuePairs = new Dictionary<long, string>();
            try
            {
                var accountTypes = new[] { AccountType.User, AccountType.Shared, AccountType.Service, AccountType.Secondary };
                foreach (var accountType in accountTypes)
                {
                    var accountTypeString = UtilityMethods.GetEnumAttribute<Description>(accountType);
                    _allNovaUser.Clear();
                    if (accountType == AccountType.User)
                    {

                        var accountRecons = AccountReconcilation.GetAllActiveAccountsByUserIdAndEndpointSystemId(DIUserId, FlinsEndpoint.Id, accountType);

                        deleteHangingRoles(accountRecons);

                        foreach (var accountRecon in accountRecons)
                        {
                            processNovaUser(accountRecon.Id, DIUserId, accountType, accountTypeString);
                        }
                    }
                    else
                    {
                        var accountRecons = DomainModel.AccountReconcilation.GetActiveAccountByUser(DIUserId,FlinsEndpoint, accountType);
                        deleteHangingRoles(accountRecons);
                        processNovaUser(accountRecons, accountType, accountTypeString);

                    }

                    SAPEamNovaUserInfo.SaveUsers(_allNovaUser.Values.ToList(), NovaEndpoint.Id, _currentuser, accountType);

                }
                keyValuePairs = SAPEamNovaUserInfo.GetAllByEndPointSystemId(NovaEndpoint.Id, DIUserId);
                UpdateEntitlementRecon(keyValuePairs);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, $"Error occured at NOVA GetSpecificUsers Discovery for the user:{DIUserId} ", ex);
                return false;
            }
            return true;
        }

        void deleteHangingRoles(List<AccountReconcilation> list)
        {
            try {

                if (list == null || list.Count() == 0) return;
                var accountnames = list.Select(a => a.AccountName).ToList();
                var query = $" DELETE FROM  DI_M_EAMNovaHangingRoleInfo WHERE ACCOUNTNAME in ('{string.Join(",'", accountnames)}')";
                EAMNovaHangingRoleInfo.ExecuteQueryWithCommandTimeout(query, 0);
            }
            catch (Exception e)
            { }
        }

        Dictionary<long,SAPEamNovaUserInfo> GetUsers(AccountType accountType)
        {
            try
            {
                var accountTypeString = UtilityMethods.GetEnumAttribute<Description>(accountType);
                if (accountType == AccountType.User)
                {
                    Dictionary<long, long?> allFlinsAccount = DomainModel.AccountReconcilation.GetAllAccountByEndpointSystemId(FlinsEndpoint.Id, accountType);
                    _idmsLogger.WriteLog(LogLevelType.Info, string.Format($"Found {allFlinsAccount.Count} Flins {accountTypeString}(s)"));

                    foreach (var id in allFlinsAccount.Keys)
                    {
                        processNovaUser(id, allFlinsAccount[id], accountType, accountTypeString);
                    }
                }
                else
                {
                    //var allUsers = DomainModel.User.GetActiveEmployees();
                    //_idmsLogger.WriteLog(LogLevelType.Info, string.Format($"Start Discovering EAM NOVA - AccountType :{accountTypeString} from all {allUsers} active user(s)"));
                     
                    processNovaUser(accountType, accountTypeString);
                }

                //_idmsLogger.WriteLog(LogLevelType.Info, $"Found {_allNovaUser.Count} Nova User(s) : AccountType - {accountTypeString}");
                return _allNovaUser;
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Error, "Error occured at NOVA Discovery of NOVA GetUsers", ex);
                throw;
            }
            finally
            {

            }
        }

        public void processNovaUser(AccountType accountType, string actionTypestring)
        {
            var accountRecons = DomainModel.AccountReconcilation.GetActiveAccountByAccountTypeAndEndpointSystemId(FlinsEndpoint, accountType);
            processNovaUser(accountRecons, accountType, actionTypestring);
        }

        public void processNovaUser(List<AccountReconcilation> accountRecons, AccountType accountType, string actionTypestring)
        {
            _idmsLogger.WriteLog(LogLevelType.Info, $"Start Process EAM accounts for AccountType :  {actionTypestring}. Total no. of accounts is {accountRecons.Count()}");
            foreach (var accountRecon in accountRecons)
            {
                checkNullAndClear();
                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Start Process FLINS Account : {accountRecon.AccountName} in EamNovaHelper.processNovaUser - AccountType :  {actionTypestring}");

                if (accountRecon == null)
                    continue;

                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AccountReconciliationId : {accountRecon.Id}, EndpointSystemId:{accountRecon.EndpointSystemId}, AccountName :{accountRecon.AccountName}, EndpointUserId:{accountRecon.EndpointUserId}");

                var entitlementRecons = DomainModel.EntitlementReconcilation.GetAllByAccountReconcilationId(accountRecon.Id);

                flins_entitlement.AddRange( entitlementRecons.Where(e => !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Entitlement.Trim()))
                                           .Select(e => e.Entitlement.Trim()));

                var selectedmatrix = NovaMatrixResponseData.NovaMatrixResponseDetails.Where(n =>
                {
                    return n.novaMatrixType == NovaMatrixType.Exceptional && n.FlinsTCode.All(f => !string.IsNullOrWhiteSpace(f) && flins_entitlement.Contains(f));

                }).Select(n => n).ToList();

                if (selectedmatrix.Count() > 0)
                {
                    var novaUser = new SAPEamNovaUserInfo();
                    novaUser.EndPointSystemId = NovaEndpoint.Id;

                    novaUser.FLINS_SAPUserInfoId = accountRecon.EndpointUserId;
                    novaUser.FLINS_UserLogonName = accountRecon.AccountName;
                    novaUser.IsLocked = false;
                    novaUser.UserId = accountRecon.UserId.HasValue ? accountRecon.UserId.Value : 0;

                    printSelectedMatrix(selectedmatrix, "FLINS");

                    if (novaUser.SAPEamNovaUserRoles == null)
                    {
                        novaUser.SAPEamNovaUserRoles = new List<SAPEamNovaUserRoleInfo>();
                    }

                    foreach (var matrix in selectedmatrix)
                    {

                        if (!string.IsNullOrWhiteSpace(matrix.Position) && !novaUser.SAPEamNovaUserRoles.Any(r => r.RoleName == matrix.Position.Trim()))
                        {
                            var sapeam = new SAPEamNovaUserRoleInfo()
                            {
                                Company = string.IsNullOrWhiteSpace(matrix.Company) ? null : matrix.Company.Trim(),
                                NovaPositionMatrixType = matrix.novaMatrixType,
                                FLINS_TCodes = string.Join(",", matrix.FlinsTCode),
                                FLINS_BI_TCodes = "",
                                AD_Groups = "",
                                IsActive = true,
                                DepartmentBranch = string.IsNullOrWhiteSpace(matrix.Department) ? null : matrix.Department.Trim(),
                                Section = string.IsNullOrWhiteSpace(matrix.Section) ? null : matrix.Section.Trim(),
                                RoleName = string.IsNullOrWhiteSpace(matrix.Position) ? string.Join(",", matrix.FlinsTCode) : matrix.Position.Trim()
                            };
                            novaUser.SAPEamNovaUserRoles.Add(sapeam);

                            fillMatchRoles(matrix.FlinsTCode, Matched_flinsRoles);
                            fillMatchRoles(FlinsCommonRole, Matched_flinsRoles);
                            fillMatchRoles(matrix.FlinsBiTCode, Matched_flinsBiRoles);
                            fillMatchRoles(FlinsBiCommonRole, Matched_flinsBiRoles);
                            fillMatchRoles(matrix.ADGroup, Matched_adGroups);
                        }
                    }

                    var isProcessed = isProcessHangingRole(accountType, accountRecon.AccountName);
                    if (novaUser != null && (novaUser.SAPEamNovaUserRoles.Any() || isProcessed))
                    {
                        AddOrUpdateUser(accountRecon.Id, novaUser);
                    }

                    
                     _idmsLogger.WriteLog(LogLevelType.DebugStep, $"NOVA {actionTypestring} for the account name  : {accountRecon.AccountName} and AccountType : {actionTypestring} in EamNovaHelper.processNovaUser special accounts");

                }
            }
            _idmsLogger.WriteLog(LogLevelType.Info, $"End Process EAM accounts for AccountType :  {actionTypestring} ");

        }

        void printSelectedMatrix(List<NovaMatrixResponseDetail> list, string type) {

            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Selected matrix for - {type}");
            list.ForEach(a => {
                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Matrix Details : Position - {a.Position}; MatrixType - {a.novaMatrixType.ToString()}, FLINS : {string.Join(";",a.FlinsTCode)}, FLINSBI - {string.Join(";", a.FlinsBiTCode)}, AD - {string.Join(";", a.ADGroup)}");

            });
        }
        void AddOrUpdateUser(long AccountReconId, SAPEamNovaUserInfo user)
        {
            if (_allNovaUser.ContainsKey(AccountReconId))
                _allNovaUser[AccountReconId] = user;
            else
                _allNovaUser.Add(AccountReconId, user);

        }

        List<string> Matched_adGroups = new List<string>();
        List<string> Matched_flinsRoles = new List<string>();
        List<string> Matched_flinsBiRoles = new List<string>();

        List<string> flins_entitlement = new List<string>();
        List<string> flinsBi_entitlement = new List<string>();
        List<string> ad_entitlement = new List<string>();


         void checkNullAndClear(List<string> obj)
        {
            if (obj == null)
                obj = new List<string>();
            obj.Clear();
        }

        void   checkNullAndClear() {
            checkNullAndClear(Matched_adGroups);
            checkNullAndClear(Matched_flinsRoles);
            checkNullAndClear(Matched_flinsBiRoles);

            checkNullAndClear(flins_entitlement);
            checkNullAndClear(flinsBi_entitlement);
            checkNullAndClear(ad_entitlement);

        }
        void fillMatchRoles(List<string> source, List<string> destination)
        {
            if (source == null) return;

            if (destination == null)
                destination = new List<string>();

            source.ForEach(s => {
                if (!destination.Contains(s))
                    destination.Add(s);
            });
        }

        void processNovaUser(long AccountReconId, long? userId, AccountType accountType, string actionTypestring)
        {
            checkNullAndClear();

            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Start Process for the User : {userId} in EamNovaHelper.processNovaUser - AccountType :  {actionTypestring}");


            List<AccountEntitlementRecon> AccountRecons = DomainModel.EntitlementReconcilation.GetAccountEntitlementReconsByUserId(AccountReconId, userId, new List<long> { FlinsBiEndpoint.Id, ADEndpoint.Id }, accountType);

            var flins = AccountRecons.FirstOrDefault(acc => acc.AccountRecon.EndpointSystemId == FlinsEndpoint.Id && !acc.AccountRecon.IsDeleted && !acc.AccountRecon.IsLocked);

            if (AccountRecons == null || flins == null || flins.AccountRecon==null) return;


            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Selected for the User : {userId} in EamNovaHelper.processNovaUser - AccountType :  {actionTypestring}");
            AccountRecons.ForEach(a => {
                _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AccountReconciliationId : {a.AccountRecon.Id}, EndpointSystemId:{a.AccountRecon.EndpointSystemId}, AccountName :{a.AccountRecon.AccountName}, EndpointUserId:{a.AccountRecon.EndpointUserId}");

            });

            
            var flinsBi = AccountRecons.FirstOrDefault(acc => acc.AccountRecon.EndpointSystemId == FlinsBiEndpoint.Id 
                                      && acc.AccountRecon.AccountName.Equals(flins.AccountRecon.AccountName, StringComparison.InvariantCultureIgnoreCase) && !acc.AccountRecon.IsDeleted && !acc.AccountRecon.IsLocked);
            var ad = AccountRecons.FirstOrDefault(acc => acc.AccountRecon.EndpointSystemId == ADEndpoint.Id 
                                        && acc.AccountRecon.AccountName.Equals(flins.AccountRecon.AccountName, StringComparison.InvariantCultureIgnoreCase) && !acc.AccountRecon.IsDeleted && !acc.AccountRecon.IsLocked);

            if (flins!=null &&  flins.Entitlement!=null )
                flins_entitlement.AddRange(flins.Entitlement.Where(e => !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Entitlement)).Select(e => e.Entitlement.ToUpper().Trim()));

            if (flinsBi != null && flinsBi.Entitlement != null)
                flinsBi_entitlement.AddRange(flinsBi.Entitlement.Where(e => !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Entitlement)).Select(e => e.Entitlement.ToUpper().Trim()));

            if (ad != null && ad.Entitlement != null)
                ad_entitlement.AddRange(ad.Entitlement.Where(e => !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Entitlement)).Select(e => e.Entitlement.ToUpper().Trim()));

            SAPEamNovaUserInfo novaUser = new SAPEamNovaUserInfo();
            novaUser.EndPointSystemId = NovaEndpoint.Id;
            novaUser.UserId = userId ?? 0;

            if (flins != null && flins.AccountRecon != null)
            {
                novaUser.FLINS_SAPUserInfoId = flins.AccountRecon.EndpointUserId;
                novaUser.FLINS_UserLogonName = flins.AccountRecon.AccountName;
            }

            if (flinsBi != null && flinsBi.AccountRecon != null)
            {
                novaUser.FLINS_BI_SAPUserInfoId = flinsBi.AccountRecon.EndpointUserId;
                novaUser.FLINS_BI_UserLogonName = flinsBi.AccountRecon.AccountName;
            }

            if (ad != null && ad.AccountRecon != null)
            {
                novaUser.ADS_UserInfoId = ad.AccountRecon.EndpointUserId;
                novaUser.AD_UserLogonName = ad.AccountRecon.AccountName;
            };

            foreach (var matrix in NovaMatrixResponseData.NovaMatrixResponseDetails)
            {
                bool flinsmatched = IsExist(matrix.FlinsTCode.Select(f => f.ToUpper().Trim()).ToList(), flins_entitlement);
                bool flinsBimatched = IsExist(matrix.FlinsBiTCode.Select(f => f.ToUpper().Trim()).ToList(), flinsBi_entitlement);
                bool admatched = IsExist(matrix.ADGroup.Select(f => f.ToUpper().Trim()).ToList(), ad_entitlement);

                if (flinsmatched && flinsBimatched && admatched)
                {
                    novaUser.SAPEamNovaUserRoles.Add(
                             new SAPEamNovaUserRoleInfo()
                             {
                                 EndPointSystemId = NovaEndpoint.Id,
                                 IsActive = true,
                                 RoleName = matrix.Position.Trim(),
                                 FLINS_TCodes = string.Join(",", matrix.FlinsTCode),
                                 FLINS_BI_TCodes = string.Join(",", matrix.FlinsBiTCode),
                                 AD_Groups = string.Join(",", matrix.ADGroup),
                                 Company = string.IsNullOrWhiteSpace(matrix.Company) ? null : matrix.Company.Trim(),
                                 Section = string.IsNullOrWhiteSpace(matrix.Section) ? null : matrix.Section.Trim(),
                                 DepartmentBranch = string.IsNullOrWhiteSpace(matrix.Department) ? null : matrix.Department.Trim(),
                                 NovaPositionMatrixType = matrix.novaMatrixType
                             });

                    fillMatchRoles(matrix.FlinsTCode, Matched_flinsRoles);
                    fillMatchRoles(FlinsCommonRole, Matched_flinsRoles);
                    fillMatchRoles(matrix.FlinsBiTCode, Matched_flinsBiRoles);
                    fillMatchRoles(FlinsBiCommonRole, Matched_flinsBiRoles);
                    fillMatchRoles(matrix.ADGroup, Matched_adGroups);
                }                
            }

            var isprocessed = isProcessHangingRole(accountType, flins.AccountRecon.AccountName);

            if (novaUser != null && (novaUser.SAPEamNovaUserRoles.Any() || isprocessed) )
            {
                AddOrUpdateUser(AccountReconId, novaUser);
            }

             

            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"End Process for the User : {userId} in EamNovaHelper.processNovaUser - {actionTypestring}");
            return;
        }

        bool  isProcessHangingRole(AccountType accountType, string accountName)
        {
            bool flag = false;
           
            var hangingFlinsRoles = FlinsExculsiveEAM.Intersect(flins_entitlement).ToList()
                              .Where(i => !Matched_flinsRoles.Contains(i, StringComparer.InvariantCultureIgnoreCase) );
            var hangingADGroups = ADGroupExculsiveEAM.Intersect(ad_entitlement).ToList().Where(i => !Matched_adGroups.Contains(i, StringComparer.InvariantCultureIgnoreCase));
            var hangingBIRoles = FlinsBIExculsiveEAM.Intersect(flinsBi_entitlement).ToList().Where(i => !Matched_flinsBiRoles.Contains(i, StringComparer.InvariantCultureIgnoreCase));
             
            if (hangingADGroups.Count() > 0 || hangingBIRoles.Count() > 0 || hangingFlinsRoles.Count() > 0 )
            {
                EAMNovaHangingRoleInfo roleInfo = new EAMNovaHangingRoleInfo();
                roleInfo.AccountName = accountName;
                roleInfo.AccountType = accountType;
                roleInfo.EndPointSystemId = this.NovaEndpoint.Id;
                roleInfo.ADGroups = string.Join(",", hangingADGroups);
                roleInfo.BIRoles = string.Join(",", hangingBIRoles);
                roleInfo.FLINSRoles = string.Join(",", hangingFlinsRoles);
               
                roleInfo.CreatedDate = DateTime.Now;
                roleInfo.CreatedBy = "System";
                roleInfo.Save("System");
                flag = true;
            }

            return flag;
        }

        #region Commented

        //void processNovaUser(long AccountReconId, long? userId, AccountType accountType,string actionTypestring)
        //{
        //    var adGroups = new List<string>(); 
        //    var flinsRoles = new List<string>();
        //    var flinsBiRoles = new List<string>();

        //    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Start Process for the User : {userId} in EamNovaHelper.processNovaUser - AccountType :  {actionTypestring}");


        //    List<AccountEntitlementRecon> AccountRecons = DomainModel.EntitlementReconcilation.GetAccountEntitlementReconsByUserId(AccountReconId, userId, new List<long> { FlinsBiEndpoint.Id, ADEndpoint.Id }, accountType);

        //    var flinsAccount = AccountRecons.FirstOrDefault(acc => acc.AccountRecon.EndpointSystemId == FlinsEndpoint.Id && acc.AccountRecon.Id == AccountReconId);

        //    if (AccountRecons == null || flinsAccount==null) return;


        //    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Selected for the User : {userId} in EamNovaHelper.processNovaUser - AccountType :  {actionTypestring}");
        //    AccountRecons.ForEach(a => {
        //        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"AccountReconciliationId : {a.AccountRecon.Id}, EndpointSystemId:{a.AccountRecon.EndpointSystemId}, AccountName :{a.AccountRecon.AccountName}, EndpointUserId:{a.AccountRecon.EndpointUserId}");

        //    });




        //    List<NovaMatrixResponseDetail> selected;
        //    var novaUser = new SAPEamNovaUserInfo();
        //    novaUser.EndPointSystemId = NovaEndpoint.Id;
        //    novaUser.UserId = userId ?? 0;

        //    string flinsAcc = null, flinsBiAcc = null, adAcc = null;
        //    long flinsAccId = 0, flinsBiAccId = 0, adAccId = 0;

        //    //FLINS

        //    selected = getflins(AccountRecons, ref flinsAcc, ref flinsAccId);

        //    if (selected == null || flinsAcc == null) return;
        //    novaUser.FLINS_UserLogonName = flinsAcc;
        //    novaUser.FLINS_SAPUserInfoId = flinsAccId;

        //    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Selected Flins Acccount  : {flinsAccId} -  Account Name : {flinsAcc} in EamNovaHelper.processNovaUser");

        //    printSelectedMatrix(selected, "FLINS");

        //    if (selected.Any(c => c.FlinsBiTCode.Count() == 0 && c.ADGroup.Count() == 0))
        //    {
        //        List<NovaMatrixResponseDetail> chosen = new List<NovaMatrixResponseDetail>();
        //        selected.Where(c => c.FlinsBiTCode.Count() == 0 && c.ADGroup.Count() == 0).Select(c=>c).Distinct(new NovaUserRoleMatrixComparer()).ToList().ForEach(detail =>
        //        {
        //           if (novaUser.SAPEamNovaUserRoles == null)
        //                novaUser.SAPEamNovaUserRoles = new List<SAPEamNovaUserRoleInfo>();

        //            if (!novaUser.SAPEamNovaUserRoles.Any(d => d.RoleName == detail.Position.Trim()))
        //            {
        //                novaUser.SAPEamNovaUserRoles.Add(
        //                    new SAPEamNovaUserRoleInfo()
        //                    {
        //                        EndPointSystemId = NovaEndpoint.Id,
        //                        IsActive = true,
        //                        RoleName = detail.Position.Trim(),
        //                        FLINS_TCodes = string.Join(",", detail.FlinsTCode),
        //                        Company = string.IsNullOrWhiteSpace(detail.Company) ? null : detail.Company.Trim(),
        //                        Section = string.IsNullOrWhiteSpace(detail.Section) ? null : detail.Section.Trim(),
        //                        DepartmentBranch = string.IsNullOrWhiteSpace(detail.Department) ? null : detail.Department.Trim(),
        //                        NovaPositionMatrixType = detail.novaMatrixType
        //                    });

        //                detail.FlinsTCode.ForEach(s =>
        //                {
        //                    if (flinsRoles.Contains(s))
        //                        flinsRoles.Add(s);
        //                });

        //                chosen.Add(detail);
        //            }
        //        });
        //        if (chosen.Count > 0)
        //        {
        //            AddOrUpdateUser(AccountReconId, novaUser);
        //            printSelectedMatrix(chosen, "FLINS Only");
        //            foreach (var r in chosen)
        //                selected.Remove(r);
        //        }
        //    }


        //    // FLINS BI
        //    var biselected = getflinsBi(AccountRecons, ref flinsBiAcc, ref flinsBiAccId, selected);

        //    if (biselected == null)
        //    {
        //        return;
        //    }
        //        novaUser.FLINS_BI_UserLogonName = flinsBiAcc;
        //        novaUser.FLINS_BI_SAPUserInfoId = flinsBiAccId;

        //        _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Selected Flins Bi Acccount  : {flinsBiAccId} -  Account Name : {flinsBiAcc} in EamNovaHelper.processNovaUser");

        //    printSelectedMatrix(biselected, "FLINS Bi");
        //    if (biselected.Count() > 0 && biselected.Any(c => c.ADGroup.Count() == 0))
        //    {
        //        List<NovaMatrixResponseDetail> chosen = new List<NovaMatrixResponseDetail>();

        //        biselected.Where(c => c.ADGroup.Count() == 0).Select(c => c).Distinct(new NovaUserRoleMatrixComparer()).ToList().ForEach(detail =>
        //            {
        //                if (novaUser.SAPEamNovaUserRoles == null)
        //                    novaUser.SAPEamNovaUserRoles = new List<SAPEamNovaUserRoleInfo>();

        //                if (!novaUser.SAPEamNovaUserRoles.Any(d => d.RoleName == detail.Position.Trim()))
        //                {
        //                    novaUser.SAPEamNovaUserRoles.Add(
        //                      new SAPEamNovaUserRoleInfo()
        //                      {
        //                          EndPointSystemId = NovaEndpoint.Id,
        //                          IsActive = true,
        //                          RoleName = detail.Position.Trim(),
        //                          FLINS_TCodes = string.Join(",", detail.FlinsTCode),
        //                          FLINS_BI_TCodes = string.Join(",", detail.FlinsBiTCode),
        //                          Company = string.IsNullOrWhiteSpace(detail.Company) ? null : detail.Company.Trim(),
        //                          Section = string.IsNullOrWhiteSpace(detail.Section) ? null : detail.Section.Trim(),
        //                          DepartmentBranch = string.IsNullOrWhiteSpace(detail.Department) ? null : detail.Department.Trim(),
        //                          NovaPositionMatrixType = detail.novaMatrixType
        //                      });

        //                    detail.FlinsTCode.ForEach(s =>
        //                    {
        //                        if (flinsRoles.Contains(s))
        //                            flinsRoles.Add(s);
        //                    });

        //                    detail.FlinsBiTCode.ForEach(s =>
        //                    {
        //                        if (flinsBiRoles.Contains(s))
        //                            flinsBiRoles.Add(s);
        //                    });


        //                    chosen.Add(detail);
        //                }

        //            });
        //        if (chosen.Count > 0)
        //        {
        //            AddOrUpdateUser(AccountReconId, novaUser);
        //            printSelectedMatrix(chosen, "FLINS Bi Only");
        //            foreach (var r in chosen)
        //                biselected.Remove(r);
        //        }
        //    }


        //    // AD

        //    var adselected = getAds(AccountRecons, ref adAcc, ref adAccId, biselected);

        //    if (adselected == null)
        //    {
        //        return;
        //    }
        //    novaUser.AD_UserLogonName = adAcc;
        //    novaUser.ADS_UserInfoId = adAccId;

        //    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Selected AD Acccount  : {adAccId} -  Account Name : {adAcc} in EamNovaHelper.processNovaUser");

        //    printSelectedMatrix(adselected, "Ad");
        //    if (adselected.Count() > 0)
        //    {
        //        adselected.Distinct(new NovaUserRoleMatrixComparer()).ToList().ForEach(detail =>
        //        {
        //            if (novaUser.SAPEamNovaUserRoles == null)
        //                novaUser.SAPEamNovaUserRoles = new List<SAPEamNovaUserRoleInfo>();

        //            if (!novaUser.SAPEamNovaUserRoles.Any(d => d.RoleName == detail.Position.Trim()))
        //            {
        //                novaUser.SAPEamNovaUserRoles.Add(
        //                new SAPEamNovaUserRoleInfo()
        //                {
        //                    EndPointSystemId = NovaEndpoint.Id,
        //                    IsActive = true,
        //                    RoleName = detail.Position.Trim(),
        //                    FLINS_TCodes = string.Join(",", detail.FlinsTCode),
        //                    FLINS_BI_TCodes = string.Join(",", detail.FlinsBiTCode),
        //                    AD_Groups = string.Join(",", detail.ADGroup),
        //                    Company = !string.IsNullOrWhiteSpace(detail.Company) ? detail.Company.Trim() : detail.Company,
        //                    Section = !string.IsNullOrWhiteSpace(detail.Section) ? detail.Section.Trim() : detail.Section,
        //                    DepartmentBranch = !string.IsNullOrWhiteSpace(detail.Department) ? detail.Department.Trim() : detail.Department,
        //                    NovaPositionMatrixType = detail.novaMatrixType
        //                });

        //                detail.FlinsTCode.ForEach(s =>
        //                {
        //                    if (flinsRoles.Contains(s))
        //                        flinsRoles.Add(s);
        //                });

        //                detail.FlinsBiTCode.ForEach(s =>
        //                {
        //                    if (flinsBiRoles.Contains(s))
        //                        flinsBiRoles.Add(s);
        //                });

        //                detail.ADGroup.ForEach(s =>
        //                {
        //                    if (adGroups.Contains(s))
        //                        adGroups.Add(s);
        //                }); 

        //            }
        //        });

        //        AddOrUpdateUser(AccountReconId, novaUser);
        //        return;
        //    }

        //    _idmsLogger.WriteLog(LogLevelType.DebugStep, $"End Process for the User : {userId} in EamNovaHelper.processNovaUser - {actionTypestring}");
        //}

        #endregion Commented
        bool IsExist(List<string> matrixGroups, List<string> currentEntitlements)
        {
            if (matrixGroups==null || matrixGroups.Count()==0)
                return true;

            if (currentEntitlements == null)
                currentEntitlements = new List<string>();

            

            if (matrixGroups.Count() > 0 && currentEntitlements.Count == 0)
                return false;

            
            foreach (var grp in matrixGroups)
            {
                if (currentEntitlements.IndexOf(grp.Trim()) == -1)
                    return false;
            }
            return true;
        }

        List<NovaMatrixResponseDetail> getflins(List<AccountEntitlementRecon> AccountRecons, ref string accountName, ref long EndPointUserId)
        {
            List<NovaMatrixResponseDetail> selected = null;
            var _novaMatrixResponse = NovaMatrixResponseData;
            var flins = AccountRecons.FirstOrDefault(acc => acc.AccountRecon.EndpointSystemId == FlinsEndpoint.Id && !acc.AccountRecon.IsDeleted && !acc.AccountRecon.IsLocked);

            if (flins != null && flins.AccountRecon != null)
            {
                List<string> entitlements = flins.Entitlement.Where(e => !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Entitlement)).Select(e => e.Entitlement.Trim()).ToList();

                selected = _novaMatrixResponse.NovaMatrixResponseDetails.Where(n =>
                {
                    return n.FlinsTCode.All(a => entitlements.Contains(a.Trim())) || n.FlinsTCode.Count() == 0;

                }
                ).Select(n => n).ToList();
            }

            if (selected != null && selected.Count > 0)
            {
                EndPointUserId = flins.AccountRecon.EndpointUserId;
                accountName = flins.AccountRecon.AccountName;
                return selected;
            }



            return new List<NovaMatrixResponseDetail>();
        }

        List<NovaMatrixResponseDetail> getflinsBi(List<AccountEntitlementRecon> AccountRecons, ref string accountName, ref long EndPointUserId, List<NovaMatrixResponseDetail> NovaMatrixResponseDetails)
        {
            List<NovaMatrixResponseDetail> selected = null;
            var flinsBi = AccountRecons.FirstOrDefault(acc => acc.AccountRecon.EndpointSystemId == FlinsBiEndpoint.Id && !acc.AccountRecon.IsDeleted && !acc.AccountRecon.IsLocked);
            if (flinsBi == null)
            {
                EndPointUserId = 0;
                accountName = string.Empty;
                return NovaMatrixResponseDetails;
            }
            if (flinsBi != null && flinsBi.AccountRecon != null)
            {
                
                List<string> entitlements = flinsBi.Entitlement.Where(e => !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Entitlement)).Select(e => e.Entitlement.Trim()).ToList();

                selected = NovaMatrixResponseDetails.Where(n =>
                {
                    return n.FlinsBiTCode!=null && n.FlinsBiTCode.Count()>0 &&  n.FlinsBiTCode.All(a => entitlements.Contains(a.Trim())) ;

                }).Select(n => n).ToList();

            }

            if (selected != null && selected.Count > 0)
            {
                EndPointUserId = flinsBi.AccountRecon.EndpointUserId;
                accountName = flinsBi.AccountRecon.AccountName;

                return NovaMatrixResponseDetails.Where(m => m.FlinsBiTCode == null || m.FlinsBiTCode.Count() > 0).Union(selected).ToList();
            }
            else
            {
                return NovaMatrixResponseDetails.Where(b=> b.FlinsBiTCode==null || b.FlinsBiTCode.Count()>0).ToList();
            }
        }

        List<NovaMatrixResponseDetail> getAds(List<AccountEntitlementRecon> AccountRecons, ref string accountName, ref long EndPointUserId, List<NovaMatrixResponseDetail> NovaMatrixResponseDetails)
        {
            List<NovaMatrixResponseDetail> selected = null;
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Inside getAds");

            var ad = AccountRecons.FirstOrDefault(acc => acc.AccountRecon.EndpointSystemId == ADEndpoint.Id && !acc.AccountRecon.IsDeleted && !acc.AccountRecon.IsLocked);
            if (ad == null)
            {
                EndPointUserId = 0;
                accountName = string.Empty;
                return NovaMatrixResponseDetails;
            }

            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Inside getAds - Ad account selected");
            _idmsLogger.WriteLog(LogLevelType.DebugStep, $"Inside getAds - Ad accountrecon is null ;{ad.AccountRecon==null}");
            EndPointUserId = ad.AccountRecon.EndpointUserId;
            accountName = ad.AccountRecon.AccountName;

            if (ad != null && ad.AccountRecon != null)
            {
                List<string> entitlements = ad.Entitlement.Where(e => !e.IsDeleted && !string.IsNullOrWhiteSpace(e.Entitlement)).Select(e => e.Entitlement.Trim()).ToList();

                selected = NovaMatrixResponseDetails.Where(n => n.ADGroup.All(a => entitlements.Contains(a.Trim())) || n.ADGroup.Count() == 0).Select(n => n).ToList();
            }

            if (selected != null && selected.Count > 0)
            {

                return selected;
            }


            return NovaMatrixResponseDetails;
        }

        public void UpdateEntitlementRecon(Dictionary<long, string> dicUsers)
        {
            List<AccountReconcilation> exsitngAccountrecon = null;
            List<EntitlementReconcilation> existingEntilements = null;
            try
            {
                exsitngAccountrecon = AccountReconcilation.GetAllByEndpointId(NovaEndpoint.Id).ToList();
                existingEntilements = EntitlementReconcilation.GetAllByEndpointSystemId(NovaEndpoint.Id);
                AccountReconcilation acctRecon = null;
                EntitlementReconcilation entRecon = null;
                SAPEamNovaUserInfo novaUserInfo = null;
                JavaScriptSerializer _serializer = new JavaScriptSerializer();
                foreach (KeyValuePair<long, string> item in dicUsers)
                {
                    novaUserInfo = null;
                    try
                    {
                        novaUserInfo = SAPEamNovaUserInfo.GetById(item.Key, true);


                        if (novaUserInfo != null && novaUserInfo.SAPEamNovaUserRoles != null)
                        {
                            acctRecon = exsitngAccountrecon.Where(a => a.EndpointUserId == novaUserInfo.Id && a.AccountName == novaUserInfo.FLINS_UserLogonName).FirstOrDefault();
                            if (acctRecon == null) continue;

                            foreach (SAPEamNovaUserRoleInfo entitlement in novaUserInfo.SAPEamNovaUserRoles)
                            {
                                var desc = entitlement.EntitlementName.Trim();

                                if (entitlement.NovaPositionMatrixType == NovaMatrixType.Exceptional)
                                {
                                    var matrix = _novaMatrixResponse.NovaMatrixResponseDetails.FirstOrDefault(m =>
                                                        m.novaMatrixType == NovaMatrixType.Exceptional &&
                                                        m.Position.Trim().Equals(entitlement.EntitlementName.Trim(), StringComparison.InvariantCultureIgnoreCase)
                                                        );
                                    if (matrix != null &&  !string.IsNullOrWhiteSpace(matrix.JobDescription))
                                    {
                                        desc = matrix.JobDescription.Trim();
                                    }
                                }


                                entRecon = existingEntilements.Where(e => e.AccountReconcilationId == acctRecon.Id &&
                                                                            e.Entitlement.Trim().Equals(entitlement.RoleName.Trim(), StringComparison.InvariantCultureIgnoreCase) && 
                                                                            e.EntitlementDescription.Trim().Equals(desc, StringComparison.InvariantCultureIgnoreCase) &&
                                                                            e.EntitlementType == (int)EntitlementType.Role).FirstOrDefault();

                                if (entRecon == null)
                                {
                                    entRecon = new EntitlementReconcilation(); 
                                    entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role , string.Empty);
                                    entRecon.EntitlementDescription = desc; 
                                    entRecon.Entitlement = entitlement.RoleName.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.UserId = novaUserInfo.UserId;                                     
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                    entRecon.CreatedDate = DateTime.Now;
                                    entRecon.CreatedBy = "System";
                                    entRecon.Functions = _serializer.Serialize(entitlement);
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.Save(_currentuser);
                                    existingEntilements.Add(entRecon);
                                }
                                else
                                {
                                    entRecon.ValidToDate = acctRecon.ValidToDate;
                                    entRecon.EndpointUserId = novaUserInfo.Id;
                                    entRecon.LastModifiedDate = DateTime.Now;
                                    entRecon.LastModifiedBy = "System";
                                    entRecon.Entitlement = entitlement.RoleName.Trim();
                                    entRecon.EntitlementId = entitlement.Id;
                                    entRecon.EntitlementDescription = desc; 
                                    entRecon.UserId = novaUserInfo.UserId;
                                    entRecon.Functions = _serializer.Serialize(entitlement);
                                    if (entRecon.IsDeleted)
                                    {
                                        EntitlementReconcilation.UpdateException(entRecon, true);
                                    }
                                    entRecon.Save(_currentuser);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. EAM NOVA User -{0}", novaUserInfo.DisplayName), ex);

                    }
                }

            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Error while Process Entitlements Recon in NOVA Connector.", ex);
            }
            finally
            {
                exsitngAccountrecon?.Clear();
                exsitngAccountrecon = null;

                existingEntilements?.Clear();
                existingEntilements = null;
            }
            try
            {
                EntitlementReconcilation.ProcessDeletedEntitlements(NovaEndpoint.Id, _process, _currentuser);
            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Error while ProcessDelete Entitlements Recon in NOVA Connector.", ex);
            }
           
        }




        internal class NovaUserRoleMatrixComparer : IEqualityComparer<NovaMatrixResponseDetail>
        {
            public bool Equals(NovaMatrixResponseDetail x, NovaMatrixResponseDetail y)
            {
                if (string.Equals(x.Position, y.Position, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Company , y.Company,StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Section,y.Section,StringComparison.OrdinalIgnoreCase) && string.Equals(x.Department,y.Department,StringComparison.OrdinalIgnoreCase)
                    && x.novaMatrixType == y.novaMatrixType
                    )
                {
                    return true;
                }
                return false;
            }

            public int GetHashCode(NovaMatrixResponseDetail obj)
            {
                return obj.GetHashCode();
            }
        }


        public  EntitlementReconcilation CreateEntitlementReconcilationShell(AccountReconcilation acctRecon, EntitlementType _entitlementType, string Entitlements = "")
        {
            return new EntitlementReconcilation()
            {
                IsSODConflict = false,
                IsSensitiveAccess = false,
                AccountReconcilationId = acctRecon.Id,
                EndpointSystemId = acctRecon.EndpointSystemId,
                EndpointUserId = acctRecon.EndpointUserId,
                AccountName = acctRecon.AccountName,
                UserId = acctRecon.UserId,
                EnteredDate = DateTime.Now,
                IsDeleted = false,
                DeletedDate = null,
                EntitlementType = (int)_entitlementType,
                EndpointType = (EndpointType)this.NovaEndpoint.EndpointSystemTypeId,
            };
        }
        public void UpdateEntitlementRecon(SAPEamNovaUserInfo novaUserInfo)
        {
            try
            {
                AccountReconcilation acctRecon = AccountReconcilation.GetByEndpointUserId(novaUserInfo.Id, this.NovaEndpoint.Id);


                EntitlementReconcilation entRecon = null;


                try
                {
                    if (acctRecon != null && novaUserInfo != null && novaUserInfo.SAPEamNovaUserRoles != null)
                    {
                        List<EntitlementReconcilation> existingEntilements = EntitlementReconcilation.GetByAccountReconcilationId(acctRecon.Id, this.NovaEndpoint.Id);
                        foreach (SAPEamNovaUserRoleInfo entitlement in novaUserInfo.SAPEamNovaUserRoles)
                        {


                            if (acctRecon == null) continue;

                            entRecon = existingEntilements.Where(e => e.EndpointUserId == novaUserInfo.Id &&
                                                                        e.Entitlement == Convert.ToString(entitlement.RoleName).Trim() &&
                                                                        e.EntitlementType == (int)EntitlementType.Role).FirstOrDefault();

                            if (entRecon == null)
                            {
                                entRecon = new EntitlementReconcilation();
                                var entitlements = (novaUserInfo.Entitlements.Select(x => x.EntitlementName.ToString()));
                                entRecon = this.CreateEntitlementReconcilationShell(acctRecon, EntitlementType.Role, string.Join(",", entitlements != null ? entitlements.ToArray() : new string[] { }));
                                entRecon.Entitlement = entitlement.RoleName.Trim();
                                entRecon.EntitlementId = entitlement.Id;
                                entRecon.EntitlementDescription = novaUserInfo.DisplayName;
                                EntitlementReconcilation.UpdateException(entRecon, true);
                                entRecon.CreatedDate = DateTime.Now;
                                entRecon.CreatedBy = "System";
                                entRecon.UserId = novaUserInfo.UserId;
                                entRecon.Save(_currentuser);
                                existingEntilements.Add(entRecon);
                            }
                            else
                            {
                                entRecon.LastModifiedDate = DateTime.Now;
                                entRecon.LastModifiedBy = "System";
                                entRecon.Entitlement = entitlement.RoleName.Trim();
                                entRecon.EntitlementId = entitlement.Id;
                                entRecon.EntitlementDescription = novaUserInfo.DisplayName;
                                entRecon.UserId = novaUserInfo.UserId;

                                if (entRecon.IsDeleted)
                                {
                                    EntitlementReconcilation.UpdateException(entRecon, true);
                                }
                                entRecon.Save(_currentuser);
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    _idmsLogger.WriteLog(LogLevelType.Exception, string.Format("Error while updating Entitlement reconciliation. EAM NOVA User -{0}", novaUserInfo.DisplayName), ex);

                }


            }
            catch (Exception ex)
            {
                _idmsLogger.WriteLog(LogLevelType.Exception, "Error while Process Entitlements Recon in NOVA Connector.", ex);
            }
        }

    }
}
