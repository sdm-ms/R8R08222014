﻿using ClassLibrary1.Misc;
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
            // We ideally don't want to do anything here. Since we're adding missing ratings in many web roles, adding fast access information here risks change conflicts. We don't get change conflicts if we only add the fast access information in the background processes, since those happen seriatim.
            // OnHighStakesKnownChanging(this.HighStakesKnown); // Since we've now made this ValueIsRelative = true, we only need to note changes in this, and it will always start at its default value of 0.
            //  OnValueRecentlyChangedChanging(this.ValueRecentlyChanged); // This will automatically be set once we actually set the RatingGroup to a value.
        }

        partial void OnHighStakesKnownChanging(bool value)
        {
            if (!RatingGroupTypesList.lowerHierarchy.Contains(this.TypeOfRatingGroup)) // i.e., if this is a top rating group
            {
                var fa2 = new FastAccessHighStakesKnownUpdateInfo() { HighStakesKnownChange = value ? 1 : -1}; // we keep track of the number of high stakes known per row, so this can go up or down
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

    public partial class AddressField
    {
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if ((action == System.Data.Linq.ChangeAction.Insert  || action == System.Data.Linq.ChangeAction.Update) && this.Field.TblRow != null)
                UpdateFastAccess();
        }

        public void UpdateFastAccess()
        {
            // NOTE: Right now, we are just putting in the GeoInfo because this is solely for filtering. Once we want to put the content in because we are creating computed columns on TblRowFieldDisplays, we'll need an extra column for the textual Address.
            if (Status == (byte)StatusOfObject.AboutToBeReplaced)
            { // no change to be made; we'll rely on the new field instead
                Status = (byte)StatusOfObject.Unavailable;
                return;
            }
            SQLGeographyInfo geoInfo = null;
            if (Status == (byte)StatusOfObject.Active && Latitude != 0 && Longitude != 0 && Latitude != null && Longitude != null)
                geoInfo = new SQLGeographyInfo() { Latitude = (decimal) Latitude, Longitude = (decimal) Longitude };
            var updater = new FastAccessAddressFieldUpdateInfo() { FieldDefinitionID = this.Field.FieldDefinitionID, GeoInfo = geoInfo };

            updater.AddToTblRow(this.Field.TblRow);
        }
    }

    public partial class NumberField
    {
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if ((action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update) && this.Field.TblRow != null)
                UpdateFastAccess();
        }

        public void UpdateFastAccess()
        {
            if (Status == (byte)StatusOfObject.AboutToBeReplaced)
            { // no change to be made; we'll rely on the new field instead
                Status = (byte)StatusOfObject.Unavailable;
                return;
            }
            decimal? number = null;
            if (Status == (byte)StatusOfObject.Active)
                number = Number;
            var updater = new FastAccessNumberFieldUpdateInfo() { FieldDefinitionID = this.Field.FieldDefinitionID, Number = number };
            updater.AddToTblRow(this.Field.TblRow);
        }
    }

    public partial class TextField
    {
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if ((action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update) && this.Field.TblRow != null)
                UpdateFastAccess();
        }

        public void UpdateFastAccess()
        {
            if (Status == (byte)StatusOfObject.AboutToBeReplaced)
            { // no change to be made; we'll rely on the new field instead
                Status = (byte)StatusOfObject.Unavailable;
                return;
            }
            string text = null;
            if (Status == (byte)StatusOfObject.Active)
                text = Text; // NOTE: Right now, we are just putting in the Text because this is solely for filtering. Once we want to put the content in because we are creating computed columns on TblRowFieldDisplays, we'll need an extra column for the Link.
            var updater = new FastAccessTextFieldUpdateInfo() { FieldDefinitionID = this.Field.FieldDefinitionID, Text = text };
            updater.AddToTblRow(this.Field.TblRow);
        }
    }

    public partial class DateTimeField
    {
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if ((action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update) && this.Field.TblRow != null)
                UpdateFastAccess();
        }

        public void UpdateFastAccess()
        {if (Status == (byte)StatusOfObject.AboutToBeReplaced)
            { // no change to be made; we'll rely on the new field instead
                Status = (byte)StatusOfObject.Unavailable;
                return;
            }
            DateTime? dateTime = null;
            if (Status == (byte)StatusOfObject.Active)
                dateTime = DateTime; 
            var updater = new FastAccessDateTimeFieldUpdateInfo() { FieldDefinitionID = this.Field.FieldDefinitionID, DateTimeInfo = dateTime };
            updater.AddToTblRow(this.Field.TblRow);
        }
    }

    public partial class ChoiceInField
    {
        partial void OnValidate(System.Data.Linq.ChangeAction action)
        {
            if ((action == System.Data.Linq.ChangeAction.Insert || action == System.Data.Linq.ChangeAction.Update) && this.ChoiceField.Field.TblRow != null)
            {
                if (this.ChoiceField.Field.FieldDefinition.ChoiceGroupFieldDefinitions.First().ChoiceGroup.AllowMultipleSelections)
                    UpdateFastAccessMultipleSelections();
                else
                    UpdateFastAccessSingleSelection();
            }
        }
        
        // For us to use this for the TblRowFieldDisplays, we're going to need to add a column for the textual version of the information.


        public void UpdateFastAccessSingleSelection()
        {
            if (Status == (byte)StatusOfObject.AboutToBeReplaced)
            { // no change to be made; we'll rely on the new field instead
                Status = (byte)StatusOfObject.Unavailable;
                return;
            }
            int? choice = null;
            if (Status == (byte)StatusOfObject.Active)
                choice = ChoiceInGroupID;
            var updater = new FastAccessChoiceFieldSingleSelectionUpdateInfo() { FieldDefinitionID = this.ChoiceField.Field.FieldDefinitionID, ChoiceInGroupID = choice };
            updater.AddToTblRow(this.ChoiceField.Field.TblRow);
        }

        public void UpdateFastAccessMultipleSelections()
        {
            // NOTE: We do not actually make these changes here. See FieldChange in ActionProcessor. The problem is that some ChoiceInFields are deleted and then readded, but we can't be sure whether something that has status AboutToBeReplaced 
            if (Status == (byte)StatusOfObject.AboutToBeReplaced)
                Status = (byte)StatusOfObject.Unavailable; 
        }
    }
}