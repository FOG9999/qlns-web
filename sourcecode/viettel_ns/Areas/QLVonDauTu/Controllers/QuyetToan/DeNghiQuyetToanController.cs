using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Viettel.Models.QLVonDauTu;

namespace VIETTEL.Areas.QLVonDauTu.Controllers.QuyetToan
{
    public class DeNghiQuyetToanController : Controller
    {
        // GET: QLVonDauTu/DeNghiQuyetToan
        public ActionResult Index()
        {
            //var data = new DeNghiQuyetToanPagingModel();
            //data._paging.CurrentPage = 1;
            //data.lstData = _aService.GetAllVDT_DM_ChiPhiPaging(ref data._paging);
            //return View(data);

            return View();
        }
    }
}