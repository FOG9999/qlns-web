using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDTBaoCaoDieuChinhKeHoachViewModel
    {
        public int iStt { get; set; }
        public int level { get; set; }
        public string sXauNoiChuoi { get; set; }
        public string iID_MaDonViQuanLy { get; set; }
        public Guid iID_LoaiCongTrinhID { get; set; }
        public Guid iID_CapPheDuyetID { get; set; }
        public Guid? iID_DuAnID { get; set; }
        public string sTenDuAn { get; set; }
        public string sSoQuyetDinh { get; set; }
        public DateTime? dNgayQuyetDinh { get; set; }
        public string sLNS { get; set; }
        public string sM { get; set; }
        public string sTM { get; set; }
        public string sTTM { get; set; }
        public string sNG { get; set; }
        public double? fTang { get; set; }
        public double? fGiam { get; set; }
        public double? fChiTieuDauNam { get; set; }
        public string sTenDonVi { get; set; }
        public string sTenLoaiCongTrinh { get; set; }
        public string sTenPhanCap { get; set; }
        public bool IsHangCha { get; set; }
        public double fKeHoachDieuChinh
        {
            get
            {
                return (fChiTieuDauNam ?? 0) - (fGiam ?? 0) + (fTang ?? 0);
            }
        }
    }
}
