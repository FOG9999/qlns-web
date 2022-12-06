using System.Web.Mvc;

namespace VIETTEL.Areas.DuToanKT
{
    public class DuToanKTAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DuToanKT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DuToanKT_default",
                "DuToanKT/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "VIETTEL.Areas.DuToanKT.Controllers" }
            );
        }
    }
}
