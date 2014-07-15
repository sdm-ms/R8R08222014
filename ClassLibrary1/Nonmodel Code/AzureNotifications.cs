using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Data.Services.Client;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ClassLibrary1.Misc;



public class AzureNotificationTableServiceEntity : TableServiceEntity
{
    public string NotificationText { get; set; }
}

public class AzureNotificationContext : TableServiceContext
{
    private static CloudStorageAccount storageAccount =
    CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
    public AzureNotificationContext()
        : base(storageAccount.TableEndpoint.AbsoluteUri,
        storageAccount.Credentials)
    {
    }
    public DataServiceQuery<AzureNotificationTableServiceEntity> AzureNotifications
    {
        get
        {
            return CreateQuery<AzureNotificationTableServiceEntity>("AzureNotifications");
        }
    }
}

[Serializable()]
public class AzureNotificationTableMetadata
{
    public DateTime? oldestPossibleItemTime { get; set; }
    public DateTime? newestPossibleItemTime { get; set; }
    public int? firstRowKeyInTable { get; set; }
    public int? lastRowKeyInTable { get; set; }
}

public class AzureNotificationTableMetadataServiceEntity : TableServiceEntity
{
    public byte[] Metadata { get; set; } // cannot use only some types, so we'll convert
}

public class AzureNotificationTableMetadataContext : TableServiceContext
{
    private static CloudStorageAccount storageAccount =
    CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
    public AzureNotificationTableMetadataContext()
        : base(storageAccount.TableEndpoint.AbsoluteUri,
        storageAccount.Credentials)
    {
    }
    public DataServiceQuery<AzureNotificationTableMetadataServiceEntity> AzureNotificationTableMetadata
    {
        get
        {
            return CreateQuery<AzureNotificationTableMetadataServiceEntity>("AzureNotificationsMetadata");
        }
    }
}

public class AzureNotificationsRequestedAlreadyDeletedException : Exception
{
}

public static class AzureNotificationProcessor
{
    const string azureNotificationsTableName = "AzureNotifications";
    const string azureNotificationsMetadataTableName = "AzureNotificationsMetadata";
    internal static bool initialized = false;
    internal static AzureNotificationTableMetadata metadata = new AzureNotificationTableMetadata();

    internal static void PrepareTable()
    {
        if (!initialized)
        {
            initialized = true;
            AzureSetup.SetConfigurationSettingPublisher();
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            tableClient.CreateTableIfNotExist(azureNotificationsTableName);
            tableClient.CreateTableIfNotExist(azureNotificationsMetadataTableName);
        }
    }

    internal static void DeleteAllNotifications()
    {
        var notificationContext = new AzureNotificationContext();
        var allNotifications = notificationContext.AzureNotifications.ToList();
        foreach (var notification in allNotifications)
            notificationContext.DeleteObject(notification);
        notificationContext.SaveChanges();
    }

    internal static void CopyLocalMetadataToTable()
    {
        var metadataContext = new AzureNotificationTableMetadataContext();
        AzureNotificationTableMetadataServiceEntity theMetadataTableServiceEntity = metadataContext.AzureNotificationTableMetadata.Where(x => x.PartitionKey == "metadata").FirstOrDefault();
        if (theMetadataTableServiceEntity == null)
        {
            theMetadataTableServiceEntity = new AzureNotificationTableMetadataServiceEntity
            {
                PartitionKey = "metadata",
                RowKey = "1",
                Metadata = ByteArrayConversions.ObjectToByteArray(metadata)
            };
            metadataContext.AddObject(azureNotificationsMetadataTableName, theMetadataTableServiceEntity);
        }
        else
        {
            theMetadataTableServiceEntity.Metadata = ByteArrayConversions.ObjectToByteArray(metadata);
            metadataContext.UpdateObject(theMetadataTableServiceEntity);
        }
        metadataContext.SaveChanges();
    }

    internal static void CopyTableMetadataLocally()
    {
        var metadataContext = new AzureNotificationTableMetadataContext();
        AzureNotificationTableMetadataServiceEntity theMetadataTableServiceEntity = metadataContext.AzureNotificationTableMetadata.Where(x => x.PartitionKey == "metadata").FirstOrDefault();
        if (theMetadataTableServiceEntity != null)
            metadata = (AzureNotificationTableMetadata) ByteArrayConversions.ByteArrayToObject(theMetadataTableServiceEntity.Metadata);
    }

    public static List<string> ReadNewNotifications(DateTime? sinceUTCTime, string notificationType)
    {
        if (sinceUTCTime == null)
            sinceUTCTime = TestableDateTime.Now;
        PrepareTable();
        CopyTableMetadataLocally(); // Because we're reading the table, we need to find information on what's been written.
        if (metadata.oldestPossibleItemTime != null && (DateTime) sinceUTCTime < metadata.oldestPossibleItemTime)
            throw new AzureNotificationsRequestedAlreadyDeletedException();

        if ((DateTime) sinceUTCTime > metadata.newestPossibleItemTime || metadata.newestPossibleItemTime == null)
            return new List<string>();

        var notificationContext = new AzureNotificationContext();

        var newNotifications = notificationContext.AzureNotifications
            .Where(x => x.PartitionKey == notificationType)
            .ToList()
            .Where(x => x.Timestamp > (DateTime) sinceUTCTime)
            .OrderBy(x => x.Timestamp)
            .Select(x => x.NotificationText)
            .ToList()
            ;
        return newNotifications;
    }

    public static void DeleteOldNotifications(TimeSpan? timeBeforeAutoDelete = null)
    {
        if (timeBeforeAutoDelete == null)
            timeBeforeAutoDelete = new TimeSpan(0, 2, 0);
        PrepareTable();
        var notificationContext = new AzureNotificationContext();
        DateTime cutoffTime = TestableDateTime.Now - (TimeSpan) timeBeforeAutoDelete;
        if (metadata.newestPossibleItemTime == null || metadata.newestPossibleItemTime < cutoffTime)
            return; // nothing to delete
        var allNotifications = notificationContext.AzureNotifications.ToList();
        DateTime now = TestableDateTime.Now;
        var oldNotifications = allNotifications.Where(x => x.Timestamp < cutoffTime || x.Timestamp > now + timeBeforeAutoDelete); // also delete notifications from the future, because of clock mistakenly moving backwards
        foreach (var notification in oldNotifications)
            notificationContext.DeleteObject(notification);
        notificationContext.SaveChanges();
        metadata.oldestPossibleItemTime = cutoffTime;
        var remainingNotifications = allNotifications.Where(x => x.Timestamp >= cutoffTime).OrderBy(x => x.Timestamp);
        if (remainingNotifications.Any())
        {
            metadata.firstRowKeyInTable = Convert.ToInt32(remainingNotifications.FirstOrDefault().RowKey);
            metadata.lastRowKeyInTable = Convert.ToInt32(remainingNotifications.Last().RowKey);
        }
        CopyLocalMetadataToTable();
    }

    public static void AddNotification(string notificationType, string notificationText)
    {
        PrepareTable();
        var notificationContext = new AzureNotificationContext();
        if (metadata.firstRowKeyInTable == null)
        {
            DeleteAllNotifications();
            metadata.firstRowKeyInTable = 1;
            metadata.lastRowKeyInTable = 1;
        }
        else
            metadata.lastRowKeyInTable++;
        int rowNum = (int)metadata.lastRowKeyInTable;
        var newNotification = new AzureNotificationTableServiceEntity
        {
            PartitionKey = notificationType,
            RowKey = rowNum.ToString(),
            NotificationText = notificationText
        };
        try
        {
            notificationContext.AddObject(azureNotificationsTableName, newNotification);
            notificationContext.SaveChanges();
        }
        catch
        {
            try
            {
                notificationContext.DeleteObject(newNotification);
                notificationContext.SaveChanges();
            }
            catch
            {
            }
            var allNotifications = notificationContext.AzureNotifications.ToList();
            metadata.lastRowKeyInTable = allNotifications.Max(x => Convert.ToInt32(x.RowKey)) + 1;
            newNotification.RowKey = metadata.lastRowKeyInTable.ToString();
            notificationContext.AddObject(azureNotificationsTableName, newNotification);
            notificationContext.SaveChanges();
        }
        metadata.newestPossibleItemTime = TestableDateTime.Now;
        CopyLocalMetadataToTable();
    }


}

