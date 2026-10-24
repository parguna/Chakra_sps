using DI.Chakra.DomainModel;
using DI.Chakra.Infrastructure;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Logger;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace DI.Chakra.Connector
{
    public class SF_Helper
    {
        public static string DateFormat = "s";
        public static int EmployeeIdMaxSize = 8;
        public static int OrgCode_MaxSize = 8;
        public static List<EmployeeEvents> CanReUseEmployeeEventIfExist = new List<EmployeeEvents> { EmployeeEvents.NewJoinee, EmployeeEvents.Rehire, EmployeeEvents.Resignation };
        public static string Config(EndpointKey Key, EndpointConfiguration endpointConfig)
        {
            var config = endpointConfig.ConfigurationDetails.FirstOrDefault(lq => lq.KeyId == (long)Key);
            return config != null ? config.Value : "";
        }

        public static NOrganisation GetOrganisation(List<NOrganisation> organisations, string orgCode)
        {
            return organisations.FirstOrDefault(c => c.Code == orgCode);
        }


        public static List<string> getPagedData<T>(IQueryable<T> query, 
                                                   Expression<Func<T, bool>> condition,  
                                                   Expression<Func<T, T>> selector, 
                                                   Expression<Func<T, string>> selectedcolumn,
                                                   List<string> completeList, 
                                                   IdmsLogger idmsLogger) where T : class
        {
            Type type = typeof(T);
            long totalCount = 0;
            int pageSize = 1000;
            int page = 0;
            List<T> list;
            try
            {
                query = query.Where(condition);
                totalCount = query.Select(selector).ToList().Count();
                while (pageSize * page < totalCount)
                {
                   list  = query.Skip(pageSize * page)
                                     .Take(pageSize)
                                     .Select(selector).ToList();
                     
                    completeList.AddUniqueRange(list.AsQueryable().Select(selectedcolumn).ToList());
                    page++;
                }
            }
            catch (Exception ee)
            {
                idmsLogger.WriteLog(LogLevelType.Error, $"Exception occurred in getPagedData for the type {type.Name}", ee);
            }
            finally
            {
                 idmsLogger.WriteLog(LogLevelType.Info, $"{totalCount} - change set records in {type.Name}. The Query is, [{query}]");

            } 
            return completeList;
        }

        public static void UpdateUserFromDiscovery(User user, List<string> propagationFields, bool canRunPropagationTrigger, string serviceUser, IdmsLogger idmsLogger, bool canSaveUser = true ,SFOData.EmpJob empJob=null)
        {
            try
            {
                InitiatePropagateTrigger(user, propagationFields, canRunPropagationTrigger, serviceUser, idmsLogger,empJob);
                if (canSaveUser)
                {
                    user.Save(serviceUser);
                    User.DeriveCOEmployeeId(user.Id);
                }
            }
            catch
            {
                throw;
            }
        }

        static void InitiatePropagateTrigger(User user, List<string> propagationFields, bool canRunPropagationTrigger, string currentUser, IdmsLogger idmsLogger, SFOData.EmpJob empJob)
        {
            try
            {
                var columnNames = new List<string>();

                var existingUser = User.GetById(user.Id);

                if (existingUser == null)
                {
                    idmsLogger.WriteLog(LogLevelType.Info, $"Propagation Event Creation: Could not find DomainModel.User for the PERNR : {user.EmployeeId}."); 
                    return;
                }
                bool isPropagationRequired = false;

                foreach (string attribute in propagationFields)
                {
                    PropertyInfo trsuserpropertyInfo = user.GetType().GetProperty(attribute);

                    string newValue = string.Empty;
                    string dt = string.Empty;

                    if (trsuserpropertyInfo != null && trsuserpropertyInfo.GetValue(user) != null)
                        newValue = trsuserpropertyInfo.GetValue(user).ToString();

                    PropertyInfo userpropertyInfo = existingUser.GetType().GetProperty(attribute);

                    string oldValue = string.Empty;

                    if (userpropertyInfo != null && userpropertyInfo.GetValue(existingUser) != null)
                        oldValue = userpropertyInfo.GetValue(existingUser).ToString();

                    if (string.Compare(oldValue, newValue, true) != 0)
                    {
                        Type userPropType = userpropertyInfo.PropertyType;
                        isPropagationRequired = true;
                        columnNames.Add(attribute);
                        continue;
                    }
                }

                if ((isPropagationRequired || empJob!=null ) && canRunPropagationTrigger )
                {
                    var columns = string.Join(",", columnNames);
                    if (!string.IsNullOrWhiteSpace(columns))
                        idmsLogger.WriteLog(LogLevelType.Info, $"Propagation Event Created for the PERNR : {user.EmployeeId}. The changes are in,{columns}");
                    if (empJob != null && existingUser.JoinDate < empJob.startDate )
                    {
                        var reason = $"Event Code:[{(empJob.eventNav != null ? empJob.eventNav.externalCode : string.Empty) }] and Reason Code:[{(empJob.eventReason)}] on the effective date :[{empJob.startDate}] ";
                        try
                        {
                            var existingTrigger = EmployeeEventTrigger.GetOldByEmployeeIdAndEvent(user.EmployeeId, EmployeeEvents.Propagation, empJob.startDate);
                            if(existingTrigger != null)
                            {
                                if (existingTrigger.IsRequestCreated)
                                {
                                    idmsLogger.WriteLog(LogLevelType.Info, $"Old Propagation Event cannot be activated for PERNR : {user.EmployeeId} Date:{empJob.startDate} SF Reason:{reason}.");
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            idmsLogger.WriteLog(LogLevelType.Info, $"EmployeeId :{user.EmployeeId} Propagation Event check failed. Error{ex.ToString()}");
                        }
                        EmployeeEventTrigger.CreatePropagationTrigger(user, currentUser, columns, empJob.startDate, reason);
                    }
                    else if((existingUser.Reserved4== DI.Chakra.Infrastructure.Constants.EmployeeStatus.OnBoarded || existingUser.Reserved4 == DI.Chakra.Infrastructure.Constants.EmployeeStatus.NewEmployee) || existingUser.JoinDate >= DateTime.Today)
                    {
                        EmployeeEventTrigger.CreatePropagationTrigger(user, currentUser, columns);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static DataTable createDataTable(string name, Dictionary<string, System.Type> keyValuePairs)
        {
            DataTable dt = new DataTable(name);

            foreach (string key in keyValuePairs.Keys)
            {
                dt.Columns.Add(new DataColumn(key, keyValuePairs[key]));
            }
            return dt;
        }


        public static DataTable getAddressDetails(SFOData.EntityContainer entityContainer, string details, string PERNR, string officeAddressType)
        {
            var addresses = entityContainer.PerAddressDEFLT
                                  .Where(e => e.addressType == officeAddressType
                                  && e.personIdExternal == PERNR
                                   )
                                  .Select(e =>
                                         new SFOData.PerAddressDEFLT()
                                         {
                                             address1 = e.address1,
                                             address2 = e.address2,
                                             address3 = e.address3,
                                             address4 = e.address4,
                                             address5 = e.address5,
                                             zipCode = e.zipCode,
                                             country = e.country,
                                             countryNav = new SFOData.Territory()
                                             {
                                                 territoryCode = e.countryNav.territoryCode,
                                                 territoryName = e.countryNav.territoryName
                                             },
                                             city = e.city,
                                             addressType = e.addressType,
                                             startDate = e.startDate,
                                             endDate = e.endDate
                                         }).ToList();

            var dt = createDataTable(details, new Dictionary<string, Type> {
                                                                   { "address1", typeof(string) },
                                                                   { "address2", typeof(string) },
                                                                   { "address3", typeof(string) },
                                                                   { "address4", typeof(string) },
                                                                   { "address5", typeof(string) },
                                                                   { "zipCode", typeof(string) },
                                                                   { "country", typeof(string) },
                                                                   { "city", typeof(string) },
                                                                   { "addressType", typeof(string) },
                                                                   { "startDate", typeof(string) },
                                                                   { "endDate", typeof(string) }
                                                             });
            if (addresses != null && addresses.Count > 0)
            {
                foreach (var address in addresses)
                {
                    var dr = dt.NewRow();
                    dr["address1"] = address.address1;
                    dr["address2"] = address.address2;
                    dr["address3"] = address.address3;
                    dr["address4"] = address.address4;
                    dr["address5"] = address.address5;

                    dr["zipCode"] = address.zipCode;
                    dr["country"] = address.countryNav != null ? address.countryNav.territoryName : address.country;
                    dr["city"] = address.city;
                    dr["addressType"] = address.addressType;
                    dr["startDate"] = address.address5;
                    if (address.endDate.HasValue)
                        dr["endDate"] = address.endDate;
                    dt.Rows.Add(dr);
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        public static DataTable getPhoneDetails(SFOData.EntityContainer entityContainer, string details, string PERNR)
        {
            var phones = entityContainer.PerPhone.Where(p => p.personIdExternal == PERNR)
                                          .Select(p => new SFOData.PerPhone() { isPrimary = p.isPrimary, phoneType = p.phoneType, phoneNumber = p.phoneNumber }).ToList();

            var dt = createDataTable(details, new Dictionary<string, Type> {
                                                                   { "isPrimary", typeof(string) },
                                                                   { "phoneType", typeof(string) },
                                                                   { "phoneNumber", typeof(string) }
                                                             });
            if (phones != null && phones.Count > 0)
            {
                foreach (var phone in phones)
                {
                    var dr = dt.NewRow();
                    dr["isPrimary"] = phone.isPrimary.HasValue ? phone.isPrimary.Value.ToString() : null;
                    dr["phoneType"] = phone.phoneType;
                    dr["phoneNumber"] = phone.phoneNumber;
                    dt.Rows.Add(dr);
                }
            }
            dt.AcceptChanges();
            return dt;
        }

        public static DataTable getEmailTable(SFOData.EntityContainer entityContainer, string details, string PERNR)
        {
            var emails = entityContainer.PerEmail
                         .Where(e => e.personIdExternal == PERNR)
                         .Select(e =>
                             new SFOData.PerEmail()
                             {
                                 personIdExternal = e.personIdExternal,
                                 emailAddress = e.emailAddress,
                                 isPrimary = e.isPrimary
                             }).ToList();

            var dt = createDataTable(details, new Dictionary<string, Type> {
                                                                   { "isPrimary", typeof(string) },
                                                                   { "emailAddress", typeof(string) }
                                                             });
            if (emails != null && emails.Count > 0)
            {
                foreach (var email in emails)
                {
                    var dr = dt.NewRow();
                    dr["isPrimary"] = email.isPrimary.HasValue ? email.isPrimary.Value.ToString() : null;
                    dr["emailAddress"] = email.emailAddress;
                    dt.Rows.Add(dr);
                }
            }
            dt.AcceptChanges();
            return dt;

        }

        public static DataTable getPersonalInfo(SFOData.EntityContainer entityContainer, string details, string PERNR)
        {

            var infos = entityContainer.PerPersonal.Where(p => p.personIdExternal == PERNR
                          ).OrderByDescending(p => p.startDate)
                                .Select(p =>
                                         new SFOData.PerPersonal()
                                         {
                                             personIdExternal = p.personIdExternal,
                                             startDate = p.startDate,
                                             endDate = p.endDate,
                                             lastName = p.lastName,
                                             firstName = p.firstName, 
                                             preferredName = p.preferredName,
                                             formalName = p.formalName,
                                             ethnicGroup = p.ethnicGroup,
                                             lastModifiedOn = p.lastModifiedOn
                                         }).ToList();


            var dt = createDataTable(details, new Dictionary<string, Type> {
                                                                   { "startDate", typeof(string) },
                                                                   { "endDate", typeof(string)  },
                                                                   { "lastName" ,  typeof(string) },
                                                                   {"firstName", typeof(string) },
                                                                   { "formalName" ,typeof(string) },
                                                                   { "ethnicGroup" ,typeof(string)},
                                                                   { "lastModifiedOn",typeof(string)}, 
                                                                   {"preferredName", typeof(string)} 
            });

            if (infos != null && infos.Count > 0)
            {
                foreach (var info in infos)
                {
                    var dr = dt.NewRow();
                    dr["startDate"] = info.startDate;
                    dr["endDate"] = info.endDate.HasValue ? info.endDate : null;
                    dr["lastName"] = info.lastName;
                    dr["firstName"] = info.firstName;
                    dr["formalName"] = info.formalName;
                    dr["ethnicGroup"] = info.ethnicGroup;
                    dr["preferredName"] = info.preferredName;
                    dr["lastModifiedOn"] = info.lastModifiedOn.HasValue ? info.lastModifiedOn : null;
                    dt.Rows.Add(dr);
                }
            }
            dt.AcceptChanges();
            return dt;
        }
        public static DataTable getEmploymentInfo(SFOData.EntityContainer entityContainer, string details, string PERNR)
        {
            var dt = createDataTable(details, new Dictionary<string, Type> {
                                                                   { "Object Name", typeof(string) },
                                                                   { "startDate", typeof(string)  },
                                                                    { "endDate", typeof(string)  },
                                                                   { "lastModifiedOn",typeof(string)}
            });

            var employments = entityContainer.EmpEmployment.Where(p => p.personIdExternal == PERNR && p.userId == PERNR)
                                                           .OrderByDescending(p => p.lastModifiedOn)
                                                           .Select(p =>
                                                                     new SFOData.EmpEmployment()
                                                                     {
                                                                         personIdExternal = p.personIdExternal,
                                                                         startDate = p.startDate,
                                                                         endDate = p.endDate,
                                                                         lastModifiedOn = p.lastModifiedOn
                                                                     }).ToList();

            if (employments != null && employments.Count > 0)
            {
                foreach (var e in employments)
                {
                    var dr = dt.NewRow();
                    dr["Object Name"] = "EmpEmployment";
                    dr["startDate"] = e.startDate;
                    dr["endDate"] = e.endDate;
                    dr["lastModifiedOn"] = e.lastModifiedOn;
                    dt.Rows.Add(dr);
                }
            }

            dt.AcceptChanges();

            //var terminations = entityContainer.EmpEmploymentTermination
            //                                  .Where(p => p.personIdExternal == PERNR && p.userId == PERNR)
            //                                  .OrderByDescending(p => p.lastModifiedOn)
            //                                  .Select(p => new SFOData.EmpEmploymentTermination()
            //                                  {
            //                                      personIdExternal = p.personIdExternal,
            //                                      endDate = p.endDate,
            //                                      lastModifiedOn = p.lastModifiedOn
            //                                  }).ToList();  

            //if (terminations != null && terminations.Count > 0)
            //{
            //    foreach (var e in terminations)
            //    {
            //        var dr = dt.NewRow();
            //        dr["Object Name"] = "EmpEmploymentTermination";
            //        dr["endDate"] = e.endDate;
            //        dr["lastModifiedOn"] = e.lastModifiedOn;
            //        dt.Rows.Add(dr);
            //    }
            //}

            //dt.AcceptChanges();

            return dt;
        }

        public static DataTable getEmploymentEvents(SFOData.EntityContainer entityContainer, string details, string PERNR)
        {
            var dt = createDataTable(details, new Dictionary<string, Type> {
                                                                    {"seqNumber", typeof(string) },             
                                                                    {"startDate", typeof(string)  },
                                                                    {"endDate", typeof(string)  },
                                                                    {"lastModifiedOn",typeof(string)},
                                                                    {"createdDateTime",typeof(string)},
                                                                    {"position",typeof(string)},
                                                                    {"costCenter",typeof(string)},
                                                                    {"company" ,typeof(string)},
                                                                    {"businessUnit" ,typeof(string)},
                                                                    {"division" ,typeof(string)},
                                                                    {"department" ,typeof(string)},
                                                                    {"customString3" ,typeof(string)},
                                                                    {"eventReason",typeof(string)},
                                                                    {"event" ,typeof(string)},
                                                                    { "employmentType" ,typeof(string)},
                                                                    { "emplStatus",typeof(string)},
                                                                    { "employeeClass",typeof(string)},
                                                                    {"customString4" ,typeof(string)},
                                                                    {"customString5" ,typeof(string)},
                                                                    {"customString6" ,typeof(string)},
                                                                    {"customString7" ,typeof(string)},
                                                                    {"customString8" ,typeof(string)},
                                                                    {"customString9" ,typeof(string)},
                                                                    {"customString2" ,typeof(string)},
                                                                    {"managerId", typeof(string) },
                                                                    {"localJobTitle", typeof(string) }
                                                                       });

            var employments = entityContainer.EmpJob.Where(e => e.userId == PERNR)
                                             .Select(e => new SFOData.EmpJob()
                                             { 
                                                 seqNumber = e.seqNumber,
                                                 createdDateTime = e.createdDateTime,
                                                 lastModifiedOn = e.lastModifiedOn,
                                                 userId = e.userId,
                                                 startDate = e.startDate,
                                                 endDate = e.endDate,
                                                 position = e.position,
                                                 costCenter = e.costCenter,
                                                 company = e.company,
                                                 businessUnit = e.businessUnit,
                                                 division = e.division,
                                                 department = e.department,
                                                 employmentType = e.employmentType,
                                                 employmentTypeNav = new SFOData.PicklistOption() { externalCode = e.employmentTypeNav.externalCode, id = e.employmentTypeNav.id },
                                                 employeeClass = e.employeeClass,
                                                 employeeClassNav = new SFOData.PicklistOption() { externalCode = e.employeeClassNav.externalCode, id = e.employeeClassNav.id },
                                                 emplStatus = e.emplStatus,
                                                 emplStatusNav = new SFOData.PicklistOption() { externalCode = e.emplStatusNav.externalCode, id = e.emplStatusNav.id },
                                                 customString3 = e.customString3,
                                                 eventReason = e.eventReason,
                                                 managerId = e.managerId,
                                                 eventNav = new SFOData.PicklistOption() { id = e.eventNav.id, externalCode = e.eventNav.externalCode },
                                                 customString2 = e.customString2, 
                                                 customString4= e.customString4,
                                                 customString5 = e.customString5,
                                                 customString6 = e.customString6,
                                                 customString7 = e.customString7,
                                                 customString8 = e.customString8,
                                                 customString9 = e.customString9,
                                                 localJobTitle = e.localJobTitle 
                                             }).ToList();


            
            if (employments != null && employments.Count > 0)
            {
                foreach (var e in employments)
                {
                    var dr = dt.NewRow();
                    dr["seqNumber"] = e.seqNumber;
                    dr["startDate"] = e.startDate;
                    dr["endDate"] = e.endDate;
                    dr["lastModifiedOn"] = e.lastModifiedOn;
                    dr["createdDateTime"] = e.createdDateTime;
                    dr["position"] = e.position;
                    dr["costCenter"] = e.costCenter;
                    dr["company"] = e.company;
                    dr["businessUnit"] = e.businessUnit;
                    dr["division"] = e.division;
                    dr["department"] = e.department;
                    dr["customString3"] = e.customString3;
                    dr["eventReason"] = e.eventReason;
                    dr["event"] = e.eventNav != null ? e.eventNav.externalCode : null;
                    dr["emplStatus"] = e.emplStatusNav!=null? e.emplStatusNav.externalCode: null;
                    dr["employmentType"] = e.employmentTypeNav != null ? e.employmentTypeNav.externalCode : null; 
                    dr["employeeClass"] = e.employeeClassNav!=null ? e.employeeClassNav.externalCode : null;
                    dr["managerId"] = e.managerId;
                    dr["customString2"] = e.customString2;
                    dr["customString4"] = e.customString4;
                    dr["customString5"] = e.customString5;
                    dr["customString6"] = e.customString6;
                    dr["customString7"] = e.customString7;
                    dr["customString8"] = e.customString8;
                    dr["customString9"] = e.customString9;
                    dr["localJobTitle"] = e.localJobTitle;

                    dt.Rows.Add(dr);
                }
            }

            dt.AcceptChanges();

            return dt;
        }
       
    }

     
    public static class PagingUtils
    {
        public static IEnumerable<T> Page<T>(this IEnumerable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }
        public static IQueryable<T> Page<T>(this IQueryable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        } 
        public static string PadWithZeros(this string s,int size)
        {
            return s.PadLeft(size, '0');
        }  
        public static string TrimLeadingSeros(this string s )
        {
            return s.TrimStart(new Char[] { '0' });
        }

        public static void ClearPendingChanges(this SFOData.EntityContainer ctx)
        {

            foreach (LinkDescriptor link in ctx.Links)

                if (link.State != EntityStates.Detached)

                    ctx.DetachLink(link.Source, link.SourceProperty, link.Target);



            foreach (EntityDescriptor entity in ctx.Entities)

                if (entity.State != EntityStates.Detached)

                    ctx.Detach(entity.Entity);

        }

        public static List<T> AddUniqueRange<T>(this List<T> currentList, List<T> list) where T : class
        {
            if (currentList == null)
                currentList = new List<T>();

            foreach (var item in list)
            {
                if (!currentList.Contains(item))
                    currentList.Add(item);
            }
            return currentList;
        }

        private static JavaScriptSerializer json;
        private static JavaScriptSerializer JSON { get { return json ?? (json = new JavaScriptSerializer()); } }

        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }


        public static feed ParseXML(this string @this) 
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(feed), "http://www.w3.org/2005/Atom");


                var reader = XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document });
                return serializer.Deserialize(reader) as feed;
            }
            catch (Exception ee)
            {
                return null;
            }
        }

        public static string GetUpsertStatus(this string @this)
        {
            feed feed = @this.ParseXML();
            if (feed != null)
                return feed.GetStatus();
            else
                return string.Empty;
        }
    } 

    public class SFDiscoveryStatus
    {
        public SFDiscoveryStatus()
        {
            SFDiscoveryStatusType = SFDiscoveryStatusType.Success;
        }

        string _message;
        public string Message
        {
            get
            { return _message; }
            set
            {
                _message = value;
                SFDiscoveryStatusType = SFDiscoveryStatusType.Failure;
            }
        }
        public SFDiscoveryStatusType SFDiscoveryStatusType { get; set; }
    }
    public enum SFDiscoveryStatusType
    {
        Failure,
        Success=1
    }
}
