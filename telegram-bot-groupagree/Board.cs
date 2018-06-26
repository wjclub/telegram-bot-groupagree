using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;
using WJClubBotFrame;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Linq;
using StringEdit = System.Globalization.CultureInfo;
using System.Runtime.Serialization;
using System.IO;

namespace telegrambotgroupagree {
	public class Board:Poll {
		public Board(int chatId, int pollId, string pollText, EAnony anony, DBHandler dBHandler, Strings.Langs lang) : this(chatId, pollId, pollText, null, anony, false, false, dBHandler, new Dictionary<int, BoardVote>(), new List<MessageID>(), lang) {
			this.pollType = EPolls.board;
		}

		public Board(int chatId, int pollId, string pollText, string pollDescription, EAnony anony, bool closed, bool archived, DBHandler dBHandler, Dictionary<int, BoardVote> pollVotes, List<MessageID> messageIds, Strings.Langs lang) : base(chatId, pollId, pollText, pollDescription, anony, closed, PercentageBars.Bars.none, false, false, archived, dBHandler, null, messageIds, lang, EPolls.board) {
			this.pollType = EPolls.board;
			this.pollVotes = pollVotes;
		}

		new private Dictionary<int, BoardVote> pollVotes;
		new public Dictionary<int, BoardVote> PollVotes { get { return pollVotes; } }

		public override string RenderVotes(List<int> pollVotesCount, int peopleCount, bool noApproximation) {
			string votesString = "";
			foreach (KeyValuePair<int, BoardVote> x in pollVotes) {
				string toAdd = (anony == EAnony.personal ? "\n\u200E<b>" + HtmlSpecialChars.Encode(x.Value.Name.Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + ": </b>" : "\n\ud83d\udc64 ") + HtmlSpecialChars.Encode(x.Value.Text) + "\n";
				if (votesString.Length + toAdd.Length <= 4000) {
					votesString += toAdd;
				} else {
					//TODO Throw stuff
					votesString += "\n▶️ <a href=\"https://telegram.me/" + Globals.GlobalOptions.Botname + "?start=" + Cryptography.Encrypt("pag:" + chatId + ":" + pollId + ":0") + "\">Show more...</a>\n";
					break;
				}
			}
			return votesString;
		}

		public override InlineKeyboardMarkup RenderInlineKeyboard(List<int> pollVotesCount, Strings strings, bool noApproximation, bool channel = false) {
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			if (!closed && !delete) {
				inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.buttonVote), url: $"https://telegram.me/{ Globals.GlobalOptions.Botname }?start={ Cryptography.Encrypt($"board:{ ChatId.ToString() }:{ PollId.ToString() }") }") });
			}
			return inlineKeyboard;
		}

		public override List<InlineKeyboardButton> RenderInlineButton(string optionTitle, int optionCount, int votersCount, bool noApproximation) => null;

		public override List<int> CountVotes(out int peopleCount) {
			List<int> output = new List<int>();
			peopleCount = 0;
			if (PollVotes != null) {
				foreach (BoardVote boardVote in PollVotes.Values) {
					output.Add(1);
					peopleCount += 1;
				}
			}
			return output;
		}

		public override string RenderPollConfig(Strings strings) {
			if (Anony == EAnony.personal) {
				return strings.GetString(Strings.StringsList.inlineDescriptionPersonalBoard);
			} else {
				return strings.GetString(Strings.StringsList.inlineDescriptionAnonymousBoard);
			}
		}

		public override string RenderInlineQueryDescription(Strings strings, int peopleCount) {
			string output = string.Format(strings.GetString(Strings.StringsList.inlineDescriptionFirstLine) + "\n", RenderPollConfig(strings), peopleCount);
			bool descTooLong = false;
			foreach (BoardVote optionTitle in PollVotes.Values) {
				if (output.Length < 100) {
					output += new StringReader(optionTitle.Text).ReadLine().RemoveAppendingText().Truncate(15) + " | ";
				} else {
					descTooLong = true;
				}
			}
			if (descTooLong) {
				output += " ...";
			} else {
				output = output.Substring(0, output.Length - 3);
			}
			return output;
		}

		public override string GetOptionText(int optionID) 
			=> PollVotes.ElementAt(optionID).Value.Text;

		public override bool DeleteOption(int optionID, string crc32Hash) {
			if (PollVotes.Count == 0) {
				return false;
			}
			string currentVoteText = PollVotes.ElementAt(optionID).Value.Text;
			if (crc32Hash == null || currentVoteText.HashCRC32() == crc32Hash) {
				PollVotes.Remove(optionID);
				dBHandler.AddToQueue(this, true);
			} else {
				throw new BoardTextDoesntMatch("Hashes for poll vote do not match");
			}
			return true;
		}

		protected ContentParts GetPollOutputOld(Strings strings, int peopleCount, List<int> pollVotesCount, bool noApproximation, bool channel = false) {
			Strings.Langs oldLang = strings.CurrentLang;
			strings.SetLanguage(lang);
			string text;
			string inlineTitle = this.pollText;
			//TODO Make this pretty
			string inlineDescription;
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
			inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>();
			//if (offset == null) {
			if (!closed) {
				inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.buttonVote), url: $"https://telegram.me/{ Globals.GlobalOptions.Botname }?start={ Cryptography.Encrypt($"board:{ ChatId.ToString() }:{ PollId.ToString() }") }") });
			}
			inlineDescription = StringEdit.CurrentCulture.TextInfo.ToTitleCase(anony.ToString()) + " " + pollType + ": " + pollText + ": ";
			text = "\ud83d\udcdd <b>" + HtmlSpecialChars.Encode(pollText).UnmarkupUsernames() + "</b>" + (!string.IsNullOrEmpty(pollDescription) ? ("\n" + HtmlSpecialChars.Encode(pollDescription) + "\n") : "\n");
			foreach (KeyValuePair<int, BoardVote> x in pollVotes) {
				string toAdd = (anony == EAnony.personal ? "\n\u200E<b>" + HtmlSpecialChars.Encode(x.Value.Name.Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + ": </b>" : "\n\ud83d\udc64 ") + HtmlSpecialChars.Encode(x.Value.Text) + "\n";
				if (text.Length + toAdd.Length <= 4000) {
					text += toAdd;
					inlineDescription += x.Value.Text + ", ";
				} else {
					text += "\n▶️ <a href=\"https://telegram.me/" + Globals.GlobalOptions.Botname + "?start=" + Cryptography.Encrypt("pag:" + chatId + ":" + pollId + ":0") + "\">" + strings.GetString(Strings.StringsList.boardShowMore) + "</a>\n";
					break;
				}
			}
			if (delete || closed) {
				inlineKeyboard = null;
			}
			inlineDescription = inlineDescription.Substring(0, inlineDescription.Length - 2);
			text += "\n" + string.Format(strings.GetString(pollVotes.Count == 0 ? Strings.StringsList.rendererZeroVotedSoFar : (pollVotes.Count == 1 ? Strings.StringsList.rendererSingleVotedSoFar : Strings.StringsList.rendererMultiVotedSoFar)), pollVotes.Count);
			if (closed)
				text += "\n" + strings.GetString(Strings.StringsList.pollClosed);
			/*} else {
				inlineDescription = "";
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
					text += string.Format(strings.GetString(Strings.StringsList.boardPagBottomLine), (offset + 1), (pollVotes.Count - offset < 5 ? pollVotes.Count - (offset - 5) : (offset + 5)),pollVotes.Count);
				} else {
					text = strings.GetString(Strings.StringsList.boardPagCantFind);
				}
			}*/
			strings.SetLanguage(oldLang);
			return new ContentParts(text, inlineKeyboard, inlineTitle, inlineDescription);
		}

		public override bool Vote(string apikey, int optionNr, User user, Message message, string inlineMessageId = null) {
			dBHandler.AddToQueue(this);
			if (closed) {
				return false;
			}
			pollVotes.Remove(user.Id);
			pollVotes.Add(user.Id, new BoardVote {
				Name = user.FirstName + (user.LastName != null ? " " + user.LastName : ""),
				Text = message.Text
			});
			return true;
		}

		public override MySqlCommand GenerateCommand(MySqlConnection connection, long currentBotChatID, Strings strings, List<Instance> instances, bool forceNoApproximation, bool change = true) {
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
				/*if (change)
					//TODO catch stuff
					Update(instances, currentBotChatID, strings, noApproximation:true).Wait();*/
			}
			return command;
		}
	}

	[Serializable]
	internal class BoardTextDoesntMatch:Exception {
		public BoardTextDoesntMatch() {
		}

		public BoardTextDoesntMatch(string message) : base(message) {
		}

		public BoardTextDoesntMatch(string message, Exception innerException) : base(message, innerException) {
		}

		protected BoardTextDoesntMatch(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}