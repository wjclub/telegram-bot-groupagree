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
			} catch (IndexOutOfRangeException) {
				Console.WriteLine("Syntax: telegram-bot-groupagree.exe <dbuser> <dbpw> <dbname>");
				Environment.Exit(0);
				return;
			}
			GroupAgreeBot groupagreebot = new GroupAgreeBot(dbname, dbuser, dbpw);
			try {
				groupagreebot.Run();
			} catch (Exception e) {
				Notifications.log(e.ToString() + "\n\n\n-------- Context --------\n\n" + CustomJsonStuff.JsonEnhancer.FormatJson(JsonConvert.SerializeObject(new { groupagreebot.CurrentUpdate , groupagreebot.CurrentPointer}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }})));
			}
			Notifications.log("Weird exit...");
			//Console.WriteLine(" Press any key...");<
			//Console.ReadKey();
		}
	}
}
