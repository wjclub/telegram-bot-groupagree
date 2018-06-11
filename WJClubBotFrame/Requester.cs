using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace WJClubBotFrame {
	public class Requester {
		public static void MakeRequestAsync(string apikey, string json, bool messageId = false) {
			MakeRequestNonAsync(String.Copy(apikey) ,String.Copy(json) , messageId);
		}

		public static async Task MakeRequestNonAsync(string apikey, string json, bool messageId = false) {
			string responseString;
			string url = $"https://api.telegram.org/bot{apikey}/";
			using (HttpClient client = new HttpClient()) {
				client.Timeout = TimeSpan.FromMinutes(1);
				StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync(url, content);
				responseString = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode) {
					Notifications.log(CustomJsonStuff.JsonEnhancer.FormatJson(responseString));
				}
			}
			if (!JsonConvert.DeserializeObject<WJClubBotFrame.Types.Response>(responseString).Ok) {
				Notifications.log(responseString);
			}
			return;
		}
	}
}