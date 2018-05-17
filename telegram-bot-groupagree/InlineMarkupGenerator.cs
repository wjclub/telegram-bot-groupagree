using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;

namespace telegrambotgroupagree {
	public class InlineMarkupGenerator {
		public static InlineKeyboardMarkup GetOneButtonMarkup(InlineKeyboardButton button) {
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup
            {
                InlineKeyboard = new List<List<InlineKeyboardButton>>()
            };
            inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton>());
			inlineKeyboard.InlineKeyboard[0].Add(button);
			return inlineKeyboard;
		}

		public static InlineKeyboardMarkup GetTheModerationKeyboard(Strings strings, long chatID, int pollID) {
			return GetOneButtonMarkup(InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.moderate), callbackData:String.Format("comm:moderate:{0}:{1}", chatID, pollID)));
		}
	}
}

