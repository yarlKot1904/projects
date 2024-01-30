using Discord;
using Discord.WebSocket;
using Discord.Net;
using Newtonsoft.Json;
using RiotSharp.Endpoints.SummonerEndpoint;
using System.Text.RegularExpressions;
using RiotSharp.Endpoints.SpectatorEndpoint;
using System.IO;

namespace RSBotXD
{
    class Program
    {
        private const long MainId = 1195760503146950758;
        private const long MessageId = 1200504806792515676;
        private const long RulesChatId = 1200508198340280340;
        private const long DefaultRole = 1200505398763995156;
        private static string matveyName = "Pepl";
        private const long KsuId = 693438453014069278;
        private static DiscordSocketClient client;
        List<Champion> championsList;
        List<Team> teamList;
        string regexpThx = @"спасибо.*";
        string regexpFck = @"иди .*";
        string regexpFck2 = @"пошел .*";
        
        Dictionary<string, Member> members = new Dictionary<string, Member>();

        DiscordSocketConfig config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
            
        };

        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            
            DataBaseHelper.Instance.Init();
            List<Member> memberList = DataBaseHelper.Instance.GetMembers();
            foreach (var item in memberList)
            {
                members.Add(item.discordName, item);
            }
            client = new DiscordSocketClient(config);
            client.MessageReceived += CommandsHandler;
            
            client.Log += Log;
            client.UserJoined += AnnounceJoinedUser;
            client.UserLeft += AnnounceLeaveUser;
            client.Ready += Client_Ready;
            client.SlashCommandExecuted += SlashCommandHandler;

            //Console.WriteLine(RiotHelper.Instance.GetLastGame("yarlKot").Result.Info.Teams[0].Win);
            //Summoner summoner = RiotHelper.Instance.GetData("yarlKot");
            //Match? lastGame = await RiotHelper.Instance.GetLastGame("yarlKot");

            //var particpantsId = lastGame.Info.Participants.Single(x => x.SummonerId == summoner.Id);
            //var participantsStats = lastGame.Info.Participants.Single(x => x.ParticipantId == particpantsId.ParticipantId);
            //if (particpantsId.Winner)
            //    Console.WriteLine("ПОБЕДА");
            //await RiotHelper.Instance.GetInfoAboutPlayer("yarlKot");

            var token = "---";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            await CheckCycle();

            //var cmds = await client.GetGlobalApplicationCommandsAsync();
            //SocketApplicationCommand? cmd = null;
            //foreach (var item in cmds)
            //{
            //    if (item.Name == "матвик")
            //        cmd = item;
            //}

            //if (cmd != null)
            //{
            //    Console.WriteLine("удаляю");
            //    await cmd.DeleteAsync();
            //}


            Console.ReadLine();
        }

        private async Task AnnounceLeaveUser(SocketGuild socketGuild, SocketUser user)
        {
            var channel = client.GetChannel(MainId) as SocketTextChannel;
            var rulesChannel = client.GetChannel(RulesChatId) as SocketTextChannel;
            //await user.SendMessageAsync($"Спасибо за игру! Нам будет тя не хватать UwU");
            await channel.SendMessageAsync($"Бро {user.Username} ливнул, пинайте его, унижайте");
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.CommandName)
            {
                case "привет":
                    {
                        await command.RespondAsync($"Привет, {command.User.Username}");
                        break;
                    }
                case "рандом":
                    {
                        Random rnd = new Random();
                        await command.RespondAsync($"Выпало число {rnd.Next(-1000, 1000)}");
                        break;
                    }
                case "рулетка":
                    {
                        await Roulete(command);
                        break;
                    }
                case "ролл":
                    {
                        await Roll(command);
                        break;
                    }
                case "регистрация":
                    {
                        await RegisterAsync(command);
                        break;
                    }
                case "обо_мне":
                    {
                        await PrintStats(command);
                        break;
                    }
                case "обзови":
                    {
                        await Taunt(command);
                        break;
                    }
                case "прикол":
                    {
                        await SendMatvey(command);
                        break;
                    }
            }
        }

        

        public async Task Client_Ready()
        {
            await BuildCommands();
        }

        private async Task BuildCommands()
        {
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("ролл");
            guildCommand.WithDescription("Выбрать случайного чемпиона").AddOption(new SlashCommandOptionBuilder()
            .WithName("роль")
            .WithDescription("роль, на которой вы хотите играть")
            .WithRequired(true)
            .AddChoice("топ", "топ")
            .AddChoice("лес", "лес")
            .AddChoice("мид", "мид")
            .AddChoice("адк", "адк")
            .AddChoice("сап", "сап")
            .AddChoice("команда", "команда")
            .WithType(ApplicationCommandOptionType.String)
        );

            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("рулетка");
            globalCommand.WithDescription("кикаем балбеса");

            var globalCommand2 = new SlashCommandBuilder();
            globalCommand2.WithName("регистрация");
            globalCommand2.WithDescription("введите свой ЛОГИН входа в игру(не имя или ник)").AddOption("login", ApplicationCommandOptionType.String, "ЛОГИН для входа в игру", true);
            var globalCommand3 = new SlashCommandBuilder();
            globalCommand3.WithName("обо_мне");
            globalCommand3.WithDescription("выведет ваш ранг, если вы зарегистрированы в системе");

            var globalCommand4 = new SlashCommandBuilder();
            globalCommand4.WithName("обзови");
            globalCommand4.WithDescription("обзывает пользователя").AddOption("user", ApplicationCommandOptionType.User, "Кого обозвать", isRequired: true);

            var globalCommand5 = new SlashCommandBuilder();
            globalCommand5.WithName("прикол");
            globalCommand5.WithDescription("Делает чувачка с текстом").AddOption(new SlashCommandOptionBuilder()
            .WithName("кого")
            .WithDescription("с кем картинку")
            .WithRequired(true)
            .AddChoice("матвик", "матвик")
            .AddChoice("пабло", "пабло")
            .AddChoice("пабло2", "пабло2")
            .AddChoice("террор", "террор")
            .AddChoice("долбоеб?", "долбоеб")
            .AddChoice("артефакт?", "артефакт")
            .AddChoice("ахегао?", "ахегао")
            .WithType(ApplicationCommandOptionType.String)
        ).AddOption("текст", ApplicationCommandOptionType.String, "Текст для вставки", isRequired: true);
            try
            {
                await client.CreateGlobalApplicationCommandAsync(guildCommand.Build());

                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build());

                await client.CreateGlobalApplicationCommandAsync(globalCommand2.Build());

                await client.CreateGlobalApplicationCommandAsync(globalCommand3.Build());

                await client.CreateGlobalApplicationCommandAsync(globalCommand4.Build());

                await client.CreateGlobalApplicationCommandAsync(globalCommand5.Build());
                
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandsHandler(SocketMessage msg)
        {
            if (!msg.Author.IsBot)
            {
                if (Regex.IsMatch(msg.Content, regexpThx, RegexOptions.IgnoreCase))
                {
                    msg.Channel.SendMessageAsync($"Да без б, {msg.Author.Mention})", messageReference: msg.Reference);
                    Emote emote = Emote.Parse("<:fh:1200507091383762944>");
                    msg.AddReactionAsync(emote);
                }
                if(Regex.IsMatch(msg.Content, regexpFck, RegexOptions.IgnoreCase))
                {
                    msg.Channel.SendMessageAsync($"Сам иди, {msg.Author.Mention})", messageReference: msg.Reference);
                    Emote emote = Emote.Parse("<:fh:1200507091383762944>");
                    msg.AddReactionAsync(emote);
                }
                if(Regex.IsMatch(msg.Content, regexpFck2, RegexOptions.IgnoreCase))
                {
                    msg.Channel.SendMessageAsync($"Пошел ТЫ, {msg.Author.Mention})", messageReference: msg.Reference);
                    Emote emote = Emote.Parse("<:fh:1200507091383762944>");
                    msg.AddReactionAsync(emote);
                }
            }    
            return Task.CompletedTask;
        }

        private Task Roulete(SocketSlashCommand command)
        {
            Task complete = Task.CompletedTask;
            Random rnd = new Random();
            
            SocketGuildUser user = command.User as SocketGuildUser;
            SocketVoiceChannel voice = user.VoiceChannel;
            if (voice == null)
            {
                command.RespondAsync($"Ты, {user.Mention}, не в канале)");
                return complete;
            }
            List<SocketGuildUser> users = GetUsersInVoice(voice);

            int random = rnd.Next(0, 100);
            if (random < 50 + 5*users.Count)
            {
                foreach(var u in users)
                {
                    Console.WriteLine(u.GlobalName);
                    if(u.GlobalName == matveyName)
                    {
                        KickMember(u);
                        Console.Write("МАТВЕЙ");
                        command.RespondAsync($"Ты, {u.Mention}, убит)");
                        return complete;
                    }
                }
            }

            int count = users.Count;
            random = rnd.Next(0, count);
            command.RespondAsync($"Ты, {users.ElementAt(random).Mention}, убит)");
            KickMember(users.ElementAt(random));
            return complete;
        }

        private static List<SocketGuildUser> GetUsersInVoice(SocketVoiceChannel voice)
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();
            Console.WriteLine(voice.Name);
            foreach (var u in voice.Users)
            {
                if (u.VoiceChannel == voice)
                    users.Add(u);
            }

            return users;
        }

        private async void KickMember(SocketUser member)
        {
            if(member == null)
            {
                Console.WriteLine("ОШИБКА");
                return;
            }
            
            SocketGuildUser? user = member as SocketGuildUser;
            if (user == null)
                return;
            await user.ModifyAsync(x => x.Channel = null);
        }
        private Task Roll(SocketSlashCommand msg)
        {
            championsList ??= GetListOfChamps("C:\\Users\\Егор\\source\\repos\\RSBotXD\\Champs.txt");
            teamList ??= GetListOfTeam("C:\\Users\\Егор\\source\\repos\\RSBotXD\\Combos.txt");
            SelectChampion(msg);
            return Task.CompletedTask;
        }

        private void SelectChampion(SocketSlashCommand msg)
        {
            bool succes = false;
            string? role = msg.Data.Options.First().Value.ToString();
            if (role == "команда")
            {
                int count = teamList.Count;
                Random rnd = new Random();
                int random = rnd.Next(0, count);
                msg.RespondAsync($"Ваша команда: {Format.Underline(string.Join(",", teamList[random].names.ToArray()))}, {Format.Italics(teamList[random].description)}");
                return;
            }
            while (!succes)
            {
                
                Console.WriteLine(role);

                int count = championsList.Count;
                Random rnd = new Random();
                int random = rnd.Next(0, count);
                if (role == null)
                    return;
                if (!championsList[random].tags.Contains(role))
                {
                    continue;
                }
                succes = true;

                msg.RespondAsync($"Ты, {msg.User.Mention}, играешь на: {Format.Underline(championsList[random].name)}: {Format.Italics(championsList[random].description)}, роли: {string.Join(", ", championsList[random].tags)}");
            }
        }
        

        private List<Champion> GetListOfChamps(string path)
        {
            List<Champion> champions = new List<Champion>();
            StreamReader reader = File.OpenText(path);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                champions.Add(new Champion(line));
                Console.WriteLine(line);
            }
            reader.Close();
            return champions;
        }
        private List<Team> GetListOfTeam(string path)
        {
            List<Team> teams = new List<Team>();
            StreamReader reader = File.OpenText(path);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                teams.Add(new Team(line));
                Console.WriteLine(line);
            }
            reader.Close();
            return teams;
        }
        private async Task RegisterAsync(SocketSlashCommand msg)
        {
            if (members.ContainsKey(msg.User.Username))
            {
                await msg.RespondAsync($"Ты, {msg.User.Mention}, уже зареган)");
                return;
            }
            Summoner? summoner = RiotHelper.Instance.GetData(msg.Data.Options.First().Value.ToString());
            if(summoner == null)
            {
                await msg.RespondAsync($"Ты, {msg.User.Mention}, что-то не так сделал");
                return;
            }
            await msg.RespondAsync($"Ты, {msg.User.Mention}, все супер сделал!");
            await Register(msg.User as SocketGuildUser, summoner.Name);
            
            return;
        }

        private Task Register(SocketGuildUser user, string discordName)
        {
            Member member = new Member(discordName, user.Username);
            members.Add(user.Username, member);
            DataBaseHelper.Instance.AddMember(member);
            return Task.CompletedTask;
        }


        private async Task CheckCycle()
        {
            int tickCounter = 10;
            while (true)
            {
                await Task.Delay(20000);
                tickCounter++;
                foreach (var u in members)
                {
                    Member member = u.Value;
                    Summoner summoner = RiotHelper.Instance.GetData(member.leagueName);
                    if (summoner == null || !(client.GetUser(u.Key).Status != UserStatus.Offline))
                        continue;
                    CurrentGame game = RiotHelper.Instance.GetCurrentGame(member.leagueName);
                    if (game == null)
                    {
                        if (member.state == State.PLAYING)
                        {
                            member.state = State.NOTPLAYING;
                            Console.WriteLine(member.state);

                            if (tickCounter >= 3)
                            {
                                tickCounter = 0;
                                Console.WriteLine("Не поздр");
                                await Congrats(tickCounter, u);

                            }
                            bool win = RiotHelper.Instance.GetMatchResult(member.leagueName).Result;
                            if (win)
                            {
                                member.AddXp(3);
                            }
                            else
                            {
                                member.AddXp(1);
                            }
                        }
                    }
                    else
                    {
                        if (member.state == State.NOTPLAYING)
                        {
                            member.state = State.PLAYING;
                            if (tickCounter >= 3)
                            {
                                var channel = client.GetChannel(MessageId) as IMessageChannel;
                                await channel.SendMessageAsync("ХОРОШЕЙ ИГРЫ!!!!!!!!!!!!!");
                                List<Player> players = await RiotHelper.Instance.GetInfoAboutPlayers(member.leagueName, game);
                                await channel.SendMessageAsync(ListPlayersToString(players));
                                tickCounter = 0;
                            }
                        }
                    }
                }
            }
        }
        
        private static string ListPlayersToString(List<Player> players)
        {
            string result = "+ -----------------------------------------------+\n";
            foreach (var player in players)
            {
                int games = player.loses + player.wins;
                float percent;
                if (games == 0)
                    percent = 0;
                else
                    percent = ((float)(player.wins * 10000 / games )) / 100;
                result += Format.Bold(player.name) + "    " + player.rank + "    ВИНРЕЙТ " + percent + "%    ВСЕГО ИГР " + (player.wins + player.loses) +
                    "\n" + Format.Bold(player.champion) + "    " + player.champRank + " РАНГ,    "+ player.champMastery + " ОЧКОВ МАСТЕРСТВА\n + -----------------------------------------------+\n";
            }
            return result;
        }

        private async Task<int> Congrats(int tickCounter, KeyValuePair<string, Member> u)
        {
            await Task.Delay(20000);
            bool win = RiotHelper.Instance.GetMatchResult(u.Value.leagueName).Result;
            var channel = client.GetChannel(MessageId) as IMessageChannel;
            await channel.SendMessageAsync("ПОЗДРАВЛЯЮ С " + (win ? "Победой, на лаки вывезли))" : "поражением, нт, боты))))"));
            Console.WriteLine("поздр");
            tickCounter = 0;
            return tickCounter;
        }

        public static async void AnnounceLevel(Member member)
        {
            var channel = client.GetChannel(MessageId) as IMessageChannel;
            var user = client.GetUser(member.discordName);
            if (user == null || channel == null)
                return;
            await channel.SendMessageAsync($"Пользователь {user.Mention} достигает уровня {member.level}");
        }

        private async Task PrintStats(SocketSlashCommand msg)
        {
            string id = members[msg.User.Username].leagueName;
            Summoner summoner = RiotHelper.Instance.GetInfo(id);
            members[msg.User.Username].AddXp(0);
            string temp = RiotHelper.Instance.GetMatchResult(id).Result ? "ПОБЕДА" : "ПОРАЖЕНИЕ";
            await msg.RespondAsync($"{msg.User.Mention}:\n Ранг: {RiotHelper.Instance.GetRank(summoner.Id).Result} \n Последняя игра: {temp} \nУровень: {members[msg.User.Username].level}");

        }

        private async Task Taunt(SocketSlashCommand msg)
        {
            var userToTaunt = msg.Data.Options.First().Value as SocketGuildUser;
            if(userToTaunt.Id != KsuId)
                await msg.RespondAsync($"{userToTaunt.Mention} Ты лох");
            else
                await msg.RespondAsync($"{userToTaunt.Mention} Ты няшка");


        }
        private async Task SendMatvey(SocketSlashCommand msg)
        {
            string input = $@"D:\{msg.Data.Options.First().Value as string}.jpg";


            string way = ImageGenerator.Generate(input, msg.Data.Options.Last().Value as string);
            FileStream stream = File.OpenRead(way);
            await msg.RespondWithFileAsync(stream, $"{msg.Data.Options.First().Value as string}.jpg", "Во");
            File.Delete(way);
            stream.Close();

        }
        public async Task AnnounceJoinedUser(SocketGuildUser user)
        {
            var channel = client.GetChannel(MainId) as SocketTextChannel;
            var rulesChannel = client.GetChannel(RulesChatId) as SocketTextChannel;
            await channel.SendMessageAsync($"Здарова, {user.Mention}))) Рады видеть тя на сервере UwU!!! Скорее заходи в чятик {rulesChannel.Mention} и читай правила))");
            await user.AddRoleAsync(DefaultRole);
        }
        public string PlayerInfo()
        {
            return "XD";
        }
    }
}
