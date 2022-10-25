using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Models.KeHoachChiTietBQP;

namespace VIETTEL.Models.QLNH
{
    public class ModalConfirmSaveNVCModels : ModalModels
    {
        public virtual List<NH_KHChiTietBQP_NhiemVuChiCreateDto> ListNhiemVuChi { get; set; }
        public virtual string KeHoachChiTietBQP { get; set; }
        public virtual string State { get; set; }
    }
}