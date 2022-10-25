using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using Viettel.Models.QLVonDauTu;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class KeHoachVonNamDuocDuyetChiTietSheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private bool _isModified;
        private bool _isDetals;
        private Guid? _idVonNamDx = Guid.Empty;

        public KeHoachVonNamDuocDuyetChiTietSheetTable() { }

        public KeHoachVonNamDuocDuyetChiTietSheetTable(string iDPhanBoVonID, string iIdParent, int iNamLamViec, Dictionary<string, string> query, string idVonNamDx, bool isDetail)
        {
            var filters = new Dictionary<string, string>();

            _isModified = !string.IsNullOrEmpty(iIdParent) ? true : false;
            _isDetals = isDetail ? true : false;
            _idVonNamDx = !string.IsNullOrEmpty(idVonNamDx) ? Guid.Parse(idVonNamDx) : Guid.Empty;

            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(iDPhanBoVonID, iNamLamViec, filters);
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {

            System.Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                    {
                        var value = dr[column.ColumnName];
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            pro.SetValue(obj, value, null);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
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

        private void fillSheet(string iIdPhanBoVonId, int iNamLamViec, Dictionary<string, string> filters)
        {
            _filters = filters ?? new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(iIdPhanBoVonId))
            {
                DataTable dtVoucherDetails = _qLVonDauTuService.GetListKHVonNamDuocDuyetChiTietById(iIdPhanBoVonId, _idVonNamDx.ToString(), iNamLamViec, _filters);

                if (dtVoucherDetails != null && dtVoucherDetails.Rows.Count > 0)
                {
                    List<VDTKHVPhanBoVonDuocDuyetChiTietViewModel> lstDetail = ConvertDataTable<VDTKHVPhanBoVonDuocDuyetChiTietViewModel>(dtVoucherDetails).OrderBy(x => x.Loai).ToList();

                    List<VDTKHVPhanBoVonDuocDuyetChiTietViewModel> results = lstDetail.GroupBy(x => new { x.iID_DuAnID, x.iID_LoaiCongTrinhID }).Select(grp => grp.LastOrDefault()).ToList();

                    dtChiTiet = ToDataTable(results.OrderBy(x => x.Loai).ToList());
                }

                if (dtChiTiet == null) dtChiTiet = new DataTable();
            }
            else
            {
                dtChiTiet = new DataTable();
            }

            System.Data.DataColumn dc = new DataColumn("bLaHangCha", typeof(bool));
            dc.DefaultValue = false;
            dtChiTiet.Columns.Add(dc);

            dtChiTiet_Cu = dtChiTiet.Copy();
            ColumnNameId = "iID_DuAnID";

            fillData();
        }

        private void fillData()
        {
            updateColumnIDs("iID_DuAnID");
            updateColumns();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();
        }

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var listColumn = new List<SheetColumn>()
            {
                new SheetColumn(columnName: "sTenDuAn", header: "Tên dự án", columnWidth:250, align: "left", hasSearch: true, isReadonly: true),
                new SheetColumn(columnName: "sLoaiDuAn", header: "Loại dự án", columnWidth:150, align: "left", hasSearch: true, isReadonly: true),
                new SheetColumn(columnName: "sLoaiCongTrinh", header: "Loại công trình", columnWidth: 120, align: "left", hasSearch: false, isReadonly: true),
                new SheetColumn(columnName: "sTenDonViThucHienDuAn", header: "Đơn vị thực hiện dự án", columnWidth:250, align: "left", hasSearch: true, isReadonly: true),

                new SheetColumn(columnName: "LNS", header: "LNS", columnWidth:100, align: "center", hasSearch: false, isReadonly: _isDetals ),
                new SheetColumn(columnName: "L", header: "L", columnWidth:60, align: "center", hasSearch: false, isReadonly: _isDetals),
                new SheetColumn(columnName: "K", header: "K", columnWidth:60, align: "center", hasSearch: false, isReadonly: _isDetals),
                new SheetColumn(columnName: "M", header: "M", columnWidth:60, align: "center", hasSearch: false, isReadonly: _isDetals),
                new SheetColumn(columnName: "TM", header: "TM", columnWidth:60, align: "center", hasSearch: false, isReadonly: _isDetals),
                new SheetColumn(columnName: "TTM", header: "TTM", columnWidth:60, align: "center", hasSearch: false, isReadonly: _isDetals),
                new SheetColumn(columnName: "NG", header: "NG", columnWidth:60, align: "center", hasSearch: false, isReadonly: _isDetals),

                new SheetColumn(columnName: "fCapPhatTaiKhoBac", header: "Rút dự toán tại KBNN", columnWidth:100, align: "right", dataType: 1, hasSearch: true, isReadonly: _isModified),
                new SheetColumn(columnName: "fCapPhatTaiKhoBacSauDC", header: "Rút dự toán tại KBNN(Sau Điều chỉnh)", columnWidth:100, align: "right", dataType: 1, hasSearch: true, isReadonly: false, isHidden: !_isModified),

                new SheetColumn(columnName: "fCapPhatBangLenhChi", header: "Cấp bằng lệnh chi tiền", columnWidth:100, align: "right", dataType: 1, hasSearch: true, isReadonly: _isModified),
                new SheetColumn(columnName: "fCapPhatBangLenhChiSauDC", header: "Cấp bằng lệnh chi tiền(Sau điều chỉnh)", columnWidth:100, align: "right", dataType: 1, hasSearch: true, isReadonly: false, isHidden: !_isModified),

                new SheetColumn(columnName: "fGiaTriThuHoiNamTruocKhoBac", header: "Thu hồi năm trước kho bạc", columnWidth:100, align: "right",dataType: 1, hasSearch: true, isReadonly: _isModified),
                new SheetColumn(columnName: "fGiaTriThuHoiNamTruocKhoBacSauDC", header: "Thu hồi năm trước kho bạc(Sau điều chỉnh)", columnWidth:100, align: "right", dataType: 1, hasSearch: true, isReadonly: false, isHidden : !_isModified),

                new SheetColumn(columnName: "fGiaTriThuHoiNamTruocLenhChi", header: "Thu hồi năm trước lệnh chi", columnWidth:100, align: "right",dataType: 1, hasSearch: true, isReadonly: _isModified),
                new SheetColumn(columnName: "fGiaTriThuHoiNamTruocLenhChiSauDC", header: "Thu hồi năm trước lệnh chi(Sau điều chỉnh)", columnWidth:100, align: "right", dataType: 1, hasSearch: true, isReadonly: false, isHidden: !_isModified),

                new SheetColumn(columnName: "sGhiChu", header: "Ghi chú", columnWidth:200, align: "left", hasSearch: false, isReadonly: false),

                new SheetColumn(columnName: "iID_DuAnID", header: "Id dự án", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "sMaDuAn", header: "Mã dự án", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "Id Chi Tiết", header: "Id Chi Tiết", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iLoaiDuAn", header: "Loại dự án", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iID_LoaiCongTrinh", header:"Id Loại công trình", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iID_KeHoachVonNam_DuocDuyetID", header:"Id Chứng từ", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iID_KeHoachVonNam_DuocDuyet_ChiTietID", header: "Id Chứng từ chi tiết", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iID_Parent", header: "IdParent", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "modelActive", header: "Model active", isHidden: true, isReadonly: true)
            };

            return listColumn;
        }
    }
}