
using System.Linq;
using System.Web.Routing;
using System.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using MoreStrings;
using ClassLibrary1.Model;

namespace ClassLibrary1.Model
{
    public enum PMRouteID
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
        RaterooDownload,
        Rules,
        SearchResults,
        TermsOfService
    }

    public class PMRoutingMap
    {
        public PMRouteID RouteID;
        public string Name;
        public string Pattern;
        public string PhysicalFile;
        public IRouteConstraint Constraint;
        public Func<RouteData, IRaterooDataContext, PMRoutingInfo> Incoming;
        public PMRoutingMap(PMRouteID routeID, string physicalFile) // for a routing with no routedata
        {
            RouteID = routeID;
            Name = routeID.ToString();
            Pattern = routeID.ToString();
            PhysicalFile = physicalFile;
            Incoming = PMRouting.IncomingNoParameters;
        }
        public PMRoutingMap(PMRouteID routeID, string name, string pattern, string physicalFile, IRouteConstraint theConstraint, Func<RouteData, IRaterooDataContext, PMRoutingInfo> incomingFunction)
        {
            RouteID = routeID;
            Name = name;
            Pattern = pattern;
            PhysicalFile = physicalFile;
            Constraint = theConstraint;
            Incoming = incomingFunction;
        }
    }

    public class PMRoutingInfo
    {
        public PMRouteID thePage;

        public PMRoutingInfo(PMRouteID page)
        {
            thePage = page;
        }
    }

    public class PMRoutingInfoMainContent : PMRoutingInfo
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

        public PMRoutingInfoMainContent()
            : base(PMRouteID.MainContent)
        {
        }

        public PMRoutingInfoMainContent(HierarchyItem hierarchyItem, TblRow entity = null, TblColumn TblColumn = null)
            : base(PMRouteID.MainContent)
        {
            theRoutingHierarchy = PMHierarchyItems.GetHierarchyAsList(hierarchyItem, true);
            theMenuHierarchy = PMHierarchyItems.GetHierarchyAsList(hierarchyItem, false);
            theTblRow = entity;
            theTblColumn = TblColumn;
        }

        public PMRoutingInfoMainContent(Tbl Tbl, TblRow entity, TblColumn TblColumn, bool isEditMode = false, bool isAddMode = false, bool isCommentsMode = false, bool isLeadersMode = false, bool isGuaranteesMode = false, bool isChangeTableMode = false, bool isPointsSettingsMode = false)
            : base(PMRouteID.MainContent)
        {
            theRoutingHierarchy = PMHierarchyItems.GetHierarchyAsList(PMHierarchyItems.GetHierarchyItemForTbl(Tbl), true);
            theTblRow = entity;
            theTblColumn = TblColumn;
            editMode = isEditMode;
            addMode = isAddMode;
            leadersMode = isLeadersMode;
            guaranteesMode = isGuaranteesMode;
            changeTblMode = isChangeTableMode;
            pointsSettingsMode = isPointsSettingsMode;
            commentsMode = isCommentsMode;
        }

        public PMRoutingInfoMainContent(IRaterooDataContext theDataContext, string hierarchyString)
            : base(PMRouteID.MainContent)
        {
            string[] remainderOfHierarchy;
            HierarchyItem theHierarchyItem = PMHierarchyItems.GetHierarchyFromStrings(hierarchyString.Split('/').Select(x => PMRouting.UrlTextDecode(x)).ToArray(), out remainderOfHierarchy, true);
            if (theHierarchyItem == null)
            {
                isValid = false;
                return;
            }
            theRoutingHierarchy = PMHierarchyItems.GetHierarchyAsList(theHierarchyItem, true);
            theMenuHierarchy = PMHierarchyItems.GetHierarchyAsList(theHierarchyItem, false);

            addMode = false;
            commentsMode = false;
            leadersMode = false;
            guaranteesMode = false;
            changeTblMode = false;
            pointsSettingsMode = false;
            editMode = false;
            string entityString = null;
            string categoryString = null;
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
                entityString = PMRouting.UrlTextDecode(remainderOfHierarchy[0]);
                int entityID = 0;
                if (entityString != null && entityString != "")
                {
                    try
                    {
                        entityID = Convert.ToInt32(entityString);
                    }
                    catch
                    {
                    }
                    theTblRow = theDataContext.GetTable<TblRow>().SingleOrDefault(x => x.Tbl == theTbl && x.TblRowID == entityID);
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
                    categoryString = PMRouting.UrlTextDecode(remainderOfHierarchy[1]);
                    int TblColumnID = 0;
                    try
                    {
                        TblColumnID = Convert.ToInt32(categoryString);
                    }
                    catch
                    {
                    }
                    if (TblColumnID != 0)
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
                theHierarchyString += PMRouting.UrlTextEncode(theRoutingHierarchy[i].HierarchyItemName);
            }
            if (theTblRow != null)
                theHierarchyString += "/" + PMRouting.UrlTextEncode(theTblRow.TblRowID.ToString());
            if (theTblColumn != null)
                theHierarchyString += "/" + PMRouting.UrlTextEncode(theTblColumn.TblColumnID.ToString());
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
                    theRoute = "mainRouteWithTblRowAndCategory";
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
            return PMRouting.Outgoing(this);
        }

        public string GetOutgoingRoute(int hierarchyLevel)
        {
            int hierarchyLevels = theRoutingHierarchy.Count();
            TblRow entityToInclude = null;
            TblColumn TblColumnToInclude = null;
            if (hierarchyLevel >= hierarchyLevels)
            {
                entityToInclude = theTblRow;
                if (hierarchyLevel >= hierarchyLevels + 1)
                    TblColumnToInclude = theTblColumn;
            }
            PMRoutingInfoMainContent routingForLevel = PMRoutingInfoMainContentFactory.GetRoutingInfo(theRoutingHierarchy[hierarchyLevel], entityToInclude, TblColumnToInclude);
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

    public static class PMRoutingInfoMainContentFactory
    {
        public static PMRoutingInfoMainContent GetRoutingInfo(IRaterooDataContext theDataContext, string hierarchyString)
        {
            string cacheKey = "HierarchyRoutingString" + hierarchyString;
            PMRoutingInfoMainContent theRoutingInfo;
            theRoutingInfo = PMCacheManagement.GetItemFromCache(cacheKey) as PMRoutingInfoMainContent;
            if (theRoutingInfo == null)
            {
                if (theDataContext == null)
                {
                    RaterooDataManipulation theDataAccessModule = new RaterooDataManipulation();
                    theDataContext = theDataAccessModule.DataContext;
                }
                theRoutingInfo = new PMRoutingInfoMainContent(theDataContext, hierarchyString ?? ""); AddRoutingInfoToCache(cacheKey, theRoutingInfo);
            }
            return theRoutingInfo;
        }

        public static PMRoutingInfoMainContent GetRoutingInfo(HierarchyItem hierarchyItem, TblRow entity = null, TblColumn TblColumn = null)
        {
            string cacheKey = "HierarchyRouting" + hierarchyItem.GetHashCode() + entity.GetHashString() + TblColumn.GetHashString();
            PMRoutingInfoMainContent theRoutingInfo;
            theRoutingInfo = PMCacheManagement.GetItemFromCache(cacheKey) as PMRoutingInfoMainContent;
            if (theRoutingInfo == null)
            {
                theRoutingInfo = new PMRoutingInfoMainContent(hierarchyItem, entity, TblColumn);
                AddRoutingInfoToCache(cacheKey, theRoutingInfo);
            }
            return theRoutingInfo;
        }

        public static PMRoutingInfoMainContent GetRoutingInfo(Tbl theTbl, TblRow entity = null, TblColumn TblColumn = null)
        {
            string cacheKey = "HierarchyRoutingTbl" + theTbl.GetHashString() + entity.GetHashString() + TblColumn.GetHashString();
            PMRoutingInfoMainContent theRoutingInfo;
            theRoutingInfo = PMCacheManagement.GetItemFromCache(cacheKey) as PMRoutingInfoMainContent;
            if (theRoutingInfo == null)
            {
                theRoutingInfo = new PMRoutingInfoMainContent(PMHierarchyItems.GetHierarchyItemForTbl(theTbl), entity, TblColumn);
                AddRoutingInfoToCache(cacheKey, theRoutingInfo);
            }
            return theRoutingInfo;
        }

        private static void AddRoutingInfoToCache(string cacheKey, PMRoutingInfoMainContent theRoutingInfo)
        {
            List<string> theDependency = (theRoutingInfo.theDomain == null) ? new List<string>() : new List<string> { "DomainID" + theRoutingInfo.theDomain.DomainID };
            if (theRoutingInfo.theTblRow != null)
                theDependency.Add("FieldForTblRowID" + theRoutingInfo.theTblRow.TblRowID.ToString());
            if (theRoutingInfo.theTbl != null)
                theDependency.Add("FieldInfoForPointsManagerID" + theRoutingInfo.theTbl.PointsManagerID.ToString());
            PMCacheManagement.AddItemToCache(cacheKey, theDependency.ToArray(), theRoutingInfo);
        }

    }

    public class PMRoutingInfoRatings : PMRoutingInfo
    {
        public int? userID;

        public PMRoutingInfoRatings(int? theUserID)
            : base(PMRouteID.Ratings)
        {
            userID = theUserID;
        }
    }

    public class PMRoutingInfoSearchResults : PMRoutingInfo
    {
        public string searchTerms;

        public PMRoutingInfoSearchResults(string theSearchTerms)
            : base(PMRouteID.SearchResults)
        {
            searchTerms = theSearchTerms;
        }
    }

    public class PMRoutingInfoLoginRedirect : PMRoutingInfo
    {
        public string redirectURL;

        public PMRoutingInfoLoginRedirect(string theRedirectURL)
            : base(PMRouteID.Login)
        {
            redirectURL = theRedirectURL;
        }
    }

    public static class PMRouting
    {
        public static List<PMRoutingMap> routingMaps = new List<PMRoutingMap> {
        new PMRoutingMap(PMRouteID.HomePage, "HomePage", "", "WebForms/HomePage.aspx", null, IncomingNoParameters),
        new PMRoutingMap(PMRouteID.ChangePwd, "WebForms/ChangePwd.aspx"),
        new PMRoutingMap(PMRouteID.Error, "WebForms/Error.aspx"),
        new PMRoutingMap(PMRouteID.ForgotPwd, "WebForms/ForgotPwd.aspx"),
        new PMRoutingMap(PMRouteID.Help, "WebForms/Help.aspx"),
        new PMRoutingMap(PMRouteID.Login, "loginWithRedirect", "Login/{redirectURL}", "WebForms/Login.aspx", null, IncomingLoginRedirect),
        new PMRoutingMap(PMRouteID.Login, "WebForms/Login.aspx"),
        new PMRoutingMap(PMRouteID.Logout, "WebForms/Logout.aspx"),
        new PMRoutingMap(PMRouteID.MyAccount, "WebForms/MyAccount.aspx"),
        new PMRoutingMap(PMRouteID.MyPoints, "WebForms/MyPoints.aspx"),
        new PMRoutingMap(PMRouteID.Ratings, "ratingsWithUser", "Ratings/{userID}", "WebForms/Ratings.aspx", null, IncomingRatings),
        new PMRoutingMap(PMRouteID.Ratings, "ratings", "Ratings", "WebForms/Ratings.aspx", null, IncomingRatings),
        new PMRoutingMap(PMRouteID.NarrowResults, "WebForms/NarrowResults.aspx"),
        new PMRoutingMap(PMRouteID.NewUser, "WebForms/NewUser.aspx"),
        new PMRoutingMap(PMRouteID.Privacy, "WebForms/Privacy.aspx"),
        new PMRoutingMap(PMRouteID.Prizes, "WebForms/Prizes.aspx"),
        new PMRoutingMap(PMRouteID.RaterooDownload, "RaterooDownload.aspx"),
        new PMRoutingMap(PMRouteID.Rules, "WebForms/Rules.aspx"),
        new PMRoutingMap(PMRouteID.SearchResults, "searchResults", "Search/{searchTerms}", "WebForms/SearchResults.aspx", null, IncomingSearchResults),
        new PMRoutingMap(PMRouteID.TermsOfService, "WebForms/TermsOfService.aspx"),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteComments", "{*hierarchy}", "WebForms/TblComments.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteLeaders", "{*hierarchy}", "WebForms/Leaders.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteGuarantees", "{*hierarchy}", "WebForms/Guarantees.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteChangeTable", "{*hierarchy}", "Admin/Tbl/ChangeTbl.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRoutePointsSettings", "{*hierarchy}", "Admin/PointsManager/ChangePointsManager.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteAddTblRow", "{*hierarchy}", "WebForms/Row.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteEditTblRow", "{*hierarchy}", "WebForms/Row.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteWithTblRowAndCategory", "{*hierarchy}", "WebForms/Main.aspx", new HierarchyMatch(), IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRouteWithTblRow", "{*hierarchy}", "WebForms/Main.aspx", new HierarchyMatch(),  IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "topicsRoute", "{*hierarchy}",  "WebForms/HomePage.aspx", new HierarchyMatch(),  IncomingMainContent),
        new PMRoutingMap(PMRouteID.MainContent, "mainRoute", "{*hierarchy}", "WebForms/Main.aspx", new HierarchyMatch(), IncomingMainContent)
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
            foreach (PMRoutingMap theRoutingMap in routingMaps)
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

        public static void OutgoingGetRoute(PMRoutingInfo theLocation, out string theRoute, out RouteValueDictionary parameters)
        {
            parameters = new RouteValueDictionary();

            Func<PMRoutingInfo, RouteValueDictionary, string> Outgoing;
            if (theLocation is PMRoutingInfoMainContent)
                Outgoing = OutgoingGetRouteMainContent;
            else if (theLocation is PMRoutingInfoSearchResults)
                Outgoing = OutgoingGetRouteSearchResults;
            else if (theLocation is PMRoutingInfoLoginRedirect)
                Outgoing = OutgoingGetRouteLoginRedirect;
            else if (theLocation is PMRoutingInfoRatings)
                Outgoing = OutgoingGetRouteRatings;
            else
                Outgoing = OutgoingGetRouteNoParameters;

            theRoute = Outgoing(theLocation, parameters);
        }

        public static string OutgoingGetRouteNoParameters(PMRoutingInfo theLocation, RouteValueDictionary parameters)
        {
            PMRoutingMap theMap = routingMaps.First(m => m.RouteID == theLocation.thePage && m.Name == m.RouteID.ToString());
            string theRoute = theMap.Name;
            return theRoute;
        }

        private static string OutgoingGetRouteRatings(PMRoutingInfo theRedirectInfo, RouteValueDictionary parameters)
        {
            int? userID = ((PMRoutingInfoRatings)theRedirectInfo).userID;
            if (userID == null)
                return "ratings";
            string userIDString = userID.ToString();
            string theRoute = "ratingsWithUser";
            parameters.Add("userID", UrlTextEncode(userIDString));
            return theRoute;
        }

        private static string OutgoingGetRouteLoginRedirect(PMRoutingInfo theRedirectInfo, RouteValueDictionary parameters)
        {
            string theRoute = "loginWithRedirect";
            parameters.Add("redirectURL", UrlTextEncode(((PMRoutingInfoLoginRedirect)theRedirectInfo).redirectURL));
            return theRoute;
        }

        private static string OutgoingGetRouteSearchResults(PMRoutingInfo theSearchResults, RouteValueDictionary parameters)
        {
            string theRoute = "searchResults";
            parameters.Add("searchTerms", UrlTextEncode(((PMRoutingInfoSearchResults)theSearchResults).searchTerms));
            return theRoute;
        }

        private static string OutgoingGetRouteMainContent(PMRoutingInfo theLocationBase, RouteValueDictionary parameters)
        {
            PMRoutingInfoMainContent theLocation = ((PMRoutingInfoMainContent)theLocationBase);
            parameters.Add("hierarchy", theLocation.GetHierarchyRoutingParameterString());
            return theLocation.GetRouteName();
        }

        public static void Redirect(HttpResponse theResponse, PMRoutingInfo theLocation)
        {
            RouteValueDictionary parameters;
            string theRoute;
            OutgoingGetRoute(theLocation, out theRoute, out parameters);

            theResponse.RedirectToRoute(theRoute, parameters);
            theResponse.End();
        }

        public static string Outgoing(PMRoutingInfo theLocation)
        {
            if (theLocation == null)
                theLocation = new PMRoutingInfo(PMRouteID.HomePage);

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



        public static string OutgoingToCurrentRoute(RouteData theRouteData, IRaterooDataContext theDataContext)
        {
            return Outgoing(Incoming(theRouteData, theDataContext));
        }

        public static string GetRouteName(Route theRoute)
        {
            return theRoute.DataTokens["RouteName"] as string;
        }

        public static PMRoutingMap GetRoutingMap(RouteData theRouteData)
        {
            return routingMaps.FirstOrDefault(x => x.Name == (theRouteData.DataTokens["RouteName"] as string));
        }

        public static PMRoutingMap GetRoutingMap(Route theRoute, HttpContextBase httpContext)
        {
            return GetRoutingMap(theRoute.GetRouteData(httpContext));
        }

        public static PMRoutingInfo Incoming(RouteData theRouteData, IRaterooDataContext theDataContext)
        {
            PMRoutingMap currentRoutingMap = GetRoutingMap(theRouteData);
            if (currentRoutingMap != null)
                return currentRoutingMap.Incoming(theRouteData, theDataContext);
            else
                return null;
        }

        public static PMRoutingInfo IncomingNoParameters(RouteData theRouteData, IRaterooDataContext theDataContext)
        {
            PMRoutingMap theRoutingMap = GetRoutingMap(theRouteData);
            return new PMRoutingInfo(theRoutingMap.RouteID);
        }

        public static PMRoutingInfoMainContent IncomingMainContent(RouteData theRouteData, IRaterooDataContext theDataContext)
        {
            string hierarchyString = theRouteData.Values["hierarchy"] as string;
            PMRoutingInfoMainContent theLocation = PMRoutingInfoMainContentFactory.GetRoutingInfo(theDataContext, hierarchyString);
            return theLocation;
        }

        public static PMRoutingInfoRatings IncomingRatings(RouteData theRouteData, IRaterooDataContext theDataContext)
        {
            string userIDString = theRouteData.Values["userID"] as string;
            if (userIDString == null)
                return new PMRoutingInfoRatings(null);
            try
            {
                int userID = Convert.ToInt32(userIDString);
                return new PMRoutingInfoRatings(userID);
            }
            catch
            {
                return new PMRoutingInfoRatings(null);
            }
        }

        public static PMRoutingInfoLoginRedirect IncomingLoginRedirect(RouteData theRouteData, IRaterooDataContext theDataContext)
        {
            string redirectURL = theRouteData.Values["redirectURL"] as string;
            if (redirectURL == null)
                return new PMRoutingInfoLoginRedirect(Outgoing(new PMRoutingInfo(PMRouteID.HomePage)));
            redirectURL = UrlTextDecode(redirectURL);
            PMRoutingInfoLoginRedirect theRedirectInfo = new PMRoutingInfoLoginRedirect(redirectURL);
            return theRedirectInfo;
        }

        public static PMRoutingInfoSearchResults IncomingSearchResults(RouteData theRouteData, IRaterooDataContext theDataContext)
        {
            string searchTerms = theRouteData.Values["searchTerms"] as string;
            if (searchTerms == null)
                throw new Exception("No search terms specified.");
            searchTerms = UrlTextDecode(searchTerms);
            PMRoutingInfoSearchResults theResults = new PMRoutingInfoSearchResults(searchTerms);
            return theResults;
        }

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
                PMRoutingInfoMainContent theRoutingInfo = PMRoutingInfoMainContentFactory.GetRoutingInfo(null, values["hierarchy"] as string);
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
                    case "mainRouteWithTblRowAndCategory":
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