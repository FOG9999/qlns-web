using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;
using Viettel.Extensions;
using static Viettel.Extensions.Constants;

namespace Viettel.Models.QLVonDauTu
{
    public class GiaiNganThanhToanViewModel : VDT_TT_DeNghiThanhToan
    {
        public Guid iID_ChuDauTuID { get; set; }
        public string sNguonVon { get; set; }
        public string sLoaiNguonVon { get; set; }
        public string sDonViQuanLy { get; set; }
        public string sSoHopDong { get; set; }
        public string sTenDuAn { get; set; }
        public string sChuDauTu { get; set; }
        public string sCoQuanThanhToan { get; set; }
        public string sLoaiThanhToan { get; set; }
        public string sGiaTriThanhToanTN
        {
            get
            {
                return (fGiaTriThanhToanTN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThanhToanNN
        {
            get
            {
                return (fGiaTriThanhToanNN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThuHoiTN
        {
            get
            {
                return (fGiaTriThuHoiTN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThuHoiNN
        {
            get
            {
                return (fGiaTriThuHoiNN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sThueGiaTriGiaTang
        {
            get
            {
                return (fThueGiaTriGiaTang ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sChuyenTienBaoHanh
        {
            get
            {
                return (fChuyenTienBaoHanh ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public string sGiaTriThuHoiUngTruocTN
        {
            get
            {
                return (fGiaTriThuHoiUngTruocTN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public string sGiaTriThuHoiUngTruocNN
        {
            get
            {
                return (fGiaTriThuHoiUngTruocNN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public double fLuyKeTTTN { get; set; }
        public double fLuyKeTTNN { get; set; }
        public string sLuyKeTTTN
        {
            get
            {
                return (fLuyKeTTTN).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public string sLuyKeTTNN
        {
            get
            {
                return (fLuyKeTTNN).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public double fLuyKeTUTN { get; set; }
        public double fLuyKeTUNN { get; set; }
        public string sLuyKeTUNN
        {
            get
            {
                return (fLuyKeTUNN).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public string sLuyKeTUTN
        {
            get
            {
                return (fLuyKeTUTN).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public double fLuyKeTUUngTruocTN { get; set; }
        public double fLuyKeTUUngTruocNN { get; set; }
        public string sLuyKeTUUngTruocTN
        {
            get
            {
                return (fLuyKeTUUngTruocTN).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }

        public string sLuyKeTUUngTruocNN
        {
            get
            {
                return (fLuyKeTUUngTruocNN).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sLuyKeGiaTriNghiemThuKLHT
        {
            get
            {
                return (fLuyKeGiaTriNghiemThuKLHT ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double? fTienHopDong { get; set; }
        public string sTienHopDong
        {
            get
            {
                return (fTienHopDong ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sTenNhaThau { get; set; }
        public string sNgayBangKLHT
        {
            get => dNgayBangKLHT.HasValue ? dNgayBangKLHT.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
        public string sNgayDeNghi
        {
            get => dNgayDeNghi.HasValue ? dNgayDeNghi.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
        public string sNgayPheDuyet
        {
            get => dNgayPheDuyet.HasValue ? dNgayDeNghi.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
        public List<PheDuyetThanhToanChiTiet> listPheDuyetChiTiet { get; set; }
        public List<KeHoachVonModel> listKeHoachVon { get; set; }
        public List<VdtTtDeNghiThanhToanChiPhiQuery> listChiPhi { get; set; }

        // new fields
        public string sTenDonVi { get; set; }
        public string CoQuanThanhToan { get; set; }
        public string sTenHopDong { get; set; }
        public int iLoaiThanhToan { get; set; }
        public int iSTT { get; set; }
        public string sThueGiaTriGiaTangDuocDuyet
        {
            get
            {
                return (fThueGiaTriGiaTangDuocDuyet ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sChuyenTienBaoHanhDuocDuyet
        {
            get
            {
                return (fChuyenTienBaoHanhDuocDuyet ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public class DeNghiThanhToanValueQuery
    {
        public double ThanhToanTN { get; set; }
        public double ThanhToanNN { get; set; }
        public double ThuHoiUngTN { get; set; }
        public double ThuHoiUngNN { get; set; }
        public double TamUngTN { get; set; }
        public double TamUngNN { get; set; }
        public double ThuHoiUngUngTruocTN { get; set; }
        public double ThuHoiUngUngTruocNN { get; set; }
        public double TamUngUngTruocTN { get; set; }
        public double TamUngUngTruocNN { get; set; }
    }

    public class KeHoachVonModel
    {
        public Guid Id { get; set; }
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public int iNamKeHoach { get; set; }
        public Guid? iID_DonViQuanLyID { get; set; }
        public string iID_MaDonViQuanLy { get; set; }
        public int iID_NguonVonID { get; set; }
        public int iPhanLoai { get; set; }
        public double? fLenhChi { get; set; }
        public double? fTongGiaTri { get; set; }
        public string sTenLoai { get; set; }
        public bool IsChecked { get; set; }
        public string sNgayQuyetDinh
        {
            get => dNgayQuyetDinh.HasValue ? dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
        public string sLenhChi
        {
            get => (fLenhChi ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
        }
        public string sTongGiaTri
        {
            get => (fTongGiaTri ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
        }
        public string sMaNguonCha { get; set; }
    }

    public class PheDuyetThanhToanChiTiet : VDT_TT_PheDuyetThanhToan_ChiTiet
    {
        public string sXauNoiMa { get; set; }
        public int iLoaiDeNghi { get; set; }
        public string sLoaiDeNghi
        {
            get
            {
                return PaymentTypeEnum.Get(iLoaiDeNghi);
            }
        }
        public int iLoai { get; set; }
        public int iLoaiNamKH { get; set; }
        public int iCoQuanThanhToanKHV { get; set; }
        public double? fGiaTriNgoaiNuoc { get; set; }
        public string sGiaTriNgoaiNuoc {
            get
            {
                return (fGiaTriNgoaiNuoc ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double ? fGiaTriTrongNuoc { get; set; }
        public string sGiaTriTrongNuoc
        {
            get
            {
                return (fGiaTriTrongNuoc ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sLoai { get; set; }
        public string sLoaiNamKH { get; set; }
        public double fTongSo { get; set; }
        public string sTongSo
        {
            get
            {
                return (fTongSo).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThuHoiNamNayTN
        {
            get
            {
                return (fGiaTriThuHoiNamNayTN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThuHoiNamNayNN
        {
            get
            {
                return (fGiaTriThuHoiNamNayNN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThuHoiNamTruocTN
        {
            get
            {
                return (fGiaTriThuHoiNamTruocTN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThuHoiNamTruocNN
        {
            get
            {
                return (fGiaTriThuHoiNamTruocNN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThanhToanTN
        {
            get
            {
                return (fGiaTriThanhToanTN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sGiaTriThanhToanNN
        {
            get
            {
                return (fGiaTriThanhToanNN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public string sTenKHV { get; set; }
        public string sTenDeNghiTU { get; set; }
        public string sMaNguonCha { get; set; }
        public bool isDelete { get; set; }
        public string sLNS { get; set; }
        public string sL { get; set; }
        public string sK { get; set; }
        public string sMoTa { get; set; }
        public double? fDefaultValueTN { get; set; }
        public string sDefaultValueTN
        {
            get
            {
                return (fDefaultValueTN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double? fDefaultValueNN { get; set; }
        public string sDefaultValueNN
        {
            get
            {
                return (fDefaultValueNN ?? 0).ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public class TongHopNguonNSDauTuQuery
    {
        public Guid Id { get; set; }
        public Guid iID_ChungTu { get; set; }
        public Guid iID_DuAnID { get; set; }
        public string sMaNguon { get; set; }
        public string sMaNguonCha { get; set; }
        public string sMaDich { get; set; }
        public double? fGiaTri { get; set; }
        public int? ILoaiUng { get; set; }
        public int iStatus { get; set; }
        public bool bIsLog { get; set; }
        public Guid? iId_MaNguonCha { get; set; }
        public int? iThuHoiTUCheDo { get; set; }
        public Guid? IIDMucID { get; set; }
        public Guid? IIDTieuMucID { get; set; }
        public Guid? IIDTietMucID { get; set; }
        public Guid? IIDNganhID { get; set; }
        [NotMapped]
        public bool? bKeHoach { get; set; }
    }

    public class VdtTtDeNghiThanhToanChiPhiQuery
    {
        public Guid IIdChiPhiId { get; set; }
        public string STenChiPhi { get; set; }
        public double FGiaTriPheDuyetDuToan { get; set; }
        public string SGiaTriPheDuyetDuToan
        {
            get => FGiaTriPheDuyetDuToan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
        }
        public double FGiaTriPheDuyetQdDauTu { get; set; }
        public string SGiaTriPheDuyetQdDauTu
        {
            get => FGiaTriPheDuyetQdDauTu.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
        }
        public bool IsChecked { get; set; }
    }

    public class VdtTtKeHoachVonQuery
    {
        public Guid IIdKeHoachVonId { get; set; }
        public string SSoQuyetDinh { get; set; }
        public int? INamKeHoach { get; set; }
        public int ILoaiKeHoachVon { get; set; }
        public int ILoaiNamKhv { get; set; }
        public int ILoaiNamTamUng { get; set; }
        public int ICoQuanThanhToan { get; set; }
        public double FGiaTriThanhToanTN { get; set; }
        public double FGiaTriThanhToanNN { get; set; }
        public double FGiaTriThuHoiTrongNuoc { get; set; }
        public double FGiaTriThuHoiNgoaiNuoc { get; set; }
        public double FGiaTriKHV { get; set; }
        public double FGiaTriKHVDaThanhToan { get; set; }
        public string SDisplayName
        {
            get
            {
                string sLoaiKeHoach = string.Empty;
                switch (ILoaiNamKhv)
                {
                    case (int)NamKeHoachEnum.Type.NAM_TRUOC:
                        if (ILoaiKeHoachVon == 1 || ILoaiKeHoachVon == 3)
                            sLoaiKeHoach = "Kế hoạch vốn năm chuyển sang";
                        else
                            sLoaiKeHoach = "Kế hoạch vốn ứng chuyển sang";
                        break;
                    case (int)NamKeHoachEnum.Type.NAM_NAY:
                        if (ILoaiKeHoachVon == 1)
                            sLoaiKeHoach = "Kế hoạch vốn năm";
                        else
                            sLoaiKeHoach = "Kế hoạch vốn ứng";
                        break;
                }
                return string.Format("{0}-{1}", SSoQuyetDinh, sLoaiKeHoach);
            }
        }
    }

    public class MlnsByKeHoachVonModel
    {
        public Guid IidKeHoachVonId { get; set; }
        public int ILoaiKeHoachVon { get; set; }
        public string LNS { get; set; }
        public string L { get; set; }
        public string K { get; set; }
        public string M { get; set; }
        public string TM { get; set; }
        public string TTM { get; set; }
        public string NG { get; set; }
        public string SMoTa { get; set; }
        public string sXauNoiMa
        {
            get
            {
                if (!string.IsNullOrEmpty(NG))
                    return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", LNS, L, K, M, TM, TTM, NG);
                else if (!string.IsNullOrEmpty(TTM))
                    return string.Format("{0}-{1}-{2}-{3}-{4}-{5}", LNS, L, K, M, TM, TTM);
                else if (!string.IsNullOrEmpty(TM))
                    return string.Format("{0}-{1}-{2}-{3}-{4}", LNS, L, K, M, TM);
                else if (!string.IsNullOrEmpty(M))
                    return string.Format("{0}-{1}-{2}-{3}", LNS, L, K, M);
                return string.Empty;
            }
        }
    }

    public class CapPhatThanhToanReportQuery : VDT_TT_DeNghiThanhToan
    {
        public string TenDuAn { get; set; }
        public string MaDuAn { get; set; }
        public string TenDonVi { get; set; }
        public string TenChuDauTu { get; set; }
        public string TenHopDong { get; set; }
        public DateTime? NgayHopDong { get; set; }
        public string SoDeNghi { get; set; }
        public int LoaiThanhToan { get; set; }
        public string TenNguonVon { get; set; }
        public int NamKeHoach { get; set; }
        public double ThanhToanTN { get; set; }
        public double ThanhToanNN { get; set; }
        public double ThuHoiTN { get; set; }
        public double ThuHoiNN { get; set; }
        public string NoiDung { get; set; }
        public double GiaTriHopDong { get; set; }
        public double ThueGiaTriGiaTang { get; set; }
        public double ChuyenTienBaoHanh { get; set; }
        public string TenNhaThau { get; set; }
        public string SoTaiKhoanNhaThau { get; set; }
        public string STKTrongNuoc { get; set; }
        public string ChiNhanhTrongNuoc { get; set; }
        public string STKNuocNgoai { get; set; }
        public string ChiNhanhNuocNgoai { get; set; }
        public string MaSoDVSDNS { get; set; }

        public string duToanPheDuyet { get; set; }
        public DateTime khvNgayQuyetDinh { get; set; }
        public DateTime thanhtoanNgayDeNghi { get; set; }
    }
}
