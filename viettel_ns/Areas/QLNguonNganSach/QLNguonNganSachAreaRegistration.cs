using System.Web.Mvc;

namespace VIETTEL.Areas.QLNguonNganSach
{
    public class QLNguonNganSachAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "QLNguonNganSach";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "QLNguonNganSach_default",
                "QLNguonNganSach/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}