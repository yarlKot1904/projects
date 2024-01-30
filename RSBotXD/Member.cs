namespace RSBotXD
{
    public class Member : IInDataBase 
    {
        private static Dictionary<int, int> levels = new Dictionary<int, int>(){
            { 0, 0 }, { 1, 10 }, { 2, 100 }, { 3, 1000 }, { 4, 5000 }, {5, int.MaxValue}
        };

        public static Dictionary<int, int> Levels { get => levels; set => levels = value; }


        public string leagueName;
        public string discordName;
        public int xp;
        public int level;
        public string rollChampion;
        public State state = State.NOTPLAYING;

        public Member(string leagueName, string discordName, int xp = 0, int level = 0)
        {
            this.leagueName = leagueName;
            this.discordName = discordName;
            this.xp = xp;
            this.level = level;
        }

        public void GetData(string dbName, string id)
        {
            throw new NotImplementedException();
        }
        public void AddXp(int count)
        {
            xp+=count;
            if (Levels[level] < xp)
            {
                level++;
                Program.AnnounceLevel(this);
                DataBaseHelper.Instance.ChangeLevel(this, level);
            }
            DataBaseHelper.Instance.ChangeXp(this, xp);
            Console.WriteLine(xp);
        }
    }
    public enum State
    {
        PLAYING,
        NOTPLAYING
    }
}