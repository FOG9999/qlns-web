using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class KeHoachVonNamDeXuat_Import_SheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;
        public int iNamKeHoach { get; set; }
        public string lstDuAnID { get; set; }

        public KeHoachVonNamDeXuat_Import_SheetTable()
        {

        }

        public KeHoachVonNamDeXuat_Import_SheetTable(DataTable datatableData,int iNamLamViec, int iNamKeHoach,bool bIsImport, Guid iID_KeHoachVonNamDeXuatID, string listIdDuAns , Dictionary<string, string> query)
        {
            this.iNamKeHoach = iNamKeHoach;
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });
            if (bIsImport)
            {
                DataTable dataImport = _qLVonDauTuService.GetListKHVonNamDeXuatChiTietById(iID_KeHoachVonNamDeXuatID.ToString(), new Guid().ToString(), listIdDuAns, filters);

                fillSheet(dataImport, iNamLamViec, filters);
            }
            else
            {
                fillSheet(datatableData, iNamLamViec, filters);
            }

        }
     

        private void fillSheet(DataTable datatableData, int iNamLamViec, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();

            dtChiTiet = datatableData;
            if(dtChiTiet == null)
            {
                dtChiTiet = new DataTable();
            }
            dtChiTiet_Cu = dtChiTiet.Copy();

            System.Data.DataColumn dc = new DataColumn("bLaHangCha", typeof(bool));
            dc.DefaultValue = false;
            dtChiTiet.Columns.Add(dc);

            System.Data.DataColumn dc1 = new DataColumn("iID_KeHoachVonNamDexuatChiTietID", typeof(Guid));
            dc1.DefaultValue = Guid.Empty;
            dtChiTiet.Columns.Add(dc1);

            #region Lay KHVN chi tiet ID
            ColumnNameId = "iID_KeHoachVonNamDexuatChiTietID";
            //ColumnNameId = "iID_DuAnID";
            //ColumnNameParentId = "IdParent";
            //ColumnNameIsParent = "IsParent";
            #endregion

            fillData();
        }

        private void fillData()
        {
            //updateSummaryRows();
            //insertSummaryRows("sCapPheDuyet");
            //UpdateColumIDsKH5NamDXChiTiet("Id");
            //updateColumnIDs("iID_DuAnID");
            updateColumnIDs("iID_KeHoachVonNamDexuatChiTietID");
            updateColumns();
            //updateColumnsParent1();
            //updateColumnsParent();
            updateCellsEditable();
            updateCellsValue();
            updateChanges();

        }

        protected override IEnumerable<SheetColumn> GetColumns()
        {
            var cNameNamLuyKe = "Lũy kế vốn thực hiện đến cuối năm " + (this.iNamKeHoach - 1);
            var cNameKeHoachVonNam = "Kế hoạch vốn năm " + (this.iNamKeHoach - 1);
            var cNameUocThucHien = "Ước thực hiện năm " + (this.iNamKeHoach - 1);
            var cNameNhuCauVonNam = "Nhu cầu vốn năm " + this.iNamKeHoach;

            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    //new SheetColumn(columnName: "STT", header: "STT", columnWidth:50, align: "left", hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sMaDuAn", header: "Mã dự án", columnWidth:200, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sTenDuAn", header: "Tên dự án", columnWidth:200, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sLoaiDuAn", header: "Loại dự án", columnWidth:80, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sChuDauTu", header: "Chủ đầu tư", columnWidth:150, align: "left", hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "fTongMucDauTuDuocDuyet", header: "Tổng mức đầu tư được duyệt", columnWidth:100, align: "right", dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fLuyKeVonNamTruoc", header: cNameNamLuyKe, columnWidth:100, align: "right", dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fKeHoachVonDuocDuyetNamNay", header: "Kế hoạch vốn được giao", headerGroup: cNameKeHoachVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fVonKeoDaiCacNamTruoc", header: "Vốn kéo dài các năm trước", headerGroup: cNameKeHoachVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fUocThucHien", header: cNameUocThucHien, columnWidth:170, align: "right", hasSearch: false, dataType: 1, isReadonly: false),

                    new SheetColumn(columnName: "fThuHoiVonUngTruoc", header: "Thu hồi vốn ứng trước", headerGroup: cNameNhuCauVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: false),

                    new SheetColumn(columnName: "fThanhToan", header: "Thanh toán", headerGroup: cNameNhuCauVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: false),
                    
                    // cot khac
                    new SheetColumn(columnName: "iID_KeHoachVonNamDeXuatChiTietID", isHidden: true),
                    new SheetColumn(columnName: "iID_DuAnID", isHidden: true),
                    new SheetColumn(columnName: "iID_KeHoachVonNamDeXuatID", isHidden: true),
                    new SheetColumn(columnName: "iID_DonViQuanLyID", isHidden: true),
                    new SheetColumn(columnName: "iID_NguonVonID", isHidden: true),
                    //new SheetColumn(columnName: "Level", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),
                    new SheetColumn(columnName: "sTenDonViQuanLy", align: "left", isFixed: true, hasSearch: true,isHidden: true),
                    new SheetColumn(columnName: "sThoiGianThucHien", align: "center", isFixed: true, hasSearch: true, isReadonly: true,isHidden:true),

                };

            #endregion

            list = list.OrderByDescending(item => !item.IsHidden).ToList();
            return list;
        }
    }
}