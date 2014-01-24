using System;
using System.Globalization;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;
using System.Diagnostics;

namespace MoreStrings
{
    /// 
    /// Summary description for Class1.
    /// 
    public static class MoreStringManip
    {

        public static string TruncateString(string theString, int maxLength)
        {
            return theString.Substring(0, Math.Min(theString.Length, maxLength));
        }

        public static string GetHashString(this object theObject)
        {
            if (theObject == null)
                return null;
            return theObject.GetHashCode().ToString();
        }

        public static string Left(this string param, int length)
        {
            //we start at 0 since we want to get the characters starting from the
            //left and with the specified lenght and assign it to a variable
            if (length > param.Length)
                length = param.Length;
            string result = param.Substring(0, length);
            //return the result of the operation
            return result;
        }
        public static string Right(this string param, int length)
        {
            //start at the index based on the lenght of the sting minus
            //the specified lenght and assign it a variable
            if (length > param.Length)
                length = param.Length;
            string result = param.Substring(param.Length - length, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(this string param, int startIndex, int length)
        {
            //start at the specified index in the string ang get N number of
            //characters depending on the lenght and assign it to a variable
            string result = param.Substring(startIndex, length);
            //return the result of the operation
            return result;
        }

        public static string Mid(this string param, int startIndex)
        {
            //start at the specified index and return all characters after it
            //and assign it to a variable
            string result = param.Substring(startIndex);
            //return the result of the operation
            return result;
        }

        public static bool IsNumeric(this string param, ref double theNumber)
        {
            CultureInfo MyCultureInfo = new CultureInfo("en-US");
            double d;
            bool returnVal = Double.TryParse(param, System.Globalization.NumberStyles.Float, MyCultureInfo, out d);
            if (returnVal)
                theNumber = d;
            return returnVal;
        }

        public static bool IsInteger(this string param, ref int theInteger)
        {
            CultureInfo MyCultureInfo = new CultureInfo("en-US");
            double d;
            bool returnVal = Double.TryParse(param, System.Globalization.NumberStyles.Integer, MyCultureInfo, out d);
            if (returnVal)
                theInteger = Convert.ToInt32(param);
            return returnVal;
        }

        public static bool IsInteger(this string param)
        {
            int intParam = 0;
            return IsInteger(param, ref intParam);
        }

        public static string IncrementNumAtEndOfString(this string param)
        {
            int numCharsAtEnd = 0;
            bool integersFound = true;
            int stringPos = param.Length - 1;
            while (stringPos >= 0 && integersFound)
            {
                integersFound = param.Mid(stringPos, 1).IsInteger();
                if (integersFound)
                {
                    stringPos--;
                    numCharsAtEnd++;
                }
            }
            string textPart;
            int theInteger;
            if (numCharsAtEnd == 0)
            {
                textPart = param;
                if (textPart.Right(1) != " ")
                    textPart += " ";
                theInteger = 2;
            }
            else
            {
                textPart = param.Left(param.Length - numCharsAtEnd);
                theInteger = Convert.ToInt32(param.Right(numCharsAtEnd)) + 1;
            }

            return textPart + theInteger.ToString();

        }

        public static string FormatToExactDecimalPlaces(decimal? value, int decimalPlaces)
        {
            if (value == null)
                return "--";
            return ((decimal)value).ToString("F" + decimalPlaces.ToString());
            //string theText;
            //if (value == null)
            //    theText = "--";
            //else
            //{
            //    if (value == 0)
            //        return "0"; 
            //    double roundedToDecimalPlaces = Math.Round((double)value, decimalPlaces);
            //    theText = String.Format("{0:#,###,###,###.######}",roundedToDecimalPlaces);
            //    string theTextNoCommas = roundedToDecimalPlaces.ToString();
            //    if (decimalPlaces == 0)
            //        return theText;

            //    double roundedToInteger = (value > 0) ? Math.Floor((double)value) : Math.Ceiling((double)value);
            //    int numDigitsLeftOfDecimalPlace = (roundedToInteger.ToString()).Length;
            //    int totalNumberofDigits = theTextNoCommas.Length;
            //    if (numDigitsLeftOfDecimalPlace != totalNumberofDigits)
            //        totalNumberofDigits--; // Don't count the period
            //    int existingDigitsToRightOfDecimalPlace = totalNumberofDigits - numDigitsLeftOfDecimalPlace;
            //    int numZerosToAdd = decimalPlaces - existingDigitsToRightOfDecimalPlace;
            //    if (numDigitsLeftOfDecimalPlace == totalNumberofDigits)
            //        theText += ".";
            //    for (int digitsToAdd = 1; digitsToAdd <= numZerosToAdd; digitsToAdd++)
            //        theText += "0";
            //}
            //return theText;
        }

        public static string MyRenderControl(this Control ctrl)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            ctrl.RenderControl(hw);
            return sb.ToString();
        }

        public class PageWithFix : Page
        {
            public override void VerifyRenderingInServerForm(Control control)
            {
                return;
            }
        }

        [DebuggerStepThrough]
        public static string RenderUnloadedUserControl(string path,
             string propertyName, object propertyValue)
        {
            PageWithFix pageHolder = new PageWithFix();
            pageHolder.EnableEventValidation = false;
            UserControl viewControl =
               (UserControl)pageHolder.LoadControl(path);

            if (propertyValue != null)
            {
                Type viewControlType = viewControl.GetType();
                PropertyInfo property =
                   viewControlType.GetProperty(propertyName);

                if (property != null)
                {
                    property.SetValue(viewControl, propertyValue, null);
                }
                else
                {
                    throw new Exception(string.Format(
                       "UserControl: {0} does not have a public {1} property.",
                       path, propertyName));
                }
            }

            pageHolder.Controls.Add(viewControl);
            StringWriter output = new StringWriter();
            HttpContext.Current.Server.Execute(pageHolder, output, false);
            return output.ToString();
        }

        public static bool ValidateNumberString(string theString, bool emptyOK, decimal? minValue, decimal? maxValue)
        {
            string throwAway = "";
            return ValidateNumberString(theString, emptyOK, minValue, maxValue, ref throwAway);
        }

        public static bool ValidateNumberString(string theString, bool emptyOK, decimal? minValue, decimal? maxValue, ref string errorMessage)
        {
            errorMessage = "";
            if (emptyOK && theString.Trim() == "")
                return true;
            decimal theNumber;
            try
            {
                theNumber = Convert.ToDecimal(theString);
            }
            catch
            {
                errorMessage = "Enter numeric data.";
                return false;
            }
            if (minValue != null && theNumber < (decimal)minValue)
            {
                errorMessage = "The minimum value is " + minValue.ToString();
                return false;
            }
            if (maxValue != null && theNumber > (decimal)maxValue)
            {
                errorMessage = "The maximum value is " + maxValue.ToString();
                return false;
            }
            return true;
        }

        public static bool ValidateNumberStringsOrdered(string smallerNumber, string biggerNumber, ref string errorMessage)
        {
            try
            {
                decimal smallerNumberDec = Convert.ToDecimal(smallerNumber);
                decimal biggerNumberDec = Convert.ToDecimal(biggerNumber);
                if (biggerNumberDec > smallerNumberDec)
                    return true;
                errorMessage = "The \"from\" number must be smaller than the \"to\" number.";
                return false;
            }
            catch
            {
                errorMessage = "Enter numeric data.";
                return false;
            }
        }

        public static string StripHtml(string text)
        {
            return Regex.Replace(text, @"<(.|\n)*?>", string.Empty);
        }

    }
}

