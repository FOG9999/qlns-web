using System.Web.Mvc;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.Admin.Controllers
{
    public class AdminReportViewModel
    {
        public string loai { get; set; }
    }
    public class ReportsController : FlexcelReportController
    {        
        public ActionResult Index(string loai)
        {            
            var vm = new AdminReportViewModel()
            {
                loai = loai,
            };

            return View(vm);
        }

    }
}
