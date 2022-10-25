using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMLoaiDuToan_SheetTable : SheetTable
    {
        private readonly IDanhMucService _dmService = DanhMucService.Default;
        public DMLoaiDuToan_SheetTable()
        {
            dtChiTiet = new DataTable();
        }

        public DMLoaiDuToan_SheetTable(Dictionary<string, string> aListFilter, string sUserName)
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
                    listFilter.Add(item.ColumnName, aListFilter[item.ColumnName].Trim());
                }
                else
                {
                    listFilter.Add(item.ColumnName, "");
                }
            }
            LoadDataView(aListFilter, sUserName);
        }

        private void LoadDataView(Dictionary<string, string> aListFilter, string sUserName)
        {
            if(aListFilter != null && aListFilter.Any())
            {
                _filters = aListFilter;
            }
            else
            {
                _filters = new Dictionary<string, string>();
            }
            var listModel = _dmService.GetListDMLoaiDuToan(sUserName);
            if (aListFilter != null && aListFilter.Any())
            {
                foreach (var item in aListFilter)
                {
                    if (item.Key == "sMaLoaiDuToan" && !string.IsNullOrEmpty(item.Value))
                    {
                        listModel = listModel.Where(x => x.sMaLoaiDuToan.Contains(item.Value)).ToList();
                    }

                    if (item.Key == "sTenLoaiDuToan" && !string.IsNullOrEmpty(item.Value))
                    {
                        listModel = listModel.Where(x => x.sTenLoaiDuToan.Contains(item.Value)).ToList();
                    }
                }
            }

            if (listModel != null && listModel.Any())
            {
                dtChiTiet = listModel.ToDataTable<DM_LoaiDuToan>();
                dtChiTiet.Columns.Add("bLaHangCha");
                dtChiTiet.Columns.Add("bConLai");
                dtChiTiet.Columns.Add("STT");
                var count = 1;
                foreach (DataRow item in dtChiTiet.Rows)
                {
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
                var id = item["iID_LoaiDuToan"];
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
                    
                    new SheetColumn(columnName: "sMaLoaiDuToan", header: "Mã loại dự toán", columnWidth:15, align: "left", hasSearch: true),
                    new SheetColumn(columnName: "sTenLoaiDuToan", header: "Tên loại dự toán", columnWidth:30, align: "left", isReadonly: false, hasSearch: true),
                    new SheetColumn(columnName: "sGhiChu", header: "Ghi chú", columnWidth:55, isReadonly: false),

                  // Cột ẩn hiển thị chỉ để lưu giá trị
                    new SheetColumn(columnName: "STT", header: "STT", columnWidth:100, align: "center", isReadonly: false, isFixed: true,isHidden:true),
                    new SheetColumn(columnName:"iID_LoaiDuToan",isHidden:true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                    new SheetColumn(columnName:"bLaHangCha",isHidden:true),
                    new SheetColumn(columnName:"bConLai",isHidden:true)
                };

            #endregion

            return list;
        }
    }
}