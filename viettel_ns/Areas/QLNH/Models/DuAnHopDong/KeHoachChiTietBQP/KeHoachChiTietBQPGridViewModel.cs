using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Models.KeHoachChiTietBQP;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNH.Models.DuAnHopDong.KeHoachChiTietBQP
{
    public class KeHoachChiTietBQPGridViewModel
    {
        public SheetViewModel Sheet { get; set; }
        public NH_KHChiTietBQP_NVCViewModel KHChiTietBQP_NVC { get; set; }
        public NH_KHChiTietBQPModel NH_KHChiTietBQPModel { get; set; }
        public bool IsUseLastTTCP { get; set; }
        public Guid? iID_TiGiaID { get; set; }
    }
}