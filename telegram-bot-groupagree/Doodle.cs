using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using WJClubBotFrame;
using WJClubBotFrame.Methods;
using WJClubBotFrame.Types;
using StringEdit = System.Globalization.CultureInfo;

namespace telegrambotgroupagree {
	public class Doodle : Poll {
		public Doodle(int chatId, int pollId, string pollText, EAnony anony, DBHandler dBHandler, Strings.Langs lang) : this(chatId, pollId, pollText, null, anony, false, PercentageBars.Bars.none, false, false, false, new Dictionary<string, List<User>>(), new List<MessageID>(), new List<User>(), dBHandler, lang) {
			
		}

		public Doodle(int chatId, int pollId, string pollText, string pollDescription, EAnony anony, bool closed, PercentageBars.Bars percentageBar, bool appendable, bool sorted, bool archived, Dictionary<string, List<User>> pollVotes, List<MessageID> messageIds, List<User> people, DBHandler dBHandler, Strings.Langs lang) : base(chatId, pollId, pollText, pollDescription, anony, closed, percentageBar, appendable, sorted, archived, dBHandler, pollVotes, messageIds, lang, EPolls.doodle) {
			this.people = people;
		}

		protected List<User> people;


		public override List<int> CountVotes(out int peopleCount) {
			peopleCount = people.Count;
			int tellMeHowICanDoThisDifferentlyButForNowItIsLikeThisSoLiveWithIt = 0;
			return base.CountVotes(out tellMeHowICanDoThisDifferentlyButForNowItIsLikeThisSoLiveWithIt);
		}
		/*protected override ContentParts GetContent(Strings strings, string apikey, bool channel = false, int? offset = null, bool moderatePane = true) {
			Strings.langs oldLang = strings.CurrentLang;
			strings.SetLanguage(lang);
			string text;
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			string description;
			{
				description = StringEdit.CurrentCulture.TextInfo.ToTitleCase(anony.ToString()) + " " + pollType + ": " + pollText + ": ";
				int optionCount = 0;
				text = "\ud83d\udcca <b>" + HtmlSpecialChars.Encode(pollText).UnmarkupUsernames() + "</b>" + (!string.IsNullOrEmpty(pollDescription) ? ("\n" + HtmlSpecialChars.Encode(pollDescription) + "\n") : "\n");
				foreach (KeyValuePair<string, List<User>> x in pollVotes) {
					int votingUserCount = x.Value.Count;
					text += "\n<b>" + HtmlSpecialChars.Encode(x.Key).UnmarkupUsernames() + "</b> [" + votingUserCount + "]\n";
					if (anony == EAnony.personal) {
						if (x.Value.Count > 0) {
							User last = x.Value.Last();
							foreach (User user in x.Value) {
								text += "\u200E" + (user == last ? "└" : "├") + " " + HtmlSpecialChars.Encode((user.FirstName + (user.LastName != null ? " " + user.LastName : "")).Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + "\n";
							}
						}
					}
					if (!closed && !channel) {
						List<InlineKeyboardButton> thisRow = new List<InlineKeyboardButton>(1);
						thisRow.Add((InlineKeyboardButton)InlineKeyboardButton.Create(x.Key + " - " + votingUserCount, callbackData: Cryptography.Encrypt(chatId + ":" + pollId + ":" + optionCount, apikey)));
						inlineKeyboard.InlineKeyboard.Add(thisRow);
					}
					description += x.Key + ", ";
					optionCount++;
				}
				description = description.Substring(0, description.Length - 2);
				int userCount = people.Count;
				if (channel)
					inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.buttonVote), url:"https://telegram.me/" + Globals.GlobalOptions.Botname + "?start=" + Cryptography.Encrypt("vote:" + ChatId.ToString() + ":" + PollId.ToString(), apikey))
					});
				text += "\n" + string.Format(strings.GetString(userCount == 0 ? Strings.stringsList.rendererZeroVotedSoFar : (userCount == 1 ? Strings.stringsList.rendererSingleVotedSoFar : Strings.stringsList.rendererMultiVotedSoFar)), userCount);
				if (delete || closed) {
					inlineKeyboard = null;
					text += "\n" + strings.GetString(Strings.stringsList.pollClosed);
				}
			}
			strings.SetLanguage(oldLang);
			return new ContentParts(text, inlineKeyboard, description);
		}*/

		public override string RenderPollConfig(Strings strings) {
			if (Anony == EAnony.personal)
				return strings.GetString(Strings.StringsList.inlineDescriptionPersonalDoodle);
			return strings.GetString(Strings.StringsList.inlineDescriptionAnonymousDoodle);
		}

		public override bool Vote(string apikey, int optionNr, User user, Message message, string inlineMessageId = null) {
			dBHandler.AddToQueue(this);
			bool result;
			if (pollVotes.ElementAt(optionNr).Value.Exists(x => x.Id == user.Id)) {
				pollVotes.ElementAt(optionNr).Value.RemoveAt(pollVotes.ElementAt(optionNr).Value.FindIndex(x => x.Id == user.Id));
				bool notThere = true;
				foreach (KeyValuePair <string, List<User>> x in pollVotes) {
					if (x.Value.Exists(y => y.Id == user.Id))
						notThere = false;
				}
				if (notThere)
					people.RemoveAll(z => z.Id == user.Id);
				result = false;
			} else {
				pollVotes.ElementAt(optionNr).Value.Add(user);
				people.RemoveAll(x => x.Id == user.Id);
				people.Add(user);
				result = true;
			}
			return result;
		}

		public override MySqlCommand GenerateCommand(MySqlConnection connection, long currentBotChatID, Strings strings, List<Instance> instances, bool noApproximation, bool change = true) {
			var command = new MySqlCommand();
			command.Connection = connection;
			if (delete) {
				command.CommandText = "DELETE FROM `polls` WHERE `chatid`=?chatid and`pollid`=?pollid;";
				command.Parameters.AddWithValue("?chatid", chatId);
				command.Parameters.AddWithValue("?pollid", pollId);
			} else {
				command.CommandText = "REPLACE INTO polls (chatid, pollid, pollText, pollDescription, pollVotes, messageIds, anony, closed, percentageBar, appendable, sorted, pollType, people, archived, lang) VALUES (?chatid, ?pollid, ?pollText, ?pollDescription, ?pollVotes, ?messageIds, ?anony, ?closed, ?percentageBar, ?appendable, ?sorted, ?pollType, ?people, ?archived, ?lang);";
				command.Parameters.AddWithValue("?chatid", chatId);
				command.Parameters.AddWithValue("?pollid", pollId);
				command.Parameters.AddWithValue("?pollText", pollText);
				command.Parameters.AddWithValue("?pollDescription", pollDescription);
				command.Parameters.AddWithValue("?pollVotes", JsonConvert.SerializeObject(this.pollVotes));
				command.Parameters.AddWithValue("?messageIds", JsonConvert.SerializeObject(messageIds));
				command.Parameters.AddWithValue("?anony", anony);
				command.Parameters.AddWithValue("?closed", closed);
				command.Parameters.AddWithValue("?percentageBar", PercentageBar);
				command.Parameters.AddWithValue("?appendable", Appendable);
				command.Parameters.AddWithValue("?sorted", Sorted);
				command.Parameters.AddWithValue("?pollType", pollType);
				command.Parameters.AddWithValue("?archived", archived);
				command.Parameters.AddWithValue("?lang", lang);
				command.Parameters.AddWithValue("?people", JsonConvert.SerializeObject(people));
				/*if (change) {
					Update(instances, currentBotChatID, strings, noApproximation:noApproximation).Wait();
				}*/
			}
			return command;
		}
	}
}

