using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class SoKiemTraDotNhanChiTietNDChiModel
    {
        public Guid iID_NoiDungChi { get; set; }
        public string sMaNoiDungChi { get; set; }
        public decimal? soKiemTra { get; set; }
    }
}