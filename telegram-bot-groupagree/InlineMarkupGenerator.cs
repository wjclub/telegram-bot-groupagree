using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;

namespace telegrambotgroupagree {
	public class InlineMarkupGenerator {
		public static InlineKeyboardMarkup GetOneButtonMarkup(InlineKeyboardButton button) {
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton>());
			inlineKeyboard.InlineKeyboard[0].Add(button);
			return inlineKeyboard;
		}
	}
}

