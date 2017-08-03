using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using WJClubBotFrame;
using System.Text.RegularExpressions;
using System;

namespace telegrambotgroupagree {
	public abstract class Poll {
		protected Poll(int chatId, int pollId, string pollText, string pollDescription, EAnony anony, bool closed, PercentageBars.Bars percentageBar, bool appendable, bool sorted, bool archived, DBHandler dBHandler, Dictionary<string, List<User>> pollVotes, List<MessageID> messageIds, Strings.langs lang, EPolls pollType = EPolls.vote) {
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
		protected Strings.langs lang;
		public Strings.langs Lang { get { return this.lang; } }
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

		public virtual void Close(string apikey, Strings strings, int messageId) {
			closed = true;
			dBHandler.AddToQueue(this);
			Update(apikey, strings, messageId);
		}

		public virtual void SetPercentage(PercentageBars.Bars bar) {
			this.PercentageBar = bar;
			dBHandler.AddToQueue(this);
		}

		public virtual void SetSorted(bool sorted) {
			this.Sorted = sorted;
			dBHandler.AddToQueue(this);
		}

		public virtual void SetAppendable(bool appendable) {
			this.Appendable = appendable;
			dBHandler.AddToQueue(this);
		}

		public virtual void Reopen(string apikey, Strings strings, int messageId) {
			closed = false;
			dBHandler.AddToQueue(this);
			Update(apikey, strings, messageId);
		}

		public virtual void Delete(string apikey, Strings strings, int messageId) {
			delete = true;
			dBHandler.AddToQueue(this, false);
			Update(apikey, strings, messageId);
		}

		public virtual void DeleteFromDeleteAll(string apikey, Strings strings) {
			delete = true;
			dBHandler.AddToQueue(this, false);
		}

		protected virtual ContentParts GetContent(Strings strings, string apikey, bool channel = false, int? offset = null, bool moderatePane = true) {
			string text = RenderTitle();
			//TODO: Make this less messy
			throw new NotImplementedException("Sorry dude");
			return new ContentParts(text, null, null);
		}

		public virtual string RenderTitle() {
			return this.PollText;
		}

		public void AddToMessageIDs(MessageID messageID) {
			messageIds.Add(messageID);
			dBHandler.AddToQueue(this, false);
		}

		protected virtual InlineKeyboardMarkup GenerateUserMarkup(Strings strings, string apikey) {
			string botname = Globals.Botname;
			if (delete) {
				return null;
			}
			InlineKeyboardMarkup inline = new InlineKeyboardMarkup {
				InlineKeyboard = new List<List<InlineKeyboardButton>>()
			};
			if (!closed) {
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.publish), switchInlineQuery: pollText) });
				if (pollType != EPolls.board) {
					inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.publishWithLink), switchInlineQuery: "$c:" + pollText) });
					inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.buttonVote), callbackData: "comm:iwannavote:" + Cryptography.Encrypt(chatId+ ":" + pollId, apikey)),
						                                                       InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.commPageRefresh), callbackData:"comm:update:" + chatId + ":" + pollId)});
				} else {
					inline.InlineKeyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.buttonVote), url: "https://telegram.me/" + Globals.Botname + "bot?start=" + Cryptography.Encrypt("board:" + chatId + ":" + pollId, apikey)),
																			InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.commPageRefresh), callbackData:"comm:update:" + chatId + ":" + pollId)});
				}
			}
			inline.InlineKeyboard.Add(new List<InlineKeyboardButton>() {
				InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.commPageOptions), callbackData:"comm:options:" + chatId + ":" + pollId),
				InlineKeyboardButton.Create((closed ? strings.GetString(Strings.stringsList.commPageReopen) : strings.GetString(Strings.stringsList.commPageClose)), callbackData:"comm:" + (closed ? "reopen:" : "close:") + chatId + ":" + pollId),
                InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.commPageDelete), callbackData:"comm:delete:" + chatId + ":" + pollId),
			});
			return inline;
		}

		ReplyMarkup GenerateOptionsMarkup(Strings strings) {
			InlineKeyboardMarkup inline = new InlineKeyboardMarkup {
				InlineKeyboard = new List<List<InlineKeyboardButton>>()
			};
			bool moderate = Appendable;
			if (this.pollType != EPolls.board) {
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.optionsPercentageNone), (PercentageBar == PercentageBars.Bars.none).ToCheck()), callbackData:"comm:percentage:none:" + chatId + ":" + pollId),
					InlineKeyboardButton.Create(PercentageBars.GetIconArray(PercentageBars.Bars.dots).Implode() + " " + (PercentageBar == PercentageBars.Bars.dots).ToCheck(), callbackData:"comm:percentage:dots:" + chatId + ":" + pollId),
					InlineKeyboardButton.Create(PercentageBars.GetIconArray(PercentageBars.Bars.thumbs).Implode() + " " + (PercentageBar == PercentageBars.Bars.thumbs).ToCheck(), callbackData:"comm:percentage:thumbs:" + chatId + ":" + pollId),
				});
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.optionsSorted), Sorted.ToCheck()), callbackData:"comm:sorted:" + chatId + ":" + pollId),
				});
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.optionsAppendable), Appendable.ToCheck()), callbackData:"comm:appendable:" + chatId + ":" + pollId),
				});
			} else {
				moderate = true;
			}
			if (moderate) {
				inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
					InlineKeyboardButton.Create("\ud83d\udd0d " + strings.GetString(Strings.stringsList.moderate), callbackData:"comm:moderate:" + chatId + ":" + pollId),
				});
			}
			inline.InlineKeyboard.Add(new List<InlineKeyboardButton> {
				InlineKeyboardButton.Create("\ud83d\udcbe " + strings.GetString(Strings.stringsList.done), callbackData:"comm:update:" + chatId + ":" + pollId)
			});
			return inline;
		}

		public virtual bool Vote(string apikey, int optionNr, User user, Message message, string inlineMessageId = null) {
			dBHandler.AddToQueue(this);
			return false;
		}

		public void Send(string apikey, Strings strings, long chatId, bool fromChannel = false) {
			dBHandler.AddToQueue(this, false);
			this.archived = true;
			ContentParts content = GetContent(strings, apikey);
			Api.SendMessage(apikey, chatId, content.Text, replyMarkup: (fromChannel ? content.InlineKeyboard : GenerateUserMarkup(strings,apikey)));
		}

		public void Send(string apikey, Strings strings, long chatId, int pagOffset) {
			ContentParts content = GetContent(strings, apikey, offset: pagOffset);
			Api.SendMessage(apikey, chatId, content.Text, replyMarkup: content.InlineKeyboard);
		}

		public void Update(string apikey, Strings strings, long chatId, int messageID, int pagOffset) {
			ContentParts content = GetContent(strings, apikey, offset: pagOffset);
			Api.EditMessageText(apikey, content.Text, content.InlineKeyboard, chatId, messageID);
		}

		public void Update(string apikey, Strings strings, int? messageId = null, string currentText = null, long? newChatId = null, bool vote = false) {
			bool getsAVote = messageId == null;
			ContentParts content = GetContent(strings, apikey);
			ContentParts contentChannel = GetContent(strings, apikey, true);
			Regex regex = new Regex("<[^>]*>");
			if (currentText == null || regex.Replace(content.Text, "") != regex.Replace(currentText, "") || vote) {
				if (messageId != null) {
					if (newChatId != null && newChatId != chatId) {
						Api.EditMessageText(apikey, strings.GetString(Strings.stringsList.votedSuccessfully), null, newChatId, messageId);
						getsAVote = true;
					} else if (vote) {
						Api.EditMessageText(apikey, content.Text, content.InlineKeyboard, chatId, messageId);
					} else {
						Api.EditMessageText(apikey, content.Text, GenerateUserMarkup(strings, apikey), chatId, messageId);
						getsAVote = true;
					}
				}
				if (getsAVote) {
					foreach (MessageID messageID in messageIds) {
						if (messageID.channel)
							Api.EditMessageText(apikey, contentChannel.Text, contentChannel.InlineKeyboard, inlineMessageId: messageID.inlineMessageId);
						else
							Api.EditMessageText(apikey, content.Text, content.InlineKeyboard, inlineMessageId: messageID.inlineMessageId);
					}
				}
			}
		}

		internal void UpdateWithOptionsPane(string apikey, Strings strings, int messageID, string text) {
			ContentParts content = GetContent(strings, apikey);
			Api.EditMessageText(apikey, "<b>" + HtmlSpecialChars.Encode(strings.GetString(Strings.stringsList.optionsForPoll)) + "</b>\n\n" + content.Text, this.GenerateOptionsMarkup(strings), chatId, messageID);
		}

		internal void UpdateWithModeratePane(string apikey, Strings strings, int messageId, string text) {
			ContentParts content = GetContent(strings, apikey, moderatePane:true);
		}

		public InlineQueryResultArticle Result(Strings strings, string apikey, bool channel) {
			if (closed)
				return null;
			ContentParts content = GetContent(strings, apikey, channel);
			return InlineQueryResultArticle.Create(chatId + ":" + pollId, pollText, InputTextMessageContent.Create(content.Text, disableWebPagePreview: true), content.InlineKeyboard, description: content.Description, thumbUrl:"https://bots.wjclub.tk/groupagreebot/res/" + pollType.ToString() + "_" + anony.ToString() + ".png", thumbWidth:256, thumbHeight:256);
		}

		public virtual MySqlCommand GenerateCommand(MySqlConnection connection, string apikey, Strings strings, bool change = true) {
			MySqlCommand command = new MySqlCommand();
			command.Connection = connection;
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
				if (change)
					Update(apikey, strings);
			}
			return command;
		}

}

	public class ContentParts {
		public ContentParts(string text, InlineKeyboardMarkup inlineKeyboard, string description) {
			this.Text = text;
			this.InlineKeyboard = inlineKeyboard;
			this.Description = description;
		}
		public string Text;
		public InlineKeyboardMarkup InlineKeyboard;
		public string Description;
	}
}