using Newtonsoft.Json;

namespace DiscordNSAbot
{
	public struct ConfigJson
	{
		[JsonProperty("token")] public string Token { get; set; }
		[JsonProperty("prefix")] public string Prefix { get; set; }

		[JsonProperty("nsadev")] public string NsaDev { get; set; }
		[JsonProperty("clipboardlog")] public string ClipboardLog { get; set; }
		[JsonProperty("nwologinclipboard")] public string NwoLogInClipboard { get; set; }
		[JsonProperty("nwolog")] public string NwoLog { get; set; }

		[JsonProperty("clipboard")] public string Clipboard { get; set; }
		[JsonProperty("nwo")] public string Nwo { get; set; }
	}
}
