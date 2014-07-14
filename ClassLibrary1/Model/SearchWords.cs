using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Transactions;
using System.Data.Linq.Mapping;
using System.Diagnostics;
////using PredRatings;
using MoreStrings;

using System.Web.Profile;
using System.Text.RegularExpressions;
using ClassLibrary1.Model;
using ClassLibrary1.EFModel;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{

    public partial class R8RDataManipulation
    {
        public static List<string> ConvertPhraseToStringList(string phrase)
        {
            if (phrase == null || phrase == "")
                return new List<string>();
            return phrase.Split(' ').ToList().Select(word => Regex.Replace(word, @"[^\w\.@-]", "").ToUpperInvariant()).Where(word => word != "").OrderBy(x => x).Distinct().ToList();
        }

        public static IQueryable<string> GetItemPathStringsForPhrase(IR8RDataContext theDataContext, string thePhrase, int maxToInclude)
        {
            throw new NotImplementedException();
        }



        public static List<AutoCompleteData> GetAutoCompleteData(IR8RDataContext theDataContext, string thePhrase)
        {
            throw new NotImplementedException();
        }

    }
}