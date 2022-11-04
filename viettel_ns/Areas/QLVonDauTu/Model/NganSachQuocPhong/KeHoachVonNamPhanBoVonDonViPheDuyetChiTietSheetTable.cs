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
    public class KeHoachVonNamPhanBoVonDonViPheDuyetChiTietSheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        private bool _isModified;
        private Guid? _idVonNamDx = Guid.Empty;

        public KeHoachVonNamPhanBoVonDonViPheDuyetChiTietSheetTable() { }

        public KeHoachVonNamPhanBoVonDonViPheDuyetChiTietSheetTable(string iDPhanBoVonID, string iIdParent, int iNamLamViec, Dictionary<string, string> query, string idVonNamDx)
        {
            var filters = new Dictionary<string, string>();

            _isModified = !string.IsNullOrEmpty(iIdParent) ? true : false;
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
                DataTable dtVoucherDetails = _qLVonDauTuService.GetListKHVonNamPhanBoVonDonViPheDuyetChiTietById(iIdPhanBoVonId, _idVonNamDx.ToString(), iNamLamViec, _filters);

                if (dtVoucherDetails != null && dtVoucherDetails.Rows.Count > 0)
                {
                    //List<VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel> lstDetail = ConvertDataTable<VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel>(dtVoucherDetails).OrderBy(x => x.Loai).ToList();

                    //List<VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel> results = ConvertDataTable<VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel>(dtVoucherDetails).OrderBy(x => x.Loai).ToList();

                    List<VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel> results = ConvertDataTable<VDTKHVPhanBoVonDonViPheDuyetChiTietViewModel>(dtVoucherDetails).OrderBy(x => x.Loai).ToList();

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
                new SheetColumn(columnName: "sTenDuAn", header: "Tên dự án", columnWidth:248, align: "left", hasSearch: true, isReadonly: true),
                new SheetColumn(columnName: "sLoaiDuAn", header: "Loại dự án", columnWidth:198, align: "left", hasSearch: true, isReadonly: true),
                new SheetColumn(columnName: "sTenLoaiCongTrinh", header: "Loại công trình", columnWidth:198, align: "left", hasSearch: false, isReadonly: true),
                new SheetColumn(columnName: "sTenDonViThucHienDuAn", header: "Đơn vị thực hiện dự án", columnWidth:298, align: "left", hasSearch: true, isReadonly: true),
                new SheetColumn(columnName: "fGiaTriDeNghi", header: "Giá trị đề nghị", columnWidth:198, align: "right", dataType: 1, hasSearch: false, isReadonly: true ),
                new SheetColumn(columnName: "fGiaTriPhanBo", header: "Kế hoạch phân bổ năm", columnWidth:198, align: "right", dataType: 1, hasSearch: false, isReadonly: false ),
                new SheetColumn(columnName: "sGhiChu", header: "Ghi chú", columnWidth:200, align: "left", hasSearch: false, isReadonly: false),

                new SheetColumn(columnName: "fGiaTriThuHoi", header: "Giá trị phân bổ thu hồi", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "iID_DonViTienTeID", header: "ID đơn vị tiền tệ", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "iID_TienTeID", header: "ID tiền tệ", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "fTiGiaDonVi", header: "ID tiền tệ", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "fTiGia", header: "Tỷ giá", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "iID_LoaiCongTrinh", header: "ID loại công trình", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "iId_Parent", header: "ID parent", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "bActive", header: "Active", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "ILoaiDuAn", header: "ID loại dự án", isHidden: true, isReadonly: true ),
                new SheetColumn(columnName: "iID_DuAnID", header: "Id dự án", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iID_PhanBoVon_DonVi_PheDuyet_ID", header: "Phân bổ vốn đơn vị phê duyệt ID", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iID_LoaiCongTrinhID", header:"Id Loại công trình", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "iID_PhanBoVon_DonVi_PheDuyet_ChiTiet_ID", header: "Phân bổ vốn đơn vị phê duyệt chi tiết ID", isHidden: true, isReadonly: true),
                new SheetColumn(columnName: "modelActive", header: "Model active", isHidden: true, isReadonly: true)
            };

            return listColumn;
        }
    }
}