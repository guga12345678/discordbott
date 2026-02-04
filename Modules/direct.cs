using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace TutorialBot.Modules
{
    public class Direct : ModuleBase<SocketCommandContext>
    {
        [Command("dmme")]
        [Summary("Sends a direct message to the user")]
        public async Task DmMeAsync()
        {
            try
            {
                // Send DM to the user who used the command
                await Context.User.SendMessageAsync(
                    "👋 Hello! This is a direct message from the bot."
                );

                // Confirm in the channel
                await ReplyAsync("📩 I sent you a DM!");
            }
            catch
            {
                // Happens if user has DMs disabled
                await ReplyAsync("❌ I couldn't DM you. Please enable DMs from server members.");
            }
        }
    }
}
