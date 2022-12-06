using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.Admin.Models
{
    public class CauHinhNamMLNSSheetTable : SheetTable
    {

        public CauHinhNamMLNSSheetTable(bool parentRowEditable = false) : base(false, parentRowEditable)
        {
        }


        public CauHinhNamMLNSSheetTable(Dictionary<string, string> filters, bool parentRowEditable = false) : this(parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public CauHinhNamMLNSSheetTable(NameValueCollection query, bool parentRowEditable = false) : this(parentRowEditable)
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
                    new SheetColumn(columnName: "WYear", header: "Năm làm việc", columnWidth:200, dataType:0, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "YearOfMLNS", header: "Năm MLNS", columnWidth:100, dataType:0, align: "center", hasSearch: true, isReadonly: false),
                    
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

    public class CauHinhNamMLNSViewModel
    {
        public CauHinhNamMLNSViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
