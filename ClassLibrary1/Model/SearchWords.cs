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
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    // Potential future improvement: Add to the database a SearchableItem, with each TblRow, TextField, ChoiceInField, Tbl,
    // PointsManager, and Domain including a 1-to-1 foreign key pointing to this SearchableItem. One of
    // the fields of the SearchableItem would be the item path string with html, and another would be the same string without html. 
    // Meanwhile, we would need to update the SearchableItem string in code such as SetSearchWordsForEntityName,
    // which is called when the EntityName changes. The advantage of this approach is that we would not need to create
    // item path strings on the fly or sort them. Also, we would be able
    // to search the SearchableItems directly, rather than aggregating
    // entities, Tbls, etc. Note that we would eliminate the SearchWordTblRowNames etc. tables, and the SearchableItem would be connected
    // to SearchWords with a junction table (SearchableItemWord).
    // It would also be good if we had some field to indicate the importance of the item, although this
    // might be hard to maintain.

    public partial class RaterooDataManipulation
    {
        public static List<string> ConvertPhraseToStringList(string phrase)
        {
            if (phrase == null || phrase == "")
                return new List<string>();
            return phrase.Split(' ').ToList().Select(word => Regex.Replace(word, @"[^\w\.@-]", "").ToUpperInvariant()).Where(word => word != "").OrderBy(x => x).Distinct().ToList();
        }

        public List<SearchWord> ConvertPhraseToSearchWordList(string phrase)
        {
            return ConvertPhraseToSearchWordList(phrase, DataContext);
        }

        public List<SearchWord> ConvertPhraseToSearchWordList(string phrase, IRaterooDataContext RaterooDB)
        {
            List<string> theList = ConvertPhraseToStringList(phrase);
            List<SearchWord> theSearchWordList = new List<SearchWord>();
            foreach (string theWord in theList)
            {
                SearchWord theSearchWord = RaterooDB.GetTable<SearchWord>().SingleOrDefault(x => x.TheWord == theWord);
                if (theSearchWord == null)
                    return new List<SearchWord>(); // return empty list if a word is not found, because no matches are possible
                theSearchWordList.Add(theSearchWord);
            }
            return theSearchWordList;
        }

        public static List<List<SearchWord>> ConvertPhraseToSearchWordListWithPartialMatches(IRaterooDataContext theDataContext, string phrase)
        {
            return ConvertPhraseToSearchWordListWithPartialMatches(phrase, theDataContext);
        }

        /// <summary>
        /// The following allows partial matches. So the phrase "blog now" will return a list, the 
        /// first element of which is all words beginning with "blog" (including "blogger" and "blogging")
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public static List<List<SearchWord>> ConvertPhraseToSearchWordListWithPartialMatches(string phrase, IRaterooDataContext theDataContext)
        {
            List<string> theList = ConvertPhraseToStringList(phrase);
            List<List<SearchWord>> theSearchWordList = new List<List<SearchWord>>();
            foreach (string theWord in theList)
            {
                List<SearchWord> theSearchWordMatches = theDataContext.GetTable<SearchWord>().Where(x => x.TheWord.StartsWith(theWord)).OrderByDescending(x => (x.TheWord == theWord) ? 1 : 0).ThenBy(x => x.SearchWordTblRowNames.Count()).Take(4).ToList();
                if (!theSearchWordMatches.Any())
                    return new List<List<SearchWord>>(); // return empty list if a word is not found, because no matches are possible
                theSearchWordList.Add(theSearchWordMatches);
            }
            return theSearchWordList;
        }

        // Note that DataContext must be submitted after call to this.
        public List<SearchWord> GetOrAddSearchWords(string phrase, bool deferDBQuery)
        {
            List<string> words = ConvertPhraseToStringList(phrase);
            List<SearchWord> theList = new List<SearchWord>();
            foreach (var word in words)
            {
                if (deferDBQuery)
                { 
                    // We don't want to query the database repeatedly to see if SearchWords exist for particular words, so when we defer, we simply add a new SearchWord and register it to be inserted.
                    // Then, when we SubmitChanges, we'll do a bulk search for matching search words, and then
                    // delete SearchWords that we don't actually need.
                    
                    SearchWord theSearchWordToMaybeAdd = DataContext.RegisteredToBeInserted.OfType<SearchWord>().SingleOrDefault(x => x.TheWord == word);
                    if (theSearchWordToMaybeAdd == null)
                        theSearchWordToMaybeAdd = AddSearchWord(word); // will register for insertion
                    theList.Add(theSearchWordToMaybeAdd);
                }
                else
                {
                    SearchWord theSearchWord = DataContext.NewOrSingleOrDefault<SearchWord>(x => x.TheWord == word);
                    if (theSearchWord == null)
                        theSearchWord = AddSearchWord(word);
                    theList.Add(theSearchWord);
                }
            }
            return theList;
        }

        public static void BulkAddSearchWordsCorrect(IRaterooDataContext theDB)
        {
            // Having deferred the database query, we now must delete redundantly added search words, and correct references.
            const int numToProcessAtATime = 1000;
            int numAlreadyTaken = 0;
            List<SearchWord> thoseToMaybeAdd = theDB.RegisteredToBeInserted.OfType<SearchWord>().OrderBy(x => x.TheWord).ToList();
            if (!thoseToMaybeAdd.Any())
                return;

            int numInList = thoseToMaybeAdd.Count();
            while (numAlreadyTaken < numInList)
            {
                List<string> searchTheseWordsNow = thoseToMaybeAdd.Skip(numAlreadyTaken).Take(numToProcessAtATime).Select(x => x.TheWord).OrderBy(x => x).ToList();
                IQueryable<SearchWord> alreadyInDatabase = theDB.GetTable<SearchWord>().Where(x => searchTheseWordsNow.Contains(x.TheWord));
                foreach (var theSearchWordAlreadyInDB in alreadyInDatabase)
                {
                    SearchWord redundantlyAdded = thoseToMaybeAdd.SingleOrDefault(x => x.TheWord == theSearchWordAlreadyInDB.TheWord);
                    if (redundantlyAdded != null)
                    {
                        var y1 = redundantlyAdded.SearchWordChoices.ToList();
                        foreach (var x in y1)
                            x.SearchWord = theSearchWordAlreadyInDB;
                        var y2 = redundantlyAdded.SearchWordHierarchyItems.ToList();
                        foreach (var x in y2)
                            x.SearchWord = theSearchWordAlreadyInDB;
                        var y4 = redundantlyAdded.SearchWordTblRowNames.ToList();
                        foreach (var x in y4)
                            x.SearchWord = theSearchWordAlreadyInDB;
                        var y5 = redundantlyAdded.SearchWordTextFields.ToList();
                        foreach (var x in y5)
                            x.SearchWord = theSearchWordAlreadyInDB;
                        theDB.GetTable<SearchWord>().DeleteOnSubmit(redundantlyAdded);
                        theDB.RegisteredToBeInserted.Remove(redundantlyAdded);
                    }
                }
                numAlreadyTaken += searchTheseWordsNow.Count();
            }
            
        }

        // Note that DataContext must be submitted after call to this.
        public SearchWord GetOrAddSearchWord(string word)
        {
                SearchWord theSearchWord = DataContext.GetTable<SearchWord>().SingleOrDefault(x => x.TheWord == word);
                if (theSearchWord == null)
                    theSearchWord = AddSearchWord(word);
                return theSearchWord;
        }

        public void IdentifySearchWordsToChange(List<SearchWord> originalList, string thePhrase, out List<SearchWord> newList, out List<SearchWord> deleteList)
        {
            List<string> theWords = ConvertPhraseToStringList(thePhrase);
            deleteList = new List<SearchWord>();
            newList = new List<SearchWord>();
            foreach (SearchWord theSearchWord in originalList)
            {
                if (!theWords.Contains(theSearchWord.TheWord))
                    deleteList.Add(theSearchWord);
            }
            foreach (string word in theWords)
            {
                if (!originalList.Any(x => x.TheWord == word))
                    newList.Add(GetOrAddSearchWord(word));
            }
        }

        public void SetSearchWordsForEntityName(TblRow theTblRow, bool submitChanges)
        {
            List<SearchWord> newList = null, deleteList = null;
            if (theTblRow.SearchWordTblRowNames.Any())
            {
                IdentifySearchWordsToChange(theTblRow.SearchWordTblRowNames.Select(x => x.SearchWord).ToList(), theTblRow.Name, out newList, out deleteList);
            }
            else
            {
                newList = GetOrAddSearchWords(theTblRow.Name, true);
            }
            foreach (SearchWord toAdd in newList)
                AddSearchWordTblRowName(toAdd, theTblRow);
            if (deleteList != null)
            {
                foreach (SearchWord toDelete in deleteList)
                    DataContext.GetTable<SearchWord>().DeleteOnSubmit(toDelete);
            }
            if (submitChanges)
                DataContext.SubmitChanges();
        }

        public void SetSearchWordsForChoiceInGroup(ChoiceInGroup theChoiceInGroup, bool submitChanges)
        {
            List<SearchWord> newList = null, deleteList = null;
            if (theChoiceInGroup.SearchWordChoices.Any())
            {
                IdentifySearchWordsToChange(theChoiceInGroup.SearchWordChoices.Select(x => x.SearchWord).ToList(), theChoiceInGroup.ChoiceText, out newList, out deleteList);
            }
            else
            {
                newList = GetOrAddSearchWords(theChoiceInGroup.ChoiceText, false);
            }
            foreach (SearchWord toAdd in newList)
                AddSearchWordChoice(toAdd, theChoiceInGroup);
            if (deleteList != null)
            {
                foreach (SearchWord toDelete in deleteList)
                    DataContext.GetTable<SearchWord>().DeleteOnSubmit(toDelete);
            }
            if (submitChanges)
                DataContext.SubmitChanges();
        }

        public void SetSearchWordsForHierarchyItem(HierarchyItem theHierarchyItem, bool submitChanges)
        {
            List<SearchWord> newList = null, deleteList = null;
            string itemName = "";
            if (theHierarchyItem.IncludeInMenu)
                itemName = theHierarchyItem.HierarchyItemName;
            if (theHierarchyItem.SearchWordHierarchyItems.Any())
            {
                IdentifySearchWordsToChange(theHierarchyItem.SearchWordHierarchyItems.Select(x => x.SearchWord).ToList(), itemName, out newList, out deleteList);
            }
            else
            {
                newList = GetOrAddSearchWords(itemName, false);
            }
            foreach (SearchWord toAdd in newList)
                AddSearchWordHierarchyItem(toAdd, theHierarchyItem);
            if (deleteList != null)
            {
                foreach (SearchWord toDelete in deleteList)
                    DataContext.GetTable<SearchWord>().DeleteOnSubmit(toDelete);
            }
            if (submitChanges)
                DataContext.SubmitChanges();
        }
       

        public void SetSearchWordsForTextField(TextField theTextField, bool submitChanges)
        {
            List<SearchWord> newList = null, deleteList = null;
            if (theTextField.SearchWordTextFields.Any())
            {
                IdentifySearchWordsToChange(theTextField.SearchWordTextFields.Select(x => x.SearchWord).ToList(), theTextField.Text, out newList, out deleteList);
            }
            else
            {
                newList = GetOrAddSearchWords(theTextField.Text, true);
            }
            foreach (SearchWord toAdd in newList)
                AddSearchWordTextField(toAdd, theTextField);
            if (deleteList != null)
            {
                foreach (SearchWord toDelete in deleteList)
                    DataContext.GetTable<SearchWord>().DeleteOnSubmit(toDelete);
            }
            if (submitChanges)
                DataContext.SubmitChanges();
        }


        internal static IQueryable<HierarchyItem> GetHierarchyItemsForWord(IRaterooDataContext theDataContext, SearchWord theSearchWord)
        {
            return theDataContext.GetTable<SearchWordHierarchyItem>().Where(x => x.SearchWord == theSearchWord).Select(x => x.HierarchyItem);
        }

        internal static IQueryable<TblRow> GetTblRowsForWord(IRaterooDataContext theDataContext, SearchWord theSearchWord, int? theTblID = null)
        {
            var Group1 = theDataContext.GetTable<SearchWordTblRowName>().Where(x => x.SearchWord == theSearchWord).Select(x => x.TblRow);
            var Group2 = theDataContext.GetTable<SearchWordTextField>().Where(x => x.SearchWord == theSearchWord).Select(x => x.TextField.Field.TblRow);
            var Group3 = theDataContext.GetTable<SearchWordChoice>().Where(x => x.SearchWord == theSearchWord).Select(x => x.ChoiceInGroup).SelectMany(x => x.ChoiceInFields).Select(x => x.ChoiceField.Field.TblRow);
            if (theTblID != null)
            {
                Group1 = Group1.Where(x => x.TblID == theTblID);
                Group2 = Group2.Where(x => x.TblID == theTblID);
                Group3 = Group3.Where(x => x.TblID == theTblID);
            }
            var MatchingTblRows = Group1.Union(Group2).Union(Group3).Distinct();
            return MatchingTblRows;
        }

        public static IQueryable<T> GetItemsForPhrase<T>(IRaterooDataContext theDataContext, List<List<SearchWord>> theWords, int? theTblID = null)
        {
            List<IQueryable<T>> listWithQueryForEachSubmittedWord = new List<IQueryable<T>>();
            foreach (List<SearchWord> searchList in theWords)
            {
                // First, we want a union of all matches for each word in this list
                IQueryable<T> theQueryForAllPartialMatchesOfWord = null;
                bool firstDone = false;
                foreach (SearchWord partiallyMatchingWord in searchList)
                {
                    IQueryable<T> theQuery = null;
                    Type theType = typeof(T);
                    if (theType == typeof(TblRow))
                        theQuery = (IQueryable<T>)GetTblRowsForWord(theDataContext, partiallyMatchingWord, theTblID);
                    else
                    {
                        if (theTblID != null)
                            throw new Exception("Can only search within a Tbl for TblRows.");
                        if (theType == typeof(HierarchyItem))
                            theQuery = (IQueryable<T>)GetHierarchyItemsForWord(theDataContext, partiallyMatchingWord);
                        else
                            throw new Exception("GetItemsForPhrase called for unsupported type.");
                    }
                    if (!firstDone)
                    {
                        theQueryForAllPartialMatchesOfWord = theQuery;
                        firstDone = true;
                    }
                    else
                        theQueryForAllPartialMatchesOfWord = theQueryForAllPartialMatchesOfWord.Union(theQuery);
                }
                if (theQueryForAllPartialMatchesOfWord != null)
                    listWithQueryForEachSubmittedWord.Add(theQueryForAllPartialMatchesOfWord);
            }
            IQueryable<T> theIntersection = null;
            bool firstDone2 = false;
            if (listWithQueryForEachSubmittedWord.Any())
            {
                foreach (var wordQuery in listWithQueryForEachSubmittedWord)
                {
                    if (!firstDone2)
                    {
                        theIntersection = wordQuery;
                        firstDone2 = true;
                    }
                    else
                        theIntersection = theIntersection.Intersect(wordQuery);
                }
            }
            return theIntersection;
        }

        public static IQueryable<TblRow> GetTblRowsForPhrase(string thePhrase, IRaterooDataContext theDataContext, int theTblID)
        {
            List<List<SearchWord>> theList = ConvertPhraseToSearchWordListWithPartialMatches(thePhrase, theDataContext);
            return GetItemsForPhrase<TblRow>(theDataContext, theList, theTblID);
        }

        public static List<string> GetItemPathStringsForPhrase<T>(IRaterooDataContext theDataContext, List<List<SearchWord>> theList, int maxToInclude)
        {
            // Note that we return a List here, not an IQueryable, because once we convert
            // to the item path strings within the code below, we can't concatenate the
            // queryables anyway.
            if (maxToInclude == 0)
                return new List<string>();
            var theQuery = GetItemsForPhrase<T>(theDataContext, theList);
            List<T> theListToReturn = theQuery.Take(maxToInclude).ToList();
            return theListToReturn.Select(x => ItemPathWrapper.GetItemPath(x,  true)).OrderBy(x => MoreStrings.MoreStringManip.StripHtml(x)).ToList();
        }

        public static List<string> GetItemPathStringsForPhrase(IRaterooDataContext theDataContext, string thePhrase, int maxToInclude)
        {
            List<List<SearchWord>> theList = ConvertPhraseToSearchWordListWithPartialMatches(theDataContext, thePhrase);
            if (theList.Any())
            {
                List<string> theItemPathStringList = new List<string>();
                for (int i = 1; i <= 2; i++)
                {
                    int numToInclude = maxToInclude - theItemPathStringList.Count();
                    List<string> stringListToAdd = null;
                    switch (i)
                    {
                        case 1:
                            stringListToAdd = GetItemPathStringsForPhrase<HierarchyItem>(theDataContext, theList, numToInclude); 
                            break;
                        case 2:
                            stringListToAdd = GetItemPathStringsForPhrase<TblRow>(theDataContext, theList, numToInclude); 
                            break;
                    }
                    theItemPathStringList.AddRange(stringListToAdd);
                }
                return theItemPathStringList;
            }
            else
                return new List<string>();
        }

        public static List<AutoCompleteData> GetAutoCompleteData<T>(IRaterooDataContext theDataContext, List<List<SearchWord>> theList)
        {
            const int maxToInclude = 10;
            var theQuery = GetItemsForPhrase<T>(theDataContext, theList).Take(maxToInclude).ToList();
            var theQueryRevised = theQuery.Select(x => ItemPathWrapper.GetAutoCompleteData(x)).
            OrderBy(x => x.ItemPath).ToList();
            return theQueryRevised;
        }

        public static List<AutoCompleteData> GetAutoCompleteData(IRaterooDataContext theDataContext, string thePhrase)
        {
            List<List<SearchWord>> theList = ConvertPhraseToSearchWordListWithPartialMatches(theDataContext, thePhrase);
            if (theList.Any())
            {
                var acHierarchyItem = GetAutoCompleteData<HierarchyItem>(theDataContext, theList);
                var acTblRow = GetAutoCompleteData<TblRow>(theDataContext, theList);
                return acHierarchyItem.Concat(acTblRow).ToList();
            }
            else
                return new List<AutoCompleteData>();
        }

    }
}