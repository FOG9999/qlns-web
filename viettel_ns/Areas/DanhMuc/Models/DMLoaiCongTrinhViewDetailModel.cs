using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;
using Viettel.Models.QLVonDauTu;
using VIETTEL.Common;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMLoaiCongTrinhViewDetailModel : VDT_DM_LoaiCongTrinh
    {
        public string sMaLoaiCongTrinhCha { get; set; }
    }
     public class DMLoaiCongTrinhViewModel
    {
        public PagingInfo _paging = new PagingInfo() { ItemsPerPage = Constants.ITEMS_PER_PAGE };
        public IEnumerable<VDTDMLoaiCongTrinhViewModel> Items { get; set; }
    }
}