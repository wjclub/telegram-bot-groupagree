using System;
using WJClubBotFrame;
using Newtonsoft.Json;

namespace telegrambotgroupagree {
	public class MainBotFrame {
		public static void Main(string[] args) {
			Globals.GlobalOptions = new Globals.Options();
			string dbuser, dbpw, dbname;
			try {
				dbuser = args[0];
				dbpw = args[1];
				dbname = args[2];
				//TODO this: DBHandler.TestCredentials();
			} catch (IndexOutOfRangeException) {
				Console.WriteLine("Syntax: telegram-bot-groupagree.exe <dbuser> <dbpw> <dbname>");
				//Windows ERROR_BAD_ARGUMENTS error code
				Environment.Exit(0xA0);
				return;
			}
			try {
				GroupAgreeBot groupagreebot = GroupAgreeBot.Factory(dbname, dbuser, dbpw).Result;
				groupagreebot.Run().Wait();
			} catch (Exception e) {
				Notifications.log(e.ToString() + "\n\n\n-------- Context --------\n\n" + CustomJsonStuff.JsonEnhancer.FormatJson(JsonConvert.SerializeObject(new { groupagreebot.CurrentUpdate , groupagreebot.CurrentPointer}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }})));
			}
			Notifications.log("Last line reached...");
		}
	}
}
