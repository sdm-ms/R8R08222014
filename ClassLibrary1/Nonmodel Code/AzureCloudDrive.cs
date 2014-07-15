using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace ClassLibrary1.Misc
{
    public class AzureCloudDrive
    {
        const string BlobContainerName = "drives"; // must be lowercase
        string DriveFileName; // specified by user -- should end in vhd

        // The following initialization of blob storage is so that we can handle
        // the unhandled exception in initial startup.
        private static bool storageInitialized = false;
        private static object gate = new Object();
        private static CloudBlobClient blobStorage;
        //private static CloudQueueClient queueStorage;
        private static CloudBlobContainer container;
        private static CloudStorageAccount storageAccount;
        public string driveLetter { get; set; }

        private void InitializeStorage()
        {
            bool inDevFabric = false; // initialize to default
            if (RoleEnvironment.IsAvailable)
            {
                var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints.Values.FirstOrDefault();
                if (endpoint != null)
                {
                    string ip = endpoint.IPEndpoint.Address.ToString();
                    inDevFabric = ip.Contains("127.0.0.1");
                }
            }

            InitializeStorageHelper(inDevFabric);
        }

        private void InitializeStorageHelper(bool inDevFabric)
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
                if (inDevFabric)
                    storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                else
                    storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");

                if (RoleEnvironment.IsAvailable) // will the code work without the next two lines?
                {
                    LocalResource localCache = RoleEnvironment.GetLocalResource("InstanceDriveCache");
                    CloudDrive.InitializeCache(localCache.RootPath, localCache.MaximumSizeInMegabytes);
                }

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

        public void Mount()
        {
            // Create cloud drive
            CloudDrive myCloudDrive = storageAccount.CreateCloudDrive(
                blobStorage
                .GetContainerReference(BlobContainerName)
                .GetPageBlobReference(DriveFileName)
                .Uri.ToString()
            );

            try
            {
                myCloudDrive.Create(64);
                // Note: If we get an Unknown Error exception here, we probably need to clear out development storage.
            }
            catch (CloudDriveException)
            {
                // handle exception here
                // exception is also thrown if all is well but the drive already exists
            }

            driveLetter = myCloudDrive.Mount(25, DriveMountOptions.Force);
        }

        public AzureCloudDrive(string driveFileName)
        {
            DriveFileName = driveFileName;

            InitializeStorage();
            Mount();
        }
    }
}
