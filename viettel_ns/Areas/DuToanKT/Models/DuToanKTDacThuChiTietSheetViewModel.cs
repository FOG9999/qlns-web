using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Models
{
    public class DacThuChiTietSheetTable : SheetTable
    {

        public DacThuChiTietSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }


        public DacThuChiTietSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public DacThuChiTietSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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

            updateColumnIDs("Id,iID_MaMucLucNganSach");
            updateColumns();
            //updateColumnsParent();
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
                    new SheetColumn(columnName: "M", header: "Mục", columnWidth:60, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Tm", header: "Tiểu mục", columnWidth:80, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "TTm", header: "Tiểu tiết mục", columnWidth:100, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "Ng", header: "Ngành", columnWidth:60, align: "center", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "MoTa", header: "Nội dung", columnWidth:360, isReadonly: true),
                    new SheetColumn(columnName: "TuChi", header: "Số đặc thù", columnWidth:200, isReadonly: false, dataType: 1),         

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
            //if (r.Field<bool>(ColumnNameIsParent))
            //{
            //    if (c == "sMauSac")
            //    {
            //        return "#EFEFEF";
            //    }
            //    else if (c == "sFontColor")
            //    {
            //        return "OrangeRed";
            //    }
            //    else if (c == "sFontBold")
            //    {
            //        return "bold";
            //    }
            //}

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

    public class DuToanKTDacThuChiTietViewModel
    {
        public DuToanKTDacThuChiTietViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
