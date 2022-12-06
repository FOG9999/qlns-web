using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.Admin.Models
{
    public class MLNSSheetTable : SheetTable
    {
        public MLNSSheetTable(bool isReadonly = true, bool parentRowEditable = false)
        {
            IsReadonly = IsReadonly;
            ParentRowEditable = parentRowEditable;
        }

        public MLNSSheetTable(Dictionary<string, string> filters, bool isReadonly = true, bool parentRowEditable = false) : this(isReadonly, parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public MLNSSheetTable(NameValueCollection query, bool isReadonly = true, bool parentRowEditable = false) : this(isReadonly, parentRowEditable)
        {
            _filters = getFilters(query);
        }

        public void FillSheet(DataTable dt)
        {
            //IsReadonly = false;

            //_filters = filters ?? new Dictionary<string, string>();

            dtChiTiet = dt.Copy();
            dtChiTiet_Cu = dtChiTiet.Copy();


            ColumnNameId = "iID_MaMucLucNganSach";
            ColumnNameParentId = "iID_MaMucLucNganSach_Cha";
            ColumnNameIsParent = "bLaHangCha";

            //if (_arrDuLieu != null) return;

            updateColumnIDs("iID_MaMucLucNganSach");
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
            return new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "sLNS", header: "LNS", columnWidth:80, hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sL", header: "L", columnWidth:40, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sK", header: "K", columnWidth:40,  align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sM", header: "M", columnWidth:50, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sTM", header: "TM", columnWidth:50,  align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sTTM", header: "TTM", columnWidth:40, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sNG", header: "NG", columnWidth:40, align: "center", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:520, isReadonly: false),
                    // cot khac
                    new SheetColumn(columnName: "iID_MaMucLucNganSach", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                };
        }

        #endregion


    }


}
