using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Viettel.Models.QLNguonNganSach
{
    public class RptChiTietTongHopQDBSDuToanNSNamModel
    {
        public Guid iID_DuToan { get; set; }
        public string sSoQuyetDinh { get; set; }
        public string iID_MaDonVi { get; set; }
        public string sTenDonVi { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public double iTongTien { get; set; }
        public string sTongTien
        {
            get
            {
                return this.iTongTien == 0 ? "" : this.iTongTien.ToString("##,#", CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public class RptTongHopQDBSDuToanNSNamModel
    {
        public List<RptChiTietTongHopQDBSDuToanNSNamModel> lstDataChiTiet { get; set; }
        public List<ThongTinDotBS> listSoQuyetDinh { get; set; }
        public List<ThongTinDonVi> listDonVi { get; set; }
        public List<ThongTinPhongBan> listPhongBan { get; set; }
        public List<ThongTinDonViPBan> listDonViPBan { get; set; }
        public double dTong
        {
            get
            {
                if (listDonVi != null && listDonVi.Count > 0)
                    return listDonVi.Sum(x => x.dTongTien);
                else if (listPhongBan != null && listPhongBan.Count > 0)
                    return listPhongBan.Sum(x => x.dTongTien);
                else return 0;
            }
        }
    }

    public class ThongTinDonVi
    {
        public int iSTT { get; set; }
        public string iID_MaDonVi { get; set; }
        public string sTenDonVi { get; set; }
        public double dTongTien { get; set; }
        public List<GiaTri> lstGiaTri { get; set; }

    }

    public class ThongTinPhongBan
    {
        public int iSTT { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public double dTongTien { get; set; }
        public List<GiaTri> lstGiaTriPhongBan { get; set; }
    }

    public class ThongTinDotBS
    {
        public Guid iID_DuToan { get; set; }
        public string sSoQuyetDinh { get; set; }
        public double dTongTien { get; set; }
    }

    public class ThongTinDonViPBan
    {
        public string iID_MaDonVi { get; set; }
        public string sTenDonVi { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public double dTongTien { get; set; }
        public List<GiaTri> lstGiaTriDonViPBan { get; set; }
    }

    public class GiaTri
    {
        public double? fSoTien { get; set; }
    }
}
