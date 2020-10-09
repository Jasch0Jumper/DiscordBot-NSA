using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace DiscordNSAbot.commands
{
	public class TestCommands : BaseCommandModule
	{
		[Command("test")]
		public async Task Test(CommandContext ctx)
		{
			await ctx.Channel.SendMessageAsync("test succsesful").ConfigureAwait(false);
		}
	}
}
