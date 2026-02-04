using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace TutorialBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        // ?? Channels where text messages will be auto-deleted
        private readonly List<ulong> AutoDeleteChannelIds = new List<ulong>
        {
            1466907225049010289, // deletion channel 1
            1466901460532072468  // deletion channel 2
        };

        // ?? Channels where messages are safe
        private readonly List<ulong> SafeChannelIds = new List<ulong>
        {
            1466537673505378548, // safe channel 1
             // safe channel 2
        };

        // ?? Number guessing game settings
        private const ulong NumberGuessChannelId = 1468014231856353340; // game channel
        private const ulong GameStarterRoleId = 1466924908217892892;    // role allowed to start game
        private const ulong WinnerRoleId = 1466804645518246042;         // role to give to winner

        private bool _gameActive = false;
        private int _currentRandomNumber;
        private readonly Random _rng = new Random();
        private bool _channelLocked = false; // lock after someone wins

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += LogAsync;

            await RegisterCommandsAsync();

            // ?? Put your bot token here
            string token = "MTQ2NzIxNzg3NzIzNTkyOTIzMQ.GeuDvG.Vaxi0q8T1cF4_Ye8qVKZSinoF1WT7WCCrpVAxw";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleMessageAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Author.IsBot) return;

            var channel = message.Channel as SocketTextChannel;

            // ?? Skip safe channels entirely
            if (SafeChannelIds.Contains(message.Channel.Id))
                return;

            // ?? Number guessing game logic
            if (message.Channel.Id == NumberGuessChannelId)
            {
                var authorUser = message.Author as SocketGuildUser;

                // Lock channel if someone already won
                if (_channelLocked)
                {
                    await message.DeleteAsync();
                    return;
                }

                // 1?? Start the game if author has starter role and game is not active
                if (!_gameActive && authorUser.Roles.Any(r => r.Id == GameStarterRoleId))
                {
                    _currentRandomNumber = _rng.Next(1, 201); // 1–500
                    _gameActive = true;
                    _channelLocked = false; // unlock channel at start
                    await message.Channel.SendMessageAsync(
                        $"ricxvis gamocnobis tamashi daiwyo! gamoicani ricxvi 1_dan 200_mde."
                    );
                    return;
                }

                // If game is not active yet, ignore any numbers from users
                if (!_gameActive)
                {
                    await message.DeleteAsync();
                    return;
                }

                // 2?? Game is active, only numbers allowed
                bool hasLetter = message.Content.Any(char.IsLetter);
                if (hasLetter)
                {
                    await message.DeleteAsync();
                    return;
                }

                // 3?? Check guesses
                if (int.TryParse(message.Content, out int guess))
                {
                    if (guess == _currentRandomNumber)
                    {
                        // Give role to the winner
                        if (authorUser != null)
                        {
                            var role = channel.Guild.GetRole(WinnerRoleId);
                            if (role != null)
                            {
                                await authorUser.AddRoleAsync(role);
                                await message.Channel.SendMessageAsync(
                                    $"{authorUser.Mention} gamoicno swori ricxvi da moigo VIP!"
                                );
                            }
                        }

                        // Stop the game immediately
                        _gameActive = false;
                        _channelLocked = true; // lock channel immediately

                        // Optional: unlock after 1 minute
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(60000);
                            _channelLocked = false;
                            await message.Channel.SendMessageAsync("chati mzataa axali tamashis dasawyebad!");
                        });

                        return; // prevent further processing
                    }
                }

                return; // end processing in game channel
            }

            // ?? Auto-delete messages in configured channels
            if (AutoDeleteChannelIds.Contains(message.Channel.Id))
            {
                // Only delete if there are NO attachments or embeds
                if (message.Attachments.Count == 0 && message.Embeds.Count == 0)
                {
                    await message.DeleteAsync();

                    // Build warning showing all safe channels
                    string safeMentions = string.Join(", ", SafeChannelIds.ConvertAll(id => $"<#{id}>"));

                    var warningMessage = await message.Channel.SendMessageAsync(
                        $"aq nudebi chayaret mesijebistvis gamoiyenet {safeMentions}"
                    );

                    // Delete warning asynchronously after 15 seconds
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(15000);
                        await warningMessage.DeleteAsync();
                    });

                    return;
                }
            }

            // ?? Command handling
            int argPos = 0;
            if (message.HasStringPrefix("!", ref argPos))
            {
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}

