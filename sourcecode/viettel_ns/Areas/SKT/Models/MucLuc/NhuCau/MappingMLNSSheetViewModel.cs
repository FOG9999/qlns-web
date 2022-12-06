using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class MappingMLNSSheetTable : SheetTable
    {
        public MappingMLNSSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }

        public MappingMLNSSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public MappingMLNSSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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
                    new SheetColumn(columnName: "Nganh_Parent", header: "Ngành cha", columnWidth:80, align: "center",hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Nganh", header: "Ngành", columnWidth:60, align: "center",hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "KyHieu", header: "Mã mục lục số nhu cầu", columnWidth:120, align: "center",hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "MoTa", header: "Nội dung", columnWidth:240, isReadonly: true),
                    new SheetColumn(columnName: "Loai", header: "Loại ngân sách", columnWidth:200, align: "center", isReadonly: true),
                    new SheetColumn(columnName: "Map", header: "Đã/Chưa map", columnWidth:120, align: "center", isReadonly: true),

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

    public class MappingMLNSViewModel
    {
        public MappingMLNSViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }
        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
