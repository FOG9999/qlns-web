using System;
using System.Collections.Generic;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;

namespace Viettel.Models.QLNH
{
    public class NHDAThongTinHopDongViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<NH_DA_HopDongModel> Items { get; set; }
    }

    public class NH_DA_HopDongModel : NH_DA_HopDong
    {
        public Guid iID_BQP { get; set; }
        public Guid iID_MaPhongBan { get; set; }
        public string sTenDonVi { get; set; }
        public string sTen { get; set; }
        public string sSoKeHoachBQP { get; set; }
        public string sTenDuAn { get; set; }
        public string sTenChuongTrinh { get; set; }
        public string sTenLoaiHopDong { get; set; }
        public string sTenNhaThau { get; set; }
        public string sTenGoiThau { get; set; }
        public string sTenTiGia { get; set; }
        public string sSoLanDieuChinh { get; set; }
        public string sDieuChinhTu { get; set; }
        public string sMaTienTeGoc { get; set; }
        public string sLoai {
            get
            {
                switch(iPhanLoai)
                {
                    case 1:
                        return "Dự án, Trong nước";
                    case 2:
                        return "Mua sắm, Trong nước";
                    case 3:
                        return "Dự án, Ngoại thương";
                    case 4:
                        return "Mua sắm, Ngoại thương";
                    default:
                        return string.Empty;
                }
            }
        }
        public string dNgayTaoStr
        {
            get
            {
                return dNgayTao.HasValue ? dNgayTao.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string dNgayHopDongStr
        {
            get
            {
                return dNgayHopDong.HasValue ? dNgayHopDong.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
    }

    public class NHDAThongTinHopDongGiaTriTienTeModel
    {
        public string sGiaTriUSD { get; set; }
        public string sGiaTriVND { get; set; }
        public string sGiaTriEUR { get; set; }
        public string sGiaTriNgoaiTeKhac { get; set; }
    }

    public class NHDAThongTinHopDongImportModel
    {
        public string sSTT { get; set; }
        public string sTenHopDong { get; set; }
        public bool IsTenHopDongWrong { get; set; }
        public string sSoHopDong { get; set; }
        public bool IsSoHopDongWrong { get; set; }
        public string sNgayKiHopDong { get; set; }
        public bool IsNgayKiHopDongWrong { get; set; }
        public string sMaLoaiHopDong { get; set; }
        public Guid? iID_LoaiHopDong { get; set; }
        public bool IsMaLoaiHopDongWrong { get; set; }
        public string sMaNhaThau { get; set; }
        public Guid? iID_NhaThau { get; set; }
        public bool IsMaNhaThauWrong { get; set; }
        public string sThoiGianThucHienTu { get; set; }
        public bool IsThoiGianThucHienTuWrong { get; set; }
        public string sThoiGianThucHienDen { get; set; }
        public bool IsThoiGianThucHienDenWrong { get; set; }
        public string sThoiGianThucHien { get; set; }
        public bool IsThoiGianThucHienWrong { get; set; }
        public string sErrorMessage { get; set; }
        public bool IsDataWrong { get; set; }
    }
}
