using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using ClassLibrary1.Model;



public partial class Main_Table_ViewCellMainUnselected : System.Web.UI.UserControl
{

    public void Setup(R8RDataAccess dataAccess, Action<int> selectFn, int columnNumber, int TblColumnID, int ratingGroupID, bool isInSortedColumn, bool multipleOutcomes, TradingStatus theTradingStatus, bool canPredict, bool doRebind, string suppStyle)
    {
        string theCSSClass = "mainCellMarker";
        if (theTradingStatus == TradingStatus.Ended)
            theCSSClass += " deletedTblRow ";
        theCSSClass += " " + NumberandTableFormatter.GetSuppStyleMain(TblColumnID);
        tdTag.AddAttribute("class", theCSSClass);
        tdTag.RenderNow();
        tdTagClose.RenderNow();
        grpC.Value = ratingGroupID.ToString();
        CellData.Setup(dataAccess, selectFn, columnNumber, TblColumnID, ratingGroupID, multipleOutcomes, theTradingStatus, canPredict, false, doRebind, suppStyle);
    }

    //UserRatingDataAccess DataAccess;
    //Action<int> SelectFn;
    //int ColumnNumber;
    //int RatingGroupID; 
    //bool IsInSortedColumn; 
    //bool MultipleOutcomes; 
    //TradingStatus TheTradingStatus;
    //bool CanPredict;
    //bool DoRebind;

    //public Main_Table_ViewCellMainUnselected(ViewCellMainUnselectedConstructorParameters theParameters) : base()
    //{
    //    DataAccess = theParameters.dataAccess;
    //    SelectFn = theParameters.selectFn;
    //    ColumnNumber = theParameters.columnNumber;
    //    RatingGroupID = theParameters.ratingGroupID;
    //    IsInSortedColumn = theParameters.isInSortedColumn;
    //    MultipleOutcomes = theParameters.multipleOutcomes;
    //    TheTradingStatus = theParameters.theTradingStatus;
    //    CanPredict = theParameters.canPredict;
    //    DoRebind = theParameters.doRebind;
    //}

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    CellData.Setup(DataAccess, SelectFn, ColumnNumber, RatingGroupID, MultipleOutcomes, TheTradingStatus, CanPredict, false, DoRebind);

    //}

 
  
   
}
    
