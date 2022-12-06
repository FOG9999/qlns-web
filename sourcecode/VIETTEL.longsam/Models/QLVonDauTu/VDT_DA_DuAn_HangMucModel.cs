using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viettel.Models.QLVonDauTu
{
    public class VDT_DA_DuAn_HangMucModel
    {
        public Guid? iID_DuAn_HangMucID { get; set; }
        public Guid? iID_DuAnID { get; set; }
        public string sMaHangMuc { get; set; }
        public string sTenHangMuc { get; set; }
        public double? fTienHangMuc { get; set; }
        public Guid? iID_TienTeID { get; set; }
        public Guid? iID_DonViTienTeID { get; set; }
        public double? fTiGiaDonVi { get; set; }
        public double? fTiGia { get; set; }
        public string sMoTa  { get; set; }
        public Guid? iID_ParentID { get; set; }
        public string sQuyMo { get; set; }
        public int iID_NguonVonID { get; set; }
        public int indexMaHangMuc { get; set; }
        public double? fHanMucDauTu { get; set; }
        public Guid? iID_LoaiCongTrinhID { get; set; }

    }
}
