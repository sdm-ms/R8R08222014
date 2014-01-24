﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using ClassLibrary1.Misc;
using ClassLibrary1.Model;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace TestProject1
{
    [TestClass]
    public class TestDataContextRepositoriesInMemory
    {
        internal const string raterooConnectionString = "Data Source=PC2012;Initial Catalog=Rateroo7;Integrated Security=True;Connect Timeout=300";
        internal RaterooDataContext UnderlyingRaterooDataContext;
        internal List<RepositoryItemAssociationInfo> repositoryList;
        internal RepositoryItemAssociationInfo fieldToAddressField;
        internal RepositoryItemAssociationInfo addressFieldToField;
        internal RepositoryItemAssociationInfo userToUserInfo;
        internal List<RepositoryItemAssociationInfo> ratingToRatingGroup;

        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [TestInitialize()]
        public void Initialize()
        {
            GetIRaterooDataContext.UseRealDatabase = false; // never use real database for these tests, since the purpose is to test the alternative to the real database
            UnderlyingRaterooDataContext = new RaterooDataContext(raterooConnectionString); 
            repositoryList = MappingInfoProcessor.ProcessDataContextMappingInfo(UnderlyingRaterooDataContext);
            fieldToAddressField = repositoryList.SingleOrDefault(x => x.TypeOfItemContainingProperty == typeof(Field) && x.TypeOfForeignItem == typeof(AddressField));
            addressFieldToField = repositoryList.SingleOrDefault(x => x.TypeOfItemContainingProperty == typeof(AddressField) && x.TypeOfForeignItem == typeof(Field));
            userToUserInfo = repositoryList.SingleOrDefault(x => x.TypeOfItemContainingProperty == typeof(User) && x.TypeOfForeignItem == typeof(UserInfo));
            ratingToRatingGroup = repositoryList.Where(x => x.TypeOfItemContainingProperty == typeof(Rating) && x.TypeOfForeignItem == typeof(RatingGroup)).ToList();
        }

        /// <summary>
        ///Cleanup() is called once during test execution after
        ///test methods in this class have executed unless
        ///this test class' Initialize() method throws an exception.
        ///</summary>
        [TestCleanup()]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void RepositoryItem_CanGetAndSetPrimaryKey()
        {
            User theUser = new User();
            int primaryKey = RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(theUser);
            primaryKey.Should().Be(0);
            RepositoryItemPrimaryKeys.SetPrimaryKeyFieldValue(theUser, 5);
            primaryKey = RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(theUser);
            primaryKey.Should().Be(5);
        }

        [TestMethod]
        public void RepositoryItem_CanGetAndSetPrimaryKey_ForUserAction()
        {
            UserAction theUserAction = new UserAction();
            int primaryKey = RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(theUserAction);
            primaryKey.Should().Be(0);
            RepositoryItemPrimaryKeys.SetPrimaryKeyFieldValue(theUserAction, 5);
            primaryKey = RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(theUserAction);
            primaryKey.Should().Be(5);
        }
        [TestMethod]
        public void MappingInfoProcessor_ReturnsCorrectResults_ForOneToMany()
        {
            repositoryList.Should().NotBeNull("because there should be exactly one association from AddressField to Field");
            fieldToAddressField.Should().NotBeNull("because there should be exactly one association from Field to AddressField");
            fieldToAddressField.PropertyReturnsEntitySet.Should().BeTrue("because in theory multiple AddressFields can point to the same field, even though we do not actually do this");
            fieldToAddressField.PropertyOfForeignKeyID.Should().BeNull("because there is no AddressFieldID in the Field table");
            fieldToAddressField.NameOfPropertyWithForeignKeyID.Should().Be("", "because there is no AddressFieldID in the Field table");
            fieldToAddressField.Property.Name.Should().Be("AddressFields");
            fieldToAddressField.NameOfPropertyConnectingToForeignItem.Should().Be("AddressFields");
        }

        [TestMethod]
        public void MappingInfoProcessor_ReturnsCorrectResults_ForOneToOneWithForeignKeyID()
        {
            addressFieldToField.PropertyReturnsEntitySet.Should().BeFalse("because an AddressFIeld can point to only one Field");
            addressFieldToField.PropertyOfForeignKeyID.Name.Should().Be("FieldID");
            addressFieldToField.NameOfPropertyWithForeignKeyID.Should().Be("FieldID");
            addressFieldToField.Property.Name.Should().Be("Field");
            addressFieldToField.NameOfPropertyConnectingToForeignItem.Should().Be("Field");
        }

        [TestMethod]
        public void MappingInfoProcessor_ReturnsCorrectResults_ForOneToOneWithoutForeignKeyID()
        {
            userToUserInfo.Should().NotBeNull("because there should be exactly one association from User to UserInfo");
            userToUserInfo.PropertyReturnsEntitySet.Should().BeFalse("because the data model explicitly specifies that only one UserInfo can point to the same User field");
            userToUserInfo.PropertyOfForeignKeyID.Should().BeNull("because there is no UserInfoID in the User table");
            userToUserInfo.NameOfPropertyWithForeignKeyID.Should().Be("", "because there is no UserInfoID in the Field table");
            userToUserInfo.Property.Name.Should().Be("UserInfo");
            userToUserInfo.NameOfPropertyConnectingToForeignItem.Should().Be("UserInfo");
        }


        [TestMethod]
        public void MappingInfoProcessor_ReturnsCorrectResults_ForMultipleOneToManyPropertiesBetweenSameTypes()
        {
            int ratingToRatingGroupCount = ratingToRatingGroup.Count();
            ratingToRatingGroupCount.Should().Be(3);
            RepositoryItemAssociationInfo ratingToRatingGroupMainProperty = ratingToRatingGroup.Single(x => x.NameOfPropertyWithForeignKeyID == "RatingGroupID");
            ratingToRatingGroupMainProperty.NameOfPropertyConnectingToForeignItem.Should().Be("RatingGroup");
            RepositoryItemAssociationInfo ratingToRatingGroupTopmostProperty = ratingToRatingGroup.Single(x => x.NameOfPropertyWithForeignKeyID == "TopmostRatingGroupID");
            ratingToRatingGroupTopmostProperty.NameOfPropertyConnectingToForeignItem.Should().Be("RatingGroup2");
            RepositoryItemAssociationInfo ratingToRatingGroupOwnedProperty = ratingToRatingGroup.Single(x => x.NameOfPropertyWithForeignKeyID == "OwnedRatingGroupID");
            ratingToRatingGroupOwnedProperty.NameOfPropertyConnectingToForeignItem.Should().Be("RatingGroup1");
        }

        [TestMethod]
        public void RepositoryItemAssociationInfo_GetAllAssociatedObjects()
        {
            Field theField = new Field();
            fieldToAddressField.GetAllAssociatedObjects(theField).Count().Should().Be(0);
            AddressField theAddressField = new AddressField() { Field = theField };
            AddressField anotherAddressField = new AddressField() { Field = theField };
            theField.AddressFields.Contains(theAddressField).Should().BeTrue();
            theField.AddressFields.Contains(anotherAddressField).Should().BeTrue();
            theAddressField.Field.Should().Equals(theField);
            fieldToAddressField.GetAllAssociatedObjects(theField).Contains(theAddressField).Should().BeTrue();
            fieldToAddressField.GetAllAssociatedObjects(theField).Contains(anotherAddressField).Should().BeTrue();
        }

        [TestMethod]
        public void RepositoryItemAssociationInfo_GetForeignKeyID()
        {
            AddressField theAddressField = new AddressField();
            theAddressField.FieldID = 3;
            addressFieldToField.GetForeignKeyID(theAddressField).Should().Be(3);
        }

        [TestMethod]
        public void RepositoryItemAssociationInfo_AddAndRemoveForeignItem_ForOneToOneWithForeignKeyID()
        {
            AddressField theAddressField = new AddressField();
            Field theField = new Field();
            addressFieldToField.AddForeignItemToProperty(theAddressField, theField);
            theAddressField.Field.Should().Equals(theField);
            theField.AddressFields.Contains(theAddressField).Should().BeTrue();
            addressFieldToField.RemoveForeignItemFromProperty(theAddressField, theField);
            theAddressField.Field.Should().BeNull();
            theField.AddressFields.Contains(theAddressField).Should().BeFalse();
        }

        [TestMethod]
        public void RepositoryItemAssociationInfo_AddAndRemoveForeignItem_ForOneToMany()
        {
            AddressField theAddressField = new AddressField();
            Field theField = new Field();
            fieldToAddressField.AddForeignItemToProperty(theField, theAddressField);
            theAddressField.Field.Should().Equals(theField);
            theField.AddressFields.Contains(theAddressField).Should().BeTrue();
            fieldToAddressField.RemoveForeignItemFromProperty(theField, theAddressField);
            theAddressField.Field.Should().BeNull();
            theField.AddressFields.Contains(theAddressField).Should().BeFalse();
        }

        [TestMethod]
        public void RepositoryItemAssociationInfo_AddAndRemoveForeignItem_ForOneToOneWithoutForeignKeyID()
        {
            User theUser = new User();
            UserInfo theUserInfo = new UserInfo();
            userToUserInfo.AddForeignItemToProperty(theUser, theUserInfo);
            theUser.UserInfo.Should().Equals(theUserInfo);
            theUserInfo.User.Should().Equals(theUser);
            userToUserInfo.RemoveForeignItemFromProperty(theUser, theUserInfo);
            theUser.UserInfo.Should().BeNull();
            theUserInfo.User.Should().BeNull();
        }

        static InMemoryRepositoryList repoList;
        static InMemoryRepository<AddressField> theInMemoryRepositoryAddressField;
        static InMemoryRepository<Field> theInMemoryRepositoryField;

        public void InitializeInMemoryRepositoryList()
        {
            repoList = new InMemoryRepositoryList(UnderlyingRaterooDataContext);
            theInMemoryRepositoryAddressField = repoList.GetRepository<AddressField>() as InMemoryRepository<AddressField>;
            theInMemoryRepositoryField = repoList.GetRepository<Field>() as InMemoryRepository<Field>;
        }

        [TestMethod]
        public void InMemoryRepositoryList_GetRepository()
        {
            InitializeInMemoryRepositoryList();
            theInMemoryRepositoryAddressField.Should().NotBeNull();
            // theInMemoryRepository.Should().BeOfType<InMemoryRepository<AddressField>>();
        }

        [TestMethod]
        public void InMemoryRepository_InsertOnSubmit()
        {
            InitializeInMemoryRepositoryList();
            AddressField theAddressField = new AddressField();
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryAddressField.ItemIsInRepositoryAndNotSetToDelete(theAddressField).Should().BeTrue();
        }

        [TestMethod]
        public void InMemoryRepository_DeleteOnSubmit()
        {
            InitializeInMemoryRepositoryList();
            AddressField theAddressField = new AddressField();
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryAddressField.ItemIsInRepositoryAndNotSetToDelete(theAddressField).Should().BeTrue();
            theInMemoryRepositoryAddressField.DeleteOnSubmit(theAddressField);
            theInMemoryRepositoryAddressField.ItemIsInRepositoryAndNotSetToDelete(theAddressField).Should().BeFalse();
        }

        [TestMethod]
        public void InMemoryRepository_GetItemByID()
        {
            InitializeInMemoryRepositoryList();
            AddressField theAddressField = new AddressField();
            theAddressField.AddressFieldID = 5;
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryAddressField.GetItemByID(5).Should().Equals(theAddressField);
        }

        [TestMethod]
        public void InMemoryRepository_SetUnsetPrimaryKey()
        {
            InitializeInMemoryRepositoryList();
            AddressField theAddressField = new AddressField();
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryAddressField.SetUnsetPrimaryKey(theAddressField);
            theInMemoryRepositoryAddressField.GetItemByID(1).Should().Equals(theAddressField);
        }

        // This isn't working right now -- but might have better luck after upgrading to VS2013 Ultimate.
        //[TestMethod]
        //public void InMemoryRepository_SetPropertiesBasedOnForeignKeyID()
        //{
        //    InitializeInMemoryRepositoryList();
        //    AddressField theAddressField = new AddressField();
        //    Field theField = new Field();
        //    theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
        //    RepositoryItemPrimaryKeys.SetPrimaryKeyFieldValue(theField, 5);
        //    theAddressField.FieldID = 5;
        //    using (ShimsContext.Create())
        //    {
        //        ClassLibrary1.Misc.Fakes.ShimInMemoryRepositoryList.AllInstances.GetItemByTypeAndIDTypeInt32 = (InMemoryRepositoryList l, Type t, int i) =>
        //         {
        //             t.Should().Be(typeof(Field));
        //             i.Should().Be(5);
        //             return theField;
        //         };

        //        theInMemoryRepositoryAddressField.SetPropertiesBasedOnForeignKeyIDAndViceVersa(theAddressField);
        //    }

        //    theAddressField.Field.Should().Equals(theField);
        //}

        [TestMethod]
        public void InMemoryRepository_CompleteInsertOnSubmit_MakingAssociationWithID()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            theField.FieldID = 5;
            AddressField theAddressField = new AddressField() { FieldID = 5 };
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep2();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep2();
            theAddressField.Field.Should().Equals(theField);
        }

        [TestMethod]
        public void InMemoryRepository_CompleteInsertOnSubmit_MakingAssociationWithProperty_WherePrimaryKeyIsSetBefore()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            theField.FieldID = 5;
            AddressField theAddressField = new AddressField() { Field = theField };
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep2();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep2();
            theField.FieldID.Should().Be(5);
            theAddressField.FieldID.Should().Be(5);
        }


        [TestMethod]
        public void InMemoryRepository_CompleteInsertOnSubmit_MakingAssociationWithProperty_WherePrimaryKeyIsSetAfterAssociationCreation()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            AddressField theAddressField = new AddressField() { Field = theField }; // automatically sets AddressField.FieldID, but that's not set yet
            theField.FieldID = 5; 
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep2();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep2();
            theAddressField.FieldID.Should().Be(5);
        }


        [TestMethod]
        public void InMemoryRepository_CompleteInsertOnSubmit_MakingAssociationWithProperty_WherePrimaryKeyIsSetOnCompletingInsertOnSubmit()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            AddressField theAddressField = new AddressField() { Field = theField }; // automatically sets AddressField.FieldID, but that's not set yet
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep1();
            theField.FieldID.Should().Be(1);
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep2();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep2();
            theAddressField.FieldID.Should().Be(1);
        }

        [TestMethod]
        public void InMemoryRepository_CompleteInsertOnSubmit_MakingAssociationWithProperty_WherePrimaryKeyIsSetIndirectly()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            AddressField theAddressField = new AddressField() { Field = theField }; // automatically sets AddressField.FieldID, but that's not set yet
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryAddressField.CompleteInsertOnSubmitStep2();
            theInMemoryRepositoryField.CompleteInsertOnSubmitStep2();
            theAddressField.FieldID.Should().Be(theField.FieldID);
        }



        [TestMethod]
        public void RepositoryItemAssociationInfo_CompleteInsertOnSubmit_WhereMultipleSimilarPropertiesExist()
        {
            var repoList2 = new InMemoryRepositoryList(UnderlyingRaterooDataContext);
            var theInMemoryRepositoryRating = repoList2.GetRepository<Rating>() as InMemoryRepository<Rating>;
            var theInMemoryRepositoryRatingGroup = repoList2.GetRepository<RatingGroup>() as InMemoryRepository<RatingGroup>;

            //RepositoryItemAssociationInfo ratingToRatingGroupMainProperty = ratingToRatingGroup.Single(x => x.NameOfPropertyWithForeignKeyID == "RatingGroupID");
            //RepositoryItemAssociationInfo ratingToRatingGroupTopmostProperty = ratingToRatingGroup.Single(x => x.NameOfPropertyWithForeignKeyID == "TopmostRatingGroupID"); // RatingGroup2 
            //RepositoryItemAssociationInfo ratingToRatingGroupOwnedProperty = ratingToRatingGroup.Single(x => x.NameOfPropertyWithForeignKeyID == "OwnedRatingGroupID"); // RatingGroup1
            RatingGroup theRatingGroup = new RatingGroup();
            Rating theRating = new Rating() { RatingGroup = theRatingGroup };

            theInMemoryRepositoryRating.InsertOnSubmit(theRating);
            theInMemoryRepositoryRatingGroup.InsertOnSubmit(theRatingGroup);

            theInMemoryRepositoryRating.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryRatingGroup.CompleteInsertOnSubmitStep1();
            theInMemoryRepositoryRating.CompleteInsertOnSubmitStep2();
            theInMemoryRepositoryRatingGroup.CompleteInsertOnSubmitStep2();

            theRating.RatingGroup.Should().Equals(theRatingGroup);
            theRating.RatingGroup2.Should().BeNull();
            theRating.RatingGroup1.Should().BeNull();
        }

        //[TestMethod]
        //public void RepositoryItemAssociationInfo_CompleteInsertOnSubmit_WhereMultipleSimilarPropertiesExist2()
        //{
        //    var repoList2 = new InMemoryRepositoryList(UnderlyingRaterooDataContext);
        //    var theInMemoryRepositoryRatingPlan = repoList2.GetRepository<RatingPlan>() as InMemoryRepository<RatingPlan>;
        //    var theInMemoryRepositoryRatingGroupAttributes = repoList2.GetRepository<RatingGroupAttribute>() as InMemoryRepository<RatingGroupAttribute>;

        //    RatingGroupAttribute theRGA = new RatingGroupAttribute();
        //    RatingPlan theRatingPlan = new RatingPlan() { RatingGroupAttribute = theRGA };

        //    theInMemoryRepositoryRatingPlan.InsertOnSubmit(theRating);
        //    theInMemoryRepositoryRatingGroupAttributes.InsertOnSubmit(theRatingGroup);

        //    theRating.RatingGroup.Should().Equals(theRatingGroup);
        //    theRating.RatingGroup2.Should().BeNull();
        //    theRating.RatingGroup1.Should().BeNull();
        //}

        [TestMethod]
        public void InMemoryRepository_ConfirmNoAssociationWithUninsertedItemOrItemSetToDelete()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            AddressField theAddressField = new AddressField() { Field = theField };
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            Action act = () => theInMemoryRepositoryAddressField.ConfirmNoAssociationWithUninsertedItemOrItemSetToDelete(theAddressField);
            act.ShouldThrow<Exception>("because theField hasn't been inserted yet");
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            act.ShouldNotThrow();
            theInMemoryRepositoryField.DeleteOnSubmit(theField);
            act.ShouldThrow<Exception>("because theField has now been set for deletion");
        }

        [TestMethod]
        public void InMemoryRepository_ConfirmNoAssociationWithNondeletedItem()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            AddressField theAddressField = new AddressField() { Field = theField };
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            Action act = () => theInMemoryRepositoryAddressField.ConfirmNoAssociationWithNondeletedItem(theAddressField);
            act.ShouldThrow<Exception>("because theField is in the datacontext (addressfield isn't)");
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            theInMemoryRepositoryAddressField.DeleteOnSubmit(theAddressField);
            act.ShouldThrow<Exception>("because theField is in the datacontext (addressfield isn't)");
        }


        [TestMethod]
        public void InMemoryRepository_CompleteDeleteOnSubmit()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            AddressField theAddressField = new AddressField() { Field = theField };
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            repoList.CleanUpBeforeSubmittingChanges();
            theInMemoryRepositoryAddressField.DeleteOnSubmit(theAddressField);
            Action act2 = () => theInMemoryRepositoryAddressField.CompleteDeleteOnSubmit();
            act2.ShouldNotThrow("because CompleteDelete will disassociate the properties");
            theField.AddressFields.Contains(theAddressField).Should().BeFalse();
        }

        [TestMethod]
        public void InMemoryRepositoryListTests()
        {
            InitializeInMemoryRepositoryList();
            Field theField = new Field();
            AddressField theAddressField = new AddressField() { Field = theField };
            theInMemoryRepositoryField.InsertOnSubmit(theField);
            theInMemoryRepositoryAddressField.InsertOnSubmit(theAddressField);
            Field theField2 = new Field();
            theInMemoryRepositoryField.InsertOnSubmit(theField2);
            repoList.CleanUpBeforeSubmittingChanges();

            theField.FieldID.Should().Be(1);
            theField2.FieldID.Should().Be(2);
            theAddressField.AddressFieldID.Should().Be(1);
            theAddressField.FieldID.Should().Be(1);
            theInMemoryRepositoryAddressField.GetItemByID(1).Should().Equals(theAddressField);
            theInMemoryRepositoryField.GetItemByID(1).Should().Equals(theField);

            repoList.GetRepositories().Count().Should().Be(2);

            InMemoryRepositoryList repoList2 = new InMemoryRepositoryList(UnderlyingRaterooDataContext, repoList);
            InMemoryRepository<AddressField> repoAddress2 = repoList2.GetRepository<AddressField>() as InMemoryRepository<AddressField>;
            InMemoryRepository<Field> repoField2 = repoList2.GetRepository<Field>() as InMemoryRepository<Field>;

            repoAddress2.Owner.Should().Equals(repoList2);
            repoField2.Owner.Should().Equals(repoAddress2);

            repoAddress2.GetItemByID(1).Should().Equals(theAddressField);
            repoField2.GetItemByID(1).Should().Equals(theField);
            repoField2.GetItemByID(2).Should().Equals(theField2);

            repoList.GetItemByTypeAndID(typeof(Field), 2).Should().Equals(theField2);

        }

        [TestMethod]
        public void SimulatedPermanentStorageTest()
        {
            var thePermanentStorage = SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingRaterooDataContext);
            thePermanentStorage.Should().NotBeNull();
            SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingRaterooDataContext).Should().Equals(thePermanentStorage);
            SimulatedPermanentStorage.Reset();
            SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingRaterooDataContext).Should().NotBe(thePermanentStorage);
        }

        [TestMethod]
        public void InMemoryContextTest()
        {
            InMemoryContext theInMemoryContext = new InMemoryContext(UnderlyingRaterooDataContext);
            IRepository<AddressField> theRepository = theInMemoryContext.GetTable<AddressField>();
            AddressField theAddressField = new AddressField();
            theRepository.InsertAllOnSubmit(new AddressField[] { theAddressField}); // test insert all at same time
            theInMemoryContext.SubmitChanges();
            var thePermanentStorage = SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingRaterooDataContext);
            thePermanentStorage.GetItemByTypeAndID(typeof(AddressField), 1).Should().Equals(theAddressField);
        }


    }
}