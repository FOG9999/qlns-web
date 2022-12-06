using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLNguonNganSach.Models
{
    public class DotNhanChiTietNDChiViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }

        public IEnumerable<NNSDMNoiDungChiViewModel> lstDMNoiDungChi { get; set; }
        public Guid iID_DotNhan { get; set; }
        public Guid iID_DotNhanChiTiet { get; set; }
        public string sNoiDung { get; set; }
        public string sMaLoaiDuToan { get; set; }
        public bool isClone { get; set; }
        public double fSoTienNhanTuBTC { get; set; }
        public double SoTienDaPhanNDC { get; set; }
        public double fSoTienConLai { get; set; }
    }
}