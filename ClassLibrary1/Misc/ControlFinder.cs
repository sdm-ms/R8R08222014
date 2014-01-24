using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public static class ControlFinder
{
    /// <summary>
    /// Similar to Control.FindControl, but recurses through child controls.
    /// </summary>
    public static T FindControl<T>(Control startingControl, string id) where T : Control
    {
        T found = startingControl.FindControl(id) as T;

        if (found == null)
        {
            found = FindChildControl<T>(startingControl, id);
        }

        return found;
    }

    /// <summary>     
    /// Similar to Control.FindControl, but recurses through child controls.
    /// Assumes that startingControl is NOT the control you are searching for.
    /// </summary>
    public static T FindChildControl<T>(Control startingControl, string id) where T : Control
    {
        T found = null;

        foreach (Control activeControl in startingControl.Controls)
        {
            found = activeControl as T;

            if (found == null || (string.Compare(id, found.ID, true) != 0))
            {
                found = FindChildControl<T>(activeControl, id);
            }

            if (found != null)
            {
                break;
            }
        }

        return found;
    }
}
