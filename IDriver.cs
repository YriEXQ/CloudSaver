using System;
using System.Collections.Generic;
using System.Text;

namespace DriveLinker
{
    interface IDriver
    {
        public void Auth();
        public IEnumerable<CommonFile> GetFilesList(string path);
        public void CreateDirectory(string path, string directoryName);
        public void DownloadFile(string path, string fileName, string localFolder);
        public void UploadFile(string path, string fileName, string localFolder);
        public void RemoveFile(string path, string fileName);
        public void RenameFile(string path, string oldFileName, string newFileName);

    }
}
