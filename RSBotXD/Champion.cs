using Discord;

namespace RSBotXD
{

    internal class Champion
    {
        public string name;
        public string description;
        public List<string> tags = new List<string>();

        public Champion(string name, string description, List<string> tags = null)
        {
            this.name = name;
            this.description = description;
            this.tags = tags;
        }
        public Champion(string lineToParse)
        {
            var stringList = lineToParse.Split('|');
            if (stringList.Length == 0)
            {
                name = "Error";
                description = "Error";
                return;
            }
            for (int i = 0; i < stringList.Length; i++)
            {
                if (i == 0)
                {
                    name = stringList[i];
                    continue;
                }
                if (i == 1)
                {
                    description = stringList[i];
                    continue;
                }
                tags.Add(stringList[i]);
            }
        }
    }
    internal class Team
    {
        public List<string> names = new List<string>(5) {"","","","",""};
        public string description;

        public Team(List<string> names, string description)
        {
            this.names = names;
            this.description = description;
        }
        public Team(string lineToParse)
        {
            var stringList = lineToParse.Split('|');
            if (stringList.Length == 0)
            {
                names.Add("Error");
                description = "Error";
                return;
            }
            for (int i = 0; i < stringList.Length; i++)
            {
                if (i < 5)
                {
                    names[i] = stringList[i];
                    continue;
                }
                if (i == 5)
                {
                    description = stringList[i];
                    continue;
                }
            }
        }
    }
}