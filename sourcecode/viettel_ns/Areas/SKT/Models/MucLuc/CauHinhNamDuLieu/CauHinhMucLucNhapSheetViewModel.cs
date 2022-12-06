using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class CauHinhMucLucNhapSheetTable : SheetTable
    {
        public CauHinhMucLucNhapSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }
        public CauHinhMucLucNhapSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }
        public CauHinhMucLucNhapSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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

            updateColumnIDs("Id,id_MLNC");
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
                    new SheetColumn(columnName: "Nganh", header: "Ngành", columnWidth:80, align: "center",hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "KyHieu", header: "Mã mục lục số kiểm tra", columnWidth:120, align: "center",hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "MoTa", header: "Nội dung", columnWidth:240, isReadonly: true),
                    new SheetColumn(columnName: "Id_PhongBans", header: "Phòng ban", columnWidth:240, isReadonly: false),
                    new SheetColumn(columnName: "Map", header: "Đã/Chưa map", columnWidth:120, align: "center", isReadonly: false),
                    
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

    public class CauHinhMucLucNhapViewModel
    {
        public CauHinhMucLucNhapViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
