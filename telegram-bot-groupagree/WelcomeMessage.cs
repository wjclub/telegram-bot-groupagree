using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;

namespace telegrambotgroupagree {
	public static class WelcomeMessage {
		private static ContentParts prepare(Strings strings, Pointer pointer) {
			string text = String.Format(strings.GetString(Strings.stringsList.startMessage),strings.GetString((Strings.stringsList)Enum.Parse(typeof(Strings.stringsList), pointer.PollType.ToString())), (pointer.Anony == EAnony.anonymous ? "✅" : "☑"));
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> ());
			inlineKeyboard.InlineKeyboard[0].Add(InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.startMessageChangePollTypeButton), callbackData:"page:chpoll"));
			inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> ());
			inlineKeyboard.InlineKeyboard[1].Add(InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.startMessagePersonalButton),(pointer.Anony == EAnony.personal ? "✅" : "☑")), callbackData:"comm:pers"));
			inlineKeyboard.InlineKeyboard[1].Add(InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.startMessageAnonyButton), (pointer.Anony == EAnony.anonymous ? "✅" : "☑")), callbackData:"comm:anony"));
			return new ContentParts(text, inlineKeyboard, null);
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

