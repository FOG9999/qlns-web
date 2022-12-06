// Credit where credit is due, most of this is lifted from Funnel Web MVC
// http://www.funnelweblog.com/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VIETTEL.Mvc
{
    public class MvcViewEngine : IViewEngine
    {
        private readonly RazorViewEngine _defaultViewEngine = new RazorViewEngine();
        private string _lastTheme;
        private RazorViewEngine _lastEngine;
        private readonly object _lock = new object();
        private readonly string _defaultTheme;

        public MvcViewEngine(string defaultTheme)
        {
            _defaultTheme = defaultTheme;
        }

        private RazorViewEngine CreateRealViewEngine()
        {
            lock (_lock)
            {
                string settingsTheme;
                try
                {
                    settingsTheme = _defaultTheme;
                    if (settingsTheme == _lastTheme)
                    {
                        return _lastEngine;
                    }
                }
                catch (Exception)
                {
                    return _defaultViewEngine;
                }

                _lastEngine = new RazorViewEngine();

                _lastEngine.PartialViewLocationFormats =
                    new[]
                    {
                        "~/Themes/" + settingsTheme + "/Views/{1}/{0}.cshtml",
                        "~/Themes/" + settingsTheme + "/Views/Shared/{0}.cshtml",
                        "~/Themes/" + settingsTheme + "/Views/Shared/{1}/{0}.cshtml",
                        "~/Themes/" + settingsTheme + "/Views/Extensions/{1}/{0}.cshtml",
                        "~/Views/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.PartialViewLocationFormats).ToArray();

                _lastEngine.ViewLocationFormats =
                    new[]
                    {
                        "~/Themes/" + settingsTheme + "/Views/{1}/{0}.cshtml",
                        "~/Themes/" + settingsTheme + "/Views/Extensions/{1}/{0}.cshtml",
                        "~/Views/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.ViewLocationFormats).ToArray();

                _lastEngine.MasterLocationFormats =
                    new[]
                    {
                        "~/Themes/" + settingsTheme + "/Views/{1}/{0}.cshtml",
                        "~/Themes/" + settingsTheme + "/Views/Extensions/{1}/{0}.cshtml",
                        "~/Themes/" + settingsTheme + "/Views/Shared/{1}/{0}.cshtml",
                        "~/Themes/" + settingsTheme + "/Views/Shared/{0}.cshtml",
                        "~/Views/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.MasterLocationFormats).ToArray();

                _lastTheme = settingsTheme;

                return _lastEngine;
            }
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return CreateRealViewEngine().FindPartialView(controllerContext, partialViewName, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return CreateRealViewEngine().FindView(controllerContext, viewName, masterName, useCache);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            CreateRealViewEngine().ReleaseView(controllerContext, view);
        }
    }


    public class MyRazorViewEngine : IViewEngine
    {
        private readonly RazorViewEngine _defaultViewEngine = new RazorViewEngine();
        private string _lastTheme;
        private RazorViewEngine _lastEngine;
        private readonly object _lock = new object();
        private readonly string _defaultTheme;

        public MyRazorViewEngine(string defaultFolder)
        {
            _defaultTheme = defaultFolder;
        }

        private RazorViewEngine CreateRealViewEngine()
        {
            lock (_lock)
            {
                string settingsTheme;
                try
                {
                    settingsTheme = _defaultTheme;
                    if (settingsTheme == _lastTheme)
                    {
                        return _lastEngine;
                    }
                }
                catch (Exception)
                {
                    return _defaultViewEngine;
                }

                _lastEngine = new RazorViewEngine();

                var views = new[]
                    {
                        settingsTheme + "/{1}/{0}.cshtml",
                        settingsTheme + "/Shared/{0}.cshtml",
                        settingsTheme + "/Shared/{1}/{0}.cshtml",
                        settingsTheme + "/Extensions/{1}/{0}.cshtml",
                        "~/Views/Extensions/{1}/{0}.cshtml"
                    };

                _lastEngine.PartialViewLocationFormats = views.Union(_lastEngine.PartialViewLocationFormats).ToArray();

                _lastEngine.ViewLocationFormats =
                    new[]
                    {
                        settingsTheme + "/{1}/{0}.cshtml",
                        settingsTheme + "/Extensions/{1}/{0}.cshtml",
                        "~/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.ViewLocationFormats).ToArray();

                _lastEngine.MasterLocationFormats =
                    new[]
                    {
                        settingsTheme + "/{1}/{0}.cshtml",
                        settingsTheme + "/Extensions/{1}/{0}.cshtml",
                        settingsTheme + "/Shared/{1}/{0}.cshtml",
                        settingsTheme + "/Shared/{0}.cshtml",
                        "~/Extensions/{1}/{0}.cshtml"
                    }.Union(_lastEngine.MasterLocationFormats).ToArray();

                _lastTheme = settingsTheme;

                return _lastEngine;
            }
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return CreateRealViewEngine().FindPartialView(controllerContext, partialViewName, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return CreateRealViewEngine().FindView(controllerContext, viewName, masterName, useCache);
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            CreateRealViewEngine().ReleaseView(controllerContext, view);
        }
    }

    public class AppViewEngine : RazorViewEngine
    {
        public AppViewEngine()
        {
            //string[] viewLocationFormatArr = new string[4];
            //viewLocationFormatArr[0] = "~/UI/{1}/{0}.cshtml";
            //viewLocationFormatArr[1] = "~/UI/{1}/{0}.vbhtml";
            //viewLocationFormatArr[2] = "~/UI/Shared/{1}/{0}.vbhtml";
            //viewLocationFormatArr[3] = "~/UI/Shared/{1}/{0}.vbhtml";
            //this.ViewLocationFormats = viewLocationFormatArr;

            //string[] masterLocationFormatArr = new string[4];
            //masterLocationFormatArr[0] = "~/UI/{1}/{0}.cshtml";
            //masterLocationFormatArr[1] = "~/UI/{1}/{0}.vbhtml";
            //masterLocationFormatArr[2] = "~/UI/Shared/{1}/{0}.vbhtml";
            //masterLocationFormatArr[3] = "~/UI/Shared/{1}/{0}.vbhtml";
            //this.MasterLocationFormats = masterLocationFormatArr;

            //string[] partialViewLocationFormatArr = new string[4];
            //partialViewLocationFormatArr[0] = "~/UI/{1}/{0}.cshtml";
            //partialViewLocationFormatArr[1] = "~/UI/{1}/{0}.vbhtml";
            //partialViewLocationFormatArr[2] = "~/UI/Shared/{1}/{0}.vbhtml";
            //partialViewLocationFormatArr[3] = "~/UI/Shared/{1}/{0}.vbhtml";
            //this.ViewLocationFormats = partialViewLocationFormatArr;


            var views = new List<string>
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",

                // reports views
                "~/Report_Views/{1}/{0}.cshtml",
                "~/Report_Views/{0}.cshtml",
                //"~/Report_Views/{2}/{1}/{0}.cshtml",
                //"~/Report_Views/{3}/{2}/{1}/{0}.cshtml",


                // sheet
                "~/Views/Shared/Sheet/{0}.cshtml",
            };

            var viewsLocation = views.ToArray();
            this.MasterLocationFormats = viewsLocation;
            this.PartialViewLocationFormats = viewsLocation;
            this.ViewLocationFormats = viewsLocation;

        }
    }


    public static class ViewEngineHelper
    {
        public static void ConfigViewEngine(this HttpApplication http)
        {
            ViewEngines.Engines.Clear();
            var ourViewEngine = new AppViewEngine();
            ViewEngines.Engines.Add(ourViewEngine);
            ViewEngines.Engines.Add(new WebFormViewEngine());
        }
    }
}
