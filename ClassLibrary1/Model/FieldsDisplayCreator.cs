using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Caching;

using System.Text.RegularExpressions;
using System.Linq.Expressions;
using ClassLibrary1.Model;
using System.Threading;

namespace ClassLibrary1.Model
{
    public class FieldDisplayHtml
    {
        public string theMainHtml;
        public string entityName;
        public bool fieldDataExists;
    }

    /// <summary>
    /// Summary description for FieldsDisplayCreator
    /// </summary>
    public class FieldsDisplayCreator
    {

        int maxWidth;
        int maxHeight;

        public void SetFieldDisplayHtmlForSomeNeedingResetting()
        {
            var dataAccess = new R8RDataAccess();
            TblRowPlusFieldInfoLoader theTblRowPlusFieldsInfoLoader = new TblRowPlusFieldInfoLoader();
            //ProfileSimple.Start("GetCompiledQuery");
            // Note that we use the following to load the data into a list, but we don't actually use the field info from the list. Because it's loaded in, it won't be loaded in again.
            List<TblRowPlusFieldInfos> theTblRowPlusFieldInfos = theTblRowPlusFieldsInfoLoader.GetTblRowPlusFieldInfos(dataAccess.R8RDB);
            //ProfileSimple.End("GetCompiledQuery");
            //ProfileSimple.Start("AfterCompiledQuery");

            foreach (var entityPlusFieldInfo in theTblRowPlusFieldInfos)
            {
                SetFieldDisplayHtml(entityPlusFieldInfo.TblRow);
            }

            //ProfileSimple.End("AfterCompiledQuery");
        }

        public void SetFieldDisplayHtmlWithoutFieldsForNow(TblRow theTblRow)
        {
            var dataAccess = new R8RDataAccess();
            TblDimensionAccess theCssAccess = new TblDimensionAccess(new R8RDataAccess());
            TblDimension theTblDimension = theCssAccess.GetTblDimensionsForRegularTbl(theTblRow.TblID);
            TblRowPlusFieldInfos tblRowInfo = TblRowPlusFieldInfoLoader.GetTblRowPlusFieldInfosWithoutFieldInfos(theTblRow);
            FieldDisplayHtml row = BuildFieldDisplayHtml(theTblDimension, FieldsLocation.RowHeading, theTblRow, tblRowInfo);
            FieldDisplayHtml rowPopup = BuildFieldDisplayHtml(theTblDimension, FieldsLocation.RowPopup, theTblRow, tblRowInfo);
            FieldDisplayHtml entityPage = BuildFieldDisplayHtml(theTblDimension, FieldsLocation.TblRowPage, theTblRow, tblRowInfo);
            theTblRow.TblRowFieldDisplay.Row = row.theMainHtml;
            theTblRow.TblRowFieldDisplay.PopUp = rowPopup.theMainHtml;
            theTblRow.TblRowFieldDisplay.TblRowPage = entityPage.theMainHtml;
        }

        public void SetFieldDisplayHtml(TblRow theTblRow)
        {
            var dataAccess = new R8RDataAccess();
            TblDimensionAccess theCssAccess = new TblDimensionAccess(new R8RDataAccess());
            TblDimension theTblDimension = theCssAccess.GetTblDimensionsForRegularTbl(theTblRow.TblID);
            FieldDisplayHtml row = BuildFieldDisplayHtml(theTblDimension, FieldsLocation.RowHeading, theTblRow);
            FieldDisplayHtml rowPopup = BuildFieldDisplayHtml(theTblDimension, FieldsLocation.RowPopup, theTblRow);
            FieldDisplayHtml entityPage = BuildFieldDisplayHtml(theTblDimension, FieldsLocation.TblRowPage, theTblRow);
            theTblRow.TblRowFieldDisplay.Row = row.theMainHtml;
            theTblRow.TblRowFieldDisplay.PopUp = rowPopup.theMainHtml;
            theTblRow.TblRowFieldDisplay.TblRowPage = entityPage.theMainHtml;
            theTblRow.TblRowFieldDisplay.ResetNeeded = false;
            theTblRow.InitialFieldsDisplaySet = true;

            TblRowFieldDisplay trfd = theTblRow.TblRowFieldDisplay;
            string newCompleteRowHeading = FastAccessTableInfo.FastAccessRowsInfo.GetCombinedRowHeadingWithPopup(trfd.Row, trfd.PopUp, theTblRow.Name, theTblRow.TblRowID);
            var faupdate = new FastAccessRowHeadingUpdateInfo() { RowHeading = newCompleteRowHeading };
            faupdate.AddToTblRow(theTblRow);

            CacheManagement.InvalidateCacheDependency("FieldForTblRowID" + theTblRow.TblRowID.ToString());
        }

        public FieldDisplayHtml GetFieldDisplayHtml(IR8RDataContext theDataContextToUse, int entityID, FieldsLocation theLocation)
        {

            string myCacheKey = "FieldDisplay" + entityID.ToString() + "," + ((int)theLocation).ToString();
            FieldDisplayHtml cachedResult = (FieldDisplayHtml)CacheManagement.GetItemFromCache(myCacheKey);
            if (cachedResult != null)
                return cachedResult;

            TblRow theTblRow = theDataContextToUse.GetTable<TblRow>().Single(e => e.TblRowID == entityID);

            FieldDisplayHtml theDisplayHtml = new FieldDisplayHtml();
            switch (theLocation)
            {
                case FieldsLocation.RowHeading:
                    theDisplayHtml.theMainHtml = theTblRow.TblRowFieldDisplay.Row as string;
                    break;


                case FieldsLocation.RowPopup:
                    theDisplayHtml.theMainHtml = theTblRow.TblRowFieldDisplay.PopUp as string;
                    break;


                case FieldsLocation.TblRowPage:
                    theDisplayHtml.theMainHtml = theTblRow.TblRowFieldDisplay.TblRowPage as string;
                    break;
            }

            if (theDisplayHtml.theMainHtml == null)
            { // if nothing is set (should not happen unless background process has not yet gotten to this row), set it and try again
                SetFieldDisplayHtml(theTblRow);
                return GetFieldDisplayHtml(theDataContextToUse, theTblRow.TblRowID, theLocation);
            }

            theDisplayHtml.theMainHtml = new Regex("/0").Replace(theDisplayHtml.theMainHtml, "/" + entityID.ToString());

            theDisplayHtml.entityName = theTblRow.Name;
            theDisplayHtml.fieldDataExists = !theDisplayHtml.theMainHtml.Contains("borderless nopop");

            string[] myDependencies = {
                    "FieldForTblRowID" + entityID.ToString(),
                    "FieldInfoForPointsManagerID" + theTblRow.Tbl.PointsManagerID.ToString()
                          };
            CacheManagement.AddItemToCache(myCacheKey, myDependencies, theDisplayHtml);

            return theDisplayHtml;
        }

        //public FieldDisplayHtml BuildFieldDisplayHtml(IR8RDataContext theDataContextToUse, TblDimension theTblDimension, FieldsLocation theLocation, int entityID)
        //{
        //    TblRow theTblRow = theDataContextToUse.TblRows.Single(e => e.TblRowID == entityID);
        //    return BuildFieldDisplayHtml(theDataContextToUse, theTblDimension, theLocation, theTblRow);
        //}



        public FieldDisplayHtml BuildFieldDisplayHtml(TblDimension theTblDimension, FieldsLocation theLocation, TblRow tblRow)
        {
            TblRowPlusFieldInfoLoader theTblRowPlusFieldsInfoLoader = new TblRowPlusFieldInfoLoader();
            //ProfileSimple.Start("GetCompiledQuery");
            TblRowPlusFieldInfos theTblRowPlusFieldInfos = theTblRowPlusFieldsInfoLoader.GetTblRowPlusFieldInfos(theLocation, tblRow);
            //ProfileSimple.End("GetCompiledQuery");
            //ProfileSimple.Start("AfterCompiledQuery");

            return BuildFieldDisplayHtml(theTblDimension, theLocation, tblRow, theTblRowPlusFieldInfos);
        }

        private FieldDisplayHtml BuildFieldDisplayHtml(TblDimension theTblDimension, FieldsLocation theLocation, TblRow tblRow, TblRowPlusFieldInfos theTblRowPlusFieldInfos)
        {
            switch (theLocation)
            {
                case FieldsLocation.RowHeading:
                    maxWidth = theTblDimension.MaxWidthOfImageInRowHeaderCell;
                    maxHeight = theTblDimension.MaxHeightOfImageInRowHeaderCell;
                    break;

                case FieldsLocation.RowPopup:
                    maxWidth = theTblDimension.MaxWidthOfImageInTblRowPopUpWindow;
                    maxHeight = theTblDimension.MaxHeightOfImageInTblRowPopUpWindow;
                    break;

                case FieldsLocation.TblRowPage:
                    maxWidth = 300;
                    maxHeight = 450;
                    break;
            }

            FieldDisplayHtml myFieldDisplayHtml = new FieldDisplayHtml();

            StringBuilder middleContent = new StringBuilder();
            IterateOverFields(middleContent, theTblRowPlusFieldInfos.Fields, theLocation);
            myFieldDisplayHtml.fieldDataExists = middleContent.Length > 0;
            string nopopString = myFieldDisplayHtml.fieldDataExists ? "" : " nopop";

            StringBuilder initialContent = new StringBuilder();
            initialContent.Append("<table class=\"borderless" + nopopString + "\" style=\"width:100%;\">");
            if (theLocation != FieldsLocation.TblRowPage)
            {
                myFieldDisplayHtml.entityName = theTblRowPlusFieldInfos.TblRow.Name;
                if (myFieldDisplayHtml.entityName != "")
                {
                    if (theLocation == FieldsLocation.RowHeading)
                    {
                        initialContent.Append("<tr><td><a href=\"");
                        initialContent.Append(Routing.Outgoing(new RoutingInfoMainContent(tblRow.Tbl, tblRow, null))); // The TblRowID might be 0, so we will need to replace that above
                        initialContent.Append("\">");
                        initialContent.Append(myFieldDisplayHtml.entityName);
                        initialContent.Append("</a></td></tr>");
                    }
                    else
                    { // FieldsLocation is for entity page or popup; no need for link.
                        //initialContent.Append("<tr><td><span>");
                        //initialContent.Append(myFieldDisplayHtml.entityName);
                        //initialContent.Append("</span></td></tr>");
                    }
                }
            }

            string finalContent = "</table>";

            middleContent.Append(finalContent);
            middleContent.Insert(0, initialContent.ToString());
            myFieldDisplayHtml.theMainHtml = middleContent.ToString();


            //ProfileSimple.End("AfterCompiledQuery");
            return myFieldDisplayHtml;
        }

        protected void IterateOverFields(StringBuilder myStringBuilder, IEnumerable<FieldDisplayInfoComplete> theFieldsToDisplay, FieldsLocation theLocation)
        {
            if (theFieldsToDisplay == null)
                return;
            StringBuilder imageForProminentDisplay = null;
            int numberIncluded = 0;
            bool addLatLngInfo = theLocation == FieldsLocation.RowHeading || theLocation == FieldsLocation.TblRowPage;
            string latLongInfo = "";
            foreach (FieldDisplayInfoComplete theFieldDisplayInfo in theFieldsToDisplay)
                ProcessField(ref numberIncluded, theFieldDisplayInfo, myStringBuilder, ref imageForProminentDisplay, ref addLatLngInfo, ref latLongInfo);
            if (myStringBuilder.Length > 0)
            {
                if (imageForProminentDisplay != null)
                {
                    myStringBuilder.Insert(0, imageForProminentDisplay);
                    imageForProminentDisplay = null;
                }
                myStringBuilder.Insert(0, "<tr><td>");
                myStringBuilder.Insert(myStringBuilder.Length, "</td></tr>");
            }
            if (latLongInfo != "")
            {
                myStringBuilder.Insert(0, latLongInfo);
                latLongInfo = "";
            }
        }

        protected void ProcessField(ref int numberIncludedAlready, FieldDisplayInfoComplete theField, StringBuilder myStringBuilder, ref StringBuilder imageForProminentDisplay, ref bool addLatLngInfo, ref string latLongInfo)
        {
            bool includeThisOne = true;

            StringBuilder myStringBuilder2 = new StringBuilder();

            FieldsDisplaySettingsMask myMask = new FieldsDisplaySettingsMask();

            if (theField.TheAddressField != null && addLatLngInfo)
            {
                if (theField.TheAddressField.Latitude != null && theField.TheAddressField.Longitude != null)
                {
                    latLongInfo = "<tr class=\"latlong\" lat=\"{0}\" long=\"{1}\"><td/></tr>";
                    latLongInfo = String.Format(latLongInfo, theField.TheAddressField.Latitude.ToString(), theField.TheAddressField.Longitude.ToString());
                }
                else
                {
                    latLongInfo = "<tr class=\"latlong\" lat=\"\" long=\"\" address=\"{0}\"><td/></tr>";
                    latLongInfo = String.Format(latLongInfo, theField.TheAddressField.AddressString);
                }
                addLatLngInfo = false; // We want to include only one address for each row.
            }

            if ((theField.DisplaySettings & myMask.Visible) != myMask.Visible)
                return;

            bool newLineBeforeFieldName = (theField.DisplaySettings & myMask.NewLineBeforeFieldName) == myMask.NewLineBeforeFieldName;
            bool includeFieldName = (theField.DisplaySettings & myMask.IncludeFieldName) == myMask.IncludeFieldName;
            bool newLineBeforeFieldValue = (theField.DisplaySettings & myMask.NewLineBeforeFieldValue) == myMask.NewLineBeforeFieldValue;
            if (newLineBeforeFieldValue && !includeFieldName)
            {
                newLineBeforeFieldName = true;
                newLineBeforeFieldValue = false;
            }
            if (!newLineBeforeFieldName && includeFieldName && newLineBeforeFieldValue)
                newLineBeforeFieldValue = false; // looks odd this way.


            if (newLineBeforeFieldName && numberIncludedAlready != 0)
                // myStringBuilder2.Append("</td></tr><tr><td>");
                myStringBuilder2.Append("<br/>");
            else
                myStringBuilder2.Append(" ");
            if (includeFieldName)
                myStringBuilder2.Append(theField.FieldDesc.FieldName + ": ");
            if (newLineBeforeFieldValue)
                myStringBuilder2.Append("<br />");

            bool thisIsImageForProminentDisplay = false;

            switch ((FieldTypes)theField.FieldDesc.FieldType)
            {
                case FieldTypes.AddressField:
                    if (theField.TheAddressField == null || theField.TheAddressField.AddressString == null || theField.TheAddressField.AddressString == "")
                        includeThisOne = false;
                    else
                        myStringBuilder2.Append(theField.TheAddressField.AddressString);
                    break;

                case FieldTypes.ChoiceField:
                    if (theField.TheChoiceField == null || theField.TheChoices == null || !theField.TheChoices.Any())
                        includeThisOne = false;
                    else
                    {
                        var choices = theField.TheChoices.Select(x => x.ChoiceText).ToArray();
                        int choicesCount = choices.Count();
                        for (int choiceNum = 1; choiceNum <= choicesCount; choiceNum++)
                        {
                            myStringBuilder2.Append(choices[choiceNum - 1]);
                            if (choiceNum != choicesCount)
                                myStringBuilder2.Append(", ");
                        }
                    }
                    break;

                case FieldTypes.DateTimeField:
                    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    if (theField.TheDateTimeField == null || theField.TheDateTimeField.DateTime == null)
                        includeThisOne = false;
                    else
                    {
                        if (theField.TheDateTimeFieldDesc.IncludeDate == true && theField.TheDateTimeFieldDesc.IncludeTime == true)
                        {
                            myStringBuilder2.Append(theField.TheDateTimeField.DateTime.ToString());
                        }
                        else if (theField.TheDateTimeFieldDesc.IncludeDate == true && theField.TheDateTimeFieldDesc.IncludeTime == false)
                        {
                            myStringBuilder2.Append(theField.TheDateTimeField.DateTime.Value.ToShortDateString());
                        }
                        else if (theField.TheDateTimeFieldDesc.IncludeDate == false && theField.TheDateTimeFieldDesc.IncludeTime == true)
                        {
                            myStringBuilder2.Append(theField.TheDateTimeField.DateTime.Value.ToShortTimeString());
                        }
                        else if (theField.TheDateTimeFieldDesc.IncludeDate == false && theField.TheDateTimeFieldDesc.IncludeTime == false)
                        {
                            myStringBuilder2.Append(theField.TheDateTimeField.DateTime.ToString());
                        }
                    }
                    break;

                case FieldTypes.NumberField:
                    if (theField.TheNumberField == null || theField.TheNumberField.Number == null)
                        includeThisOne = false;
                    else
                    {
                        decimal theNumber = (decimal)theField.TheNumberField.Number;
                        string theNumberString;
                        if (theField.TheNumberFieldDesc != null)
                            theNumberString = MoreStrings.MoreStringManip.FormatToExactDecimalPlaces(theNumber, theField.TheNumberFieldDesc.DecimalPlaces);
                        else
                            theNumberString = theNumber.ToString();
                        myStringBuilder2.Append(theNumberString);
                    }
                    break;

                case FieldTypes.TextField:
                    if (theField.TheTextField == null || theField.TheTextField.Text == null || (theField.TheTextField.Text == "" && theField.TheTextField.Link == ""))
                        includeThisOne = false;
                    else
                    {
                        string Link = theField.TheTextField.Link;

                        if (Link.Trim() == "")
                        {
                            myStringBuilder2.Append(theField.TheTextField.Text);
                        }
                        else
                        {
                            if (Link.Contains("yimg.com/nimage") || Link.EndsWith("jpg") || Link.EndsWith("jpeg") || Link.EndsWith("gif") || Link.EndsWith("png") || Link.EndsWith("bmp"))
                            {
                                bool displayInTopRight = (theField.DisplaySettings & myMask.DisplayInTopRightCorner) == myMask.DisplayInTopRightCorner;
                                string linkName;
                                if (Link.StartsWith("http://"))
                                    linkName = Link;
                                else
                                    linkName = "http://" + Link;
                                if (displayInTopRight)
                                {
                                    thisIsImageForProminentDisplay = true;
                                    imageForProminentDisplay = new StringBuilder();
                                    imageForProminentDisplay.Append("<img src=\"");
                                    imageForProminentDisplay.Append(linkName);
                                    imageForProminentDisplay.Append("\" class=\"imageInField imageFloat\" ");
                                    imageForProminentDisplay.Append(" style=\"max-width:" + maxWidth.ToString() + "px; max-height:" + maxHeight.ToString() + "px;\" ");
                                    imageForProminentDisplay.Append(" />");
                                }
                                else
                                {
                                    myStringBuilder2.Append("<img src=\"");
                                    myStringBuilder2.Append(linkName);
                                    myStringBuilder2.Append("\" class=\"imageInField\" ");
                                    myStringBuilder2.Append(" style=\"max-width:" + maxWidth.ToString() + "px; max-height:" + maxHeight.ToString() + "px;\" ");
                                    myStringBuilder2.Append(" />");
                                }
                            }
                            else
                            { // Not a link to a picture
                                myStringBuilder2.Append("<a href=\"");
                                if (!Link.StartsWith("http://"))
                                    myStringBuilder2.Append("http://");
                                myStringBuilder2.Append(Link);
                                myStringBuilder2.Append("\">");
                                myStringBuilder2.Append(theField.TheTextField.Text);
                                myStringBuilder2.Append("</a>");
                            }
                        }
                    }
                    break;

                default:
                    throw new Exception("Unknown Field type in FieldsType:AddItemToTableRow");

            }

            if (includeThisOne && !thisIsImageForProminentDisplay) // image for prominent display will be handled separately
            {
                myStringBuilder.Append(myStringBuilder2.ToString());
                numberIncludedAlready++;
            }
        }
    }
}