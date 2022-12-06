using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLNH;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNH.Models.DuAnHopDong.KeHoachTongTheTTCP
{
    public class KeHoachTongTheTTCPChiTietGridViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public NH_KHTongTheTTCP_NVCViewModel NH_KHTongTheTTCP_NVCViewModel { get; set; }

        public NH_KHTongTheTTCPModel NH_KHTongTheTTCPModel { get; set; }
        public bool IsUseLastTTCP { get; set; }
    }
}