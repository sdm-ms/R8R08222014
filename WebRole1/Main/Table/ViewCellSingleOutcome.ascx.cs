using System;
using System.Linq;
using System.Collections.Generic;
using ClassLibrary1.Model;

public partial class Main_Table_ViewCellSingleOutcome : System.Web.UI.UserControl
{

    public void Setup(RaterooDataAccess dataAccess, int ratingGroupID, TradingStatus theTradingStatus, bool canPredict, bool selected, string suppStyle)
    {
        Rating theRating = dataAccess.RaterooDB.GetTable<Rating>().SingleOrDefault(m => m.RatingGroupID == ratingGroupID);
        if (theRating == null)
            return;

        RatingValueCell.Setup(dataAccess,  theRating.RatingID, theRating.CurrentValue, theRating.RatingCharacteristic.DecimalPlaces, theRating.RatingCharacteristic.MinimumUserRating, theRating.RatingCharacteristic.MaximumUserRating, theRating.RatingGroup.TypeOfRatingGroup == (Byte) RatingGroupTypes.singleDate, "", theRating.RatingGroup.TblColumnID, theTradingStatus, canPredict, selected, suppStyle);
    }

    public List<RatingIdAndUserRatingValue> GetRatingsAndUserRatings()
    {
        List<RatingIdAndUserRatingValue> theList = new List<RatingIdAndUserRatingValue>();
        int thisRatingID = 0;
        decimal? thisUserRating = 0;
        RatingValueCell.GetRatingAndProposedValue(ref thisRatingID, ref thisUserRating);
        if (thisUserRating != null)
            theList.Add(new RatingIdAndUserRatingValue
            {
                RatingID = thisRatingID,
                UserRatingValue = (decimal)thisUserRating
            }
            );

        return theList;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
