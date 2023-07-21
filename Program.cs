using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;
using System.Timers;
using YouTubeBot.Commands;
using YouTubeBot.Config;
using YouTubeBot.YouTube;

namespace YouTubeBot
{
    public sealed class Program
    {
        //Discord Properties
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }

        //YouTube Properties
        private static YouTubeVideo _video = new YouTubeVideo();
        private static YouTubeVideo temp = new YouTubeVideo();
        private static YouTubeEngine _YouTubeEngine = new YouTubeEngine();
        static async Task Main(string[] args)
        {
            //1. Get the details of your config.json file by deserialising it
            var configJsonFile = new JSONReader();
            await configJsonFile.ReadJSON();

            //2. Setting up the Bot Configuration
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJsonFile.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            //3. Apply this config to our DiscordClient
            Client = new DiscordClient(discordConfig);

            //4. Set the default timeout for Commands that use interactivity
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            //5. Set up the Task Handler Ready event
            Client.Ready += OnClientReady;

            //6. Set up the Commands Configuration
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJsonFile.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            //7. Register your commands

            Commands.RegisterCommands<Basic>();

            //8. Connect to get the Bot online
            await Client.ConnectAsync();

            //9. Start the YouTube notification service
            await StartYouTubeNotifier(Client, 123456789); //INSERT VALID CHANNEL ID, OR IMPLEMENT A CHANNEL SYSTEM

            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static async Task StartYouTubeNotifier(DiscordClient client, ulong channelIdToNotify)
        {
            var timer = new Timer(120000); //Timer set for 2 min

            timer.Elapsed += async (sender, e) => {

                _video = _YouTubeEngine.GetLatestVideo(); //Get latest video using API
                DateTime lastCheckedAt = DateTime.Now;

                if (_video != null)
                {
                    if (temp.videoTitle == _video.videoTitle) //This ensures that only the newest videos get sent through
                    {
                        Console.WriteLine("Same name");
                    }
                    else if (_video.PublishedAt < lastCheckedAt) //If the new video is actually new
                    {
                        var message = $"NEW VIDEO | **{_video.videoTitle}** \n" +
                                      $"Published at: {_video.PublishedAt} \n" +
                                      "URL: " + _video.videoUrl;

                        await client.GetChannelAsync(channelIdToNotify).Result.SendMessageAsync(message);
                        temp = _video;
                    }
                    else //NO new videos were found here
                    {
                        Console.WriteLine("[" + lastCheckedAt.ToString() + "]" + "YouTube API: No new videos were found");
                    }
                }
            };
            timer.Start();
        }
    }
}
