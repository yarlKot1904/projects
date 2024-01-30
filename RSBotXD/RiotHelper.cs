using RiotSharp;
using RiotSharp.Endpoints.ChampionMasteryEndpoint;
using RiotSharp.Endpoints.LeagueEndpoint;
using RiotSharp.Endpoints.MatchEndpoint;
using RiotSharp.Endpoints.SpectatorEndpoint;
using RiotSharp.Endpoints.StaticDataEndpoint.Champion;
using RiotSharp.Endpoints.SummonerEndpoint;
using RiotSharp.Interfaces;
using RiotSharp.Misc;

using System.Diagnostics;

namespace RSBotXD
{
    public partial class RiotHelper
    {
        RiotApi api;
        private static RiotHelper instance;
        private ChampionListStatic championStatic;
        public static RiotHelper Instance { 
            get
            {
                instance ??= new RiotHelper();
                return instance;
            }
        }
        
        private RiotHelper()
        {
            api = RiotApi.GetDevelopmentInstance("RGAPI-dddc8b39-0dd7-46fa-aeae-25a5e26b5190");
            championStatic = api.DataDragon.Champions.GetAllAsync("14.1.1", Language.ru_RU).Result;


        }
        public Summoner? GetData(string name)
        {
            Summoner? summoner = null;
            try
            {
                summoner = api.Summoner.GetSummonerByNameAsync(Region.Ru, name).Result;
                return summoner;

            }
            catch (Exception ex)
            {
                if (summoner != null)
                    Console.WriteLine(summoner.Name);
                else
                    Console.WriteLine("Summoner not found");
                Console.WriteLine(ex.Message);
                // Handle the exception however you want.
                return null;
            }
        }
        public Summoner? GetDataByName(string name)
        {
            try
            {
                var summoner = api.Summoner.GetSummonerByNameAsync(Region.Ru, name).Result;
                return summoner;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Handle the exception however you want.
                return null;
            }
        }



        public CurrentGame GetCurrentGame(string id)
        {
            Summoner summoner = api.Summoner.GetSummonerByNameAsync(Region.Ru, id).Result;
            CurrentGame? currentGame;
            try
            {
                currentGame = api.Spectator.GetCurrentGameAsync(Region.Ru, summoner.Id).Result;
            }
            catch (Exception)
            {
                currentGame = null;
                
            }
            return currentGame;
        }
        //TODO

        public async Task<List<Player>> GetInfoAboutPlayers(string id, CurrentGame currentGame)
        {
            Summoner summoner = await api.Summoner.GetSummonerByNameAsync(Region.Ru, id);
            List<Player> players = new List<Player>();
            string summonerId = "";
            string champName = "";
            currentGame.Participants.ForEach(particiant =>
            {
                summonerId = particiant.SummonerId;
                foreach (var ch in championStatic.Champions)
                {
                    if (ch.Value.Id == particiant.ChampionId)
                        champName = ch.Value.Name;
                }
                Summoner curSummoner = api.Summoner.GetSummonerBySummonerIdAsync(Region.Ru, summonerId).Result;
                var entries = api.League.GetLeagueEntriesBySummonerAsync(Region.Ru, curSummoner.Id).Result;
                string name = api.Account.GetAccountByPuuidAsync(Region.Europe, curSummoner.Puuid).Result.GameName;
                Console.WriteLine(curSummoner.Puuid + " " + particiant.ChampionId);
                ChampionMastery mastery = null;
                try
                {
                    mastery = api.ChampionMastery.GetChampionMasteryAsync(Region.Ru, curSummoner.Puuid, particiant.ChampionId).Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                int wins = 0, loses = 0;
                if (entries != null && entries.Count > 0)
                {
                    wins = entries[0].Wins; //fix may be null
                    loses = entries[0].Losses;
                }
                string rank = GetRankBySummoner(summoner).Result;
                int level = mastery == null ? 0 : mastery.ChampionLevel;
                int points = mastery == null ? 0 : mastery.ChampionPoints;
                Player player = new Player(particiant.SummonerName, name, champName, wins, loses, rank, points, level);
                players.Add(player);
            });
            
            return players;
        }
        public async Task<Match?> GetLastGame(string id)
        {
            Summoner summoner = await api.Summoner.GetSummonerByNameAsync(Region.Ru, id);
            Console.WriteLine(summoner.Name);
            var matchId = await api.Match.GetMatchListAsync(Region.Europe, summoner.Puuid);
            var match = await api.Match.GetMatchAsync(Region.Europe, matchId[0]);
            return match;
            
        }

        public async Task<bool> GetMatchResult(string player)
        {
            Match? lastGame = await GetLastGame(player);
            Summoner summoner = await api.Summoner.GetSummonerByNameAsync(Region.Ru, player);
            if (lastGame == null)
                return false;
            var particpantsId = lastGame.Info.Participants.Single(x => x.SummonerId == summoner.Id);
            var participantsStats = lastGame.Info.Participants.Single(x => x.ParticipantId == particpantsId.ParticipantId);
            return participantsStats.Winner;
        }

        public Summoner GetInfo(string playerName)
        {
            return GetDataByName(playerName);
        }
        public async Task<string> GetRank(string summonerId)
        {
            var summoner = api.Summoner.GetSummonerBySummonerIdAsync(Region.Ru, summonerId).Result;
            Console.WriteLine(summoner.Name + " " + summoner.Id);
            return await GetRankBySummoner(summoner);
        }

        public async Task<string> GetRankBySummoner(Summoner summoner)
        {
            var rank = await api.League.GetLeagueEntriesBySummonerAsync(Region.Ru, summoner.Id);

            if (rank == null)
                return "ОШИБКА";
            foreach (var v in rank)
            {
                Console.WriteLine(v.Tier);
            }
            if (rank.Count > 0)
                return rank[0].Tier + rank[0].Rank;
            else
                return "БЕЗ РАНГА";
        }
    }
}