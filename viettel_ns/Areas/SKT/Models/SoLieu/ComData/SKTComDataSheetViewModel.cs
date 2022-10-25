using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class SKTComDataSheetTable : SheetTable
    {

        public SKTComDataSheetTable(SKT_ComData_ChungTu chungtu, DataRow dataR = null, bool isReadonly = true, bool parentRowEditable = false) : base(isReadonly, parentRowEditable)
        {
            ctu = chungtu;
            dtr = dataR;
        }

        public SKTComDataSheetTable(Dictionary<string, string> filters, SKT_ComData_ChungTu chungtu, DataRow dataR = null, bool isReadonly = true, bool parentRowEditable = false) : this(chungtu, dataR, isReadonly, parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public SKTComDataSheetTable(NameValueCollection query, SKT_ComData_ChungTu chungtu, DataRow dataR = null, bool isReadonly = true, bool parentRowEditable = false) : this(chungtu, dataR, isReadonly, parentRowEditable)
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

        public DataRow dtr { get; private set; }
        public SKT_ComData_ChungTu ctu { get; private set; }
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
            };
            var Nam_Skt = ctu.NamLamViec - 1;
            var lstNT = new List<List<string>>();
            lstNT.Add(dtr["NamNS_2"].ToString().Split('_').ToList());
            lstNT.Add(dtr["NamNS_1"].ToString().Split('_').ToList());
            if (ctu.iLoai == 1)
            {
                items.AddRange(
                    new List<SheetColumn>()
                    {
                        new SheetColumn(columnName: "TuChi_1", header: "Chi bằng tiền", columnWidth:100, isReadonly: false, isFixed: false,  dataType: 1),
                        new SheetColumn(columnName: "TuChi_2", header: "Chi bằng tiền", columnWidth:100, isReadonly: false, isFixed: false,  dataType: 1),
                        new SheetColumn(columnName: "TuChi_3", header: $"Số kiểm tra năm {Nam_Skt} (3)", columnWidth: 100, isReadonly: false, isFixed: false, dataType: 1),
                    });
                if (lstNT[0][0] == "QT")
                {
                    items.First(x => x.ColumnName == "TuChi_2").Header = "Quyết toán năm " + lstNT[0][1] + " (1)";
                }
                else
                {
                    items.First(x => x.ColumnName == "TuChi_2").Header = "Dự toán đầu năm " + lstNT[0][1] + " (1)";
                }
                if (lstNT[1][0] == "QT")
                {
                    items.First(x => x.ColumnName == "TuChi_1").Header = "Quyết toán năm " + lstNT[1][1] + " (2)";
                }
                else
                {
                    items.First(x => x.ColumnName == "TuChi_1").Header = "Dự toán đầu năm " + lstNT[1][1] + " (2)";
                }
            }
            else if (ctu.iLoai == 2)
            {
                items.AddRange(
                    new List<SheetColumn>()
                    {
                        new SheetColumn(columnName: "MuaHang_1", header: "Hàng nhập", columnWidth: 120, isReadonly: false, isFixed: false, dataType: 1),
                        new SheetColumn(columnName: "MuaHang_2", header: "Hàng nhập", columnWidth: 120, isReadonly: false, isFixed: false, dataType: 1),
                        new SheetColumn(columnName: "MuaHang_3", headerGroup: $"Số kiểm tra năm {Nam_Skt}", headerGroupIndex: 2, header: "Mua hàng (3)", columnWidth: 120, isReadonly: false, isFixed: false, dataType: 1),
                        new SheetColumn(columnName: "PhanCap_3", headerGroup: $"Số kiểm tra năm {Nam_Skt}", headerGroupIndex: 2, header: "Phân cấp (4)",columnWidth: 120, isReadonly: false, isFixed: false, dataType: 1),
                    });
                if (lstNT[0][0] == "QT")
                {
                    items.First(x => x.ColumnName == "MuaHang_2").Header = "Quyết toán năm " + lstNT[0][1] + " (1)";
                }
                else
                {
                    items.First(x => x.ColumnName == "MuaHang_2").Header = "Dự toán đầu năm " + lstNT[0][1] + " (1)";
                }
                if (lstNT[1][0] == "QT")
                {
                    items.First(x => x.ColumnName == "MuaHang_1").Header = "Quyết toán năm " + lstNT[1][1] + " (2)";
                }
                else
                {
                    items.First(x => x.ColumnName == "MuaHang_1").Header = "Dự toán đầu năm " + lstNT[1][1] + " (2)";
                }
            }            
            items.AddRange(
                new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "Id", isHidden: true),
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
                if (ctu.Id_PhongBan != "07")
                {
                    if (c == "sMauSac")
                        return "#EFEFEF";
                } else if (ctu.Id_PhongBan == "07")
                {                    
                    if (c == "sMauSac")
                    {
                        return "#00FFFF";
                    }
                    else if (c == "sFontColor")
                    {
                        return "OrangeRed";
                    }
                    else if (c == "sFontBold")
                    {
                        return "";
                    }
                }
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
                    });

                    _arrEdit.Add(items);
                });
        }
    }

    public class SKTComDataSheetViewModel
    {
        public SKTComDataSheetViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }
        public SheetViewModel Sheet { get; set; }
        public ComDataChungTuDetailsViewModel ChungTuViewModel { get; set; }
        public Guid Id { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
