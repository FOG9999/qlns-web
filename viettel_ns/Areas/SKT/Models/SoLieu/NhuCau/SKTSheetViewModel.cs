using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.SKT.Models
{
    public class SKTSheetTable : SheetTable
    {

        public SKTSheetTable(SKT_ChungTu chungtu, DataRow dataR = null, bool isReadonly = true, bool parentRowEditable = false) : base(isReadonly, parentRowEditable)
        {
            ctu = chungtu;
            dtr = dataR;
        }

        public SKTSheetTable(Dictionary<string, string> filters, SKT_ChungTu chungtu, DataRow dataR = null, bool isReadonly = true, bool parentRowEditable = false) : this(chungtu, dataR, isReadonly, parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public SKTSheetTable(NameValueCollection query, SKT_ChungTu chungtu, DataRow dataR = null, bool isReadonly = true, bool parentRowEditable = false) : this(chungtu, dataR, isReadonly, parentRowEditable)
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
                        new SheetColumn(columnName: "TuChi_DTT_1", header: "Chi bằng tiền", columnWidth:100, isReadonly: true, isFixed: true,  dataType: 1),
                        new SheetColumn(columnName: "TuChi_DTT_2", header: "Chi bằng tiền", columnWidth:100, isReadonly: true, isFixed: true,  dataType: 1),
                        new SheetColumn(columnName: "TuChi_DTT_3", header: $"Số kiểm tra năm {Nam_Skt} (3)", columnWidth: 100, isReadonly: true, isFixed: true, dataType: 1),
                    });
                if (lstNT[0][0] == "QT")
                {
                    items.First(x => x.ColumnName == "TuChi_DTT_2").Header = "Quyết toán năm " + lstNT[0][1] + " (2)";
                }
                else
                {
                    items.First(x => x.ColumnName == "TuChi_DTT_2").Header = "Dự toán đầu năm " + lstNT[0][1] + " (2)";
                }
                if (lstNT[1][0] == "QT")
                {
                    items.First(x => x.ColumnName == "TuChi_DTT_1").Header = "Quyết toán năm " + lstNT[1][1] + " (1)";
                }
                else
                {
                    items.First(x => x.ColumnName == "TuChi_DTT_1").Header = "Dự toán đầu năm " + lstNT[1][1] + " (1)";
                }
                if (ctu.Id_PhongBan == "11")
                {
                    items.AddRange(
                    new List<SheetColumn>()
                    {
                            //new SheetColumn(columnName: "HuyDong_Bql", header: "Huy động SD tồn kho (4)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TuChi_Bql", header: "BQL đề nghị chi bằng tiền (4)", columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TuChi_B2", header: "B2 đề nghị chi bằng tiền (5)", columnWidth: 120, isReadonly: true, dataType: 1),
                            //new SheetColumn(columnName: "TongCong_Bql", header: "Cộng (6)=(4+5)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TuChi", header: "Cục duyệt (6)", columnWidth: 120, isReadonly: false, dataType: 1),
                            //new SheetColumn(columnName: "Giam", header: "Giảm (6)", headerGroup: "Cục duyệt so với số B2 đề nghị", headerGroupIndex: 2, columnWidth: 120, isReadonly: false, dataType: 1),
                    });
                }
                else if (ctu.Id_PhongBan == "02")
                {
                    items.AddRange(
                        new List<SheetColumn>()
                        {
                            new SheetColumn(columnName: "TongCong_DV", header: "Cộng (4)=(6+7)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TonKho_DV", header: $"GT Tồn kho 01/01/{Nam_Skt} (5)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4,columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "HuyDong_DV", header: "Huy động SD tồn kho (6)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4,columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TuChi_DV", header: "Chi bằng tiền (7)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4,columnWidth:120, isReadonly: true, dataType: 1),
                            //new SheetColumn(columnName: "HuyDong_Bql", header: "Huy động SD tồn kho (8)", headerGroup: "Bql đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                            //new SheetColumn(columnName: "TuChi_Bql", header: "Chi bằng tiền (8)", headerGroup: "Bql đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TuChi_Bql", header: "Bql đề nghị chi bằng tiền (8)", columnWidth: 120, isReadonly: true, dataType: 1),
                            //new SheetColumn(columnName: "TongCong_Bql", header: "Cộng (10)=(8+9)", headerGroup: "Bql đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                            //new SheetColumn(columnName: "HuyDong", header: "Huy động SD tồn kho (11)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                            //new SheetColumn(columnName: "TuChi", header: "Chi bằng tiền (9)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "TuChi", header: "B2 đề nghị chi bằng tiền (9)", columnWidth: 120, isReadonly: false, dataType: 1),
                            //new SheetColumn(columnName: "TongCong", header: "Cộng (13)=(11+12)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Tang", header: "Tăng (10)=(9-3)", headerGroup: "Ch.lệch b2 với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Giam", header: "Giảm (11)=(3-9)", headerGroup: "Ch.lệch b2 với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                        }
                    );
                }
                else
                {
                    items.AddRange(
                        new List<SheetColumn>()
                        {
                            new SheetColumn(columnName: "TongCong_DV", header: "Cộng (4)=(6+7)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TonKho_DV", header: $"GT Tồn kho 01/01/{Nam_Skt} (5)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4,columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "HuyDong_DV", header: "Huy động SD tồn kho (6)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4,columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "TuChi_DV", header: "Chi bằng tiền (7)", headerGroup: "Đơn vị đề nghị", headerGroupIndex: 4,columnWidth:120, isReadonly: false, dataType: 1),
                            //new SheetColumn(columnName: "HuyDong", header: "Huy động SD tồn kho (8)", headerGroup: "Bql đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                            //new SheetColumn(columnName: "TuChi", header: "Chi bằng tiền (8)", headerGroup: "Bql đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "TuChi", header: "Bql đề nghị chi bằng tiền (8)", columnWidth: 120, isReadonly: false, dataType: 1),
                            //new SheetColumn(columnName: "TongCong", header: "Cộng (10)=(8+9)", headerGroup: "Bql đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Tang", header: "Tăng (9)=(8-3)", headerGroup: "Ch.lệch Bql với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Giam", header: "Giảm (10)=(3-8)", headerGroup: "Ch.lệch Bql với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                        }
                    );
                }
                
            }
            else if (ctu.iLoai == 2)
            {
                items.AddRange(
                    new List<SheetColumn>()
                    {
                        new SheetColumn(columnName: "MuaHang_DTT_1", header: "Hàng nhập", columnWidth: 120, isReadonly: true, isFixed: true, dataType: 1),
                        new SheetColumn(columnName: "MuaHang_DTT_2", header: "Hàng nhập", columnWidth: 120, isReadonly: true, isFixed: true, dataType: 1),
                        new SheetColumn(columnName: "MuaHang_DTT_3", headerGroup: $"Số kiểm tra năm {Nam_Skt}", headerGroupIndex: 2, header: "Mua hàng (3)", columnWidth: 120, isReadonly: true, isFixed: true, dataType: 1),
                        new SheetColumn(columnName: "PhanCap_DTT_3", headerGroup: $"Số kiểm tra năm {Nam_Skt}", headerGroupIndex: 2, header: "Phân cấp (4)",columnWidth: 120, isReadonly: true, isFixed: true, dataType: 1),
                    });
                if (lstNT[0][0] == "QT")
                {
                    items.First(x => x.ColumnName == "MuaHang_DTT_2").Header = "Quyết toán năm " + lstNT[0][1] + " (2)";
                }
                else
                {
                    items.First(x => x.ColumnName == "MuaHang_DTT_2").Header = "Dự toán đầu năm " + lstNT[0][1] + " (2)";
                }
                if (lstNT[1][0] == "QT")
                {
                    items.First(x => x.ColumnName == "MuaHang_DTT_1").Header = "Quyết toán năm " + lstNT[1][1] + " (1)";
                }
                else
                {
                    items.First(x => x.ColumnName == "MuaHang_DTT_1").Header = "Dự toán đầu năm " + lstNT[1][1] + " (1)";
                }
                if (ctu.Id_PhongBan == "11")
                {
                    items.AddRange(
                        new List<SheetColumn>()
                        {
                                //new SheetColumn(columnName: "HuyDong_Bql", header: "Huy động SD tồn kho (5)", headerGroup: "B2 đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                                new SheetColumn(columnName: "MuaHang_Bql", header: "Mua hàng cấp hiện vật (5)", headerGroup: "BQL đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                                new SheetColumn(columnName: "PhanCap_Bql", header: "Ngành phân cấp bằng tiền (6)", headerGroup: "BQL đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                                new SheetColumn(columnName: "TongCong_Bql", header: "Cộng (7)=(5+6)", headerGroup: "BQL đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                                new SheetColumn(columnName: "MuaHang_B2", header: "Mua hàng cấp hiện vật (8)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                                new SheetColumn(columnName: "PhanCap_B2", header: "Ngành phân cấp bằng tiền (9)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                                new SheetColumn(columnName: "TongCong_B2", header: "Cộng (10)=(8+9)", headerGroup: "B2 đề nghị", headerGroupIndex: 3, columnWidth: 120, isReadonly: true, dataType: 1),
                                new SheetColumn(columnName: "MuaHang", header: "Mua hàng cấp hiện vật (11)", headerGroup: "Cục duyệt", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                                new SheetColumn(columnName: "PhanCap", header: "Ngành phân cấp bằng tiền (12)", headerGroup: "Cục duyệt", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                                new SheetColumn(columnName: "TongCong", header: "Cộng (13)=(11+12)", headerGroup: "Cục duyệt", headerGroupIndex: 3, columnWidth: 120, isReadonly: false, dataType: 1),
                        }
                    );                    
                }
                else if (ctu.Id_PhongBan == "02")
                {                   
                    items.AddRange(
                        new List<SheetColumn>()
                        {
                            new SheetColumn(columnName: "TongCong_DV", header: "Cộng (5)=(6+7+8)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TonKho_DV", header: $"GT Tồn kho 01/01/{Nam_Skt} (6)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "HuyDong_DV", header: "Huy động SD tồn kho (7)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "MuaHang_DV", header: "Mua hàng cấp hiện vật (8)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "PhanCap_DV", header: "Ngành phân cấp bằng tiền (9)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "MuaHang_Bql", header: "Mua hàng cấp hiện vật (10)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "HangNhap_Bql", header: "Mua hàng nhập ngoại (11)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "PhanCap_Bql", header: "Ngành phân cấp bằng tiền (12)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TongCong_Bql", header: "Cộng (13)=(10+11+12)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "MuaHang", header: "Mua hàng cấp hiện vật(14)", headerGroup: "B2 đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "HangNhap", header: "Mua hàng nhập ngoại (15)", headerGroup: "B2 đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "PhanCap", header: "Ngành phân cấp bằng tiền(16)", headerGroup: "B2 đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "TongCong", header: "Cộng (17)=(14+15+16)", headerGroup: "B2 đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Tang", header: "Tăng (18)=(17-3-4)", headerGroup: "Ch.lệch b2 với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Giam", header: "Giảm (19)=(3+4-17)", headerGroup: "Ch.lệch b2 với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                        }
                    );
                }
                else
                {
                    items.AddRange(
                        new List<SheetColumn>()
                        {
                            new SheetColumn(columnName: "TongCong_DV", header: "Cộng (5)=(6+7+8)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "TonKho_DV", header: $"GT Tồn kho 01/01/{Nam_Skt} (5)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "HuyDong_DV", header: "Huy động SD tồn kho (6)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "MuaHang_DV", header: "Mua hàng cấp hiện vật (7)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "PhanCap_DV", header: "Ngành phân cấp bằng tiền (8)", headerGroup: "Ngành đề nghị", headerGroupIndex: 5, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "MuaHang", header: "Mua hàng cấp hiện vật(9)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "HangNhap", header: "Mua hàng nhập ngoại (10)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "PhanCap", header: "Ngành phân cấp bằng tiền(11)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "TongCong", header: "Cộng (12)=(9+10+11)", headerGroup: "Bql đề nghị", headerGroupIndex: 4, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Tang", header: "Tăng (13)=(12-3-4)", headerGroup: "Ch.lệch Bql với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                            new SheetColumn(columnName: "Giam", header: "Giảm (14)=(3+4-12)", headerGroup: "Ch.lệch Bql với số kiểm tra năm " + (ctu.NamLamViec - 1), headerGroupIndex: 2, columnWidth: 120, isReadonly: true, dataType: 1),
                        }
                    );
                }
                
            }
            else if (ctu.iLoai == 3)
            {
                items.AddRange(
                        new List<SheetColumn>()
                        {
                            new SheetColumn(columnName: "Tang", header: "Tăng (1)", headerGroup: "Bql đề nghị tăng/giảm nhiệm vụ", headerGroupIndex: 2, columnWidth: 120, isReadonly: false, dataType: 1),
                            new SheetColumn(columnName: "Giam", header: "Giảm (2)", headerGroup: "Bql đề nghị tăng/giảm nhiệm vụ", headerGroupIndex: 2, columnWidth: 120, isReadonly: false, dataType: 1),
                        }
                    );
            }
            else if (ctu.iLoai == 4)
            {
                items.AddRange(
                        new List<SheetColumn>()
                        {
                            new SheetColumn(columnName: "Giam", header: "Giảm tự chủ bệnh viện", columnWidth: 120, isReadonly: false, dataType: 1),
                        }
                    );
            }
            var user = dtr["UserInput"].ToString();
            if (!"thuyb2,duongb2,tungb2".Contains(user))
            {
                items.AddRange(
                    new List<SheetColumn>()
                    {
                        new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:400, isReadonly: false),                    
                    }
                );
            }
            items.AddRange(
                new List<SheetColumn>()
                {
                    new SheetColumn(columnName: "Nganh_Parent", columnWidth:50, hasSearch: true, isHidden: true),
                    new SheetColumn(columnName: "Nganh", columnWidth:50, hasSearch: true, isHidden: true),
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
                        else
                        {
                            items.Add(c.ColumnName.EndsWith("_DV") ? "1" : "");
                        }
                    });

                    _arrEdit.Add(items);
                });
        }
        private bool checkValue(DataRow r)
        {
            if (r.Field<double>("TuChi") == 0)
            {
                return false;
            }
            else
            {
                if (r.Field<double>("HuyDong") == r.Field<double>("HuyDong_Bql") ||
                    r.Field<double>("HuyDong") == r.Field<double>("HuyDong_DV"))
                {
                    return true;
                }
                else if (r.Field<double>("TuChi") == r.Field<double>("TuChi_Bql") ||
                         r.Field<double>("TuChi") == r.Field<double>("TuChi_DV"))
                {
                    return true;
                }
                else if (r.Field<double>("MuaHang") == r.Field<double>("MuaHang_Bql") ||
                         r.Field<double>("MuaHang") == r.Field<double>("MuaHang_DV"))
                {
                    return true;
                }
                else if (r.Field<double>("PhanCap") == r.Field<double>("PhanCap_Bql") ||
                         r.Field<double>("PhanCap") == r.Field<double>("PhanCap_DV"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public class SKTSheetViewModel
    {
        public SKTSheetViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }
        public SheetViewModel Sheet { get; set; }
        public ChungTuDetailsViewModel ChungTuViewModel { get; set; }
        public Guid Id { get; set; }
        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
