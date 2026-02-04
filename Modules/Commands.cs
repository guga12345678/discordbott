using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TutorialBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private static readonly Random _rand = new Random();

        private string[] kissingImages = new string[]
        {
            "https://imgs.search.brave.com/cuv5x7HHr4M0FJb4WcQLz-Ny2na5lGzonQWEL5EJEs4/rs:fit:500:0:0/g:ce/aHR0cHM6Ly9pbWFn/ZXMudW5zcGxhc2gu/Y29tL3Bob3RvLTE1/Njk3MDY1NDg2NDUt/ODAzNDQ0OWY2YTdi/P2l4bGliPXJiLTQu/MC4zJml4aWQ9TTN3/eE1qQTNmREI4TUh4/bGVIQnNiM0psTFda/bFpXUjhNWHg4ZkdW/dWZEQjhmSHg4ZkE9/PSZ3PTEwMDAmcT04/MA.jpeg",
            "https://imgs.search.brave.com/FJlrpXJqBgwKdCGZE8ZuoGb0IW4FDD2benOhHu_ycik/rs:fit:500:0:0/g:ce/aHR0cHM6Ly9pbWcu/ZnJlZXBpay5jb20v/ZnJlZS1waG90by9n/dXktbWFrZXMtaGVy/LWZlZWwtbGlrZS1w/cmluY2Vzc18zMjkx/ODEtMTg2MTAuanBn/P3NpemU9NjI2JmV4/dD1qcGc",
            "https://imgs.search.brave.com/gAd1ea6EOYBAoDTLg-g_I62oWiCP6BgM8ymoyL2RYY4/rs:fit:500:0:0/g:ce/aHR0cHM6Ly9pLnBp/bmltZy5jb20vb3Jp/Z2luYWxzLzBjLzRj/LzBmLzBjNGMwZmI0/OGIwMTAxYjQ3NTE0/MDg0NWMxNjg2Mjgx/LmpwZw"
        };

        private string[] nudesVideos = new string[]
        {
            "https://discord.com/channels/1466537672922107981/1466901460532072468/1467979734830022728"
        };

        private readonly CommandService _commands;

        public Commands(CommandService commands)
        {
            _commands = commands;
        }

        // ================= HELP COMMAND =================
        [Command("help")]
        [Alias("commands")]
        public async Task ShowHelp()
        {
            string commandsList = @"
!მელოზა
!ნუდები
!შენი
!ჩაიხადე
!აკოცე
!მელოზააა
!ბაგირა
!ბანი დაადე
!გააგდე
!გააფრთხილე";

            await ReplyAsync($"**Available commands:**\n{commandsList}");
        }

        // ================= SIMPLE COMMANDS =================
        [Command("მელოზა")]
        public async Task მელოზა()
        {
            await ReplyAsync("გამარჯობა რით შემიძლია დაგეხმარო?");
        }

        [Command("ნუდები")]
        public async Task ნუდები()
        {
            await ReplyAsync("ვისი ნუდები გინდა?");
        }

        [Command("შენი")]
        public async Task მელოზასშენი()
        {
            await ReplyAsync("თუ მელოზას ნუდების ნახვა გსურს შესაბამისი როლი გჭირდება.");
        }

        [Command("ჩაიხადე")]
        public async Task ჩაიხადე()
        {
            await ReplyAsync("მთელი ცხოვრება ჩახდილი და ფეხებ გადაშლილი დავდივარ ისედაც");
        }

        [Command("აკოცე")]
        public async Task KissUser(SocketUser user = null)
        {
            if (user == null)
            {
                await ReplyAsync("ვის გინდა რომ აკოცო?!");
                return;
            }

            string response = $"შენ აკოცე {user.Mention}'ს! 😘";

            var builder = new EmbedBuilder()
                .WithTitle("კოცნის ქომანდი")
                .WithDescription(response)
                .WithImageUrl(kissingImages[_rand.Next(kissingImages.Length)])
                .WithColor(Color.Purple);

            await ReplyAsync(embed: builder.Build());
        }

        [Command("მელოზააა")]
        public async Task მელოზააა()
        {
            var videoUrl = nudesVideos[_rand.Next(nudesVideos.Length)];
            await ReplyAsync(videoUrl);
        }

        // ================= MODERATION =================
        [Command("ბანი დაადე")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "შენ არ გაქვს უფლება")]
        public async Task BanMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("მონიშნეთ წევრი!");
                return;
            }

            if (reason == null) reason = "არ არის მითითებული";

            await Context.Guild.AddBanAsync(user, 1, reason);
            await ReplyAsync($":white_check_mark: {user.Mention} დაედო ბანი - მიზეზი: {reason}");
        }

        [Command("გააგდე")]
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "შენ არ გაქვს უფლება")]
        public async Task KickMember(IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                await ReplyAsync("მონიშნეთ წევრი!");
                return;
            }

            if (reason == null) reason = "არ არის მითითებული";

            await user.KickAsync(reason);
            await ReplyAsync($":white_check_mark: {user.Mention} გააგდეს - მიზეზი: {reason}");
        }
    }
}
