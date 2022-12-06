using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Models.QLNguonNganSach;

namespace Viettel.Domain.DomainModel
{
    public class BaoCaoTongHopNguonViewModel
    {
        public Guid iID_Nguon { get; set; }
        public string sMaCTMT { get; set; }
        public string sLoai { get; set; }
        public string sKhoan { get; set; }
        public string sNoiDung { get; set; }
        public decimal dTDauNam { get; set; }
        public decimal dtChuyenSang { get; set; }
        public decimal NhaNuocBosung { get; set; }
        public decimal TongDuToan { get; set; }
        public decimal dagiaoDauNam { get; set; }
        public decimal dagiaoChuyenSang { get; set; }
        public decimal dagiaoDuToan { get; set; }
        public decimal dagiaoTongDuToan { get; set; }
        public decimal conLai { get; set; }
        public string bLaHangCha { get; set; }
        public string depth { get; set; }
        public DateTime dNgayQuyetDinh { get; set; }
        public string sSoQuyetDinh { get; set; }

        public List<DotNhanBoSungTrongNamViewModel> BoSungTrongNams { get; set; }
        public List<DotNhanBoSungTrongNamViewModel> DaGiaoTrongNams { get; set; }
        public List<SoTien> lstGiaTri { get; set; }
    }

    public class TinhTongSoTienBaoCaoDotNhanViewModel
    {
        public decimal TongDauNam { get; set; }
        public decimal TongChuyenSang { get; set; }
        public decimal TongBoSung { get; set; }
        public decimal TongDuToanAll { get; set; }
        public List<decimal> TongCacDotBoSung { get; set; }
        public decimal DaGiaoTongDauNam { get; set; }
        public decimal DaGiaoTongChuyenSang { get; set; }
        public decimal DaGiaoTongDuToan { get; set; }
        public decimal DaGiaoTongDuToanAll { get; set; }
        public decimal ConLai { get; set; }
        public List<decimal> DaGiaoTongCacDotDuToan { get; set; }
    }

    public class SoTien
    {
        public decimal? fSoTien { get; set; }
    }
}
