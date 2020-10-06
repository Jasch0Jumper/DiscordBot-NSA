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
			ConfigJson configJson = await ReadJsonConfig().ConfigureAwait(false);

			SetupClient(configJson);
			SetupCommands(configJson);

			await Client.ConnectAsync();
			await Task.Delay(-1);
		}

		private static async Task<ConfigJson> ReadJsonConfig()
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

			Commands.RegisterCommands<TestCommand>();
		}

		private void SubscribeToEvents()
		{
			Client.Ready += OnReady;
			Client.MessageCreated += OnMessageCreated;
		}
		private Task OnReady(DiscordClient sender, ReadyEventArgs e)
		{
			Console.WriteLine("-------------");
			Console.WriteLine("NSA is online");
			Console.WriteLine("-------------");

			return default;
		}

		private Task<Task> OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
		{
			if (e.Author.IsBot) { return default; }
			if (e.Message.Attachments.Count == 0) { return default; }

			e.Message.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder()
				.WithColor(new DiscordColor("#18ff08"))
				.WithAuthor(e.Message.Author.Username, null, e.Message.Author.AvatarUrl)
				.WithDescription($"{e.Channel.Mention} \n")
				.WithFooter($"Send {e.Message.Attachments.Count} Files")
			);

			foreach (var file in e.Message.Attachments)
			{
				using (var wc = new WebClient())
				{
					wc.DownloadFileAsync(new Uri(file.Url), $"{file.FileName}");
					
					using (var fs = File.Open($"{file.FileName}", FileMode.Open))
					{
						e.Message.Channel.SendFileAsync(fs);
						
						File.Delete(fs.Name);
					}
				}
			}

			return default;
		}
	}
}
