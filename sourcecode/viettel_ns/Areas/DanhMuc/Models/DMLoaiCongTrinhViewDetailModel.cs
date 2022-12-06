using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Viettel.Domain.DomainModel;

namespace VIETTEL.Areas.DanhMuc.Models
{
    public class DMLoaiCongTrinhViewDetailModel : VDT_DM_LoaiCongTrinh
    {
        public string sMaLoaiCongTrinhCha { get; set; }
    }
}