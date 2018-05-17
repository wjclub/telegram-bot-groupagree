using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;

namespace telegrambotgroupagree {
	public static class LangMessage {
		private static ContentParts prepare(Strings strings, Pointer pointer) {
			string text = strings.GetString(Strings.StringsList.setLanguage);
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			var values = Enum.GetValues(typeof(Strings.Langs));
			int j = -1;
			for (int i = 1; i < values.Length; i++) {
				int wI = i - 1;
				if (wI % 2 == 0) {
					inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton>());
					j++;
				}
				inlineKeyboard.InlineKeyboard[j].Add(InlineKeyboardButton.Create(strings.GetLangName((Strings.Langs)values.GetValue(i)) + (values.GetValue(i).Equals(pointer.Lang) ? " ✅" : ""), callbackData:("comm:lang:" + values.GetValue(i))));
			}
			inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> {
				InlineKeyboardButton.Create("\ud83d\udcbe " + strings.GetString(Strings.StringsList.done), callbackData:"comm:showWelcome")
			});
			return new ContentParts(text, inlineKeyboard, null, null);
		}
		public static void Refresh (string apikey, Strings strings, Pointer pointer, long chatId, int messageId) {
			ContentParts content = prepare(strings, pointer);
			Api.EditMessageText(apikey,content.Text, content.InlineKeyboard, chatId, messageId);
		}

		public static void Send(string apikey, Strings strings, Pointer pointer) {
			ContentParts content = prepare(strings, pointer);
			Api.SendMessage(apikey,pointer.ChatId, content.Text, replyMarkup:content.InlineKeyboard);
		}
	}
}

