using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordNSAbot.commands
{
	public class TestCommand : BaseCommandModule
	{
		[Command("test")]
		public async Task Test(CommandContext ctx)
		{
			await ctx.Channel.SendMessageAsync("DEV !").ConfigureAwait(false);
		}
	}
}
