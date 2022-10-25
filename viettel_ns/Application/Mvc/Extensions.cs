using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIETTEL.Models;

namespace VIETTEL.Controllers
{
    public static class Extensions
    {
        public static string ControllerName(this Controller controller)
        {
            return controller.ControllerContext.RouteData.Values["controller"].ToString();
        }
        
    }
}