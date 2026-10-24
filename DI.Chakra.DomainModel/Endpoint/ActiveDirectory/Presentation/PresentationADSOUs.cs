using AutoMapper;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationADSOUs
    {
        public long OuId { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public Nullable<DateTime> whenCreated { get; set; }

        public Nullable<DateTime> whenChanged { get; set; }

        public string HostName { get; set; }

        public string Institution { get; set; }

        public static PresentationADSOUs ORMap(EntityModel.Presentation.PresentationADSOUs presentationADSOUsEntityModel)
        {
            if (presentationADSOUsEntityModel == null)
                return null;
            return Mapper.Map<EntityModel.Presentation.PresentationADSOUs, PresentationADSOUs>(presentationADSOUsEntityModel);
        }

        public static List<PresentationADSOUs> ORMapList(IList<EntityModel.Presentation.PresentationADSOUs> presentationADSOUsEntityModels)
        {
            return presentationADSOUsEntityModels.Select(ORMap).ToList();
        }

        public static List<PresentationADSOUs> GetByEndPointSystemId(long endpointSystemid)
        {
            List<PresentationADSOUs> ouReportDatalist = new List<PresentationADSOUs>();
            PresentationADSOUs presentationADSOUsReportData = new PresentationADSOUs();

            using (var dataAccessTransaction = new DataAccessTransaction())
            {
                List<ADSOuInfo> adsOuInfoList = ADSOuInfo.GetByEndPointSystemId(endpointSystemid);
                foreach (ADSOuInfo ou in adsOuInfoList)
                {
                    presentationADSOUsReportData = new PresentationADSOUs();
                    presentationADSOUsReportData.OuId = ou.Id;
                    presentationADSOUsReportData.name = ou.name;
                    presentationADSOUsReportData.description = ou.description;
                    //presentationADSOUsReportData.whenCreated = ou.whenCreated.HasValue ? ou.whenCreated.Value.ToString() : string.Empty;
                    //presentationADSOUsReportData.whenChanged = ou.whenChanged.HasValue ? ou.whenChanged.Value.ToString() : string.Empty;
                    if(ou.whenCreated.HasValue)
                        presentationADSOUsReportData.whenCreated = ou.whenCreated;
                    if (ou.whenChanged.HasValue)
                        presentationADSOUsReportData.whenChanged = ou.whenChanged;
                    presentationADSOUsReportData.HostName = EndPoint.GetById(endpointSystemid).DisplayName;
                    ouReportDatalist.Add(presentationADSOUsReportData);
                }
            }
            return ouReportDatalist;
        }
    }
}
