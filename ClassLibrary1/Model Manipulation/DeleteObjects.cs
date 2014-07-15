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
using ClassLibrary1.EFModel;


namespace ClassLibrary1.Model
{
    /// <summary>
    /// Summary description for R8RSupport
    /// </summary>
    public partial class R8RDataManipulation
    {
        // Methods for deletion of objects.

        public void DeleteChangesGroup(Guid changesGroupID)
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
        //            var theField = R8RDB.GetTable<AddressField>().Single(x => x.AddressFieldID == objectID);
        //            R8RDB.GetTable<AddressField>().DeleteOnSubmit(theField);
        //            break;
        //        case TypeOfObject.ChoiceField:
        //            var theChoiceInFields = R8RDB.GetTable<ChoiceInField>().Where(cif => cif.Status == (byte)StatusOfObject.Active && cif.ChoiceFieldID == objectID);
        //            foreach (var theChoiceInField in theChoiceInFields)
        //                R8RDB.GetTable<ChoiceInField>().DeleteOnSubmit(theChoiceInField);
        //            var theField2 = R8RDB.GetTable<ChoiceField>().Single(x => x.ChoiceFieldID == objectID);
        //            R8RDB.GetTable<ChoiceField>().DeleteOnSubmit(theField2);
        //            break;
        //        case TypeOfObject.DateTimeField:
        //            var theField3 = R8RDB.GetTable<DateTimeField>().Single(x => x.DateTimeFieldID == objectID);
        //            R8RDB.GetTable<DateTimeField>().DeleteOnSubmit(theField3);
        //            break;
        //        case TypeOfObject.NumberField:
        //            var theField4 = R8RDB.GetTable<NumberField>().Single(x => x.NumberFieldID == objectID);
        //            R8RDB.GetTable<NumberField>().DeleteOnSubmit(theField4);
        //            break;
        //        case TypeOfObject.TextField:
        //            var theField5 = R8RDB.GetTable<TextField>().Single(x => x.TextFieldID == objectID);
        //            R8RDB.GetTable<TextField>().DeleteOnSubmit(theField5);
        //            break;
        //    }
        //    R8RDB.SubmitChanges();
        //}

        /// <summary>
        /// Delete a field, including all components (e.g., the DateTimeField table).
        /// </summary>
        /// <param name="theField"></param>
    //    public void DeleteField(Field theField)
    //    {
    //        Guid fieldID = theField.FieldID;
    //        switch ((FieldTypes)theField.FieldDefinition.FieldType)
    //        {
    //            case FieldTypes.AddressField:
    //                var referringAddressFields = R8RDB.GetTable<AddressField>().Where(nf => nf.FieldID == fieldID);
    //                foreach (var theReferringField in referringAddressFields)
    //                    R8RDB.GetTable<AddressField>().DeleteOnSubmit(theReferringField);
    //                break;
    //            case FieldTypes.ChoiceField:
    //                var referringChoiceFields = R8RDB.GetTable<ChoiceField>().Where(cf => cf.FieldID == fieldID);
    //                foreach (var theReferringField in referringChoiceFields)
    //                {
    //                    var choicesInField = R8RDB.GetTable<ChoiceInField>().Where(cf => cf.ChoiceFieldID == theReferringField.ChoiceFieldID);
    //                    foreach (var theChoiceInField in choicesInField)
    //                        R8RDB.GetTable<ChoiceInField>().DeleteOnSubmit(theChoiceInField);
    //                    R8RDB.GetTable<ChoiceField>().DeleteOnSubmit(theReferringField);
    //                }
    //                break;
    //            case FieldTypes.DateTimeField:
    //                var referringDateTimeFields = R8RDB.GetTable<DateTimeField>().Where(dtf => dtf.FieldID == fieldID);
    //                foreach (var theReferringField in referringDateTimeFields)
    //                    R8RDB.GetTable<DateTimeField>().DeleteOnSubmit(theReferringField);
    //                break;
    //            case FieldTypes.NumberField:
    //                var referringNumberFields = R8RDB.GetTable<NumberField>().Where(nf => nf.FieldID == fieldID);
    //                foreach (var theReferringField in referringNumberFields)
    //                    R8RDB.GetTable<NumberField>().DeleteOnSubmit(theReferringField);
    //                break;
    //            case FieldTypes.TextField:
    //                var referringTextFields = R8RDB.GetTable<TextField>().Where(tf => tf.FieldID == fieldID);
    //                foreach (var theReferringField in referringTextFields)
    //                    R8RDB.GetTable<TextField>().DeleteOnSubmit(theReferringField);
    //                break;
    //        }
    //        R8RDB.GetTable<Field>().DeleteOnSubmit(theField);
    //        R8RDB.SubmitChanges();
    //    }

    }
}
