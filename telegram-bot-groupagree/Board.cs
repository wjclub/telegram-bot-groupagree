using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;
using WJClubBotFrame;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Linq;
using StringEdit = System.Globalization.CultureInfo;

namespace telegrambotgroupagree {
	public class Board : Poll {
		public Board(int chatId, int pollId, string pollText, EAnony anony, DBHandler dBHandler, Strings.langs lang) : this(chatId, pollId, pollText, null, anony, false, false, dBHandler, new Dictionary<int, BoardVote>(), new List<MessageID>(), lang) {
			
		}

		public Board(int chatId, int pollId, string pollText, string pollDescription, EAnony anony, bool closed, bool archived, DBHandler dBHandler, Dictionary<int, BoardVote> pollVotes, List<MessageID> messageIds, Strings.langs lang) : base(chatId, pollId, pollText, pollDescription, anony, closed, PercentageBars.Bars.none, false, false, archived, dBHandler, null, messageIds, lang, EPolls.board) {
			this.pollVotes = pollVotes;
		}

		new private Dictionary<int, BoardVote> pollVotes;
		new public Dictionary<int, BoardVote> PollVotes { get{ return pollVotes; } }

		protected override ContentParts GetContent(Strings strings, string apikey, bool channel = false, int? offset = null, bool moderatePane = true) {
			Strings.langs oldLang = strings.CurrentLang;
			strings.SetLanguage(lang);
			string text;
			string description;
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			if (offset == null) {
				if (!closed)
					inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.buttonVote), url: "https://telegram.me/" + Globals.Botname + "bot?start=" + Cryptography.Encrypt("board:" + ChatId.ToString() + ":" + PollId.ToString(), apikey)) });
					//inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.buttonVote), url: "https://telegram.me/" + Globals.Botname + "bot?start=" + "board:" + ChatId + ":" + PollId) });
				description = StringEdit.CurrentCulture.TextInfo.ToTitleCase(anony.ToString()) + " " + pollType + ": " + pollText + ": ";
				text = "\ud83d\udcdd <b>" + HtmlSpecialChars.Encode(pollText).UnmarkupUsernames() + "</b>" + (!string.IsNullOrEmpty(pollDescription) ? ("\n" + HtmlSpecialChars.Encode(pollDescription) + "\n") : "\n");
				foreach (KeyValuePair<int, BoardVote> x in pollVotes) {
					string toAdd = (anony == EAnony.personal ? "\n\u200E<b>" + HtmlSpecialChars.Encode(x.Value.Name.Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + ": </b>" : "\n\ud83d\udc64 ") + HtmlSpecialChars.Encode(x.Value.Text) + "\n";
					if (text.Length + toAdd.Length <= 4000) {
						text += toAdd;
						description += x.Value.Text + ", ";
					} else {
						text += "\n▶️ <a href=\"https://telegram.me/" + Globals.Botname + "bot?start=" + Cryptography.Encrypt("pag:" + chatId + ":" + pollId + ":0", apikey) + "\">" + strings.GetString(Strings.stringsList.boardShowMore) + "</a>\n";
						break;
					}
				}
				if (delete || closed)
					inlineKeyboard = null;
				description = description.Substring(0, description.Length - 2);
				text += "\n" + string.Format(strings.GetString(pollVotes.Count == 0 ? Strings.stringsList.rendererZeroVotedSoFar : (pollVotes.Count == 1 ? Strings.stringsList.rendererSingleVotedSoFar : Strings.stringsList.rendererMultiVotedSoFar)), pollVotes.Count);
				if (closed)
					text += "\n" + strings.GetString(Strings.stringsList.pollClosed);
			} else {
				description = "";
				if (offset < pollVotes.Count) {
					int difference = (int)offset - (pollVotes.Count - 1);
					text = "";
					for (int i = (int)offset; i < offset + 5; i++) {
						try {
							var x = pollVotes.ElementAt(i);
							text += (anony == EAnony.personal ? "\u200E<b>" + HtmlSpecialChars.Encode(x.Value.Name.Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + ": </b>" : "\ud83d\udc64 ") + HtmlSpecialChars.Encode(x.Value.Text) + "\n\n";
						} catch (ArgumentOutOfRangeException){
							break;
						}
					}
					inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> {
							InlineKeyboardButton.Create("<<", callbackData:"comm:pag:" + chatId + ":" + pollId + ":" + (offset - 5 >= 0 ? (offset - 5) : 0)),
							InlineKeyboardButton.Create(">>", callbackData:"comm:pag:" + chatId + ":" + pollId + ":" + (offset + 5 < pollVotes.Count ? (offset + 5) : pollVotes.Count - 1))
						});
					text += string.Format(strings.GetString(Strings.stringsList.boardPagBottomLine), (offset + 1), (pollVotes.Count - offset < 5 ? pollVotes.Count - (offset - 5) : (offset + 5)),pollVotes.Count);
				} else {
					text = strings.GetString(Strings.stringsList.boardPagCantFind);
				}
			}
			strings.SetLanguage(oldLang);
			return new ContentParts(text, inlineKeyboard, description);
		}

		public override bool Vote(string apikey, int optionNr, User user, Message message, string inlineMessageId = null) {
			dBHandler.AddToQueue(this);
			if (closed)
				return false;
			pollVotes.Remove(user.Id);
			pollVotes.Add(user.Id, new BoardVote {
				Name = user.FirstName + (user.LastName != null ? " " + user.LastName : ""),
				Text = message.Text
			});
			return true;
		}

		public override MySqlCommand GenerateCommand(MySqlConnection connection, string apikey, Strings strings, bool change = true) {
			MySqlCommand command = new MySqlCommand();
			command.Connection = connection;
			if (delete) {
				command.CommandText = "DELETE FROM `polls` WHERE `chatid`=?chatid and`pollid`=?pollid;";
				command.Parameters.AddWithValue("?chatid", chatId);
				command.Parameters.AddWithValue("?pollid", pollId);
			} else {
				command.CommandText = "REPLACE INTO polls (chatid, pollid, pollText, pollDescription, pollVotes, messageIds, anony, closed, pollType, archived, lang) VALUES (?chatid, ?pollid, ?pollText, ?pollDescription, ?pollVotes, ?messageIds, ?anony, ?closed, ?pollType, ?archived, ?lang);";
				command.Parameters.AddWithValue("?chatid", chatId);
				command.Parameters.AddWithValue("?pollid", pollId);
				command.Parameters.AddWithValue("?pollText", pollText);
				command.Parameters.AddWithValue("?pollDescription", pollDescription);
				command.Parameters.AddWithValue("?pollVotes", JsonConvert.SerializeObject(pollVotes));
				command.Parameters.AddWithValue("?messageIds", JsonConvert.SerializeObject(messageIds));
				command.Parameters.AddWithValue("?anony", anony);
				command.Parameters.AddWithValue("?closed", closed);
				command.Parameters.AddWithValue("?pollType", pollType);
				command.Parameters.AddWithValue("?archived", archived);
				command.Parameters.AddWithValue("?lang", lang);
				if (change)
					Update(apikey, strings);
			}
			return command;
		}
	}
}

