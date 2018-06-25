using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using WJClubBotFrame;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace telegrambotgroupagree {
	public abstract class Poll {
		protected Poll(int chatId, int pollId, string pollText, string pollDescription, EAnony anony, bool closed, PercentageBars.Bars percentageBar, bool appendable, bool sorted, bool archived, DBHandler dBHandler, Dictionary<string, List<User>> pollVotes, List<MessageID> messageIds, Strings.Langs lang, EPolls pollType = EPolls.vote) {
			this.chatId = chatId;
			this.pollId = pollId;
			this.pollText = pollText;
			this.pollDescription = pollDescription;
			this.anony = anony;
			this.pollVotes = pollVotes;
			this.messageIds = messageIds;
			this.closed = closed;
			this.archived = archived;
			this.dBHandler = dBHandler;
			this.pollType = pollType;
			this.lang = lang;
			this.PercentageBar = percentageBar;
			this.Appendable = appendable;
			this.Sorted = sorted;
		}

		protected int chatId;
		public int ChatId { get { return this.chatId; } }
		protected int pollId;
		public int PollId { get { return this.pollId; } }
		protected string pollText;
		public string PollText { get { return this.pollText; } }
		protected string pollDescription;
		public string PollDescription { get { return this.pollDescription; } }
		protected Dictionary<string, List<User>> pollVotes;
		public Dictionary<string, List<User>> PollVotes { get { return this.pollVotes; } }
		protected List<MessageID> messageIds;
		public List<MessageID> MessageIds { get { return this.messageIds; } }
		protected EAnony anony;
		public EAnony Anony { get { return this.anony; } }
		protected bool closed;
		public bool Closed { get { return this.closed; } }
		protected bool archived;
		public bool Archived { get { return this.archived; } }
		protected EPolls pollType;
		protected bool delete;
		public EPolls PollType { get { return this.pollType; } }
		protected Strings.Langs lang;
		public Strings.Langs Lang { get { return this.lang; } }
		public PercentageBars.Bars PercentageBar { get; internal set; }
		public bool Sorted { get; internal set; }
		public bool Appendable { get; internal set; }
		public DBHandler dBHandler;

		public override string ToString() {
			return "Whoopsy daisy\nI thought I could remove this...\nApparently I cannot...\nPlease forward this message to @wjclub :D";
		}

		public virtual bool AddOption(string text) {
			try {
				pollVotes.Add(text, new List<User>());
			} catch (System.ArgumentException) {
				return false;
			}
			dBHandler.AddToQueue(this, false);
			return true;
		}

		public virtual void AddDescription(string text) {
			this.pollDescription = text;
			dBHandler.AddToQueue(this, false);
		}

		public virtual void Close(List<Instance> instances, long currentBotChatID, Strings strings, int messageId) {
			closed = true;
			dBHandler.AddToQueue(this, change: true, forceNoApproximation: true);
			//TODO catch stuff
			Update(instances, currentBotChatID, strings, true, messageId: messageId).Wait();
		}

		public virtual void SetPercentage(PercentageBars.Bars bar) {
			this.PercentageBar = bar;
			dBHandler.AddToQueue(this, true);
		}

		public virtual void SetSorted(bool sorted) {
			this.Sorted = sorted;
			dBHandler.AddToQueue(this, change: true);
		}

		public virtual void SetAppendable(bool appendable) {
			this.Appendable = appendable;
			dBHandler.AddToQueue(this, change: true);
		}

		public virtual void Reopen(List<Instance> instances, long currentBotChatID, Strings strings, int messageId) {
			closed = false;
			dBHandler.AddToQueue(this, change: true);
			//TODO catch stuff
			Update(instances, currentBotChatID, strings, true, messageId: messageId).Wait();
		}

		public virtual void Delete(List<Instance> instances, long currentBotChatID, Strings strings, int messageId) {
			delete = true;
			dBHandler.AddToQueue(this, change: true, forceNoApproximation: true);
			//TODO catch stuff
			Update(instances, currentBotChatID, strings, true, messageId: messageId).Wait();
		}

		public virtual void DeleteFromDeleteAll(string apikey, Strings strings) {
			delete = true;
			dBHandler.AddToQueue(this, change: true, forceNoApproximation: true);
		}

		protected virtual ContentParts GetContent(Strings strings, string apikey, bool noApproximation, bool channel = false, int? offset = null, bool moderatePane = false) {
			List<int> pollVotesCount = CountVotes(out int peopleCount);
			ContentParts output;
			if (moderatePane) {
				output = GetModeratePane(strings);
			} else {
				Strings.Langs oldLang = strings.CurrentLang;
				strings.SetLanguage(lang);
				output = GetPollOutput(strings, peopleCount, pollVotesCount, noApproximation, channel: channel);
				strings.SetLanguage(oldLang);
			}
			return output;
		}

		public virtual List<int> CountVotes(out int peopleCount) {
			List<int> output = new List<int>();
			peopleCount = 0;
			if (PollVotes != null) {
				foreach (List<User> voters in PollVotes.Values) {
					output.Add(voters.Count);
					peopleCount += voters.Count;
				}
			}
			return output;
		}

		#region GetPollOutput
		protected virtual ContentParts GetPollOutput(Strings strings, int peopleCount, List<int> pollVotesCount, bool noApproximation, bool channel = false) {
			return new ContentParts(RenderText(strings, peopleCount, pollVotesCount, noApproximation), RenderInlineKeyboard(pollVotesCount, strings, noApproximation: noApproximation, channel: channel), RenderInlineQueryTitle(), RenderInlineQueryDescription(strings, peopleCount));
		}

		#region RenderText
		public virtual string RenderText(Strings strings, int peopleCount, List<int> pollVotesCount, bool noApproximation) {
			return RenderTitleEmoji(strings) + " " + RenderTitle() + "\n" +
			RenderDescription() +
			RenderVotes(pollVotesCount, peopleCount, noApproximation) + "\n" +
			RenderPeopleCount(strings, peopleCount, noApproximation) + "\n" +
			RenderOptionalInsert(strings) +
			//RenderExpansion(strings) +
			RenderState(strings);
		}

		public virtual string RenderTitleEmoji(Strings strings) {
			return "\ud83d\udcca";
		}

		public virtual string RenderTitle() {
			return "<b>" + HtmlSpecialChars.Encode(this.PollText).UnmarkupUsernames() + "</b>";
		}

		public virtual string RenderDescription() {
			return (!string.IsNullOrEmpty(this.PollDescription) ? (HtmlSpecialChars.Encode(this.PollDescription) + "\n") : "");
		}

		public virtual string RenderVotes(List<int> pollVotesCount, int peopleCount, bool noApproximation) {
			string output = "";
			Dictionary<string, List<User>> MaybeSortedPollVotes;
			if (this.Sorted) {
				MaybeSortedPollVotes = pollVotes.CloneDictionary();
				MaybeSortedPollVotes = MaybeSortedPollVotes.Sort();
			} else {
				MaybeSortedPollVotes = pollVotes;
			}
			foreach (KeyValuePair<string, List<User>> currentOption in MaybeSortedPollVotes) {
				output += RenderOptionTitle(currentOption, noApproximation: noApproximation);
				if (this.PercentageBar != PercentageBars.Bars.none)
					output += RenderPercentage(currentOption.Value.Count, peopleCount);
				if (Anony == EAnony.personal)
					output += RenderVotingUsers(currentOption.Value, peopleCount);
			}
			return output;
		}

		public static Match GetAppendingMatch(string input) {
			Regex regex = new Regex(@"(?<=^\/\/BY:)\d*(?=\/\/)");
			return regex.Match(input);
		}

		public virtual string RenderOptionTitle(KeyValuePair<string, List<User>> option, bool noApproximation, bool moderate = false) {
			Match match = GetAppendingMatch(option.Key);
			string[] lines = option.Key.Split('\n');
			//TODO Display Name
			string output = "\n" + (match.Success ? (moderate ? RenderUserForModeration(match.Value, displayName: null) : "") : "") + string.Format("<b>{0}</b> [{1}]\n", HtmlSpecialChars.Encode((match.Success ? lines[0].Substring(match.Index + match.Length + 2) : lines[0])).UnmarkupUsernames(), RenderNumberUpdateFriendly(option.Value.Count, noApproximation));
			for (int i = 1; i < lines.Length; i++)
				output += (option.Value.Count != 0 ? "┆ " : "") + HtmlSpecialChars.Encode(lines[i]) + "\n";
			return output;
		}

		string RenderPercentage(int currentOptionUserCount, int peopleCount) {
			return PercentageBars.RenderPercentage((double)currentOptionUserCount / peopleCount, this.PercentageBar, true);
		}

		public virtual string RenderPollConfig(Strings strings) {
			throw new KeyNotFoundException("You need to put this method in your poll class");
		}

		public virtual string RenderVotingUsers(List<User> users, int peopleCount) {
			string output = "";
			int count = 0;
			foreach (User user in users) {
				output += "\u200E" + (count == users.Count - 1 ? "└" : "├") + " " + HtmlSpecialChars.Encode((user.FirstName + (user.LastName != null ? " " + user.LastName : "")).Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + "\n";
				count++;
			}
			return output;
		}

		public virtual string RenderPeopleCount(Strings strings, int peopleCount, bool noApproximation) {
			return string.Format(strings.GetString(peopleCount == 0 ? Strings.StringsList.rendererZeroVotedSoFar : (peopleCount == 1 ? Strings.StringsList.rendererSingleVotedSoFar : Strings.StringsList.rendererMultiVotedSoFar)), RenderNumberUpdateFriendly(peopleCount, noApproximation));
		}

		public virtual string RenderNumberUpdateFriendly(int input, bool skipApproximation) {
			if (skipApproximation) {
				return input.ToString();
			}
			int[] lookupTable = {
				10, 15, 20, 30, 50, 100, 200, 300, 500, 750, 1000,
			};
			int power = 0;
			while (input > 1000) {
				input /= 1000;
				power += 1;
			}
			string[] powerLookup = { "", "K", "M", "B" };
			string output = "";
			if (input <= lookupTable[0]) {
				output = input.ToString();
			} else {
				for (int i = 1; i < lookupTable.Length; i++) {
					if (input - lookupTable[i] <= 0) {
						output = $"{lookupTable[i - 1].ToString()}-{lookupTable[i].ToString()}";
						break;
					}
				}
			}
			output += powerLookup[power];
			return output;
		}

		public virtual string RenderOptionalInsert(Strings strings) {
			return "";
		}

		public virtual string RenderState(Strings strings) {
			if (this.Closed)
				return "\n" + strings.GetString(Strings.StringsList.pollClosed);
			if (this.delete) {
				return "\n" + strings.GetString(Strings.StringsList.pollDeleted);
			} else {
				return "";
			}
		}

		public virtual string RenderExpansion(Strings strings) {
			return "<a href=\"" + "https://hamilton.wjclub.tk/" + Globals.GlobalOptions.Botname + "/" + Cryptography.Encrypt("lookup:" + this.ChatId + ":" + this.PollId, Globals.GlobalOptions.Apikey) + "\">\ud83c\udf0e " + strings.GetString(Strings.StringsList.viewInBrowserExpansion) + "</a>";
		}
		#endregion

		#region RenderInlineKeyboard
		public virtual InlineKeyboardMarkup RenderInlineKeyboard(List<int> pollVotesCount, Strings strings, bool noApproximation, bool channel = false) {
			if (Closed || delete) {
				return new InlineKeyboardMarkup();
			}
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup {
				InlineKeyboard = new List<List<InlineKeyboardButton>>()
			};
			if (channel) {
				inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.buttonVote), url: "https://telegram.me/" + Globals.GlobalOptions.Botname + "?start=" + Cryptography.Encrypt("vote:" + ChatId + ":" + PollId, Globals.GlobalOptions.Apikey)) });
			} else {
				int optionCount = 0;
				foreach (string optionTitle in PollVotes.Keys) {
					inlineKeyboard.InlineKeyboard.Add(RenderInlineButton(optionTitle, optionCount, pollVotesCount.ElementAt(optionCount), noApproximation));
					optionCount++;
				}
			}
			if (Appendable) {
				inlineKeyboard.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.buttonAppend), url: "https://telegram.me/" + Globals.GlobalOptions.Botname + "?start=" + Cryptography.Encrypt("append:" + ChatId + ":" + PollId, Globals.GlobalOptions.Apikey)) });
			}
			return inlineKeyboard;
		}

		public virtual List<InlineKeyboardButton> RenderInlineButton(string optionTitle, int optionCount, int votersCount, bool noApproximation) {
			string oneLineTitle = optionTitle.Split('\n')[0];
			Match match = GetAppendingMatch(oneLineTitle);
			if (match.Success) {
				oneLineTitle = oneLineTitle.Substring(match.Index + match.Length + 2);
			}
			var inlineKeyboardRow = new List<InlineKeyboardButton> {
				InlineKeyboardButton.Create(oneLineTitle.Truncate(30) + " - " + RenderNumberUpdateFriendly(votersCount, noApproximation), callbackData: Cryptography.Encrypt(chatId + ":" + pollId + ":" + optionCount, Globals.GlobalOptions.Apikey))
			};
			return inlineKeyboardRow;
		}
		#endregion

		#region RenderInlineQueryTitle
		public virtual string RenderInlineQueryTitle() {
			return this.PollText.Truncate(25);
		}
		#endregion

		#region RenderInlineQueryDescription
		public virtual string RenderInlineQueryDescription(Strings strings, int peopleCount) {
			string output = string.Format(strings.GetString(Strings.StringsList.inlineDescriptionFirstLine) + "\n", RenderPollConfig(strings), peopleCount);
			bool descTooLong = false;
			foreach (string optionTitle in PollVotes.Keys) {
				if (output.Length < 100) {
					output += new StringReader(optionTitle).ReadLine().RemoveAppendingText().Truncate(15) + " | ";
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

		public virtual string GetOptionText(int optionID) {
			return PollVotes.ElementAt(optionID).Key.RemoveAppendingText();
		}

		public virtual bool DeleteOption(int optionID, string crc32Hash) {
			if (PollVotes.Count <= 1) {
				return false;
			}
			string currentKey = PollVotes.ElementAt(optionID).Key;
			if (currentKey.HashCRC32() == crc32Hash) {
				PollVotes.Remove(currentKey);
				dBHandler.AddToQueue(this, true);
			} else {
				throw new FormatException("Hashes for poll vote do not match");
			}
			return true;
		}

		#endregion
		#endregion

		#region GetModeratePane
		protected virtual ContentParts GetModeratePane(Strings strings) {
			return new ContentParts(RenderModerationText(strings), RenderModerationInlineKeyboard(strings), "Contact @wjclub with a screenshot", "Please send @wjclub a screenshot of this (Error info:" + chatId + ":" + pollId + ")");
		}

		protected virtual string RenderModerationText(Strings strings) {
			string output = RenderModerationEmoji() + " " + RenderModerationTitle(strings) + "\n";
			output += RenderModerationDescription();
			output += RenderModerationVotes();
			return output;
		}

		protected virtual string RenderModerationEmoji() {
			return "✏️";
		}

		protected virtual string RenderModerationTitle(Strings strings) {
			return RenderTitle();
		}

		protected virtual string RenderModerationDescription() {
			return RenderDescription().Replace('\n', ' ').Truncate(25) + "\n";
		}

		protected virtual string RenderModerationVotes() {
			string output = "";
			int loopCycles = 0;
			foreach (KeyValuePair<string, List<User>> currentOption in PollVotes) {
				output += RenderOptionTitle(currentOption, noApproximation: true, moderate: true)
					+ "/delete_" + HashWorker.Base53Encode(PollId + ":" + loopCycles + ":" + CRC32.HashCRC32(currentOption.Key)) + "\n";
				loopCycles++;
			}
			return output;
		}

		//protected virtual string RenderModerationVoteOptions(Strings strings) {
		//	string output = "";
		//	Regex regex = new Regex(@"(?<=^\/\/BY:)\d*(?=\/\/)");
		//	foreach (KeyValuePair<string,List<User>> option in PollVotes) {
		//		Match match = regex.Match(option.Key);
		//		if (match.Success) {
		//			output += RenderModerationVoteOption(match.Value, option.Key, option.Value.Count, match.Index + match.Length + 2);
		//		} else {
		//			output += RenderOptionTitle(option);
		//		}
		//	}
		//	return output;
		//}

		//"\n<a href='tg://user?id=" + match.Value + "'>" + match.Value + "</a>: <b>" + option.Key.Substring(match.Index + match.Length + 2) + "</b>\n";

		//protected virtual string RenderModerationVoteOption(string creatorChatID, string optionText, int peopleCount, int cutFront) {
		//	return "\n<b>" + optionText.Substring(cutFront) + "</b>\n";
		//}

		protected virtual string RenderUserForModeration(string userChatID, string displayName = null)
			=> "<a href='tg://user?id=" + (displayName ?? userChatID) + "'>" + userChatID + "</a>: ";

		public virtual InlineKeyboardMarkup RenderModerationInlineKeyboard(Strings strings) {
			InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup {
				InlineKeyboard = new List<List<InlineKeyboardButton>>
			{
				new List<InlineKeyboardButton> {
				InlineKeyboardButton.Create(EmojiStore.Done + " " + strings.GetString(Strings.StringsList.done), callbackData:"comm:options:" + this.ChatId + ":" + this.PollId)
			}
			}
			};
			return inlineKeyboard;
		}

		#endregion

		public void AddToMessageIDs(MessageID messageID) {
			messageIds.Add(messageID);
			dBHandler.AddToQueue(this, false);
		}

		protected virtual InlineKeyboardMarkup GenerateUserMarkup(Strings strings, string apikey) {
			if (delete) {
				return null;
			}
			InlineKeyboardMarkup inline = new InlineKeyboardMarkup {
				InlineKeyboard = new List<List<InlineKeyboardButton>>()
			};
			if (!closed) {
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.publish), switchInlineQuery: pollText) });
				if (pollType != EPolls.board) {
					inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.publishWithLink), switchInlineQuery: "$c:" + pollText) });
					inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.buttonVote), callbackData: "comm:iwannavote:" + Cryptography.Encrypt(chatId+ ":" + pollId, apikey)),
																			   InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.commPageRefresh), callbackData:"comm:update:" + chatId + ":" + pollId)});
				} else {
					inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.buttonVote), callbackData: "comm:url" + Cryptography.Encrypt("board:" + chatId + ":" + pollId, apikey)),
																			InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.commPageRefresh), callbackData:"comm:update:" + chatId + ":" + pollId)});
				}
			}
			inline.InlineKeyboard.Add(new List<InlineKeyboardButton>() {
				InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.commPageOptions), callbackData:"comm:options:" + chatId + ":" + pollId),
				InlineKeyboardButton.Create((closed ? strings.GetString(Strings.StringsList.commPageReopen) : strings.GetString(Strings.StringsList.commPageClose)), callbackData:"comm:" + (closed ? "reopen:" : "close:") + chatId + ":" + pollId),
				InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.commPageDelete), callbackData:"comm:delete:" + chatId + ":" + pollId),
			});
			return inline;
		}

		ReplyMarkup GenerateOptionsMarkup(Strings strings) {
			InlineKeyboardMarkup inline = new InlineKeyboardMarkup {
				InlineKeyboard = new List<List<InlineKeyboardButton>>()
			};
			if (this.pollType != EPolls.board) {
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.StringsList.optionsPercentageNone), (PercentageBar == PercentageBars.Bars.none).ToCheck()), callbackData:"comm:percentage:none:" + chatId + ":" + pollId),
					InlineKeyboardButton.Create(PercentageBars.GetIconArray(PercentageBars.Bars.dots).Implode() + " " + (PercentageBar == PercentageBars.Bars.dots).ToCheck(), callbackData:"comm:percentage:dots:" + chatId + ":" + pollId),
					InlineKeyboardButton.Create(PercentageBars.GetIconArray(PercentageBars.Bars.thumbs).Implode() + " " + (PercentageBar == PercentageBars.Bars.thumbs).ToCheck(), callbackData:"comm:percentage:thumbs:" + chatId + ":" + pollId),
				});
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.StringsList.optionsSorted), Sorted.ToCheck()), callbackData:"comm:sorted:" + chatId + ":" + pollId),
				});
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.StringsList.optionsAppendable), Appendable.ToCheck()), callbackData:"comm:appendable:" + chatId + ":" + pollId),
				});
			}
			inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
				InlineKeyboardButton.Create(RenderModerationEmoji() + " " + strings.GetString(Strings.StringsList.moderate), callbackData:"comm:moderate:" + chatId + ":" + pollId),
				});
			inline.InlineKeyboard.Add(new List<InlineKeyboardButton>
			{
				InlineKeyboardButton.Create(string.Format(strings.GetString(Strings.StringsList.clone), EmojiStore.Clone), callbackData:"comm:clone:" + this.ChatId + ":" + this.PollId)
			});
			inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
				InlineKeyboardButton.Create("\ud83d\udcbe " + strings.GetString(Strings.StringsList.done), callbackData:"comm:update:" + chatId + ":" + pollId)
			});
			return inline;
		}

		public virtual bool Vote(string apikey, int optionNr, User user, Message message, string inlineMessageId = null) {
			dBHandler.AddToQueue(this);
			return false;
		}

		public void Send(string apikey, Strings strings, long chatId, bool fromChannel = false) {
			FinishCreation();
			ContentParts content = GetContent(strings, apikey, noApproximation: true);
			Api.SendMessageAsync(apikey, chatId, content.Text, replyMarkup: (fromChannel ? content.InlineKeyboard : GenerateUserMarkup(strings, apikey)));
		}

		public void Send(string apikey, Strings strings, long chatId, int pagOffset) {
			ContentParts content = GetContent(strings, apikey, noApproximation: true, offset: pagOffset);
			Api.SendMessageAsync(apikey, chatId, content.Text, replyMarkup: content.InlineKeyboard);
		}

		public void FinishCreation() {
			dBHandler.AddToQueue(this, false);
			this.archived = true;
		}

		public async Task Update(string apikey, Strings strings, long chatId, int messageID, int pagOffset, bool noApproximation) {
			ContentParts content = GetContent(strings, apikey, noApproximation, offset: pagOffset);
			Api.EditMessageTextAsync(apikey, content.Text, content.InlineKeyboard, chatId, messageID);
		}

		public async Task Update(List<Instance> instances, long currentBotChatID, Strings strings, bool noApproximation, string currentInlineMessageID = null, int? messageId = null, string currentText = null, long? newChatId = null, bool voteButtonPressed = false) {
			//If a user pressed the update button, the messageId is not null (so nobody voted)
			bool getsAVote = messageId == null;
			Instance currentInstance = instances.Find(x => x.chatID == currentBotChatID);
			string apikey = currentInstance.apikey;
			//Fully fledged poll
			ContentParts content = GetContent(strings, apikey, noApproximation: noApproximation);
			//Has only link buttons
			ContentParts contentChannel = GetContent(strings, apikey, noApproximation: noApproximation, channel: true);
			//Filters HTML-Tags from the new message to compare with the old message text (to reduce unecessary updates)
			Regex regex = new Regex("<[^>]*>");
			if (currentText == null || regex.Replace(content.Text, "") != regex.Replace(currentText, "") || voteButtonPressed) {
				if (messageId != null) {
					//User voted in private chat and is not the poll owner
					if (newChatId != null && newChatId != this.chatId) {
						Api.EditMessageTextAsync(apikey, strings.GetString(Strings.StringsList.votedSuccessfully), null, newChatId, messageId);
						getsAVote = true;
						//User requests to vote on his own poll
					} else if (voteButtonPressed) {
						Api.EditMessageTextAsync(apikey, content.Text, content.InlineKeyboard, this.chatId, messageId);
						//Poll owner voted in private chat
					} else {
						Api.EditMessageTextAsync(apikey, content.Text, GenerateUserMarkup(strings, apikey), this.chatId, messageId);
						getsAVote = true;
					}
				}
				//Refreshes all messages shared via inline mode
				if (getsAVote) {
					if (currentInlineMessageID != null) {
						MessageID currentMessageID = messageIds.Find(x => x.inlineMessageId == currentInlineMessageID);
						if (currentMessageID.messageIDInvalid) {
							throw new UpdateMessageIDInvalidException();
						} else if (currentMessageID.botChatID == currentBotChatID) {
							ContentParts contentToSend = currentMessageID.channel ? contentChannel : content;
							if (RequestHandler.DoUpdate(instance: currentInstance, messageID: currentMessageID, messageTextLength: content.Text.Length, necessary: true)) { 
									try {
										await Api.EditMessageTextAsync(
										currentInstance.apikey,
										contentToSend.Text,
										contentToSend.InlineKeyboard,
										inlineMessageID: currentMessageID.inlineMessageId
										);
									} catch (WJClubBotFrame.Exceptions.MessageIDInvalid) {
										currentMessageID.messageIDInvalid = true;
									} catch (WJClubBotFrame.Exceptions.TooManyRequests ex) {
										currentInstance.retryAt = DateTime.Now + TimeSpan.FromSeconds(ex.RetryAfter);
									}
								}

						} else {
							throw new UpdateBotsDontMatchException();
						}
					}
					dBHandler.UpdateQueue.Add(new UpdateQueueObject {
						poll = this,
						priorityUpdates = new List<string> { currentInlineMessageID },
						doneUpdates = new List<string>(),
						important = false,
						fromBotID = currentBotChatID,
					});
				}
			}
		}

		public async Task Update(List<Instance> instances, Strings strings, UpdateQueueObject updateQueueObject, bool noApproximation = true) {
			bool allDone = false;
			try {
				Instance currentInstance = instances.Find(x => x.chatID == updateQueueObject.fromBotID);
				ContentParts content = GetContent(strings, currentInstance.apikey, noApproximation: noApproximation);
				ContentParts contentChannel = GetContent(strings, currentInstance.apikey, noApproximation: noApproximation, channel: true);
				foreach (MessageID messageID in messageIds) {
					if (messageID.messageIDInvalid) {
						continue;
					}
					ContentParts contentToSend = messageID.channel ? contentChannel : content;
					Instance currentLoopInstance = currentInstance;
					bool instanceQuestionable = false;
					try {
						currentLoopInstance = instances.Find(x => x.chatID == messageID.botChatID);
					} catch (NullReferenceException) {
						instanceQuestionable = true;
					}
					if (RequestHandler.DoUpdate(instance: currentLoopInstance, messageID: messageID, messageTextLength: content.Text.Length, necessary: updateQueueObject.important)) {
						try {
							await Api.EditMessageTextAsync(
								currentLoopInstance.apikey,
								contentToSend.Text,
								contentToSend.InlineKeyboard,
								inlineMessageID: messageID.inlineMessageId
								);
							messageID.Last30Updates.Add(DateTime.Now);
							updateQueueObject.doneUpdates.Add(messageID.inlineMessageId);
							if (instanceQuestionable) {
								messageID.botChatID = currentLoopInstance.chatID;
							}
							//Thrown when the message was deleted
						} catch (WJClubBotFrame.Exceptions.MessageIDInvalid) {
							messageID.messageIDInvalid = true;
						} catch (WJClubBotFrame.Exceptions.TooManyRequests ex) {
							currentLoopInstance.retryAt = DateTime.Now + TimeSpan.FromSeconds(ex.RetryAfter);
						}
					} else {
						allDone = false;
						continue;
					}
				}
			} finally {
				if (allDone) {
					dBHandler.UpdateQueue.Remove(updateQueueObject);
				}
			}
		}

		internal void UpdateWithOptionsPane(string apikey, Strings strings, int messageID, string text) {
			ContentParts content = GetContent(strings, apikey, noApproximation:true);
			Api.EditMessageTextAsync(apikey, "<b>" + HtmlSpecialChars.Encode(strings.GetString(Strings.StringsList.optionsForPoll)) + "</b>\n\n" + content.Text, this.GenerateOptionsMarkup(strings), chatId, messageID);
		}

		internal void UpdateWithModeratePane(string apikey, Strings strings, int messageId, string text) {
			ContentParts content = GetContent(strings, apikey, noApproximation:true,  moderatePane:true);
			Api.EditMessageTextAsync(apikey, content.Text, content.InlineKeyboard, chatId, messageId);
		}

		public InlineQueryResultArticle Result(Strings strings, string apikey, bool channel) {
			if (closed)
				return null;
			ContentParts content = GetContent(strings, apikey, noApproximation:true /*TODO Request handler here*/, channel:channel);
			return InlineQueryResultArticle.Create(chatId + ":" + pollId, content.InlineTitle, InputTextMessageContent.Create(content.Text, disableWebPagePreview: true), content.InlineKeyboard, description: content.InlineDescription, thumbUrl:"https://wjclub.capella.uberspace.de/groupagreebot/res/" + pollType.ToString() + "_" + anony.ToString() + ".png", thumbWidth:256, thumbHeight:256);
		}

		public virtual MySqlCommand GenerateCommand(MySqlConnection connection, long currentBotChatID, Strings strings, List<Instance> instances, bool forceNoApproximation, bool change = true) {
            MySqlCommand command = new MySqlCommand
            {
                Connection = connection
            };
            if (delete) {
				command.CommandText = "DELETE FROM `polls` WHERE `chatid`=?chatid and`pollid`=?pollid;";
				command.Parameters.AddWithValue("?chatid", chatId);
				command.Parameters.AddWithValue("?pollid", pollId);
			} else {
				command.CommandText = "REPLACE INTO polls (chatid, pollid, pollText, pollDescription, pollVotes, messageIds, anony, closed, percentageBar, appendable, sorted, pollType, archived, lang) VALUES (?chatid, ?pollid, ?pollText, ?pollDescription, ?pollVotes, ?messageIds, ?anony, ?closed, ?percentageBar, ?appendable, ?sorted, ?pollType, ?archived, ?lang);";
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
				command.Parameters.AddWithValue("?archived", archived);
				command.Parameters.AddWithValue("?pollType", pollType);
				command.Parameters.AddWithValue("?lang", this.Lang);
				if (change) {
					//TODO catch stuff
					Update(instances, currentBotChatID, strings, forceNoApproximation).Wait();
				}
			}
			return command;
		}
    }

	[Serializable]
	internal class UpdateBotsDontMatchException:Exception {
		public UpdateBotsDontMatchException() {
		}

		public UpdateBotsDontMatchException(string message) : base(message) {
		}

		public UpdateBotsDontMatchException(string message, Exception innerException) : base(message, innerException) {
		}

		protected UpdateBotsDontMatchException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}

	public class ContentParts {
		public ContentParts(string text, InlineKeyboardMarkup inlineKeyboard, string inlineTitle, string inlineDescription) {
			this.Text = text;
			this.InlineKeyboard = inlineKeyboard;
			this.InlineTitle = inlineTitle;
            this.InlineDescription = inlineDescription;
		}
		public string Text;
		public InlineKeyboardMarkup InlineKeyboard;
		public string InlineTitle;
		public string InlineDescription;
	}
}