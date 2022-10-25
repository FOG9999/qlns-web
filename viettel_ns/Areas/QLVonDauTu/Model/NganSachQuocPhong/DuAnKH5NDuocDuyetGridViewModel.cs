using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class DuAnKH5NDuocDuyetGridViewModel
    {
        public DuAnKH5NDXGridViewModel DuAnKH5NDX { get; set; }
        public DanhMucDuAnGridViewModel DanhMucDuAn { get; set; }

    }

    public class DuAnKH5NDXGridViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }

        public VDT_KHV_KeHoach5Nam_DeXuat KH5NamDeXuat { get; set; }
        public Guid? iID_KeHoach5NamID { get; set; }
    }

    public class DanhMucDuAnGridViewModel
    {
        public SheetViewModel Sheet { get; set; }

        public int Filter { get; set; }

        public Dictionary<int, string> FilterOptions { get; set; }

        public VDT_DA_DuAn VDTDuAn { get; set; }

        public Guid? iID_KeHoach5NamID { get; set; }

        public VDT_KHV_KeHoach5Nam KH5NamDuocDuyet { get; set; }

        public bool isAddDuAn { get; set; }

        public bool isRedirectToKHTHDD { get; set; }
    }
}