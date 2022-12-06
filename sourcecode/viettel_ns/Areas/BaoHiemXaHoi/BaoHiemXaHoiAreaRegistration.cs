using System.Web.Mvc;

namespace VIETTEL.Areas.BaoHiemXaHoi
{
    public class BaoHiemXaHoiAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "BaoHiemXaHoi";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "BaoHiemXaHoi_default",
                "BaoHiemXaHoi/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}