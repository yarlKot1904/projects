using RiotSharp.Endpoints.StaticDataEndpoint.Champion;
using System.Data;
using System.Data.SqlClient;

namespace RSBotXD
{
    public class DataBaseHelper
    {
        private static DataBaseHelper instance;
        public static DataBaseHelper Instance
        {
            get
            {
                instance ??= new DataBaseHelper();
                return instance;
            }
        }
        private SqlConnection sqlConnection;
        public void Init()
        {
            sqlConnection = new SqlConnection(@"Data Source=DESKTOP-KBSMD19;Initial Catalog=MainDataBase;Integrated Security=True");
            //OpenConnection();
        }

        private void OpenConnection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        private void CloseConnection()
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public void AddMember(Member member)
        {

            string queryString = $"insert into members_db (nameDiscord, nameLeague, xp, playerLevel) values('{member.discordName}', '{member.leagueName}', {member.xp}, {member.level})";
            SqlCommand command = new SqlCommand(queryString, sqlConnection);

            OpenConnection();
            if (command.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("Все ок");
            }
            else
            {
                Console.WriteLine("Все не ок");
            }
            CloseConnection();
        }
        public List<Member> GetMembers()
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            DataTable table = new DataTable();
            string queryString = $"select * from members_db";
            SqlCommand command = new SqlCommand(queryString, sqlConnection);

            sqlDataAdapter.SelectCommand = command;
            sqlDataAdapter.Fill(table);
            if (table.Rows.Count == 0)
            {
                return new List<Member>();
            }
            foreach (DataColumn column in table.Columns)
                Console.Write("\t{0}", column.ColumnName);
            List<Member> members = new List<Member>();
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    Console.Write("\t{0}", row[column]);
                }
                string discordName = row[1].ToString();
                string leagueName = row[2].ToString();
                int xp = int.Parse(row[3].ToString());
                int level = int.Parse(row[4].ToString());
                Member member = new(leagueName, discordName, xp, level);
                members.Add(member);

            }
            return members;
        }

        public Task ChangeXp(Member member, int xp)
        {
            string queryString = $"update members_db set xp = {xp} where nameDiscord = '{member.discordName}'";
            SqlCommand command = new SqlCommand(queryString, sqlConnection);

            OpenConnection();
            if (command.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("Все ок");
            }
            else
            {
                Console.WriteLine("Все не ок");
            }
            CloseConnection();
            return Task.CompletedTask;
        }
        public Task ChangeLevel(Member member, int level)
        {
            string queryString = $"update members_db set playerLevel = {level} where nameDiscord = '{member.discordName}'";
            SqlCommand command = new SqlCommand(queryString, sqlConnection);

            OpenConnection();
            if (command.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("Все ок");
            }
            else
            {
                Console.WriteLine("Все не ок");
            }
            CloseConnection();
            return Task.CompletedTask;
        }
    }
}