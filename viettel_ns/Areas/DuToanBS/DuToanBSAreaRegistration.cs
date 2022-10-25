using System.Web.Mvc;

namespace VIETTEL.Areas.DuToanBS
{
    public class DuToanBSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DuToanBS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DuToanBS_default",
                "DuToanBS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "VIETTEL.Areas.DuToanBS.Controllers" }
            );
        }
    }
}
