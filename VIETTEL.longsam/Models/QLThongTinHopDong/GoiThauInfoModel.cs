using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLThongTinHopDong
{
    public class GoiThauInfoModel
    {
        public Guid? IdGoiThauNhaThau { get; set; }
        public Guid Id { get; set; }
        public Guid IIDGoiThauID { get; set; }
        public Guid? IIDDuAnID { get; set; }
        public string SMaGoiThau { get; set; }
        public string STenGoiThau { get; set; }
        public Guid? IIDGoiThauGocID { get; set; }
        public double FTienTrungThau { get; set; }
        public Guid IIdNhaThauId { get; set; }
        public double fGiaTriGoiThau { get; set; }
        public double FGiaTriTrungThau { get; set; }
        public bool IsChecked { get; set; }
        public Guid iID_HopDongID { get; set; }

        public double? fGiaTriHD { get; set; }
    }
}
