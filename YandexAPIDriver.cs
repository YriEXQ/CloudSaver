using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YandexDisk.Client;
using YandexDisk.Client.Clients;
using YandexDisk.Client.Http;
using YandexDisk.Client.Protocol;
using DriveLinker;

namespace YandexAPITest
{
    class YandexAPIDriver : IDriver
    {

        public YandexAPIDriver()
        {
            Auth();
        }

        IDiskApi diskApi = null;

        public void Auth()
        {
            string oauthToken = "<token>";
            diskApi = new DiskHttpApi(oauthToken);
        }

        public IEnumerable<CommonFile> GetFilesList(string path)
        {
            var resourceList = GetFilesListAsync(diskApi, path).Result;
            var commonFiles = new List<CommonFile>();
            foreach (var resource in resourceList)
            {
                commonFiles.Add(ResourceToCommon(resource));
            }
            return commonFiles;
        }

        public void CreateDirectory(string path, string directoryName)
        {
            CreateDirectoryAsync(diskApi, directoryName).Wait();
        }

        public void DownloadFile(string path, string fileName, string localFolder)
        {
            DownloadFileAsync(diskApi, path, fileName, localFolder).Wait();
        }

        public void UploadFile(string path, string fileName, string localFolder)
        {
            UploadFileAsync(diskApi, path, fileName, localFolder).Wait();
        }

        public void RemoveFile(string path, string fileName)
        {
            RemoveFileAsync(diskApi, path, fileName).Wait();
        }

        public void RenameFile(string path, string oldFileName, string newFileName)
        {
            RenameFileAsync(diskApi, path, oldFileName, newFileName).Wait();
        }

        private static async Task DownloadAllFilesInFolder(IDiskApi diskApi)
        {
            //Getting information about folder /foo and all files in it
            Resource fooResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = "/Path", //Folder on Yandex Disk
            }, CancellationToken.None);

            //Getting all files from response
            IEnumerable<Resource> allFilesInFolder =
                fooResourceDescription.Embedded.Items.Where(item => item.Type == ResourceType.File);

            //Path to local folder for downloading files
            string localFolder = @"D:\Download";

            //Run all downloadings in parallel. DiskApi is thread safe.
            IEnumerable<Task> downloadingTasks =
                allFilesInFolder.Select(file =>
                  diskApi.Files.DownloadFileAsync(path: file.Path,
                                                  localFile: System.IO.Path.Combine(localFolder, file.Name)));

            //Wait all done
            await Task.WhenAll(downloadingTasks);
        }

        private static async Task UploadFileAsync(IDiskApi diskApi, string pathToFile, string fileName, string localFolder)
        {
            await diskApi.Files.UploadFileAsync(path: $"{pathToFile}", overwrite: false, localFile: $"{localFolder}{fileName}", cancellationToken: CancellationToken.None);
        }

        private static async Task DownloadFileAsync(IDiskApi diskApi, string pathoToFile, string fileName, string localFolder)
        {
            await diskApi.Files.DownloadFileAsync(path: pathoToFile, localFile: System.IO.Path.Combine(localFolder, fileName));
        }

        private static async Task<IEnumerable<Resource>> GetFilesListAsync(IDiskApi diskApi, string path)
        {
            Resource fooResourceDescription = await diskApi.MetaInfo.GetInfoAsync(new ResourceRequest
            {
                Path = $"/{path}" //Folder on Yandex Disk
            }, CancellationToken.None);

            //Getting all files from response
            IEnumerable<Resource> allFilesInFolder =
                fooResourceDescription.Embedded.Items;
            return allFilesInFolder;
        }

        private static async Task CreateDirectoryAsync(IDiskApi diskApi, string name)
        {
            await diskApi.Commands.CreateDictionaryAsync(name);
        }

        private static async Task RemoveFileAsync(IDiskApi diskApi, string path, string fileName)
        {
           await diskApi.Commands.DeleteAsync(new DeleteFileRequest(), CancellationToken.None);
        }

        private static async Task RenameFileAsync(IDiskApi diskApi, string path, string oldFileName, string newFileName)
        {
            await diskApi.Commands.MoveAsync(new MoveFileRequest
            {
                From = System.IO.Path.Combine(path, oldFileName),
                Path = System.IO.Path.Combine(path, newFileName),
                Overwrite = true
            }, CancellationToken.None);
        }

        private static CommonFile ResourceToCommon(Resource resource)
        {
            return new CommonFile(resource);
        }

    }

}
