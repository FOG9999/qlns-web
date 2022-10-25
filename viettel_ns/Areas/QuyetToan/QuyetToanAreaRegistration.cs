using System.Web.Mvc;

namespace VIETTEL.Areas.QuyetToan
{
    public class QuyetToanAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QuyetToan";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QuyetToan_default",
                "QuyetToan/{controller}/{action}/{id}",
                new
                {
                    action = "Index",
                    id = UrlParameter.Optional
                },
                namespaces: new[] { "VIETTEL.Areas.QuyetToan.Controllers" }
            );
        }
    }
}
