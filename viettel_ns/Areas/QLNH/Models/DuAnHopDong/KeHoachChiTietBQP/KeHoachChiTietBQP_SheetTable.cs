using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using Viettel.Models.KeHoachChiTietBQP;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNH.Models.DuAnHopDong.KeHoachChiTietBQP
{
    public class KeHoachChiTietBQP_SheetTable : SheetTable
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;

        public KeHoachChiTietBQP_SheetTable() { }

        public KeHoachChiTietBQP_SheetTable(string state, Guid? KHTTCP_ID, Guid? KHBQP_ID, Guid? iID_BQuanLyID, Guid? iID_DonViID, bool isUseLastTTCP, Guid? iID_TiGiaID, string sTenNhiemVuChi, string sTenPhongBan, string sTenDonVi, Dictionary<string, string> filters)
        {
            var conditions = new Dictionary<string, string>();
            ColumnsSearch.ToList()
            .ForEach(c =>
            {
                if (c.ColumnName == "state")
                {
                    conditions.Add(c.ColumnName, state);
                }
                else if (c.ColumnName == "KHTTCP_ID")
                {
                    conditions.Add(c.ColumnName, KHTTCP_ID.ToString());
                }
                else if (c.ColumnName == "KHBQP_ID")
                {
                    conditions.Add(c.ColumnName, KHBQP_ID.ToString());
                }
                else if (c.ColumnName == "IsVNDToUSD")
                {
                    conditions.Add(c.ColumnName, "");
                }
                else 
                {
                    conditions.Add(c.ColumnName, filters.ContainsKey(c.ColumnName) ? filters[c.ColumnName] : "");
                }
            });

            fillSheet(state, KHTTCP_ID, KHBQP_ID, iID_BQuanLyID, iID_DonViID, isUseLastTTCP, iID_TiGiaID, sTenNhiemVuChi, sTenPhongBan, sTenDonVi, filters);
        }

        private void fillSheet(string state, Guid? kHTTCP_ID, Guid? kHBQP_ID, Guid? iID_BQuanLyID, Guid? iID_DonViID, bool isUseLastTTCP, Guid? iID_TiGiaID, string sTenNhiemVuChi, string sTenPhongBan, string sTenDonVi, Dictionary<string, string> filters)
        {
            _filters = filters ?? new Dictionary<string, string>();
            NH_KHChiTietBQP_NVCViewModel khBQP_NVCViewModel = _qlnhService.GetDetailKeHoachChiTietBQP(state, kHTTCP_ID, kHBQP_ID, iID_BQuanLyID, iID_DonViID, sTenNhiemVuChi, sTenPhongBan, sTenDonVi, isUseLastTTCP);

            dtChiTiet = ToDataTable<NH_KHChiTietBQP_NVCModel>(khBQP_NVCViewModel.Items);
            dtChiTiet.Columns.Add("IsVNDToUSD", typeof(bool));
            dtChiTiet.Columns.Add("TiGia", typeof(float));

            dtChiTiet_Cu = dtChiTiet.Copy();

            ColumnNameId = "ID";
            ColumnNameParentId = "iID_ParentID";
            ColumnNameIsParent = "bIsHasChild";

            fillData(iID_TiGiaID);
        }

        private void fillData(Guid? iID_TiGiaID)
        {
            updateSumRows(iID_TiGiaID);
            updateColumnIDsKHCTBQP("ID");
            updateColumns();
            updateColumnsParent();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
        }

        protected override void updateColumnsParent()
        {
            //Xác định hàng là hàng cha, con
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                var r = dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                var isParent = Convert.ToString(r[ColumnNameParentId]) == "" || Convert.ToBoolean(r[ColumnNameIsParent]) || Convert.ToBoolean(r["bIsTTCP"]);
                arrLaHangCha.Add(isParent);
            }
        }

        private void updateSumRows(Guid? iID_TiGiaID)
        {
            float tiGia = 0F;
            bool isVNDtoUSD = false;
            var tiGiaChiTiet = _qlnhService.GetTiGiaChiTietByTiGiaId(iID_TiGiaID != null ? iID_TiGiaID.Value : Guid.Empty);

            bool fromUSD = tiGiaChiTiet.Any(x => x.sMaTienTeGoc.ToUpper().Equals("USD"));
            bool fromVND = tiGiaChiTiet.Any(x => x.sMaTienTeGoc.ToUpper().Equals("VND"));

            if (fromUSD)
            {
                isVNDtoUSD = false;
                var toVND = tiGiaChiTiet.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                if (toVND != null)
                {
                    tiGia = toVND.fTiGia;
                }
            }
            else if (fromVND)
            {
                isVNDtoUSD = true;
                var toUSD = tiGiaChiTiet.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                if (toUSD != null)
                {
                    tiGia = toUSD.fTiGia;
                }
            }
            else
            {
                isVNDtoUSD = false;
                var toUSD = tiGiaChiTiet.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("USD")).FirstOrDefault();
                if (toUSD != null)
                {
                    var toVND = tiGiaChiTiet.Where(x => x.sMaTienTeQuyDoi.ToUpper().Equals("VND")).FirstOrDefault();
                    if (toVND != null && toUSD.fTiGia != 0)
                    {
                        tiGia = toVND.fTiGia / toUSD.fTiGia;
                    }
                }
            }
            float money;

            //Tính tiền theo tỉ giá
            for (int i = 0; i <= dtChiTiet.Rows.Count - 1; i++)
            {
                dtChiTiet.Rows[i]["IsVNDToUSD"] = isVNDtoUSD;
                dtChiTiet.Rows[i]["TiGia"] = tiGia;
                if (isVNDtoUSD)
                {
                    money = Convert.ToString(dtChiTiet.Rows[i]["fGiaTriBQP_VND"]) != "" ? float.Parse(dtChiTiet.Rows[i]["fGiaTriBQP_VND"].ToString()) : 0F;
                    dtChiTiet.Rows[i]["fGiaTriBQP_USD"] = (money * tiGia).ToString("0.00" + new string('#', 400), new CultureInfo("en-US"));
                }
                else
                {
                    money = Convert.ToString(dtChiTiet.Rows[i]["fGiaTriBQP_USD"]) != "" ? float.Parse(dtChiTiet.Rows[i]["fGiaTriBQP_USD"].ToString()) : 0F;
                    dtChiTiet.Rows[i]["fGiaTriBQP_VND"] = (money * tiGia).ToString("0", new CultureInfo("en-US"));
                }
            }
        }

        protected void updateColumnIDsKHCTBQP(string columns)
        {
            _arrDSMaHang = new List<string>();
            var fields = columns.Split(',').ToList();
            var numRow = dtChiTiet.Rows.Count;
            dtChiTiet.AsEnumerable()
                .ToList()
                .ForEach(r =>
                {
                    var id = fields.Select(x => r[x] == null ? string.Empty : r[x].ToString()).Join("_");
                    if (numRow == 1 && (id == "" || id == null))
                    {
                        id = Guid.Empty.ToString();
                    }
                    _arrDSMaHang.Add(id);
                });
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties();

            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            List<SheetColumn> list = new List<SheetColumn>()
            {
                // cot fix
                new SheetColumn(columnName: "sMaThuTu", header: "STT", columnWidth: 100, align: "left", isReadonly: true),
                new SheetColumn(columnName: "sTenNhiemVuChi", header: "Tên chương trình, nhiệm vụ chi", hasSearch: true, columnWidth: 300, align: "left", isReadonly: false),
                new SheetColumn(columnName: "sTenPhongBan", header: "B Quản lý", hasSearch: true, columnWidth: 220, align: "left", isReadonly: true),
                new SheetColumn(columnName: "sTenDonVi", header: "Đơn vị", hasSearch: true, columnWidth: 220, align: "left", dataType: 3, isReadonly: false),
                new SheetColumn(columnName: "fGiaTriTTCP_USD", header: "Giá trị TTCP phê duyệt (USD)", columnWidth: 200, align: "right", isReadonly: true, dataType: 1),
                new SheetColumn(columnName: "fGiaTriBQP_USD", header: "Giá trị BQP phê duyệt (USD)", columnWidth: 200, align: "right", isReadonly: false, dataType: 1),
                new SheetColumn(columnName: "fGiaTriBQP_VND", header: "Giá trị BQP phê duyệt (VND)", columnWidth: 200, align: "right", isReadonly: false, dataType: 1),

                // cot khac
                new SheetColumn(columnName: "ID", isHidden: true),
                new SheetColumn(columnName: "iID_BQuanLyID", isHidden: true),
                new SheetColumn(columnName: "bIsTTCP", isHidden: true),
                new SheetColumn(columnName: "iID_KHTTTTCP_NhiemVuChiID", isHidden: true),
                new SheetColumn(columnName: "iID_ParentID", isHidden: true),
                new SheetColumn(columnName: "iID_DonViID", isHidden: true),
                new SheetColumn(columnName: "iID_MaDonVi", isHidden: true),
                new SheetColumn(columnName: "bIsHasChild", isHidden: true),
                new SheetColumn(columnName: "bIsAction", isHidden: true),
                new SheetColumn(columnName: "IsVNDToUSD", isHidden: true, hasSearch: true),
                new SheetColumn(columnName: "TiGia", isHidden: true),
                new SheetColumn(columnName: "isAdd", isHidden: true),
                new SheetColumn(columnName: "state", isHidden: true, hasSearch: true),
                new SheetColumn(columnName: "KHTTCP_ID", isHidden: true, hasSearch: true),
                new SheetColumn(columnName: "KHBQP_ID", isHidden: true, hasSearch: true),
                new SheetColumn(columnName: "isUseLastTTCP", isHidden: true, hasSearch: true),
            };

            return list;
        }
    }
}