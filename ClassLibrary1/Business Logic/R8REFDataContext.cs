
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClassLibrary1.Nonmodel_Code;
using System.Linq;
using System.Data.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;
using ClassLibrary1.EFModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Common;

namespace ClassLibrary1.Model
{
    public class R8REFDataContext : EFDataContext, IR8RDataContext
    {
        R8RContext _underlyingR8RContext;

        public R8REFDataContext() : base(new R8RContext(ConnectionString.GetR8RNormalizedDatabaseConnectionString()))
        {
            _underlyingR8RContext = (R8RContext)UnderlyingDbContext;
        }

        public R8REFDataContext(string connectionString)
            : base(new R8RContext(connectionString))
        {
            _underlyingR8RContext = (R8RContext)UnderlyingDbContext;
        }

        bool isReal = true;
        public R8REFDataContext(DbConnection dbConnection)
            : base(new R8RContext(dbConnection))
        {
            isReal = false; // Use this only with Effort to mock the in-memory database
            _underlyingR8RContext = (R8RContext)UnderlyingDbContext;
        }

        public bool IsRealDatabase()
        {
            return isReal;
        }

        public IR8RDataContext GetRealDatabaseIfExists()
        {
            return this;
        }

        internal DictionaryByType _RegisteredToBeInserted = new DictionaryByType();
        public DictionaryByType RegisteredToBeInserted
        {
            get
            {
                return _RegisteredToBeInserted;
            }
            set
            {
                _RegisteredToBeInserted = value;
            }
        }

        internal Dictionary<string, object> _TempCache;
        public Dictionary<string, object> TempCache
        {
            get
            {
                if (_TempCache == null)
                    _TempCache = new Dictionary<string, object>();
                return _TempCache;
            }
            set
            {
                _TempCache = value;
            }
        }


        public bool ResolveConflictsIfPossible()
        {
            return true;
        }

        public override void BeforeSubmitChanges()
        {
            //foreach (ObjectStateEntry entry in
            //    GetObjectContext().ObjectStateManager.GetObjectStateEntries(
            //    EntityState.Added)) // | EntityState.Modified))
            foreach (DbEntityEntry<TblRow> entry in UnderlyingDbContext.ChangeTracker.Entries<TblRow>())
            {
                TblRow tblRow = entry.Entity as TblRow;
                if (entry.State == EntityState.Added)
                {
                    tblRow.NotYetAddedToDatabase = false;
                    tblRow.AddFastAccessForNewData();
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Property(p => p.ElevateOnMostNeedsRating).IsModified)
                        tblRow.OnElevateOnMostNeedsRatingChanging(tblRow.ElevateOnMostNeedsRating);
                    else if (entry.Property(p => p.CountNonnullEntries).IsModified)
                        tblRow.OnCountNonnullEntriesChanging(tblRow.CountNonnullEntries);
                    else if (entry.Property(p => p.CountUserPoints).IsModified)
                        tblRow.OnCountUserPointsChanging(tblRow.CountUserPoints);
                    else if (entry.Property(p => p.Name).IsModified)
                        tblRow.OnNameChanging(tblRow.Name);
                    else if (entry.Property(p => p.Status).IsModified)
                        tblRow.OnStatusChanging(tblRow.Status);
                }
            }
            foreach (DbEntityEntry<TblColumn> entry in UnderlyingDbContext.ChangeTracker.Entries<TblColumn>())
            {
                TblColumn tblColumn = entry.Entity as TblColumn;
                tblColumn.NotYetAddedToDatabase = false;
            }
            foreach (DbEntityEntry<Field> entry in UnderlyingDbContext.ChangeTracker.Entries<Field>())
            {
                Field field = entry.Entity as Field;
                field.NotYetAddedToDatabase = false;
            }
            foreach (DbEntityEntry<RatingGroup> entry in UnderlyingDbContext.ChangeTracker.Entries<RatingGroup>())
            {
                RatingGroup ratingGroup = entry.Entity as RatingGroup;

                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    if (entry.Property(p => p.HighStakesKnown).IsModified)
                        ratingGroup.OnHighStakesKnownChanging(ratingGroup.HighStakesKnown);
                    if (entry.Property(p => p.ValueRecentlyChanged).IsModified)
                        ratingGroup.OnValueRecentlyChangedChanging(ratingGroup.ValueRecentlyChanged);
                }
            }

            foreach (DbEntityEntry<AddressField> entry in UnderlyingDbContext.ChangeTracker.Entries<AddressField>())
            {
                AddressField addressField = entry.Entity as AddressField;
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    addressField.UpdateFastAccess();
                }
            }
            foreach (DbEntityEntry<TextField> entry in UnderlyingDbContext.ChangeTracker.Entries<TextField>())
            {
                TextField textField = entry.Entity as TextField;
                
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    textField.UpdateFastAccess();
                }
            }
            foreach (DbEntityEntry<NumberField> entry in UnderlyingDbContext.ChangeTracker.Entries<NumberField>())
            {
                NumberField numberField = entry.Entity as NumberField;
                
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    numberField.UpdateFastAccess();
                }
            }
            foreach (DbEntityEntry<DateTimeField> entry in UnderlyingDbContext.ChangeTracker.Entries<DateTimeField>())
            {
                DateTimeField dateTimeField = entry.Entity as DateTimeField;
                
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    dateTimeField.UpdateFastAccess();
                }
            }
            foreach (DbEntityEntry<ChoiceInField> entry in UnderlyingDbContext.ChangeTracker.Entries<ChoiceInField>())
            {
                ChoiceInField choiceInField = entry.Entity as ChoiceInField;
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    if (choiceInField.ChoiceField.Field.FieldDefinition.ChoiceGroupFieldDefinitions.FirstOrDefault().ChoiceGroup.AllowMultipleSelections)
                        choiceInField.UpdateFastAccessMultipleSelections();
                    else
                        choiceInField.UpdateFastAccessSingleSelection();
                }
            }

            this.PreSubmittingChangesTasks(); // We must do this to ensure that we really do want to submit changes.
            base.BeforeSubmitChanges();
        }

        public System.Linq.IQueryable<AddressField> UDFDistanceWithin(float? latitude, float? longitude, float? distance)
        {
            throw new System.NotImplementedException();
        }
    }
}
