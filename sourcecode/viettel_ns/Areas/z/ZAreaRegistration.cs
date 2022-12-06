using System.Web.Mvc;

namespace VIETTEL.Areas.Z
{
    public class ZAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "z";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Z_default",
                "z/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "VIETTEL.Areas.z.Controllers" }

            );
        }
    }
}