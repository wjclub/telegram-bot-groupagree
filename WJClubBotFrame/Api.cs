using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using WJClubBotFrame.Types;
using CustomJsonStuff;
using System.Threading.Tasks;
using System.Net.Http;

namespace WJClubBotFrame.Methods {
	public class Api {
		public static User GetMe(string apikey) {
#if DEBUG
			Console.WriteLine("\nGetting me [" + DateTime.Now + "]...\n");
#endif
			string json;
			{
				HttpWebRequest client = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot" + apikey + "/getMe");
				try {
					Stream data = client.GetResponse().GetResponseStream();
					using (StreamReader reader = new StreamReader(data)) {
						json = reader.ReadToEnd();
					}
				} catch (WebException e) {
					Notifications.log(e.ToString());
					return null;
				}
			}
#if DEBUG
			Console.WriteLine(JsonEnhancer.FormatJson(json));
#endif
			GetMeResponse response = JsonConvert.DeserializeObject<GetMeResponse>(json);
			return response.Result;
		}

		public static void DeleteWebhook(string apikey) {
			HttpWebRequest client = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot" + apikey + "/setWebhook?url=");
			try {
				Stream data = client.GetResponse().GetResponseStream();
				using (StreamReader reader = new StreamReader(data)) {
					reader.ReadToEnd();
				}
			} catch (WebException e) {
				Notifications.log(e.ToString());
			}
		}

		public static Update[] GetUpdates(string apikey, int offset) {
#if DEBUG
			Console.WriteLine("\nGetting updates [" + DateTime.Now + "]...\n");
#endif
			string json;
			{ //TODO Remove this
				HttpWebRequest client = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot" + apikey + "/getUpdates?timeout=0&offset=" + offset);
				client.Timeout = 60000;
				try {
					Stream data = client.GetResponse().GetResponseStream();
					using (StreamReader reader = new StreamReader(data)) {
						json = reader.ReadToEnd();
						reader.Close();
					}
				} catch (WebException e) {
					Notifications.log(e.ToString());
					return null;
				}
			}
			//Console.WriteLine(JsonEnhancer.FormatJson(json));
			return JsonConvert.DeserializeObject<Response>(json).Result;
		}

		public static async Task<Update[]> GetUpdatesAsync(string apikey, int offset) {
			string json = null;
			int timeoutSeconds = 60;
			using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSeconds) }) {
				string url = $"https://api.telegram.org/bot{apikey}/getUpdates?timeout={timeoutSeconds}&offset={offset}";
				HttpResponseMessage response = await client.GetAsync(url);
				json = await response.Content.ReadAsStringAsync();
			}
			return JsonConvert.DeserializeObject<Response>(json).Result;
		}

		public static void SendMessage(string apikey, long chatId, string text, bool noWeb = true, bool silent = false, int? replyToId = null, ReplyMarkup replyMarkup = null, bool messageId = false) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendMessage",
				chat_id = chatId,
				text = text,
				parse_mode = "HTML",
				disable_web_page_preview = noWeb,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
			});
			Requester.MakeRequestAsync(apikey, json, messageId);
		}

		public static void ForwardMessage(string apikey, long chatId, long fromChatId, int messageId, bool silent = false) {
			string json = JsonConvert.SerializeObject(new {
				method = "forwardMessage",
				chat_id = chatId,
				from_chat_id = fromChatId,
				message_id = messageId,
				disable_notification = silent,
			});
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendPhoto(string apikey, long chatId, string photo, string caption, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendPhoto",
				chat_id = chatId,
				photo = photo,
				caption = caption,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendAudio(string apikey, long chatId, string audio, int? duration = null, string performer = null, string title = null, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendAudio",
				chat_id = chatId,
				audio = audio,
				duration = duration,
				performer = performer,
				title = title,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendDocument(string apikey, long chatId, string document, string caption, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendDocument",
				chat_id = chatId,
				document = document,
				caption = caption,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendSticker(string apikey, long chatId, string sticker, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendSticker",
				chat_id = chatId,
				sticker = sticker,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendVideo(string apikey, long chatId, string video, int? duration = null, int? width = null, int? height = null, string caption = null, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendVideo",
				chat_id = chatId,
				video = video,
				duration = duration,
				width = width,
				height = height,
				caption = caption,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendVoice(string apikey, long chatId, string voice, int? duration = null, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendVoice",
				chat_id = chatId,
				voice = voice,
				duration = duration,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendLocation(string apikey, long chatId, int latitude, int longitude, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendLocation",
				chat_id = chatId,
				latitude = latitude,
				longitude = longitude,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void SendVenue(string apikey, long chatId, int latitude, int longitude, string title, string adress, string foursquareId, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendVenue",
				chat_id = chatId,
				latitude = latitude,
				longitude = longitude,
				title = title,
				adress = adress,
				foursquare_id = foursquareId,
				disable_notification = silent,
				reply_to_message_id = replyToId,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void AnswerCallbackQuery(string apikey, string callbackQueryId, string text = null, bool showAlert = false, string url = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "answerCallbackQuery",
				callback_query_id = callbackQueryId,
				text = text,
				show_alert = showAlert,
				url = url,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void AnswerInlineQuery(string apikey, string inlineQueryId, List<InlineQueryResult> results, int? cacheTime = null, bool personal = false, string pmText = null, string pmParameter = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "answerInlineQuery",
				inline_query_id = inlineQueryId,
				results = results,
				cache_time = cacheTime,
				is_personal = personal,
				switch_pm_text = pmText,
				switch_pm_parameter = pmParameter,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void EditMessageText(string apikey, string text, ReplyMarkup replyMarkup = null, long? chatId = null, int? messageId = null, string inlineMessageId = null, bool noWeb = true) {
			string json = JsonConvert.SerializeObject(new {
				method = "editMessageText",
				chat_id = chatId,
				message_id = messageId,
				inline_message_id = inlineMessageId,
				text = text,
				parse_mode = "HTML",
				disable_web_page_preview = noWeb,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}

		public static void EditMessageReplyMarkup(string apikey, long chatID, int messageID, object replyMarkup) {
			string json = JsonConvert.SerializeObject(new {
				method = "editMessageReplyMarkup",
				chat_id = chatID,
				message_id = messageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Requester.MakeRequestAsync(apikey, json);
		}
	}
}

