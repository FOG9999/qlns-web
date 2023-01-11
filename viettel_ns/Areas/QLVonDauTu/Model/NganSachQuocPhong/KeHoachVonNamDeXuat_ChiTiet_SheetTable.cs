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
    public class KeHoachVonNamDeXuat_ChiTiet_SheetTable : SheetTable
    {
        private readonly IQLVonDauTuService _qLVonDauTuService = QLVonDauTuService.Default;

        public bool isTongHop { get; set; }
        public int iNamKeHoach { get; set; }
        public DateTime dNgayLap { get; set; }
        public string sMaDonViQL { get; set; }
        public int iNguonVonID { get; set; }
        public bool isDieuChinh { get; set; }
        public bool bIsDetail { get; set; }
        public string lstDuAnID { get; set; }
        public string iID_KeHoachVonNamDeXuatID { get; set; }
        public KeHoachVonNamDeXuat_ChiTiet_SheetTable()
        {

        }

        public KeHoachVonNamDeXuat_ChiTiet_SheetTable(string iID_KeHoachVonNamDeXuatID, int iNamKeHoach, DateTime dNgayLap, string sMaDonViQL, int iNguonVonID, bool isDieuChinh, bool bIsDetail, string lstDuAnID, Dictionary<string, string> query)
        {
            this.iNamKeHoach = iNamKeHoach;
            this.dNgayLap = dNgayLap;
            this.sMaDonViQL = sMaDonViQL;
            this.iNguonVonID = iNguonVonID;
            this.isDieuChinh = isDieuChinh;
            this.bIsDetail = bIsDetail;
            this.lstDuAnID = lstDuAnID;
            this.iID_KeHoachVonNamDeXuatID = iID_KeHoachVonNamDeXuatID;
            Guid IdKHVNDX = Guid.Parse(iID_KeHoachVonNamDeXuatID);
            VDT_KHV_KeHoachVonNam_DeXuat itemquery = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(IdKHVNDX);
            if (itemquery != null)
            {
                if (itemquery.iID_TongHopParent != null || itemquery.sTongHop != null)
                {
                    isTongHop = true;
                }
                else
                    isTongHop = false;
            }
            var filters = new Dictionary<string, string>();
            ColumnsSearch.ToList()
                .ForEach(c =>
                {
                    filters.Add(c.ColumnName, query.ContainsKey(c.ColumnName) ? query[c.ColumnName] : "");
                });

            fillSheet(iID_KeHoachVonNamDeXuatID, filters);
        }

        private void fillSheet(string iID_KeHoachVonNamDeXuatID, Dictionary<string, string> filters)
        {

            _filters = filters ?? new Dictionary<string, string>();
            Dictionary<string, List<VDT_DA_DuAn_HangMuc>> dicHangMuc = new Dictionary<string, List<VDT_DA_DuAn_HangMuc>>();
            //dtChiTiet = _qLVonDauTuService.GetListKHVonNamDeXuatChiTietById(iID_KeHoachVonNamDeXuatID, _filters);
            var iID_Parrent = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(Guid.Parse(iID_KeHoachVonNamDeXuatID)).iID_ParentId;
            DataTable data = new DataTable();
            if ((iID_Parrent != null || iID_Parrent != new Guid()) && isDieuChinh)
            {
                 data = _qLVonDauTuService.GetListKHVonNamDeXuatChiTietById(iID_Parrent.ToString(), iID_KeHoachVonNamDeXuatID, lstDuAnID, _filters);

            }
            else
            {
               // if(iID_Parrent != null) this.isDieuChinh = true;
                data = _qLVonDauTuService.GetListKHVonNamDeXuatChiTietById(iID_KeHoachVonNamDeXuatID,(new Guid()).ToString(), lstDuAnID, _filters);
            
            }
            if (data == null)
                data = new DataTable();

            dtChiTiet = data.Clone();

            List<VDT_DA_DuAn_HangMuc> lstHangMuc = _qLVonDauTuService.GetDuAnHangMucByListDuAnID(iID_KeHoachVonNamDeXuatID, lstDuAnID).ToList();
            Dictionary<Guid, VDT_DM_LoaiCongTrinh> dicLoaiCongTrinh = _qLVonDauTuService.GetListDMLoaiCongTrinh().ToDictionary(n => n.iID_LoaiCongTrinh, n => n);
            if (lstHangMuc != null)
                dicHangMuc = lstHangMuc.GroupBy(n => n.iID_DuAnID).ToDictionary(n => n.Key.ToString(), n => n.ToList());
            foreach (DataRow dr in data.Rows)
            {
                //Guid iIdChiTiet = Guid.Parse(Convert.ToString(dr["iID_KeHoachVonNamDeXuatChiTietID"]));
                //if (iIdChiTiet == Guid.Empty && dicHangMuc.ContainsKey(Convert.ToString(dr["iID_DuAnID"])))
                //{
                //    foreach (var itemHangMuc in dicHangMuc[Convert.ToString(dr["iID_DuAnID"])])
                //    {
                //        DataRow drNew = dr;
                //        drNew["sTenLoaiCongTrinh"] = itemHangMuc.sTenHangMuc;
                //        drNew["iID_LoaiCongTrinh"] = itemHangMuc.iID_LoaiCongTrinhID;
                //        dtChiTiet.Rows.Add(drNew.ItemArray);
                //    }
                //}
                //else
                //{
                Guid iIdLoaiCongTrinhId = Guid.Empty;
                if (Guid.TryParse(Convert.ToString(dr["iID_LoaiCongTrinh"]), out iIdLoaiCongTrinhId))
                {
                    if (iIdLoaiCongTrinhId != Guid.Empty && dicLoaiCongTrinh.ContainsKey(iIdLoaiCongTrinhId))
                    {
                        DataRow drNew = dr;
                        drNew["sTenLoaiCongTrinh"] = dicLoaiCongTrinh[iIdLoaiCongTrinhId].sTenLoaiCongTrinh;
                        //drNew["iID_DuAn_HangMucID"] = lstHangMuc.Where(h => h.)
                        dtChiTiet.Rows.Add(drNew.ItemArray);
                        continue;
                    }
                }
                dtChiTiet.Rows.Add(dr.ItemArray);
                //}
            }

            System.Data.DataColumn dc = new DataColumn("bLaHangCha", typeof(bool));
            dc.DefaultValue = false;
            dtChiTiet.Columns.Add(dc);

            dtChiTiet_Cu = dtChiTiet.Copy();

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
            var cNameNamLuyKe = "Lũy kế vốn thực hiện đến cuối năm " + (this.iNamKeHoach - 2);
            var cNameKeHoachVonNam = "Kế hoạch vốn năm " + (this.iNamKeHoach - 1);
            var cNameNhuCauVonNam = "Nhu cầu vốn năm " + this.iNamKeHoach;
            var cNameVonDaBoTriHetNam = "Lũy kế vốn đã được bố trí hết năm " + (this.iNamKeHoach - 1);
            var cNameUocThucHien = "Ước thực hiện năm " + (this.iNamKeHoach - 1);
            var cNameUocThucHienDC = "Ước thực hiện năm " + (this.iNamKeHoach - 1) + " (Sau điều chỉnh)";
            var iID_Parrent = _qLVonDauTuService.GetKeHoachVonNamDeXuatById(Guid.Parse(iID_KeHoachVonNamDeXuatID)).iID_ParentId;
            if (iID_Parrent != null) isDieuChinh = true;


            var list = new List<SheetColumn>();

            #region columns

            list = new List<SheetColumn>()
                {
                    // cot fix
                    //new SheetColumn(columnName: "STT", header: "STT", columnWidth:50, align: "left", hasSearch: false, isReadonly: true),
                    new SheetColumn(columnName: "sMaDuAn", header: "Mã dự án", columnWidth:200, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sTenDuAn", header: "Tên dự án", columnWidth:200, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    //Hidden Don vi thuc hien du An add DonViQuanLY
                    new SheetColumn(columnName: "sTenDonViQuanLy", header: "Đơn vị", columnWidth:200, align: "left", isFixed: true, hasSearch: true, isReadonly: true,isHidden: false),
                    new SheetColumn(columnName: "sLoaiDuAn", header: "Loại dự án", columnWidth:80, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sThoiGianThucHien", header: "Thời gian thực hiện", columnWidth:80, align: "center", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sTenLoaiCongTrinh", header: "Loại công trình", columnWidth:80, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sCapPheDuyet", header: "Cấp phê duyệt", columnWidth:80, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "sChuDauTu", header: "Chủ đầu tư", columnWidth:150, align: "left", isFixed: true, hasSearch: true, isReadonly: true),
                    new SheetColumn(columnName: "fTongMucDauTuDuocDuyet", header: "Tổng mức đầu tư được duyệt", columnWidth:100, align: "right", dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fKeHoachTrungHanDuocDuyet", header: "KHTH được duyệt", columnWidth:100, align: "right", dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fLuyKeVonNamTruoc", header: cNameNamLuyKe, columnWidth:100, align: "right", dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fLuyKeVonDaBoTriHetNam", header: cNameVonDaBoTriHetNam, columnWidth:120, align: "right", dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fKeHoachVonDuocDuyetNamNay", header: "Kế hoạch vốn được giao", headerGroup: cNameKeHoachVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: true),
                    new SheetColumn(columnName: "fVonKeoDaiCacNamTruoc", header: "Vốn kéo dài các năm trước", headerGroup: cNameKeHoachVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: bIsDetail),

                    new SheetColumn(columnName: "fUocThucHien", header: cNameUocThucHien, columnWidth:170, align: "right", hasSearch: false, dataType: 1, isReadonly: bIsDetail),
                    new SheetColumn(columnName: "fUocThucHienDC", header: cNameUocThucHienDC, columnWidth:170, align: "right", hasSearch: false, dataType: 1, isReadonly: bIsDetail, isHidden:!isDieuChinh),

                    //new SheetColumn(columnName: "fLuyKeVonDaBoTriHetNam", header: cNameVonDaBoTriHetNam, columnWidth:120, align: "right", dataType: 1, isReadonly: true),

                    new SheetColumn(columnName: "fThuHoiVonUngTruoc", header: "Thu hồi vốn ứng trước", headerGroup: cNameNhuCauVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: bIsDetail),
                    new SheetColumn(columnName: "fThuHoiVonUngTruocDC", header: "Thu hồi vốn ứng trước (Sau điều chỉnh)", headerGroup: cNameNhuCauVonNam, headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: bIsDetail, isHidden:!isDieuChinh),

                    new SheetColumn(columnName: "fThanhToan", header: "Thanh toán", headerGroup: "Nhu cầu vốn năm 2020", headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: bIsDetail),
                    new SheetColumn(columnName: "fThanhToanDC", header: "Thanh toán (Sau điều chỉnh)", headerGroup: "Nhu cầu vốn năm 2020", headerGroupIndex: 2, columnWidth:120, align: "right", dataType: 1, isReadonly: bIsDetail, isHidden:!isDieuChinh),
                    
                    // cot khac
                     new SheetColumn(columnName: "sTen", header: "Đơn vị thực hiện dự án", columnWidth:200, align: "left", isFixed: true, hasSearch: false, isReadonly: true, isHidden: true),

                    new SheetColumn(columnName: "iID_KeHoachVonNamDeXuatChiTietID", isHidden: true),
                    new SheetColumn(columnName: "iID_DuAnID", isHidden: true),
                    new SheetColumn(columnName: "iID_KeHoachVonNamDeXuatID", isHidden: true),
                    new SheetColumn(columnName: "iID_DonViQuanLyID", isHidden: true),
                    new SheetColumn(columnName: "iID_DonViThucHienDuAn", isHidden: true),
                    new SheetColumn(columnName: "iID_NguonVonID", isHidden: true),
                    new SheetColumn(columnName: "iID_LoaiCongTrinh", isHidden: true),
                    new SheetColumn(columnName: "iID_DuAn_HangMucID", isHidden: true),
                    new SheetColumn(columnName: "Level", isHidden: true),
                    new SheetColumn(columnName: "sMauSac", isHidden: true),
                    new SheetColumn(columnName: "sFontColor", isHidden: true),
                    new SheetColumn(columnName: "sFontBold",isHidden: true),

                };

            #endregion

            list = list.OrderByDescending(item => !item.IsHidden).ToList();
            return list;
        }
    }
}