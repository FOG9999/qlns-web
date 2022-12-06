using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTKeHoachVonUngDuocDuyetViewModel
    {
        public int iId { get; set; }
        public int? iParentId { get; set; }
        public Guid iID_DuAnID { get; set; }
        public Guid iID_KeHoachUng_ChiTietID { get; set; }
        public Guid iID_KeHoachUngID { get; set; }
        public Guid? iID_NhomQuanLyID { get; set; }
        public Guid? iID_NganhID { get; set; }
        public string sTenDuAn { get; set; }
        public string sTrangThaiDuAnDangKy { get; set; }
        public string sMaKetNoi { get; set; }
        public decimal? fGiaTriUng { get; set; }
        public decimal? fGiaTriDauTu { get; set; }
        public string sGhiChu { get; set; }
        public string sSoQuyetDinh { get; set; }
        public string sTenDonViQuanLy { get; set; }
        public string sTenNguonVon { get; set; }
        public string sTenNhomQuanLy { get; set; }
        public string sSoDeNghi_KHVUDX { get; set; }
        public int iNamKeHoach { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string sNgayQuyetDinh
        {
            get
            {
                return dNgayQuyetDinh.HasValue ? dNgayQuyetDinh.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public List<VdtKhvKeHoachVonUngChiTietModel> listChiTietDuAn { get; set; }

        public double fSumTongMucDauTu
        {
            get
            {
                double sum = 0;
                if (listChiTietDuAn != null && listChiTietDuAn.Any())
                    sum = listChiTietDuAn.Sum(x => x.fTongMucDauTu);
                return sum;
            }
        }
        public string sSumTongMucDauTu
        {
            get
            {
                return this.fSumTongMucDauTu.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fSumCapPhatTaiKhoBac
        {
            get
            {
                double sum = 0;
                if (listChiTietDuAn != null && listChiTietDuAn.Any())
                    sum = listChiTietDuAn.Sum(x => x.fCapPhatTaiKhoBac);
                return sum;
            }
        }
        public string sSumCapPhatTaiKhoBac
        {
            get
            {
                return this.fSumCapPhatTaiKhoBac.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fSumCapPhatBangLenhChi
        {
            get
            {
                double sum = 0;
                if (listChiTietDuAn != null && listChiTietDuAn.Any())
                    sum = listChiTietDuAn.Sum(x => x.fCapPhatBangLenhChi);
                return sum;
            }
        }
        public string sSumCapPhatBangLenhChi
        {
            get
            {
                return this.fSumCapPhatBangLenhChi.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }
}
