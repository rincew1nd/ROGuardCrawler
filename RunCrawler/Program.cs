using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROGuardCrawler;

namespace RunCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var crawler = new Crawler();
            crawler.Start();
            Console.ReadKey();
        }
    }
}
