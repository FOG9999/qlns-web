using System.Web.Mvc;

namespace VIETTEL.Areas.CapPhat
{
    public class CapPhatAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CapPhat";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CapPhat_default",
                "CapPhat/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "VIETTEL.Areas.CapPhat.Controllers", "VIETTEL.Areas.CapPhat.Controllers.Reports" }
            );
        }
    }
}
