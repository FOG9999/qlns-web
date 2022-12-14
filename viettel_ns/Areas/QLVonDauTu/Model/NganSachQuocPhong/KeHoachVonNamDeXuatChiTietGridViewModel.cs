using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class KeHoachVonNamDeXuatChiTietGridViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }

        public VDT_KHV_KeHoachVonNam_DeXuat KHVonNamDeXuat { get; set; }

        public IEnumerable<DM_NoiDungChi> lstDMNoiDungChi { get; set; }

        public string Id { get; set; }

        public string sTen { get; set; }

        public string sTenNoiDungChi { get; set; }

        public string STT { get; set; }

        public Guid IdReference { get; set; }
        public virtual Guid iID_LoaiCongTrinhID { get; set; }
        public virtual int iID_NguonVonID { get; set; }
    }
}