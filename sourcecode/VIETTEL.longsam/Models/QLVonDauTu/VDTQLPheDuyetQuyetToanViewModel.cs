using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTQLPheDuyetQuyetToanViewModel
    {
        public int SttExport { get; set; }
        public Guid iID_DonViQuanLyID { get; set; }
        public Guid iID_QuyetToanID { get; set; }
        public Guid iID_DuAnID { get; set; }
        public string sSoQuyetDinh { get; set; }
        public string sNguoiLap { get; set; }
        public string sGhiChu { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public string sTen { get; set; }
        public int IdNguonVon { get; set; }
        public string sDiaDiem { get; set; }
        public string sKhoiCong { get; set; }
        public string sKetThuc { get; set; }
        public string sCoQuanPheDuyet { get; set; }
        public string sNguoiKy { get; set; }
        public string sNoiDung { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string StrThoiGianBaoCao => dNgayQuyetDinh.HasValue ? dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
        public string sTenDonViQuanLy { get; set; }
        public double fTongMucDauTuPheDuyet { get; set; }
        public double fGiaTriUng { get; set; }
        public double fLKSoVonDaTamUng { get; set; }
        public double fLKThuHoiUng { get; set; }
        public double fConPhaiThuHoi { get; set; }
        public double fGiaTriThuHoiUngNgoaiChiTieu { get; set; }
        public double fTongTienTrungThau { get; set; }
        public double fTienQuyetToanPheDuyet { get; set; }
        public double fChiPhiThietHai { get; set; }
        public double fChiPhiKhongTaoNenTaiSan { get; set; }
        public double fTaiSanDaiHanThuocCDTQuanLy { get; set; }
        public double fTaiSanDaiHanDonViKhacQuanLy { get; set; }
        public double fTaiSanNganHanThuocCDTQuanLy { get; set; }
        public double fTaiSanNganHanDonViKhacQuanLy { get; set; }
        public string sDuToan { get; set; }
        public Guid? iID_DuToanID { get; set; }
        public Guid? iID_DeNghiQuyetToanID { get; set; }
        public int iID_LoaiQuyetToan { get; set; }

        //Noi dung quyet toan
        public IEnumerable<VDTNguonVonDauTuTableModel> arrNguonVon { get; set; }
        public double tongGiaTriPheDuyet { get; set; }

        //Noi dung quyet toan result view
        public IEnumerable<VDTNguonVonDauTuViewModel> lstNoiDungQuyetToan { get; set; }
        public double fTongGiaTriPhanBo { get; set; }
        public double fTongGiaTriChenhLech { get; set; }
        public string sChiPhiThietHai
        {
            get
            {
                return fChiPhiThietHai != 0 ? fChiPhiThietHai.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChiPhiKhongTaoNenTaiSan
        {
            get
            {
                return fChiPhiKhongTaoNenTaiSan != 0 ? fChiPhiKhongTaoNenTaiSan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanDaiHanThuocCDTQuanLy
        {
            get
            {
                return fTaiSanDaiHanThuocCDTQuanLy != 0 ? fTaiSanDaiHanThuocCDTQuanLy.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanDaiHanDonViKhacQuanLy
        {
            get
            {
                return fTaiSanDaiHanDonViKhacQuanLy != 0 ? fTaiSanDaiHanDonViKhacQuanLy.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanNganHanThuocCDTQuanLy
        {
            get
            {
                return fTaiSanNganHanThuocCDTQuanLy != 0 ? fTaiSanNganHanThuocCDTQuanLy.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sTaiSanNganHanDonViKhacQuanLy
        {
            get
            {
                return fTaiSanNganHanDonViKhacQuanLy != 0 ? fTaiSanNganHanDonViKhacQuanLy.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
    }

    public class VDTChiPhiDauTuModel
    {
        public Guid iID_QuyetToan_ChiPhiID { get; set; }
        public Guid iID_QuyetToanID { get; set; }
        public Guid iID_ChiPhiID { get; set; }
        public double fTienPheDuyet { get; set; }
        public string sTenChiPhi { get; set; }
        public double fChiPhiThietHai { get; set; }
        public double fChiPhiKhongTaoNenTaiSan { get; set; }
        public string sChiPhiThietHai
        {
            get
            {
                return fChiPhiThietHai != 0 ? fChiPhiThietHai.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public string sChiPhiKhongTaoNenTaiSan
        {
            get
            {
                return fChiPhiKhongTaoNenTaiSan != 0 ? fChiPhiKhongTaoNenTaiSan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
    }

    public class VDTNguonVonDauTuModel
    {
        public Guid iID_QuyetToan_NguonVonID { get; set; }
        public Guid iID_QuyetToanID { get; set; }
        public int iID_NguonVonID { get; set; }
        public double fTienPheDuyet { get; set; }
        public string sTen { get; set; }
    }

    public class VDTNguonVonDauTuTableModel
    {
        public int iID_NguonVonID { get; set; }
        public double fTienPheDuyet { get; set; }
    }

    public class VDTNguonVonDauTuViewModel
    {
        public int Stt { get; set; }
        public string sTenDuAn { get; set; }
        public string sMaDuAn { get; set; }
        public int IdNguonVon { get; set; }
        public int iID_MaNguonNganSach { get; set; }
        public string sTen { get; set; }
        public double fGiaTriDuToan { get; set; }
        public double fGiaTriQuyetToan { get; set; }
        public double fGiaTriChenhLech { get; set; }
        public string sChenhLech { get; set; }
        public double fTienPheDuyet { get; set; }
        public string sTienPheDuyet
        {
            get
            {
                return fTienPheDuyet != 0 ? fTienPheDuyet.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double fGiaTriDeNghiQuyetToan { get; set; }
        public string sGiaTriDeNghiQuyetToan
        {
            get
            {
                return fGiaTriDeNghiQuyetToan != 0 ? fGiaTriDeNghiQuyetToan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }        
        public string sGiaTriQuyetToan
        {
            get
            {
                return fGiaTriQuyetToan != 0 ? fGiaTriQuyetToan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
    }

    public class VDTQLPheDuyetQuyetToanModel : VDT_QT_QuyetToan
    {
        public VDTQLPheDuyetQuyetToanViewModel quyetToan { get; set; }
        public IEnumerable<VDTChiPhiDauTuModel> listQuyetToanChiPhi { get; set; }
        public IEnumerable<VDTNguonVonDauTuModel> listQuyetToanNguonVon { get; set; }
        public IEnumerable<VDTNguonVonDauTuViewModel> listNguonVonChenhLech { get; set; }

        public VDTQLPheDuyetQuyetToanViewModel dataDuAnQT { get; set; }
        public List<VDT_DA_DuToan_ChiPhi_ViewModel> listChiPhi { get; set; }
        public List<VDT_DA_DuToan_HangMuc_ViewModel> listHangMuc { get; set; }
        public List<VDT_DA_NhaThau_GoiThau_ViewModel> listGoiThau { get; set; }
    }
}
