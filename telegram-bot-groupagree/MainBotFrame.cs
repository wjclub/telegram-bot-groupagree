using System;
using WJClubBotFrame;
using Newtonsoft.Json;

namespace telegrambotgroupagree {
	public class MainBotFrame {
		public static void Main(string[] args) {
			var groupagreebot = new GroupAgreeBot(Globals.Apikey);
			try {
				groupagreebot.Run();
			} catch (Exception e) {
				Notifications.log(e.ToString() + "\n\n\n-------- Context --------\n\n" + CustomJsonStuff.JsonEnhancer.FormatJson(JsonConvert.SerializeObject(new { groupagreebot.CurrentUpdate , groupagreebot.CurrentPointer}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }})));
			}
			//Console.WriteLine(" Press any key...");<
			//Console.ReadKey();
		}
	}
}
