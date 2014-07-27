using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.Web.UI.DataVisualization.Charting;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Web;
using System.Web.Caching;

namespace ClassLibrary1.Nonmodel_Code
{
    public class AzureChartControlBlobHandler : IChartStorageHandler
    {

        const string BlobContainerName = "chartstorage"; // MUST be all lowercase
        const string ChartNamePrefix = "C";

        // The following initialization of blob storage is so that we can handle
        // the unhandled exception in initial startup.
        private static bool storageInitialized = false;
        private static object gate = new Object();
        private static CloudBlobClient blobStorage;
        //private static CloudQueueClient queueStorage;
        private static CloudBlobContainer container;

        private void InitializeStorage()
        {
            if (storageInitialized)
            {
                return;
            }

            lock (gate)
            {
                if (storageInitialized)
                {
                    return;
                }


                // read account configuration settings

                AzureSetup.SetConfigurationSettingPublisher();

                var storageAccount =
                  CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

                // create blob container for images
                blobStorage =
                    storageAccount.CreateCloudBlobClient();
                container = blobStorage.
                    GetContainerReference(BlobContainerName);

                container.CreateIfNotExist();

                // configure container for public access
                var permissions = container.GetPermissions();
                permissions.PublicAccess =
                     BlobContainerPublicAccessType.Container;
                container.SetPermissions(permissions);

                storageInitialized = true;
            }
        }

        public AzureChartControlBlobHandler()
        {
            InitializeStorage();

        }
        #region IChartStorageHandler Members

        public void Delete(string key)
        {
            CloudBlob image = container.GetBlobReference(ChartNamePrefix + key);
            image.Delete();
        }

        public bool Exists(string key)
        {
            bool exists = true;
            WebClient webClient = new WebClient();
            try
            {
                using (Stream stream = webClient.OpenRead(ChartNamePrefix + key))
                { }
            }
            catch (WebException)
            {
                exists = false;
            }
            return exists;
        }

        public byte[] Load(string key)
        {
            //sometimes load gets called before save is done

            const int maxTries = 40;
            int tryNumber = 0;
            CloudBlob image = container.GetBlobReference(ChartNamePrefix + key);
            byte[] imageArray;
            object cachedItem = HttpRuntime.Cache["Chart" + ChartNamePrefix + key];
            if (cachedItem != null)
                return (byte[])cachedItem;
            TryLabel:
            try
            {
                tryNumber++;
                imageArray = image.DownloadByteArray();
            }
            catch (Exception)
            {
                if (tryNumber <= maxTries)
                {
                    System.Threading.Thread.Sleep(50);
                    goto TryLabel;
                }
                else
                    return null;

            }
            // cache it for 10 minutes
            HttpRuntime.Cache.Add("Chart" + ChartNamePrefix + key, imageArray, new CacheDependency(null, new string[] {}), TestableDateTime.Now + new TimeSpan(0,0,10,0,0), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            return imageArray;
        }

        public void Save(string key, byte[] data)
        {
            HttpContext.Current.Cache["MostRecentChartKey"] = key;
            CloudBlob image = container.GetBlobReference(ChartNamePrefix + key);
            image.UploadByteArray(data);
        }

        #endregion

    }
}
