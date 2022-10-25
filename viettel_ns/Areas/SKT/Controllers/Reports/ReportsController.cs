using System.Web.Mvc;
using VIETTEL.Areas.SKT.Models;
using VIETTEL.Controllers;

namespace VIETTEL.Areas.SKT.Controllers
{
    public class ReportsController : FlexcelReportController
    {        
        public ActionResult Index(string loai)
        {            
            var vm = new SKTReportViewModel()
            {
                loai = loai,
            };

            return View(vm);
        }

    }
}
