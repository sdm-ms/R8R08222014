using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Model
{
    public partial class TblRow
    {
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {

            if (action == System.Data.Linq.ChangeAction.Insert) // this.FastAccessInitialCopy && this.FastAccessUpdated == null)
                AddFastAccessForNewData();
        }

        public void AddFastAccessForNewData()
        {
            OnElevateOnMostNeedsRatingChanging(this.ElevateOnMostNeedsRating);
            OnCountNonnullEntriesChanging(this.CountNonnullEntries);
            OnCountUserPointsChanging(this.CountUserPoints);
            OnNameChanging(this.Name);
            OnStatusChanging(this.Status);
        }

        partial void OnElevateOnMostNeedsRatingChanging(bool value)
        {
            var fa = new FastAccessElevateOnMostNeedsRatingUpdateInfo() { ElevateOnMostNeedsRating = value };
            fa.AddToTblRow(this);
        }

        partial void OnCountNonnullEntriesChanging(int value)
        {
            var facnnei = new FastAccessCountNonNullEntriesUpdateInfo() { CountNonNullEntries = value };
            facnnei.AddToTblRow(this);
        }

        partial void OnCountUserPointsChanging(decimal value)
        {
            var facup = new FastAccessCountUserPointsUpdateInfo() { CountUserPoints = value };
            facup.AddToTblRow(this);
        }

        partial void OnNameChanging(string value)
        {
            var faname = new FastAccessTblRowNameUpdateInfo() { Name = value };
            faname.AddToTblRow(this);
        }

        partial void OnStatusChanging(byte value)
        {
            var fadel = new FastAccessDeletedUpdateInfo() { Deleted = value == (byte)StatusOfObject.Unavailable };
            fadel.AddToTblRow(this);
        }

    }

    public partial class RatingGroup
    {
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if (action == System.Data.Linq.ChangeAction.Insert && this.TblRow != null)
                AddFastAccessForNewData();
        }

        public void AddFastAccessForNewData()
        {
            OnHighStakesKnownChanging(this.HighStakesKnown);
            OnValueRecentlyChangedChanging(this.ValueRecentlyChanged);
        }

        partial void OnHighStakesKnownChanging(bool value)
        {
            if (!RatingGroupTypesList.lowerHierarchy.Contains(this.TypeOfRatingGroup)) // i.e., if this is a top rating group
            {
                var fa2 = new FastAccessHighStakesKnownUpdateInfo() { HighStakesKnown = value };
                fa2.AddToTblRow(this.TblRow);
            }
        }

        partial void OnValueRecentlyChangedChanging(bool value)
        {
            var farui = new FastAccessRecentlyChangedInfo()
            {
                TblColumnID = this.TblColumnID,
                RecentlyChanged = value,
            };
            farui.AddToTblRow(this.TblRow);
        }
    }
}
