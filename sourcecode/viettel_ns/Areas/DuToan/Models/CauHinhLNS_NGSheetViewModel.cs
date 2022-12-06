using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToan.Models
{
    public class CauHinhLNS_NGSheetTable : SheetTable
    {
        public CauHinhLNS_NGSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }
        public CauHinhLNS_NGSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }
        public CauHinhLNS_NGSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = getFilters(query);
        }

        public void FillSheet(DataTable dt)
        {
            dtChiTiet = dt.Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();

            ColumnNameId = "Id";
            ColumnNameIsParent = "";
            ColumnNameParentId = "";

            updateColumnIDs("Id");
            updateColumns();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
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

        #region Columns

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var items = new List<SheetColumn>()
                {
                    // cot nhap
                    new SheetColumn(columnName: "Nam", header: "Năm làm việc", columnWidth:120, align: "left", isReadonly: false),
                    new SheetColumn(columnName: "sLNS", header: "Loại ngân sách", columnWidth:140, align: "left", isReadonly: false),
                    new SheetColumn(columnName: "sNG", header: "Ngành", columnWidth:140, align: "left", isReadonly: false),
                    new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:300, align:"left", isReadonly: false),
                    
                    // cot khac
                    new SheetColumn(columnName: "Id", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };

            return items;
        }

        #endregion

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

    public class CauHinhLNS_NGViewModel
    {
        public CauHinhLNS_NGViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
