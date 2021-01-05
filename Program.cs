using DriveLinker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverYandexAPI;

namespace CloudSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            IDriver yandex = new YandexAPIDriver();
            var filesList = yandex.GetFilesList("");
            foreach (var element in filesList)
            {
                Console.WriteLine(element.name);
            }
            yandex.CreateDirectory("/", "Mama");
            filesList = yandex.GetFilesList("");
            foreach (var element in filesList)
            {
                Console.WriteLine(element.name);
            }
            yandex.RenameFile("/", "Mama", "Papa");
            filesList = yandex.GetFilesList("");
            foreach (var element in filesList)
            {
                Console.WriteLine(element.name);
            }
            //yandex.UploadFile("/Papa/Mama.txt", "Mama.txt", @"D:\");
            //filesList = yandex.GetFilesList("");
            //foreach (var element in filesList)
            //{
            //    Console.WriteLine(element.name);
            //}
        }
    }
}
