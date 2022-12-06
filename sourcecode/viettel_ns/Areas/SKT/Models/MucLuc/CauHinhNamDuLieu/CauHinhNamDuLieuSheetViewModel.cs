using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class CauHinhNamDuLieuSheetTable : SheetTable
    {
        public CauHinhNamDuLieuSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }
        public CauHinhNamDuLieuSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }
        public CauHinhNamDuLieuSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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
                    new SheetColumn(columnName: "NamLamViec", header: "Năm làm việc", columnWidth:120, align: "left", isReadonly: false,hasSearch:true),
                    new SheetColumn(columnName: "NamNS_1", header: "Năm ngân sách 1", columnWidth:140, align: "left", isReadonly: false,hasSearch:true),
                    new SheetColumn(columnName: "NamNS_2", header: "Năm ngân sách 2", columnWidth:140, align: "left", isReadonly: false,hasSearch:true),
                    new SheetColumn(columnName: "QuyetDinh", header: "Quyết định BQP", columnWidth:300, align:"left", isReadonly: false),
                    new SheetColumn(columnName: "CVKHNS", header: "Công văn KHNS", columnWidth:300, align:"left", isReadonly: false),
                    
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

    public class CauHinhNamDuLieuViewModel
    {
        public CauHinhNamDuLieuViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
