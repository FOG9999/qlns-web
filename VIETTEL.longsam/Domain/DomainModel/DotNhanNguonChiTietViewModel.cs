using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Domain.DomainModel
{
    public class DotNhanNguonChiTietViewModel
    {
        public Guid iID_DotNhanChiTiet { get; set; }
        public Guid iID_Nguon { get; set; }
        public Guid? iID_NguonCha { get; set; }
        public string sMaCTMT { get; set; }
        public string sLoai { get; set; }
        public string sKhoan { get; set; }
        public string sNoiDung { get; set; }
        public decimal? SoTien { get; set; }
        public string GhiChu { get; set; }
        public string isCha { get; set; }
        public string locations { get; set; }

    }

    public class DotNhanNguonViewModel
    {
        public Guid iID_DotNhan { get; set; }
        public string noiDungDotNhan { get; set; }
        public string sNoiDung { get; set; }
        public List<DotNhanNguonChiTietViewModel> ListNNS_DotNhanChiTiet { get; set; }
    }
}
