using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;
using WJClubBotFrame;
using System.Linq;
using StringEdit = System.Globalization.CultureInfo;

namespace telegrambotgroupagree {
	public class PVote : Poll {
		public PVote(long chatId, long pollId, string pollText, EAnony anony, DBHandler dBHandler, Strings.Langs lang) : base(chatId, pollId, pollText, null, anony, false, PercentageBars.Bars.none, false, false, false, dBHandler, new Dictionary<string, List<User>>(), new List<MessageID>(), lang, EPolls.vote) {

		}

		public PVote(long chatId, long pollId, string pollText, string pollDescription, EAnony anony, bool closed, PercentageBars.Bars percentageBar, bool appendable, bool sorted, bool archived, Dictionary<string, List<User>> pollVotes, List<MessageID> messageIds, DBHandler dBHandler, Strings.Langs lang) : base(chatId, pollId, pollText, pollDescription, anony, closed, percentageBar, appendable, sorted, archived, dBHandler, pollVotes, messageIds, lang, EPolls.vote) {

		}

		/*protected ContentParts GetContent2(Strings strings, string apikey, bool channel = false, long? offset = null, bool moderatePane = true) {
			Strings.langs oldLang = strings.CurrentLang;
			strings.SetLanguage(lang);
			string text;
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			string description = "";
			{
				int userCount = 0;
				int optionCount = 0;
				int percentageUserCount = 0;
				text = " <b>" + HtmlSpecialChars.Encode(pollText).UnmarkupUsernames() + "</b>" + (!string.IsNullOrEmpty(pollDescription) ? ("\n" + HtmlSpecialChars.Encode(pollDescription) + "\n") : "\n");
				Dictionary<string, List<User>> newVotes;
				if (this.Sorted) {
					newVotes = pollVotes.CloneDictionary();
					newVotes = newVotes.Sort();
				} else {
					newVotes = pollVotes;
				}
				if (this.PercentageBar != PercentageBars.Bars.none) {
					foreach (KeyValuePair<string, List<User>> x in pollVotes) {
						percentageUserCount += x.Value.Count;
					}
				}
				bool descTooLong = false;
				foreach (KeyValuePair<string, List<User>> x in newVotes) {
					int votingUserCount = x.Value.Count;
					text += String.Format("\n<b>{0}</b> [{1}]\n", HtmlSpecialChars.Encode(x.Key).UnmarkupUsernames(), votingUserCount);
					if (anony == EAnony.personal) {
						if (x.Value.Count > 0) {
							if (PercentageBar != PercentageBars.Bars.none)
								text += PercentageBars.RenderPercentage((double)x.Value.Count / percentageUserCount, this.PercentageBar, true);
							User last = x.Value.Last();
							foreach (User user in x.Value) {
								text += "\u200E" + (user == last ? "└" : "├") + " " + HtmlSpecialChars.Encode((user.FirstName + (user.LastName != null ? " " + user.LastName : "")).Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + "\n";
								userCount++;
							} 
						} else {
							if (PercentageBar != PercentageBars.Bars.none)
								text += PercentageBars.RenderPercentage((double)x.Value.Count / percentageUserCount, this.PercentageBar);
						}
					} else {
						if (PercentageBar != PercentageBars.Bars.none)
							text += PercentageBars.RenderPercentage((double)x.Value.Count/percentageUserCount, this.PercentageBar);
						userCount += x.Value.Count;
					}
					string unsortedKey = pollVotes.Keys.ElementAt(optionCount);
					if (!closed && !channel) {
						List<InlineKeyboardButton> thisRow = new List<InlineKeyboardButton>(1);
						thisRow.Add((InlineKeyboardButton)InlineKeyboardButton.Create(unsortedKey + " - " + pollVotes.Values.ElementAt(optionCount).Count, callbackData: Cryptography.Encrypt(chatId + ":" + pollId + ":" + optionCount, apikey)));
						inlineKeyboard.InlineKeyboard.Add(thisRow);
					}
					if (description.Length < 150)
						description += unsortedKey.Truncate(15) + " | ";
					else
						descTooLong = true;
					optionCount++;
				}
				if (descTooLong)
					description += " | ...";
				description = "Personal vote - " + userCount + " participants\n" + description;
				if (!closed && channel)
					inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					//InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.buttonVote), url:"https://telegram.me/" + Globals.Botname + "?start=" + Cryptography.Encrypt("vote:" + ChatId + ":" + PollId, apikey))
					});
				if (!closed && Appendable)
					inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					//InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.buttonAppend), url:"https://telegram.me/" + Globals.Botname + "?start=" + Cryptography.Encrypt("append:" + ChatId + ":" + PollId, apikey))
					});
				if (delete)
					inlineKeyboard = null;
				description = description.Substring(0, description.Length - 3);
				text += "\n" + string.Format(strings.GetString(userCount == 0 ? Strings.stringsList.rendererZeroVotedSoFar : (userCount == 1 ? Strings.stringsList.rendererSingleVotedSoFar : Strings.stringsList.rendererMultiVotedSoFar)), userCount);
				if (closed)
					text += "\n" + strings.GetString(Strings.stringsList.pollClosed);
			}
			strings.SetLanguage(oldLang);
			return new ContentParts(text, inlineKeyboard, description);
		}*/

		public override string RenderPollConfig(Strings strings) {
			if (Anony == EAnony.personal)
				return strings.GetString(Strings.StringsList.inlineDescriptionPersonalVote);
			return strings.GetString(Strings.StringsList.inlineDescriptionAnonymousVote);
		}

		public override bool Vote(string apikey, int optionNr, User user, Message message, string inlineMessageId = null) {
			dBHandler.AddToQueue(this);
			bool result;
			if (pollVotes.ElementAt(optionNr).Value.Exists(x => x.Id == user.Id)) {
				pollVotes.ElementAt(optionNr).Value.RemoveAt(pollVotes.ElementAt(optionNr).Value.FindIndex(x => x.Id == user.Id));
				result = false;
			} else {
				foreach (KeyValuePair<string, List<User>> x in pollVotes) {
					x.Value.RemoveAll(y => y.Id == user.Id);
				}
				pollVotes.ElementAt(optionNr).Value.Add(user);
				result = true;
			}
			return result;
		}
	}
}

