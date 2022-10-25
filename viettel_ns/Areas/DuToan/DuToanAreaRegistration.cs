using System.Web.Mvc;

namespace VIETTEL.Areas.DuToan
{
    public class DuToanAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DuToan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DuToan_default",
                "DuToan/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "VIETTEL.Areas.DuToan.Controllers" }
            );
        }
    }
}
