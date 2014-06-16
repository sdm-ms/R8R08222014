
using System.Linq;
using System.Web.Routing;
using System.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using MoreStrings;
using ClassLibrary1.Model;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public enum RouteID
    {
        HomePage,
        ChangePwd,
        Error,
        ForgotPwd,
        Help,
        Login,
        Logout,
        MainContent,
        MyAccount,
        MyPoints,
        Ratings,
        NarrowResults,
        NewUser,
        Privacy,
        Prizes,
        R8RDownload,
        Rules,
        SearchResults,
        TermsOfService
    }

    public class RoutingMap
    {
        public RouteID RouteID;
        public string Name;
        public string Pattern;
        public string PhysicalFile;
        public IRouteConstraint Constraint;
        public Func<RouteData, IR8RDataContext, RoutingInfo> Incoming;
        public RoutingMap(RouteID routeID, string physicalFile) // for a routing with no routedata
        {
            RouteID = routeID;
            Name = routeID.ToString();
            Pattern = routeID.ToString();
            PhysicalFile = physicalFile;
            Incoming = Routing.IncomingNoParameters;
        }
        public RoutingMap(RouteID routeID, string name, string pattern, string physicalFile, IRouteConstraint theConstraint, Func<RouteData, IR8RDataContext, RoutingInfo> incomingFunction)
        {
            RouteID = routeID;
            Name = name;
            Pattern = pattern;
            PhysicalFile = physicalFile;
            Constraint = theConstraint;
            Incoming = incomingFunction;
        }
    }

    public class RoutingInfo
    {
        public RouteID thePage;

        public RoutingInfo(RouteID page)
        {
            thePage = page;
        }
    }

    public class RoutingInfoMainContent : RoutingInfo
    {
        public List<HierarchyItem> theRoutingHierarchy;
        public List<HierarchyItem> theMenuHierarchy;
        public HierarchyItem lastItemInHierarchy { get { return theRoutingHierarchy == null ? null : theRoutingHierarchy.LastOrDefault(); } }
        public Domain theDomain { get { return (theTbl == null) ? null : theTbl.PointsManager.Domain; } }
        public PointsManager thePointsManager { get { return (theTbl == null) ? null : theTbl.PointsManager; } }
        public Tbl theTbl { get { return (lastItemInHierarchy == null) ? null : lastItemInHierarchy.Tbl; } }
        public TblRow theTblRow;
        public TblColumn theTblColumn;
        public bool addMode;
        public bool commentsMode;
        public bool leadersMode;
        public bool guaranteesMode;
        public bool changeTblMode;
        public bool pointsSettingsMode;
        public bool editMode;
        public bool isValid = true;

        public RoutingInfoMainContent()
            : base(RouteID.MainContent)
        {
        }

        public RoutingInfoMainContent(HierarchyItem hierarchyItem, TblRow tblRow = null, TblColumn TblColumn = null)
            : base(RouteID.MainContent)
        {
            theRoutingHierarchy = HierarchyItems.GetHierarchyAsList(hierarchyItem, true);
            theMenuHierarchy = HierarchyItems.GetHierarchyAsList(hierarchyItem, false);
            theTblRow = tblRow;
            theTblColumn = TblColumn;
        }

        public RoutingInfoMainContent(Tbl Tbl, TblRow tblRow, TblColumn TblColumn, bool isEditMode = false, bool isAddMode = false, bool isCommentsMode = false, bool isLeadersMode = false, bool isGuaranteesMode = false, bool isChangeTableMode = false, bool isPointsSettingsMode = false)
            : base(RouteID.MainContent)
        {
            theRoutingHierarchy = HierarchyItems.GetHierarchyAsList(HierarchyItems.GetHierarchyItemForTbl(Tbl), true);
            theTblRow = tblRow;
            theTblColumn = TblColumn;
            editMode = isEditMode;
            addMode = isAddMode;
            leadersMode = isLeadersMode;
            guaranteesMode = isGuaranteesMode;
            changeTblMode = isChangeTableMode;
            pointsSettingsMode = isPointsSettingsMode;
            commentsMode = isCommentsMode;
        }

        public RoutingInfoMainContent(IR8RDataContext theDataContext, string hierarchyString)
            : base(RouteID.MainContent)
        {
            string[] remainderOfHierarchy;
            HierarchyItem theHierarchyItem = HierarchyItems.GetHierarchyFromStrings(hierarchyString.Split('/').Select(x => PrettyURLEncode.UrlTextDecode(x)).ToArray(), out remainderOfHierarchy, true);
            if (theHierarchyItem == null)
            {
                isValid = false;
                return;
            }
            theRoutingHierarchy = HierarchyItems.GetHierarchyAsList(theHierarchyItem, true);
            theMenuHierarchy = HierarchyItems.GetHierarchyAsList(theHierarchyItem, false);

            addMode = false;
            commentsMode = false;
            leadersMode = false;
            guaranteesMode = false;
            changeTblMode = false;
            pointsSettingsMode = false;
            editMode = false;
            string tblRowString = null;
            string tblColumnString = null;
            if (remainderOfHierarchy.Count() >= 1)
            {
                if (remainderOfHierarchy[0] == "Add")
                {
                    addMode = true;
                    return;
                }
                if (remainderOfHierarchy[0] == "Comments")
                {
                    commentsMode = true;
                    return;
                }
                if (remainderOfHierarchy[0] == "Leaders")
                {
                    leadersMode = true;
                    return;
                }
                if (remainderOfHierarchy[0] == "Guarantees")
                {
                    guaranteesMode = true;
                    return;
                }
                if (remainderOfHierarchy[0] == "ChangeTable")
                {
                    changeTblMode = true;
                    return;
                }
                if (remainderOfHierarchy[0] == "PointsSettings")
                {
                    pointsSettingsMode = true;
                    return;
                }
                tblRowString = PrettyURLEncode.UrlTextDecode(remainderOfHierarchy[0]);
                int tblRowID = 0;
                if (tblRowString != null && tblRowString != "")
                {
                    try
                    {
                        tblRowID = Convert.ToInt32(tblRowString);
                    }
                    catch
                    {
                    }
                    theTblRow = theDataContext.GetTable<TblRow>().SingleOrDefault(x => x.Tbl == theTbl && x.TblRowID == tblRowID);
                }
                if (theTblRow == null)
                    return;

                if (remainderOfHierarchy.Count() >= 2)
                {
                    if (remainderOfHierarchy[1] == "Edit")
                    {
                        editMode = true;
                        return;
                    }
                    tblColumnString = PrettyURLEncode.UrlTextDecode(remainderOfHierarchy[1]);
                    int TblColumnID = 0;
                    try
                    {
                        TblColumnID = Convert.ToInt32(tblColumnString);
                    }
                    catch
                    {
                    }
                    if (TblColumnID != -1)
                        theTblColumn = theDataContext.GetTable<TblColumn>().Single(x => x.TblTab.Tbl == theTbl && x.TblColumnID == TblColumnID);
                }
            }
        }

        public string GetHierarchyRoutingParameterString()
        {
            string theHierarchyString = "";
            int itemCount = theRoutingHierarchy.Count();
            for (int i = 0; i < itemCount; i++)
            {
                if (i != 0)
                    theHierarchyString += "/";
                theHierarchyString += PrettyURLEncode.UrlTextEncode(theRoutingHierarchy[i].HierarchyItemName);
            }
            if (theTblRow != null)
                theHierarchyString += "/" + PrettyURLEncode.UrlTextEncode(theTblRow.TblRowID.ToString());
            if (theTblColumn != null)
                theHierarchyString += "/" + PrettyURLEncode.UrlTextEncode(theTblColumn.TblColumnID.ToString());
            if (addMode)
                theHierarchyString += "/Add";
            if (commentsMode)
                theHierarchyString += "/Comments";
            if (leadersMode)
                theHierarchyString += "/Leaders";
            if (guaranteesMode)
                theHierarchyString += "/Guarantees";
            if (changeTblMode)
                theHierarchyString += "/ChangeTable";
            if (pointsSettingsMode)
                theHierarchyString += "/PointsSettings";
            if (editMode)
                theHierarchyString += "/Edit";
            return theHierarchyString;
        }

        public string GetRouteName()
        {
            string theRoute;
            if (theTblRow != null)
            {
                if (theTblColumn != null)
                    theRoute = "mainRouteWithTblRowAndColumn";
                else
                {
                    if (editMode)
                        theRoute = "mainRouteEditTblRow";
                    else
                        theRoute = "mainRouteWithTblRow";
                }
            }
            else if (theTbl != null)
            {
                if (addMode)
                    theRoute = "mainRouteAddTblRow";
                else if (commentsMode)
                    theRoute = "mainRouteComments";
                else if (leadersMode)
                    theRoute = "mainRouteLeaders";
                else if (guaranteesMode)
                    theRoute = "mainRouteGuarantees";
                else if (changeTblMode)
                    theRoute = "mainRouteChangeTable";
                else if (pointsSettingsMode)
                    theRoute = "mainRoutePointsSettings";
                else
                    theRoute = "mainRoute";
            }
            else
                theRoute = "topicsRoute";
            return theRoute;
        }

        public string GetOutgoingRoute()
        {
            return Routing.Outgoing(this);
        }

        public string GetOutgoingRoute(int hierarchyLevel)
        {
            int hierarchyLevels = theRoutingHierarchy.Count();
            TblRow tblRow = null;
            TblColumn TblColumnToInclude = null;
            if (hierarchyLevel >= hierarchyLevels)
            {
                tblRow = theTblRow;
                if (hierarchyLevel >= hierarchyLevels + 1)
                    TblColumnToInclude = theTblColumn;
            }
            RoutingInfoMainContent routingForLevel = RoutingInfoMainContentFactory.GetRoutingInfo(theRoutingHierarchy[hierarchyLevel], tblRow, TblColumnToInclude);
            return routingForLevel.GetOutgoingRoute();
        }

        public int GetTotalLevels()
        {
            int totalLevels = theRoutingHierarchy.Count();
            if (theTblRow != null)
                totalLevels++;
            if (theTblColumn != null)
                totalLevels++;
            return totalLevels;
        }

        public string GetName()
        {
            int totalLevels = GetTotalLevels();
            return GetName(totalLevels - 1);
        }


        public string GetName(int hierarchyLevel)
        {
            int hierarchyLevels = theRoutingHierarchy.Count();
            if (hierarchyLevel < hierarchyLevels)
                return theRoutingHierarchy[hierarchyLevel].HierarchyItemName;
            else if (hierarchyLevel == hierarchyLevels && theTblRow != null)
                return theTblRow.Name;
            else if (hierarchyLevel == hierarchyLevels + 1 && theTblColumn != null)
                return theTblColumn.Name;
            else
                return "";
        }

        public void GetInfoAllLevels(out List<string> names, out List<string> routes)
        {
            int totalLevels = GetTotalLevels();
            names = new List<string>();
            routes = new List<string>();
            for (int level = 0; level < totalLevels; level++)
            {
                names.Add(GetName(level));
                routes.Add(GetOutgoingRoute(level));
            }
        }
    }

    public static class RoutingInfoMainContentFactory
    {
        public static RoutingInfoMainContent GetRoutingInfo(IR8RDataContext theDataContext, string hierarchyString)
        {
            string cacheKey = "HierarchyRoutingString" + hierarchyString;
            RoutingInfoMainContent theRoutingInfo;
            theRoutingInfo = CacheManagement.GetItemFromCache(cacheKey) as RoutingInfoMainContent;
            if (theRoutingInfo == null)
            {
                if (theDataContext == null)
                {
                    R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
                    theDataContext = theDataAccessModule.DataContext;
                }
                theRoutingInfo = new RoutingInfoMainContent(theDataContext, hierarchyString ?? "");             
                AddRoutingInfoToCache(cacheKey, theRoutingInfo);
            }
            return theRoutingInfo;
        }

        public static RoutingInfoMainContent GetRoutingInfo(HierarchyItem hierarchyItem, TblRow tblRow = null, TblColumn TblColumn = null)
        {
            string cacheKey = "HierarchyRouting" + hierarchyItem.GetHashCode() + tblRow.GetHashString() + TblColumn.GetHashString();
            RoutingInfoMainContent theRoutingInfo;
            theRoutingInfo = CacheManagement.GetItemFromCache(cacheKey) as RoutingInfoMainContent;
            if (theRoutingInfo == null)
            {
                theRoutingInfo = new RoutingInfoMainContent(hierarchyItem, tblRow, TblColumn);
                AddRoutingInfoToCache(cacheKey, theRoutingInfo);
            }
            return theRoutingInfo;
        }

        public static RoutingInfoMainContent GetRoutingInfo(Tbl theTbl, TblRow tblRow = null, TblColumn TblColumn = null)
        {
            string cacheKey = "HierarchyRoutingTbl" + theTbl.GetHashString() + tblRow.GetHashString() + TblColumn.GetHashString();
            RoutingInfoMainContent theRoutingInfo;
            theRoutingInfo = CacheManagement.GetItemFromCache(cacheKey) as RoutingInfoMainContent;
            if (theRoutingInfo == null)
            {
                theRoutingInfo = new RoutingInfoMainContent(HierarchyItems.GetHierarchyItemForTbl(theTbl), tblRow, TblColumn);
                AddRoutingInfoToCache(cacheKey, theRoutingInfo);
            }
            return theRoutingInfo;
        }

        private static void AddRoutingInfoToCache(string cacheKey, RoutingInfoMainContent theRoutingInfo)
        {
            List<string> theDependency = (theRoutingInfo.theDomain == null) ? new List<string>() : new List<string> { "DomainID" + theRoutingInfo.theDomain.DomainID };
            if (theRoutingInfo.theTblRow != null)
                theDependency.Add("FieldForTblRowID" + theRoutingInfo.theTblRow.TblRowID.ToString());
            if (theRoutingInfo.theTbl != null)
                theDependency.Add("FieldInfoForPointsManagerID" + theRoutingInfo.theTbl.PointsManagerID.ToString());
            CacheManagement.AddItemToCache(cacheKey, theDependency.ToArray(), theRoutingInfo);
        }

    }

    public class RoutingInfoRatings : RoutingInfo
    {
        public int? userID;

        public RoutingInfoRatings(int? theUserID)
            : base(RouteID.Ratings)
        {
            userID = theUserID;
        }
    }

    public class RoutingInfoSearchResults : RoutingInfo
    {
        public string searchTerms;

        public RoutingInfoSearchResults(string theSearchTerms)
            : base(RouteID.SearchResults)
        {
            searchTerms = theSearchTerms;
        }
    }

    public class RoutingInfoLoginRedirect : RoutingInfo
    {
        public string redirectURL;

        public RoutingInfoLoginRedirect(string theRedirectURL)
            : base(RouteID.Login)
        {
            redirectURL = theRedirectURL;
        }
    }

    public static class Routing
    {
        public static List<RoutingMap> routingMaps = new List<RoutingMap> {
        new RoutingMap(RouteID.HomePage, "HomePage", "", "WebForms/HomePage.aspx", null, IncomingNoParameters),
        new RoutingMap(RouteID.ChangePwd, "WebForms/ChangePwd.aspx"),
        new RoutingMap(RouteID.Error, "WebForms/Error.aspx"),
        new RoutingMap(RouteID.ForgotPwd, "WebForms/ForgotPwd.aspx"),
        new RoutingMap(RouteID.Help, "WebForms/Help.aspx"),
        new RoutingMap(RouteID.Login, "loginWithRedirect", "Login/{redirectURL}", "WebForms/Login.aspx", null, IncomingLoginRedirect),
        new RoutingMap(RouteID.Login, "WebForms/Login.aspx"),
        new RoutingMap(RouteID.Logout, "WebForms/Logout.aspx"),
        new RoutingMap(RouteID.MyAccount, "WebForms/MyAccount.aspx"),
        new RoutingMap(RouteID.MyPoints, "WebForms/MyPoints.aspx"),
        new RoutingMap(RouteID.Ratings, "ratingsWithUser", "Ratings/{userID}", "WebForms/Ratings.aspx", null, IncomingRatings),
        new RoutingMap(RouteID.Ratings, "ratings", "Ratings", "WebForms/Ratings.aspx", null, IncomingRatings),
        new RoutingMap(RouteID.NarrowResults, "WebForms/NarrowResults.aspx"),
        new RoutingMap(RouteID.NewUser, "WebForms/NewUser.aspx"),
        new RoutingMap(RouteID.Privacy, "WebForms/Privacy.aspx"),
        new RoutingMap(RouteID.Prizes, "WebForms/Prizes.aspx"),
        new RoutingMap(RouteID.R8RDownload, "R8RDownload.aspx"),
        new RoutingMap(RouteID.Rules, "WebForms/Rules.aspx"),
        new RoutingMap(RouteID.SearchResults, "searchResults", "Search/{searchTerms}", "WebForms/SearchResults.aspx", null, IncomingSearchResults),
        new RoutingMap(RouteID.TermsOfService, "WebForms/TermsOfService.aspx"),
        new RoutingMap(RouteID.MainContent, "mainRouteComments", "{*hierarchy}", "WebForms/TblComments.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRouteLeaders", "{*hierarchy}", "WebForms/Leaders.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRouteGuarantees", "{*hierarchy}", "WebForms/Guarantees.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRouteChangeTable", "{*hierarchy}", "Admin/Tbl/ChangeTbl.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRoutePointsSettings", "{*hierarchy}", "Admin/PointsManager/ChangePointsManager.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRouteAddTblRow", "{*hierarchy}", "WebForms/Row.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRouteEditTblRow", "{*hierarchy}", "WebForms/Row.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRouteWithTblRowAndColumn", "{*hierarchy}", "WebForms/Main.aspx", new HierarchyMatch(), IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRouteWithTblRow", "{*hierarchy}", "WebForms/Main.aspx", new HierarchyMatch(),  IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "topicsRoute", "{*hierarchy}",  "WebForms/HomePage.aspx", new HierarchyMatch(),  IncomingMainContent),
        new RoutingMap(RouteID.MainContent, "mainRoute", "{*hierarchy}", "WebForms/Main.aspx", new HierarchyMatch(), IncomingMainContent)
    };

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.Ignore("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
            routes.Ignore("{*allasmx}", new { allasmx = @".*\.asmx(/.*)?" });
            routes.Ignore("{*allaxd}", new { allaxd = @".*\.axd(/.*)?" });
            routes.Ignore("{*alljs}", new { alljs = @".*\.js(/.*)?" });
            routes.Ignore("{*allcss}", new { allcss = @".*\.css(/.*)?" });
            routes.Ignore("{*allgif}", new { allgif = @".*\.gif(/.*)?" });
            routes.Ignore("{*alljpg}", new { alljpg = @".*\.jpg(/.*)?" });
            routes.Ignore("{*allpng}", new { allpng = @".*\.png(/.*)?" });
            routes.Ignore("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.Ignore("{Forums*}", new { Forums = @"Forums(/.*)?" });
            foreach (RoutingMap theRoutingMap in routingMaps)
            {
                RouteValueDictionary constraints = new RouteValueDictionary();
                if (theRoutingMap.Constraint != null)
                    constraints.Add("hierarchy", theRoutingMap.Constraint);
                Route route = routes.MapPageRoute(theRoutingMap.Name, theRoutingMap.Pattern, "~/" + theRoutingMap.PhysicalFile, false, null, constraints);
                /* store the route name in the DataTokens of the route so that we can access it later */
                route.DataTokens = new RouteValueDictionary();
                route.DataTokens.Add("RouteName", theRoutingMap.Name);
            }
        }

        public static void OutgoingGetRoute(RoutingInfo theLocation, out string theRoute, out RouteValueDictionary parameters)
        {
            parameters = new RouteValueDictionary();

            Func<RoutingInfo, RouteValueDictionary, string> Outgoing;
            if (theLocation is RoutingInfoMainContent)
                Outgoing = OutgoingGetRouteMainContent;
            else if (theLocation is RoutingInfoSearchResults)
                Outgoing = OutgoingGetRouteSearchResults;
            else if (theLocation is RoutingInfoLoginRedirect)
                Outgoing = OutgoingGetRouteLoginRedirect;
            else if (theLocation is RoutingInfoRatings)
                Outgoing = OutgoingGetRouteRatings;
            else
                Outgoing = OutgoingGetRouteNoParameters;

            theRoute = Outgoing(theLocation, parameters);
        }

        public static string OutgoingGetRouteNoParameters(RoutingInfo theLocation, RouteValueDictionary parameters)
        {
            RoutingMap theMap = routingMaps.First(m => m.RouteID == theLocation.thePage && m.Name == m.RouteID.ToString());
            string theRoute = theMap.Name;
            return theRoute;
        }

        private static string OutgoingGetRouteRatings(RoutingInfo theRedirectInfo, RouteValueDictionary parameters)
        {
            int? userID = ((RoutingInfoRatings)theRedirectInfo).userID;
            if (userID == null)
                return "ratings";
            string userIDString = userID.ToString();
            string theRoute = "ratingsWithUser";
            parameters.Add("userID", PrettyURLEncode.UrlTextEncode(userIDString));
            return theRoute;
        }

        private static string OutgoingGetRouteLoginRedirect(RoutingInfo theRedirectInfo, RouteValueDictionary parameters)
        {
            string theRoute = "loginWithRedirect";
            parameters.Add("redirectURL", PrettyURLEncode.UrlTextEncode(((RoutingInfoLoginRedirect)theRedirectInfo).redirectURL));
            return theRoute;
        }

        private static string OutgoingGetRouteSearchResults(RoutingInfo theSearchResults, RouteValueDictionary parameters)
        {
            string theRoute = "searchResults";
            parameters.Add("searchTerms", PrettyURLEncode.UrlTextEncode(((RoutingInfoSearchResults)theSearchResults).searchTerms));
            return theRoute;
        }

        private static string OutgoingGetRouteMainContent(RoutingInfo theLocationBase, RouteValueDictionary parameters)
        {
            RoutingInfoMainContent theLocation = ((RoutingInfoMainContent)theLocationBase);
            parameters.Add("hierarchy", theLocation.GetHierarchyRoutingParameterString());
            return theLocation.GetRouteName();
        }

        public static void Redirect(HttpResponse theResponse, RoutingInfo theLocation)
        {
            RouteValueDictionary parameters;
            string theRoute;
            OutgoingGetRoute(theLocation, out theRoute, out parameters);

            theResponse.RedirectToRoute(theRoute, parameters);
            theResponse.End();
        }

        public static string Outgoing(RoutingInfo theLocation)
        {
            if (theLocation == null)
                theLocation = new RoutingInfo(RouteID.HomePage);

            RouteValueDictionary parameters;
            string theRoute;
            OutgoingGetRoute(theLocation, out theRoute, out parameters);

            string vpd = "";
            bool hasHttpContext = HttpContext.Current != null;
            if (hasHttpContext)
            {
                var virtualPath = RouteTable.Routes.GetVirtualPath(null, theRoute, parameters);
                if (virtualPath != null)
                    vpd = virtualPath.VirtualPath;
            }
            if (!hasHttpContext || vpd == "")
            {
                vpd = GetVirtualPathWithoutHttpContext(theRoute, parameters);
                if (!vpd.StartsWith("/"))
                    vpd = "/" + vpd;
            }
            return vpd;
        }

        static RouteCollection additionalRouteCollection = null;
        public static string GetVirtualPathWithoutHttpContext(string routeName, RouteValueDictionary values)
        {
            RouteBase route = RouteTable.Routes[routeName];
            if (route == null)
            {
                if (additionalRouteCollection == null)
                {
                    additionalRouteCollection = new RouteCollection();
                    RegisterRoutes(additionalRouteCollection);
                }
                route = additionalRouteCollection[routeName];
            }

            return route.GetVirtualPath(new RequestContext(new TempHttpContext("/"), new RouteData()), values).VirtualPath;
        }



        public static string OutgoingToCurrentRoute(RouteData theRouteData, IR8RDataContext theDataContext)
        {
            return Outgoing(Incoming(theRouteData, theDataContext));
        }

        public static string GetRouteName(Route theRoute)
        {
            return theRoute.DataTokens["RouteName"] as string;
        }

        public static RoutingMap GetRoutingMap(RouteData theRouteData)
        {
            return routingMaps.FirstOrDefault(x => x.Name == (theRouteData.DataTokens["RouteName"] as string));
        }

        public static RoutingMap GetRoutingMap(Route theRoute, HttpContextBase httpContext)
        {
            return GetRoutingMap(theRoute.GetRouteData(httpContext));
        }

        public static RoutingInfo Incoming(RouteData theRouteData, IR8RDataContext theDataContext)
        {
            RoutingMap currentRoutingMap = GetRoutingMap(theRouteData);
            if (currentRoutingMap != null)
                return currentRoutingMap.Incoming(theRouteData, theDataContext);
            else
                return null;
        }

        public static RoutingInfo IncomingNoParameters(RouteData theRouteData, IR8RDataContext theDataContext)
        {
            RoutingMap theRoutingMap = GetRoutingMap(theRouteData);
            return new RoutingInfo(theRoutingMap.RouteID);
        }

        public static RoutingInfoMainContent IncomingMainContent(RouteData theRouteData, IR8RDataContext theDataContext)
        {
            string hierarchyString = theRouteData.Values["hierarchy"] as string;
            RoutingInfoMainContent theLocation = RoutingInfoMainContentFactory.GetRoutingInfo(theDataContext, hierarchyString);
            return theLocation;
        }

        public static RoutingInfoRatings IncomingRatings(RouteData theRouteData, IR8RDataContext theDataContext)
        {
            string userIDString = theRouteData.Values["userID"] as string;
            if (userIDString == null)
                return new RoutingInfoRatings(null);
            try
            {
                int userID = Convert.ToInt32(userIDString);
                return new RoutingInfoRatings(userID);
            }
            catch
            {
                return new RoutingInfoRatings(null);
            }
        }

        public static RoutingInfoLoginRedirect IncomingLoginRedirect(RouteData theRouteData, IR8RDataContext theDataContext)
        {
            string redirectURL = theRouteData.Values["redirectURL"] as string;
            if (redirectURL == null)
                return new RoutingInfoLoginRedirect(Outgoing(new RoutingInfo(RouteID.HomePage)));
            redirectURL = PrettyURLEncode.UrlTextDecode(redirectURL);
            RoutingInfoLoginRedirect theRedirectInfo = new RoutingInfoLoginRedirect(redirectURL);
            return theRedirectInfo;
        }

        public static RoutingInfoSearchResults IncomingSearchResults(RouteData theRouteData, IR8RDataContext theDataContext)
        {
            string searchTerms = theRouteData.Values["searchTerms"] as string;
            if (searchTerms == null)
                throw new Exception("No search terms specified.");
            searchTerms = PrettyURLEncode.UrlTextDecode(searchTerms);
            RoutingInfoSearchResults theResults = new RoutingInfoSearchResults(searchTerms);
            return theResults;
        }


        public class TempHttpContext : HttpContextBase
        {
            private string applicationPath;
            private TempHttpRequest request;

            public TempHttpContext(string applicationPath)
            {
                this.applicationPath = applicationPath;
            }

            public override HttpRequestBase Request
            {
                get
                {
                    if (request == null)
                        request = new TempHttpRequest(applicationPath);

                    return request;
                }
            }

            private class TempHttpRequest : HttpRequestBase
            {
                private string applicationPath;

                public TempHttpRequest(string applicationPath)
                {
                    this.applicationPath = applicationPath;
                }

                public override string ApplicationPath
                {
                    get
                    {
                        return applicationPath;
                    }
                }
            }
        }

        public class HierarchyMatch : IRouteConstraint
        {

            #region IRouteConstraint Members

            public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
            {
                string routeName = GetRouteName(route);

                if (parameterName != "hierarchy")
                    return false;
                RoutingInfoMainContent theRoutingInfo = RoutingInfoMainContentFactory.GetRoutingInfo(null, values["hierarchy"] as string);
                if (!theRoutingInfo.isValid)
                    return false;

                switch (routeName)
                {
                    case "mainRouteAddTblRow":
                        return theRoutingInfo.addMode;
                    case "mainRouteComments":
                        return theRoutingInfo.commentsMode;
                    case "mainRouteLeaders":
                        return theRoutingInfo.leadersMode;
                    case "mainRouteGuarantees":
                        return theRoutingInfo.guaranteesMode;
                    case "mainRouteChangeTable":
                        return theRoutingInfo.changeTblMode;
                    case "mainRoutePointsSettings":
                        return theRoutingInfo.pointsSettingsMode;
                    case "mainRouteEditTblRow":
                        return theRoutingInfo.editMode;
                    case "mainRouteWithTblRowAndColumn":
                        return theRoutingInfo.theTblRow != null && theRoutingInfo.theTblColumn != null;
                    case "mainRouteWithTblRow":
                        return theRoutingInfo.theTblRow != null && theRoutingInfo.theTblColumn == null;
                    case "topicsRoute":
                        return theRoutingInfo.theTbl == null && theRoutingInfo.theRoutingHierarchy != null;
                    case "mainRoute":
                        return theRoutingInfo.theTbl != null && theRoutingInfo.theTblRow == null;
                }

                return false;
            }

            #endregion
        }
    }

}