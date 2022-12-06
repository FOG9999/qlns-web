using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class CauHinhMucLucDacThuSheetTable : SheetTable
    {
        public CauHinhMucLucDacThuSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }
        public CauHinhMucLucDacThuSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }
        public CauHinhMucLucDacThuSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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
                    // cot fix
                    new SheetColumn(columnName: "NamNS", header: "Năm ngân sách", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "M", header: "Mã mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Tm", header: "Mã tiểu mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Ttm", header: "Mã tiểu tiết mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Ng", header: "Mã ngành", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:240, isReadonly: true),

                    // cot nhap
                    new SheetColumn(columnName: "DacThu", header: "Chọn/Bỏ chọn(0)", columnWidth:120, align: "center", isReadonly: false),
                    
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

    public class CauHinhMucLucDacThuViewModel
    {
        public CauHinhMucLucDacThuViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
