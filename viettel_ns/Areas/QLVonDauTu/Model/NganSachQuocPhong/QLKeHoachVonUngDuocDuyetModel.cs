using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Services;
using VIETTEL.Models;

namespace VIETTEL.Areas.QLVonDauTu.Model.NganSachQuocPhong
{
    public class QLKeHoachVonUngDuocDuyetModel
    {
        private static string[] _lstDonViExclude = new string[] { "0", "1" };
        private readonly IQLVonDauTuService _vdtService = QLVonDauTuService.Default;

        
    }
}