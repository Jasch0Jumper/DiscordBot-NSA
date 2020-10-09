using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace DiscordNSAbot
{
	public static class Channels
	{
		public static DiscordChannel NSADev { get; private set; }
		public static DiscordChannel ClipboardLog { get; private set; }
		public static DiscordChannel NwoLogInClipboard{ get; private set; }
		public static DiscordChannel NwoLog{ get; private set; }

		public static async void AssignChannelsAsync(DiscordClient client, ConfigJson configJson)
		{
			NSADev = await client.GetChannelAsync(Convert.ToUInt64(configJson.NsaDev));
			ClipboardLog = await client.GetChannelAsync(Convert.ToUInt64(configJson.ClipboardLog));
			NwoLogInClipboard = await client.GetChannelAsync(Convert.ToUInt64(configJson.NwoLogInClipboard));
			
			NwoLog = await client.GetChannelAsync(Convert.ToUInt64(configJson.NwoLog));
		}

		public static List<DiscordChannel> GetLogChannel(DiscordMessage message)
		{
			List<DiscordChannel> logChannels = new List<DiscordChannel>();
			var guild = message.Channel.Guild;

			if (guild == Guilds.Clipboard)
			{
				logChannels.Add(ClipboardLog);
			}
			else if (guild == Guilds.Nwo)
			{
				logChannels.Add(NwoLog);
				logChannels.Add(NwoLogInClipboard);
			}
			else
			{
				logChannels.Add(NSADev);
			}
			
			return logChannels;
		}
	}
}
