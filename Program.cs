using System;
using Newtonsoft.Json;
using OpenScraping;
using OpenScraping.Config;
using HtmlAgilityPack;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Парсер
{
    class Program
    {
        static void Main(string[] args)
        {
            var configJson = @"
            {
                '':'//a[contains(text(), \'File\')]/@href'
            }
            ";

            var config = StructuredDataConfig.ParseJsonString(configJson);

            var html = @"http://rule34.paheal.net/post/list";
            HtmlWeb web = new HtmlWeb();
            WebClient wc = new WebClient();
            var htmlDoc = web.Load(html);
            var body = htmlDoc.Text;
            var path = Directory.GetCurrentDirectory() + "\\img\\";

            void CreataFolder()
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            CreataFolder();
            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(body);
            char[] charstotrim = { '\x5C', '\x22', '\x7B', '\x20' };
            var output = JsonConvert.SerializeObject(scrapingResults, Formatting.Indented).Split(',');
            output[0] = output[0].Remove(0, 10);
            output[output.Length-1] = output[output.Length-1].Remove(output[output.Length-1].Length-8, 8);
            string fileName;
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = output[i].Remove(0, 7);
                output[i] = output[i].Trim(charstotrim);
                if (output[i].Contains("webm"))
                {
                    fileName = path + i + ".webm";
                    Console.WriteLine(fileName);
                }
                else
                {
                    fileName = path +i + "." + output[i].Remove(0,output[i].Length - 3);
                    Console.WriteLine(fileName);
                }
               
                wc.DownloadFile((string)output[i],fileName);
            }
            Console.WriteLine("----------------------------");
            for(int i = 0; i < output.Length; i++)
            {
                Console.WriteLine(output[i]);
            }
            Console.ReadKey();
    
        }
    }
}
