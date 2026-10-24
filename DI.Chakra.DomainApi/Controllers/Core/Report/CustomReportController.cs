using AutoMapper;
using DI.Chakra.DomainApi.Controllers;
using DI.Chakra.Dto;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace DI.Chakra.ReportApi.Controllers
{
    [RoutePrefix("CustomReport")]
    public class CustomReportController : BaseController<DomainModel.CustomReport, EntityModel.CustomReport, Dto.CustomReport>
    {
        [HttpGet]
        [Route("GetAllRunReports")]
        public List<CustomReport> GetAllRunReports()
        {
            return DtoMapList(DomainModel.CustomReport.GetAllRunReports());
        }

        [HttpGet]
        [Route("GetAllColumnsFromQuery")]
        public List<string> GetAllColumnsFromQuery(string query)
        {
            return DomainModel.CustomReport.GetAllColumnsFromQuery(query);
        }

        [HttpPost]
        [Route("GetReportViewerDataForExport")]
        public byte[] GetReportViewerDataForExport([FromUri]long reportId, [FromUri]bool isExcelReport, [FromBody]string updateQuery, [FromUri]string requestedUser, [FromUri]string filterExpression)
        {
            return DomainModel.CustomReport.GetReportViewerDataForExport(reportId, isExcelReport, updateQuery, requestedUser, filterExpression);
        }

        [HttpPost]
        [Route("GetReportViewerDataForExport")]
        public byte[] GetReportViewerDataForExport([FromUri]string customQuery, [FromUri]bool isExcelReport, [FromBody]string updateQuery, [FromUri]string requestedUser)
        {
            return DomainModel.CustomReport.GetReportViewerDataForExport(customQuery, isExcelReport, updateQuery, requestedUser);
        }

        [HttpGet]
        [Route("GetQueryResults")]
        public DataTable GetQueryResults(string customQuery)
        {
            return DomainModel.CustomReport.GetQueryResults(customQuery);
        }

        [HttpPost]
        [Route("GetReportViewerData")]
        public PresentationCustomReportData GetReportViewerData([FromUri]long reportId, [FromUri]int startRowIndex, [FromUri]int maximumRows, [FromUri]string FilterExpression, [FromUri]string sortExpression, [FromBody]string updatedQuery)
        {
            return PCR_DtoMap(DomainModel.CustomReport.GetReportViewerData(reportId, startRowIndex, maximumRows, FilterExpression, sortExpression, false, updatedQuery));
        }

        [HttpPost]
        [Route("GetReportViewerData")]
        public PresentationCustomReportData GetReportViewerData([FromUri]string query, [FromUri]int startRowIndex, [FromUri]int maximumRows, [FromUri]string FilterExpression, [FromUri]string sortExpression, [FromBody]string updatedQuery)
        {
            return PCR_DtoMap(DomainModel.CustomReport.GetReportViewerData(query, startRowIndex, maximumRows, FilterExpression, sortExpression, false, updatedQuery));
        }

        [HttpGet]
        [Route("DeleteCustomReportByRuleId")]
        public int DeleteCustomReportByRuleId(long reportId)
        {
            return DomainModel.CustomReport.DeleteCustomReportByRuleId(reportId);
        }

        public PresentationCustomReportData PCR_DtoMap(DomainModel.Presentation.PresentationCustomReportData tModel)
        {
            if (tModel == null)
                return null;

            return Mapper.Map<DomainModel.Presentation.PresentationCustomReportData, PresentationCustomReportData>(tModel);
        }

        public List<PresentationCustomReportData> PCR_DtoMapList(IList<DomainModel.Presentation.PresentationCustomReportData> tModelList)
        {
            return tModelList.Select(PCR_DtoMap).ToList();
        }
    }
}