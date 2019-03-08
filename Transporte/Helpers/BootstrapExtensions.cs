using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Transporte.Helpers
{
    public static class BootstrapExtensions
    {
        public static MvcForm BeginHorizontalForm(this HtmlHelper helper)
        {
            return helper.BeginForm(null, null, FormMethod.Post, new { @class = "form-horizontal" });
        }
    }
}