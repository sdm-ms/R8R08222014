using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Xml.Schema;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


using LinqToExcel;

using ClassLibrary1.Misc;
using ClassLibrary1.EFModel;

namespace ClassLibrary1.Model
{

    /// <summary>
    /// Summary description for ImportExportTblRows
    /// </summary>
    public class ImportExport
    {
        internal Tbl TheTbl;
        internal bool dictionariesInitialized = false;
        internal Dictionary<string, int> dictionaryNameToID;
        internal Dictionary<int, string> dictionaryIDToName;
        internal R8RDataAccess DataAccess;

        public ImportExport(Tbl theTbl, R8RDataAccess theDataAccess)
        {
            TheTbl = theTbl;
            DataAccess = theDataAccess;
        }

        public ImportExport(Tbl theTbl)
        {
            TheTbl = theTbl;
            DataAccess = new R8RDataAccess();
        }

        public bool IsXmlValid(string xml, ref string errorMessage)
        {
            CreateXSDFile();
            string xsd = GetXSDFileReference().GetPathToLocalFile();
            XmlSchemaSet Xs = new XmlSchemaSet();
            XmlReader xsdReader = XmlReader.Create(xsd);
            Xs.Add("", xsdReader);
            bool validateOK = false;
            try
            {
                XDocument theXMLFile = XDocument.Load(xml);
                theXMLFile.Validate(Xs, null);
                validateOK = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            finally
            {
                xsdReader.Close();
            }
            return validateOK;
        }

        // helper routine
        public static XmlNode[] TextToNodeArray(string text)
        {
            XmlDocument doc = new XmlDocument();
            return new XmlNode[1] { doc.CreateTextNode(text) };
        }

        public static string RemoveSpecialCharacters(string theString)
        {
            // First, remove __0, __1, etc. We use this in Excel files to differentiate multiple columns
            // of the same name, but our algorithm for converting the Excel files looks for matching
            // column names. We can't simply leave them matched in the Excel file, because then our
            // Excel file reader will automatically append simple integers to the column names,
            // and we want to be able to have field names with integers at the end.

            theString = System.Text.RegularExpressions.Regex.Replace(theString, "__[0-9]+", "");

            // Removing special characters
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < theString.Length; i++)
            {
                if ((theString[i] >= '0' && theString[i] <= '9') || (theString[i] >= 'A' && theString[i] <= 'z'))
                {
                    sb.Append(theString[i]);
                }
            }

            return sb.ToString();
        }

        private void InitializeDictionaries()
        {
            if (!dictionariesInitialized)
            {
                var theFieldDefinitions = DataAccess.R8RDB.GetTable<FieldDefinition>().Where(fd => fd.Tbl == TheTbl);
                dictionaryIDToName = new Dictionary<int, string>();
                dictionaryNameToID = new Dictionary<string, int>();
                foreach (var fd in theFieldDefinitions)
                {
                    Guid theID = fd.FieldDefinitionID;
                    string theName = RemoveSpecialCharacters(fd.FieldName);

                    dictionaryIDToName.Add(theID, theName);
                    dictionaryNameToID.Add(theName, theID);
                }
                dictionariesInitialized = true;
            }
        }

        public string GetAbbreviatedFieldName(int FieldDefinitionID)
        {
            InitializeDictionaries();
            string returnVal;
            try
            {
                returnVal = dictionaryIDToName[FieldDefinitionID];
            }
            catch
            {
                throw new Exception("The field definition " + FieldDefinitionID + "was not found.");
            }
            return returnVal;
        }

        public Guid GetFieldDefinitionID(string abbreviatedFieldName)
        {
            InitializeDictionaries();
            int returnVal = 0;
            try
            {
                returnVal = dictionaryNameToID[abbreviatedFieldName];
            }
            catch
            {
                throw new Exception("No field definition was found for " + abbreviatedFieldName);
            }
            return returnVal;
        }

        public int GetFieldNum(string abbreviatedFieldName)
        {
            Guid FieldDefinitionID = GetFieldDefinitionID(abbreviatedFieldName);
            return DataAccess.R8RDB.GetTable<FieldDefinition>().Single(f => f.FieldDefinitionID == FieldDefinitionID).FieldNum;
        }

        public FieldTypes GetFieldType(string abbreviatedFieldName, ref bool allowsMultipleSelections)
        {
            Guid theFieldDefinitionID = GetFieldDefinitionID(abbreviatedFieldName);
            FieldDefinition theFieldDefinition = DataAccess.R8RDB.GetTable<FieldDefinition>().Single(x => x.FieldDefinitionID == theFieldDefinitionID);
            if (theFieldDefinition.FieldType != (int)FieldTypes.ChoiceField)
                allowsMultipleSelections = false;
            else
            {
                ChoiceGroupFieldDefinition theCGFD = DataAccess.R8RDB.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == theFieldDefinitionID);
                allowsMultipleSelections = theCGFD.ChoiceGroup.AllowMultipleSelections;
            }
            return (FieldTypes)theFieldDefinition.FieldType;
        }

        public class ExcelFileColumnTracker
        {
            public class ExcelFileFieldInfo
            {
                public string abbreviatedFieldName = "";
                public int fieldNum;
                public Guid FieldDefinitionID;
                public FieldTypes theFieldType;
                public bool allowsMultipleSelections;
                public int firstOrOnlyColumnNumber;
                public int lastColumnNumber { get; set; }

                public ExcelFileFieldInfo(ImportExport theImportExport, string abbreviatedName, int colNum)
                {
                    abbreviatedFieldName = abbreviatedName;
                    fieldNum = theImportExport.GetFieldNum(abbreviatedFieldName);
                    FieldDefinitionID = theImportExport.GetFieldDefinitionID(abbreviatedFieldName);

                    theFieldType = theImportExport.GetFieldType(abbreviatedName, ref allowsMultipleSelections);
                    firstOrOnlyColumnNumber = colNum;
                    lastColumnNumber = colNum;
                }

            }

            public List<string> ColumnNames;
            public List<ExcelFileFieldInfo> FieldInfos = new List<ExcelFileFieldInfo>();
            public int? actionOnImportColumn;
            public int? tblRowIDColumn;
            public int? initialStatusColumn;
            public int? statusChangeColumn;
            public int? rowNameColumn;
            public ImportExport theImportExport;

            public ExcelFileColumnTracker(ImportExport theImportExportToUse, List<string> theColumnNames)
            {
                ColumnNames = theColumnNames;
                theImportExport = theImportExportToUse;
                int excelColumnNum = -1;
                foreach (string colNameFull in ColumnNames)
                {
                    string columnName = ImportExport.RemoveSpecialCharacters(colNameFull); // Note that if it's already abbreviated, this will have no effect

                    excelColumnNum++;

                    if (columnName.Contains("SKIPCOL"))
                        continue;

                    if (columnName == "ActionOnImport")
                        actionOnImportColumn = excelColumnNum;
                    else if (columnName == "TblRowID")
                        tblRowIDColumn = excelColumnNum;
                    else if (columnName == "InitialStatus")
                        initialStatusColumn = excelColumnNum;
                    else if (columnName == "StatusChange")
                        statusChangeColumn = excelColumnNum;
                    else if (columnName == "TblRowName")
                        rowNameColumn = excelColumnNum;
                    else
                    {
                        ExcelFileFieldInfo theFieldInfo = FieldInfos.SingleOrDefault(x => x.abbreviatedFieldName == columnName);
                        if (theFieldInfo == null)
                            FieldInfos.Add(new ExcelFileFieldInfo(theImportExport, columnName, excelColumnNum));
                        else
                            theFieldInfo.lastColumnNumber = excelColumnNum;
                    }
                }

                if (FieldInfos != null)
                    FieldInfos = FieldInfos.OrderBy(f => f.fieldNum).ThenBy(f => f.FieldDefinitionID).ToList(); // OK to order by ID here -- just assuring consistent ordering
            }
        }

        public void ConvertExcelFileToXML(string excelFilePath, string xmlFileName)
        {
            ExcelProvider provider = ExcelProvider.Create(excelFilePath, "Sheet1");
            IEnumerable<ExcelRow> theRows = provider.Where(x => true);
            ExcelRow aRow = theRows.First(); // we do this because this will trigger loading of the column names.
            ExcelFileColumnTracker theColumnTracker = new ExcelFileColumnTracker(this, provider.columnNames);
            string typeOfTblRow = GetTypeOfTblRowForTbl();
            XElement entireList = new XElement(typeOfTblRow + "list");
            foreach (ExcelRow theRow in theRows)
            {
                XElement theTblRowElement = new XElement(typeOfTblRow);
                string actionOnImportString = "addfields";
                if (theColumnTracker.actionOnImportColumn != null)
                    actionOnImportString = theRow[(int)theColumnTracker.actionOnImportColumn].ToString();
                XAttribute theActionOnImportAttribute = new XAttribute("ActionOnImport", actionOnImportString);
                string tblRowIDString = "";
                if (theColumnTracker.tblRowIDColumn != null)
                    tblRowIDString = theRow[(int)theColumnTracker.tblRowIDColumn].ToString();
                XAttribute theTblRowIDAttribute = new XAttribute("TblRowID", tblRowIDString);
                string initialStatusString = "notexist";
                if (theColumnTracker.initialStatusColumn != null)
                    initialStatusString = theRow[(int)theColumnTracker.initialStatusColumn].ToString();
                XAttribute theInitialStatusAttribute = new XAttribute("InitialStatus", initialStatusString);
                string statusChangeString = "activate";
                if (theColumnTracker.statusChangeColumn != null)
                    statusChangeString = theRow[(int)theColumnTracker.statusChangeColumn].ToString();
                XAttribute theStatusChangeAttribute = new XAttribute("StatusChange", statusChangeString);
                theTblRowElement.Add(theActionOnImportAttribute);
                theTblRowElement.Add(theTblRowIDAttribute);
                theTblRowElement.Add(theInitialStatusAttribute);
                theTblRowElement.Add(theStatusChangeAttribute);

                if (theColumnTracker.rowNameColumn != null)
                    theTblRowElement.Add(new XElement("TblRowName", theRow[(int)theColumnTracker.rowNameColumn].ToString()));
                foreach (var fieldInfo in theColumnTracker.FieldInfos)
                {
                    XElement theFieldElement = null;
                    string theText = theRow[(int)fieldInfo.firstOrOnlyColumnNumber].ToString();
                    if (theText == "")
                        continue;
                    if (fieldInfo.theFieldType == FieldTypes.TextField)
                    {
                        theFieldElement = new XElement(fieldInfo.abbreviatedFieldName);
                        string theLink = "";
                        if (fieldInfo.firstOrOnlyColumnNumber != fieldInfo.lastColumnNumber)
                        {
                            if (fieldInfo.lastColumnNumber != fieldInfo.firstOrOnlyColumnNumber + 1)
                                throw new Exception("A text field can have only two columns, one for text, and one for link.");
                            theLink = theRow[(int)fieldInfo.lastColumnNumber].ToString();
                        }
                        if (theLink == "" && theText.Length > 7 && theText.Substring(0, 7) == "http://")
                        {
                            theLink = theText;
                            theText = "";
                        }
                        theFieldElement.Add(new XElement("Text", theText));
                        theFieldElement.Add(new XElement("Link", theLink));
                    }
                    else if (fieldInfo.theFieldType == FieldTypes.AddressField
                        || fieldInfo.theFieldType == FieldTypes.NumberField
                        || (fieldInfo.theFieldType == FieldTypes.ChoiceField && fieldInfo.allowsMultipleSelections == false))
                    {
                        theFieldElement = new XElement(fieldInfo.abbreviatedFieldName, theText);
                    }
                    else if (fieldInfo.theFieldType == FieldTypes.DateTimeField)
                    {
                        try
                        {
                            DateTime theDateTime = Convert.ToDateTime(theText);
                            theFieldElement = new XElement(fieldInfo.abbreviatedFieldName, theDateTime);
                        }
                        catch
                        {
                        }
                    }
                    else if (fieldInfo.theFieldType == FieldTypes.ChoiceField && fieldInfo.allowsMultipleSelections == true)
                    {
                        theFieldElement = new XElement(fieldInfo.abbreviatedFieldName);
                        List<string> choicesMadeAlready = new List<string>();
                        for (int multChoiceColNum = (int)fieldInfo.firstOrOnlyColumnNumber; multChoiceColNum <= (int)fieldInfo.lastColumnNumber; multChoiceColNum++)
                        {
                            string choiceText = theRow[multChoiceColNum].ToString().Trim();
                            if (choiceText == "")
                                multChoiceColNum = fieldInfo.lastColumnNumber + 1; // abort for loop
                            else
                            {
                                if (!choicesMadeAlready.Any(x => x == choiceText)) // ignore redundant entries
                                {
                                    theFieldElement.Add(new XElement("Choice", choiceText));
                                    choicesMadeAlready.Add(choiceText);
                                }
                            }
                        }
                    }

                    if (theFieldElement != null)
                        theTblRowElement.Add(theFieldElement);
                }

                entireList.Add(theTblRowElement);
            }

            XDocument XTblRow = new XDocument(
              new XDeclaration("1.0", "utf-8", "yes"),
              new XComment("TblRow Information"), entireList);

            XTblRow.Save(xmlFileName);
        }

        public string GetTypeOfTblRowForTbl()
        {
            string typeOfTblRow = TheTbl.TypeOfTblRow;
            if (typeOfTblRow == "")
                typeOfTblRow = "TblRow";
            typeOfTblRow = RemoveSpecialCharacters(typeOfTblRow);
            return typeOfTblRow;
        }


        public void AddEntryToImportLog(XElement theLogList, int elementNumber, XElement theElement, string errorMessage)
        {
            //ProfileSimple.Start("AddEntryToImportLog");
            XElement errorElement = new XElement("ErrorLog" + elementNumber);
            errorElement.Add(new XElement("ErrorMessage", errorMessage));
            errorElement.Add(theElement);
            theLogList.Add(errorElement);
            //ProfileSimple.End("AddEntryToImportLog");
        }

        public FieldSetDataInfo ConvertXElementToFieldSetDataInfo(XElement theElement, TblRow theTblRow, bool copyValues)
        {
            InitializeDictionaries();
            FieldSetDataInfo theInfo = new FieldSetDataInfo(theTblRow, TheTbl, DataAccess);
            var theFields = theElement.Elements();
            foreach (var theField in theFields)
            {
                if (theField.Name == "SerializedValues" && !copyValues)
                    continue;
                if (theField.Name == "SerializedValues" && copyValues)
                {
                    if (!String.IsNullOrWhiteSpace(theField.Value))
                        theInfo.defaultTblVals = JsonSerializer.Deserialize<List<TblVal>>(theField.Value);
                    continue;
                }
                if (theField.Name == "TblRowName")
                    theInfo.theRowName = theField.Value;
                else
                {
                    int FieldDefinitionID = dictionaryNameToID[theField.Name.ToString()];
                    FieldDefinition theFieldDefinition = DataAccess.R8RDB.TempCacheGet("FieldDefinition" + FieldDefinitionID) as FieldDefinition;
                    if (theFieldDefinition == null)
                    {
                        theFieldDefinition = DataAccess.R8RDB.GetTable<FieldDefinition>().SingleOrDefault(fd => fd.FieldDefinitionID == FieldDefinitionID);
                        DataAccess.R8RDB.TempCacheAdd("FieldDefinition" + FieldDefinitionID, theFieldDefinition);
                    }
                    if (theFieldDefinition == null)
                        throw new Exception("field definition not found for " + theElement.Name);
                    switch (theFieldDefinition.FieldType)
                    {
                        case (int)FieldTypes.AddressField:
                            theInfo.AddFieldDataInfo(new AddressFieldDataInfo(theFieldDefinition, theField.Value, null, null, theInfo, DataAccess));
                            break;
                        case (int)FieldTypes.ChoiceField:
                            if (theFieldDefinition.ChoiceGroupFieldDefinitions.Single().ChoiceGroup.AllowMultipleSelections)
                            {
                                List<string> theChoices = theField.Elements().Select(x => x.Value).ToList();
                                theInfo.AddFieldDataInfo(new ChoiceFieldDataInfo(theFieldDefinition, theChoices, theInfo, DataAccess));
                            }
                            else
                            {
                                theInfo.AddFieldDataInfo(new ChoiceFieldDataInfo(theFieldDefinition, theField.Value, theInfo, DataAccess));
                            }
                            break;
                        case (int)FieldTypes.DateTimeField:
                            theInfo.AddFieldDataInfo(new DateTimeFieldDataInfo(theFieldDefinition, (DateTime)theField, theInfo, DataAccess));
                            break;
                        case (int)FieldTypes.NumberField:
                            theInfo.AddFieldDataInfo(new NumericFieldDataInfo(theFieldDefinition, (decimal)theField, theInfo, DataAccess));
                            break;
                        case (int)FieldTypes.TextField:
                            string theText = theField.Element("Text").Value;
                            string theLink = theField.Element("Link").Value;
                            theInfo.AddFieldDataInfo(new TextFieldDataInfo(theFieldDefinition, theText, theLink, theInfo, DataAccess));
                            break;
                    }
                }
            }
            theInfo.VerifyCanBeAdded();
            return theInfo;
        }

        public void PerformImportHelper(string storedFileName, string storedLogName, string entityType, Guid UserID, int startingRecord, int lastRecord, bool copyValuesIntoTbl)
        {
            R8RFile storedFile = new R8RFile("import", storedFileName);
            string storedFileLocation = storedFile.LoadPreviouslyStored();
            R8RFile storedLog = new R8RFile("importlog", storedFileName);
            string storedLogLocation = storedLog.LoadPreviouslyStored();

            string fileLocCacheKey = "IMPORT" + storedFileLocation;
            XElement TblRowList;
            TblRowList = (XElement)CacheManagement.GetItemFromCache(fileLocCacheKey);
            if (TblRowList == null)
            {
                TblRowList = XElement.Load(storedFileLocation);
                CacheManagement.AddItemToCache(fileLocCacheKey, new string[] { }, TblRowList);
            }
            XDocument theLogListFile = XDocument.Load(storedLogLocation); // We don't cache this, since it may change and should be smallish.


            XElement theLogList = theLogListFile.Element("LogList");
            ActionProcessor theActionProcessor = new ActionProcessor();

            if (TblRowList != null)
            {
                IEnumerable<XElement> theElements = TblRowList.Descendants(entityType).Skip(startingRecord - 1).Take(lastRecord - startingRecord + 1);

                System.Diagnostics.Trace.TraceInformation("Import from " + storedLogLocation + " element numbers " + startingRecord + " to " + lastRecord);
                int elementNum = startingRecord - 1;
                foreach (XElement theTblRowElement in theElements)
                {
                    //ProfileSimple.Start("TblRow import");
                    elementNum++;

                    XAttribute TblRowIDAttribute = theTblRowElement.Attribute("TblRowID");
                    XAttribute ActionOnImportAttribute = theTblRowElement.Attribute("ActionOnImport");
                    XAttribute InitialStatusAttribute = theTblRowElement.Attribute("InitialStatus");
                    XAttribute StatusChangeAttribute = theTblRowElement.Attribute("StatusChange");

                    int? ExistingTblRowID = null;
                    TblRow theTblRow = null;
                    // Check correctness of InitialStatusAttribute and corresponding TblRowIDAttribute
                    if (InitialStatusAttribute.Value == "notexist" && TblRowIDAttribute.Value != "")
                    {
                        AddEntryToImportLog(theLogList, elementNum, theTblRowElement, "No TblRowID should be provided if initial status is notexist");
                        continue;
                    }
                    if (InitialStatusAttribute.Value == "notexist" && ActionOnImportAttribute.Value == "updatefields")
                    {
                        AddEntryToImportLog(theLogList, elementNum, theTblRowElement, "Cannot update fields of a tblRow that did not exist. Use addfields to add the tblRow.");
                        continue;
                    }
                    if (InitialStatusAttribute.Value != "notexist")
                    {
                        try
                        {
                            ExistingTblRowID = int.Parse(TblRowIDAttribute.Value);
                        }
                        catch
                        {
                            AddEntryToImportLog(theLogList, elementNum, theTblRowElement, "TblRowID was not in correct format for an existing entity");
                            continue;
                        }
                        theTblRow = DataAccess.R8RDB.GetTable<TblRow>().SingleOrDefault(x => x.TblRowID == ExistingTblRowID && x.Tbl == TheTbl);
                        if (theTblRow == null)
                        {
                            AddEntryToImportLog(theLogList, elementNum, theTblRowElement, "'RowId' of tblRow given by user " + ExistingTblRowID + " does not exist for this Tbl.");
                            continue;
                        }
                    }

                    if (ActionOnImportAttribute.Value == "updatefields" || ActionOnImportAttribute.Value == "addfields")
                    { // load the fields and update them or add them and the new tblRow.
                        try
                        {
                            //ProfileSimple.Start("ConvertXElement");
                            FieldSetDataInfo theFieldSetDataInfo = ConvertXElementToFieldSetDataInfo(theTblRowElement, theTblRow, copyValuesIntoTbl);
                            //ProfileSimple.End("ConvertXElement");
                            if (ActionOnImportAttribute.Value == "updatefields" && ExistingTblRowID != null)
                                theActionProcessor.FieldSetImplement(theFieldSetDataInfo, UserId, false, false); // No rewards currently on mass import
                            else
                            {
                                //ProfileSimple.Start("TblRowCreateWithFields");
                                if (!String.IsNullOrWhiteSpace(theFieldSetDataInfo.theRowName)) // skip unnamed table rows
                                    theTblRow = theActionProcessor.TblRowCreateWithFields(theFieldSetDataInfo, UserId);
                                //ProfileSimple.End("TblRowCreateWithFields");
                            }


                        }
                        catch (Exception ex)
                        {
                            AddEntryToImportLog(theLogList, elementNum, theTblRowElement, "The following message occurred in processing the table row: " + ex.Message);
                            continue;
                        }
                    }

                    ActionProcessor Obj = new ActionProcessor();
                    if (StatusChangeAttribute.Value == "activate" && theTblRow != null && theTblRow.Status != (int)StatusOfObject.Active)
                        Obj.TblRowDeleteOrUndelete(theTblRow.TblRowID, false, true, UserId, null);
                    else if (StatusChangeAttribute.Value == "inactivate" && theTblRow != null && (theTblRow.Status == (int)StatusOfObject.Active || theTblRow.Status == (int)StatusOfObject.DerivativelyUnavailable))
                        Obj.TblRowDeleteOrUndelete(theTblRow.TblRowID, true, true, UserId, null);
                    //ProfileSimple.End("TblRow import");
                }

            }
            theLogListFile.Save(storedLogLocation);
            storedLog.StorePermanently();
        }

        R8RFile mainFile;
        R8RFile logFile;
        public void ChooseImportFileNames()
        {
            if (mainFile == null || logFile == null)
            {
                DateTime theTime = TestableDateTime.Now;
                string theFullTimeString = TestableDateTime.Now.ToString("yyMMdd HHmmss");
                string idName = "Table " + TheTbl.TblID.ToString() + " " + theFullTimeString;
                mainFile = new R8RFile("import", idName);
                logFile = new R8RFile("importlog", idName); 
            }
        }

        public void PerformImport(string sourceFileLocation, Guid userID, bool copyValuesIntoTbl)
        {
            ChooseImportFileNames();
            string storedFileLocation = mainFile.CreateTemporary();
            File.Copy(sourceFileLocation, storedFileLocation, true);
            string storedLogLocation = logFile.CreateTemporary();

            XElement theLogList = new XElement("LogList");
            XDocument theLogFile = new XDocument(
               new XDeclaration("1.0", "utf-8", "yes"),
               new XComment("TblRow Information"), theLogList);
            theLogFile.Save(storedLogLocation);


            XElement TblRowList = XElement.Load(storedFileLocation);
            string tblRowType = GetTypeOfTblRowForTbl();
            int elementCount = TblRowList.Descendants(tblRowType).Count();

            // initiate a long process to actually perform the upload
            R8RDataManipulation theDataAccessModule = new R8RDataManipulation();
            const int numToImportAtATime = 100;
            UploadTblRowsInfo theInfo = new UploadTblRowsInfo(TheTbl.TblID, 1, elementCount, numToImportAtATime, mainFile.FileName, logFile.FileName, tblRowType, UserId, copyValuesIntoTbl);
            theDataAccessModule.AddOrResetLongProcess(R8RDataManipulation.LongProcessTypes.uploadTblRows, 120, null, null, R8RDataManipulation.GetBasePriorityLevelForLongProcess(R8RDataManipulation.LongProcessTypes.uploadTblRows), theInfo);

            mainFile.StorePermanently();
            logFile.StorePermanently();

            //The following code is for synchronous upload.
            //for (int i = 1; i <= elementCount; i++)
            //    PerformImportHelper(storedFileLocation, storedLogLocation, tblRowType, UserId, i, i);


        }

        public R8RFile GetXSDFileReference()
        {
            return new R8RFile("xsd", "Tbl" + TheTbl.TblID + ".xsd");
        }

        public void CreateXSDFile()
        {
            R8RFile theFile = GetXSDFileReference();
            theFile.DeletePermanently();
            theFile.CreateTemporary();
            CreateXSDFile(theFile.GetPathToLocalFile());
            theFile.StorePermanently();
        }

        internal void CreateXSDFile(string fileLocation)
        {
            ActionProcessor Obj = new ActionProcessor();

            var Filterdata = Obj.DataContext.GetTable<FieldDefinition>()
                                 .Where(fd => fd.TblID == TheTbl.TblID && fd.Status == Convert.ToByte(StatusOfObject.Active))
                                 .OrderBy(fd => fd.FieldNum).ThenBy(fd => fd.FieldDefinitionID); // OK to order by ID to get consistent ordering

            XmlSchema Schema = new XmlSchema();
            string typeOfTblRow = GetTypeOfTblRowForTbl();

            XmlSchemaElement TopElement = new XmlSchemaElement();

            Schema.Items.Add(TopElement);
            TopElement.Name = typeOfTblRow + "list";
            XmlSchemaComplexType Ct = new XmlSchemaComplexType();
            TopElement.SchemaType = Ct;

            XmlSchemaElement Element = new XmlSchemaElement();
            // Schema.Items.Add(Element);
            Element.Name = typeOfTblRow;
            XmlSchemaComplexType CtTblRows = new XmlSchemaComplexType();
            Element.SchemaType = CtTblRows;

            XmlSchemaSequence xsr = new XmlSchemaSequence();
            xsr.MaxOccursString = "unbounded";
            Ct.Particle = xsr;
            xsr.Items.Add(Element);

            XmlSchemaAttribute EAtd = new XmlSchemaAttribute();
            EAtd.Name = "TblRowID";
            EAtd.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            CtTblRows.Attributes.Add(EAtd);

            XmlSchemaAttribute PAtd = new XmlSchemaAttribute();
            PAtd.Name = "ActionOnImport";
            PAtd.Use = XmlSchemaUse.Required;
            XmlSchemaSimpleType xsm = new XmlSchemaSimpleType();
            XmlSchemaSimpleTypeRestriction smr = new XmlSchemaSimpleTypeRestriction();
            smr.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            XmlSchemaEnumerationFacet Pf0 = new XmlSchemaEnumerationFacet();
            Pf0.Value = "addfields";
            XmlSchemaEnumerationFacet Pf1 = new XmlSchemaEnumerationFacet();
            Pf1.Value = "updatefields";
            XmlSchemaEnumerationFacet Pf2 = new XmlSchemaEnumerationFacet();
            Pf2.Value = "nofieldschange";
            smr.Facets.Add(Pf0);
            smr.Facets.Add(Pf1);
            smr.Facets.Add(Pf2);
            xsm.Content = smr;
            PAtd.SchemaType = xsm;
            CtTblRows.Attributes.Add(PAtd);


            XmlSchemaAttribute PAtd2 = new XmlSchemaAttribute();
            PAtd2.Name = "InitialStatus";
            PAtd2.Use = XmlSchemaUse.Required;
            XmlSchemaSimpleType xsm2 = new XmlSchemaSimpleType();
            XmlSchemaSimpleTypeRestriction smr2 = new XmlSchemaSimpleTypeRestriction();
            smr2.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            XmlSchemaEnumerationFacet Pf3 = new XmlSchemaEnumerationFacet();
            Pf3.Value = "notexist";
            XmlSchemaEnumerationFacet Pf4 = new XmlSchemaEnumerationFacet();
            Pf4.Value = "active";
            XmlSchemaEnumerationFacet Pf5 = new XmlSchemaEnumerationFacet();
            Pf5.Value = "inactive";
            smr2.Facets.Add(Pf3);
            smr2.Facets.Add(Pf4);
            smr2.Facets.Add(Pf5);
            xsm2.Content = smr2;
            PAtd2.SchemaType = xsm2;
            CtTblRows.Attributes.Add(PAtd2);


            XmlSchemaAttribute PAtd3 = new XmlSchemaAttribute();
            PAtd3.Name = "StatusChange";
            PAtd3.Use = XmlSchemaUse.Required;
            XmlSchemaSimpleType xsm3 = new XmlSchemaSimpleType();
            XmlSchemaSimpleTypeRestriction smr3 = new XmlSchemaSimpleTypeRestriction();
            smr3.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            XmlSchemaEnumerationFacet Pf6 = new XmlSchemaEnumerationFacet();
            Pf6.Value = "none";
            XmlSchemaEnumerationFacet Pf7 = new XmlSchemaEnumerationFacet();
            Pf7.Value = "activate";
            XmlSchemaEnumerationFacet Pf8 = new XmlSchemaEnumerationFacet();
            Pf8.Value = "inactivate";
            smr3.Facets.Add(Pf6);
            smr3.Facets.Add(Pf7);
            smr3.Facets.Add(Pf8);
            xsm3.Content = smr3;
            PAtd3.SchemaType = xsm3;
            CtTblRows.Attributes.Add(PAtd3);

            XmlSchemaSequence SqTblRow = new XmlSchemaSequence();
            CtTblRows.Particle = SqTblRow;

            XmlSchemaElement ValuesElement = new XmlSchemaElement();
            SqTblRow.Items.Add(ValuesElement);
            ValuesElement.Name = "SerializedValues";
            ValuesElement.MinOccurs = 0;
            ValuesElement.MaxOccurs = 1;
            ValuesElement.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            XmlSchemaElement NameElement = new XmlSchemaElement();
            SqTblRow.Items.Add(NameElement);
            NameElement.Name = "TblRowName";
            NameElement.MinOccurs = 1;
            NameElement.MaxOccurs = 1;
            NameElement.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            foreach (var m in Filterdata)
            {

                XmlSchemaElement FElement = new XmlSchemaElement();
                FElement.MinOccurs = 0;
                FElement.MaxOccurs = 1;
                SqTblRow.Items.Add(FElement);

                FElement.Name = GetAbbreviatedFieldName(m.FieldDefinitionID);
                FElement.Annotation = new XmlSchemaAnnotation();
                XmlSchemaDocumentation theFieldDefinitionIDDocumented = new XmlSchemaDocumentation();
                FElement.Annotation.Items.Add(theFieldDefinitionIDDocumented);
                theFieldDefinitionIDDocumented.Markup = TextToNodeArray(m.FieldDefinitionID.ToString());


                if ((FieldTypes)m.FieldType == FieldTypes.TextField)
                {
                    XmlSchemaComplexType sm = new XmlSchemaComplexType();

                    XmlSchemaSequence theSequence = new XmlSchemaSequence();

                    XmlSchemaElement em = new XmlSchemaElement();
                    em.Name = "Text";
                    em.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

                    XmlSchemaElement em2 = new XmlSchemaElement();
                    em2.Name = "Link";
                    em2.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

                    sm.Particle = theSequence;
                    theSequence.Items.Add(em);
                    theSequence.Items.Add(em2);

                    FElement.SchemaType = sm;
                }
                if ((FieldTypes)m.FieldType == FieldTypes.NumberField)
                {
                    XmlSchemaSimpleType st = new XmlSchemaSimpleType();

                    XmlSchemaSimpleTypeRestriction sr = new XmlSchemaSimpleTypeRestriction();
                    sr.BaseTypeName = new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema");
                    var values = Obj.DataContext.GetTable<NumberFieldDefinition>().Single(x => x.FieldDefinitionID == m.FieldDefinitionID && x.Status == (byte)StatusOfObject.Active);

                    if (values.Minimum != null)
                    {
                        XmlSchemaMinInclusiveFacet Min = new XmlSchemaMinInclusiveFacet();
                        Min.Value = values.Minimum.ToString();
                        sr.Facets.Add(Min);
                    }
                    if (values.Maximum != null)
                    {
                        XmlSchemaMaxInclusiveFacet max = new XmlSchemaMaxInclusiveFacet();
                        max.Value = values.Maximum.ToString();

                        sr.Facets.Add(max);
                    }


                    st.Content = sr;


                    FElement.SchemaType = st;
                }
                if ((FieldTypes)m.FieldType == FieldTypes.ChoiceField)
                {
                    //FElement.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

                    bool AllowMutiple = Obj.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == m.FieldDefinitionID && x.Status == (byte)StatusOfObject.Active).ChoiceGroup.AllowMultipleSelections;
                    if (AllowMutiple == false)
                    {
                        XmlSchemaSimpleType st = new XmlSchemaSimpleType();

                        XmlSchemaSimpleTypeRestriction sr = new XmlSchemaSimpleTypeRestriction();

                        sr.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

                        var ChoiceInGroups = Obj.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == m.FieldDefinitionID && x.Status == (byte)StatusOfObject.Active).ChoiceGroup.ChoiceInGroups.Where(z => z.Status == (byte)StatusOfObject.Active).Select(y => new { Choice = y.ChoiceText });
                        foreach (var Choice in ChoiceInGroups)
                        {
                            XmlSchemaEnumerationFacet se = new XmlSchemaEnumerationFacet();
                            se.Value = Choice.Choice;
                            sr.Facets.Add(se);
                        }




                        st.Content = sr;


                        FElement.SchemaType = st;
                    }
                    else
                    {
                        XmlSchemaComplexType sm = new XmlSchemaComplexType();
                        XmlSchemaChoice xsc = new XmlSchemaChoice();
                        sm.Particle = xsc;
                        xsc.MinOccurs = 0;
                        xsc.MaxOccursString = "unbounded";
                        XmlSchemaElement em = new XmlSchemaElement();
                        xsc.Items.Add(em);
                        em.Name = "Choice";

                        XmlSchemaSimpleType st = new XmlSchemaSimpleType();
                        XmlSchemaSimpleTypeRestriction sr = new XmlSchemaSimpleTypeRestriction();
                        sr.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

                        var ChoiceInGroups = Obj.DataContext.GetTable<ChoiceGroupFieldDefinition>().Single(x => x.FieldDefinitionID == m.FieldDefinitionID).ChoiceGroup.ChoiceInGroups.Where(x => true).Select(y => new { Choice = y.ChoiceText });
                        foreach (var Choice in ChoiceInGroups)
                        {
                            XmlSchemaEnumerationFacet se = new XmlSchemaEnumerationFacet();
                            se.Value = Choice.Choice;
                            sr.Facets.Add(se);
                        }
                        st.Content = sr;
                        em.SchemaType = st;
                        FElement.SchemaType = sm;
                    }
                }
                if ((FieldTypes)m.FieldType == FieldTypes.DateTimeField)
                {
                    FElement.SchemaTypeName = new System.Xml.XmlQualifiedName("dateTime", "http://www.w3.org/2001/XMLSchema");
                }
                if ((FieldTypes)m.FieldType == FieldTypes.AddressField)
                {
                    FElement.SchemaTypeName = new System.Xml.XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
                }
            }


            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            XmlTextWriter xw = new XmlTextWriter(fileLocation, System.Text.Encoding.UTF8);
            Schema.Write(xw, nsmgr);
            xw.Close();
        }


        public void PerformExport(string xmlFileName, IQueryable<TblRow> theTblRowsElement, bool includeValues)
        {
            string typeOfTblRow = GetTypeOfTblRowForTbl();

            XElement entireList = new XElement(typeOfTblRow + "list");

            foreach (var E in theTblRowsElement)
            {
                XElement theTblRowElement = new XElement(typeOfTblRow);
                XAttribute theActionOnImportAttribute = new XAttribute("ActionOnImport", "nofieldschange");
                XAttribute theTblRowIDAttribute = new XAttribute("TblRowID", E.TblRowID.ToString());
                XAttribute theInitialStatusAttribute = new XAttribute("InitialStatus", E.Status == (int)StatusOfObject.Active ? "active" : "inactive");
                XAttribute theStatusChangeAttribute = new XAttribute("StatusChange", "none");
                theTblRowElement.Add(theActionOnImportAttribute);
                theTblRowElement.Add(theTblRowIDAttribute);
                theTblRowElement.Add(theInitialStatusAttribute);
                theTblRowElement.Add(theStatusChangeAttribute);

                if (includeValues)
                {
                    var theTblVals = E.RatingGroups.SelectMany(x => x.Ratings).Where(x => x.RatingGroup.TblColumn.Status == (int)StatusOfObject.Active && x.CurrentValue != null).Select(x => new TblVal { TblCol = x.RatingGroup.TblColumn.Name, NumInGroup = x.NumInGroup, CurrentValue = (decimal) x.CurrentValue }).ToList();
                    string theTblValsString = JsonSerializer.Serialize<List<TblVal>>(theTblVals);
                    theTblRowElement.Add(new XElement("SerializedValues", theTblValsString));
                }

                theTblRowElement.Add(new XElement("TblRowName", E.Name));

                var theFields = DataAccess.R8RDB.GetTable<Field>().Where(f => f.TblRowID == E.TblRowID).OrderBy(f => f.FieldDefinition.FieldNum).ThenBy(f => f.FieldDefinition.FieldDefinitionID); // OK to order by ID to get consistent ordering
                foreach (var theField in theFields)
                {
                    XElement theFieldElement = null;
                    string theAbbreviatedFieldName = GetAbbreviatedFieldName(theField.FieldDefinitionID);
                    switch ((FieldTypes)theField.FieldDefinition.FieldType)
                    {
                        case FieldTypes.TextField:
                            var theTextField = theField.TextFields.SingleOrDefault(x => x.Status == (int)StatusOfObject.Active);
                            if (theTextField != null && theTextField.Text != "")
                            {
                                theFieldElement = new XElement(theAbbreviatedFieldName);
                                theFieldElement.Add(new XElement("Text", theTextField.Text));
                                theFieldElement.Add(new XElement("Link", theTextField.Link));
                            }
                            break;
                        case FieldTypes.NumberField:
                            var theNumberField = theField.NumberFields.SingleOrDefault(x => x.Status == (int)StatusOfObject.Active);
                            if (theNumberField != null && theNumberField.Number != null)
                            {
                                theFieldElement = new XElement(theAbbreviatedFieldName, theNumberField.Number.ToString());
                            }
                            break;
                        case FieldTypes.ChoiceField:
                            var theChoiceField = theField.ChoiceFields.SingleOrDefault(x => x.Status == (int)StatusOfObject.Active);
                            if (theChoiceField != null)
                            {
                                var choicesInFields = DataAccess.R8RDB.GetTable<ChoiceInField>().Where(cif => cif.ChoiceFieldID == theChoiceField.ChoiceFieldID && cif.Status == (int)StatusOfObject.Active);
                                var theChoiceGroupFieldDefinition = theChoiceField.Field.FieldDefinition.ChoiceGroupFieldDefinitions.Single();
                                if (theChoiceGroupFieldDefinition.ChoiceGroup.AllowMultipleSelections)
                                {
                                    if (choicesInFields.Any())
                                    {
                                        theFieldElement = new XElement(theAbbreviatedFieldName);
                                        foreach (var choiceInField in choicesInFields)
                                        {
                                            XElement theChoiceElement = new XElement("Choice", choiceInField.ChoiceInGroup.ChoiceText);
                                            theFieldElement.Add(theChoiceElement);
                                        }
                                    }
                                }
                                else
                                {
                                    if (choicesInFields.Count() > 1)
                                        throw new Exception("Multiple selection in single choice field.");
                                    if (choicesInFields.Any())
                                    {
                                        foreach (var choiceInField in choicesInFields)
                                        {
                                            theFieldElement = new XElement(theAbbreviatedFieldName, choiceInField.ChoiceInGroup.ChoiceText);
                                        }
                                    }
                                }

                            }
                            break;
                        case FieldTypes.DateTimeField:
                            var theDateTimeField = theField.DateTimeFields.SingleOrDefault(x => x.Status == (int)StatusOfObject.Active);
                            if (theDateTimeField != null && theDateTimeField.DateTime != null)
                            {
                                theFieldElement = new XElement(theAbbreviatedFieldName, theDateTimeField.DateTime);
                            }
                            break;
                        case FieldTypes.AddressField:
                            var theAddressField = theField.AddressFields.SingleOrDefault(x => x.Status == (int)StatusOfObject.Active);
                            if (theAddressField != null && theAddressField.AddressString != "")
                            {
                                theFieldElement = new XElement(theAbbreviatedFieldName, theAddressField.AddressString);
                            }
                            break;
                    }
                    if (theFieldElement != null)
                        theTblRowElement.Add(theFieldElement);
                }
                entireList.Add(theTblRowElement);
            }

            XDocument XTblRow = new XDocument(
              new XDeclaration("1.0", "utf-8", "yes"),
              new XComment("TblRow Information"), entireList);

            XTblRow.Save(xmlFileName);

        }

    }
}
