namespace RSBotXD
{
    public interface IInDataBase
    {
        public static string serverName;
        abstract void GetData(string dbName, string id);
    }
}