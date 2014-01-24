using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.IO;
using System.Web;
using System.Diagnostics;
using ClassLibrary1.Misc;

namespace ClassLibrary1.Model
{
    public static class RaterooBlobAccess
    {
        public static CloudBlobClient GetBlobClient()
        {
            AzureSetup.SetConfigurationSettingPublisher();
            CloudStorageAccount account = CloudStorageAccount.FromConfigurationSetting("BlobConnectionString");
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            return blobClient;
        }

        public static void DeleteAllBlobs()
        {

            CloudBlobClient blobClient = GetBlobClient();
            var theContainers = blobClient.ListContainers();
            foreach (var container in theContainers)
                container.Delete();
        }

        public static CloudBlobContainer GetBlobContainerReference(string containerName)
        {
            CloudBlobClient blobClient = GetBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExist();
            return container;
        }

        public static CloudBlob GetBlobReference(string containerName, string fileName)
        {
            CloudBlobContainer container = GetBlobContainerReference(containerName);
            CloudBlob blob = container.GetBlobReference(fileName);           
            return blob;
        }

        // Only way to do this is to throw an exception. Thus, we use DebuggerHidden so that debugging is not interrupted.
        [DebuggerHidden]
        public static bool BlobExists(string containerName, string fileName)
        {
            CloudBlob theBlob = GetBlobReference(containerName, fileName);
            try
            {
                theBlob.FetchAttributes();
                return true;
            }
            catch (StorageClientException e)
            {
                if (e.ErrorCode == StorageErrorCode.ResourceNotFound)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public static void UploadBlob(string containerName, string fileName, string sourceFileName)
        {
            if (BlobExists(containerName, fileName))
                throw new Exception("Cannot upload blob, because file " + fileName + " already exists in container " + containerName + ".");
            CloudBlob blob = GetBlobReference(containerName, fileName);
            blob.UploadFile(sourceFileName);
        }

        public static void DownloadBlob(string containerName, string fileName, string destinationFileName)
        {
            CloudBlob blob = GetBlobReference(containerName, fileName);
            blob.DownloadToFile(destinationFileName);
        }

        public static void DeleteBlobIfExists(string containerName, string fileName)
        {
            CloudBlob blob = GetBlobReference(containerName, fileName);
            blob.DeleteIfExists();
        }
    }

    /// <summary>
    /// Allows creation of temporary files that can then be stored permanently and later access.
    /// The creation and access uses Azure local storage, while the permanent storage uses an Azure blob.
    /// </summary>
    public class RaterooFile
    {
        public string ContainerName;
        public string FileName;

        public RaterooFile(string containerName, string fileName)
        {
            ContainerName = containerName;
            FileName = fileName;
        }

        public string GetPathToLocalFile()
        {
            LocalResource localCache = RoleEnvironment.GetLocalResource("raterooLocal");
            string containerPath = Path.Combine(localCache.RootPath, ContainerName);
            Directory.CreateDirectory(containerPath); // create if necessary
            string pathToFile = Path.Combine(localCache.RootPath, ContainerName, FileName);
            return pathToFile;
        }

        public string CreateTemporary()
        {
            string path = GetPathToLocalFile();
            var fileStream = File.Create(path);
            fileStream.Close();
            return path;
        }

        public void StorePermanently()
        {
            string path = GetPathToLocalFile();
            RaterooBlobAccess.DeleteBlobIfExists(ContainerName, FileName);
            RaterooBlobAccess.UploadBlob(ContainerName, FileName, path);
        }

        public void DeleteTemporary()
        {
            string path = GetPathToLocalFile();
            if (File.Exists(path))
                File.Delete(path);
        }

        public void DeletePermanently()
        {
            DeleteTemporary();
            RaterooBlobAccess.DeleteBlobIfExists(ContainerName, FileName);
        }

        public string LoadPreviouslyStored()
        {
            string path = GetPathToLocalFile();
            if (File.Exists(path))
                return path;
            if (!RaterooBlobAccess.BlobExists(ContainerName, FileName))
                throw new Exception("File does not exist.");
            RaterooBlobAccess.DownloadBlob(ContainerName, FileName, path);
            return path;
        }

        public void DownloadToUserBrowser(HttpResponse response, string contentType = "text/plain")
        {
            string path = LoadPreviouslyStored();
            response.ClearContent();
            response.ClearHeaders();
            response.ContentType = contentType;
            response.AppendHeader("content-disposition", "attachment; filename=" + Path.GetFileName( path ));
            response.TransmitFile(path, 0, -1);
           response.End();
           //response.Close();
            // causes error response.End();
            //System.IO.FileStream downloadFile = new System.IO.FileStream(path, System.IO.FileMode.Open);
            //response.Write(downloadFile.Length + "#");
            //downloadFile.Close();
            //response.WriteFile(path);
            //response.Flush();
            //response.End();
        }
        
    }

}
