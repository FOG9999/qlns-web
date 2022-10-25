using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLThongTinHopDong
{
    public class ChiPhiInfoModel
    {
        public Guid IIDGoiThauID { get; set; }
        public Guid IIDChiPhiID { get; set; }
        public Guid? IIDHopDongID { get; set; }
        public Guid? IIDHangMucID { get; set; }
        public Guid? IIDNhaThauID { get; set; }
        public bool IsChecked { get; set; }
        public string STenChiPhi { get; set; }
        public double FGiaTriDuocDuyet { get; set; }
        public int IThuTu { get; set; }
        public Guid? IdChiPhiDuAnParent { get; set; }
        public Guid? HangMucParentId { get; set; }
        public string MaOrDer { get; set; }
        public Guid? Id { get; set; }
        public Guid? IdGoiThauNhaThau { get; set; }
        public string STenHangMuc { get; set; }
        public double? FTienGoiThau { get; set; }
        public double FGiaTriConLai { get; set; }
        public bool IsHangCha { get; set; }
        public bool IsHasHangMuc { get; set; }
    }
}
