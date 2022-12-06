using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNH.Models.DuAnHopDong.KeHoachTongTheTTCP
{
    public class KeHoachTongTheTTCP_ChiTIet_SheetTable : SheetTable
    {
        private readonly IQLNHService _qlnhService = QLNHService.Default;
        private Guid? _idKhtt = Guid.NewGuid();
        public KeHoachTongTheTTCP_ChiTIet_SheetTable()
        {

        }

        public KeHoachTongTheTTCP_ChiTIet_SheetTable(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan, Dictionary<string, string> filters)
        {
            var conditions = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    if(c.ColumnName == "state")
                    {
                        conditions.Add(c.ColumnName, state);
                    }
                    else if (c.ColumnName == "KHTTCP_ID")
                    {
                        conditions.Add(c.ColumnName, KHTTCP_ID.ToString());
                    }
                    else
                    {
                        conditions.Add(c.ColumnName, filters.ContainsKey(c.ColumnName) ? filters[c.ColumnName] : "");
                    }
                });

            fillSheet(state, KHTTCP_ID, iID_BQuanLyID, sTenNhiemVuChi, sTenPhongBan, conditions);
        }

        #region private methods

        private void fillSheet(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID, string sTenNhiemVuChi, string sTenPhongBan, Dictionary<string, string> filters)
        {
            NH_KHTongTheTTCP_NVCViewModel itemQuery = _qlnhService.GetDetailKeHoachTongTheTTCP(state, KHTTCP_ID, iID_BQuanLyID, sTenNhiemVuChi, sTenPhongBan);
            _filters = filters ?? new Dictionary<string, string>();
            if (itemQuery == null)
            {
                dtChiTiet = new DataTable();
            }
            else dtChiTiet = ToDataTable(itemQuery.Items);
            dtChiTiet_Cu = dtChiTiet.Copy();

            #region Lay XauNoiMa_Cha
            ColumnNameId = "ID";
            ColumnNameParentId = "iID_ParentID";
            ColumnNameIsParent = "bIsHasChild";
            #endregion

            fillData(state, KHTTCP_ID, iID_BQuanLyID);
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

        private void fillData(string state, Guid? KHTTCP_ID, Guid? iID_BQuanLyID)
        {
            updateColumnIDsCTKHTH("ID");
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
                dtChiTiet.Rows[i]["fGiaTri"] = dtChiTiet.Rows[i]["fGiaTri"] != DBNull.Value && dtChiTiet.Rows[i]["fGiaTri"] != null && dtChiTiet.Rows[i]["fGiaTri"].ToString() != ""
                        ? Convert.ToDouble(dtChiTiet.Rows[i]["fGiaTri"]).ToString("0.00", new CultureInfo("en-US")) : "";
                //Xac dinh hang nay co phai la hang cha khong?
                var isParent = Convert.ToString(r[ColumnNameParentId]) == "" || Convert.ToBoolean(r[ColumnNameIsParent]);
                arrLaHangCha.Add(isParent);
            }
        }

        protected void updateColumnIDsCTKHTH(string columns)
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
        #endregion

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    new SheetColumn(columnName: "sMaThuTu", header: "STT", columnWidth: 100, align: "left", hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sTenNhiemVuChi", header: "Tên chương trình, nhiệm vụ chi", columnWidth: 743, align: "left", hasSearch: true, isReadonly: false),
                    new SheetColumn(columnName: "sTenPhongBan", header: "B Quản lý", columnWidth: 500, align: "left", hasSearch: true, dataType: 3, isReadonly: false),
                    new SheetColumn(columnName: "fGiaTri", header: "Giá trị phê duyệt (USD)", columnWidth: 200, align: "right", dataType: 1, format: "2", hasSearch: false, isReadonly: false),
                    
                    // cot khac
                    new SheetColumn(columnName: "ID", isHidden: true),
                    new SheetColumn(columnName: "iID_ParentID", isHidden: true),
                    new SheetColumn(columnName: "bIsHasChild", isHidden: true),
                    new SheetColumn(columnName: "iID_BQuanLyID", isHidden: true),
                    new SheetColumn(columnName: "isAdd", isHidden: true),
                    new SheetColumn(columnName: "state", isHidden: true, hasSearch: true),
                    new SheetColumn(columnName: "KHTTCP_ID", isHidden: true, hasSearch: true),
                };

            #endregion

            return list;
        }
    }
}