using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq.Expressions;
using ClassLibrary1.Model;

/// <summary>
/// Summary description for Fields
/// </summary>
/// 

namespace ClassLibrary1.Model
{

    /// <summary>
    /// FieldTypes
    /// Enumerates the different types of fields that can be used to describe an entity.
    /// </summary>
    public enum FieldTypes
    {
        TextField,
        NumberField,
        ChoiceField,
        AddressField,
        DateTimeField
    };


    public enum FieldsLocation
    {
        RowHeading,
        RowPopup,
        TblRowPage
    };

    /// <summary>
    /// FieldsDisplaySettingsMask
    /// This is used to test and set bits in the integer display settings variable
    /// First, create myMask = new FieldsDisplaySettingsMask(). Then test/set bits
    /// To test a bit: (myDisplaySettings & myMask.LargerFont == myMask.LargerFont)
    /// To set a bit: myDisplaySettings = myDisplaySettings | myMask.LargerFont;
    /// To clear a bit: myDisplaySettings = myDisplaySettings ^ myMask.LargerFont;
    /// </summary>
    public class FieldsDisplaySettingsMask
    {
        public int ReservedForFutureUse = 1;
        public int DisplayGoogleMapForAddress = 2;
        public int DisplayInTopRightCorner = 4;
        public int NewLineBeforeFieldValue = 8;
        public int NewLineBeforeFieldName = 16;
        public int IncludeFieldName = 32;
        public int LargerFont = 64;
        public int SmallerFont = 128;
        public int Visible = 256;

        public static int GetFieldDisplaySetting(bool ReservedForFutureUse, bool DisplayGoogleMapForAddress, bool DisplayInTopRightCorner, bool NewLineBeforeFieldValue, bool NewLineBeforeFieldName, bool IncludeFieldName, bool LargerFont, bool SmallerFont, bool Visible)
        {
            var myMask = new FieldsDisplaySettingsMask();
            int theValue = 0;
            if (ReservedForFutureUse)
                theValue = theValue | myMask.ReservedForFutureUse;
            if (DisplayGoogleMapForAddress)
                theValue = theValue | myMask.DisplayGoogleMapForAddress;
            if (DisplayInTopRightCorner)
                theValue = theValue | myMask.DisplayInTopRightCorner;
            if (NewLineBeforeFieldValue)
                theValue = theValue | myMask.NewLineBeforeFieldValue;
            if (NewLineBeforeFieldName)
                theValue = theValue | myMask.NewLineBeforeFieldName;
            if (IncludeFieldName)
                theValue = theValue | myMask.IncludeFieldName;
            if (LargerFont)
                theValue = theValue | myMask.LargerFont;
            if (SmallerFont)
                theValue = theValue | myMask.SmallerFont;
            if (Visible)
                theValue = theValue | myMask.Visible;
            return theValue;
        }

    };

    /// <summary>
    /// FieldDisplayInfo
    /// Information on a field to be displayed -- FieldsCompiledQuery returns a queryable of this
    /// </summary>
    public class FieldDisplayInfo
    {
        public int FieldID { get; set; }
        public int DisplaySettings { get; set; }
    }


    public class TblRowPlusFieldInfos
    {
        public TblRow TblRow { get; set; }
        public PointsManager PointsManager { get; set; }
        public IEnumerable<FieldDisplayInfoComplete> Fields { get; set; }
    }


    public class FieldDisplayInfoComplete
    {
        public int DisplaySettings { get; set; }
        public Field Field { get; set; }
        public FieldDefinition FieldDesc { get; set; }
        public AddressField TheAddressField { get; set; }
        public NumberField TheNumberField { get; set; }
        public TextField TheTextField { get; set; }
        public DateTimeField TheDateTimeField { get; set; }
        public ChoiceField TheChoiceField { get; set; }
        public IEnumerable<ChoiceInGroup> TheChoices { get; set; }
        public DateTimeFieldDefinition TheDateTimeFieldDesc { get; set; }
        public NumberFieldDefinition TheNumberFieldDesc { get; set; }
        public TextFieldDefinition TheTextFieldDesc { get; set; }
        public ChoiceGroupFieldDefinition TheChoiceGroupFieldDesc { get; set; }
    }

    /// <summary>
    /// FieldsCompiledQueryRequest
    /// Used internally by FieldsCompiledQuery to specify the query to compile.
    /// </summary>
    public class TblRowPlusFieldInfoLoaderRequest
    {
        public FieldsLocation TheFieldsLocation;
        public TblRow TblRow;

        public TblRowPlusFieldInfoLoaderRequest(FieldsLocation theFieldsLocation, TblRow tblRow)
        {
            TheFieldsLocation = theFieldsLocation;
            TblRow = tblRow;
        }
    }

    public class TblRowPlusFieldInfoLoader
    {
        static bool initialized = false;
        static Func<TblRowPlusFieldInfoLoaderRequest, TblRowPlusFieldInfos> theQueryForSingleTblRow;
        static Func<RaterooDataContext, IQueryable<TblRowPlusFieldInfos>>  theQueryForMultipleNeedingResetting;

        public TblRowPlusFieldInfoLoader()
        {
            if (!initialized)
            {
                DoInitialization();
                initialized = true;
            }
        }

        public void DoInitialization()
        {
            FieldsDisplaySettingsMask bitMask = new FieldsDisplaySettingsMask();
            int visibleMask = bitMask.Visible;

            theQueryForSingleTblRow = (TblRowPlusFieldInfoLoaderRequest theRequest) => new TblRowPlusFieldInfos
                {
                    TblRow = theRequest.TblRow,
                    PointsManager = theRequest.TblRow.Tbl.PointsManager,
                    Fields =
                        theRequest.TblRow.Fields
                                        .Where(f => f.TblRow == theRequest.TblRow && f.Status == (int)StatusOfObject.Active && f.FieldDefinition.Status == (int)StatusOfObject.Active && 
                        (
                        theRequest.TheFieldsLocation == FieldsLocation.TblRowPage ? ((f.FieldDefinition.DisplayInTblRowPageSettings & visibleMask) == visibleMask) :
                            (theRequest.TheFieldsLocation == FieldsLocation.RowHeading ? ((f.FieldDefinition.DisplayInTableSettings & visibleMask) == visibleMask || f.FieldDefinition.FieldType == (int)FieldTypes.AddressField) :
                            ((f.FieldDefinition.DisplayInPopupSettings & visibleMask) == visibleMask))
                        ))
                    .OrderBy(f => f.FieldDefinition.FieldNum)
                    .ThenBy(f => f.FieldDefinitionID)
                    .Select(f => new FieldDisplayInfoComplete
                    {
                        Field = f,
                        FieldDesc = f.FieldDefinition,
                        DisplaySettings = theRequest.TheFieldsLocation == FieldsLocation.TblRowPage ? f.FieldDefinition.DisplayInTblRowPageSettings  :
                            (theRequest.TheFieldsLocation == FieldsLocation.RowHeading ? f.FieldDefinition.DisplayInTableSettings  :
                            f.FieldDefinition.DisplayInPopupSettings ),
                        TheAddressField = (f.FieldDefinition.FieldType == (int)FieldTypes.AddressField) ? f.AddressFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheNumberField = (f.FieldDefinition.FieldType == (int)FieldTypes.NumberField) ? f.NumberFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheTextField = (f.FieldDefinition.FieldType == (int)FieldTypes.TextField) ? f.TextFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheDateTimeField = (f.FieldDefinition.FieldType == (int)FieldTypes.DateTimeField) ? f.DateTimeFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheChoiceField = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.ChoiceFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheChoices = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.ChoiceFields.Where(z => z.Status == (int)StatusOfObject.Active).SelectMany(y => y.ChoiceInFields).Where(z2 => z2.Status == (int)StatusOfObject.Active).Select(y => y.ChoiceInGroup) : null,
                        TheChoiceGroupFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.FieldDefinition.ChoiceGroupFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheDateTimeFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.DateTimeField) ? f.FieldDefinition.DateTimeFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheNumberFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.NumberField) ? f.FieldDefinition.NumberFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                        TheTextFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.TextField) ? f.FieldDefinition.TextFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null
                    }
                    ).ToList()
                };


            // Note: Here we load all the fields. We won't actually use the projected data, but this serves to 
            // load the relevant data into the database, so that we can use the single query above in memory
            // for each entity needing updating.
            const int maxAtOnce = 500;
            theQueryForMultipleNeedingResetting = CompiledQuery.Compile((RaterooDataContext CurrentDataContext) =>                               
                    CurrentDataContext.GetTable<Field>()
                    .Join<Field,TblRow,TblRow,Field>(
                        CurrentDataContext.GetTable<TblRow>().Where(x => x.TblRowFieldDisplay.ResetNeeded).OrderBy(x => x.InitialFieldsDisplaySet).Take(50),
                        theField => theField.TblRow, // outer
                        theTblRow => theTblRow, // inner (from MultipleTblRows)
                        (theField, theTblRow) => theField // keep the field
                    )
                    .GroupBy(f => f.TblRow)
                    .Select(x => new TblRowPlusFieldInfos {
                            TblRow = x.Key,
                            PointsManager = x.Key.Tbl.PointsManager,
                            Fields = x
                                .OrderBy(f => f.FieldDefinition.FieldNum)
                                .ThenBy(f => f.FieldDefinitionID)
                                .Select(f => new FieldDisplayInfoComplete
                                {
                                    Field = f,
                                    FieldDesc = f.FieldDefinition,
                                    DisplaySettings = f.FieldDefinition.DisplayInTblRowPageSettings, /* note: other might be used in query above */
                                    TheAddressField = (f.FieldDefinition.FieldType == (int)FieldTypes.AddressField) ? f.AddressFields.Where(z => z.Status == (int) StatusOfObject.Active).FirstOrDefault() : null,
                                    TheNumberField = (f.FieldDefinition.FieldType == (int)FieldTypes.NumberField) ? f.NumberFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheTextField = (f.FieldDefinition.FieldType == (int)FieldTypes.TextField) ? f.TextFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheDateTimeField = (f.FieldDefinition.FieldType == (int)FieldTypes.DateTimeField) ? f.DateTimeFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheChoiceField = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.ChoiceFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheChoices = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.ChoiceFields.Where(z => z.Status == (int)StatusOfObject.Active).SelectMany(y => y.ChoiceInFields).Where(z2 => z2.Status == (int)StatusOfObject.Active).Select(y => y.ChoiceInGroup) : null,
                                    TheChoiceGroupFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.FieldDefinition.ChoiceGroupFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheDateTimeFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.DateTimeField) ? f.FieldDefinition.DateTimeFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheNumberFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.NumberField) ? f.FieldDefinition.NumberFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheTextFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.TextField) ? f.FieldDefinition.TextFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null
                                }
                                ) // Note: We can't concatenate to this the table rows that have no fields, so we will deal with that in GetTblRowPlusFieldInfos.
                            }
                    )
                    .Take(maxAtOnce)
                );

        }

        public List<TblRowPlusFieldInfos> GetTblRowPlusFieldInfosWithoutCompiledQuery(IRaterooDataContext dataContextToUse)
        {
            return dataContextToUse.GetTable<Field>()
                    .Join<Field,TblRow,TblRow,Field>(
                        dataContextToUse.GetTable<TblRow>().Where(x => x.TblRowFieldDisplay.ResetNeeded).OrderBy(x => x.InitialFieldsDisplaySet).Take(50),
                        theField => theField.TblRow, // outer
                        theTblRow => theTblRow, // inner (from MultipleTblRows)
                        (theField, theTblRow) => theField // keep the field
                    )
                    .GroupBy(f => f.TblRow)
                    .Select(x => new TblRowPlusFieldInfos {
                            TblRow = x.Key,
                            PointsManager = x.Key.Tbl.PointsManager,
                            Fields = x
                                .OrderBy(f => f.FieldDefinition.FieldNum)
                                .ThenBy(f => f.FieldDefinitionID)
                                .Select(f => new FieldDisplayInfoComplete
                                {
                                    Field = f,
                                    FieldDesc = f.FieldDefinition,
                                    DisplaySettings = f.FieldDefinition.DisplayInTblRowPageSettings, /* note: other might be used in query above */
                                    TheAddressField = (f.FieldDefinition.FieldType == (int)FieldTypes.AddressField) ? f.AddressFields.Where(z => z.Status == (int) StatusOfObject.Active).FirstOrDefault() : null,
                                    TheNumberField = (f.FieldDefinition.FieldType == (int)FieldTypes.NumberField) ? f.NumberFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheTextField = (f.FieldDefinition.FieldType == (int)FieldTypes.TextField) ? f.TextFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheDateTimeField = (f.FieldDefinition.FieldType == (int)FieldTypes.DateTimeField) ? f.DateTimeFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheChoiceField = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.ChoiceFields.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheChoices = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.ChoiceFields.Where(z => z.Status == (int)StatusOfObject.Active).SelectMany(y => y.ChoiceInFields).Where(z2 => z2.Status == (int)StatusOfObject.Active).Select(y => y.ChoiceInGroup) : null,
                                    TheChoiceGroupFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.ChoiceField) ? f.FieldDefinition.ChoiceGroupFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheDateTimeFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.DateTimeField) ? f.FieldDefinition.DateTimeFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheNumberFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.NumberField) ? f.FieldDefinition.NumberFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null,
                                    TheTextFieldDesc = (f.FieldDefinition.FieldType == (int)FieldTypes.TextField) ? f.FieldDefinition.TextFieldDefinitions.Where(z => z.Status == (int)StatusOfObject.Active).FirstOrDefault() : null
                                }
                                ) // Note: We can't concatenate to this the table rows that have no fields, so we will deal with that in GetTblRowPlusFieldInfos.
                            }
                    ).ToList()
                ;
        }

        public static TblRowPlusFieldInfos GetTblRowPlusFieldInfosWithoutFieldInfos(TblRow theTblRow)
        {
            return new TblRowPlusFieldInfos() { Fields = null, PointsManager = theTblRow.Tbl.PointsManager, TblRow = theTblRow };
        }

        public TblRowPlusFieldInfos GetTblRowPlusFieldInfos(FieldsLocation theLocation, TblRow theTblRow)
        {
            TblRowPlusFieldInfoLoaderRequest theRequest = new TblRowPlusFieldInfoLoaderRequest(theLocation, theTblRow); 
            return theQueryForSingleTblRow(theRequest);
        }

        public List<TblRowPlusFieldInfos> GetTblRowPlusFieldInfos(IRaterooDataContext dataContextToUse)
        {
            FieldsDisplaySettingsMask bitMask = new FieldsDisplaySettingsMask();
            int visibleMask = bitMask.Visible;

            const int maxSimpleRowsAtOnce = 2000;
            var rowsWithNoFieldsNeedingResetting = dataContextToUse.GetTable<TblRow>()
                                 .Where(x => x.TblRowFieldDisplay.ResetNeeded && !x.Fields.Any())
                                 .OrderBy(x => x.InitialFieldsDisplaySet)
                                 .Select(x => new TblRowPlusFieldInfos
                                 {
                                     TblRow = x,
                                     PointsManager = x.Tbl.PointsManager,
                                     Fields = null
                                 }
                                 )
                                 .Take(maxSimpleRowsAtOnce)
                                 .ToList();


            List<TblRowPlusFieldInfos> theListToReturn;
            if (dataContextToUse.IsRealDatabase())
                theListToReturn = theQueryForMultipleNeedingResetting(dataContextToUse.GetRealDatabaseIfExists()).ToList();
            else
                theListToReturn = GetTblRowPlusFieldInfosWithoutCompiledQuery(dataContextToUse);
            
            theListToReturn.AddRange(rowsWithNoFieldsNeedingResetting);

            return theListToReturn;
        }
    }

    /// <summary>
    /// FieldDefinitionInfo
    /// Stores basic information about a field descriptor. This is used by the FieldsBox.
    /// </summary>
    [TypeConverter(typeof(FieldDefinitionInfoConverter))]
    public class FieldDefinitionInfo
    {
        public FieldDefinitionInfo() { }
        public FieldDefinitionInfo(int fieldDefinitionID, string fieldName, FieldTypes fieldType, int fieldNum)
        {
            FieldDefinitionID = fieldDefinitionID;
            FieldName = fieldName;
            FieldType = fieldType;
            FieldNum = fieldNum;
        }
        public int FieldDefinitionID { get; set; }
        public string FieldName { get; set; }
        public FieldTypes FieldType { get; set; }
        public int FieldNum { get; set; }
    }

    /// <summary>
    /// FieldDefinitionInfoConverter
    /// Allows serialization of FieldDefinitionInfo to ViewState
    /// </summary>
    public class FieldDefinitionInfoConverter : System.ComponentModel.TypeConverter
    {

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(String))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(String))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            String state = value as String;
            if (state == null)
                return base.ConvertFrom(context, culture, value);
            String[] parts = state.Split('#');
            return new FieldDefinitionInfo(Convert.ToInt32(parts[0]), parts[1], (FieldTypes)Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]));
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
                throw new ArgumentException("destinationType");
            FieldDefinitionInfo fdi = value as FieldDefinitionInfo;
            if (fdi != null)
                return fdi.FieldDefinitionID.ToString() + "#" + fdi.FieldName + "#" + Convert.ToInt32(fdi.FieldType).ToString() + "#" + fdi.FieldNum.ToString();
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
