using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Viettel.Domain.DomainModel;

namespace Viettel.Models.QLVonDauTu
{
    public class ThongTinGoiThauViewModel : VDT_DA_GoiThau
    {
        public string sTenDuAn { get; set; }
        public string sTenNhaThau { get; set; }

        public float? giatriDieuChinh { get; set; }
        public int? soLanDieuChinh { get; set; }
        public VDT_DA_GoiThau goiThau { get; set; }
        public IEnumerable<GoiThauChiPhiViewModel> listChiPhi { get; set; }
        public IEnumerable<GoiThauNguonVonViewModel> listNguonVon { get; set; }
        public IEnumerable<GoiThauHangMucViewModel> listHangMuc { get; set; }
    }

    public class ThongTinHopDongModel
    {
        public Guid IIdHopDongId { get; set; }
        public Guid IIdGoiThauId { get; set; }
        public Guid IIdNhaThauId { get; set; }
        public string SSoHopDong { get; set; }
        public string STenNhaThau { get; set; }
        public string SHinhThucHopDong { get; set; }
        public double FGiaTri { get; set; }
        public DateTime? DNgayHopDong { get; set; }
        public string NgayHopDong
        {
            get => DNgayHopDong.HasValue ? DNgayHopDong.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
    }


    public class DuAnViewModel : VDT_DA_DuAn
    {
        public string sTenDonvi { get; set; }
        public string sTenChuDauTu { get; set; }
        public string sTenNhomDuAn { get; set; }
        public string sTenNhomQuanLy { get; set; }
        public string sTenHinhThucQuanLy { get; set; }
    }

    public class GoiThauChiPhiViewModel : VDT_DA_GoiThau_ChiPhi
    {
        public string sTenChiPhi { get; set; }
        public double? giaTriTruocDC { get; set; }
        public double? giaTriDC { get; set; }
        public double? giaTriSauDC { get; set; }
        public Guid? iID_ChiPhi_Parent { get; set; }
    }
    public class GoiThauNguonVonViewModel : VDT_DA_GoiThau_NguonVon
    {
        public string sTenNguonVon { get; set; }
    }
    public class GoiThauHangMucViewModel : VDT_DA_GoiThau_HangMuc
    {
        public string sTenHangMuc { get; set; }
        public string maOrder { get; set; }
    }
}
