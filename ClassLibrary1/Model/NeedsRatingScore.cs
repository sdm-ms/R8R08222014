using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public static class NeedsRatingScore
    {
        public static void SetNeedsRatingScoreFields(IR8RDataContext dataContext, IQueryable<TblRow> rows)
        {
            int[] excludedRatingGroupTypes = { (int) RatingGroupTypes.hierarchyNumbersBelow, (int) RatingGroupTypes.probabilityHierarchyBelow, (int) RatingGroupTypes.probabilityMultipleOutcomesHiddenHierarchy };
            DateTime now = TestableDateTime.Now;
            var rowsInfo = rows.Select(x => new
            {
                Row = x,
                HighStakesKnownCount = x.RatingGroups
                                    .Where(y => !excludedRatingGroupTypes.Contains((int)y.TypeOfRatingGroup))
                                    .Where(y => y.TblColumn.Status == (int)StatusOfObject.Active)
                                    .Count(y => y.HighStakesKnown && y.RatingGroupPhaseStatus.OrderByDescending(z => z.ActualCompleteTime).First().HighStakesBecomeKnown < now),
                NonNullCount = x.RatingGroups
                                    .Where(y => !excludedRatingGroupTypes.Contains((int)y.TypeOfRatingGroup))
                                    .Where(y => y.TblColumn.Status == (int) StatusOfObject.Active)
                                    .Count(y => y.CurrentValueOfFirstRating != null),
                UserRaterPointsTotals = x.RatingGroups
                                    .Where(y => !excludedRatingGroupTypes.Contains((int)y.TypeOfRatingGroup))
                                    .Where(y => y.TblColumn.Status == (int)StatusOfObject.Active)
                                    .Select(y => y.UserRatingGroups.OrderByDescending(w => w.WhenMade).First())
                                    .SelectMany(z => z.UserRatings.First().User.PointsTotals.Where(t => t.PointsManagerID == x.Tbl.PointsManagerID)) /* This is the points totals for ALL of the top ratings for the row */
            });
            foreach (var rowInfo in rowsInfo)
            {
                rowInfo.Row.CountHighStakesNow = rowInfo.HighStakesKnownCount;
                bool prevValue = rowInfo.Row.ElevateOnMostNeedsRating;
                rowInfo.Row.ElevateOnMostNeedsRating = rowInfo.Row.CountHighStakesNow > 0;
                if (!prevValue && rowInfo.Row.ElevateOnMostNeedsRating)
                    rowInfo.Row.Tbl.PointsManager.HighStakesNoviceNumActive++;
                else if (prevValue && !rowInfo.Row.ElevateOnMostNeedsRating)
                    rowInfo.Row.Tbl.PointsManager.HighStakesNoviceNumActive--;
                rowInfo.Row.CountNonnullEntries = rowInfo.NonNullCount;
                rowInfo.Row.CountUserPoints = 0;
                foreach (var urpt in rowInfo.UserRaterPointsTotals.ToList())
                    rowInfo.Row.CountUserPoints += (decimal) urpt.PointsPerRating;
            }
            dataContext.SubmitChanges();
        }

        public static void SetCountUserPoints(IR8RDataContext dataContext, TblRow row, User formerUser, User currentUser, bool multipleRatingsInRatingGroup)
        {
            decimal formerPointsPerRating = 0;
            decimal currentPointsPerRating;
            if (formerUser != null)
            {
                PointsTotal former = formerUser.PointsTotals.SingleOrDefault(x => x.PointsManagerID == row.Tbl.PointsManagerID);
                formerPointsPerRating = former == null ? 0 : former.PointsPerRating; 
            }
            PointsTotal current = currentUser.PointsTotals.SingleOrDefault(x => x.PointsManagerID == row.Tbl.PointsManagerID);
            currentPointsPerRating = current == null ? 0 : current.PointsPerRating;
            decimal increase = currentPointsPerRating - formerPointsPerRating;
            row.CountUserPoints += increase;
            if (multipleRatingsInRatingGroup)
                throw new NotImplementedException(); // must copy to fast access once implementing this
        }
    }
}
