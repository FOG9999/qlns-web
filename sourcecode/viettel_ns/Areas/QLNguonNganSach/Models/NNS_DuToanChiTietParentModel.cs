using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class NNS_DuToanChiTietParentModel
    {
        public string iID_DuToanChiTiet { get; set; }
        public string Id { get; set; }
        public string sMaNoiDungChi { get; set; }
        public string sTenNoiDungChi { get; set; }
        public string iID_MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string sMaPhongBan { get; set; }
        public string sTenPhongBan { get; set; }
        public string SoTien { get; set; }
        public string GhiChu { get; set; }
        public string sMauSac { get; set; }
        public string sFontColor { get; set; }
        public string sFontBold { get; set; }
        public string iThayDoi { get; set; }
        public string STT { get; set; }
        public int Level { get; set; }
        public Guid IdNoiDungChi { get; set; }
        public Guid? IdNoiDungChiParent { get; set; }
        public bool bLaHangCha { get; set; }
        public bool bConLai { get; set; }
        public string IdCheck { get; set; }
        public string IdsChild { get; set; }
        public int STTNumber { get; set; }
        public string iID_NhiemVu { get; set; }
        public string iID_MaChungTu { get; set; }
        public bool bTongTien { get; set; }
        public List<NNS_DuToanChiTietParentModel> ListChild { get; set; }
    }
}