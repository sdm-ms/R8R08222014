using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Misc
{
    public static class PrettyURLEncode
    {


        public static string UrlTextEncode(string theString)
        {
            SuperEncode(ref theString);
            Prettify(ref theString);
            theString = System.Web.HttpUtility.UrlEncode(theString);
            SuperEncode(ref theString);
            return theString;
        }

        public static string UrlTextDecode(string theString)
        {
            SuperDecode(ref theString);
            theString = System.Web.HttpUtility.UrlDecode(theString);
            Prettify(ref theString);
            SuperDecode(ref theString);
            return theString;
        }

        private static void SuperEncode(ref string theString)
        {
            /* some characters will return bad requests even if urlencoded, */
            /* when they are not part of a query string. */
            /* for most complete results, do this before and after urlencoding, */
            /* so that % character won't end up in URI. */
            if (theString == null)
                return;
            theString = theString.Replace("!", "!!");
            theString = theString.Replace(":", "!C");
            theString = theString.Replace("'", "!A");
            theString = theString.Replace("\"", "!Q");
            theString = theString.Replace("<", "!L");
            theString = theString.Replace(">", "!G");
            theString = theString.Replace("&", "!N");
            theString = theString.Replace("%", "!P");
            theString = theString.Replace("*", "!S");
            theString = theString.Replace("\\", "!B");
        }

        private static void SuperDecode(ref string theString)
        {
            /* some characters will return bad requests even if urlencoded. */
            if (theString == null)
                return;
            theString = theString.Replace("!!", "%%%%");
            theString = theString.Replace("!C", ":");
            theString = theString.Replace("!A", "'");
            theString = theString.Replace("!Q", "\"");
            theString = theString.Replace("!L", "<");
            theString = theString.Replace("!G", ">");
            theString = theString.Replace("!N", "&");
            theString = theString.Replace("!P", "%");
            theString = theString.Replace("!S", "*");
            theString = theString.Replace("!B", "\\");
            theString = theString.Replace("%%%%", "!");
        }


        private static void Prettify(ref string theString)
        {
            /* swap space and underscore to make it prettier when urlencoded */
            if (theString == null)
                return;
            theString = theString.Replace(" ", "%%%%");
            theString = theString.Replace("_", " ");
            theString = theString.Replace("%%%%", "_");
        }
    }
}
