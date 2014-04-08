using System;
using System.Data;
using System.EnterpriseServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web.DataAccess;
using System.Configuration;
using System.Data.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
using MoreStrings;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for RaterooSupport
    /// </summary>
    public partial class RaterooDataManipulation
    {
        // Methods for deletion of objects.

        public void DeleteChangesGroup(int changesGroupID)
        {
            ChangesGroup theChangesGroup = DataContext.GetTable<ChangesGroup>().Single(x => x.ChangesGroupID == changesGroupID);
            var changesStatusOfObjects = DataContext.GetTable<ChangesStatusOfObject>().Where(csoo => csoo.ChangesGroupID == changesGroupID).ToList();
            foreach (var changesStatusOfObject in changesStatusOfObjects)
                DataContext.GetTable<ChangesStatusOfObject>().DeleteOnSubmit(changesStatusOfObject);
            DataContext.GetTable<ChangesGroup>().DeleteOnSubmit(theChangesGroup);
            DataContext.SubmitChanges();
        }

        /// <summary>
        /// Delete a subfield (e.g., an AddressField item) without deleting the Field itself.
        /// This is useful when replacing a field with a new item.
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="objectType"></param>
        //public void DeleteSubfield(int objectID, TypeOfObject objectType)
        //{
        //    switch (objectType)
        //    {
        //        case TypeOfObject.AddressField:
        //            var theField = RaterooDB.GetTable<AddressField>().Single(x => x.AddressFieldID == objectID);
        //            RaterooDB.GetTable<AddressField>().DeleteOnSubmit(theField);
        //            break;
        //        case TypeOfObject.ChoiceField:
        //            var theChoiceInFields = RaterooDB.GetTable<ChoiceInField>().Where(cif => cif.Status == (byte)StatusOfObject.Active && cif.ChoiceFieldID == objectID);
        //            foreach (var theChoiceInField in theChoiceInFields)
        //                RaterooDB.GetTable<ChoiceInField>().DeleteOnSubmit(theChoiceInField);
        //            var theField2 = RaterooDB.GetTable<ChoiceField>().Single(x => x.ChoiceFieldID == objectID);
        //            RaterooDB.GetTable<ChoiceField>().DeleteOnSubmit(theField2);
        //            break;
        //        case TypeOfObject.DateTimeField:
        //            var theField3 = RaterooDB.GetTable<DateTimeField>().Single(x => x.DateTimeFieldID == objectID);
        //            RaterooDB.GetTable<DateTimeField>().DeleteOnSubmit(theField3);
        //            break;
        //        case TypeOfObject.NumberField:
        //            var theField4 = RaterooDB.GetTable<NumberField>().Single(x => x.NumberFieldID == objectID);
        //            RaterooDB.GetTable<NumberField>().DeleteOnSubmit(theField4);
        //            break;
        //        case TypeOfObject.TextField:
        //            var theField5 = RaterooDB.GetTable<TextField>().Single(x => x.TextFieldID == objectID);
        //            RaterooDB.GetTable<TextField>().DeleteOnSubmit(theField5);
        //            break;
        //    }
        //    RaterooDB.SubmitChanges();
        //}

        /// <summary>
        /// Delete a field, including all components (e.g., the DateTimeField table).
        /// </summary>
        /// <param name="theField"></param>
    //    public void DeleteField(Field theField)
    //    {
    //        int fieldID = theField.FieldID;
    //        switch ((FieldTypes)theField.FieldDefinition.FieldType)
    //        {
    //            case FieldTypes.AddressField:
    //                var referringAddressFields = RaterooDB.GetTable<AddressField>().Where(nf => nf.FieldID == fieldID);
    //                foreach (var theReferringField in referringAddressFields)
    //                    RaterooDB.GetTable<AddressField>().DeleteOnSubmit(theReferringField);
    //                break;
    //            case FieldTypes.ChoiceField:
    //                var referringChoiceFields = RaterooDB.GetTable<ChoiceField>().Where(cf => cf.FieldID == fieldID);
    //                foreach (var theReferringField in referringChoiceFields)
    //                {
    //                    var choicesInField = RaterooDB.GetTable<ChoiceInField>().Where(cf => cf.ChoiceFieldID == theReferringField.ChoiceFieldID);
    //                    foreach (var theChoiceInField in choicesInField)
    //                        RaterooDB.GetTable<ChoiceInField>().DeleteOnSubmit(theChoiceInField);
    //                    RaterooDB.GetTable<ChoiceField>().DeleteOnSubmit(theReferringField);
    //                }
    //                break;
    //            case FieldTypes.DateTimeField:
    //                var referringDateTimeFields = RaterooDB.GetTable<DateTimeField>().Where(dtf => dtf.FieldID == fieldID);
    //                foreach (var theReferringField in referringDateTimeFields)
    //                    RaterooDB.GetTable<DateTimeField>().DeleteOnSubmit(theReferringField);
    //                break;
    //            case FieldTypes.NumberField:
    //                var referringNumberFields = RaterooDB.GetTable<NumberField>().Where(nf => nf.FieldID == fieldID);
    //                foreach (var theReferringField in referringNumberFields)
    //                    RaterooDB.GetTable<NumberField>().DeleteOnSubmit(theReferringField);
    //                break;
    //            case FieldTypes.TextField:
    //                var referringTextFields = RaterooDB.GetTable<TextField>().Where(tf => tf.FieldID == fieldID);
    //                foreach (var theReferringField in referringTextFields)
    //                    RaterooDB.GetTable<TextField>().DeleteOnSubmit(theReferringField);
    //                break;
    //        }
    //        RaterooDB.GetTable<Field>().DeleteOnSubmit(theField);
    //        RaterooDB.SubmitChanges();
    //    }

    }
}
