using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNguonNganSach;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class RptTongHopGiaoDuToanModel
    {
        public IEnumerable<NNSNganSachTheoNamLamViec> lstTongNNS { get; set; }
        public IEnumerable<DM_NoiDungChi> lstNoiDungChi { get; set;  }
        public IEnumerable<NNSDuToanTheoNamLamViecModel> lstTongTienDuToan { get; set; }
        public IEnumerable<RptNNSDuToanChiTietModel> lstDuToanChiTiet { get; set; }
        public IEnumerable<RptNNSDuToanChiTietModel> lstDuToanChoPheDuyet { get; set; }
        public IEnumerable<PhongBanNhiemVu> lstPhongBanNhiemVu { get; set; }
        public IEnumerable<DMNoiDungChiViewModel> treeNoiDungChi { get; set; }
    }

    public class TongTienDuToan
    {
        public List<GiaTri> lstGiaTriTongDuToan { get; set; }
    }
}