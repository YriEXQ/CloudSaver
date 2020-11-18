using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using GFile = Google.Apis.Drive.v3.Data.File;
using System.Threading.Tasks;
using System.Web;


namespace CloudSaver
{
    //namespace CommonAdapter
    //{
    //    interface IGenericResourceAdapter
    //    {
    //    }

    //    class GoogleDriveResource : Google.Apis.Drive.v3.Data.File, IGenericResourceAdapter
    //    {
    //    }

    //    class YandexDriveResource : Resource, IGenericResourceAdapter
    //    { }
    //}

    


    class GoogleDriveConnector
    {

        public static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
        //Общие переменные предназначенные для входа
        protected static string[] scopes = { DriveService.Scope.Drive };
        protected UserCredential credential;
        static string ApplicationName = "CloudSaver";
        protected DriveService service;

        public void Auth()
        {

            using (var stream =
            new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public IList<Google.Apis.Drive.v3.Data.File> GetFilesList
            (string fieds = "", string request = "", int? pageSize = 100)
        {
            try
            {
                FilesResource.ListRequest listRequest = service.Files.List();
                if (pageSize != null && pageSize > 0)
                {
                    listRequest.PageSize = pageSize;
                }

                if (fieds != "")
                {
                    listRequest.Fields = fieds;
                }

                if (request != "")
                {
                    listRequest.Q = request;
                }
                return listRequest.Execute().Files;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public GFile CreateDirectory(string name, string id = "root", string desc = "")
        {
            // Create metaData for a new Directory
            GFile NewDirectory = new GFile();
            NewDirectory.Name = name;
            NewDirectory.Description = desc;
            NewDirectory.Parents = new[] { id };
            NewDirectory.MimeType = "application/vnd.google-apps.folder";
            try
            {
                var request = service.Files.Create(NewDirectory);
                request.Fields = "id, name, parents, createdTime, modifiedTime, mimeType";

                return request.Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Stream> DownloadFile(string fileId)
        {
            try
            {
                Stream outputstream = new MemoryStream();
                var request = service.Files.Get(fileId);
                try
                {
                    await request.DownloadAsync(outputstream);
                }
                catch (Exception ex)
                { Console.WriteLine(ex.Message); }

                outputstream.Position = 0;
                return outputstream;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public bool RemoveFile(string id)
        {
            try
            {
                service.Files.Delete(id).Execute();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public bool RenameFile(string name, string id)
        {
            try
            {
                GFile file = service.Files.Get(id).Execute();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }


            var update = new GFile();
            update.Name = name;

            try
            {
                service.Files.Update(update, id).Execute();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public async Task<Google.Apis.Drive.v3.Data.File> UploadFile(string filePath, string documentId)
        {
            GFile body = new GFile();
            if (System.IO.File.Exists(filePath))
            {

                body.Name = System.IO.Path.GetFileName(filePath);
                body.MimeType = GetMimeType(filePath);
                body.Parents = new[] { documentId };
            }
            else
            {
                return null;
            }

            byte[] byteArray = System.IO.File.ReadAllBytes(filePath);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
            try
            {
                FilesResource.CreateMediaUpload request = service.Files.Create(body, stream, GetMimeType(filePath));
                request.Fields = "id, name, parents, createdTime, modifiedTime, mimeType, thumbnailLink";
                await request.UploadAsync();
                return request.ResponseBody;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }
        }

    }



}
