using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;

namespace VIETTEL.Models
{
    public class ChuKySheetTable : SheetTable
    {

        public ChuKySheetTable(NameValueCollection query)
        {
            _filters = getFilters(query);
        }

        public ChuKySheetTable(Dictionary<string, string> filters = null)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            return new List<SheetColumn>()
            {
                    new SheetColumn(columnName: "#", header: "#", columnWidth: 40, align: "center"),
                    new SheetColumn(columnName: "sKyHieu", header: "Ký hiệu", columnWidth: 120, isReadonly: false, hasSearch: true),
                    new SheetColumn(columnName: "sTen", header: "Tên chữ ký", columnWidth: 320, isReadonly: false, hasSearch: true),
                    new SheetColumn(columnName: "iSTT", header: "STT", columnWidth: 100, isReadonly: false, dataType: 1),
                    new SheetColumn(columnName: "sMoTa", header: "Ghi chú", columnWidth: 369, isReadonly: false),

                    // cot khac
                    new SheetColumn(columnName: "iID_MaChuKy", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
            };
        }

        private Dictionary<string, string> getFilters(NameValueCollection query)
        {
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query[c.ColumnName]);
                });
            return filters;
        }

        public void FillSheet(DataTable dt)
        {
            dt.Columns.Add("#", typeof(int));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["#"] = i + 1;
            }

            dtChiTiet = dt.Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();

            ColumnNameId = "iID_MaChuKy";
            ColumnNameParentId = "";
            ColumnNameIsParent = "";

            //updateSummaryRows();
            //insertSummaryRows("sMoTa");

            updateColumnIDs("iID_MaChuKy");
            updateColumns();
            //updateColumnsParent();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
        }


        protected override string getCellValue(DataRow r, int i, string c, IEnumerable<string> columnsName)
        {
            if (i % 2 == 0)
            {
                if (c == "sMauSac")
                {
                    return "#EFEFEF";
                }
            }

            return base.getCellValue(r, i, c, columnsName);
        }

    }
}
