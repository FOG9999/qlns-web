using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class KeHoachVonNamPhanBoVonDonViPheDuyetChiTietGridViewModel
    {
        public SheetViewModel Sheet { get; set; }
        public int Filter { get; set; }
        public Dictionary<int, string> FilterOptions { get; set; }
        public VDT_KHV_PhanBoVon_DonVi_PheDuyet KHVonNamPhanBoVon_DonVi_PheDuyet { get; set; }
    }
}