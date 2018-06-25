using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using WJClubBotFrame.Types;
using System.Threading.Tasks;
using System.Net.Http;

namespace WJClubBotFrame.Methods {
	public class Api {
		public static async Task<User> GetMeAsync(string apikey) {
			string json = JsonConvert.SerializeObject(new {
				method = "getMe",
			}, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore,
				Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
			});
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<User>(response.Result);
			} else {
				return null; //TODO Rethink this, maybe throw custom exceptions?
			}
		}

		public static async Task<bool> DeleteWebhookAsync(string apikey) {
			//TODO Is this fine?
			//This is fine
			//Ay hope
			return (await Requester.MakeRequestNonAsync(apikey, JsonConvert.SerializeObject(new { method = "deleteWebhook" }))).Ok;
		}

		public static async Task<Update[]> GetUpdatesAsync(string apikey, int offset) {
			int timeoutSeconds = 60;
			using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(timeoutSeconds) }) {
				string url = $"https://api.telegram.org/bot{apikey}/getUpdates?timeout={timeoutSeconds}&offset={offset}";
				HttpResponseMessage response;
				System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();
				try {
					response = await client.GetAsync(url, cancellationToken:cancellationTokenSource.Token);
				} catch (WebException ex) {
					Notifications.log($"GetUpdatesAsync WebException for URL: {url}");
					throw;
				} catch (TaskCanceledException ex) {
					if (ex.CancellationToken == cancellationTokenSource.Token) {
						throw;
					} else {
						return null;
					}
				}
				string json = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<Update[]>(JsonConvert.DeserializeObject<Response>(json).Result);
			}
		}

		public static async Task<Message> SendMessageAsync(string apikey, long chatID, string messageText, bool disableWebPagePreview = true, bool disableNotification = false, int? replyToMessageID = null, ReplyMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendMessage",
				chat_id = chatID,
				text = messageText,
				parse_mode = "HTML",
				disable_web_page_preview = disableWebPagePreview,
				disable_notification = disableNotification,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
			});
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> ForwardMessage(string apikey, long chatID, long fromChatID, int messageID, bool disableNotification = false) {
			string json = JsonConvert.SerializeObject(new {
				method = "forwardMessage",
				chat_id = chatID,
				from_chat_id = fromChatID,
				message_id = messageID,
				disable_notification = disableNotification,
			});
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendPhoto(string apikey, long chatID, string photo, string caption, bool disableNotification = false, int? replyToMessageID = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendPhoto",
				chat_id = chatID,
				photo = photo,
				caption = caption,
				disable_notification = disableNotification,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendAudio(string apikey, long chatId, string audio, int? duration = null, string performer = null, string title = null, bool silent = false, int? replyToId = null, ReplyKeyboardMarkup replyMarkup = null) {
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
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendDocument(string apikey, long chatID, string document, string caption, bool disableNotification = false, int? replyToMessageID = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendDocument",
				chat_id = chatID,
				document = document,
				caption = caption,
				disable_notification = disableNotification,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendSticker(string apikey, long chatID, string sticker, bool disableNotification = false, int? replyToMessageID = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendSticker",
				chat_id = chatID,
				sticker = sticker,
				disable_notification = disableNotification,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendVideo(string apikey, long chatID, string video, int? duration = null, int? width = null, int? height = null, string caption = null, bool disableNotifications = false, int? replyToMessageID = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendVideo",
				chat_id = chatID,
				video = video,
				duration = duration,
				width = width,
				height = height,
				caption = caption,
				disable_notification = disableNotifications,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendVoice(string apikey, long chatID, string voice, int? duration = null, bool disableNotifications = false, int? replyToMessageID = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendVoice",
				chat_id = chatID,
				voice = voice,
				duration = duration,
				disable_notification = disableNotifications,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendLocation(string apikey, long chatID, int latitude, int longitude, bool disableNotifications = false, int? replyToMessageID = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendLocation",
				chat_id = chatID,
				latitude = latitude,
				longitude = longitude,
				disable_notification = disableNotifications,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<Message> SendVenue(string apikey, long chatID, int latitude, int longitude, string title, string adress, string foursquareId, bool disableNotifications = false, int? replyToMessageID = null, ReplyKeyboardMarkup replyMarkup = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "sendVenue",
				chat_id = chatID,
				latitude = latitude,
				longitude = longitude,
				title = title,
				adress = adress,
				foursquare_id = foursquareId,
				disable_notification = disableNotifications,
				reply_to_message_id = replyToMessageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (response.Ok) {
				return JsonConvert.DeserializeObject<Message>(response.Result);
			} else {
				return null; //TODO Look above
			}
		}

		public static async Task<bool> AnswerCallbackQuery(string apikey, string callbackQueryId, string text = null, bool showAlert = false, string url = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "answerCallbackQuery",
				callback_query_id = callbackQueryId,
				text = text,
				show_alert = showAlert,
				url = url,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			return response.Ok; //TODO see above
		}

		public static async Task<bool> AnswerInlineQuery(string apikey, string inlineQueryId, List<InlineQueryResult> results, int? cacheTime = null, bool personal = false, string pmText = null, string pmParameter = null) {
			string json = JsonConvert.SerializeObject(new {
				method = "answerInlineQuery",
				inline_query_id = inlineQueryId,
				results = results,
				cache_time = cacheTime,
				is_personal = personal,
				switch_pm_text = pmText,
				switch_pm_parameter = pmParameter,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			return response.Ok; //TODO see above
		}

		//TODO Make this return message optionally
		/// <summary>
		/// Edit a message, works with inline messags as well as normal messages
		/// </summary>
		/// <param name="apikey"></param>
		/// <param name="text"></param>
		/// <param name="replyMarkup"></param>
		/// <param name="chatID"></param>
		/// <param name="messageID"></param>
		/// <param name="inlineMessageID"></param>
		/// <param name="disableWebPagePreview"></param>
		/// <returns>Nothing ;D</returns>
		/// <exception cref="Exceptions.MessageIDInvalid"></exception>
		/// <exception cref="Exceptions.MessageTooLong"></exception>
		/// <exception cref="Exceptions.TooManyRequests"></exception>
		public static async Task EditMessageTextAsync(string apikey, string text, ReplyMarkup replyMarkup = null, long? chatID = null, int? messageID = null, string inlineMessageID = null, bool disableWebPagePreview = true) {
			string json = JsonConvert.SerializeObject(new {
				method = "editMessageText",
				chat_id = chatID,
				message_id = messageID,
				inline_message_id = inlineMessageID,
				text = text,
				parse_mode = "HTML",
				disable_web_page_preview = disableWebPagePreview,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			if (!response.Ok) {
				switch (response.ErrorCode) {
					case 400: {
						switch (response.Description) {
							case "Bad Request: MESSAGE_ID_INVALID":
								throw new Exceptions.MessageIDInvalid();
							case "Bad Request: MESSAGE_TOO_LONG":
								throw new Exceptions.MessageTooLong();
						}
					} break;
					case 429:
						throw new Exceptions.TooManyRequests(response.Parameters.RetryAfter);
				}
			}
		}

		public static async Task<bool> EditMessageReplyMarkup(string apikey, long chatID, int messageID, object replyMarkup) {
			string json = JsonConvert.SerializeObject(new {
				method = "editMessageReplyMarkup",
				chat_id = chatID,
				message_id = messageID,
				reply_markup = replyMarkup,
			}, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() } });
			Response response = await Requester.MakeRequestNonAsync(apikey, json);
			return response.Ok;
		}
	}
}

