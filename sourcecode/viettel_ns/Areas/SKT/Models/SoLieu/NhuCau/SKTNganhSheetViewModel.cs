using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class SKTNganhSheetTable : SheetTable
    {

        public SKTNganhSheetTable(SKT_ChungTu chungtu, bool isReadonly = true, bool parentRowEditable = false) : base(isReadonly, parentRowEditable)
        {
            ctu = chungtu;
        }

        public SKTNganhSheetTable(Dictionary<string, string> filters, SKT_ChungTu chungtu, bool isReadonly = true, bool parentRowEditable = false) : this(chungtu, isReadonly, parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public SKTNganhSheetTable(NameValueCollection query, SKT_ChungTu chungtu, bool isReadonly = true, bool parentRowEditable = false) : this(chungtu, isReadonly, parentRowEditable)
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

            updateSummaryRows();
            insertSummaryRows("MoTa");

            updateColumnIDs("Id,Id_ChungTuChiTiet");
            updateColumns();
            updateColumnsParent();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
        }

        public SKT_ChungTu ctu { get; private set; }
        public string readonlyKyHieu { get; set; }

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
                new SheetColumn(columnName: "Nganh_Parent", header: "Ngành cha", columnWidth:50, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
                new SheetColumn(columnName: "Nganh", header: "Ngành", columnWidth:50, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
                new SheetColumn(columnName: "MoTa", header: "Nội dung", columnWidth:240, isReadonly: true, isFixed: true),
                new SheetColumn(columnName: "sTenDonVi", header: "Đơn vị", columnWidth:150, isReadonly: false, dataType: 3),
                new SheetColumn(columnName: "iID_MaPhongBanDich", header: "B", columnWidth:50, isReadonly: false, dataType: 0),                
            };   
            if (ctu.iLoai == 5)
            {
                items.AddRange(
                    new List<SheetColumn>
                    {
                        new SheetColumn(columnName: "Tang", header: "Tăng (1)", headerGroup: "Ngành thẩm định", headerGroupIndex: 2, columnWidth: 120, isReadonly: false, dataType: 1),
                        new SheetColumn(columnName: "Giam", header: "Giảm (2)", headerGroup: "Ngành thẩm định", headerGroupIndex: 2, columnWidth: 120, isReadonly: false, dataType: 1),
                    }
                );
            } else if (ctu.iLoai == 6)
            {
                items.AddRange(
                   new List<SheetColumn>
                   {
                        new SheetColumn(columnName: "MuaHang", header: "Hàng mua", headerGroup: "Ngành thẩm định", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                        new SheetColumn(columnName: "HangNhap", header: "Chi bằng ngoại tệ", headerGroup: "Ngành thẩm định", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                        new SheetColumn(columnName: "PhanCap", header: "Ngành phân cấp bằng tiền", headerGroup: "Ngành thẩm định", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                   }
               );
            }
            items.AddRange(
                new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:400, isReadonly: false),
                    new SheetColumn(columnName: "Id", isHidden: true),
                    new SheetColumn(columnName: "iID_MaDonVi", isReadonly: false, isHidden: true),
                    new SheetColumn(columnName: "KyHieu", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                }
            );
            return items;
        }
        #endregion

        protected override string getCellValue(DataRow r, int i, string c, IEnumerable<string> columnsName)
        {
            // hang tong cong
            if (i == 0 || i == dtChiTiet.Rows.Count - 1)
            {
                if (c == "sMauSac")
                {
                    return "LightGray";
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
            else if (r.Field<bool>(ColumnNameIsParent))
            {                
                if (c == "sMauSac")
                    return "#EFEFEF";                
            }

            return base.getCellValue(r, i, c, columnsName);
        }
        protected override void updateCellsEditable()
        {
            _arrEdit = new List<List<string>>();

            dtChiTiet.AsEnumerable().ToList()
                .ForEach(r =>
                {
                    var isReadonlyRow = false;

                    if (!string.IsNullOrWhiteSpace(ColumnNameIsParent) && Convert.ToBoolean(r[ColumnNameIsParent]) && !ParentRowEditable) isReadonlyRow = true;

                    readonlyKyHieu.Split(',').ToList().ForEach(x => {
                        var kh = r["KyHieu"];
                        if (x.Equals(kh))
                        {
                            isReadonlyRow = true;
                        }
                    });

                    var items = new List<string>();
                    Columns.ToList()
                    .ForEach(c =>
                    {
                        if (IsEditable && !isReadonlyRow)
                        {
                            items.Add(c.IsReadonly ? "" : "1");
                        }
                        else
                        {
                            items.Add("");
                        }
                    });

                    _arrEdit.Add(items);
                });
        }        
    }

    public class SKTNganhSheetViewModel
    {
        public SKTNganhSheetViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }
        public SheetViewModel Sheet { get; set; }
        public ChungTuDetailsViewModel ChungTuViewModel { get; set; }
        public Guid Id { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
