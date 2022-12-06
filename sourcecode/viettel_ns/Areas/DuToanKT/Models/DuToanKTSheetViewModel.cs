using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using VIETTEL.Models;

namespace VIETTEL.Areas.DuToanKT.Models
{
    public class DTKTSheetTable : SheetTable
    {

        public DTKTSheetTable(int loai, int irequest, bool isReadonly = true, bool parentRowEditable = false) : base(isReadonly, parentRowEditable)
        {
            iLoai = loai;
            iRequest = irequest;
        }


        public DTKTSheetTable(Dictionary<string, string> filters, int loai, int irequest, bool isReadonly = true, bool parentRowEditable = false) : this(loai, irequest, isReadonly, parentRowEditable)
        {
            _filters = filters ?? new Dictionary<string, string>();
        }

        public DTKTSheetTable(NameValueCollection query, int loai, int irequest, bool isReadonly = true, bool parentRowEditable = false) : this(loai, irequest, isReadonly, parentRowEditable)
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
            insertSummaryRows("sMoTa");

            updateColumnIDs("Id,Id_ChungTuChiTiet");
            updateColumns();
            updateColumnsParent();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
        }

        public int iLoai { get; private set; }

        public int iRequest { get; private set; }

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
        //protected override IEnumerable<SheetColumn> GetColumns()
        //{
        //    var items = new List<SheetColumn>()
        //        {
        //            // cot fix
        //            new SheetColumn(columnName: "Code", header: "Mã", columnWidth:120, hasSearch: true, isReadonly: true, isFixed: true),
        //            new SheetColumn(columnName: "Ng", header: "Ng", columnWidth:40, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
        //            new SheetColumn(columnName: "Nganh", header: "Ng", columnWidth:40, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
        //            new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:240, isReadonly: true, isFixed: true),

        //            // cot nhap
        //            new SheetColumn(columnName: "TuChi_Bql", header: "Tự chi (Bql đề nghị)", columnWidth:140, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "TuChi_B2", header: "Tự chi (B2 đề nghị)", columnWidth:140, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "TuChi", header: "Tự chi", columnWidth:140, isReadonly: false, dataType: 1),
        //            new SheetColumn(columnName: "HangNhap_Bql", header: "Hàng nhập (Bql đề nghị)", columnWidth:140, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "HangNhap_B2", header: "Hàng nhập (B2 đề nghị)", columnWidth:140, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "HangNhap", header: "Hàng nhập", columnWidth:140, isReadonly: false, dataType: 1),
        //            new SheetColumn(columnName: "HangMua_Bql", header: "Hàng mua (Bql đề nghị)", columnWidth:140, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "HangMua_B2", header: "Hàng mua (B2 đề nghị)", columnWidth:140, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "HangMua", header: "Hàng mua", columnWidth:140, isReadonly: false, dataType: 1),
        //            new SheetColumn(columnName: "DacThu_Bql", header: "Số đặc thù (Bql đề nghị)", columnWidth: 180, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "DacThu", header: "Số đặc thù", columnWidth: 140, isReadonly: false, dataType: 1),
        //            new SheetColumn(columnName: "TangNV_Bql", header: "Số tăng do nhiệm vụ (Bql đề nghị)", columnWidth: 180, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "TangNV", header: "Số tăng do nhiêm vụ", columnWidth: 160, isReadonly: false, dataType: 1),
        //            new SheetColumn(columnName: "GiamNV_Bql", header: "Số giảm do nhiệm vụ (Bql đề nghị)", columnWidth: 180, isReadonly: true, dataType: 1),
        //            new SheetColumn(columnName: "GiamNV", header: "Số giảm do nhiệm vụ", columnWidth: 160, isReadonly: false, dataType: 1),
        //            new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:360, isReadonly: false),

        //            // cot khac
        //            new SheetColumn(columnName: "Id", isHidden: true),
        //            new SheetColumn(columnName: "sMauSac", isHidden: true),
        //            new SheetColumn(columnName: "sFontColor", isHidden: true),
        //            new SheetColumn(columnName: "sFontBold",isHidden: true),
        //        };

        //    // neu nhap theo don vi thi chi co tu chi
        //    if (iLoai == 1)
        //    {
        //        items.Remove(items.First(x => x.ColumnName == "HangNhap_Bql"));
        //        items.Remove(items.First(x => x.ColumnName == "HangMua_Bql"));
        //        items.Remove(items.First(x => x.ColumnName == "HangNhap_B2"));
        //        items.Remove(items.First(x => x.ColumnName == "HangMua_B2"));
        //        items.Remove(items.First(x => x.ColumnName == "HangNhap"));
        //        items.Remove(items.First(x => x.ColumnName == "HangMua"));
        //        if (iRequest == 0)
        //        {
        //            items.Remove(items.First(x => x.ColumnName == "TuChi_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "TuChi_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "DacThu_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "TangNV_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "GiamNV_Bql"));
        //        }
        //        else if (iRequest == 1)
        //        {
        //            items.Remove(items.First(x => x.ColumnName == "TuChi_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "DacThu"));
        //            items.Remove(items.First(x => x.ColumnName == "TangNV"));
        //            items.Remove(items.First(x => x.ColumnName == "GiamNV"));
        //            items.First(x => x.ColumnName == "TuChi").Header = "B2 Đề nghị";
        //        }
        //        else
        //        {
        //            items.Remove(items.First(x => x.ColumnName == "DacThu"));
        //            items.Remove(items.First(x => x.ColumnName == "TangNV"));
        //            items.Remove(items.First(x => x.ColumnName == "GiamNV"));
        //            items.First(x => x.ColumnName == "TuChi").Header = "Cục duyệt";
        //        }
        //    }
        //    else
        //    {
        //        if (iRequest == 0)
        //        {
        //            items.Remove(items.First(x => x.ColumnName == "TuChi_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "TuChi_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "HangNhap_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "HangNhap_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "HangMua_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "HangMua_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "DacThu_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "TangNV_Bql"));
        //            items.Remove(items.First(x => x.ColumnName == "GiamNV_Bql"));
        //        }
        //        else if (iRequest == 1)
        //        {
        //            items.Remove(items.First(x => x.ColumnName == "TuChi_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "HangNhap_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "HangMua_B2"));
        //            items.Remove(items.First(x => x.ColumnName == "DacThu"));
        //            items.Remove(items.First(x => x.ColumnName == "TangNV"));
        //            items.Remove(items.First(x => x.ColumnName == "GiamNV"));
        //            items.First(x => x.ColumnName == "TuChi").Header = "B2 Đề nghị (TC)";
        //            items.First(x => x.ColumnName == "HangNhap").Header = "B2 Đề nghị (HN)";
        //            items.First(x => x.ColumnName == "HangMua").Header = "B2 Đề nghị (HM)";
        //        }
        //        else
        //        {
        //            items.Remove(items.First(x => x.ColumnName == "DacThu"));
        //            items.Remove(items.First(x => x.ColumnName == "TangNV"));
        //            items.Remove(items.First(x => x.ColumnName == "GiamNV"));
        //            items.First(x => x.ColumnName == "TuChi").Header = "Cục duyệt (TC)";
        //            items.First(x => x.ColumnName == "HangNhap").Header = "Cục duyệt (HN)";
        //            items.First(x => x.ColumnName == "HangMua").Header = "Cục duyệt (HM)";
        //        }
        //    }

        //    return items;
        //}
        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var items = new List<SheetColumn>()
                {
                #region fix 1

                //// cot fix
                //new SheetColumn(columnName: "Code", header: "Mã",columnWidth:120, hasSearch: true, isReadonly: true, isFixed: true),
                //new SheetColumn(columnName: "Ng", header: "Ng", columnWidth:40, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
                //new SheetColumn(columnName: "Nganh", header: "Ng", columnWidth:40, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
                //new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:240, isReadonly: true, isFixed: true),

                //// cot nhap
                //// Tự chi                    
                //new SheetColumn(columnName: "TuChi", header: "Tự chi", columnWidth:140, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "DacThu", header: "Số đặc thù (TC)", columnWidth: 140, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "TangNV", header: "Số tăng do nhiêm vụ (TC)", columnWidth: 160, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "GiamNV", header: "Số giảm do nhiệm vụ (TC)", columnWidth: 160, isReadonly: false, dataType: 1),

                //// Hàng nhập
                //new SheetColumn(columnName: "HangNhap", header: "Hàng nhập", columnWidth:140, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "DacThu_HN", header: "Số đặc thù (HN)", columnWidth: 140, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "TangNV_HN", header: "Số tăng do nhiêm vụ (HN)", columnWidth: 160, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "GiamNV_HN", header: "Số giảm do nhiệm vụ (HN)", columnWidth: 160, isReadonly: false, dataType: 1),

                //// Hàng mua
                //new SheetColumn(columnName: "HangMua", header: "Hàng mua", columnWidth:140, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "DacThu_HM", header: "Số đặc thù (HM)", columnWidth: 140, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "TangNV_HM", header: "Số tăng do nhiêm vụ (HM)", columnWidth: 160, isReadonly: false, dataType: 1),
                //new SheetColumn(columnName: "GiamNV_HM", header: "Số giảm do nhiệm vụ (HM)", columnWidth: 160, isReadonly: false, dataType: 1),

                //new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:360, isReadonly: false),

                //// cot khac
                //new SheetColumn(columnName: "Id", isHidden: true),
                //new SheetColumn(columnName: "sMauSac", isHidden: true),
                //new SheetColumn(columnName: "sFontColor", isHidden: true),
                //new SheetColumn(columnName: "sFontBold",isHidden: true),

                #endregion

                #region fix2

                
                // cot fix
                new SheetColumn(columnName: "Code", header: "Mã",columnWidth:120, hasSearch: true, isReadonly: true, isFixed: true),
                new SheetColumn(columnName: "Ng", header: "Ng", columnWidth:40, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
                new SheetColumn(columnName: "Nganh", header: "Ng", columnWidth:40, align: "center", hasSearch: true, isReadonly: true, isFixed: true),
                new SheetColumn(columnName: "sMoTa", header: "Nội dung", columnWidth:240, isReadonly: true, isFixed: true),

                // cot nhap
                // Tự chi                    
                new SheetColumn(columnName: "TuChi", header: "Số đề nghị", headerGroup: "Tự chi" , headerGroupIndex: 4, columnWidth:160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "DacThu", header: "Số đặc thù", headerGroup: "Tự chi" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "TangNV", header: "Số tăng do nhiệm vụ", headerGroup: "Tự chi" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "GiamNV", header: "Số giảm do nhiệm vụ", headerGroup: "Tự chi" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),

                // Hàng nhập
                new SheetColumn(columnName: "HangNhap", header: "Số đề nghị", headerGroup: "Hàng nhập" , headerGroupIndex: 4, columnWidth:160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "DacThu_HN", header: "Số đặc thù", headerGroup: "Hàng nhập" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "TangNV_HN", header: "Số tăng do nhiệm vụ", headerGroup: "Hàng nhập" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "GiamNV_HN", header: "Số giảm do nhiệm vụ", headerGroup: "Hàng nhập" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),

                // Hàng mua
                new SheetColumn(columnName: "HangMua", header: "Số đề nghị", headerGroup: "Hàng mua" , headerGroupIndex: 4, columnWidth:160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "DacThu_HM", header: "Số đặc thù", headerGroup: "Hàng mua" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "TangNV_HM", header: "Số tăng do nhiệm vụ", headerGroup: "Hàng mua" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "GiamNV_HM", header: "Số giảm do nhiệm vụ", headerGroup: "Hàng mua" , headerGroupIndex: 4, columnWidth: 160, isReadonly: false, dataType: 1),

                new SheetColumn(columnName: "GhiChu", header: "Ghi chú", columnWidth:360, isReadonly: false),

                // cot khac
                new SheetColumn(columnName: "Id", isHidden: true),
                new SheetColumn(columnName: "sMauSac", isHidden: true),
                new SheetColumn(columnName: "sFontColor", isHidden: true),
                new SheetColumn(columnName: "sFontBold",isHidden: true),

                #endregion

            };

            // neu nhap theo don vi thi chi co tu chi
            if (iLoai == 1)
            {
                items.Remove(items.First(x => x.ColumnName == "HangNhap"));
                items.Remove(items.First(x => x.ColumnName == "DacThu_HN"));
                items.Remove(items.First(x => x.ColumnName == "TangNV_HN"));
                items.Remove(items.First(x => x.ColumnName == "GiamNV_HN"));

                items.Remove(items.First(x => x.ColumnName == "HangMua"));
                items.Remove(items.First(x => x.ColumnName == "DacThu_HM"));
                items.Remove(items.First(x => x.ColumnName == "TangNV_HM"));
                items.Remove(items.First(x => x.ColumnName == "GiamNV_HM"));
                if (iRequest == 0)
                {
                    items.Remove(items.First(x => x.ColumnName == "TangNV"));
                    items.Remove(items.First(x => x.ColumnName == "GiamNV"));
                    items.First(x => x.ColumnName == "TuChi").HeaderGroupIndex = 2;
                    items.First(x => x.ColumnName == "DacThu").HeaderGroupIndex = 2;
                }
                else
                {
                    items.First(x => x.ColumnName == "TuChi").IsReadonly = true;
                    items.First(x => x.ColumnName == "DacThu").IsReadonly = true;
                }
            }
            else
            {
                if (iRequest == 0)
                {
                    items.Remove(items.First(x => x.ColumnName == "TangNV"));
                    items.Remove(items.First(x => x.ColumnName == "GiamNV"));
                    items.Remove(items.First(x => x.ColumnName == "TangNV_HM"));
                    items.Remove(items.First(x => x.ColumnName == "GiamNV_HM"));
                    items.Remove(items.First(x => x.ColumnName == "TangNV_HN"));
                    items.Remove(items.First(x => x.ColumnName == "GiamNV_HN"));
                    items.First(x => x.ColumnName == "TuChi").HeaderGroupIndex = 2;
                    items.First(x => x.ColumnName == "DacThu").HeaderGroupIndex = 2;
                    items.First(x => x.ColumnName == "HangNhap").HeaderGroupIndex = 2;
                    items.First(x => x.ColumnName == "DacThu_HN").HeaderGroupIndex = 2;
                    items.First(x => x.ColumnName == "HangMua").HeaderGroupIndex = 2;
                    items.First(x => x.ColumnName == "DacThu_HM").HeaderGroupIndex = 2;
                }
                else
                {
                    items.First(x => x.ColumnName == "TuChi").IsReadonly = true;
                    items.First(x => x.ColumnName == "DacThu").IsReadonly = true;
                    items.First(x => x.ColumnName == "HangNhap").IsReadonly = true;
                    items.First(x => x.ColumnName == "DacThu_HN").IsReadonly = true;
                    items.First(x => x.ColumnName == "HangMua").IsReadonly = true;
                    items.First(x => x.ColumnName == "DacThu_HM").IsReadonly = true;
                }
            }

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
                {
                    return "#EFEFEF";
                }
            }
            //else if (checkValue(r))
            //{
            //    if (c == "sMauSac")
            //    {
            //        return "#2abd54";
            //    }
            //}

            return base.getCellValue(r, i, c, columnsName);
        }

        //private bool checkValue(DataRow r)
        //{
        //    if (r.Field<double>("TuChi") == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        if (r.Field<double>("TuChi") == r.Field<double>("TuChi_Bql") ||
        //            r.Field<double>("TuChi") == r.Field<double>("TuChi_B2"))
        //        {
        //            return true;
        //        }
        //        else if (r.Field<double>("HangNhap") == r.Field<double>("HangNhap_Bql") ||
        //                 r.Field<double>("HangNhap") == r.Field<double>("HangNhap_B2"))
        //        {
        //            return true;
        //        }
        //        else if (r.Field<double>("HangMua") == r.Field<double>("HangMua_Bql") ||
        //                 r.Field<double>("HangMua") == r.Field<double>("HangMua_B2"))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}
    }

    public class DTKTSheetViewModel
    {
        public DTKTSheetViewModel()
        {
            FilterOptions = new Dictionary<string, string>();
        }

        public SheetViewModel Sheet { get; set; }

        public ChungTuDetailsViewModel ChungTuViewModel { get; set; }

        public Guid Id { get; set; }

        public Dictionary<string, string> FilterOptions { get; set; }
    }
}
