using DiscordNSAbot.commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordNSAbot
{
	public class Bot
	{
		public DiscordClient Client { get; private set; }
		public CommandsNextExtension Commands { get; private set; }

		public async Task RunAsync()
		{
			ConfigJson configJson = await GetConfigVars().ConfigureAwait(false);

			SetupClient(configJson);
			SetupCommands(configJson);

			Channels.AssignChannelsAsync(Client, configJson);
			Guilds.AssignGuildsAsync(Client, configJson);

			await Client.ConnectAsync();
			await Task.Delay(-1);
		}

		private async Task<ConfigJson> GetConfigVars()
		{
			if (File.Exists("config.json"))
			{
				return await ReadJsonConfig().ConfigureAwait(false);
			}
			else
			{
				return new ConfigJson
				{
					Token = Environment.GetEnvironmentVariable("token"),
					Prefix = Environment.GetEnvironmentVariable("prefix"),

					NsaDev = Environment.GetEnvironmentVariable("nsadev"),
					ClipboardLog = Environment.GetEnvironmentVariable("clipboardlog"),
					NwoLogInClipboard = Environment.GetEnvironmentVariable("nwologinclipboard"),
					NwoLog = Environment.GetEnvironmentVariable("nwolog"),

					Clipboard = Environment.GetEnvironmentVariable("clipboard"),
					Nwo = Environment.GetEnvironmentVariable("nwo")
				};
			}
			
		}

		private async Task<ConfigJson> ReadJsonConfig()
		{
			var json = string.Empty;

			using (var fs = File.OpenRead("config.json"))
			using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
				json = await sr.ReadToEndAsync().ConfigureAwait(false);

			var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
			return configJson;
		}

		private void SetupClient(ConfigJson configJson)
		{
			var discordConfig = new DiscordConfiguration
			{
				Token = configJson.Token,
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				UseRelativeRatelimit = true
			};

			Client = new DiscordClient(discordConfig);

			SubscribeToEvents();
		}
		private void SetupCommands(ConfigJson configJson)
		{
			var commandsConfig = new CommandsNextConfiguration
			{
				StringPrefixes = new string[] { configJson.Prefix },
				EnableMentionPrefix = true,
				EnableDms = false,
			};

			Commands = Client.UseCommandsNext(commandsConfig);

			Commands.RegisterCommands<TestCommands>();
		}

		private void SubscribeToEvents()
		{
			Client.Ready += OnReady;
			Client.MessageCreated += OnMessageCreated;
			Client.MessageUpdated += OnMessageUpdate;
			Client.MessageDeleted += OnMessageDeleted;
		}

		private Task OnReady(DiscordClient sender, ReadyEventArgs e)
		{
			Console.WriteLine("-------------");
			Console.WriteLine("NSA is online");
			Console.WriteLine("-------------");

			return default;
		}
		private Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
		{
			if (e.Author.IsBot) { return default; }
			if (e.Message.Attachments.Count == 0) { return default; }

			DownloadAttachments(e);

			SendDiscordEmbed(e.Message, LogType.Send, string.Empty);

			SendAndDeleteFiles(e);

			return default;
		}
		private Task OnMessageUpdate(DiscordClient sender, MessageUpdateEventArgs e)
		{
			if (e.Author.IsBot) { return default; }
			if (e.MessageBefore.Content.Equals(e.Message.Content)) { return default; }

			string logMessage = $"**Original:** \n {e.MessageBefore.Content} \n **Edit:** \n {e.Message.Content}";
			SendDiscordEmbed(e.Message, LogType.Edit, logMessage);
			return default;
		}
		private Task OnMessageDeleted(DiscordClient sender, MessageDeleteEventArgs e)
		{
			if (e.Message.Author.IsBot) { return default; }
			if (String.IsNullOrEmpty(e.Message.Content)) { return default; }

			string logMessage = $"**Message:** \n{e.Message.Content}";
			SendDiscordEmbed(e.Message, LogType.Delete, logMessage);
			return default;
		}

		private void DownloadAttachments(MessageCreateEventArgs e)
		{
			foreach (var file in e.Message.Attachments)
			{
				using var wc = new WebClient();
				wc.DownloadFile(new Uri(file.Url), $"{file.FileName}");
			}
		}

		private void SendDiscordEmbed(DiscordMessage message, LogType logType, string additionalLogMessage)
		{
			DiscordUser author = message.Author;
			string nickname = message.Channel.Guild.GetMemberAsync(author.Id).Result.Nickname;

			string color = string.Empty;
			string logTypeMessage = string.Empty;

			switch (logType)
			{
				case LogType.Send:
					color = "#18ff08";
					logTypeMessage = "Send File"; 
					break;
				case LogType.Edit:
					color = "#fcf400";
					logTypeMessage = "Edit"; 
					break;
				case LogType.Delete:
					color = "#b00e0e";
					logTypeMessage = "Delete"; 
					break;
			}

			Channels.NSADev.SendMessageAsync(embed: new DiscordEmbedBuilder()
				.WithColor(new DiscordColor(color))
				.WithAuthor($"{author.Username}#{author.Discriminator} --- {nickname}", null, author.AvatarUrl)
				.WithDescription($"{message.Channel.Mention} | {logTypeMessage} | {message.Timestamp} | ID: {message.Id} \n {additionalLogMessage}")
			);
		}

		private async void SendAndDeleteFiles(MessageCreateEventArgs e)
		{
			foreach (var file in e.Message.Attachments)
			{
				using var fs = File.Open($"{file.FileName}", FileMode.Open);
				await Channels.NSADev.SendFileAsync(fs);

				fs.Close();

				File.Delete(fs.Name);
			}
		}
	}
}
