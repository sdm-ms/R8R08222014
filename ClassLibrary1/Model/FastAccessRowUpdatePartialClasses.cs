using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Model
{
    public partial class TblRow
    {
        partial void OnElevateOnMostNeedsRatingChanging(bool value)
        {
            var fa = new FastAccessElevateOnMostNeedsRatingUpdateInfo() { ElevateOnMostNeedsRating = value };
            fa.AddToTblRow(this);
        }


    }

    public partial class RatingGroup
    {
        partial void OnHighStakesKnownChanging(bool value)
        {
            if (!RatingGroupTypesList.lowerHierarchy.Contains(this.TypeOfRatingGroup)) // i.e., if this is a top rating group
            {
                var fa2 = new FastAccessHighStakesKnownUpdateInfo() { HighStakesKnown = false };
                fa2.AddToTblRow(this.TblRow);
            }
        }
    }
}
