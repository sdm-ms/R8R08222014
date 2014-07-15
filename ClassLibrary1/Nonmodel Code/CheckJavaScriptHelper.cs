using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class CheckJavaScriptHelper
{
    public static bool IsJavascriptEnabled
    {
        get
        {
            if (HttpContext.Current.Session == null)
                return true; // this is R8R-specific. This will only be true if we call this from the web service, which will only happen if we are using Javascript.

            if (HttpContext.Current.Session["JS"] == null)
                HttpContext.Current.Session["JS"] = true;

            return (bool)HttpContext.Current.Session["JS"];
        }
    }
}
