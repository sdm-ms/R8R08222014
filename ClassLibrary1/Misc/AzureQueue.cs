using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

public static class AzureQueue
{
    internal static CloudQueue GetCloudQueue(string queueName)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
        CloudQueueClient Qsvc = storageAccount.CreateCloudQueueClient();
        CloudQueue q = Qsvc.GetQueueReference(queueName);
        q.CreateIfNotExist();
        return q;
    }

    public static void Push(string queueName, Object theObject)
    {
        byte[] theMessage = ByteArrayConversions.ObjectToByteArray(theObject);
        CloudQueueMessage theNewMessage = new CloudQueueMessage(theMessage);
        CloudQueue q = GetCloudQueue(queueName);
        q.AddMessage(theNewMessage);
    }

    public static Object Peek(string queueName)
    {
        CloudQueue q = GetCloudQueue(queueName);
        CloudQueueMessage message = q.PeekMessage();
        return ByteArrayConversions.ByteArrayToObject(message.AsBytes);
    }

    public static Object Pop(string queueName)
    {
        CloudQueue q = GetCloudQueue(queueName);
        CloudQueueMessage message = q.GetMessage();
        Object theObject = ByteArrayConversions.ByteArrayToObject(message.AsBytes);
        q.DeleteMessage(message);
        return theObject;
    }

    public static List<Object> Pop(string queueName, int numMessages)
    {
        CloudQueue q = GetCloudQueue(queueName);
        List<CloudQueueMessage> messages = q.GetMessages(numMessages).ToList();
        List<Object> objects = messages.Select(x => ByteArrayConversions.ByteArrayToObject(x.AsBytes)).ToList();
        messages.ForEach(x => q.DeleteMessage(x));
        return objects;
    }

    public static List<Object> GetMessages(string queueName, int numMessages)
    {
        CloudQueue q = GetCloudQueue(queueName);
        List<CloudQueueMessage> messages = q.GetMessages(numMessages).ToList();
        List<Object> objects = messages.Select(x => ByteArrayConversions.ByteArrayToObject(x.AsBytes)).ToList();
        messages.ForEach(x => q.DeleteMessage(x));
        return objects;
    }
}

public class AzureQueueWithErrorRecovery
{
    internal List<CloudQueueMessage> Messages;
    internal int MaxAttempts = 5;
    internal Action<object> FailureAction;

    public AzureQueueWithErrorRecovery(int maxAttemptsBeforeTakingFailureAction, Action<object> failureAction)
    {
        MaxAttempts = maxAttemptsBeforeTakingFailureAction;
        FailureAction = failureAction;
    }

    public List<Object> GetMessages(string queueName, int numMessages)
    {
        CloudQueue q = AzureQueue.GetCloudQueue(queueName);
        Messages = new List<CloudQueueMessage>();
        int numMessagesToProcess = numMessages;
        while (numMessagesToProcess > 0)
        {
            int numMessagesToProcessThisTime = (numMessagesToProcess > 32) ? 32 : numMessagesToProcess; // maximum retrievable at once
            Messages.AddRange(q.GetMessages(numMessagesToProcessThisTime).ToList());
            numMessagesToProcess = numMessagesToProcess - numMessagesToProcessThisTime;
        }
        List<Object> objects = new List<object>();
        foreach (var message in Messages)
        {
            var theObject = ByteArrayConversions.ByteArrayToObject(message.AsBytes);
            if (message.DequeueCount > MaxAttempts)
            {
                q.DeleteMessage(message);
                if (FailureAction != null)
                    FailureAction(theObject);
            }
            else
                objects.Add(theObject);
        }
        return objects;
    }

    public void ConfirmProperExecution(string queueName)
    {
        CloudQueue q = AzureQueue.GetCloudQueue(queueName);
        if (Messages != null)
        {
            Messages.ForEach(x => q.DeleteMessage(x));
        }
    }
}
