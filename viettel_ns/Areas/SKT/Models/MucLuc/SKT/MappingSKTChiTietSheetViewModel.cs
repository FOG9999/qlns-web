using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class MappingSKTChiTietSheetTable : SheetTable
    {
        public MappingSKTChiTietSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }

        public MappingSKTChiTietSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public MappingSKTChiTietSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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

            updateColumnIDs("Id,Id_MaSKT");
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
                    new SheetColumn(columnName: "Nganh_Parent", header: "Ngành quản lý", columnWidth:120, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Nganh", header: "Mã ngành", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "KyHieu", header: "Mã mục lục", columnWidth:120, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "MoTa", header: "Nội dung", columnWidth:240, isReadonly: true),

                    // cot nhap
                    new SheetColumn(columnName: "Map", header: "Chọn/Bỏ chọn(0)", columnWidth:120, align: "center", isReadonly: false),

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

    public class MappingSKTChiTietViewModel
    {
        public MappingSKTChiTietViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }
        public Guid Id { get; set; }
        public int Loai { get; set; }
        public SheetViewModel Sheet { get; set; }
        public SKT_MLNhuCau SKTViewModel { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
