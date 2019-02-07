using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using ROGuardCrawler.Models;
using ROGuardCrawler.Utils;

namespace ROGuardCrawler.Crawlers
{
    public class MonsterCrawler
    {
        private const string Site = "https://www.roguard.net";
        private const string MonsterPageUrl = "/db/monsters/?page={0}";
        private const string MonsterLinkXPath = "//*[@id='content']/div/table/tbody/tr/td[2]/div[1]/a";
        private const string CommonXPath = "//*[@id='content']/div/div[h2[contains(text(), 'Common')]]/table/tbody";
        private const string AttributesXPath = "//*[@id='content']/div/div[h2[contains(text(), 'Attributes')]]/table[1]/tbody";
        private const string StatXPath = "//*[@id='content']/div/div[h2[contains(text(), 'Attributes')]]/table[2]/tbody";
        private const string CardXPath = "//*[@id='content']/div/div[h2[contains(text(), 'Card')]]/div/a";
        private const string ItemXPath = "//*[@id='content']/div/div[h2[contains(text(), 'Drops')]]";
        private const string LocationsXPath = "//*[@id='content']/div/div[h2[contains(text(), 'Locations')]]";
        private const string PageCountXPath = "//*[@id='content']/div/nav[1]/ul/li[last()]/a";

        private readonly List<Monster> _monsters;
        private readonly int _pageCount;

        public MonsterCrawler()
        {
            _monsters = new List<Monster>();
            _pageCount = FetchPageCount();
        }

        private int FetchPageCount()
        {
            var webPage = new HtmlWeb();
            var firstMainPage = webPage.Load(string.Format(Site+MonsterPageUrl, 1));
            var pageCountString = firstMainPage.DocumentNode.GetInnerTextSafe(PageCountXPath);
            return int.Parse(pageCountString);
        }

        public List<Monster> LoadMonsterPages()
        {
            var monsterTasks = new List<Task>();

            for (var i = 1; i <= _pageCount; i++)
            {
                var webPage = new HtmlWeb();
                var mainMonsterPage = webPage.Load(string.Format(Site + MonsterPageUrl, i));
                var monsterPages = mainMonsterPage.DocumentNode.SelectNodes(MonsterLinkXPath);
                foreach (var monsterPage in monsterPages)
                {
                    var monsterTask = new Task(() => LoadMonsterData(monsterPage.InnerText, monsterPage.Attributes["href"].Value));
                    monsterTask.Start();
                    monsterTasks.Add(monsterTask);
                }
            }

            Task.WaitAll(monsterTasks.ToArray());
            return _monsters;
        }

        private void LoadMonsterData(string name, string monsterPageUrl)
        {
            var monster = new Monster()
            {
                Loot = new List<(int, int, float)>(),
                Locations = new List<int>()
            };

            var webPage = new HtmlWeb();
            var mainMonsterPage = webPage.Load(string.Format(Site + monsterPageUrl, 1));

            //Parse monster ID
            var urlParts = monsterPageUrl.Split('/');
            if (urlParts.Length >= 2)
            {
                monster.Id = urlParts[urlParts.Length - 2].SafeParse<int>();
            }
            monster.Name = name;

            //Parse common monster attributes
            var monsterCommonTable = mainMonsterPage.DocumentNode.SelectSingleNode(CommonXPath);
            if (monsterCommonTable == null)
            {
                Console.WriteLine($"Monster {monster.Name}({monster.Id}) common table not found!");
            }
            else
            {
                monster.Level = monsterCommonTable.GetInnerTextSafe("./tr[2]/td[2]").SafeParse<int>();
                monster.Race = EnumHelper.ParseEnum<MonsterRace>(monsterCommonTable.GetInnerTextSafe("./tr[5]/td[2]").FullTrim("-"));
                monster.Element = EnumHelper.ParseEnum<MonsterElement>(monsterCommonTable.GetInnerTextSafe("./tr[6]/td[2]"));
                monster.Size = EnumHelper.ParseEnum<MonsterSize>(monsterCommonTable.GetInnerTextSafe("./tr[7]/td[2]"));
                monster.PassiveLevel = monsterCommonTable.GetInnerTextSafe("./tr[8]/td[2]").SafeParse<int>();
                monster.BaseExp = monsterCommonTable.GetInnerTextSafe("./tr[9]/td[2]").SafeParse<int>();
                monster.JobExp = monsterCommonTable.GetInnerTextSafe("./tr[10]/td[2]").SafeParse<int>();
            }

            //Parse monster stats
            var monsterStatTable = mainMonsterPage.DocumentNode.SelectSingleNode(StatXPath);
            if (monsterStatTable == null)
            {
                Console.WriteLine($"Monster {monster.Name}({monster.Id}) stat table not found!");
            }
            else
            {
                monster.Str = monsterStatTable.GetInnerTextSafe("./tr[1]/td[2]").SafeParse<int>();
                monster.Dex = monsterStatTable.GetInnerTextSafe("./tr[2]/td[2]").SafeParse<int>();
                monster.Int = monsterStatTable.GetInnerTextSafe("./tr[3]/td[2]").SafeParse<int>();
                monster.Vit = monsterStatTable.GetInnerTextSafe("./tr[4]/td[2]").SafeParse<int>();
                monster.Agi = monsterStatTable.GetInnerTextSafe("./tr[5]/td[2]").SafeParse<int>();
                monster.Luk = monsterStatTable.GetInnerTextSafe("./tr[6]/td[2]").SafeParse<int>();
            }

            //Parse attributes stats
            var monsterAttributesTable = mainMonsterPage.DocumentNode.SelectSingleNode(AttributesXPath);
            if (monsterAttributesTable == null)
            {
                Console.WriteLine($"Monster {monster.Name}({monster.Id}) attributes table not found!");
            }
            else
            {
                monster.Atk = monsterAttributesTable.GetInnerTextSafe("./tr[1]/td[2]").SafeParse<int>();
                monster.Matk = monsterAttributesTable.GetInnerTextSafe("./tr[2]/td[2]").SafeParse<int>();
                monster.Def = monsterAttributesTable.GetInnerTextSafe("./tr[3]/td[2]").SafeParse<int>();
                monster.Mdef = monsterAttributesTable.GetInnerTextSafe("./tr[4]/td[2]").SafeParse<int>();
                monster.Health = monsterAttributesTable.GetInnerTextSafe("./tr[5]/td[2]").SafeParse<int>();
                monster.Hit = monsterAttributesTable.GetInnerTextSafe("./tr[6]/td[2]").SafeParse<int>();
                monster.Flee = monsterAttributesTable.GetInnerTextSafe("./tr[7]/td[2]").SafeParse<int>();
                monster.MoveSpd = monsterAttributesTable.GetInnerTextSafe("./tr[8]/td[2]").SafeParse<float>();
                monster.AtkSpd = monsterAttributesTable.GetInnerTextSafe("./tr[9]/td[2]").SafeParse<float>();
            }
            
            //Parse loot data
            var monsterLootTable = mainMonsterPage.DocumentNode.SelectSingleNode(ItemXPath);
            if (monsterLootTable == null)
            {
                Console.WriteLine($"Monster {monster.Name}({monster.Id}) loot table not found!");
            }
            else
            {
                foreach (var item in monsterLootTable.SelectNodes("./div"))
                {
                    var countStr = item.InnerText.IndexOf("&times;", StringComparison.Ordinal);
                    var chanceStr = Regex.Match(item.InnerText, "(\\w|\\s)+\\((\\d+\\.\\d+)%\\)");
                    var itemUrlParts = item.SelectSingleNode("./a").Attributes["href"].Value.Split('/');
                    
                    if (itemUrlParts.Length >= 2)
                    {
                        var count = 0;
                        float chance = 0;

                        var itemId = itemUrlParts[itemUrlParts.Length - 2].SafeParse<int>();
                        if (countStr >= 0)
                        {
                            count = item.InnerText.Substring(0, countStr).Trim(' ').SafeParse<int>();
                        }
                        if (chanceStr.Groups.Count >= 2 && Regex.IsMatch(chanceStr.Groups[2].Value, "\\d+\\.\\d+"))
                        {
                            chance = chanceStr.Groups[2].Value.SafeParse<float>();
                        }

                        monster.Loot.Add((itemId, count, chance));
                    }
                    else
                    {
                        Console.WriteLine($"Item id not found for {item.InnerText}!");
                    }
                }
            }

            //Parse monster card ID
            var monsterCard = mainMonsterPage.DocumentNode.SelectSingleNode(CardXPath);
            if (monsterCard == null)
            {
                Console.WriteLine($"Monster {monster.Name}({monster.Id}) card not found!");
            }
            else
            {
                var itemUrlParts = monsterCard.Attributes["href"].Value.Split('/');
                if (itemUrlParts.Length >= 2)
                {
                    var itemId = itemUrlParts[itemUrlParts.Length - 2].SafeParse<int>();
                    monster.Loot.Add((itemId, 0, 0));
                }
            }

            //Parse monster locations
            var monsterLocationTable = mainMonsterPage.DocumentNode.SelectSingleNode(LocationsXPath);
            if (monsterLocationTable == null)
            {
                Console.WriteLine($"Monster {monster.Name}({monster.Id}) card not found!");
            }
            else
            {
                foreach (var location in monsterLocationTable.SelectNodes("./a"))
                {
                    var locationUrlParts = location.Attributes["href"].Value.Split('/');
                    if (locationUrlParts.Length >= 2)
                    {
                        monster.Locations.Add(locationUrlParts[locationUrlParts.Length - 2].SafeParse<int>());
                    }
                }
            }

            _monsters.Add(monster);
        }
    }
}
