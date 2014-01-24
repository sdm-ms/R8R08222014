using System.Text;
using System.IO;
using System.Web.UI;
using MoreStrings;
using System.Web;
using System.Web.Security;
namespace ClassLibrary1.Model
{
    public static class PMCacheSubstitution
    {
        public static string LoginInfoStatus(HttpContext context)
        {
            return MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/CommonControl/LoginInfoStatus.ascx", null, null);
        }

        public static string MyPointsSidebar(HttpContext context)
        {
            return MoreStrings.MoreStringManip.RenderUnloadedUserControl("~/CommonControl/MyPointsSidebarContainer.ascx", null, null);
        }

    }
}