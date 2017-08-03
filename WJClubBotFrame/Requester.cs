using System;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using CustomJsonStuff;
using WJClubBotFrame.Types;

namespace WJClubBotFrame {
	public class Requester {
		public static void MakeRequest(string apikey, string json, bool messageId = false) {
			#if DEBUG
			Console.WriteLine("\nMaking Request [" + DateTime.Now + "]\n");
			//Console.WriteLine("\nJson to be send:\n");
			//Console.WriteLine(JsonEnhancer.FormatJson(json));
			#endif
			string url = "https://api.telegram.org/bot" + apikey + "/";
			string response;
			WebRequest request = WebRequest.Create (url);
			request.Method = "POST";
			request.Timeout = 10000;
			request.ContentType = "application/json";
			int tries = 0;
			do {
				try {
					using (var dataStream = new StreamWriter(request.GetRequestStream())) {
						dataStream.Write(json);
						dataStream.Close();
					}
					break;
				} catch (System.Net.WebException) {
					tries++;
				}
			} while (tries < 3);
			try {
				using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream())) {
					response = reader.ReadToEnd();
					reader.Close();
				}
			} catch (Exception) {
				return;
			}
			//#if DEBUG
			//Console.WriteLine("\nURL:");
			//Console.WriteLine(url);
			//Console.WriteLine("\nResponse:\n");
			//Console.WriteLine(JsonEnhancer.FormatJson(response));
			//#endif
			//if (messageId) {
			//	SendMessageResponse resp = JsonConvert.DeserializeObject<SendMessageResponse> (response);
			//	return new MessageID {
			//		chatId = (long?)resp.Result.Chat.Id,
			//		messageId = resp.Result.MessageId
			//	};
			//} else
			//	return null;
		}
	}
}