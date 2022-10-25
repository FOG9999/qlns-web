using System;
using System.IO;
using Aspose.Cells;
using DataTable = System.Data.DataTable;

namespace VIETTEL.Areas.z.Models
{
    public class ExcelHelpers
    {

        public static DataTable LoadExcelDataTable(byte[] fileBytes, int column = 0, int row = 0, int sheet = 0, bool asString = false)
        {
            // excel columns
            using (var ms = new MemoryStream(fileBytes))
            {
                var workbook = new Workbook(ms);
                Worksheet worksheet = workbook.Worksheets[sheet];
                return loadExcelTable(worksheet, column, row, sheet, asString);
            }
        }

        private static DataTable loadExcelTable(Worksheet worksheet, int column = 0, int row = 0, int sheet = 0,
            bool asString = false)
        {
            DataTable dt = null;
            // data from excel file
            int columnCount = worksheet.Cells.MaxDataColumn + 1;

            int rowCount = worksheet.Cells.MaxDataRow + 1;
            Range range = worksheet.Cells.CreateRange(row, column, rowCount - row, columnCount);
            if (asString)
            {
                dt = range.ExportDataTableAsString();
            }
            else
            {
                try
                {
                    dt = range.ExportDataTable();
                }
                catch (Exception ex)
                {
                    dt = range.ExportDataTableAsString();
                }
            }
            return dt;
        }

    }
}