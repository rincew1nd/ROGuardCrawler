using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROGuardCrawler.Models
{
    public class Monster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public MonsterRace Race { get; set; }
        public MonsterElement Element { get; set; }
        public MonsterSize Size { get; set; }
        public int PassiveLevel { get; set; }
        public int BaseExp { get; set; }
        public int JobExp { get; set; }

        public int Str { get; set; }
        public int Dex { get; set; }
        public int Int { get; set; }
        public int Vit { get; set; }
        public int Agi { get; set; }
        public int Luk { get; set; }
        
        public int Atk { get; set; }
        public int Matk { get; set; }
        public int Def { get; set; }
        public int Mdef { get; set; }
        public int Health { get; set; }
        public int Hit { get; set; }
        public int Flee { get; set; }
        public float MoveSpd { get; set; }
        public float AtkSpd { get; set; }

        public List<(int, int, float)> Loot;
        
        public List<int> Locations;
    }
}
