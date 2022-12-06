using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Viettel.Models.QLNguonNganSach
{
    public class RptChiTietTongHopDuToanNganSachNamModel
    {
        public string iID_MaDonVi { get; set; }
        public string sTenDonVi { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public double dDaGiaoDauNam { get; set; }
        public string sDaGiaoDauNam
        {
            get
            {
                return this.dDaGiaoDauNam == 0 ? "" : this.dDaGiaoDauNam.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double dBSTrongNam { get; set; }
        public string sBSTrongNam
        {
            get
            {
                return this.dBSTrongNam == 0 ? "" : this.dBSTrongNam.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
        public double dTongDuToan { get; set; }
        public string sTongDuToan
        {
            get
            {
                return this.dTongDuToan == 0 ? "" : this.dTongDuToan.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public class RptTongHopDuToanNganSachNamModel
    {
        public List<RptChiTietTongHopDuToanNganSachNamModel> lstDataChiTiet { get; set; }
        public double? dTongDaGiaoDauNam
        {
            get
            {
                return this.lstDataChiTiet.Count == 0 ? 0 : this.lstDataChiTiet.Sum(x => x.dDaGiaoDauNam);
            }
        }
        public string sTongDaGiaoDauNam
        {
            get
            {
                return this.dTongDaGiaoDauNam.HasValue ? this.dTongDaGiaoDauNam.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double? dTongBSTrongNam
        {
            get
            {
                return this.lstDataChiTiet.Count == 0 ? 0 : this.lstDataChiTiet.Sum(x => x.dBSTrongNam);
            }
        }
        public string sTongBSTrongNam
        {
            get
            {
                return this.dTongBSTrongNam.HasValue ? this.dTongBSTrongNam.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
        public double? dTongDuToan
        {
            get
            {
                return this.lstDataChiTiet.Count == 0 ? 0 : this.lstDataChiTiet.Sum(x => x.dTongDuToan);
            }
        }
        public string sTongDuToan
        {
            get
            {
                return this.dTongDuToan.HasValue ? this.dTongDuToan.Value.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN")) : "";
            }
        }
    }
}
