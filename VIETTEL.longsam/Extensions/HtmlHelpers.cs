using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace System.Web.Mvc
{
    public static class HtmlHelpers
    {
        public static HtmlString ToHtmlString(this string input)
        {
            return new HtmlString(input);
        }

        #region script block

        //https://jadnb.wordpress.com/2011/02/16/rendering-scripts-from-partial-views-at-the-end-in-mvc/

        #endregion

        private class ScriptBlock : IDisposable
        {
            private const string scriptsKey = "scripts";

            public static List<string> pageScripts
            {
                get
                {
                    if (HttpContext.Current.Items[scriptsKey] == null)
                        HttpContext.Current.Items[scriptsKey] = new List<string>();
                    return (List<string>)HttpContext.Current.Items[scriptsKey];
                }
            }

            WebViewPage webPageBase;

            public ScriptBlock(WebViewPage webPageBase)
            {
                this.webPageBase = webPageBase;
                this.webPageBase.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                pageScripts.Add(((StringWriter)this.webPageBase.OutputStack.Pop()).ToString());
            }
        }

        public static IDisposable BeginScripts(this HtmlHelper helper)
        {
            return new ScriptBlock((WebViewPage)helper.ViewDataContainer);
        }

        public static MvcHtmlString PageScripts(this HtmlHelper helper)
        {
            return
                MvcHtmlString.Create(string.Join(Environment.NewLine, ScriptBlock.pageScripts.Select(s => s.ToString())));
        }




        #region style block

        private class StypeBlock : IDisposable
        {
            private const string stypesKey = "styles";

            public static List<string> pageScripts
            {
                get
                {
                    if (HttpContext.Current.Items[stypesKey] == null)
                        HttpContext.Current.Items[stypesKey] = new List<string>();
                    return (List<string>)HttpContext.Current.Items[stypesKey];
                }
            }

            WebViewPage webPageBase;

            public StypeBlock(WebViewPage webPageBase)
            {
                this.webPageBase = webPageBase;
                this.webPageBase.OutputStack.Push(new StringWriter());
            }

            public void Dispose()
            {
                pageScripts.Add(((StringWriter)this.webPageBase.OutputStack.Pop()).ToString());
            }
        }

        public static IDisposable BeginStyles(this HtmlHelper helper)
        {
            return new StypeBlock((WebViewPage)helper.ViewDataContainer);
        }

        public static MvcHtmlString PageStyles(this HtmlHelper helper)
        {
            return MvcHtmlString.Create(string.Join(Environment.NewLine, StypeBlock.pageScripts.Select(s => s.ToString())));
        }

        #endregion
    }

}