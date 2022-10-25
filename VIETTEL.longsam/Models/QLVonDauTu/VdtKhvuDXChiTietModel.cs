using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class VdtKhvuDXChiTietModel : VDT_KHV_KeHoachVonUng_DX
    {
        public string sTenDonVi { get; set; }
        public string sTenNguonVon { get; set; }
        public int sSTT { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string sMaDuAn { get; set; }
        public string sTenDuAn { get; set; }
        public string sGhiChu { get; set; }
        public double? fGiaTriDeNghi { get; set; }
        public string sGiaTriDeNghi
        {
            get
            {
                return this.fGiaTriDeNghi.HasValue ? fGiaTriDeNghi.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : 0.ToString();
            }

        }
        public string sNgayDeNghi
        {
            get
            {
                return dNgayDeNghi.HasValue ? dNgayDeNghi.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }
        public List<VdtKhcKeHoachVonUngDeXuatChiTietModel> listKhvuChiTiet { get; set; }
        public List<VdtKhvuDXChiTietModel> listChungTuChild { get; set; }
        public double fSumTongMucDauTu
        {
            get
            {
                double fSum = 0;
                if (listKhvuChiTiet != null && listKhvuChiTiet.Any())
                {
                    fSum = listKhvuChiTiet.Sum(x => x.fTongMucDauTu);
                }
                return fSum;
            }
        }
        public string sSumTongMucDauTu
        {
            get
            {
                return this.fSumTongMucDauTu.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double fSumGiaTriDeNghi
        {
            get
            {
                double fSum = 0;
                if (listKhvuChiTiet != null && listKhvuChiTiet.Any())
                {
                    fSum = listKhvuChiTiet.Sum(x => x.fGiaTriDeNghi ?? 0);
                }
                return fSum;
            }
        }
        public string sSumGiaTriDeNghi
        {
            get
            {
                return this.fSumGiaTriDeNghi.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }
}
