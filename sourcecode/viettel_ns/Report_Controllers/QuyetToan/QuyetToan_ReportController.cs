using System.Web.Mvc;
using VIETTEL.Controllers;

namespace VIETTEL.Report_Controllers.QuyetToan
{
    public class QuyetToan_ReportController : AppController
    {
        //
        // GET: /QuyetToan_Report/
        public string sViewPath = "~/Report_Views/QuyetToan/";
        public ActionResult Index(int? loai)
        {
            if (!loai.HasValue)
            {
                return View(@"~/Views/Report_Views\QuyetToan\Index.cshtml");
            }
            else if (loai == 5)
            {
                return QuyetToanNam();
            }
            else
            {
                var view = sViewPath + "Report_Index.aspx";
                return View(view);
            }

        }

        public ActionResult QuyetToanNam()
        {
            var view = "~/Views/Report_Views/QuyetToan/Index_QuyetToan_Nam.cshtml";
            return View(view);
        }



    }
}
