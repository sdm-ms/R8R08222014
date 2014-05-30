using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using MoreStrings;
using ClassLibrary1.Model;


    public partial class RatingTypeSelector : System.Web.UI.UserControl, IFilterField
    {
        // The following are required to implement IFilterField
        public R8RDataAccess DataAccess { get; set; }
        public FieldsBoxMode Mode { get; set; }
        public int? TblRowID { get; set; }
        public int FieldDefinitionOrTblColumnID { get; set; }
        public FilterRule GetFilterRule()
        {
            return null;
        }
        public FieldDataInfo GetFieldValue(FieldSetDataInfo theGroup)
        {
            return null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool InputDataValidatesOK(ref string errorMessage)
        {
            if (rangeOfNumbersRating.Checked)
                return MoreStrings.MoreStringManip.ValidateNumberString(rangeFrom.Text, false, null, null, ref errorMessage) && MoreStrings.MoreStringManip.ValidateNumberString(rangeTo.Text, false, null, null, ref errorMessage) && MoreStrings.MoreStringManip.ValidateNumberStringsOrdered(rangeFrom.Text, rangeTo.Text, ref errorMessage);
            if (multipleChoiceRating.Checked)
            {
                List<string> multipleChoices = multipleChoiceList.Text.Split('\n').Select(x => MoreStringManip.StripHtml(x.Trim())).Where(x => x != "").ToList();
                if (multipleChoices.Count() < 2)
                {
                    errorMessage = "You must have at least two choices.";
                    return false;
                }
                if (multipleChoices.Count() != multipleChoices.Distinct().Count())
                {
                    errorMessage = "Each choice must be unique.";
                    return false;
                }
                return true;
            }
            if (standardRating.Checked || yesNoRating.Checked)
                return true;

            errorMessage = "You must pick a rating type.";
            return false;

        }

        public UserSelectedRatingInfo GetUserSelectedRatingInfo()
        {
            if (standardRating.Checked)
                return new UserSelectedRatingInfo { theRatingType = UserSelectableRatingTypes.standardRating };
            if (yesNoRating.Checked)
                return new UserSelectedRatingInfo { theRatingType = UserSelectableRatingTypes.yesNo };
            if (multipleChoiceRating.Checked)
                return new UserSelectedRatingInfo { 
                    theRatingType = UserSelectableRatingTypes.multipleChoice,
                    multipleChoices = multipleChoiceList.Text.Split('\n').Select(x => MoreStringManip.StripHtml(x.Trim())).Where(x => x != "").ToList() 
                };
            if (rangeOfNumbersRating.Checked)
                return new UserSelectedRatingInfo
                {
                    theRatingType = UserSelectableRatingTypes.range,
                    fromRange = Convert.ToDecimal(rangeFrom.Text),
                    toRange = Convert.ToDecimal(rangeTo.Text)
                };
            throw new Exception("Internal error: unknown user selected rating info type.");
        }


    }