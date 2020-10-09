using DSharpPlus;
using DSharpPlus.Entities;
using System;

namespace DiscordNSAbot
{
	public static class Guilds
	{
		public static DiscordGuild Clipboard { get; private set; }
		public static DiscordGuild Nwo { get; private set; }

		public static async void AssignGuildsAsync(DiscordClient client, ConfigJson configJson)
		{
			Clipboard = await client.GetGuildAsync(Convert.ToUInt64(configJson.Clipboard));
			Nwo = await client.GetGuildAsync(Convert.ToUInt64(configJson.Nwo));
		}
	}
}
