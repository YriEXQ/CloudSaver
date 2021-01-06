using DriveLinker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverYandexAPI;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace CloudSaver
{
    class Program
    {
        static void Main(string[] args)
        {
            //RegistryKey key;
            //key = Registry.ClassesRoot.CreateSubKey("cloudsaver");
            //key.SetValue("", "URL: CloudSaver Protocol");
            //key.SetValue("URL Protocol", "");
            //key = key.CreateSubKey("shell");
            //key = key.CreateSubKey("open");
            //key = key.CreateSubKey("command");
            //key.SetValue("", "D:\\Documents\\GitHub\\CloudSaver\\bin\\Debug\\CloudSaver.exe");

            string[] result = new string[5];
            string token = "";
            foreach (string s in args)
            {
                Console.WriteLine("\t" + s);
                result = Regex.Split(s, @"(?<=\=)(.*?)(?=\&)");
            }
            if (result[1] != null)
            {
                token = result[1];
            }
            if (result != null)
            {
                foreach (var r in result)
                {
                    Console.WriteLine("\t" + r);
                }
            }
            Console.WriteLine("----");
            Console.WriteLine("Нажмите Enter для продолжения");
            Console.ReadLine();
            if (token == "")
            {
                string loginRequest = "https://oauth.yandex.ru/authorize?response_type=token&client_id=8a13de9b8e344b21867b99ddb8207158&redirect_uri=cloudsaver://token";
                System.Diagnostics.Process.Start(loginRequest);
                return;
            }



            IDriver yandex = new YandexAPIDriver(token);
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
            Console.WriteLine("----");
            Console.WriteLine("Нажмите Enter для выхода");
            Console.ReadLine();
            //yandex.UploadFile("/Papa/Mama.txt", "Mama.txt", @"D:\");
            //filesList = yandex.GetFilesList("");
            //foreach (var element in filesList)
            //{
            //    Console.WriteLine(element.name);
            //}
        }
    }
}
