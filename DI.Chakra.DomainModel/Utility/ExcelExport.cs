
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System.Data;

namespace DI.Chakra.DomainModel
{
    public class ExcelExport
    {
        private readonly DataTable _dataToExport;
        private readonly string _reportName;
        private readonly string _userName;

        // Used to format data range on worksheet
        private string _modelRange;
        private const int ColumnStart = 1;
        private const int RowsStart = 5;
        private const double minWidth = 15;
        private const double maxWidth = 50;
        private readonly int _rowCount;

        // Reflection used to identify DateTime fields for formatting
        private readonly Type _typeNullDatetime = typeof(DateTime?);
        private readonly Type _typeDateTime = typeof(DateTime);
        private readonly DataColumnCollection _modelProperties;

        public ExcelExport(DataTable dataToExport, string reportName, string userName)
        {
            _dataToExport = dataToExport;
            _reportName = reportName;
            _userName = userName;
            _rowCount = GetRowCount();
            _modelProperties = dataToExport == null ? null : dataToExport.Columns;
        }

        private int GetRowCount()
        {
            var count = _dataToExport == null ? RowsStart : _dataToExport.Rows.Count + RowsStart;
            return count;
        }


        // Without this, DateTimes are formatted as integers
        private void FormatDateTimeAsDate(ExcelWorksheet worksheet)
        {
            if (_modelProperties == null) return;

            for (var j = 0; j < _modelProperties.Count; j++)
            {
                if (_modelProperties[j].DataType == _typeNullDatetime
                        && DateTimeFormatInfo.CurrentInfo != null)
                    worksheet.Column(j + ColumnStart).Style.Numberformat.Format =
                        "dd-MM-yyyy hh:mm:ss";//DateTimeFormatInfo.CurrentInfo.SortableDateTimePattern;

                if (_modelProperties[j].DataType != _typeDateTime) continue;
                if (DateTimeFormatInfo.CurrentInfo != null)
                    worksheet.Column(j + ColumnStart).Style.Numberformat.Format =
                        "dd-MM-yyyy hh:mm:ss";
            }
        }

        public byte[] ExportToExcel()
        {
            using (var excelPackage = new ExcelPackage())
            {
                // create worksheet
                excelPackage.Workbook.Worksheets.Add("Sheet1");
                var worksheet = excelPackage.Workbook.Worksheets[1];
                worksheet.Name = "Sheet1";
                excelPackage.Workbook.Properties.Author = _userName;

                AddHeaderInfo(worksheet);
                if (_rowCount - RowsStart <= 0)
                    worksheet.Cells[RowsStart, 1].Value = "No records exist.";
                else
                    worksheet.Cells[RowsStart, 1].LoadFromDataTable(_dataToExport, true);

                if (_dataToExport != null)
                {
                    FormatDateTimeAsDate(worksheet);
                    AutoFitColumns(worksheet);
                    FormatDataTable(worksheet);
                }

                return excelPackage.GetAsByteArray();
            }
        }

        private void FormatDataTable(ExcelWorksheet worksheet)
        {
            if (_rowCount == 0)
            {
                var range = worksheet.Cells["A" + RowsStart.ToString()];

                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffc7ce"));
                range.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#be0006"));

            }
            else
            {
                var dataRange = string.Concat("A" + RowsStart.ToString() + ":", GetRightmostColumnLetter(), (_rowCount + 1).ToString());
                var range = worksheet.Cells[dataRange];
                var table = worksheet.Tables.Add(range, "DataExport");
                table.TableStyle = TableStyles.Medium15;

            }
        }

        private static string GetExcelColumnName(int columnNumber)
        {
            var dividend = columnNumber;
            var columnName = string.Empty;
            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }
            return columnName;
        }

        private void AutoFitColumns(ExcelWorksheet worksheet)
        {

            var rightCol = GetRightmostColumnLetter();
            var fullRows = (_rowCount <= 3 ? 3 : _rowCount).ToString();

            _modelRange = string.Concat("A" + RowsStart.ToString() + ":", rightCol, fullRows);
            var modelTable = worksheet.Cells[_modelRange];
            modelTable.AutoFitColumns(minWidth, maxWidth);
        }

        private string GetRightmostColumnLetter()
        {
            var dataPropertiesInfos = _dataToExport != null ? _dataToExport.Columns : null;
            var dataCols = dataPropertiesInfos == null ? 0 : dataPropertiesInfos.Count;
            var fullColumns = dataCols; //dataCols + ColumnStart;
            var rightCol = GetExcelColumnName(fullColumns);
            return rightCol;
        }

        private void AddHeaderInfo(ExcelWorksheet worksheet)
        {
            worksheet.Cells[1, 1].Value = "Report Name";
            worksheet.Cells[1, 2].Value = _reportName;
            worksheet.Cells[2, 1].Value = "Time Stamp";
            worksheet.Cells[2, 2].Value =
                DateTime.Now.ToString(CultureInfo.InvariantCulture);
            worksheet.Cells[3, 1].Value = "Requested By";
            worksheet.Cells[3, 2].Value = _userName;

            worksheet.Cells["A1:B3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:B3"].Style.Fill.BackgroundColor.SetColor(Color.LightYellow);

            worksheet.Cells["A1:A3"].Style.Font.Bold = true;
            worksheet.Cells["B1:B3"].Style.Font.Italic = true;

        }
    }
}
