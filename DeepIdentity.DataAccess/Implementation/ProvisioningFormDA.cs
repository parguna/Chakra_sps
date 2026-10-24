using DeepIdentity.Infrastructure;
using DeepIdentity.Infrastructure.Enums;
using DeepIdentity.Model;
using DeepIdentity.Model.SAP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DeepIdentity.DataAccess
{
    public class ProvisioningFormDA
    {
        #region Private Members
        private QueryExecutor executor;
        private IACMDbOperator iacmOperator;
        private ProvisioningForm ProvisioningForm;
        private IACMDBContext oIACMDBContext = new IACMDBContext();
        #endregion

        #region public Constructor(s)

        #endregion

        #region public Methods

        private class entityDetails
        {
            public string entityName;
            public string tableName;
        }

        private class entityAttributeDetails
        {
            public string attributeName;
            public string attributeType;
            public string fullName;
        }

        private IList fetchColumns_COLUMNNAME_FULLNAME(List<entityDetails> listEntityDetails, List<string> notRequiredColumnList)
        {
            if (listEntityDetails == null)
                return null;

            var objContext = ((IObjectContextAdapter)oIACMDBContext).ObjectContext;
            var mdw = objContext.MetadataWorkspace.GetItems(DataSpace.CSpace);
            List<entityAttributeDetails> resList = new List<entityAttributeDetails>();
            foreach (entityDetails aED in listEntityDetails)
            {
                IList<EdmProperty> clmns = mdw.Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                                     .SelectMany(meta => ((EntityType)meta).Properties)
                                     .Where(P => P.DeclaringType.Name == aED.entityName) // || P.Name == aED.tableName)
                                     .ToList();

                foreach (EdmProperty aItem in clmns)
                {
                    entityAttributeDetails resItem = new entityAttributeDetails();
                    resItem.attributeName = aItem.Name;
                    resItem.fullName = aED.tableName + "." + aItem.Name;

                    resList.Add(resItem);
                }
            }

            var details = (from aItem in resList.AsEnumerable()
                           select new
                           {
                               //FULLNAME = aItem.fullName
                               AttributeName = aItem.attributeName
                           }).ToList();

            if (notRequiredColumnList.Count() > 0)
            {
                List<int> removeItemsAt = new List<int>();
                int intIndex = -1;
                foreach (string rClmn in notRequiredColumnList)
                {
                    intIndex = -1;
                    foreach (var rItem in details)
                    {
                        ++intIndex;
                        if (rItem.AttributeName.ToLower() == rClmn.ToLower())
                        {
                            //details.Remove(rItem);
                            removeItemsAt.Add(intIndex);
                        }
                    }
                }
                removeItemsAt.Sort();
                removeItemsAt.Reverse();
                if (removeItemsAt.Count() > 0)
                {
                    foreach (int rItem in removeItemsAt)
                    {
                        details.RemoveAt(rItem);
                    }
                }
            }

            //var orderedList = details.OrderBy(col => col.COLUMNNAME).ToList();
            var orderedList = details.Select(col => col.AttributeName).ToList();
            return orderedList;
        }



        private bool AddProvisioningFormGeneralData(List<ProvisioningFormGeneralData> formGeneralDataList, long _provisioningFormId, ProvisioningForm _ProvisioningForm, List<ProvisioningFormGeneralData> lstGenData)
        {
            //string query = "DI_spUpdateProvisioningFormGeneralData";
            bool isdataSaved = true;

            //  int _provisioningFormId = formGeneralDataList != null && formGeneralDataList.Count > 0 ? formGeneralDataList[0].ProvisioningFormId : -1;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    foreach (ProvisioningFormGeneralData formGeneralData in formGeneralDataList)
                    {
                        IList<ProvisioningFormGeneralData> editPFGen = new List<ProvisioningFormGeneralData>();
                        if (formGeneralData.IsGroup == false)
                        {
                            editPFGen = oIACMDBContext.ProvisioningFormGeneralDatas.Where(pfgd => pfgd.AttributeName == formGeneralData.AttributeName && pfgd.ProvisioningFormId == _provisioningFormId).ToList();
                            if (editPFGen.Count > 0)
                            {
                                for (int i = 0; i < editPFGen.Count; i++)
                                {
                                    editPFGen[i].ProvisioningFormId = _provisioningFormId;
                                    editPFGen[i].AttributeName = formGeneralData.AttributeName;
                                    editPFGen[i].AttributeColumnName = formGeneralData.AttributeColumnName;
                                    editPFGen[i].IsGroup = formGeneralData.IsGroup;
                                    editPFGen[i].Remarks = formGeneralData.Remarks;
                                    editPFGen[i].LastModifiedBy = _ProvisioningForm.LastModifiedBy;
                                    editPFGen[i].LastModifiedDate = _ProvisioningForm.LastModifiedDate;
                                }
                                oIACMDBContext.SaveChanges();
                            }
                            else
                            {
                                formGeneralData.ProvisioningFormId = _provisioningFormId; //
                                formGeneralData.CreatedBy = _ProvisioningForm.CreatedBy;
                                formGeneralData.CreatedDate = _ProvisioningForm.CreatedDate;
                                formGeneralData.LastModifiedBy = _ProvisioningForm.LastModifiedBy;
                                formGeneralData.LastModifiedDate = _ProvisioningForm.LastModifiedDate;
                                oIACMDBContext.ProvisioningFormGeneralDatas.Add(formGeneralData);
                                oIACMDBContext.SaveChanges();
                            }
                        }
                        else
                        {
                            editPFGen = oIACMDBContext.ProvisioningFormGeneralDatas.Where(pfgd => pfgd.AttributeColumnName == formGeneralData.AttributeColumnName && pfgd.ProvisioningFormId == _provisioningFormId).ToList();
                            if (editPFGen.Count > 0)
                            {
                                for (int i = 0; i < editPFGen.Count; i++)
                                {
                                    editPFGen[i].ProvisioningFormId = _provisioningFormId;
                                    editPFGen[i].AttributeName = formGeneralData.AttributeName;
                                    editPFGen[i].AttributeColumnName = formGeneralData.AttributeColumnName;
                                    editPFGen[i].IsGroup = formGeneralData.IsGroup;
                                    editPFGen[i].Remarks = formGeneralData.Remarks;
                                    editPFGen[i].LastModifiedBy = _ProvisioningForm.LastModifiedBy;
                                    editPFGen[i].LastModifiedDate = _ProvisioningForm.LastModifiedDate;
                                }
                                oIACMDBContext.SaveChanges();
                            }
                            else
                            {
                                formGeneralData.ProvisioningFormId = _provisioningFormId;
                                formGeneralData.CreatedBy = _ProvisioningForm.CreatedBy;
                                formGeneralData.CreatedDate = _ProvisioningForm.CreatedDate;
                                formGeneralData.LastModifiedBy = _ProvisioningForm.LastModifiedBy;
                                formGeneralData.LastModifiedDate = _ProvisioningForm.LastModifiedDate;
                                oIACMDBContext.ProvisioningFormGeneralDatas.Add(formGeneralData);
                                oIACMDBContext.SaveChanges();
                            }
                        }
                    }

                    if (_provisioningFormId > 0)
                    {
                        DeleteProvisioningFormGeneralData(_provisioningFormId, _ProvisioningForm, lstGenData);
                    }
                }
            }
            catch
            {
                isdataSaved = false;
                throw;
            }
            return isdataSaved;
        }

        private bool DeleteProvisioningFormGeneralData(long _provisioningFormId, ProvisioningForm _ProvisioningForm, List<ProvisioningFormGeneralData> lstGenData)
        {
            //string query = "DI_spDeleteProvisioningFormGeneralData";
            List<ProvisioningFormGeneralData> _ProvisioningFormGenDelData = lstGenData;
            bool isdataSaved = true;
            if (lstGenData == null)
                return false;

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    foreach (ProvisioningFormGeneralData FormGenData in lstGenData)
                    {
                        IList<ProvisioningFormGeneralData> delProvisioningFormGeneralData = new List<ProvisioningFormGeneralData>();
                        if (FormGenData.IsGroup == false || FormGenData.IsGroup == null)
                            delProvisioningFormGeneralData = oIACMDBContext.ProvisioningFormGeneralDatas.Where(pf => pf.AttributeName == FormGenData.AttributeName && pf.ProvisioningFormId == _provisioningFormId).ToList();
                        else
                            delProvisioningFormGeneralData = oIACMDBContext.ProvisioningFormGeneralDatas.Where(pf => pf.AttributeColumnName == FormGenData.AttributeColumnName && pf.ProvisioningFormId == _provisioningFormId).ToList();

                        for (int i = 0; i < delProvisioningFormGeneralData.Count; i++)
                        {
                            oIACMDBContext.ProvisioningFormGeneralDatas.Remove(delProvisioningFormGeneralData[i]);
                        }

                        oIACMDBContext.SaveChanges();
                    }
                }
            }
            catch
            {
                isdataSaved = false;
                throw;
            }
            return isdataSaved;
        }

        public void AddProvisioningFormConfig(IProvisioningForm EPProvisioningForm)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    if (EPProvisioningForm.GetType().Name == "ProvisioningFormAD")
                    {
                        _context.ProvisioningFormADs.Add((ProvisioningFormAD)EPProvisioningForm);
                    }
                    else if (EPProvisioningForm.GetType().Name == "ProvisioningFormLNS")
                    {
                        _context.ProvisioningFormLNSs.Add((ProvisioningFormLNS)EPProvisioningForm);
                    }
                    else if (EPProvisioningForm.GetType().Name == "ProvisioningFormSAP")
                    {
                        _context.ProvisioningFormSAPs.Add((ProvisioningFormSAP)EPProvisioningForm);
                    }
                    else if (EPProvisioningForm.GetType().Name == "ProvisioningFormSAPEP")
                    {
                        _context.ProvisioningFormSAPEPs.Add((ProvisioningFormSAPEP)EPProvisioningForm);
                    }

                    else if (EPProvisioningForm.GetType().Name == "ProvisioningFormOracleEBSs")
                    {
                        _context.ProvisioningFormOracleEBSs.Add((ProvisioningFormOracleEBS)EPProvisioningForm);
                    }

                    else if (EPProvisioningForm.GetType().Name == "ProvisioningFormOracle11gs")
                    {
                        _context.ProvisioningFormOracle11gs.Add((ProvisioningFormORCLUM)EPProvisioningForm);
                    }



                    else if (EPProvisioningForm.GetType().Name == "ProvisioningFormLINUX")
                    {
                        _context.ProvisioningFormLINUX.Add((ProvisioningFormLINUX)EPProvisioningForm);
                    }

                    _context.Entry(EPProvisioningForm.ProvisioningForm.EndPoint).State = EntityState.Unchanged;
                    _context.Entry(EPProvisioningForm.ProvisioningForm.EndPoint.EndPointSystemType).State = EntityState.Unchanged;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while Adding Endpoint Configuration", ex);
                throw ex;
            }
        }

        public void UpdateProvisioningFormConfig(IProvisioningForm EPProvisioningForm)
        {
            try
            {
                if (EPProvisioningForm.GetType().Name.Contains("ProvisioningFormAD"))
                {
                    ProvisioningFormAD epProvisioningForm = null;

                    epProvisioningForm = (ProvisioningFormAD)EPProvisioningForm;

                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        var ExistingProvForm = _context.ProvisioningFormADs.Include("ProvisioningForm").Include("ProvisioningForm.FormGeneralData").Where(b => b.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.ProvisioningFormGeneralDatas.RemoveRange(ExistingProvForm.ProvisioningForm.FormGeneralData);
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        foreach (var item in epProvisioningForm.ProvisioningForm.FormGeneralData)
                        {
                            item.ProvisioningFormId = epProvisioningForm.ProvisioningFormId.Value;
                            _context.Entry(item).State = EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        ProvisioningFormAD editProvisioningForm = _context.ProvisioningFormADs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.EndPoint.EndPointSystemType").Where(pf => pf.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.Entry(editProvisioningForm.ProvisioningForm).State = EntityState.Modified;
                        _context.Entry(editProvisioningForm).State = EntityState.Modified;
                       
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint).State = EntityState.Unchanged;
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint.EndPointSystemType).State = EntityState.Unchanged;

                        _context.Entry(editProvisioningForm).CurrentValues.SetValues(epProvisioningForm);

                        _context.Entry(editProvisioningForm.ProvisioningForm).CurrentValues.SetValues(epProvisioningForm.ProvisioningForm);
                        
                        _context.SaveChanges();
                    }
                }
                else if (EPProvisioningForm.GetType().Name.Contains("ProvisioningFormLNS"))
                {
                    ProvisioningFormLNS epProvisioningForm = null;

                    epProvisioningForm = (ProvisioningFormLNS)EPProvisioningForm;

                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        var ExistingProvForm = _context.ProvisioningFormLNSs.Include("ProvisioningForm").Include("ProvisioningForm.FormGeneralData").Where(b => b.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.ProvisioningFormGeneralDatas.RemoveRange(ExistingProvForm.ProvisioningForm.FormGeneralData);
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        foreach (var item in epProvisioningForm.ProvisioningForm.FormGeneralData)
                        {
                            item.ProvisioningFormId = epProvisioningForm.ProvisioningFormId.Value;
                            _context.Entry(item).State = EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        ProvisioningFormLNS editProvisioningForm = _context.ProvisioningFormLNSs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.EndPoint.EndPointSystemType").Where(pf => pf.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.Entry(editProvisioningForm.ProvisioningForm).State = EntityState.Modified;
                        _context.Entry(editProvisioningForm).State = EntityState.Modified;
                       
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint).State = EntityState.Unchanged;
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint.EndPointSystemType).State = EntityState.Unchanged;

                        _context.Entry(editProvisioningForm).CurrentValues.SetValues(epProvisioningForm);

                        _context.Entry(editProvisioningForm.ProvisioningForm).CurrentValues.SetValues(epProvisioningForm.ProvisioningForm);
                       
                        _context.SaveChanges();
                    }
                }
                else if (EPProvisioningForm.GetType().Name == "ProvisioningFormSAP")
                {
                    ProvisioningFormSAP epProvisioningForm = null;

                    epProvisioningForm = (ProvisioningFormSAP)EPProvisioningForm;

                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        var ExistingProvForm = _context.ProvisioningFormSAPs.Include("ProvisioningForm").Include("ProvisioningForm.FormGeneralData").Where(b => b.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.ProvisioningFormGeneralDatas.RemoveRange(ExistingProvForm.ProvisioningForm.FormGeneralData);
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        foreach (var item in epProvisioningForm.ProvisioningForm.FormGeneralData)
                        {
                            item.ProvisioningFormId = epProvisioningForm.ProvisioningFormId.Value;
                            _context.Entry(item).State = EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        ProvisioningFormSAP editProvisioningForm = _context.ProvisioningFormSAPs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.EndPoint.EndPointSystemType").Where(pf => pf.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.Entry(editProvisioningForm.ProvisioningForm).State = EntityState.Modified;
                        _context.Entry(editProvisioningForm).State = EntityState.Modified;
                       
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint).State = EntityState.Unchanged;
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint.EndPointSystemType).State = EntityState.Unchanged;

                        _context.Entry(editProvisioningForm).CurrentValues.SetValues(epProvisioningForm);

                        _context.Entry(editProvisioningForm.ProvisioningForm).CurrentValues.SetValues(epProvisioningForm.ProvisioningForm);
                       
                        _context.SaveChanges();
                    }
                }
                else if (EPProvisioningForm.GetType().Name.Contains("ProvisioningFormSAPEP"))
                {
                    ProvisioningFormSAPEP epProvisioningForm = null;

                    epProvisioningForm = (ProvisioningFormSAPEP)EPProvisioningForm;

                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        var ExistingProvForm = _context.ProvisioningFormSAPEPs.Include("ProvisioningForm").Include("ProvisioningForm.FormGeneralData").Where(b => b.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.ProvisioningFormGeneralDatas.RemoveRange(ExistingProvForm.ProvisioningForm.FormGeneralData);
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        foreach (var item in epProvisioningForm.ProvisioningForm.FormGeneralData)
                        {
                            item.ProvisioningFormId = epProvisioningForm.ProvisioningFormId.Value;
                            _context.Entry(item).State = EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        ProvisioningFormSAPEP editProvisioningForm = _context.ProvisioningFormSAPEPs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.EndPoint.EndPointSystemType").Where(pf => pf.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.Entry(editProvisioningForm.ProvisioningForm).State = EntityState.Modified;
                        _context.Entry(editProvisioningForm).State = EntityState.Modified;
                       
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint).State = EntityState.Unchanged;
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint.EndPointSystemType).State = EntityState.Unchanged;

                        _context.Entry(editProvisioningForm).CurrentValues.SetValues(epProvisioningForm);

                        _context.Entry(editProvisioningForm.ProvisioningForm).CurrentValues.SetValues(epProvisioningForm.ProvisioningForm);
                       
                        _context.SaveChanges();
                    }
                }


                else if (EPProvisioningForm.GetType().Name.Contains("ProvisioningFormOracleEBS"))
                {
                    ProvisioningFormOracleEBS epProvisioningForm = null;

                    epProvisioningForm = (ProvisioningFormOracleEBS)EPProvisioningForm;

                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        var ExistingProvForm = _context.ProvisioningFormOracleEBSs.Include("ProvisioningForm").Include("ProvisioningForm.FormGeneralData").Where(b => b.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.ProvisioningFormGeneralDatas.RemoveRange(ExistingProvForm.ProvisioningForm.FormGeneralData);
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        foreach (var item in epProvisioningForm.ProvisioningForm.FormGeneralData)
                        {
                            item.ProvisioningFormId = epProvisioningForm.ProvisioningFormId.Value;
                            _context.Entry(item).State = EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        ProvisioningFormOracleEBS editProvisioningForm = _context.ProvisioningFormOracleEBSs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.EndPoint.EndPointSystemType").Where(pf => pf.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.Entry(editProvisioningForm.ProvisioningForm).State = EntityState.Modified;
                        _context.Entry(editProvisioningForm).State = EntityState.Modified;
                        
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint).State = EntityState.Unchanged;
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint.EndPointSystemType).State = EntityState.Unchanged;

                        _context.Entry(editProvisioningForm).CurrentValues.SetValues(epProvisioningForm);

                        _context.Entry(editProvisioningForm.ProvisioningForm).CurrentValues.SetValues(epProvisioningForm.ProvisioningForm);
                       
                        _context.SaveChanges();
                    }
                }

                else if (EPProvisioningForm.GetType().Name.Contains("ProvisioningFormLINUX"))
                {
                    ProvisioningFormLINUX epProvisioningForm = null;

                    epProvisioningForm = (ProvisioningFormLINUX)EPProvisioningForm;

                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        var ExistingProvForm = _context.ProvisioningFormLINUX.Include("ProvisioningForm").Include("ProvisioningForm.FormGeneralData").Where(b => b.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.ProvisioningFormGeneralDatas.RemoveRange(ExistingProvForm.ProvisioningForm.FormGeneralData);
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        foreach (var item in epProvisioningForm.ProvisioningForm.FormGeneralData)
                        {
                            item.ProvisioningFormId = epProvisioningForm.ProvisioningFormId.Value;
                            _context.Entry(item).State = EntityState.Added;
                        }
                        _context.SaveChanges();
                    }
                    using (IACMDBContext _context = new IACMDBContext())
                    {
                        ProvisioningFormLINUX editProvisioningForm = _context.ProvisioningFormLINUX.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.EndPoint.EndPointSystemType").Where(pf => pf.Id == epProvisioningForm.Id).FirstOrDefault();

                        _context.Entry(editProvisioningForm.ProvisioningForm).State = EntityState.Modified;
                        _context.Entry(editProvisioningForm).State = EntityState.Modified;
                       
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint).State = EntityState.Unchanged;
                        _context.Entry(editProvisioningForm.ProvisioningForm.EndPoint.EndPointSystemType).State = EntityState.Unchanged;

                        _context.Entry(editProvisioningForm).CurrentValues.SetValues(epProvisioningForm);

                        _context.Entry(editProvisioningForm.ProvisioningForm).CurrentValues.SetValues(epProvisioningForm.ProvisioningForm);
                        
                        _context.SaveChanges();
                    }
                }


            }
            catch (Exception ex)
            {

            }

        }

        public IList<ProvisioningFormEndpointSystemType> GetAllProvisioningFormsEndpointSystemType(int EndpointSystemTypeId, int EndpointSystem)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return (from provForm in oIACMDBContext.ProvisioningForms.Include("EndPoint").Include("EndPoint.EndPointSystemType")
                            //join eSysType in oIACMDBContext.EndpointSystemTypes on provForm.EndpointSystemType equals eSysType.EndpointSystemTypeId
                            //join eSys in oIACMDBContext.EndpointSystems on provForm.EndpointSystemId equals eSys.EndpointSystemId
                            //where (EndpointSystemTypeId < 0 ? 1 == 1 : provForm.EndpointSystemType == EndpointSystemTypeId) && (EndpointSystem < 0 ? 1 == 1 : provForm.EndPoint.EndpointSystemId == EndpointSystem)
                            where (EndpointSystemTypeId < 0 ? 1 == 1 : provForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == EndpointSystemTypeId) && (EndpointSystem < 0 ? 1 == 1 : provForm.EndPoint.EndpointSystemId == EndpointSystem)
                            select new ProvisioningFormEndpointSystemType
                            {
                                ProvisioningFormId = provForm.ProvisioningFormId,
                                FormName = provForm.FormName,
                                FormDescritpion = provForm.FormDescritpion,
                                EndpointSystemType = provForm.EndPoint.EndPointSystemType.EndpointSystemTypeId, // provForm.EndpointSystemType,
                                EndpointSystemTypeName = provForm.EndPoint.EndPointSystemType.DisplayName, // eSysType.EndpointSystemTypeName,
                                EndpointSystem = provForm.EndPoint.DisplayName // eSys.HostName
                            }).ToList();

                    //IList<ProvisioningFormEndpointSystemType> provisioningFormEndpointSystemTypeList = new List<ProvisioningFormEndpointSystemType>();

                    //IList<ProvisioningForm> provisioningFormList = oIACMDBContext.ProvisioningForms.Include("EndPoint").Include("EndPoint.EndPointSystemType").
                    //                                                Where(provForm => (EndpointSystemTypeId < 0 ? 1 == 1 : provForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == EndpointSystemTypeId) && (EndpointSystem < 0 ? 1 == 1 : provForm.EndPoint.EndpointSystemId == EndpointSystem)).ToList();
                    //foreach (ProvisioningForm provForm in provisioningFormList)
                    //{
                    //    ProvisioningFormEndpointSystemType provisioningFormEndpointSystemType = new ProvisioningFormEndpointSystemType();
                    //    provisioningFormEndpointSystemType.ProvisioningFormId = provForm.ProvisioningFormId;
                    //    provisioningFormEndpointSystemType.FormName = provForm.FormName;
                    //    provisioningFormEndpointSystemType.FormDescritpion = provForm.FormDescritpion;
                    //    provisioningFormEndpointSystemType.EndpointSystemType = provForm.EndPoint.EndPointSystemType.EndpointSystemTypeId; // provForm.EndpointSystemType,
                    //    provisioningFormEndpointSystemType.EndpointSystemTypeName = provForm.EndPoint.EndPointSystemType.EndpointSystemTypeName; // eSysType.EndpointSystemTypeName,
                    //    provisioningFormEndpointSystemType.EndpointSystem = provForm.EndPoint.HostName; // eSys.HostName

                    //    provisioningFormEndpointSystemTypeList.Add(provisioningFormEndpointSystemType);
                    //}
                    //return provisioningFormEndpointSystemTypeList;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public IList<ProvisioningForm> GetAllProvisioningForms()
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ProvisioningForms.ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProvisioningForm GetProvisioningFormByID(long FormID, bool deepLoad)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ProvisioningForms.Include("EndPoint").Include("EndPoint.EndPointSystemType")
                        .Where(pf => pf.ProvisioningFormId == FormID).FirstOrDefault();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IProvisioningForm GetProvisioningForm(ProvisioningForm ProvisioningForm)
        {
            try
            {
                IProvisioningForm ProvForm = null;

                using (IACMDBContext _context = new IACMDBContext())
                {

                    if (ProvisioningForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == (byte)EndpointType.ActiveDirectory)
                    {
                        ProvForm = _context.ProvisioningFormADs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.FormGeneralData").Include("ProvisioningForm.EndPoint.EndPointSystemType").FirstOrDefault(b => b.ProvisioningForm.ProvisioningFormId == ProvisioningForm.ProvisioningFormId);
                    }
                    else if (ProvisioningForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == (byte)EndpointType.SAP)
                    {
                        ProvForm = _context.ProvisioningFormSAPs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.FormGeneralData").Include("ProvisioningForm.EndPoint.EndPointSystemType").FirstOrDefault(b => b.ProvisioningForm.ProvisioningFormId == ProvisioningForm.ProvisioningFormId);
                    }
                    else if (ProvisioningForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == (byte)EndpointType.SAPEnterprisePortal)
                    {
                        ProvForm = _context.ProvisioningFormSAPEPs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.FormGeneralData").Include("ProvisioningForm.EndPoint.EndPointSystemType").FirstOrDefault(b => b.ProvisioningForm.ProvisioningFormId == ProvisioningForm.ProvisioningFormId);
                    }
                    else if (ProvisioningForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == (byte)EndpointType.LotusNotes)
                    {
                        ProvForm = _context.ProvisioningFormLNSs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.FormGeneralData").Include("ProvisioningForm.EndPoint.EndPointSystemType").FirstOrDefault(b => b.ProvisioningForm.ProvisioningFormId == ProvisioningForm.ProvisioningFormId);
                    }
                    else if (ProvisioningForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == (byte)EndpointType.OracleEBS)
                    {
                        ProvForm = _context.ProvisioningFormOracleEBSs.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.FormGeneralData").Include("ProvisioningForm.EndPoint.EndPointSystemType").FirstOrDefault(b => b.ProvisioningForm.ProvisioningFormId == ProvisioningForm.ProvisioningFormId);
                    }
                    else if (ProvisioningForm.EndPoint.EndPointSystemType.EndpointSystemTypeId == (byte)EndpointType.Linux)
                    {
                        ProvForm = _context.ProvisioningFormLINUX.Include("ProvisioningForm").Include("ProvisioningForm.EndPoint").Include("ProvisioningForm.FormGeneralData").Include("ProvisioningForm.EndPoint.EndPointSystemType").FirstOrDefault(b => b.ProvisioningForm.ProvisioningFormId == ProvisioningForm.ProvisioningFormId);
                    }
                    return ProvForm;
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public ProvisioningFormSAPEP GetProvisioningFormSAPEP(long FormID)
        {
            //string query = "DI_spGetProvisioningFormSAPEPByFormID";
            ProvisioningFormSAPEP _ProvisioningFormSAPEP = null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormSAPEP> _ProvisioningFormSAPEPList = oIACMDBContext.ProvisioningFormSAPEPs.Include("AccountNameConstructor").Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormSAPEPList.Count > 0)
                        _ProvisioningFormSAPEP = _ProvisioningFormSAPEPList.First();
                }
                return _ProvisioningFormSAPEP;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ProvisioningFormLNS GetProvisioningFormLNS(long FormID)
        {
            //string query = "DI_spGetProvisioningFormLNSByFormID";
            ProvisioningFormLNS _ProvisioningFormLNS = null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormLNS> _ProvisioningFormLNSList = oIACMDBContext.ProvisioningFormLNSs.Include("ShortNameConstructor")
                        .Include("LongNameConstructor").Include("MailFileNameConstructor")
                        .Include("UserIdFileNameConstructor").Include("InternetAdderssNameConstructor").Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormLNSList.Count > 0)
                        _ProvisioningFormLNS = _ProvisioningFormLNSList.First();
                }
                return _ProvisioningFormLNS;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProvisioningFormSAP GetProvisioningFormSAP(long FormID)
        {
            //string query = "DI_spGetProvisioningFormSAPByFormID";
            ProvisioningFormSAP _ProvisioningFormSAP = null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormSAP> _ProvisioningFormSAPList = oIACMDBContext.ProvisioningFormSAPs.Include("AccountNameConstructor").Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormSAPList.Count > 0)
                        _ProvisioningFormSAP = _ProvisioningFormSAPList.First();
                }
                return _ProvisioningFormSAP;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProvisioningFormOracleEBS GetProvisioningFormOracleEBS(long FormID)
        {
            ProvisioningFormOracleEBS _provisioningFormOracleEBS = null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormOracleEBS> _ProvisioningFormOEBSList = oIACMDBContext.ProvisioningFormOracleEBSs.Include("AccountNameConstructor").Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormOEBSList.Count > 0)
                        _provisioningFormOracleEBS = _ProvisioningFormOEBSList.First();
                }
                return _provisioningFormOracleEBS;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProvisioningFormLINUX GetProvisioningFormLinux(long FormID)
        {
            ProvisioningFormLINUX _provisioningFormLinux = null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormLINUX> _ProvisioningFormLinuxList = oIACMDBContext.ProvisioningFormLINUX.Include("AccountNameConstructor").Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormLinuxList.Count > 0)
                        _provisioningFormLinux = _ProvisioningFormLinuxList.First();
                }
                return _provisioningFormLinux;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ProvisioningFormORCLUM GetProvisioningFormORCLUM(long FormID)
        {
            ProvisioningFormORCLUM _provisioningFormOracle11g= null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormORCLUM> _ProvisioningFormOEBSList = oIACMDBContext.ProvisioningFormOracle11gs.Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormOEBSList.Count > 0)
                        _provisioningFormOracle11g = _ProvisioningFormOEBSList.First();
                }
                return _provisioningFormOracle11g;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private EndpointType GetEndPointSystemTypeByFormID(long FormID)
        {
            EndpointType EndpointSystemName = EndpointType.ActiveDirectory;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    ProvisioningForm provForm = oIACMDBContext.ProvisioningForms.Where(pf => pf.ProvisioningFormId == FormID).FirstOrDefault();
                    if ((provForm != null) && (provForm.EndPoint != null))
                    {

                        EndpointSystemName = provForm.EndPoint.EndPointSystemType.EndpointSystemTypeName;
                    }
                }
                return EndpointSystemName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProvisioningFormAD GetProvisioningFormAD(long FormID)
        {
            //string query = "DI_spGetProvisioningFormADByFormID";
            ProvisioningFormAD _ProvisioningFormAD = null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormAD> _ProvisioningFormADList = oIACMDBContext.ProvisioningFormADs.Include("AccountNameConstructor").Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormADList.Count > 0)
                        _ProvisioningFormAD = _ProvisioningFormADList.First();
                }
                return _ProvisioningFormAD;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ProvisioningFormLNS GetProvisioningFormLotusNote(long FormID)
        {
            //string query = "DI_spGetProvisioningFormLotusNoteByFormID";
            ProvisioningFormLNS _provisioningFormLNS = null;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    IList<ProvisioningFormLNS> _ProvisioningFormLNSList = oIACMDBContext.ProvisioningFormLNSs.Include("ShortNameConstructor")
                        .Include("LongNameConstructor").Include("MailFileNameConstructor")
                        .Include("UserIdFileNameConstructor").Include("InternetAdderssNameConstructor").Where(pf => pf.ProvisioningFormId == FormID).ToList();

                    if (_ProvisioningFormLNSList.Count > 0)
                        _provisioningFormLNS = _ProvisioningFormLNSList.First();
                }
                return _provisioningFormLNS;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ProvisioningFormGeneralData> GetFormGeneralData(long FormID)
        {
            //string query = "DI_spGetProvisioningFormGenData";
            Dictionary<string, string> _ProvisioningFormGenData = new Dictionary<string, string>();
            List<ProvisioningFormGeneralData> list = new List<ProvisioningFormGeneralData>();
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    list = oIACMDBContext.ProvisioningFormGeneralDatas.Where(pfg => pfg.ProvisioningFormId == FormID).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        public IList<ProvisioningForm> GetProvisioningFormsByEndPointType(int endpointTypeId)
        {
            //string query = "DI_spGetProvisioningFormsByEndPointType";
            IList<ProvisioningForm> list = new List<ProvisioningForm>();

            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    list = oIACMDBContext.ProvisioningForms.Where(pf => pf.EndPoint.EndpointSystemTypeId == endpointTypeId).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting Provisioning Forms By Endpoint type.", ex);
                throw;
            }
            return list;
        }

        public ProvisioningForm GetProvisioningFormByEndpoint(int endpointId, AccountType accountType)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ProvisioningForms.FirstOrDefault(pf => pf.EndpointSystemId == endpointId && pf.AccountType == accountType);
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion

        public bool DeleteProvisioningForm(long _ProvisioningFormID)
        {
            //string query = "DI_spDeleteProvisioningForm";
            bool isdataSaved = true;
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {

                    IList<AccessRequest> delprovAR = oIACMDBContext.AccessRequests.Where(prl => prl.ProvisioningFormId == _ProvisioningFormID).ToList();
                    for (int i = 0; i < delprovAR.Count; i++)
                    {
                        oIACMDBContext.AccessRequests.Remove(delprovAR[i]);
                    }

                    IList<ProvisioningFormGeneralData> delProvGen = oIACMDBContext.ProvisioningFormGeneralDatas.Where(prg => prg.ProvisioningFormId == _ProvisioningFormID).ToList();
                    for (int i = 0; i < delProvGen.Count; i++)
                    {
                        oIACMDBContext.ProvisioningFormGeneralDatas.Remove(delProvGen[i]);
                    }
                    IList<ProvisioningFormAD> delProvAD = oIACMDBContext.ProvisioningFormADs.Where(pra => pra.ProvisioningFormId == _ProvisioningFormID).ToList();
                    for (int i = 0; i < delProvAD.Count; i++)
                    {
                        oIACMDBContext.ProvisioningFormADs.Remove(delProvAD[i]);
                    }

                    IList<ProvisioningFormLNS> delProvL = oIACMDBContext.ProvisioningFormLNSs.Where(prl => prl.ProvisioningFormId == _ProvisioningFormID).ToList();
                    for (int i = 0; i < delProvL.Count; i++)
                    {
                        oIACMDBContext.ProvisioningFormLNSs.Remove(delProvL[i]);
                    }

                    IList<ProvisioningFormSAP> delProvsap = oIACMDBContext.ProvisioningFormSAPs.Where(prs => prs.ProvisioningFormId == _ProvisioningFormID).ToList();
                    for (int i = 0; i < delProvsap.Count; i++)
                    {
                        oIACMDBContext.ProvisioningFormSAPs.Remove(delProvsap[i]);
                    }

                    IList<ProvisioningFormSAPEP> delProvsapep = oIACMDBContext.ProvisioningFormSAPEPs.Where(prsapep => prsapep.ProvisioningFormId == _ProvisioningFormID).ToList();
                    for (int i = 0; i < delProvsapep.Count; i++)
                    {
                        oIACMDBContext.ProvisioningFormSAPEPs.Remove(delProvsapep[i]);
                    }

                    RoleMatrixDetail delProvRMD = oIACMDBContext.RoleMatrixDetails.FirstOrDefault(prl => prl.ProvisioningFormId == _ProvisioningFormID);
                    if (delProvRMD != null)
                    {
                        oIACMDBContext.RoleMatrixDetails.Remove(delProvRMD);
                    }

                    IList<ProvisioningForm> delProvF = oIACMDBContext.ProvisioningForms.Where(prf => prf.ProvisioningFormId == _ProvisioningFormID).ToList();
                    for (int i = 0; i < delProvF.Count; i++)
                    {
                        oIACMDBContext.ProvisioningForms.Remove(delProvF[i]);
                    }

                    oIACMDBContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                isdataSaved = false;
                throw;
            }
            return isdataSaved;
        }

        public Dictionary<long, string> GetProvisioningForms(string searchString, int EndpointTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ProvisioningForms.Where(item => item.FormName.Contains(searchString) && (EndpointTypeId == -1 || item.EndPoint.EndPointSystemType.EndpointSystemTypeId == EndpointTypeId))
                        .Select(x => new { Key = x.ProvisioningFormId, Value = x.FormName })
                        .Distinct().ToDictionary(o => o.Key, o => o.Value);
                }
            }
            catch
            {
                throw;
            }
        }

        public ProvisioningForm GetProvisioningFormByName(string FormName)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.ProvisioningForms.Where(pf => pf.FormName == FormName).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.ERROR, "Error occur while getting Provisioning Forms By Form Name.", ex);
                throw;
            }
        }

        public IList<DefaultProvisioningFormGeneralData> loadDefaultProvisioningGeneralAttributes(int endpointTypeId)
        {
            try
            {
                using (IACMDBContext _context = new IACMDBContext())
                {
                    return _context.DefaultProvisioningFormGeneralDatas.Where(gendata => gendata.EndpointSystemTypeId == endpointTypeId).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.General.WriteMessage(LogLevel.INFO, "Error Occured while getting Provisioning Form General Data", ex);
                throw;
            }
        }

        public ProvisioningForm GetCreateProvisioningFormByEndpoint(int endpointId, AccountType accountType)
        {
            try
            {
                using (IACMDBContext oIACMDBContext = new IACMDBContext())
                {
                    return oIACMDBContext.ProvisioningForms.FirstOrDefault(pf => pf.EndpointSystem == endpointId &&
                           pf.AccountType == accountType && pf.IsCreateAccountForm.Value);
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
