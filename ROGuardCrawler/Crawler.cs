using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROGuardCrawler.Crawlers;
using ROGuardCrawler.Utils;

namespace ROGuardCrawler
{
    public class Crawler
    {
        private MonsterCrawler _monsterCrawler;
        private ItemCrawler _itemCrawler;

        public Crawler()
        {
            _monsterCrawler= new MonsterCrawler();
            _itemCrawler = new ItemCrawler();
        }

        public void Start()
        {
            var monsters = _monsterCrawler.LoadMonsterPages();
        }
    }
}
