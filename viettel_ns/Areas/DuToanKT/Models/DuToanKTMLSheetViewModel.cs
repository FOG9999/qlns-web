using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Models
{
    public class MucLucSheetTable : SheetTable
    {

        public MucLucSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }


        public MucLucSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public MucLucSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = getFilters(query);
        }

        public void FillSheet(DataTable dt)
        {
            dtChiTiet = dt.Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();

            ColumnNameId = "Id";
            ColumnNameParentId = "Id_Parent";
            ColumnNameIsParent = "IsParent";

            updateColumnIDs("Id");
            updateColumns();
            updateColumnsParent();
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
                    new SheetColumn(columnName: "Loai", header: "Loại nhập", columnWidth:100, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "Code", header: "Mã mục lục", columnWidth:120, hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "Ng", header: "Ngành quản lý", columnWidth:120, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "Nganh", header: "Mã ngành", columnWidth:100, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "STT", header: "Số thứ tự đánh dấu", columnWidth:120, align: "center", isReadonly: false),
                    new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:240, isReadonly: false),
                    new SheetColumn(columnName: "XauNoiMa", header: "LNS-L-K-M-???", columnWidth:240, isReadonly: false),
                    new SheetColumn(columnName: "XauNoiMa_x", header: "LNS-L-K-M-??? (Ko lấy)", columnWidth:240, isReadonly: false),                    

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
            if (r.Field<bool>(ColumnNameIsParent))
            {
                if (c == "sMauSac")
                {
                    return "#EFEFEF";
                }
                else if (c == "sFontColor")
                {
                    return "OrangeRed";
                }
                else if (c == "sFontBold")
                {
                    return "bold";
                }
            }

            return base.getCellValue(r, i, c, columnsName);
        }

    }

    public class DuToanKTMucLucViewModel
    {
        public DuToanKTMucLucViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
