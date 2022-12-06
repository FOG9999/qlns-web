using AutoMapper.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class MappingMLNSChiTietSheetTable : SheetTable
    {
        public MappingMLNSChiTietSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }

        public MappingMLNSChiTietSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public MappingMLNSChiTietSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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

            updateColumnIDs("Id,Xau");
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
                    new SheetColumn(columnName: "Lns", header: "Mã loại ngân sách", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "L", header: "Mã loại", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "K", header: "Mã khoản", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "M", header: "Mã mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "TM", header: "Mã tiểu mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "TTM", header: "Mã tiểu tiết mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "NG", header: "Mã ngành", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "MoTa", header: "Nội dung", columnWidth:240, isReadonly: true),

                    // cot nhap
                    new SheetColumn(columnName: "Map", header: "Chọn/Bỏ chọn(0)", columnWidth:120, align: "center", isReadonly: false),

                    // cot khac
                    new SheetColumn(columnName: "Id", isHidden: true),
                    new SheetColumn(columnName: "Xau", isHidden: true),
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
    
    public class MappingMLNSChiTietViewModel
    {
        public MappingMLNSChiTietViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }
        public Guid Id { get; set; }
        public int Loai { get; set; }
        public SKT_MLNhuCau NhuCauViewModel { get; set; }
        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
