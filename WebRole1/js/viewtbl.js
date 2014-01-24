
function viewtbl() {

    var badBrowser = false;
    var mainTable = null;
    var mainTableBody = null;
    var divAroundMainTable = null;
    var rightPageColumn = null;
    var mainTableOrigWidth = null;
    var divAroundMainTableOrigWidth = null;
    var rightPageColumnOrigWidth = null;
    var mainPartOfPageOrigWidth = null;
    var tableInfo = null;
    var theCellWithFocusInfo = null;
    var targettedInput = null;
    var waitingForProcessing = null;
    var numberMessages = 0;
    var numberSelections = 0;
    var numberCurrentSelections = 0;
    var theUserAccessInfo = null;
    var rowsVisibleEstimate = 18;
    /*The height of a row*/
    var rowHeight = 50;
    var pendingServerCalls = 0;
    /*the number of extra rows to load when populating the table */
    var rowsInitialToLoad = 15;
    var rowsBuffer = 5;
    var rowCount = 0;
    /*store cellInfo data structures here*/
    var recordedCellInfos = new Array();
    /* We will calculate here the maximum table height*/
    var maximumTableHeight = 0;
    /* For sorting */
    var sortableTH = null;
    /* For accessing tabs */
    var theTabs = null;
    /* For narrow results popup */
    var narrowResultsSetup = false;
    /* map  */
    var map = null;
    var markers = new Array();
    var myPointsSidebar = null;

    /*** page loading ***/

    $(document).ready(function () {
        myPageLoad();
    });


    function myPageLoad() {
        if (narrowResultsSetup)
            return; // just initializing an iframe

        badBrowser = (/MSIE ((5\.5)|6)/.test(navigator.userAgent) && navigator.platform == "Win32");

        waitingForProcessing = true;
        mainTable = $("#maint");
        headerTable = $("#headert");
        divAroundMainTable = $(".divAroundMainTable");
        rightPageColumn = $("#rightPageColumn");
        mainPartOfPage = $("#mainPartOfPage");

        initializeMyPointsSidebar();

        tableInfo = $("#tableInfo");

        $("#btnsAboveMainTable").removeClass("btnsAboveMainTable");
        $('[id*=RefreshPageStill]').show().bind('click', loadUpdatedValuesForPage).autoHilite();
        $('[id*=submitBulk]').bind('click', submitBulk).autoHilite();
        $('[id*=cancelBulk]').bind('click', cancelBulk).autoHilite();
        setupLiveEvents();

        if (divAroundMainTable.length == 0) { /* we're on row page -- to do --> fix col widths */
        }

        var theInstance = Sys.WebForms.PageRequestManager.getInstance();
        theInstance.add_beginRequest(pageLoadUpdateStart);
        theInstance.add_endRequest(pageLoadUpdateEnd);



        $('[id$=PagerRow]').remove();
        makeTableTabs();

        mainTableOrigWidth = mainTable.innerWidth();
        divAroundMainTableOrigWidth = divAroundMainTable.innerWidth();
        rightPageColumnOrigWidth = rightPageColumn.innerWidth();
        mainPartOfPageOrigWidth = mainPartOfPage.innerWidth();
        resizePageVertically();



        var statusPanel = $find("statusPanelBehavior");
        if (statusPanel != null) {
            statusPanel.add_expandComplete(resizePageVerticallyHandler);
            statusPanel.add_collapseComplete(resizePageVerticallyHandler);
        }

        theOriginalValues = new Array();
        getUserAccessInfo();

        if ($("#shouldPopulate").val() == "true") {
            waitingForProcessing = false;
            populateTable(true, 0);
        }
        else // postback that shouldn't lead to repopulation
        {
            $("#shouldPopulate").val("true"); // populate next time by default
        }
    }

    function pageLoadUpdateStart() {
        refreshPageAnimate();
        $("#btnsAboveMainTable").removeClass("btnsAboveMainTable");
    }

    function pageLoadUpdateEnd() {
        refreshPageStopAnimating();
    }

    function getUserAccessInfo() {
        var userAccessInfoDiv = $("#userAccessInfo");
        var userNameDiv = $("#userName", userAccessInfoDiv);
        var passwordDiv = $("#passwordForWebService", userAccessInfoDiv);
        theUserAccessInfo = new ClassLibrary1.Model.UserAccessInfo();
        theUserAccessInfo.userName = userNameDiv.text();
        theUserAccessInfo.passwordForWebService = passwordDiv.text();
    }

    /*** table and row population ***/


    /* table population */

    function populateTable(getHeaderRow, rowsToSkip) {
        if (headerRow == null)
            getHeaderRow = true; // must get header row, because we don't have one yet, even if we ordinarily wouldn't
        if (tableInfo != null) {
            $("#mainTableScrollArea").unbind('scroll');
            clearTableContents(getHeaderRow);
            allPotentialRows = $([]);
            launchPopulate(getHeaderRow, rowsToSkip);
        }
        waitingForProcessing = false;
    }

    /* remove the rows from the table */
    function clearTableContents(getHeaderRow) {
        if (getHeaderRow) {
            var theSearchRow = $(".searchRow");
            if (theSearchRow.length == 0)
                prepareForGetHeaderRow();
            headerTable.find(".headerRow,.invHeaderRow").remove();
            //$("#maint > col").remove();
        }
        hideNoRowsMessage();
        removeAddresses(mainTable);
        if (mainTable != null) {
            //        $("tr.prow", mainTable).remove();
            //        $("#maint > col").remove();
            mainTable.replaceWith('<table id="maint" class="mainTable mainTablePositioning"><tbody></tbody></table>');
            mainTable = $("#maint");
        }
        if (getHeaderRow)
            setScrollAreaHeight();
    }

    /* insert a search row and space for a header row to be inserted later */
    function prepareForGetHeaderRow() {
        var searchRow = "<table class=\"searchRow\"><tr><td class=\"searchWordsLabel\"><img src=\"/images/NarrowResultsLabel.png\" /></td><td class=\"searchWordsInput\" ><input ID=\"searchWordsFilter\" /><img id=\"doNarrowBySearchWords\" class=\"reppng\" src=\"/images/go-normal.png\"/></td><td class=\"searchWordsAdvanced\"><img id=\"narrowTrigger\" src=\"/images/filter-normal.png\"></td><td class=\"searchWordsSort\"><img id=\"sortTrigger\" src=\"/images/sort-normal.png\"></td></tr></table>";
        headerTable.before(searchRow);
        $(".reppng").replacePngWithGif(); /* for ie6 */
        $("#doNarrowBySearchWords").click(doNarrowBySearchWords).customHilite();
        $("#searchWordsFilter").focus().keypress(narrowBySearchWordsKeyPress);
        narrowResultsSetup = false;
    }

    /* call the web service to populate the page from scratch */
    function launchPopulate(getHeaderRow, rowsToSkip) {
        if (waitingForProcessing == true)
            return;
        var onPopulateCallback = Function.createCallback(onPopulate, null);
        var theTableInfoHtml = tableInfo.html();
        if (theTableInfoHtml != null) {
            insertLoadingRow();
            pendingServerCalls++;
            waitingForProcessing = true;
            PMWebServices.PMWebService.PopulateTableInitially(theTableInfoHtml, rowsVisibleEstimate,
                        rowsInitialToLoad, rowsToSkip, getHeaderRow, onPopulateCallback);
        }
    }

    /* process initial results from server */
    function onPopulate(result) {
        removeLoadingRow();
        rowCount = result.rowCount;
        if (result == null || result.success == false || rowCount == 0) {
            showNoRowsOrErrorMessage(!(result == null || result.success == false));
            setupNarrowResultsAndSort(false);
            return;
        }

        if (result.tableInfoForReset != "") {
            tableInfo.html(result.tableInfoForReset);
        }


        setupNarrowResultsAndSort(true);

        if (result.headerRow != null && result.headerRow != "")
            addHeaderRow(result.headerRow);
        if (result.mainRows != null && result.mainRows != "")
            addInitialMainTableRows(result.mainRows);

        pendingServerCalls--;
        requestNeededRowsFromServer();

        addAddressesToMap(mainTable);
        updateMarkers();
    }

    function setupNarrowResultsAndSort(enableSort) {
        $("#narrowTrigger").click(openNarrowResults).customHilite();
        if (enableSort)
            $("#sortTrigger").customEnable();
        else
            $("#sortTrigger").customDisable();
    }

    var headerRow = null;
    function addHeaderRow(headerRowResult) {
        var headerRowText = "<tr class=\"headerRow\">" + headerRowResult + "</tr>";
        headerTable.append(headerRowText);
        headerRow = $(".headerRow", headerTable).eq(0);
        calculateColWidths();
        sortableTH = $(".sortasc,.sortdesc,.sortableasc,.sortabledesc,.sortascVertText,.sortdescVertText,.sortableascVertText,.sortabledescVertText", headerTable);
        sortableTH.click(changeSortEvent);
        setupSortMenu();
    }

    function addInitialMainTableRows(mainRowsResult) {
        mainTableBody = $("#maint > tbody");
        applyColWidthsToMainArea();
        var maxTableHeight = determineMaximumTableHeight();
        var tableBodyHeight = maxTableHeight - mainTable.height();
        var lastRow = null;
        var firstRow = null;
        mainTable.append(mainRowsResult);
        microAdjustColWidths();
        if (mainRowsResult.indexOf("treenew") != -1)
            $('div.treenew', mainTableBody).setupTree();
        var matchResult = mainRowsResult.match(/somerow/g);
        var numRowsEst = 0;
        if (matchResult != null)
            numRowsEst = matchResult.length;
        if (rowsVisibleEstimate == null) {
            rowHeight = Math.floor(mainTable.height() / numRowsEst);
            rowsVisibleEstimate = Math.floor(tableBodyHeight / rowHeight) + 1;
        }
        var theRows = $('tr.somerow', mainTable);
        firstRow = theRows.first();
        lastRow = theRows.last();
        var firstRowNum = firstRow.getRowNum();
        var lastRowNum = $(lastRow).getRowNum();
        numRowsEst = theRows.length;

        checkIncreaseNumberCell(lastRowNum);

        /* add waiting rows */
        var waitingRowsText = "";
        /* add waiting rows to beginning */
        if (firstRowNum > 1) {
            var skippedRowsText = generateMissingRows(1, firstRowNum - 1);
            firstRow.before(skippedRowsText);
        }
        /* add waiting rows to end */
        var numWaitingRows = Math.min(75, rowCount - numRowsEst - (firstRowNum - 1)); /* put in waiting rows to save time on initial arrow down */
        for (var i2 = firstRowNum - 1 + numRowsEst + 1; i2 <= firstRowNum - 1 + numRowsEst + numWaitingRows; i2++)
            waitingRowsText += generateWaitingRow(i2);
        var missingRowsText = "";
        if (firstRowNum - 1 + numRowsEst + numWaitingRows < rowCount)
            missingRowsText = generateMissingRows(firstRowNum - 1 + numRowsEst + numWaitingRows + 1, rowCount);
        $(lastRow).after(waitingRowsText + missingRowsText);

        /* scroll to correct place */
        /* set some variables that are needed */
        var didSplit = true;
        mainTableScrollArea.scrollTo(firstRow);
        refreshScrolledInCache();
        while (didSplit) {
            didSplit = findMissingRowsToSplit();
            mainTableScrollArea.scrollTo(firstRow);
            refreshScrolledInCache();
        }
        $("#mainTableScrollArea").scroll(findMissingRowsToSplit);
    }

    /*
    * Determines the maximum height available to the table based on the available space
    */
    function determineMaximumTableHeight() {
        if (maximumTableHeight == 0) {
            maximumTableHeight = (divAroundMainTable.height() - 2);
        }
        return maximumTableHeight;
    }

    /*
    * We've retrieved new records. Replace waiting rows.
    * If we cannot replace all visible waiting rows we query the server again.
    */
    function rowsRetrieved(result, dataOnCallBack, methodName) {
        if (!(result == null || result.success == false || dataOnCallBack.tableInfoString == null || dataOnCallBack.tableInfoString != tableInfo.html())) {

            if (result.tableInfoForReset != "") { // We've reset b/c user has been on page a long time
                clearTableContents();
                onPopulate(result);
                return;
            }

            replaceWaitingRows(result.mainRows);
        }
        pendingServerCalls--;
        findMissingRowsToSplit();
        refreshScrolledInCache();
    }

    /*** table row information and display ***/

    /* converts main table rows into an array */
    function extractRowsArray(rowsText) {
        var theRows = new Array();
        var endOfCurrentRow = 0;
        var keepGoing = true;
        var restOfString = rowsText;
        while (keepGoing) {
            endOfCurrentRow = (restOfString.substring(1)).toLowerCase().indexOf("<tr id=\"maintr") + 1; /* ignore first character to get position of last character of this row */
            if (endOfCurrentRow == 0) {
                keepGoing = false;
                Array.add(theRows, restOfString);
            }
            else {
                Array.add(theRows, restOfString.substring(0, endOfCurrentRow));
                restOfString = restOfString.substring(endOfCurrentRow);
            }
        }
        return theRows;
    }

    /* returns an array of row numbers for the rows extracted above */
    function extractRowNums(rowsArray) {
        var theRowNums = new Array();
        for (var i = 0; i < rowsArray.length; i++) {
            Array.add(theRowNums, getRowNumFromString(rowsArray[i]));
        }
        return theRowNums;
    }

    /* find a matching row from the rowsArray by row number */
    function findMatchingRowInArray(rowsArray, rowNumsArray, rowNum) {
        for (var i = 0; i < rowsArray.length; i++) {
            if (rowNumsArray[i] == rowNum)
                return rowsArray[i];
        }
        return null;
    }

    /*retrieves the last row from the table*/
    function findLastRow() {
        var lastWaitingRow = mainTableBody.children($("tr.waitingRow")).last();
        var lastMainRow = mainTableBody.children($("tr.somerow")).last();
        if (lastWaitingRow.length == 0 && lastMainRow.length == 0)
            return null;
        if (lastWaitingRow.length == 0)
            return lastMainRow;
        if (lastMainRow.length == 0)
            return lastWaitingRow;
        if (lastWaitingRow.position().top > lastMainRow.position().top)
            return lastWaitingRow;
        else
            return lastMainRow;
    }

    /*retrieves the first row from the table*/
    var countScrolledInWithinPixels = 1; //  50; /* allows loading of rows just above and below area */
    var allPotentialRows = $([]);
    var positionOfFirstLast = null;
    var positionOfLastLast = null;
    var numRowsLast = null;
    var lastScrolledInRows = null;

    function refreshScrolledInCache() {
        allPotentialRows = $("#maint > tbody > tr.prow"); // mainTableBody.children($("tr.prow"));
        lastScrolledInRows = null; /* reset scrolled in rows */
        positionOfFirstLast = null;
        positionOfLastLast = null;
    }

    function getScrolledInRows() {
        var scrolledInRows = $([]);
        if (allPotentialRows.length == 0)
            return scrolledInRows;
        //try {
        var positionOfFirst = 0 - mainTableScrollArea[0].scrollTop;
        /* slower jQuery equivalent: var positionOfFirst = allPotentialRows.eq(0).position().top; */
        //    }
        //catch(err) {
        //    return scrolledInRows;
        //}
        if (positionOfFirst == positionOfFirstLast && numRowsLast == allPotentialRows.length && lastScrolledInRows != null)
            return lastScrolledInRows;
        positionOfFirstLast = positionOfFirst;
        numRowsLast = allPotentialRows.length;
        var theMainTableHeight = getMainTableHeight();
        var indexFirstIn = null;
        var indexLastIn = null;
        for (var index = 0; index < allPotentialRows.length; index++) {
            if (allPotentialRows[index].offsetTop + positionOfFirst > 0 - countScrolledInWithinPixels) {
                indexFirstIn = index - 1;
                if (indexFirstIn == -1)
                    indexFirstIn = 0;
                break;
            }
        }
        if (indexFirstIn == null) {
            if (allPotentialRows.length > 0)
                indexFirstIn = allPotentialRows.length - 1; /* will be missed when top of last row is above top of table */
            else {
                lastScrolledInRows = scrolledInRows;
                return scrolledInRows;
            }
        }
        for (var index2 = indexFirstIn + 1; index2 < allPotentialRows.length; index2++) {
            if (allPotentialRows[index2].offsetTop + positionOfFirst > theMainTableHeight + countScrolledInWithinPixels) {
                indexLastIn = index2 - 1;
                break;
            }
        }
        if (indexLastIn == null)
            indexLastIn = allPotentialRows.length - 1;
        for (var index3 = indexFirstIn; index3 <= indexLastIn; index3++) {
            scrolledInRows = addPotentialRowToScrolledInRows(scrolledInRows, allPotentialRows, index3);
        }
        lastScrolledInRows = scrolledInRows;
        return scrolledInRows;
    }

    function addPotentialRowToScrolledInRows(scrolledInRows, allPotentialRows, theRowNum) {
        scrolledInRows = scrolledInRows.add(allPotentialRows.eq(theRowNum));
        return scrolledInRows;
    }

    function rowIsScrolledIn(row) {
        var rowElement = $(row);
        if (row == null || rowElement.length == 0)
            return false;
        var theMainTableHeight = getMainTableHeight();
        try {
            var rowTop = rowElement.position().top;
        }
        catch (err) {
            return false;
        }
        return (rowTop < theMainTableHeight && (rowTop + rowElement.height() > 0));
    }
    $.expr[':'].rowScrolledIn = function (row) {
        return rowIsScrolledIn(row);
    };

    function findFirstRow() {
        var scrolledInRows = getScrolledInRows();
        var singleRows = scrolledInRows.filter(".prow:not(.missingRows)");
        if (singleRows.length == 0)
            return null;
        return singleRows.eq(0);
    }


    /*
    * inserts a row indicating table is in initial loading
    */
    function insertLoadingRow() {
        var loadingRow = "<tr id='loadingRow' style='background-color: #D8D8D8;'><td colspan=\"99\" style=\"padding-left:235px;\" height=\"" +
            ($("#mainTableScrollArea").height() - 3) +
            "\"><span><b>Loading ratings...</b></span>&nbsp;&nbsp;&nbsp;<img id=\"waitingForData\" src=\"/images/Button-Refresh-Moving.gif\" ><br/><img src=\"/images/top_logo.gif\"></td></tr>";
        mainTable.append(loadingRow);
    }

    /*
    * removes the initial loading row 
    */
    function removeLoadingRow() {
        mainTable.find("tr[id^='loadingRow']").remove();
    }
    /*
    * Shows a message indicating that no rows have been found
    */
    function showNoRowsOrErrorMessage(noRows) {
        var theMessage = null;
        if (noRows)
            theMessage = "Sorry. No records found.";
        else
            theMessage = "Sorry. An error occurred loading data. Please refresh the page.";
        var noRowsFoundMessage = "<tr id='noRowsFound'><td colspan=\"99\" height=\"25px\" class=\"noRowsFound\">" + theMessage + "</td></tr>";
        mainTable.append(noRowsFoundMessage);
    }

    /*
    * Hides the message indicating that no rows have been found
    */
    function hideNoRowsMessage() {
        mainTable.find("tr[id^='noRowsFound']").remove();
    }

    /*** dummy waiting rows ***/

    /*replaces all dummy waiting rows with actual rows */

    function replaceWaitingRows(rowsText) {
        var waitingRows = getWaitingRows();
        var rowsArray = extractRowsArray(rowsText);
        var rowNumsArray = extractRowNums(rowsArray);
        var replacedRows = 0;
        var nextRowNum = null;
        var nextRowItem = null;
        var waitingRowToReplace = null;
        var setOfWaitingRowsToReplace = $([]);
        var htmlToInsert = "";
        for (var i = 0; i < waitingRows.length; i++) {
            waitingRowToReplace = getWaitingRowWithIndex(waitingRows, i);
            /* usually, nextRowNum and nextRowItem will be set on the previous item, but we must do it for first */
            if (i == 0) {
                nextRowNum = waitingRowToReplace.getRowNum();
                nextRowItem = findMatchingRowInArray(rowsArray, rowNumsArray, nextRowNum);
            }
            /* set current waitingRowNumber and rowItem based on previous item */
            var waitingRowNumber = nextRowNum;
            var rowItem = nextRowItem;
            /* determine whether to hold off on insertion; we do this if the next waiting row will also be filled now */
            var holdOffOnInsert = false;
            if (i < waitingRows.length - 1) {
                nextRowNum = getWaitingRowWithIndex(waitingRows, i + 1).getRowNum();
                nextRowItem = findMatchingRowInArray(rowsArray, rowNumsArray, nextRowNum);
                holdOffOnInsert = rowItem == null || ((nextRowNum == waitingRowNumber + 1) && nextRowItem != null);
            }
            if (rowItem != null) {
                setOfWaitingRowsToReplace = setOfWaitingRowsToReplace.add(waitingRowToReplace);
                htmlToInsert += rowItem;
            }
            if (!holdOffOnInsert && htmlToInsert != "") {
                doReplacement(setOfWaitingRowsToReplace, htmlToInsert);
                setOfWaitingRowsToReplace = $([]);
                htmlToInsert = "";
            }
        }

        if (rowsText.indexOf("treenew") != -1)
            $('div.treenew', mainTableBody).setupTree();

        refreshScrolledInCache();
    }


    function getWaitingRows() {
        return $("#maint > tbody > tr.waitingRow");  // mainTableBody.children($("tr.waitingRow")); This mysteriously returns a non-matching element as the first element.
    }

    function getWaitingRowWithIndex(waitingRows, num) {
        return waitingRows.eq(num);
    }

    function doReplacement(setOfWaitingRowsToReplace, htmlToInsert) {
        var newElements = $(htmlToInsert).addAddressesFromRow();
        updateMarkers();
        doInsertBefore(newElements, setOfWaitingRowsToReplace.eq(0));
        setOfWaitingRowsToReplace.remove();
    }

    function doInsertBefore(newElements, beforeThis) {
        beforeThis.before(newElements);
    }

    /*Generates a waiting row for a given index*/
    var waitingRowsList = new Array();
    var waitingRowFragment1 = "<tr id='waitingRow' class='waitingRow prow ";
    var waitingRowFragment1b = "'><td class=\"nmcl\">";
    var waitingRowFragment2 = "</td><td colspan=\"99\" height=\"";
    var waitingRowFragment3 = "px\"><img id=\"waitingForData\" src=\"/images/Button-Refresh-Moving.gif\" class=\"waitingForData\"></td></tr>";
    function generateWaitingRow(index) {
        var suppClass = "";
        if (index % 2 == 0)
            suppClass = "altrow";
        else if (index % 2 == 1)
            suppClass = "row";
        var formattedRow = waitingRowFragment1 + suppClass + waitingRowFragment1b + index + waitingRowFragment2 + rowHeight + waitingRowFragment3;
        return formattedRow;
    }

    /* Add zeros */
    function digits(number) {
        return Math.floor(Math.log(number) / Math.log(10)) + 1;
    }

    function addOpeningZeros(number, numdigits) {
        var currentDigits = digits(number);
        var openingZeros = "";
        for (var i = currentDigits + 1; i <= numdigits; i++)
            openingZeros += "0";
        return openingZeros + number;
    }

    /* Generates a missing rows row (corresponding to 1 or more missing rows) */
    var missingRowsFragment1 = "<tr id='missing";
    var missingRowsFragment2 = "' class='missingRows prow potentialVisibleRows'><td></td><td colspan=\"99\" height=\"";
    var missingRowsFragment3 = "px\"><div class=\"waitingForData missingRowsImage\"></td></tr>";
    function generateMissingRows(firstRowNum, lastRowNum) {
        var firstNumberWithExtraDigits = addOpeningZeros(firstRowNum, 10);
        var formattedRow = missingRowsFragment1 + firstNumberWithExtraDigits + "," + lastRowNum + missingRowsFragment2 + (rowHeight * (lastRowNum - firstRowNum + 1)) + missingRowsFragment3;
        return formattedRow;
    }

    function splitMissingRows(rows, numToSplit) {
        var range = rows.attr('id');
        var firstRow = parseInt(range.substring(8, 17), 10); /* must specify base 10 with leading zeros */
        var lastRow = parseInt(range.substring(18), 10);
        var numRows = lastRow - firstRow + 1;
        if (numToSplit > numRows)
            numToSplit = numRows;
        var numRowsEach = Math.floor(numRows / numToSplit);
        var textToAdd = "";
        var newFirstRowNum = 0;
        var newLastRowNum = firstRow - 1;
        for (var index = 1; index <= numToSplit; index++) {
            newFirstRowNum = newLastRowNum + 1;
            if (index == numToSplit)
                numRowsEach = numRows - numRowsEach * (index - 1); /* do all remaining rows */
            newLastRowNum = newFirstRowNum + numRowsEach - 1;
            if (numRowsEach == 1)
                textToAdd += generateWaitingRow(newFirstRowNum);
            else {
                var missingRows = generateMissingRows(newFirstRowNum, newLastRowNum);
                textToAdd += missingRows;
            }
        }
        rows.before(textToAdd);
        var waitingRows = $("#maint > tbody > tr.waitingRow"); // mainTableBody.children($("tr.waitingRow"));
        setupHover(waitingRows);
        rows.remove();
        refreshScrolledInCache();
    }

    var previousScrollTop = null;
    var lastMissingRowsCheckTime = null;
    function findMissingRowsToSplit() {
        var didSplit = false;
        if (previousScrollTop == null || mainTableScrollArea[0].scrollTop != previousScrollTop || lastMissingRowsCheckTime == null || new Date() - lastMissingRowsCheckTime > 125) {
            var outerLoopKeepGoing = true;
            lastMissingRowsCheckTime = new Date();
            while (outerLoopKeepGoing) { /* user may have scrolled again */
                var keepGoing = true;
                while (keepGoing) {
                    keepGoing = findMissingRowsToSplitHelper();
                    if (keepGoing)
                        didSplit = true;
                }
                previousScrollTop = mainTableScrollArea[0].scrollTop;
                var visibleMissingRows = getScrolledInRows().filter(".missingRows");
                outerLoopKeepGoing = visibleMissingRows.length > 0;
            }
        }
        requestNeededRowsFromServer();
        return didSplit;
    }

    function findMissingRowsToSplitHelper() {
        var visibleMissingRows = getScrolledInRows().filter(".missingRows");
        if (visibleMissingRows.length == 0)
            return false;
        $.each(visibleMissingRows, function (index, item) {
            splitMissingRows($(item), 50);
        });
        return true;
    }

    /* Fill in rows that are currently designated as waiting rows, if there are no current calls pending to the server. */

    function requestNeededRowsFromServer() {
        refreshScrolledInCache();
        if (pendingServerCalls > 0)
            return;
        $(this).everyTime(100, "checkCompleteNeededRows", checkWhetherToCompleteRequestNeededRowsFromServer);
    }

    /* Check whether the scrolling has stopped, in which case we complete the call. */
    var savedFirstScrolledInRowNum = null;
    var lastTime = null;
    var minMillisecondsForScrollingStoppedCheck = 250;
    function checkWhetherToCompleteRequestNeededRowsFromServer() {
        if (lastTime != null) {
            var timeDifferential = new Date().getTime() - lastTime;
            if (timeDifferential < minMillisecondsForScrollingStoppedCheck)
                return;
        }
        var firstRow = findFirstRow();
        if (firstRow == null)
            return;
        var firstScrolledInRowNum = firstRow.getRowNum();
        if (lastTime == null || savedFirstScrolledInRowNum != firstScrolledInRowNum) {
            lastTime = new Date().getTime();
            savedFirstScrolledInRowNum = firstScrolledInRowNum;
            return;
        }
        savedFirstScrolledInRowNum = null;
        lastTime = null;
        $(this).stopTime("checkCompleteNeededRows");
        completeRequestNeededRowsFromServer();
    }


    /* We request a range of rows from the server. */
    function completeRequestNeededRowsFromServer() {

        var firstScrolledInWaitingRow = getScrolledInRows().filter(".waitingRow").eq(0);
        if (firstScrolledInWaitingRow.length == 0) {
            return; // no request needed
        }
        var firstScrolledInWaitingRowNum = firstScrolledInWaitingRow.getRowNum();

        var allWaitingRows = $("tr.waitingRow", mainTable);
        var indexOfWaitingRow = null;
        var firstInRange = null;
        var firstInRangeRowNum = null;
        var lastInRange = null;
        var lastInRangeRowNum = null;
        var aRow = null;
        for (var i = 0; i < allWaitingRows.length; i++) {
            aRow = allWaitingRows.eq(i);
            var theRowNum = aRow.getRowNum();
            if (firstInRange == null && theRowNum >= firstScrolledInWaitingRowNum - 10) {
                firstInRange = i;
                firstInRangeRowNum = theRowNum;
                lastInRange = i;
                lastInRangeRowNum = theRowNum;
            }
            if (firstInRange != null) {
                if (theRowNum > firstScrolledInWaitingRowNum + 32)
                    break;
                lastInRange = i;
                lastInRangeRowNum = theRowNum;
            }
        }
        if (i < allWaitingRows.length - 1)
            aRow = allWaitingRows.eq(allWaitingRows.length - 1); // set to last row
        checkIncreaseNumberCell(aRow.getRowNum());
        if (pendingServerCalls == 0) {
            pendingServerCalls++;
            //we retrieve the rows from the server
            tableInfoString = tableInfo.html();
            var theFirstRowScrolledIn = findFirstRow();
            var onRepopulateScrollTo = 1;
            if (theFirstRowScrolledIn != null)
                onRepopulateScrollTo = theFirstRowScrolledIn.getRowNum();  /* if necessary b/c of lapse of time, scroll back here */
            var dataOnCallBack = new Object();
            dataOnCallBack.tableInfoString = tableInfoString;
            var getRowsFromServerCallback = Function.createCallback(rowsRetrieved, dataOnCallBack);

            PMWebServices.PMWebService.PopulateTableSpecificRows(tableInfo.html(), firstInRangeRowNum, lastInRangeRowNum - firstInRangeRowNum + 1, rowsVisibleEstimate, onRepopulateScrollTo, getRowsFromServerCallback, getRowsFromServerCallback, dataOnCallBack);
        }
    }



    /*** column width manipulation ***/

    var theScrollbarWidth = null;
    function scrollbarWidth() {
        var div = $('<div style="width:50px;height:50px;overflow:hidden;position:absolute;top:-200px;left:-200px;"><div style="height:100px;"></div>');
        // Append our div, do our calculation and then remove it
        $('body').append(div);
        var w1 = $('div', div).innerWidth();
        div.css('overflow-y', 'scroll');
        var w2 = $('div', div).innerWidth();
        $(div).remove();
        if (w1 - w2 < 2)
            return 17;
        return (w1 - w2);
    }
    function getScrollbarWidth() {
        if (theScrollbarWidth == null)
            theScrollbarWidth = scrollbarWidth();
        return theScrollbarWidth;
    }


    /* column sizing -- columns can be specified in absolute pixel widths */
    /* (by a class beginning "wf") or relative pixel widths (by a class beginning */
    /* "wv"). */
    function getPlannedWidth(tableHeader) {
        var widthPlan = new Object();
        var classList = $(tableHeader).attr('class').split(' ');
        $.each(classList, function (index, item) {
            var prefix = item.substring(0, 2);
            if (prefix == "wv")
                widthPlan.Variable = parseInt(item.substring(2));
            else if (prefix == "wf")
                widthPlan.Fixed = parseInt(item.substring(2));
        });
        if (widthPlan.Variable == null && widthPlan.Fixed == null)
            widthPlan.Variable = 10; /* average size column */
        else if (widthPlan.Variable != null && widthPlan.Fixed != null)
            widthPlan.Variable = null;
        return widthPlan;
    }

    /* calculate table widths */
    var colWidths = null;
    var allTableHeaders = null;
    var borderAdjustment = 1;
    var totalTableHeaders = 0;
    function calculateColWidths() {
        var mainTableWidth = 0;
        if (divAroundMainTable.length > 0)
            mainTableWidth = divAroundMainTable.width() - getScrollbarWidth();
        else
            mainTableWidth = 557; /* row view page */
        $("#headert, #maint").css("width", mainTableWidth + getScrollbarWidth()).css("max-width", mainTableWidth + getScrollbarWidth());
        allTableHeaders = $("th", headerRow);
        totalTableHeaders = allTableHeaders.length;
        colWidths = new Array(totalTableHeaders);
        var fixedTotal = 0;
        var variableTotal = 0;
        $.each(allTableHeaders, function (index, item) {
            colWidths[index] = getPlannedWidth(allTableHeaders[index]);
            if (colWidths[index].Fixed == null)
                variableTotal += colWidths[index].Variable;
            else
                fixedTotal += colWidths[index].Fixed;
        });
        if (variableTotal > 0) {
            var amountToDistribute = mainTableWidth - fixedTotal;
            var perVariablePoint = amountToDistribute / variableTotal;
        }
        var totalAllButLast = 0;
        $.each(allTableHeaders, function (index, item) {
            if (colWidths[index].Fixed == null)
                colWidths[index].Fixed = Math.floor(perVariablePoint * colWidths[index].Variable);
            if (index < totalTableHeaders - 1)
                totalAllButLast += colWidths[index].Fixed;
            else
                colWidths[index].Fixed = mainTableWidth - totalAllButLast;
            if (colWidths[index].Fixed < 15)
                colWidths[index].Fixed = 15;
            //$(allTableHeaders[index]).css("width", colWidths[index].Fixed - borderAdjustment).css("max-width", colWidths[index].Fixed - borderAdjustment);
        });
        $(allTableHeaders[totalTableHeaders - 1]).after("<th style=\"padding:0; width:" + getScrollbarWidth() + "px;\" />");
    }


    var mainTableScrollArea = null;
    function applyColWidthsToMainAreaMethod2(mainTableOnly) { // needed for nonsupporting browsers
        var invisibleTHRow = "<tr class=\"invHeaderRow\">";
        $.each(colWidths, function (index, item) {
            invisibleTHRow += "<th style=\"height: 0px; width:" + (colWidths[index].Fixed - borderAdjustment) + "px; max-width:" + (colWidths[index].Fixed - borderAdjustment) + "px\"";
            if (index == 0)
                invisibleTHRow += " class=\"nmcl\"";
            invisibleTHRow += "/>";
        });
        invisibleTHRow += "<th style=\"height: 0px; width:" + getScrollbarWidth() + "px;\"/>";
        invisibleTHRow += "</tr>";
        if (!mainTableOnly)
            $("#headert").prepend(invisibleTHRow);
        mainTable.append(invisibleTHRow);
        /* resizePageToFitTableIfNecessary(); uncomment if we want to allow table to get bigger when there are a lot of columns */
    }
    function applyColWidthsToMainAreaMethod1() {
        var colInfoList = "";
        $.each(colWidths, function (index, item) {
            colInfoList += "<col width=\"" + (colWidths[index].Fixed) + "px\"/>";
        });
        colInfoList += "<col width=\"" + getScrollbarWidth() + "px\"/>";
        var tbody = $("tbody", mainTable).eq(0);
        tbody.before(colInfoList);
        /* resizePageToFitTableIfNecessary(); uncomment if we want to allow table to get bigger when there are a lot of columns */
    }
    /* the following compensates for some cross-border incompatibilities -- a bit hacky */
    function microAdjustColWidths() {
        return; /* not currently implemented */
    }

    function applyColWidthsToMainArea() {
        var existingInvHeaderRows = $(".invHeaderRow").length;
        if (existingInvHeaderRows == 0)
            applyColWidthsToMainAreaMethod2(false);
        else if (existingInvHeaderRows == 1)
            applyColWidthsToMainAreaMethod2(true);
        if ($("#maint > col").length == 0)
            applyColWidthsToMainAreaMethod1();
        microAdjustColWidths();
        mainTableScrollArea = $("#mainTableScrollArea");
        setScrollAreaHeight();
        $(".rotatedColHeader").load(setScrollAreaHeight);
        numDigitsOfHighest = 2;
    }

    function increaseSizeOfNumberCell(pixels) {
        colWidths[0].Fixed += pixels;
        colWidths[1].Fixed -= pixels;
        increaseSizeOfNumberCellHelper($(".headerRow"));
        increaseSizeOfNumberCellHelper($(".invHeaderRow"));
        var cols = $("col", mainTable);
        cols.eq(0).attr("width", (colWidths[0].Fixed) + "px");
        cols.eq(1).attr("width", (colWidths[1].Fixed) + "px");
        //cols.eq(0).css("max-width", (colWidths[0].Fixed) + "px");
        //cols.eq(1).css("max-width", (colWidths[1].Fixed) + "px");
        microAdjustColWidths();
    }

    function increaseSizeOfNumberCellHelper(headerRows) {
        for (var i = 0; i < headerRows.length; i++) {
            var theHeaders = $("th", headerRows.eq(i));
            theHeaders.eq(0).css("width", colWidths[0].Fixed - borderAdjustment);
            theHeaders.eq(1).css("width", colWidths[1].Fixed - borderAdjustment);
            theHeaders.eq(0).css("max-width", colWidths[0].Fixed - borderAdjustment);
            theHeaders.eq(1).css("max-width", colWidths[1].Fixed - borderAdjustment);
        }
    }

    var numDigitsOfHighest = null; /* set to default of 2 when setting initial column widths, then call this whenever inserting row */
    function checkIncreaseNumberCell(highest) {
        var digitsOfHighest = digits(highest);
        if (digitsOfHighest > numDigitsOfHighest) {
            increaseSizeOfNumberCell(10 * (digitsOfHighest - numDigitsOfHighest));
            numDigitsOfHighest = digitsOfHighest;
        }
    }

    function setScrollAreaHeight() {
        var theSearchRow = $(".searchRow");
        var theSearchRowHeight = 0;
        if (theSearchRow.length > 0)
            theSearchRowHeight = theSearchRow.eq(0).height();
        var theHeaderTableHeight = 0;
        if (headerTable != null && headerTable.length > 0)
            theHeaderTableHeight = headerTable.height();
        $("#mainTableScrollArea").height(divAroundMainTable.height() - theSearchRowHeight - 1 - theHeaderTableHeight);
    }

    /*** cell selection ***/


    var mostRecentSelectNumberCellEvent = null;
    var mostRecentClickedInput = this;
    var theDeferredFunction = null;
    function deferSelection(theEvent, theClickSource, deferredFunction) {
        if (theDeferredFunction != null) { /* now we're running what was previously deferred */
            theDeferredFunction = null;
            return false;
        }
        mostRecentSelectNumberCellEvent = theEvent;
        mostRecentClickedInput = theClickSource;
        if (theClickSource.className == "rtg rtgFixed") {
            theDeferredFunction = deferredFunction;
            return true; /* must defer processing until after jstree has processed */
        }
        theDeferredFunction = null;
        return false;
    }

    function selectNumberCellFromClick(event) {
        $(this).focus();
    }

    function selectNumberCellAfterFocus(event) {
        /* if (deferSelection(event, this, selectNumberCellAfterFocus))
        return; deferral no longer needed? */
        mostRecentSelectNumberCellEvent = event;
        mostRecentClickedInput = this;

        if (waitingForProcessing == true)
            return;
        var theCellToSelectSummaryInfo = getCellSummaryInfoFromEvent(mostRecentSelectNumberCellEvent, mostRecentClickedInput);
        if (!$(theCellToSelectSummaryInfo.TheCell).hasClass("selectedcell")) {
            removeButtonsFromCell(theCellToSelectSummaryInfo.TheCell);
            selectNumberCell(theCellToSelectSummaryInfo);
        }
    }

    var inputToHighlight = null;
    function deferredInputSelectAll() {
        if (inputToHighlight != null)
            inputToHighlight.select();
    }

    function onInputBlur() {
        if (theCellWithFocusInfo != null) {
            if (!cellHasChanged(theCellWithFocusInfo)) {
                deselectNumberCell(theCellWithFocusInfo.TheCell);
            }
        }
    }

    function selectNumberCell(theCellToSelectSummaryInfo) {
        var theCellInfo = recordOriginalCellInfo(theCellToSelectSummaryInfo.TheCell);
        if (theCellToSelectSummaryInfo.FocusAtTimeOfEvent == theCellWithFocusInfo) /* make sure no change has occurred */
            setFocusToCell(theCellInfo);
        numberSelections++;
        updateBulkButtons(true);
        removeButtonsFromCell(theCellToSelectSummaryInfo.TheCell);
        var contentsToAppend = '<div style="text-align: center;" visible="false" id="entercancelinner' + numberSelections + '"><img style="border-width: 0px;" src="/images/accept.gif" id="btnenternum' + numberSelections + '"/><img style="border-width: 0px;" src="/images/cancel.gif" id="btncancelnum' + numberSelections + '"/><img style="border-width: 0px; display: none;" src="/images/Button-Refresh-Still.gif" id="btnloadnum' + numberSelections + '"/><img style="border-width: 0px; display: none;" src="/images/Button-Refresh-Moving.gif" id="ajaxprogressnum' + numberSelections + '"/></div>';
        $(theCellToSelectSummaryInfo.TheCell).append(contentsToAppend).show();
        $(theCellToSelectSummaryInfo.TheCell).addClass("selectedcell");
        $('input', theCellToSelectSummaryInfo.TheCell).unbind('click').attr('readonly', false).bind('click', theCellInfo, handleInputClick);
        inputToHighlight = theCellToSelectSummaryInfo.TargettedInput;
        $(this).oneTime(100, deferredInputSelectAll);
        enableEnterCancelLoad(theCellInfo);

        mostRecentSelectNumberCellEvent = null;
        mostRecentClickedInput = null;
    }

    function handleInputClick(event) {
        /* if (deferSelection(event, this, handleInputClick))
        return; */
        mostRecentSelectNumberCellEvent = event;
        mostRecentClickedInput = this;

        setFocusToCell(mostRecentSelectNumberCellEvent.data);
        var theCellSummaryInfo = getCellSummaryInfoFromEvent(mostRecentSelectNumberCellEvent, mostRecentClickedInput);
        theCellSummaryInfo.TargettedInput.select();
        mostRecentSelectNumberCellEvent = null;
        mostRecentClickedInput = null;
    }

    function setFocusToCell(theCellInfo) {
        theCellWithFocusInfo = theCellInfo;
    }

    /* cancellation */

    function deselectNumberCell(theCell) {
        removeButtonsFromCell(theCell);
        $(theCell).removeClass("selectedcell").trigger('blur');
        $('input', theCell).attr('readonly', true).unbind('click').bind('click', selectNumberCellFromClick);
        updateBulkButtons(false);
    }

    function cancelChangesToCell(theCellInfo) {
        restoreOriginalValues(theCellInfo);
        deselectNumberCell(theCellInfo.TheCell);

    }

    function cancelChangesToCellBasedOnEvent(event) {
        var theCellInfo = event.data;
        cancelChangesToCell(theCellInfo);
    }

    function cancelBulk(event) {
        $('[id^=btncancelnum]').trigger('click');
    }


    /* submission */

    function submitEntriesBasedOnEvent(event) {
        if (event == null || event.data == null)
            return;
        var theCellInfo = event.data;
        submitEntries(theCellInfo);
    }

    function submitEntries(theCellInfo) {
        refreshCellAnimate(theCellInfo.TheCell);
        disableEnterCancelLoad(theCellInfo);

        var newValuesList = new Array();
        recordNewValuesToCellInfo(theCellInfo);
        var theUserRatingsList = createChangedList(theCellInfo.OriginalValues, theCellInfo.NewValues, theCellInfo.RatingIDs);
        var successCallback = Function.createCallback(onSubmitSuccess, theCellInfo);
        var failureCallback = Function.createCallback(onSubmitFailure, theCellInfo);
        PMWebServices.PMWebService.ProcessRatings(theUserAccessInfo, theUserRatingsList, successCallback, failureCallback, theCellInfo);
    }

    function onSubmitSuccess(result, theCellInfo, methodName) {
        refreshCellStopAnimating(theCellInfo.TheCell);
        if (result == null || result.result == null)
            onSubmitFailure(result, theCellInfo, methodName);
        else {
            if (result.result.success) {
                reportMessage(theCellInfo, "Note", result.result.userMessage);
                copyChangedValues(theCellInfo.TheCell, result.currentValues);
                deselectNumberCell(theCellInfo.TheCell);
            }
            else {
                reportMessage(theCellInfo, "Error", result.result.userMessage);
                enableEnterCancelLoad(theCellInfo);
            }
            waitingForProcessing = false;
        }
    }

    function onSubmitFailure(result, theCellInfo, methodName) {
        refreshCellStopAnimating(theCellInfo.TheCell);
        if (result != null && result.result != null && result.result.userMessage != null && result.result.userMessage != "")
            reportMessage(theCellInfo, "Error", result.result.userMessage);
        else
            reportMessage(theCellInfo, "Error", 'Unable to contact the server.');
        enableEnterCancelLoad(theCellInfo);
        waitingForProcessing = false;
    }

    function submitBulk(event) {
        var theCellInfosArray = new Array();
        var activeCells = $('[id^=btnenternum]');
        for (var i = 0; i < activeCells.length; i++) {
            var theCellInfo = getCellInfo($('input.mgID', activeCells.eq(i).parent().parent()).val());
            Array.add(theCellInfosArray, theCellInfo);
        }
        submitBulkHelper(theCellInfosArray);
        // $('[id^=btnenternum]').trigger('click');
    }

    function submitBulkHelper(theCellInfoArray) {
        var theUserRatingsListList = new Array();
        for (var i = 0; i < theCellInfoArray.length; i++) {
            refreshCellAnimate(theCellInfoArray[i].TheCell);
            disableEnterCancelLoad(theCellInfoArray[i]);

            var newValuesList = new Array();
            recordNewValuesToCellInfo(theCellInfoArray[i]);
            var theUserRatingsList = createChangedList(theCellInfoArray[i].OriginalValues, theCellInfoArray[i].NewValues, theCellInfoArray[i].RatingIDs);
            Array.add(theUserRatingsListList, theUserRatingsList);
        }
        var successCallback = Function.createCallback(onSubmitBulkSuccess, theCellInfoArray);
        var failureCallback = Function.createCallback(onSubmitBulkFailure, theCellInfoArray);
        PMWebServices.PMWebService.ProcessRatingsBulk(theUserAccessInfo, theUserRatingsListList, successCallback, failureCallback, theCellInfoArray);
    }

    function onSubmitBulkSuccess(result, theCellInfoArray, methodName) {
        for (var i = 0; i < theCellInfoArray.length; i++) {
            onSubmitSuccess(result[i], theCellInfoArray[i], methodName);
        }
    }

    function onSubmitBulkFailure(result, theCellInfoArray, methodName) {
        for (var i = 0; i < theCellInfoArray.length; i++) {
            onSubmitFailure(result[i], theCellInfoArray[i], methodName);
        }
    }



    /*** key processing ***/

    function processKeyPress(event) {
        if (theCellWithFocusInfo != null) {
            if (event.which == 13) {
                submitEntries(theCellWithFocusInfo);
                return false;
            }
            else if (event.which == 27) {
                cancelChangesToCell(theCellWithFocusInfo);
                return false;
            }
        }
        else {
            if (event.which == 13)
                return false;
            else
                return true; /* default processing */
        }
    }

    function narrowBySearchWordsKeyPress(event) {
        if (event.which == 13) {
            var searchButton = $("#doNarrowBySearchWords");
            searchButton.replaceAttr("src", /\./, "-pushed\.");
            $(this).oneTime(800, function () {
                searchButton.replaceAttr("src", /-pushed./, "\.");
            });
            doNarrowBySearchWords();
            return false;
        }
        return true;
    }




    /*** updating numeric values ***/

    function loadUpdatedValues(theCellInfo) {
        var ratingGroupIDString = $('input.mgID', theCellInfo.TheCell).attr("value");
        var successCallback = Function.createCallback(onLoadUpdatedSuccess, null);
        var failureCallback = Function.createCallback(onLoadUpdatedFailure, null);
        refreshCellAnimate(theCellInfo.TheCell);
        PMWebServices.PMWebService.GetUpdatedRatings(ratingGroupIDString, successCallback, failureCallback, theCellInfo);
    }

    function loadUpdatedValuesBasedOnEvent(event) {
        var theCellInfo = event.data;
        loadUpdatedValues(theCellInfo);
    }

    function onLoadUpdatedSuccess(result, theCellInfo, methodName) {
        if (result.result.success) {
            copyChangedValues(theCellInfo.TheCell, result.currentValues);
        }
        refreshCellStopAnimating(theCellInfo.TheCell);
    }

    function onLoadUpdatedFailure(result, theCellInfo, methodName) {
        refreshCellStopAnimating(theCellInfo.TheCell);
    }

    function loadUpdatedValuesForPage() {
        if (waitingForProcessing == true)
            return false;
        waitingForProcessing = true;
        refreshPageAnimate();
        var allRatingIDs = recordRatingIDs(mainTable);
        var successCallback = Function.createCallback(onLoadPageSuccess, null);
        var failureCallback = Function.createCallback(onLoadPageFailure, null);
        PMWebServices.PMWebService.GetUpdatedRatingsMultiple(allRatingIDs, successCallback, failureCallback);
        return false;
    }

    function onLoadPageSuccess(result, context, methodName) {
        refreshPageStopAnimating();
        if (result.result.success) {
            copyChangedValues(mainTable, result.currentValues);
        }
        waitingForProcessing = false;
    }

    function onLoadPageFailure(result, context, methodName) {
        refreshPageStopAnimating();
        waitingForProcessing = false;
    }


    /*** cell manipulation and information ***/

    function restoreOriginalValues(theCellInfo) {
        var totalOriginalValues = theCellInfo.OriginalValues.length;
        var index;
        for (index = 0; index < totalOriginalValues; index++) {
            var theInputFieldNameFilter = 'input[name=mkt' + theCellInfo.RatingIDs[index] + ']';
            var theInputField = $(theInputFieldNameFilter, theCellInfo.TheCell);
            if (theInputField.val() != theCellInfo.OriginalValues[index])
                theInputField.val(theCellInfo.OriginalValues[index]);
        };
    }

    function copyChangedValues(lCell, lNewRatingsAndUserRating) {
        var totalNewValues = lNewRatingsAndUserRating.length;
        var index;
        for (index = 0; index < totalNewValues; index++) {
            var newRatingID = lNewRatingsAndUserRating[index].ratingID;
            var newUserRating = lNewRatingsAndUserRating[index].theUserRating;
            var theInputFieldNameFilter = 'input[name=mkt' + newRatingID + ']';
            var theInputField = $(theInputFieldNameFilter, lCell);
            if (theInputField.val() != newUserRating)
                theInputField.val(newUserRating);
        };
    }

    function cellHasChanged(theCellInfo) {
        var totalOriginalValues = theCellInfo.OriginalValues.length;
        var index;
        for (index = 0; index < totalOriginalValues; index++) {
            var theInputFieldNameFilter = 'input[name=mkt' + theCellInfo.RatingIDs[index] + ']';
            var theInputField = $(theInputFieldNameFilter, theCellInfo.TheCell);
            if (theInputField.val() != theCellInfo.OriginalValues[index])
                return true;
        };
        return false;
    }

    function getRatingID(theInputField) {
        var ratingID = theInputField.attr("name").substring(3, 100);
        return ratingID;
    }

    function getRatingGroupID(aCellOrOtherArea) {
        var theInputField = $('input.mgID:first', aCellOrOtherArea);
        var ratingGroupID = theInputField.val();
        return ratingGroupID;
    }

    function recordRatingIDs(aCellOrOtherArea) {
        var ratingIDs = new Array();
        $('input.rtg', aCellOrOtherArea).each(function () {
            var ratingID = getRatingID($(this));
            Array.add(ratingIDs, ratingID);
        });
        return ratingIDs;
    }

    function recordRatingValues(aCell) {
        var valuesList = new Array();
        $('input.rtg', aCell).each(function () {
            Array.add(valuesList, $(this).val());
        });
        return valuesList;
    }

    function recordOriginalCellInfo(aCell) {
        var theCellInfo = new Object();
        theCellInfo.TheCell = aCell;
        theCellInfo.RatingGroupID = getRatingGroupID(aCell);
        theCellInfo.RatingIDs = recordRatingIDs(aCell);
        theCellInfo.OriginalValues = recordRatingValues(aCell);
        rememberCellInfo(theCellInfo);
        return theCellInfo;
    }

    var recordedCellInformation = new Object();
    function rememberCellInfo(theCellInfo) {
        recordedCellInformation[theCellInfo.RatingGroupID] = theCellInfo;
    }
    function getCellInfo(ratingGroupID) {
        return recordedCellInformation[ratingGroupID];
    }

    function recordNewValuesToCellInfo(theCellInfo) {
        var valuesList = new Array();
        $('input.rtg', theCellInfo.TheCell).each(function () {
            Array.add(valuesList, $(this).val());
        });
        theCellInfo.NewValues = valuesList;
    }


    function createUserRatingAndRatingObject(theUserRating, theRatingID) {
        var theUserRatingAndRating = new ClassLibrary1.Model.RatingAndUserRatingString();
        theUserRatingAndRating.theUserRating = theUserRating;
        theUserRatingAndRating.ratingID = theRatingID;
        return theUserRatingAndRating;
    }

    function createChangedList(originalList, newList, theRatingIDs) {
        var maxNumUserRatings = newList.length;
        var theUserRatingsList = new Array();
        var index = 0;
        for (index = 0; index < maxNumUserRatings; index++) {
            if (originalList[index] != newList[index]) {
                var theUserRatingAndRating = createUserRatingAndRatingObject(newList[index], theRatingIDs[index]);
                Array.add(theUserRatingsList, theUserRatingAndRating);
            }
        };
        return theUserRatingsList;
    }

    function getCellFromItemInCell(item) {
        var theParent = item;
        while (theParent.nodeName != "TD" || theParent.className.substring(0, 14) != "mainCellMarker") {
            theParent = theParent.parentNode;
        }
        return theParent;
    }

    function getCellSummaryInfoFromItemInCell(item) {
        var theCellSummaryInfo = new Object();
        theCellSummaryInfo.TheCell = getCellFromItemInCell(item);
        theCellSummaryInfo.TargettedInput = $(item);
        theCellSummaryInfo.FocusAtTimeOfEvent = theCellWithFocusInfo;
        return theCellSummaryInfo;
    }

    function getCellSummaryInfoFromEvent(event, theEventContext) {
        var theCellSummaryInfo = new Object();
        theCellSummaryInfo.TheCell = getCellFromItemInCell(theEventContext.parentNode);
        theCellSummaryInfo.TargettedInput = $(event.target);
        theCellSummaryInfo.FocusAtTimeOfEvent = theCellWithFocusInfo;
        return theCellSummaryInfo;
    }

    /* helps determine whether we searched for class=nmcl instead of class="nmcl" */
    var omitQuotesAroundNMCL = null;

    function getRowNumFromString(rowString) {
        var charsToSkip = indexOfTD(rowString, true) + 17;
        if (omitQuotesAroundNMCL)
            charsToSkip -= 2;
        var keepGoingThrough = indexOfTD(rowString, false);
        return parseInt(rowString.substring(charsToSkip, keepGoingThrough))
    }


    function indexOfTD(rowString, opening) {
        var lowercase;
        var upcase;
        var lowercaseNoQuote = -1;
        var upcaseNoQuote = -1;
        if (opening) {
            lowercaseNoQuote = rowString.indexOf("<td class=nmcl>");
            upcaseNoQuote = rowString.indexOf("<TD class=nmcl>");
            lowercase = rowString.indexOf("<td class=\"nmcl\">");
            upcase = rowString.indexOf("<TD class=\"nmcl\">");
        }
        else {
            lowercase = rowString.indexOf("</td>");
            upcase = rowString.indexOf("</TD>");
        }
        if (lowercaseNoQuote == -1 && upcaseNoQuote == -1) {
            omitQuotesAroundNMCL = false;
            if (lowercase == -1)
                return upcase;
            if (upcase == -1)
                return lowercase;
            if (lowercase < upcase)
                return lowercase;
            return upcase;
        }
        else {
            omitQuotesAroundNMCL = true;
            if (lowercaseNoQuote == -1)
                return upcaseNoQuote;
            if (upcaseNoQuote == -1)
                return lowercaseNoQuote;
            if (lowercaseNoQuote < upcaseNoQuote)
                return lowercaseNoQuote;
            return upcaseNoQuote;
        }
    }


    jQuery.fn.getRowNum = function () {
        //var theRowNum = this.data('rownum');
        //if (theRowNum == null)
        theRowNum = getRowNumFromString(this[0].innerHTML);
        return theRowNum;
    }

    /*** error and message reporting ***/

    function reportMessage(theCellInfo, theTitle, theMessage) {
        reportMessageAtLocation(theCellInfo.TheCell, theTitle, theMessage);
    }

    function reportMessageAtLocation(theLocation, theTitle, theMessage) {
        if (theMessage != null && theMessage != "") {
            numberMessages++;
            $('[id^=cluetipdiv]', theLocation).remove();
            var contentToAdd = '<div id="cluetipdiv' + numberMessages + '"><a id="cluetipanchor' + numberMessages + '" href="#messagetext' + numberMessages + '" rel="#messagetext' + numberMessages + '" title="' + theTitle + '" /><p id="messagetext' + numberMessages + '" visible="false">' + theMessage + '</p></a></div>';
            $("[id^=entercancelinner]", theLocation).prepend(contentToAdd);
            var cluetipmessageanchor = $("#cluetipanchor" + numberMessages, theLocation);
            cluetipmessageanchor.cluetip({
                activation: 'click',
                closePosition: 'title',
                closeText: '<img src="/images/cross.png" alt="" />',
                delayedClose: 20000,
                arrows: true,
                local: true,
                clickThrough: true,
                sticky: true
            }
                    );
            cluetipmessageanchor.trigger('click');
        }

    }

    /*** button image methods ***/

    function enableEnterCancelLoad(theCellInfo) {
        $('img[id^=btnenternum]', theCellInfo.TheCell).show().css('opacity', 1.0).bind('click', theCellInfo, submitEntriesBasedOnEvent).autoHilite();
        $('img[id^=btncancelnum]', theCellInfo.TheCell).show().css('opacity', 1.0).bind('click', theCellInfo, cancelChangesToCellBasedOnEvent).autoHilite();
        $('img[id^=btnloadnum]', theCellInfo.TheCell).show().css('opacity', 1.0).bind('click', theCellInfo, loadUpdatedValuesBasedOnEvent).autoHilite();
    }

    function disableEnterCancelLoad(theCellInfo) {
        $('img[id^=btnenternum]', theCellInfo.TheCell).show().css('opacity', 0.20).unbind();
        $('img[id^=btncancelnum]', theCellInfo.TheCell).show().css('opacity', 0.20).unbind();
        $('img[id^=btnloadnum]', theCellInfo.TheCell).css('opacity', 0.20).unbind();
        $('input.rtg', theCellInfo.TheCell).blur();
    }

    function refreshCellAnimate(theCell) {
        $('img[id*=btnloadnum]', theCell).hide();
        $('img[id*=ajaxprogress]', theCell).show();
    }

    function refreshCellStopAnimating(theCell) {
        $('img[id*=btnloadnum]', theCell).show();
        $('img[id*=ajaxprogress]', theCell).hide();
    }

    function refreshPageAnimate() {
        $('img[id*=RefreshPageStill]').hide();
        $('img[id*=RefreshPageMoving]').show();
    }

    function refreshPageStopAnimating() {
        $('img[id*=RefreshPageMoving]').hide();
        $('img[id*=RefreshPageStill]').show();
    }

    function removeButtonsFromCell(theCell) {
        $('[id^=entercancelinner]', theCell).remove();
        $("input", theCell);
    }

    function updateBulkButtons(adding) {
        if (adding)
            numberCurrentSelections++;
        else
            numberCurrentSelections--;
        if (numberCurrentSelections == 0)
            $('[id$=Bulk]').hide();
        else
            $('[id$=Bulk]').show();
    }


    /*** table styling ***/

    function setWidthAndMax(element, newSize) {
        element.css({ 'width': newSize + 'px', 'max-width': newSize + 'px' });
    }


    function resizePageVerticallyHandler(sender, args) {
        resizePageVertically(); /* in resize.js */
    }


    var lastWidthDifference = -1;
    function resizePageToFitTableIfNecessary() {
        var theInnerWidth = mainTable.innerWidth();
        if (mainTableOrigWidth == 0)
            mainTableOrigWidth = 608; /* IE6 issue */
        var difference = theInnerWidth - mainTableOrigWidth;
        if (difference != lastWidthDifference) {
            if (difference > 0) {
                setWidthAndMax(divAroundMainTable, divAroundMainTableOrigWidth + difference);
                setWidthAndMax(rightPageColumn, rightPageColumnOrigWidth + difference);
                setWidthAndMax(mainPartOfPage, mainPartOfPageOrigWidth + difference);
            }
            else {
                setWidthAndMax(divAroundMainTable, divAroundMainTableOrigWidth);
                setWidthAndMax(rightPageColumn, rightPageColumnOrigWidth);
                setWidthAndMax(mainPartOfPage, mainPartOfPageOrigWidth);
            }
            lastWidthDifference = difference;
        }
    }
    /*
    *Stripes table rows
    */
    function zebraStripe() {
        $("#maint > tbody > tr.somerow").zebraStripeRow();
    }


    jQuery.fn.zebraStripeRow = function () {
        return this.each(function () {
            var theRow = $(this);
            if (theRow.data('zebra'))
                return;
            theRow.data('zebra', true);
            rowNum = theRow.getRowNum();
            if (rowNum % 2 == 0)
                theRow.addClass("altrow")
            else if (rowNum % 2 == 1)
                theRow.addClass("row");
        });
    }



    /*** Handle clicks in trees within cells ***/

    function handleClickInTree(NODE, TREE_OBJ) {
        $(this).oneTime(200, function () {
            if (theDeferredFunction != null)
                theDeferredFunction(mostRecentSelectNumberCellEvent); // hack to get selection after jstree is done processing
        }
        );
        return false;
    }

    function handleOpenOrClose(NODE, TREE_OBJ) {
    }

    jQuery.fn.setupTree = function () {
        return this.each(function () {
            setupTree($(this));
        });
    }

    function setupTree(theTreeDiv) {
        theTreeDiv.tree({
            callback: {
                beforechange: handleClickInTree,
                beforeopen: handleOpenOrClose,
                beforeclose: handleOpenOrClose
            },
            types:
            {
                "default": {
                    clickable: true,
                    renameable: false,
                    deletable: false,
                    creatable: false,
                    draggable: false
                }
            },
            ui: {
                dots: false,
                theme_name: "classic"
            }
        }).removeClass('treenew');
    }


    /*** setup live events ***/

    function setupLiveEvents() {
        setupRatingInputs();
        setupHover();
        setupClueTips();
    }

    function setupRatingInputs() {
        $('input.rtg')/*.live('keypress', processKeyPress)*/.live('click', selectNumberCellFromClick).live('focus', selectNumberCellAfterFocus).live('blur', onInputBlur); /* needed for cells populated on server */
        $('form').keypress(processKeyPress);
    }

    jQuery.fn.liveCluetips = function (cluetipSettings) {
        $(this.selector).live("mouseenter", function (event) {
            var theSource = $(this);
            if (!theSource.data("liveCluetipSetup")) {
                theSource.data("liveCluetipSetup", true).cluetip(cluetipSettings).trigger('liveCluetip');
            }
        });
        return this;
    }

    function setupClueTips() {
        $('div.loadcolumnpopup', headerTable).liveCluetips({ positionBy: 'bottomTop', local: true, clickThrough: true, cursor: 'pointer', arrows: false, delayedClose: 5000 });
        $('div.loadrowpopup', mainTable).liveCluetips({ local: true, clickThrough: true, cursor: 'pointer', arrows: true });
    }

    var currentlyHilited = null;
    function setupHover() {
        $("#maint > tbody > tr.somerow")
            .live("mouseenter",
                function (event) {
                    var thisRow = $(this);
                    if (currentlyHilited != null && currentlyHilited != thisRow)
                        currentlyHilited.removeClass("hilited");
                    if (currentlyHilited == null || currentlyHilited.getRowNum() != thisRow.getRowNum()) {
                        currentlyHilited = $(this);
                        currentlyHilited.addClass("hilited");
                        zoomIntoMarker(thisRow);
                    }
                })
            .live("mouseleave",
                function (event) {
                    var theRow = $(this);
                    if (currentlyHilited != null && theRow.getRowNum() == currentlyHilited.getRowNum()) {
                        theRow.removeClass("hilited");
                        currentlyHilited = null;
                        zoomOutOfMarker();
                    };
                });
        $("#GoogleMap").bind("mouseenter", mouseOverMap).bind("mouseleave", mouseOutOfMap);
    }

    var mainTableHeight = null;
    function getMainTableHeight() {
        if (mainTableHeight == null)
            mainTableHeight = divAroundMainTable.height() - $(".searchRow").eq(0).height() - 1; /* compensate for absolute positioning */
        return mainTableHeight;
    }


    /*** table sorting ***/

    /* set up sort context menu */

    function setupSortMenu() {
        setupSortMenuContents();
        resetSortMenuFunctionality();
    }

    function resetSortMenuFunctionality() {
        $(".searchWordsSort").destroyContextMenu();
        $(".searchWordsSort").contextMenu({
            menu: "sortMenuContents"
        },
            processTableSortInstruction);
    }

    /* set up the sort menu contexts by loading data from tableInfo */

    var sortMenuContents = null;
    function setupSortMenuContents() {
        var frame = "<ul id=\"sortMenuContents\" class=\"contextMenu\"></ul>";
        $("#sortTrigger").before(frame);  // must put by element for plugin to work properly tableInfo.after(frame);
        sortMenuContents = $("#sortMenuContents");
        $("li", sortMenuContents).remove();
        var theTableInfoObject = getTableInfoObject();
        var itemsForSortMenu = theTableInfoObject.SortMenu;
        for (var count = 1; count <= itemsForSortMenu.length; count++) {
            var itemForSortMenu = itemsForSortMenu[count - 1];
            var separatorText = "";
            var itemText = "<li id=\"SortMenu" + count + "\" class=\"" + separatorText + "\">" + "<a href=\"#" + itemForSortMenu.I + "\">" + itemForSortMenu.M + "</a>" + "</li>";
            sortMenuContents.append(itemText);
        }
    }

    /* after a user clicks in a table header to sort a column or by name, */
    /* change the sort to the table header triggering the event, */
    /* and then actually update the table */

    function changeSortEvent() {
        if (waitingForProcessing == true)
            return;
        changeSort($(this) /* th */, null, true);
    }

    /* set a particular column in the table header to sort by. */
    function specifySort(TblColumnID, sortAsc, doUpdateTable) {
        var anchor = sortableTH.find("#SortColumn" + TblColumnID);
        var th;
        if (anchor.length == 1) {
            th = anchor.eq(0).parent().parent();
            changeSort(th, sortAsc, doUpdateTable);
        }
    }

    /* places the sorting indicator on a particular table header. */
    /* marks as ascending/descending, or if sortAsc is null, sets it based on current status. */

    function changeSort(th, sortAsc, doUpdateTable) {
        if (sortAsc == null)
            sortAsc = th.hasClass("sortdesc") || th.hasClass("sortableasc") || th.hasClass("sortdescVertText") || th.hasClass("sortableascVertText");
        var currentlySorted = th.hasClass("sortasc") || th.hasClass("sortdesc") || th.hasClass("sortascVertText") || th.hasClass("sortdescVertText");
        var vert = th.hasClass("sortdescVertText") || th.hasClass("sortableascVertText") || th.hasClass("sortascVertText") || th.hasClass("sortabledescVertText");
        if (!currentlySorted) {
            cancelCurrentSorting(false);
        }
        setSortAttributes(th, true, sortAsc, vert, doUpdateTable);
    }

    /* takes the column heading currently designated as sorting and makes it so that it isn't so designated. */
    /* should call this either before changing the indicated sort column or if sorting by something other than a column */

    function cancelCurrentSorting(doUpdateTable) {
        var prevSort = sortableTH.filter(".sortasc,.sortdesc,.sortascVertText,.sortdescVertText");
        if (prevSort.length > 0) {
            prevSort = prevSort.eq(0);
            var wasAsc = prevSort.hasClass("sortasc") || prevSort.hasClass("sortascVertText");
            var prevSortVert = prevSort.hasClass("sortdescVertText") || prevSort.hasClass("sortableascVertText") || prevSort.hasClass("sortascVertText") || prevSort.hasClass("sortdescVertText");
            setSortAttributes(prevSort, false, wasAsc, prevSortVert, doUpdateTable);
        }
    }

    /* change the current sorting designations in a particular table header and can update the table */
    /* based on that table header (i.e., can sort by column, including entity name */

    function setSortAttributes(th, sorted, asc, vert, doUpdateTable) {
        th.removeClass("sortasc").removeClass("sortdesc").removeClass("sortableasc").removeClass("sortabledesc").removeClass("sortascVertText").removeClass("sortdescVertText").removeClass("sortableascVertText").removeClass("sortabledescVertText");
        var theString = "sort";
        if (!sorted)
            theString = theString + "able";
        if (asc)
            theString = theString + "asc";
        else
            theString = theString + "desc";
        if (vert)
            theString = theString + "VertText";
        th.addClass(theString);

        var colImg = $("img", th).filter('[id*=ColImage]').eq(0);
        if (colImg != null && colImg.size() != 0) {
            var colImgSrc = colImg.attr('src');
            var newImgSrc = colImgSrc.substr(0, colImgSrc.length - 1) + (sorted ? "1" : "0");
            colImg.attr('src', newImgSrc);
        }

        if (doUpdateTable) {
            var TblColumnToSort;
            if (!sorted)
                TblColumnToSort = null;
            else {
                TblColumnToSort = $("a", th).attr("id").substr(10, 1000);
                if (TblColumnToSort == "")
                    TblColumnToSort = null;
            }
            updateTableSortedByCategory(TblColumnToSort, asc);
        }
    }

    /* process table sort instruction */
    /* For sorting by distance, we must insert the correct longitude/latitude. */
    /* Also, we must update the user interface. */
    function processTableSortInstruction(instruction, el, pos) {
        if (instruction == null || instruction == "")
            return;
        while (instruction.indexOf('#') != -1) {
            var theIndex = instruction.indexOf('#');
            instruction = instruction.substr(theIndex + 1);
        }
        var splitResult = instruction.split(",");
        if (splitResult[0] == "C") {
            specifySort(parseInt(splitResult[1]), splitResult[2] == "true", false);
        }
        else {
            if (splitResult[0] == "D") {
                if (splitResult[1] == -1 && splitResult[2] == -1) {
                    var userLoc = google.loader.ClientLocation;
                    if (userLoc == null) {
                        alert('Sorry, your location could not be determined.');
                        return;
                    }
                    instruction = "D," + userLoc.latitude + "," + userLoc.longitude + "," + splitResult[3] + ",true";
                }
                else if (splitResult[1] == -2 && splitResult[2] == -2) {
                    var userLoc2;
                    if (map != null)
                        userLoc2 = map.getCenter();
                    if (map == null || userLoc2 == null) {
                        alert('Sorry, the center of the map could not be determined.');
                        return;
                    }
                    instruction = "D," + userLoc2.lat() + "," + userLoc2.lng() + "," + splitResult[3] + ",true";
                }
            }
            cancelCurrentSorting(false);
        }
        updateTableSort(instruction);
    }

    /* updates the table to the specified category descriptor and sort direction */

    function updateTableSortedByCategory(TblColumnToSort, sortAsc) {
        var instruction;
        if (TblColumnToSort == null)
            instruction = sortInstructionTblRow(sortAsc);
        else
            instruction = sortInstructionCategory(TblColumnToSort, sortAsc);
        updateTableSort(instruction);
    }

    /* updates the table sort based on a particular instruction */

    function updateTableSort(instruction) {
        setSortInstruction(instruction);
        populateTable(false, 0);
    }


    /* sets the sort instruction */
    function setSortInstruction(theInstruction) {
        var theTableInfoObject = getTableInfoObject();
        theTableInfoObject.SortInstruction = theInstruction;
        replaceTableInfo(theTableInfoObject);
    }


    /* generates sort instructions for some types of sorting */
    /* only those that need to be generated on the fly are included */
    function sortInstructionCategory(TblColumnID, ascending) {
        return "C," + TblColumnID + "," + (ascending ? "true" : "false");
    }

    function sortInstructionDistance(latitude, longitude, ascending) {
        return "D," + latitude + "," + longitude + "," + (ascending ? "true" : "false");
    }

    function sortInstructionTblRow(ascending) {
        return "E," + (ascending ? "true" : "false");
    }

    /*** table tabs -- changing the set of columns that are shown ***/

    /* produce a list of tabs that is scrollable if necessary, */
    /* converting from the drop down list that is passed in */
    function makeTableTabs() {
        var theSelect = $('[id$=DdlCategory]'); /* convert from select used for non-js users */
        if (theSelect.length == 0)
            return;
        var theOptions = $('[id$=DdlCategory] option');
        var tabsString = "<ul class=\"css-tabs items\">";
        var panesString = "<div class=\"tabPanes\">";
        for (var i = 0; i < theOptions.length; i++) {
            var theOption = $(theOptions[i]);
            panesString = panesString + "<div>" + theOption.attr("value") + "</div>";
            tabsString = tabsString + "<li><a href=\"#\">" + theOption.html() + "</a></li>";
        }

        panesString = panesString + "</div>";
        tabsString = tabsString + "</ul>";

        var combined = "<a class=\"prevPage browse nextToTabs left\"></a><div class=\"scrollable\">" + tabsString + "</div><a class=\"nextPage browse nextToTabs right\"></a>" + panesString;
        theSelect.after(combined);
        theSelect.remove();
        theTabs = $("ul.css-tabs").tabs($("div.tabPanes > div"), { api: true });
        theTabs.onClick(changeTblTab);
        var totalWidth = 0;
        $(".css-tabs > li").each(function () {
            totalWidth += $(this).width();
        });
        if (totalWidth > 450) // (theTabs.length > 2)
            $("div.scrollable").scrollable({ size: 1, items: '.css-tabs' });
        else
            $(".nextToTabs").hide();
    }

    function changeTblTab(event, index) {
        var TblTabID = theTabs.getPanes().eq(index).text();
        updateTableToNewTblTab(TblTabID);
    }


    function updateTableToNewTblTab(newTblTabID) {
        var currentHtml = tableInfo.html();
        currentHtml = currentHtml.replace(new RegExp("TblTabID\":(null|[0-9]*)"), "TblTabID\":" + newTblTabID);
        tableInfo.html(currentHtml);
        var firstRow = findFirstRow();
        var rowsToSkip = 0;
        if (firstRow != null)
            rowsToSkip = firstRow.getRowNum() - 1;
        populateTable(true, rowsToSkip);
    }

    /*** narrow results popup ***/

    function openNarrowResults() {
        if (waitingForProcessing == true)
            return;

        if (!narrowResultsSetup) {
            var popupString = "<div id=\"narrowPopup\" class=\"narrowPopup\"><div id=\"controlNarrow\" class=\"controlNarrow\" ><table><tr><td><img id=\"doNarrow\" src=\"/images/go-normal.png\"/></td><td><img id=\"doClose\" src=\"/images/close-normal.png\"/></td></tr></table></div><div id=\"narrowIFrameContainer\" class=\"narrowIFrameContainer\" /></div>";
            $("#narrowTrigger").after(popupString);
            $("#doClose").click(closeNarrowResults).customHilite();
            $("#doNarrow").click(doNarrowResults).customHilite();
        }
        var narrowPopup = $("#narrowPopup");
        narrowPopup.show();

        if (!narrowResultsSetup) {
            narrowResultsSetup = true;
            var theIFrameContainer = $("#narrowIFrameContainer", narrowPopup.eq(0));
            var theTableInfoObject = getTableInfoObject();
            var iFrameString = "<iframe id=\"narrowIFrame\" class=\"narrowIFrame\" src=\"/WebForms/NarrowResults.aspx?TableId=" + theTableInfoObject.TblID + "&SubtableId=" + theTableInfoObject.TblTabID + "\" />";
            theIFrameContainer.append(iFrameString);
            completeOpenResults();
        }

        return false; // don't postback
    }

    function completeOpenResults() {
    }


    function getIFrame(selector) {
        var iFrame = $("#narrowIFrame");
        if (iFrame == null || iFrame.length == 0)
            return null;
        return iFrame.contents().find(selector);
    }


    function closeNarrowResults() {
        var narrowPopup = $("#narrowPopup");
        narrowPopup.hide();
    }

    function getNarrowResultsCount() {
        var narrowResultsCount = getIFrame("#narrowResultsCount");
        if (narrowResultsCount == null)
            return "";
        var currentVal = narrowResultsCount.eq(0).val();
        return currentVal;
    }

    var narrowResultsCountLast = null;
    function doNarrowResults() {
        narrowResultsCountLast = getNarrowResultsCount();
        $(this).everyTime(50, "narrowResultsCheck", checkChange); /* wait for the count to change after the form is submitted */
        getIFrame("form").submit();
    }

    function checkChange() {
        if (waitingForProcessing == true)
            return;
        var currentVal = getNarrowResultsCount();
        if (currentVal != narrowResultsCountLast) {
            var iFrameCheck = getIFrame("#FilterRulesInfo");
            if (iFrameCheck != null && iFrameCheck.length == 1) { // must wait for the iFrame to completely load
                $(this).stopTime("narrowResultsCheck");
                respondToChange();
            }
        }
    }

    function respondToChange() {
        if (waitingForProcessing == true)
            return;
        var validationField = getIFrame("#FieldsBox_FieldsValidator");
        var newFieldsValidatorText = "";
        if (validationField != null && validationField.length == 1)
            newFieldsValidatorText = validationField.text();
        if (newFieldsValidatorText != "") {
            alert(newFieldsValidatorText);
            reportMessageAtLocation($("#doNarrow").eq(0), "Error", newFieldsValidatorText);
        }
        else {
            var filterRulesOutput = getIFrame("#FilterRulesInfo");
            if (filterRulesOutput != null && filterRulesOutput.length == 1) {
                var theTableInfoObject = getTableInfoObject();
                var existingFilterText = $.toJSON(theTableInfoObject.Filters);
                var filterRulesOutputText = filterRulesOutput.eq(0).text();
                if (existingFilterText != filterRulesOutputText) {
                    theTableInfoObject.Filters = $.evalJSON(filterRulesOutputText);
                    theTableInfoObject.Filters.GetNewDateTime = true; // start a brand new query
                    replaceTableInfo(theTableInfoObject);
                    closeNarrowResults();
                    populateTable(false, 0);
                }
            }
        }
    }

    function doNarrowBySearchWords() {
        if (waitingForProcessing == true)
            return;
        var theTableInfoObject = getTableInfoObject();
        var searchText = $("#searchWordsFilter").val();
        searchText = searchText.replace(new RegExp("[^a-z 0-9A-Z]", "g"), "");
        theTableInfoObject.Filters.theFilterRules = $.evalJSON("[{\"id\":" + theTableInfoObject.TblID + ",\"type\":\"search\",\"search\":\"" + searchText + "\"}]");
        theTableInfoObject.Filters.GetNewDateTime = true;
        var newTableInfo = $.toJSON(theTableInfoObject);
        newTableInfo = convertToMSDateFormat(newTableInfo);
        tableInfo.html(newTableInfo);
        populateTable(false, 0);
    }


    /*** Google maps & marker management ***/

    function initGoogleMapStep1() {
        var script = document.createElement("script");
        var scriptsrc = "http://www.google.com/jsapi?key=ABQIAAAAs-ZODfxj8f9LNfbR_FZHTRTfRkUUk3oX8DgAj5sWGEMsn5h-rhRi9M18xlAezldqPnUIGtD9BZ6w3g";
        // DEVELOPMENT VERSION scriptsrc = "http://www.google.com/jsapi?key=ABQIAAAAnfs7bKE82qgb3Zc2YyS-oBT2yXp_ZAY8_ufC3CFXhHIE1NvwkxSySz_REpPq-4WZA27OwgbtyR3VcA";
        // RELEASE VERSION
        //        script.src = scriptsrc;
        //        script.type = "text/javascript";
        //        document.getElementsByTagName("head")[0].appendChild(script);
        jQuery.getScript(scriptsrc, initGoogleMapStep2);
    }

    function initGoogleMapStep2() {
        google.load("maps", "2.x", { "callback": initGoogleMapStep3 });
    }

    function initGoogleMapStep3() {
        jQuery.getScript('/js/markerclusterer.js', initGoogleMapStep4);
        jQuery.getScript('/js/mapiconmaker.js', initGoogleMapStep4);
    }

    var numCallsInitGoogleStep4 = 0;
    var markerManager = null;
    var markerCluster = null;
    function initGoogleMapStep4() {
        numCallsInitGoogleStep4++;
        if (numCallsInitGoogleStep4 < 2)
            return;
        createMap();
    }

    function createMap() {
        var mapArea = $("#GoogleMap");
        mapArea.show();
        resizePageVertically();
        if (map != null)
            delete map;
        map = new google.maps.Map2(mapArea.get(0)); /* must use get, not eq */
        map.setCenter(new google.maps.LatLng(38.0, -97), 2);
        map.addControl(new GLargeMapControl());
        mcOptions = { gridSize: 40, maxZoom: 13 };
        markerCluster = new MarkerClusterer(map, [], mcOptions);
        updateMarkers();
    }


    jQuery.fn.addAddressesFromRow = function () {
        return this.each(function () {
            addAddressesToMap($(this));
        });
    }

    function addAddressesToMap(theArea) {
        var matches = $(".latlong", theArea);
        if (matches.length > 0) {
            for (var i = 0; i < matches.length; i++) {
                var match = matches.eq(i);
                var row = match.parents("tr.somerow").eq(0);
                var lat = match.attr("lat");
                var lng = match.attr("long");
                var address = "";
                address = match.attr("address");
                var newMarker = markerInfo(lat, lng, address, row, true, false);
                row.data('marker', newMarker);
                addMarkerToList(newMarker);
            }
        }
    }

    function geocodeBeforeAdding(marker) {
        var geocoder = new GClientGeocoder();
        if (marker.Address != null) {
            geocoder.getLatLng(marker.Address, function (point) { /* note that this fn will be called later */
                if (point) {
                    marker.Lat = point.lat();
                    marker.Lng = point.lng();
                    if (!marker.Remove)
                        marker.Add = true;
                }
                else {
                    marker.Add = false;
                    marker.Remove = true;
                }
                updateMarkers();
            }
     );
        }
        else {
            marker.Add = false;
            marker.Remove = true;
            updateMarkers();
        }
    }

    function removeAddresses(theArea) {
        if (map == null)
            return;
        if (theArea == mainTable) {
            removeAllMarkers();
            mapInitializationComplete = false;
            return;
        }
        var matches = $(".latlong", theArea);
        if (matches.length > 0) {
            for (var i = 0; i < matches.length; i++) {
                var match = matches.eq(i);
                var row = match.parents(".row,.altrow").eq(0);
                removeMarkerFromList(row);
            }
        }
        updateMarkers();
    }

    function markerIsInListForRowNum(rowNum) {
        return getMarkerForRowNum(rowNum) != null;
    }

    function getMarkerForRowNum(rowNum) {
        for (var i = 0; i < markers.length; i++) {
            if (markers[i] != null && markers[i].RowNum == rowNum) {
                return markers[i];
            }
        }
        return null;
    }



    function swapMarkers(oldMarker, newMarker) {
        for (var i = 0; i < markers.length; i++) {
            if (markers[i].GMarker == oldMarker) {
                markerCluster.removeMarker(oldMarker);
                markers[i].GMarker = newMarker;
                markerCluster.addMarker(newMarker);
                return true;
            }
        }
        return false;
    }

    var hiddenMarker = null;
    var currentlyHilitedMarker = null;
    function hiliteMarker(theGMarker) {
        // unhiliteMarker();
        // var latLng = theGMarker.getLatLng();
        // var newIcon = MapIconMaker.createMarkerIcon({ width: 64, height: 64, primaryColor: "#00ff00" });
        // var newMarker = new GMarker(latLng, { icon: newIcon });
        // hiddenMarker = theGMarker;
        // currentlyHilitedMarker = newMarker;
        // swapMarkers(theGMarker, newMarker);
    }

    function unhiliteMarker() {
        // if (hiddenMarker != null) {
        // swapMarkers(currentlyHilitedMarker, hiddenMarker);
        // hiddenMarker = null;
        // currentlyHilitedMarker = null;
        // }
    }

    var repositionAfterZoomingOut = false;
    function repositionMap() {
        if (map != null) {
            if (netEntrances > 0 || mouseOverMapMode) {
                repositionAfterZoomingOut = true;
                return;
            }
            repositionAfterZoomingOut = false;
            var bounds = new GLatLngBounds;
            var reposition = false;
            for (var i = 0; i < markers.length; i++) {
                if (markers[i] != null && markers[i].GMarker != null) {
                    var theGLatLng = new GLatLng(markers[i].Lat, markers[i].Lng);
                    bounds.extend(theGLatLng);
                    reposition = true;
                }
            }
            if (reposition) {
                map.setZoom(map.getBoundsZoomLevel(bounds));
                map.setCenter(bounds.getCenter());
                preserveZoomInfo();
            }
        }
    }

    var netEntrances = 0;
    var outZoomLevel = null;
    var outCenter = null;
    var inZoomLevel = null;
    var inCenter = null;
    var zoomOutComplete = true;
    var rememberMarker = null;
    function preserveZoomInfo() {
        if (map == null)
            return;
        if (lastZoomWasIntoMarker) {
            inZoomLevel = map.getZoom();
            inCenter = map.getCenter();
        }
        else {
            outZoomLevel = map.getZoom();
            outCenter = map.getCenter();
        }

    }

    function checkUserHasMovedMap(aboutToZoomOut) {
        if (map == null)
            return false;
        if (aboutToZoomOut && (map.getZoom() != inZoomLevel || map.getCenter() != inCenter)) // about to zoom out
            return true;
        if (!aboutToZoomOut && (map.getZoom() != outZoomLevel || map.getCenter() != outCenter)) // about to zoom in
            return true;
        return false;
    }

    var lastZoomWasIntoMarker = false;
    function zoomIntoMarker(row) {
        netEntrances++;
        if (netEntrances > 1 && lastZoomWasIntoMarker) { // we missed a mouseout event
            netEntrances = 1;
            lastZoomWasIntoMarker = false;
        }
        lastZoomWasIntoMarker = true;
        if (map != null && mapInitializationComplete) {
            var theMarker = row.data('marker');
            if (theMarker != null && theMarker.GMarker != null) {
                if (netEntrances == 1 && zoomOutComplete)
                    zoomOutComplete = false;
                hiliteMarker(theMarker.GMarker);
                map.setZoom(Math.max(Math.min(17, outZoomLevel + 1), 14));
                var newCenter = theMarker.GMarker.getLatLng();
                map.setCenter(newCenter);
                // var infoHtml = 'test text';
                // map.openInfoWindowHtml(newCenter, infoHtml,
                // {
                // onOpenFn: function() {
                // var iw = map.getInfoWindow();
                // if (iw != null)
                // iw.reset(iw.getPoint(), iw.getTabs(), new GSize(60, 25), null, null);
                // }
                // }
                // );
                markerCluster.resetViewport();
                //theMarker.GMarker.setImage("http://www.google.com/intl/en_us/mapfiles/ms/micons/green-dot.png");
                //rememberMarker = theMarker;
                preserveZoomInfo();
            }
        }
    }

    var mouseOverMapMode = false;
    function mouseOverMap() {
        // we might have been just hovering on a row, but now we're in map -- don't zoom out yet
        mouseOverMapMode = true;
    }

    function mouseOutOfMap() {
        if (mouseOverMapMode) {
            mouseOverMapMode = false;
            $(this).everyTime(2000, "completeZoomOut", completeZoomOutOfMarker);
        }
    }

    function zoomOutOfMarker() {
        netEntrances--;
        lastZoomWasIntoMarker = false;
        if (netEntrances < 0) // shouldn't happen if we get all events, but can happen anyway, it seems
            netEntrances = 0;
        $(this).everyTime(2000, "completeZoomOut", completeZoomOutOfMarker);
    }

    function completeZoomOutOfMarker() {
        var userMoved = checkUserHasMovedMap(true); // if user has moved, we won't zoom out -- but we won't permanently change location either
        if (netEntrances <= 0 && outZoomLevel != null && outCenter != null && !mouseOverMapMode) { /* if we are already zoomed in again on another row, we don't want to zoom out in the interim */
            zoomOutComplete = true;
            $(this).stopTime("completeZoomOut");
            if (repositionAfterZoomingOut) {
                if (userMoved) {
                    var tempZoomLevel = map.getZoom();
                    var tempCenter = map.getCenter();
                    repositionMap();
                    map.setZoom(tempZoomLevel);
                    map.setCenter(tempCenter);
                }
                else
                    repositionMap();
            }
            else {
                if (!userMoved) {
                    map.setZoom(outZoomLevel);
                    map.setCenter(outCenter);
                }
            }
            unhiliteMarker();
            markerCluster.resetViewport();
        }
    }

    function addMarkerToList(marker) {
        if (!markerIsInListForRowNum(marker.RowNum)) {
            for (var i = 0; i < markers.length; i++) {
                if (markers[i] == null)
                    break;
            }
            markers[i] = marker;
        }
    }


    function removeMarkerFromList(row) {
        var rowNum = row.getRowNum();
        removeMarkerFromListByRowNum(rowNum);
    }

    function removeMarkerFromListByRowNum(rowNum) {
        for (var i = 0; i < markers.length; i++) {
            if (markers[i] != null && markers[i].RowNum == rowNum) {
                markers[i].Remove = true;
                return;
            }
        }
    }

    function removeAllMarkers() {
        for (var i = 0; i < markers.length; i++) {
            if (markers[i] != null && markers[i].Row != null) {
                markers[i] = null;
            }
        }
        if (markerCluster != null) {
            markerCluster.clearMarkers();
            map.clearOverlays();
            mcOptions = { gridSize: 40, maxZoom: 13 };
            markerCluster = new MarkerClusterer(map, [], mcOptions);
        }
    }

    function updateMarkers() {
        if (map == null && markers.length > 0) {
            initGoogleMapStep1();
        }
        else {
            for (var i = 0; i < markers.length; i++) {
                var marker = markers[i];
                if (marker != null) {
                    if (marker.Add) {
                        if (marker.Lat == "0" || marker.Lat == "" || marker.Lat == 0)
                            geocodeBeforeAdding(marker); /* we'll addMarkerToMap once we call updateMarkers again */
                        else
                            addMarkerToMap(marker);
                        marker.Add = false;
                    }
                    else if (marker.Remove) {
                        removeMarkerFromMap(marker);
                        markers[i] = null;
                    }
                }
            }
            completeAddMarkersToMap();
            repositionMap();
        }
    }

    var theMarkersToAddOnComplete = null;
    function addMarkerToMap(marker) {
        if (theMarkersToAddOnComplete == null)
            theMarkersToAddOnComplete = new Array();
        var point = new GLatLng(marker.Lat, marker.Lng);
        marker.GMarker = new GMarker(point);
        GEvent.addListener(marker.GMarker, "mouseover", mouseOverMarker);
        GEvent.addListener(marker.GMarker, "mouseout", mouseOutMarker);
        var existingRowPopup = $('div.loadrowpopup', marker.Row).eq(0);
        if (existingRowPopup != null) {
            var newPopup = '<div rel="' + existingRowPopup.attr("rel") + '" title="' + existingRowPopup.attr("title") + '" class="loadmappopup" />';
            existingRowPopup.after(newPopup);
            marker.Popup = $(".loadmappopup", marker.Row).eq(0);
        }
        Array.add(theMarkersToAddOnComplete, marker.GMarker);
        markerCluster.addMarker(marker.GMarker);
    }

    var mapInitializationComplete = false;
    function completeAddMarkersToMap() {
        if (markerCluster != null && theMarkersToAddOnComplete != null) {
            markerCluster.addMarkers(theMarkersToAddOnComplete);
            markerCluster.resetViewport();
            theMarkersToAddOnComplete = null;
            mapInitializationComplete = true;
        }
    }

    function removeMarkerFromMap(marker) {
        if (marker.GMarker != null && markerCluster != null) {
            // GEvent.addListener(marker.GMarker, "click", function () { });
            // GEvent.addListener(marker.GMarker, "mouseover", function () { });
            // GEvent.addListener(marker.GMarker, "mouseout", function () { });
            //  this doesn't work markerCluster.removeMarker(marker.GMarker);
        }
    }

    function getMarkerFromGLatLng(gLatLng) {
        for (var i = 0; i < markers.length; i++) {
            if (markers[i] != null && markers[i].GMarker != null) {
                var markerGLatLng = markers[i].GMarker.getLatLng();
                if (markerGLatLng == gLatLng) {
                    return markers[i];
                }
            }
        }
        return null;
    }

    function mouseOverMarker(gLatLng) {
        var marker = getMarkerFromGLatLng(gLatLng);
        if (marker != null) {
            if (marker.Row != null) {
                if (marker.Popup != null && (marker.Popping == null || !marker.Popping)) {
                    mainTableBody.children($("tr.hilited")).removeClass("hilited");
                    marker.Row.addClass("hilited");
                    mainTableScrollArea.scrollTo(marker.Row);
                    if (marker.Popup.data("liveCluetipSetup"))
                        marker.Popup.trigger('showCluetip');
                    else
                        marker.Popup
     .data("liveCluetipSetup", true)
     .cluetip({ local: true, clickThrough: true, cursor: 'pointer', arrows: true })
     .trigger('showCluetip');
                    marker.Popping = true;
                }
            }
        }
    }

    function mouseOutMarker(gLatLng) {
        var marker = getMarkerFromGLatLng(gLatLng);
        if (marker != null) {
            if (marker.Row != null)
                marker.Row.trigger('mouseout');
            $(document).trigger('hideCluetip');
            marker.Popping = false;
        }
    }



    function markerInfo(lat, lng, address, row, add, remove) {
        var theMarkerInfo = new Object();
        theMarkerInfo.Lat = lat;
        theMarkerInfo.Lng = lng;
        theMarkerInfo.Address = address;
        theMarkerInfo.Row = row;
        theMarkerInfo.RowNum = row.getRowNum();
        theMarkerInfo.Add = add;
        theMarkerInfo.Remove = remove;
        return theMarkerInfo;
    }

    /*** my points sidebar ***/
    function initializeMyPointsSidebar() {
        myPointsSidebar = $("#MyPointsSidebarDiv");
        $("UpdateMyPoints", myPointsSidebar).click(updateMyPointsSidebar);
    }

    function updateMyPointsSidebar() {
        pointsManagerID = myPointsSidebar.attr("data-PMID");
        PMWebServices.PMWebService.GetSidebarInfo(pointsManagerID, getUserAccessInfo(), Function.createCallback(updateMyPointsSidebarComplete, null));
    }

    function updateMyPointsSidebarComplete(result) {
        if (result == null)
            return;
        $("#CurrentPeriod", myPointsSidebar).html(result.CurrentPeriod);
        $("#CurrentPrizeInfo", myPointsSidebar).html(result.CurrentPrizeInfo);
        $("#CurrentInfo", myPointsSidebar).html(result.CurrentInfo);
        $("#PointsThisPeriod", myPointsSidebar).html(result.PointsThisPeriod);
        $("#PendingPointsThisPeriod", myPointsSidebar).html(result.PendingPointsThisPeriod);
        $("#ScoredRatings", myPointsSidebar).html(result.ScoredRatings);
        $("#PointsPerRating", myPointsSidebar).html(result.PointsPerRating);
    }


    /*** table info serialization and deserialization ***/


    function getTableInfoObject() {
        return $.evalJSON(tableInfo.html());
    }

    function replaceTableInfo(theTableInfoObject) {
        var newTableInfo = $.toJSON(theTableInfoObject);
        newTableInfo = convertToMSDateFormat(newTableInfo);
        tableInfo.html(newTableInfo);
    }

    function convertToMSDateFormat(stringIncludingDates) {
        return stringIncludingDates.replace(new RegExp("/Date\\((-?[0-9]*)\\)/", "g"), "\\/Date\($1\)\\/"); /* correctly format date -- json dates are nonstandard */
    }

    /*** trace for debugging ***/

    function trace(s) {
        if (this.console && typeof console.log != "undefined")
            console.log(s);
        /* else
        $("#spaceAtVeryBottomOfPage").html(s); */
    }
}

var myDoViewTbl = new viewtbl();
