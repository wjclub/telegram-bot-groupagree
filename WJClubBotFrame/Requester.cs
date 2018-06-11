using System;
using WJClubBotFrame.Types;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace WJClubBotFrame {
	public class Requester {
		/*public static void MakeRequestAsync(string apikey, string json, bool messageId = false) {
			MakeRequestNonAsync(String.Copy(apikey) ,String.Copy(json) , messageId);
		}*/

		/// <summary>
		/// I send am HTTP-Post request to Telegrams API endpoint containing the string you give me (it better be JSON-Encoded)
		/// </summary>
		/// <param name="apikey">Telegram Bot Key</param>
		/// <param name="jsonToSend">Request containing a Bot API method and its parameters</param>
		/// <returns>I return the (prety undocumented)[not my fault] Response Type to that request</returns>
		public static async Task<Response> MakeRequestNonAsync(string apikey, string jsonToSend) { //TODO Rename
			using (HttpClient client = new HttpClient()) {
				client.Timeout = TimeSpan.FromMinutes(1);
				StringContent contentToSend = new StringContent(
					content:jsonToSend, 
					encoding:System.Text.Encoding.UTF8, 
					mediaType:"application/json"
					);
				HttpResponseMessage postResponse = await client.PostAsync(
					requestUri: $"https://api.telegram.org/bot{apikey}/", 
					content:contentToSend
					);
				string responseString = await postResponse.Content.ReadAsStringAsync();
				//Thanks https://stackoverflow.com/a/3247635, this doesn't interfere with using ;D
				return JsonConvert.DeserializeObject<WJClubBotFrame.Types.Response>(responseString);
			}
		}
	}
}