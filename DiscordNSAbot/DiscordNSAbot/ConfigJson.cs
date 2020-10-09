using Newtonsoft.Json;

namespace DiscordNSAbot
{
	public struct ConfigJson
	{
		[JsonProperty("token")] public string Token { get; private set; }
		[JsonProperty("prefix")] public string Prefix { get; private set; }

		[JsonProperty("nsadev")] public string NsaDev { get; private set; }
		[JsonProperty("clipboardlog")] public string ClipboardLog { get; private set; }
		[JsonProperty("nwologinclipboard")] public string NwoLogInClipboard { get; private set; }
		[JsonProperty("nwolog")] public string NwoLog { get; private set; }

		[JsonProperty("clipboard")] public string Clipboard { get; private set; }
		[JsonProperty("nwo")] public string Nwo { get; private set; }
	}
}
