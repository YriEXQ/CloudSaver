using System;
using System.Collections.Generic;
using System.Text;
using YandexDisk.Client.Protocol;

namespace DriveLinker
{
    class CommonFile
    {
        public string name;

        public CommonFile(Resource resource)
        {
            name = resource.Name;
        }
    }
}
