using AutoMapper;
using DI.Chakra.Infrastructure.Enums;
using DI.Chakra.Repository;
using DI.Chakra.Transaction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DI.Chakra.DomainModel.Presentation
{
    public class PresentationCustomReportData : DomainModelBase<PresentationCustomReportData, EntityModel.Presentation.PresentationCustomReportData>
    {
        public long Count { get; set; }
        public DataTable ReportData { get; set; }
        public List<DataColumn> ReportColumns { get; set; }

        public static PresentationCustomReportData ORMap(EntityModel.Presentation.PresentationCustomReportData presentationCustomReportEntityModel)
        {
            if (presentationCustomReportEntityModel == null)
                return null;

            return Mapper.Map<EntityModel.Presentation.PresentationCustomReportData, PresentationCustomReportData>(presentationCustomReportEntityModel);
        }

        public static List<PresentationCustomReportData> ORMapList(IList<EntityModel.Presentation.PresentationCustomReportData> presentationCustomReportEntityModel)
        {
            return presentationCustomReportEntityModel.Select(ORMap).ToList();
        }
    }
}
