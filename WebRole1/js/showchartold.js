    $('[id$=BtnShowChart]').bind('click', showChart).bind('mousedown', hiliteImage).bind('mouseup', unhiliteImage).bind('mouseout',unhiliteImage);

var previousChartDisplayed = null;
//    <div id="cluetipchartdiv" visible="true">
//        <a id="cluetipanchor2" href="#chartjframe" rel="#chartjframe" />
//        <div id="chartjframe" />
//        <p id="temp" visible="true">copied</p>
//    </div>

//function showChart() {
//    /* Problems: We need to set the href, rel dynamically to the appropriate ratinggroupid */
//    /* We need to put the whole thing in a jframe or something that will allow interaction */
//        $("a.cluetiptemp2").remove();
//        $("#cluetipanchor2").clone().appendTo(theSelectedCell).addClass("cluetiptemp2").show();     
//        var cluetiplaunchanchor = null;   
//        cluetiplaunchanchor = $('a.cluetiptemp2');
//        cluetiplaunchanchor.cluetip( {
//                    activation: 'click',  
//                    closePosition: 'title',
//                    closeText: '<img src="../../images/cross.png" alt="" />',
//                    delayedClose: 20000,
//                    width: 450,
//                    sticky: true
//                }
//                ); 
//        cluetiplaunchanchor.trigger('click');
//    
//}

function pausecomp(millis)
{
var date = new Date();
var curDate = null;

do { curDate = new Date(); }
while(curDate-date < millis);
} 

function showChart() {
    showChartTemp();
}

function showChartTemp() {
    /* Problems: We need to set the href, rel dynamically to the appropriate ratinggroupid */
    /* We need to put the whole thing in a jframe or something that will allow interaction */
//       $(".cluetiptemp2").remove();
//        var test = $("#cluetipchartdiv");
        if (previousChartDisplayed != null)
            previousChartDisplayed.remove();
        var theClueTipContents = $("#cluetipchartdiv")
            .contents().andSelf();
        previousChartDisplayed = theClueTipContents.clone();
        previousChartDisplayed.appendTo(theSelectedCell)
            .addClass("cluetiptemp2")
            .show();
        var chartjframe = $("#chartjframe", previousChartDisplayed);
        chartjframe.loadJFrame(
            "../Table/RatingOverTimeGraph.aspx?RatingGroupID=18538",
            function(){}
            );  
//        var cluetipmessageanchor = null;   
//        cluetipmessageanchor = $('a.cluetiptemp2');
//        cluetipmessageanchor.cluetip( {
//            positionBy: 'fixed',
//            leftOffset: 5,
//            topOffset: -20,
//            width: 450,
//            activation: 'click',  
//            closePosition: 'title',
//            closeText: '<img src="../../images/cross.png" alt="" />',
//            local: true,
//            sticky: true
//        }
//        ); 
//        cluetipmessageanchor.trigger('click');
//    
//    pausecomp(5000);
    //theClone.remove();
}

//function reportMessage3() {
//        $("a.cluetiptemp2").remove();
//        $("#chartjframe").clone().appendTo(theSelectedCell).addClass("cluetiptemp2").show();
//        var cluetipmessageanchor = null;   
//        cluetipmessageanchor = $('a.cluetiptemp2');
//        cluetipmessageanchor.cluetip( {
//            positionBy: 'fixed',
//            leftOffset: 5,
//            topOffset: -20,
//            width: 450,
//            activation: 'click',  
//            closePosition: 'title',
//            closeText: '<img src="../../images/cross.png" alt="" />',
//            local: true,
//            sticky: true
//        }
//        ); 
//        cluetipmessageanchor.trigger('click');
//    
//}

function reportMessage2(theTitle, theMessage) {
    if (theMessage != null && theMessage != "")
    {    
        $("a.cluetiptemp2").remove();
        $("#cluetipanchor2").clone().appendTo(theSelectedCell).addClass("cluetiptemp2").show();     
        var cluetipmessageanchor = null;   
        cluetipmessageanchor = $('a.cluetiptemp2');
        cluetipmessageanchor.attr(
            { title: theTitle }
            ); 
        $('[id$=messagetext2]')
            .text( theMessage);    
        cluetipmessageanchor.cluetip( {
                    activation: 'click',  
                    closePosition: 'title',
                    closeText: '<img src="../../images/cross.png" alt="" />',
                    delayedClose: 20000,
                    local: true,
                    sticky: true
                }
                ); 

        cluetipmessageanchor.trigger('click');
    }
    
}