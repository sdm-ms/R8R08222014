using System;
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

        public static object[] unsetPrimaryKeyValues = new object[] { -1, default(int) /* 0 */, null, default(Guid) }; // this assumes that any primary key value that is -1 or 0 has not yet been set by the database, because when creating an object, its primary key is default(int), i.e. 0 (even though SQL Server will also use this as a primary key value), unless it is set to -1 (which is useful as a way of distinguishing genuine 0 primary keys from objects that have not yet been assigned primary keys).

        internal static Dictionary<Type, PropertyInfo> foreignKeyForEntityType = new Dictionary<Type, PropertyInfo>();

        /// <summary>
        /// Returns the type of the primary key for this entity. This assumes that the entity has a non-composite primary key.
        /// If there is not a single unique primary key field, it will throw an exception.
        /// </summary>
        /// <param name="entity">The entity, which must be a Linq-to-SQL entity.</param>
        /// <returns>The type of the primary key.</returns>
        internal static PropertyInfo GetPropertyInfoForPrimaryKeyField(object entity)
        {
            Type theType = entity.GetType();
            PropertyInfo thePrimaryKeyFieldPropertyInfo = null;
            if (foreignKeyForEntityType.ContainsKey(theType))
                thePrimaryKeyFieldPropertyInfo = foreignKeyForEntityType[theType];
            if (thePrimaryKeyFieldPropertyInfo != null)
                return thePrimaryKeyFieldPropertyInfo;
            PropertyInfo[] thePropertyInfos = theType.GetProperties();
            thePrimaryKeyFieldPropertyInfo = thePropertyInfos
                .FirstOrDefault(pi =>
                    pi.GetCustomAttributes(false).OfType<System.Data.Linq.Mapping.ColumnAttribute>()
                        .Any(ca =>
                            ca.IsPrimaryKey == true));
            if (thePrimaryKeyFieldPropertyInfo == null)
                throw new Exception("Internal error: No primary key for " + theType.ToString());
            foreignKeyForEntityType.Add(theType, thePrimaryKeyFieldPropertyInfo);
            return thePrimaryKeyFieldPropertyInfo;
        }

        /// <summary>
        /// Returns true if the primary key field is int.
        /// </summary>
        /// <param name="theItem"></param>
        /// <returns></returns>
        internal static bool PrimaryKeyFieldIsInt(object theItem)
        {
            return GetPropertyInfoForPrimaryKeyField(theItem).PropertyType == typeof(int);
        }

        /// <summary>
        /// Returns true if the primary key field is GUID.
        /// </summary>
        /// <param name="theItem"></param>
        /// <returns></returns>
        internal static bool PrimaryKeyFieldIsGUID(object theItem)
        {
            return GetPropertyInfoForPrimaryKeyField(theItem).PropertyType == typeof(Guid);
        }

        /// <summary>
        /// Gets the primary key field value for an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static object GetPrimaryKeyFieldValue(object entity)
        {
            PropertyInfo thePrimaryKeyFieldPropertyInfo = GetPropertyInfoForPrimaryKeyField(entity);
            return thePrimaryKeyFieldPropertyInfo.GetValue(entity, null);
        }

        /// <summary>
        /// Sets the primary key field to a particular value. This is called when an item is to be added to the database, and its primary key has not yet been set.
        /// </summary>
        /// <param name="entity">The entity whose primary key needs to be set.</param>
        /// <param name="theValue">The value to which the primary key needs to be set.</param>
        public static void SetPrimaryKeyFieldValue(object entity, object theValue)
        {
            PropertyInfo thePrimaryKeyFieldPropertyInfo = GetPropertyInfoForPrimaryKeyField(entity);
            thePrimaryKeyFieldPropertyInfo.SetValue(entity, theValue, null);
        }
    }

    /// <summary>
    /// This provides information on an association between two objects. For example, an Order might have a single Customer and one or more Products. This could represent the association between the Order and Customer entities or between the Order and Products entities.
    /// </summary>
    public class RepositoryItemAssociationInfo
    {
        /// <summary>
        /// The type of the entity that contains the property (e.g., Order)
        /// </summary>
        public Type TypeOfItemContainingProperty;
        /// <summary>
        /// The name of the navigation property that links this object with the foreign item (e.g., Customer if Order.Customer returns the Customer and Products if Order.Products returns the Products).
        /// </summary>
        public string NameOfPropertyConnectingToForeignItem;
        /// <summary>
        /// The name of the property containing the foreign key ID (e.g., CustomerID if the Order has a CustomerID field and "" otherwise; always "" for the one side of a one-to-many relationship). So, if TypeOfItemContainingProperty were Product, then this would be OrderID.
        /// </summary>
        public string NameOfPropertyWithForeignKeyID;
        /// <summary>
        /// This is information on the property (e.g., Order.Customer navigation property).
        /// </summary>
        public PropertyInfo Property;
        /// <summary>
        /// This is information on the property of the foreign key ID (e.g., CustomerID) or null if none (as will be the case on the one side of a one-to-many relationship).
        /// </summary>
        public PropertyInfo PropertyOfForeignKeyID;
        /// <summary>
        /// The type of the item on the other side of the relationship (e.g., Customer or Product)
        /// </summary>
        public Type TypeOfForeignItem;
        /// <summary>
        /// If this is a one-to-many relationship with the many on the other side of the relationship (e.g., many Products in an order), then this is true
        /// </summary>
        public bool IsMany;
        /// <summary>
        /// This is used internally to store the method for removing an item from the collection if IsMany is true.
        /// </summary>
        private MethodInfo _MethodToRemoveItemFromCollection;
        /// <summary>
        /// This is used internally to store the Type of the collection (e.g., EntitySet of Product) if IsMany is true.
        /// </summary>
        private Type _CollectionType;
        /// <summary>
        /// This returns the Type of the collection if IsMany is true and sets it if it has not yet been determined.
        /// </summary>
        public Type CollectionType
        {

            get
            {
                if (_CollectionType == null)
                    SetCollectionTypeAndRemoveMethod();
                return _CollectionType;
            }
            set
            {
                _CollectionType = value;
            }
        }
        /// <summary>
        /// This returns information on the method to remove an item from a one-to-many collection.
        /// </summary>
        public MethodInfo MethodToRemoveItemFromCollection
        {
            get
            {
                if (_MethodToRemoveItemFromCollection == null)
                    SetCollectionTypeAndRemoveMethod();
                return _MethodToRemoveItemFromCollection;
            }
            set
            {
                _MethodToRemoveItemFromCollection = value;
            }
        }

        /// <summary>
        /// This records the type of collection and the method to remove an item from this collection.
        /// </summary>
        internal void SetCollectionTypeAndRemoveMethod()
        {
            Type genericEntitySet = typeof(EntitySet<>);
            Type[] typeArgs = { TypeOfForeignItem };
            CollectionType = genericEntitySet.MakeGenericType(typeArgs);
            MethodToRemoveItemFromCollection = CollectionType.GetMethods().Single(x => x.Name == "Remove");
        }

        /// <summary>
        /// Gets all items on the other side of this association for an entity in the repository (e.g., Customer or Products).
        /// </summary>
        /// <param name="entityInTheRepository">An entity of type TypeOfItemContainingProperty</param>
        /// <returns>A List of objedts, which will have type TypeOfForeignItem</returns>
        public List<object> GetAllAssociatedObjects(object entityInTheRepository)
        {
            if (IsMany)
            {
                IEnumerable theEntitySet = Property.GetValue(entityInTheRepository, null) as IEnumerable;
                List<object> theList = new List<object>();
                foreach (var item in theEntitySet)
                    theList.Add(item);
                return theList;
            }
            else
            {
                object theValue = Property.GetValue(entityInTheRepository, null);
                if (theValue == null)
                    return new List<object>() { };
                else
                    return new List<object>() { theValue };
            }
        }

        /// <summary>
        /// This gets the foreign key ID (or null, if null) for this association for an entity in the repository.
        /// </summary>
        /// <param name="entityInTheRepository">An entity of type TypeOfItemContainingProperty</param>
        /// <returns>An object (of the type of the Foreign Key ID)</returns>
        public object GetForeignKeyID(object entityInTheRepository)
        {
            return PropertyOfForeignKeyID.GetValue(entityInTheRepository, null);
        }

        /// <summary>
        /// This severs the association for an entity in this repository, on this side of the relationship.
        /// For example, it would set Order.CustomerID to null, or it would remove a Product from the collection
        /// of products for this Order.
        /// </summary>
        /// <param name="theRepositoryItem"></param>
        /// <param name="foreignItemToRemove"></param>
        public void RemoveForeignItemFromProperty(object theRepositoryItem, object foreignItemToRemove)
        {
            if (IsMany)
            {
                object theEntitySet = Property.GetValue(theRepositoryItem, null); // EntitySet<typeof(TypeOfForeignItem)>;
                if (foreignItemToRemove != null)
                    CollectionType.InvokeMember("Remove", BindingFlags.Default | BindingFlags.InvokeMethod, null, theEntitySet, new object[] { foreignItemToRemove });
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

        /// <summary>
        /// Adds a foreign item to the property. For example, it sets the Customer for an order, or adds a Product to an order.
        /// </summary>
        /// <param name="entityInTheRepository">An entity of type TypeOfItemContainingProperty</param>
        /// <param name="foreignItemToAdd">An entity of type TypeOfForeignItem</param>
        public void AddForeignItemToProperty(object entityInTheRepository, object foreignItemToAdd)
        {
            if (IsMany)
            {
                object theEntitySet = Property.GetValue(entityInTheRepository, null); // EntitySet<typeof(TypeOfForeignItem)>;
                CollectionType.InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, theEntitySet, new object[] { foreignItemToAdd });
            }
            else if (PropertyOfForeignKeyID != null)
            {
                Property.SetValue(entityInTheRepository, foreignItemToAdd, null);
            }
            else
            { // don't want to set the EntityRef to null, want to set EntityRef<T>.Entity to null
                Property.SetValue(entityInTheRepository, foreignItemToAdd, null);
                //object theEntityRef = Property.GetValue(theObject, null); // EntityRef<typeof(TypeOfForeignItem)>;
                //Type theType = theEntityRef.GetType();
                //PropertyInfo thePropertyInfo = theType.GetProperty("Entity");
                //thePropertyInfo.SetValue(theEntityRef, foreignItemToAdd, null);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}.{2}={3}.<PrimaryId>", this.GetType().FullName,
                TypeOfItemContainingProperty.Name, NameOfPropertyWithForeignKeyID, NameOfPropertyConnectingToForeignItem);
        }
    }

    public static class MappingInfoProcessor
    {
        /// <summary>
        /// Stores RepositoryItemAssociationInfo items for each association, for each entity type in the data context.
        /// </summary>
        public static Dictionary<Type, List<RepositoryItemAssociationInfo>> mappingInfoForDataContexts = new Dictionary<Type, List<RepositoryItemAssociationInfo>>();

        /// <summary>
        /// Gets the mapping information from the data context, using information already stored if available or else by analyzing the data context.
        /// </summary>
        /// <param name="theDataContext">The data context to get mapping information about</param>
        /// <returns>A list of associations between objects; note that one relationship may return two associations (e.g., from the perspective of both the Customer and the Product)</returns>
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

        /// <summary>
        /// Analyzes the data context to return mapping information about it.
        /// </summary>
        /// <param name="theDataContext">The data context to get mapping information about</param>
        /// <returns>A list of associations between objects; note that one relationship may return two associations (e.g., from the perspective of both the Customer and the Product)</returns>
        public static List<RepositoryItemAssociationInfo> ProcessDataContextMappingInfo(DataContext theDataContext)
        {
            var associations = from x in theDataContext.Mapping.GetTables()
                               let rowType = x.RowType
                               let dataMembers = rowType.DataMembers
                               from dataMember in dataMembers
                               let memberInfo = dataMember.Member
                               where memberInfo is System.Reflection.PropertyInfo
                               let propertyInfo = (PropertyInfo)memberInfo
                               let propertyInfoAssociationAttribute = (AssociationAttribute)propertyInfo.GetCustomAttributes(typeof(AssociationAttribute), false).FirstOrDefault()
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
                                   IsMany = association.IsMany
                               };
            return associations.ToList();
        }
    }

    public interface IInMemoryRepositoryWithSubmitChangesSupport
    {
        void CompleteInsertOnSubmitStep1();
        void SetNavigationPropertiesForEntitiesBeingInserted();
        void CompleteDeleteOnSubmit();
        bool ItemIsNotInRepositoryOrIsSetToDelete(object item);
        bool ItemIsInRepositoryAndNotSetToDelete(object item);
        IInMemoryRepositoryWithSubmitChangesSupport CloneTo(InMemoryDataContext newOwner);
        object GetItemByID(object ID);
    }

    public class InMemoryRepository<T> : IInMemoryRepositoryWithSubmitChangesSupport, IRepository<T> where T : class, INotifyPropertyChanging, INotifyPropertyChanged
    {
        int maxPrimaryKeyID = 0;
        HashSet<T> EntitiesInRepository;
        HashSet<T> EntitiesBeingInserted;
        HashSet<T> EntitiesBeingDeleted;
        public InMemoryDataContext Owner { get; set; }
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

        public InMemoryRepository(HashSet<T> entitiesInRepository, InMemoryDataContext owner)
        {
            if (entitiesInRepository == null)
                EntitiesInRepository = new HashSet<T>();
            else
                EntitiesInRepository = entitiesInRepository;
            if (EntitiesInRepository.Any())
            {
                bool primaryKeyIsInt = RepositoryItemPrimaryKeys.PrimaryKeyFieldIsInt(entitiesInRepository.First());
                if (primaryKeyIsInt)
                    maxPrimaryKeyID = EntitiesInRepository.Max(x => (int)RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x));
            }
            EntitiesBeingDeleted = new HashSet<T>();
            EntitiesBeingInserted = new HashSet<T>();
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


        public IInMemoryRepositoryWithSubmitChangesSupport CloneTo(InMemoryDataContext newOwner)
        {
            HashSet<T> newSet = new HashSet<T>();
            foreach (var entityInRepository in EntitiesInRepository)
                newSet.Add(entityInRepository); // cloning the objects isn't working right now, largely because the entityset connections are messed up .Select(x => CloneObject<T>(x)));
            return new InMemoryRepository<T>(newSet, newOwner);
        }

        public Type ElementType { get { return EntitiesInRepository.AsQueryable().ElementType; } }
        public Expression Expression { get { return EntitiesInRepository.AsQueryable().Expression; } }
        public IQueryProvider Provider { get { return EntitiesInRepository.AsQueryable().Provider; } }

        public bool ItemIsNotInRepositoryOrIsSetToDelete(object item)
        {
            return !EntitiesInRepository.Contains(item) || EntitiesBeingDeleted.Contains(item);
        }

        public bool ItemIsInRepositoryAndNotSetToDelete(object item)
        {
            return EntitiesInRepository.Contains(item) && !EntitiesBeingDeleted.Contains(item);
        }

        public object GetItemByID(object ID)
        {
            return EntitiesInRepository.Single(x => RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x).Equals(ID));
        }

        public void InsertOnSubmit(T theObject)
        {
            if (EntitiesInRepository.Contains(theObject))
                throw new Exception("Trying to insert an object that has already been inserted into the table.");
            EntitiesInRepository.Add(theObject);
            EntitiesBeingInserted.Add(theObject);
        }

        public void InsertOnSubmitIfNotAlreadyInserted(T theObject)
        {
            if (!EntitiesInRepository.Contains(theObject))
            {
                InsertOnSubmit(theObject);
            }
        }

        public void DeleteOnSubmit(T theObject)
        {
            if (EntitiesInRepository.Contains(theObject))
            {
                EntitiesInRepository.Remove(theObject);
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
            if (RepositoryItemPrimaryKeys.PrimaryKeyFieldIsInt(theItem))
                RepositoryItemPrimaryKeys.SetPrimaryKeyFieldValue(theItem, maxPrimaryKeyID);
            else if (RepositoryItemPrimaryKeys.PrimaryKeyFieldIsGUID(theItem))
                RepositoryItemPrimaryKeys.SetPrimaryKeyFieldValue(theItem, Guid.NewGuid());
        }

        internal void SetUnsetPrimaryKeys()
        {
            IEnumerable<T> theUnset = EntitiesInRepository.Where(x => RepositoryItemPrimaryKeys.unsetPrimaryKeyValues.Any(y => y == RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x)));
            foreach (var unset in theUnset)
                SetUnsetPrimaryKey(unset);
        }

        internal void SetPropertiesBasedOnForeignKeyIDAndViceVersa(T theItem)
        {
            // We need to check each property to see if the foreign key is set but the property is not
            // For example, if AddressField is created and FieldID = 35 but AddressFIeld.Field is not set to the object with
            // primary key id == 35, then
            // we must set Field to that object.
            foreach (var property in PropertiesForThisItemType.Where(x => !x.IsMany && x.PropertyOfForeignKeyID != null))
            {
                object foreignKeyID = property.GetForeignKeyID(theItem);
                Type foreignItemType = property.TypeOfForeignItem;
                List<object> associatedObjects = property.GetAllAssociatedObjects(theItem);
                if (foreignKeyID != null && !RepositoryItemPrimaryKeys.unsetPrimaryKeyValues.Any(x => foreignKeyID.Equals(x)))
                { // set property based on id
                    if (!associatedObjects.Any(x =>
                        // x.GetType() == foreignItemType &&
                        !RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(x).Equals(foreignKeyID)))
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
                        object primaryKeyFieldValue = RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(associatedObject);
                        if (!primaryKeyFieldValue.Equals(foreignKeyID))
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
                if (RepositoryItemPrimaryKeys.unsetPrimaryKeyValues.Contains(RepositoryItemPrimaryKeys.GetPrimaryKeyFieldValue(item)))
                    SetUnsetPrimaryKey(item); // we must set all primary keys for all repositories before setting the foreign key ids
            }
        }


        public void SetNavigationPropertiesForEntitiesBeingInserted()
        {
            foreach (var item in EntitiesBeingInserted)
            {
                SetPropertiesBasedOnForeignKeyIDAndViceVersa(item);
            }
            EntitiesBeingInserted = new HashSet<T>(); // clear out the list
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
            EntitiesBeingDeleted = new HashSet<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return EntitiesInRepository.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EntitiesInRepository.GetEnumerator();
        }
    }

    public class InMemoryDataContext
    {
        public Dictionary<Type, IInMemoryRepositoryWithSubmitChangesSupport> inMemoryRepositoryDictionary;
        public List<IInMemoryRepositoryWithSubmitChangesSupport> inMemoryRepositoryList;
        public DataContext UnderlyingDataContext { get; set; }
        public List<RepositoryItemAssociationInfo> MappingInfo { get; set; }

        public InMemoryDataContext(DataContext underlyingDataContext)
        {
            UnderlyingDataContext = underlyingDataContext;
            MappingInfo = MappingInfoProcessor.GetMappingInfoForDataContext(underlyingDataContext);
            inMemoryRepositoryDictionary = new Dictionary<Type, IInMemoryRepositoryWithSubmitChangesSupport>();
            inMemoryRepositoryList = new List<IInMemoryRepositoryWithSubmitChangesSupport>();
        }

        public InMemoryDataContext(DataContext underlyingDataContext, InMemoryDataContext listToClone)
        {
            UnderlyingDataContext = underlyingDataContext;
            MappingInfo = MappingInfoProcessor.GetMappingInfoForDataContext(underlyingDataContext);
            CloneFrom(listToClone);
        }

        public void CloneFrom(InMemoryDataContext listToClone)
        {
            inMemoryRepositoryDictionary = new Dictionary<Type, IInMemoryRepositoryWithSubmitChangesSupport>();
            inMemoryRepositoryList = new List<IInMemoryRepositoryWithSubmitChangesSupport>();
            foreach (var entry in listToClone.inMemoryRepositoryDictionary)
            {
                IInMemoryRepositoryWithSubmitChangesSupport theClone = entry.Value.CloneTo(this);
                inMemoryRepositoryDictionary.Add(entry.Key, theClone);
                inMemoryRepositoryList.Add(theClone);
            }
        }

        public IRepository<T> GetRepository<T>() where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            Type theType = typeof(T);
            IInMemoryRepositoryWithSubmitChangesSupport item;
            if (inMemoryRepositoryDictionary.ContainsKey(theType))
            {
                item = inMemoryRepositoryDictionary[theType];
                return item as InMemoryRepository<T>;
            }
            else
            {
                InMemoryRepository<T> newRepository = new InMemoryRepository<T>(null, this);
                inMemoryRepositoryDictionary.Add(theType, newRepository);
                inMemoryRepositoryList.Add(newRepository);
                return newRepository;
            }
        }

        public List<IInMemoryRepositoryWithSubmitChangesSupport> GetRepositories()
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
                inMemoryRepositoryList[i].SetNavigationPropertiesForEntitiesBeingInserted();
            }
        }

        public IInMemoryRepositoryWithSubmitChangesSupport GetRepositoryForItem(object item)
        {
            Type theType = item.GetType();
            if (!inMemoryRepositoryDictionary.ContainsKey(theType))
                return null;
            IInMemoryRepositoryWithSubmitChangesSupport repository = inMemoryRepositoryDictionary[theType];
            return repository;
        }

        public object GetItemByTypeAndID(Type type, object ID)
        {
            IInMemoryRepositoryWithSubmitChangesSupport repository = inMemoryRepositoryDictionary[type];
            return repository.GetItemByID(ID);
        }

        public void ConfirmNotAlreadyInAnotherDataContext(Dictionary<object, InMemoryDataContext> originalDataContexts, object item)
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
        public static Dictionary<Type, InMemoryDataContext> inMemoryRepositoryDictionary;
        public static Dictionary<object, InMemoryDataContext> originalInMemoryRepositoryList = new Dictionary<object, InMemoryDataContext>();

        public static InMemoryDataContext GetSimulatedPermanentStorageForDataContextType(DataContext underlyingDataContext)
        {
            Type theType = underlyingDataContext.GetType();

            if (inMemoryRepositoryDictionary == null)
                inMemoryRepositoryDictionary = new Dictionary<Type, InMemoryDataContext>();

            if (inMemoryRepositoryDictionary.ContainsKey(theType))
                return inMemoryRepositoryDictionary[theType];
            else
            {
                InMemoryDataContext newPermanentStorage = new InMemoryDataContext(underlyingDataContext);
                inMemoryRepositoryDictionary.Add(theType, newPermanentStorage);
                return newPermanentStorage;
            }
        }

        public static void SetSimulatedPermanentStorageForDataContextType(DataContext underlyingDataContext, InMemoryDataContext inMemoryRepositoryList)
        { // This is a faster way of doing submit changes.
            Type theType = underlyingDataContext.GetType();

            if (inMemoryRepositoryDictionary == null)
                inMemoryRepositoryDictionary = new Dictionary<Type, InMemoryDataContext>();

            if (inMemoryRepositoryDictionary.ContainsKey(theType))
                inMemoryRepositoryDictionary[theType] = inMemoryRepositoryList;
            else
                inMemoryRepositoryDictionary.Add(theType, inMemoryRepositoryList);
        }

        public static void Reset()
        {
            inMemoryRepositoryDictionary = null;
            originalInMemoryRepositoryList = new Dictionary<object, InMemoryDataContext>();
        }
    }


    public class InMemoryContext : IDataContext
    {
        public InMemoryDataContext inMemoryRepositories;

        public DataContext UnderlyingDataContext { get; set; }

        public InMemoryContext(DataContext underlyingDataContext)
        {
            UnderlyingDataContext = underlyingDataContext;
            Type theType = underlyingDataContext.GetType();
            LoadFromPermanentStorage();
        }

        public bool TooLateToSetPageLoadOptions { get; set; }

        public void Reset()
        {
            // LoadFromPermanentStorage() doesn't seem to help and is a bit time consuming.
        }

        internal void LoadFromPermanentStorage()
        {
            if (UseFasterSubmitChanges.setting)
                inMemoryRepositories = SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext);
            else
                inMemoryRepositories = new InMemoryDataContext(UnderlyingDataContext, SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext));
        }

        public IRepository<T> GetTable<T>() where T : class, INotifyPropertyChanging, INotifyPropertyChanged
        {
            return inMemoryRepositories.GetRepository<T>();
        }

        public void SubmitChanges(System.Data.Linq.ConflictMode conflictMode)
        {
            SubmitChanges();
        }

        public void SubmitChanges()
        {
            BeforeSubmitChanges();
            CompleteSubmitChanges(ConflictMode.ContinueOnConflict);
        }

        public virtual void BeforeSubmitChanges()
        {
        }

        public void CompleteSubmitChanges(System.Data.Linq.ConflictMode conflictMode)
        {
            inMemoryRepositories.CleanUpBeforeSubmittingChanges();
            if (inMemoryRepositories != null && UseFasterSubmitChanges.setting)
                SimulatedPermanentStorage.SetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext, inMemoryRepositories);
            else
            {
                InMemoryDataContext permanentStorage = SimulatedPermanentStorage.GetSimulatedPermanentStorageForDataContextType(UnderlyingDataContext);
                permanentStorage.CloneFrom(inMemoryRepositories);
            }
        }
    }
}

