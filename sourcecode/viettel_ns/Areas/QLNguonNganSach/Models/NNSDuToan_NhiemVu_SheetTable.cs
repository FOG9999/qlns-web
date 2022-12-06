using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Models.QLNguonNganSach;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NNSDuToan_NhiemVu_SheetTable : SheetTable
    {
        private readonly IQLNguonNganSachService _qLNguonNSService = QLNguonNganSachService.Default;
        public NNSDuToan_NhiemVu_SheetTable()
        {
            dtChiTiet = new DataTable();
        }

        public NNSDuToan_NhiemVu_SheetTable(Dictionary<string, string> aListFilter, string iID_DuToan, string sUserName)
        {
            dtChiTiet = new DataTable();
            var listFilter = new Dictionary<string, string>();
            if (aListFilter == null || !aListFilter.Any())
            {
                aListFilter = new Dictionary<string, string>();
            }

            foreach (var item in ColumnsSearch)
            {
                if (aListFilter.ContainsKey(item.ColumnName))
                {
                    listFilter.Add(item.ColumnName, aListFilter[item.ColumnName]);
                }
                else
                {
                    listFilter.Add(item.ColumnName, "");
                }
            }
            LoadDataView(aListFilter, iID_DuToan, sUserName);
        }

        private void LoadDataView(Dictionary<string, string> aListFilter, string iID_DuToan, string sUserName)
        {
            if (aListFilter != null && aListFilter.Any())
            {
                _filters = aListFilter;
            }
            else
            {
                _filters = new Dictionary<string, string>();
            }
            var listModel = _qLNguonNSService.GetListDuToan_NhiemVu_ByIdDuToan(iID_DuToan, sUserName);
            if (aListFilter != null && aListFilter.Any())
            {
                foreach (var item in aListFilter)
                {
                    if (item.Key == "sNhiemVu" && !string.IsNullOrEmpty(item.Value))
                    {
                        listModel = listModel.Where(x => x.sNhiemVu.Contains(item.Value)).ToList();
                    }
                }
            }

            if (listModel != null && listModel.Any())
            {
                dtChiTiet = listModel.ToDataTable<NNS_DuToan_NhiemVu_ViewGridModel>();
                dtChiTiet.Columns.Add("bLaHangCha");
                dtChiTiet.Columns.Add("bConLai");
                dtChiTiet.Columns.Add("STT");
                var count = 1;
                foreach (DataRow item in dtChiTiet.Rows)
                {
                    item["sSoTien"] = string.Format("{0:0,0}", item["sSoTien"]);
                    item["bLaHangCha"] = false;
                    item["bConlai"] = true;
                    item["STT"] = count;

                    count++;
                }
            }
            UpdateData();
        }

        private void UpdateData()
        {
            SetIdKeyData();
            updateColumns();
            updateCellsEditable();
            updateChanges();
            updateCellsValue();
        }

        private void SetIdKeyData()
        {
            _arrDSMaHang = new List<string>();
            foreach (DataRow item in dtChiTiet.Rows)
            {
                var id = item["iID_NhiemVu"];
                if (id == null)
                {
                    _arrDSMaHang.Add(null);
                    continue;
                }
                _arrDSMaHang.Add(id.ToString());
            }
        }

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                  // Cột hiển thị trên view
                    
                    new SheetColumn(columnName: "sNhiemVu", header: "Nhiệm vụ", columnWidth:650, align: "left", isReadonly: false, hasSearch: true),
                    new SheetColumn(columnName: "sSoTien", header: "Số tiền", columnWidth:250, align: "right", isReadonly: true, dataType: 1),

                  // Cột ẩn hiển thị chỉ để lưu giá trị
                    new SheetColumn(columnName:"STT",isHidden:true),
                    new SheetColumn(columnName:"iID_NhiemVu",isHidden:true),
                    new SheetColumn(columnName:"iID_MaChungTu",isHidden:true),
                    new SheetColumn(columnName:"iID_DuToan",isHidden:true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                    new SheetColumn(columnName:"bLaHangCha",isHidden:true),
                    new SheetColumn(columnName:"bConLai",isHidden:true),
                    new SheetColumn(columnName:"sMaNoiDungChi", isHidden: true),
                    new SheetColumn(columnName:"sTenNoiDungChi", isHidden: true)
                };

            #endregion

            return list;
        }
    }
}