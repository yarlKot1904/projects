using System.Text;

namespace RSBotXD
{
    public class Player
    {
        public string riotName;
        public string name;
        public string champion;
        public int wins;
        public int loses;
        public string rank;
        public int champMastery;
        public int champRank;

        public Player(string riotName, string name, string champion, int hotStreak, int coldStreak, string rank, int champMastery, int champRank)
        {
            this.riotName = riotName;
            this.name = name;
            this.champion = champion;
            this.wins = hotStreak;
            this.loses = coldStreak;
            this.rank = rank;
            this.champMastery = champMastery;
            this.champRank = champRank;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(riotName + " " + name + " " + champion + " Побед" + wins + " Антипобед" + loses + " Ранг:" + rank + " " + champRank + "РАНГ" + champMastery);
            return stringBuilder.ToString();
        }
    }
}