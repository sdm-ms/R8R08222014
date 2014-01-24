﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.ComponentModel;
using System.Data.Linq;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace ClassLibrary1.Misc
{
    public static class UseFasterSubmitChanges
    {
        public static bool setting = false;
        public static void Set(bool newSetting)
        {
            setting = newSetting;
        }
    }

    public static class RepositoryItemPrimaryKeys
    {
        internal static Dictionary<Type, PropertyInfo> dictionary = new Dictionary<Type, PropertyInfo>();

        internal static PropertyInfo GetPropertyInfoForPrimaryKeyField(object theItem)
        {
            Type theType = theItem.GetType();
            PropertyInfo thePrimaryKeyFieldPropertyInfo = null;
            if (dictionary.ContainsKey(theType))
                thePrimaryKeyFieldPropertyInfo = dictionary[theType];
            if (thePrimaryKeyFieldPropertyInfo != null)
                return thePrimaryKeyFieldPropertyInfo;
            PropertyInfo[] thePropertyInfos = theType.GetProperties();
            thePrimaryKeyFieldPropertyInfo = thePropertyInfos
                .FirstOrDefault(pi =>  
                    pi.GetCustomAttributes(false).OfType<System.Data.Linq.Mapping.ColumnAttribute>()
                        .Any(ca => 
                            ca.IsPrimaryKey == true &&
                            ca.IsDbGenerated == true));
            if (thePrimaryKeyFieldPropertyInfo == null)
                throw new Exception("Internal error: No primary key found with autogenerated values for " + theType.ToString());
            dictionary.Add(theType, thePrimaryKeyFieldPropertyInfo);
            return thePrimaryKeyFieldPropertyInfo;
        }

        public static int GetPrimaryKeyFieldValue(object theItem)
        {
            PropertyInfo thePrimaryKeyFieldPropertyInfo = GetPropertyInfoForPrimaryKeyField(theItem);
            return (int) thePrimaryKeyFieldPropertyInfo.GetValue(theItem, null);
        }

        public static void SetPrimaryKeyFieldValue(object theItem, int theValue)
        {
            PropertyInfo thePrimaryKeyFieldPropertyInfo = GetPropertyInfoForPrimaryKeyField(theItem);
            thePrimaryKeyFieldPropertyInfo.SetValue(theItem, theValue, null);
        }
    }

    public class RepositoryItemAssociationInfo
    {
        public Type TypeOfItemContainingProperty;
        public PropertyInfo Property;
        public PropertyInfo PropertyOfForeignKeyID;
        public string NameOfPropertyConnectingToForeignItem;
        public string NameOfPropertyWithForeignKeyID;
        public Type TypeOfForeignItem;
        public bool PropertyReturnsEntitySet;
        private MethodInfo _EntitySetRemoveMethod;
        private Type _SpecificEntitySetType;
        public Type SpecificEntitySetType
        {
            get
            {
                if (_SpecificEntitySetType == null)
                    SetEntitySetTypeAndRemoveMethod();
                return _SpecificEntitySetType;
            }
            set
            {
                _SpecificEntitySetType = value;
            }
        }
        public MethodInfo EntitySetRemoveMethod
        {
            get
            {
                if (_EntitySetRemoveMethod == null)
                    SetEntitySetTypeAndRemoveMethod();
                return _EntitySetRemoveMethod;
            }
            set
            {
                _EntitySetRemoveMethod = value;
            }
        }

        internal void SetEntitySetTypeAndRemoveMethod()
        {
            Type genericEntitySet = typeof(EntitySet<>);
            Type[] typeArgs = { TypeOfForeignItem };
            SpecificEntitySetType = genericEntitySet.MakeGenericType(typeArgs);
            EntitySetRemoveMethod = SpecificEntitySetType.GetMethods().Single(x => x.Name == "Remove");
        }

        public List<object> GetAllAssociatedObjects(object theRepositoryItem)
        {
            if (PropertyReturnsEntitySet)
            {
                IEnumerable theEntitySet = Property.GetValue(theRepositoryItem, null) as IEnumerable;
                List<object> theList = new List<object>();
                foreach (var item in theEntitySet)
                    theList.Add(item);
                return theList;
            }
            else
            {
                object theValue = Property.GetValue(theRepositoryItem, null);
                if (theValue == null)
                    return new List<object>() { };
                else
                    return new List<object>() { theValue };
            }
        }

        public int? GetForeignKeyID(object theRepositoryItem)
        {
            return (int?) PropertyOfForeignKeyID.GetValue(theRepositoryItem, null);
        }

        public void RemoveForeignItemFromProperty(object theRepositoryItem, object foreignItemToRemove)
        {
            if (PropertyReturnsEntitySet)
            {
                object theEntitySet = Property.GetValue(theRepositoryItem, null); // EntitySet<typeof(TypeOfForeignItem)>;
                if (foreignItemToRemove != null)
                    SpecificEntitySetType.InvokeMember("Remove", BindingFlags.Default | BindingFlags.InvokeMethod, null, theEntitySet, new object[] { foreignItemToRemove });
            }
            else if (PropertyOfForeignKeyID != null)
            {
                Property.SetValue(theRepositoryItem, null, null);
            }
            else
            { // don't want to set the EntityRef to null, want to set EntityRef<T>.Entity to null

                Property.SetValue(theRepositoryItem, null, null);
                //object theEntityRef = Property.GetValue(theRepositoryItem, null); // EntityRef<typeof(TypeOfForeignItem)>;
                //Type theType = theEntityRef.GetType();
                //PropertyInfo thePropertyInfo = theType.GetProperty("Entity");
                //thePropertyInfo.SetValue(theEntityRef, null, null);
            }
        }

        public void AddForeignItemToProperty(object theObject, object foreignItemToAdd)
        {
            if (PropertyReturnsEntitySet)
            {
                object theEntitySet = Property.GetValue(theObject, null); // EntitySet<typeof(TypeOfForeignItem)>;
                SpecificEntitySetType.InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, theEntitySet, new object[] { foreignItemToAdd });
            }
            else if (PropertyOfForeignKeyID != null)
            {
                Property.SetValue(theObject, foreignItemToAdd, null);
            }
            else
            { // don't want to set the EntityRef to null, want to set EntityRef<T>.Entity to null
                Property.SetValue(theObject, foreignItemToAdd, null);
                //object theEntityRef = Property.GetValue(theObject, null); // EntityRef<typeof(TypeOfForeignItem)>;
                //Type theType = theEntityRef.GetType();
                //PropertyInfo thePropertyInfo = theType.GetProperty("Entity");
                //thePropertyInfo.SetValue(theEntityRef, foreignItemToAdd, null);
            }
        }
        public override string ToString()
        {
            return String.Format("{0}: {1}.{2}={3}.<PrimaryId>", this.GetType().FullName,
                TypeOfItemContainingProperty.Name, NameOfPropertyWithForeignKeyID, NameOfPropertyConnectingToForeignItem );
        }
    }

    public static class MappingInfoProcessor
    {
        public static Dictionary<Type, List<RepositoryItemAssociationInfo>> mappingInfoForDataContexts = new Dictionary<Type, List<RepositoryItemAssociationInfo>>();

        public static List<RepositoryItemAssociationInfo> GetMappingInfoForDataContext(DataContext theDataContext)
        {
            Type theType = theDataContext.GetType();
            List<RepositoryItemAssociationInfo> theInfo;
            if (mappingInfoForDataContexts.ContainsKey(theType))
                theInfo = mappingInfoForDataContexts[theType];
            else
            {
                theInfo = ProcessDataContextMappingInfo(theDataContext);
                mappingInfoForDataContexts.Add(theType, theInfo);
            }
            return theInfo;
        }

        public static List<RepositoryItemAssociationInfo> ProcessDataContextMappingInfo(DataContext theDataContext)
        {
            var associations = from x in theDataContext.Mapping.GetTables()
                               let rowType = x.RowType
                               let dataMembers = rowType.DataMembers
                               from dataMember in dataMembers
                               let memberInfo = dataMember.Member
                               where memberInfo is System.Reflection.PropertyInfo
                               let propertyInfo = (PropertyInfo)memberInfo
                               let propertyInfoAssociationAttribute = (AssociationAttribute) propertyInfo.GetCustomAttributes(typeof(AssociationAttribute), false).FirstOrDefault()
                               let association = dataMember.Association
                               where dataMember.Association != null
                               let foreignKeyIDPropertyInfo = dataMembers.SingleOrDefault(y => propertyInfoAssociationAttribute != null && propertyInfoAssociationAttribute.ThisKey == y.Name && propertyInfoAssociationAttribute.IsForeignKey == true)
                               let foreignKeyIDProperty = (association.IsMany || foreignKeyIDPropertyInfo == null) ? null : (PropertyInfo)foreignKeyIDPropertyInfo.Member
                               select new RepositoryItemAssociationInfo
                               {
                                   TypeOfItemContainingProperty = rowType.Type,
                                   Property = propertyInfo,
                                   NameOfPropertyConnectingToForeignItem = dataMember.Name,
                                   PropertyOfForeignKeyID = foreignKeyIDProperty,
                                   NameOfPropertyWithForeignKeyID = foreignKeyIDProperty == null ? "" : foreignKeyIDProperty.Name,
                                   TypeOfForeignItem = association.OtherType.Type,
                                   PropertyReturnsEntitySet = association.IsMany
                               };
            return associations.ToList();
        }
    }

    public interface IInMemoryRepositorySubmitChangesActions
    {
        void CompleteInsertOnSubmitStep1();
        void CompleteInsertOnSubmitStep2();
        void CompleteDeleteOnSubmit();
        bool ItemIsNotInRepositoryOrIsSetToDelete(object item);
        bool ItemIsInRepositoryAndNotSetToDelete(object item);
        IInMemoryRepositorySubmitChangesActions CloneTo(InMemoryRepositoryList newOwner);
        object GetItemByID(int ID);
    }

    public class InMemoryRepository<T> : IInMemoryRepositorySubmitChangesActions, IRepository<T> where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        int maxPrimaryKeyID = 0;
        List<T> ListOfEntities;
        List<T> EntitiesBeingInserted;
        List<T> EntitiesBeingDeleted;
        public InMemoryRepositoryList Owner { get; set; }
        internal List<RepositoryItemAssociationInfo> _PropertiesForThisItemType;
        public List<RepositoryItemAssociationInfo> PropertiesForThisItemType
        {
            get
            {
                if (_PropertiesForThisItemType == null)
                    _PropertiesForThisItemType = Owner.MappingInfo.Where(x => x.TypeOfItemContainingProperty == typeof(T)).ToList();
                return _PropertiesForThisItemType;
            }
        }

        public InMemoryRepository(List<T> listOfEntities, InMemoryRepositoryList owner)
        {
            if (listOfEntities == null)
                ListOfEntities = new List<T>();
            else
                ListOfEntities = listOfEntities;
            if (ListOfEntities.Any())
                maxPrimaryKeyID = ListOfEntities.Max(x => RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x));
            EntitiesBeingDeleted = new List<T>();
            EntitiesBeingInserted = new List<T>();
            Owner = owner;
        }

        internal T2 CloneObject<T2>(T2 obj) where T2 : class
        {
            if (obj == null) return null;
            System.Reflection.MethodInfo inst = obj.GetType().GetMethod("MemberwiseClone",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (inst != null)
                return (T2)inst.Invoke(obj, null);
            else
                return null;
        }


        public IInMemoryRepositorySubmitChangesActions CloneTo(InMemoryRepositoryList newOwner)
        {
            List<T> newList = new List<T>();
            newList.AddRange(ListOfEntities); // cloning the objects isn't working right now, largely because the entityset connections are messed up .Select(x => CloneObject<T>(x)));
            return new InMemoryRepository<T>(newList, newOwner);
        }

        public Type ElementType { get { return ListOfEntities.AsQueryable().ElementType; } }
        public Expression Expression { get { return ListOfEntities.AsQueryable().Expression; } }
        public IQueryProvider Provider { get { return ListOfEntities.AsQueryable().Provider; } }

        public bool ItemIsNotInRepositoryOrIsSetToDelete(object item)
        {
            return !ListOfEntities.Contains(item) || EntitiesBeingDeleted.Contains(item);
        }

        public bool ItemIsInRepositoryAndNotSetToDelete(object item)
        {
            return ListOfEntities.Contains(item) && !EntitiesBeingDeleted.Contains(item);
        }

        public object GetItemByID(int ID)
        {
            return ListOfEntities.Single(x => RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x) == ID);
        }

        public void InsertOnSubmit(T theObject)
        {
            if (ListOfEntities.Contains(theObject))
                throw new Exception("Trying to insert an object that has already been inserted into the table.");
            ListOfEntities.Add(theObject);
            EntitiesBeingInserted.Add(theObject);
        }

        public void InsertOnSubmitIfNotAlreadyInserted(T theObject)
        {
            if (!ListOfEntities.Contains(theObject))
            {
                InsertOnSubmit(theObject);
            }
        }

        public void DeleteOnSubmit(T theObject)
        {
            if (ListOfEntities.Contains(theObject))
            {
                ListOfEntities.Remove(theObject);
                EntitiesBeingDeleted.Add(theObject);
            }
        }


        public T GetOriginalEntityState(T theObject)
        {
            if (EntitiesBeingInserted.Contains(theObject))
                return null;
            return theObject;
        }

        internal void SetUnsetPrimaryKey(T theItem)
        {
            maxPrimaryKeyID++;
            RepositoryItemPrimaryKeys.SetPrimaryKeyFieldValue(theItem, maxPrimaryKeyID);
        }

        internal void SetUnsetPrimaryKeys()
        {
            IEnumerable<T> theUnset = ListOfEntities.Where(x => RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x) == 0);
            foreach (var unset in theUnset)
                SetUnsetPrimaryKey(unset);
        }

        internal void SetPropertiesBasedOnForeignKeyIDAndViceVersa(T theItem)
        {
            // We need to check each property to see if the foreign key is set but the property is not
            // For example, if AddressField is created and FieldID = 35 but AddressFIeld.Field is not set to the object with 
            // primary key id == 35, then
            // we must set Field to that object.
            foreach (var property in PropertiesForThisItemType.Where(x => !x.PropertyReturnsEntitySet && x.PropertyOfForeignKeyID != null))
            {
                int? foreignKeyID = property.GetForeignKeyID(theItem);
                Type foreignItemType = property.TypeOfForeignItem;
                List<object> associatedObjects = property.GetAllAssociatedObjects(theItem);
                if (foreignKeyID != null && foreignKeyID != default(int))
                { // set property based on id
                    if (!associatedObjects.Any(x =>
                        // x.GetType() == foreignItemType &&
                        RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x) != foreignKeyID))
                    {
                        object foreignItem = Owner.GetItemByTypeAndID(foreignItemType, (int)foreignKeyID);
                        property.AddForeignItemToProperty(theItem, foreignItem);
                    }
                }
                else
                { // set foreign key id based on property (i.e., associated object's primary key id)
                    object associatedObject = associatedObjects.SingleOrDefault( /*x => x.GetType() == foreignItemType */);
                    if (associatedObject != null)
                    {
                        int primaryKeyFieldValue = RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(associatedObject);
                        if (primaryKeyFieldValue != foreignKeyID)
                        {
                            // we can't simply set the foreign key id, because since we've already set the property, we'll get an exception that the foreign key id has already been set.
                            // as a result, we need to reset the property.
                            property.RemoveForeignItemFromProperty(theItem, associatedObject);
                            property.AddForeignItemToProperty(theItem, associatedObject);
                            // doesn't work property.PropertyOfForeignKeyID.SetValue(theItem, primaryKeyFieldValue, null);
                        }
                    }
                }
            }
        }

        public void CompleteInsertOnSubmitStep1()
        {
            foreach (var item in EntitiesBeingInserted)
            {
                Owner.ConfirmNotAlreadyInAnotherDataContext(SimulatedPermanentStorage.originalInMemoryRepositoryList, item);
                if (!UseFasterSubmitChanges.setting)
                    ConfirmNoAssociationWithUninsertedItemOrItemSetToDelete(item); // note that we only need to check this on entities being inserted, because if there is some other kind of problem, it will be found on deletion
                if (RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(item) == default(int))
                    SetUnsetPrimaryKey(item); // we must set all primary keys for all repositories before setting the foreign key ids
            }
        }


        public void CompleteInsertOnSubmitStep2()
        {
            foreach (var item in EntitiesBeingInserted)
            {
                SetPropertiesBasedOnForeignKeyIDAndViceVersa(item);
            }
            EntitiesBeingInserted = new List<T>(); // clear out the list
        }

        internal List<object> GetAllAssociatedObjects(T item)
        {
            return PropertiesForThisItemType.SelectMany(x => x.GetAllAssociatedObjects(item)).ToList();
        }

        internal void RemoveAssociationsWithOtherItems(T item)
        {
            foreach (var property in PropertiesForThisItemType)
            {
                List<object> theAssociatedObjects = property.GetAllAssociatedObjects(item);
                foreach (var associatedObject in theAssociatedObjects)
                {
                    property.RemoveForeignItemFromProperty(item, associatedObject);
                }
            }
        }

        internal void ConfirmNoAssociationWithUninsertedItemOrItemSetToDelete(T item)
        {
            List<object> associatedObjects = GetAllAssociatedObjects(item);
            foreach (var associatedItem in associatedObjects)
                Owner.ConfirmNoAssociationWithUninsertedOrDeletedItem(associatedItem);
        }

        internal void ConfirmNoAssociationWithNondeletedItem(T item)
        {
            List<object> associatedObjects = GetAllAssociatedObjects(item);
            foreach (var associatedItem in associatedObjects)
                Owner.ConfirmNoAssociationWithNondeletedItem(associatedItem);
        }

        public void CompleteDeleteOnSubmit()
        {
            foreach (T item in EntitiesBeingDeleted)
            {
                RemoveAssociationsWithOtherItems(item);
                //ConfirmNoAssociationWithNondeletedItem(item); We now remove associations. E.g., ChangesGroup has a User. We want to delete the ChangesGroup, not the User.
            }
            EntitiesBeingDeleted = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ListOfEntities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ListOfEntities.GetEnumerator();
        }
    }

    public class InMemoryRepositoryList
    {
        public Dictionary<Type, IInMemoryRepositorySubmitChangesActions> inMemoryRepositoryDictionary;
        public List<IInMemoryRepositorySubmitChangesActions> inMemoryRepositoryList;
        public DataContext UnderlyingDataContext { get; set; }
        public List<RepositoryItemAssociationInfo> MappingInfo { get; set; }

        public InMemoryRepositoryList(DataContext underlyingDataContext)
        {
            UnderlyingDataContext = underlyingDataContext;
            MappingInfo = MappingInfoProcessor.GetMappingInfoForDataContext(underlyingDataContext);
            inMemoryRepositoryDictionary = new Dictionary<Type, IInMemoryRepositorySubmitChangesActions>();
            inMemoryRepositoryList = new List<IInMemoryRepositorySubmitChangesActions>();
        }

        public InMemoryRepositoryList(DataContext underlyingDataContext, InMemoryRepositoryList listToClone)
        {
            UnderlyingDataContext = underlyingDataContext;
            MappingInfo = MappingInfoProcessor.GetMappingInfoForDataContext(underlyingDataContext);
            CloneFrom(listToClone);
        }

        public void CloneFrom(InMemoryRepositoryList listToClone)
        {
            inMemoryRepositoryDictionary = new Dictionary<Type, IInMemoryRepositorySubmitChangesActions>();
            inMemoryRepositoryList = new List<IInMemoryRepositorySubmitChangesActions>();
            foreach (var entry in listToClone.inMemoryRepositoryDictionary)
            {
                IInMemoryRepositorySubmitChangesActions theClone = entry.Value.CloneTo(this);
                inMemoryRepositoryDictionary.Add(entry.Key, theClone);
                inMemoryRepositoryList.Add(theClone);
            }
        }

        public IRepository<T> GetRepository<T>() where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            Type theType = typeof(T);
            IInMemoryRepositorySubmitChangesActions item;
            if (inMemoryRepositoryDictionary.ContainsKey(theType))
            {
                item = inMemoryRepositoryDictionary[theType];
                return item as InMemoryRepository<T>;
            }
            else
            {
                InMemoryRepository<T> newRepository = new InMemoryRepository<T>(null,this);
                inMemoryRepositoryDictionary.Add(theType, newRepository);
                inMemoryRepositoryList.Add(newRepository);
                return newRepository;
            }
        }

        public List<IInMemoryRepositorySubmitChangesActions> GetRepositories()
        {
            return inMemoryRepositoryList;
        }

        public void CleanUpBeforeSubmittingChanges()
        {
            var count = inMemoryRepositoryList.Count();
            for (int i = 0; i < count; i++)
            {
                inMemoryRepositoryList[i].CompleteDeleteOnSubmit();
            }
            for (int i = 0; i < count; i++)
            {
                inMemoryRepositoryList[i].CompleteInsertOnSubmitStep1();
            }
            for (int i = 0; i < count; i++)
            {
                inMemoryRepositoryList[i].CompleteInsertOnSubmitStep2();
            }
        }

        public IInMemoryRepositorySubmitChangesActions GetRepositoryForItem(object item)
        {
            Type theType = item.GetType();
            if (!inMemoryRepositoryDictionary.ContainsKey(theType))
                return null;
            IInMemoryRepositorySubmitChangesActions repository = inMemoryRepositoryDictionary[theType];
            return repository;
        }

        public object GetItemByTypeAndID(Type type, int ID)
        {
            IInMemoryRepositorySubmitChangesActions repository = inMemoryRepositoryDictionary[type];
            return repository.GetItemByID(ID);
        }

        public void ConfirmNotAlreadyInAnotherDataContext(Dictionary<object, InMemoryRepositoryList> originalDataContexts, object item)
        {
            if (originalDataContexts.ContainsKey(item))
            {
                if (originalDataContexts[item] != this)
                    throw new Exception("Trying to attach an item from another DataContext.");
            }
            else
                originalDataContexts.Add(item, this);
        }

        public void ConfirmNoAssociationWithUninsertedOrDeletedItem(object item)
        {
            var repo = GetRepositoryForItem(item);
            if (repo == null || repo.ItemIsNotInRepositoryOrIsSetToDelete(item))
                throw new Exception("Trying to submit datacontext with association to item not in datacontext.");
        }

        public void ConfirmNoAssociationWithNondeletedItem(object item)
        {
            var repo = GetRepositoryForItem(item);
            if (repo == null || repo.ItemIsInRepositoryAndNotSetToDelete(item))
                throw new Exception("Foreign key constraint violated on item being deleted from the DataContext.");
        }
    }

    public static class SimulatedPermanentStorage
    {
        public static Dictionary<Type, InMemoryRepositoryList> inMemoryRepositoryDictionary;
        public static Dictionary<object, InMemoryRepositoryList> originalInMemoryRepositoryList = new Dictionary<object, InMemoryRepositoryList>();

        public static InMemoryRepositoryList GetSimulatedPermanentStorageForDataContextType(DataContext underlyingDataContext)
        {
            Type theType = underlyingDataContext.GetType();

            if (inMemoryRepositoryDictionary == null)
                inMemoryRepositoryDictionary = new Dictionary<Type, InMemoryRepositoryList>();

            if (inMemoryRepositoryDictionary.ContainsKey(theType))
                return inMemoryRepositoryDictionary[theType];
            else
            {
                InMemoryRepositoryList newPermanentStorage = new InMemoryRepositoryList(underlyingDataContext);
                inMemoryRepositoryDictionary.Add(theType, newPermanentStorage);
                return newPermanentStorage;
            }
        }

        public static void SetSimulatedPermanentStorageForDataContextType(DataContext underlyingDataContext, InMemoryRepositoryList inMemoryRepositoryList)
        { // This is a faster way of doing submit changes. 
            Type theType = underlyingDataContext.GetType();

            if (inMemoryRepositoryDictionary == null)
                inMemoryRepositoryDictionary = new Dictionary<Type, InMemoryRepositoryList>();

            if (inMemoryRepositoryDictionary.ContainsKey(theType))
                inMemoryRepositoryDictionary[theType] = inMemoryRepositoryList;
            else
                inMemoryRepositoryDictionary.Add(theType, inMemoryRepositoryList);
        }

        public static void Reset()
        {
            inMemoryRepositoryDictionary = null;
            originalInMemoryRepositoryList = new Dictionary<object, InMemoryRepositoryList>();
        }
    }


    public class InMemoryContext : IDataContext
    {
        public InMemoryRepositoryList inMemoryRepositories;

        public DataContext UnderlyingDataContext { get; set; }

        public InMemoryContext(DataContext underlyingDataContext)
        {
            UnderlyingDataContext = underlyingDataContext;
            Type theType = underlyingDataContext.GetType();
            LoadFromPermanentStorage();
        }

        public void Reset()
        {
            // LoadFromPermanentStorage() doesn't seem to help and is a bit time consuming.
        }

        internal void LoadFromPermanentStorage()
        {
            if (UseFasterSubmitChanges.setting)
                inMemoryRepositories = SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext);
            else
                inMemoryRepositories = new InMemoryRepositoryList(UnderlyingDataContext, SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext));
        }

        public IRepository<T> GetTable<T>() where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            return inMemoryRepositories.GetRepository<T>();
        }

        public virtual void SubmitChanges(System.Data.Linq.ConflictMode conflictMode)
        {
            SubmitChanges();
        }

        public virtual void SubmitChanges()
        {
            inMemoryRepositories.CleanUpBeforeSubmittingChanges();
            if (inMemoryRepositories != null && UseFasterSubmitChanges.setting)
                SimulatedPermanentStorage.SetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext, inMemoryRepositories);
            else
            {
                InMemoryRepositoryList permanentStorage = SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext);
                permanentStorage.CloneFrom(inMemoryRepositories);
            }
        }
    }
}