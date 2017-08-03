using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using System.Globalization;
namespace telegrambotgroupagree {
	public class PollContainer {
		public PollContainer (DBHandler dBHandler, Strings strings) {
			this.dBHandler = dBHandler;
			this.strings = strings;
			this.pollDB = dBHandler.GetPolls();
		}

		private List<Poll> pollDB;
		private DBHandler dBHandler;
		private Strings strings;

		public override string ToString() {
			return @JsonConvert.SerializeObject (pollDB, new JsonSerializerSettings {
				MaxDepth = 5
			});
		}

		public Poll Add(Pointer pointer, int chatId, string pollText) {
			Poll poll;
			switch (pointer.PollType) {
				case EPolls.vote:
					try {
						poll = new PVote(chatId, pointer.LastPollId, pollText, pointer.Anony, dBHandler, pointer.Lang);
					} catch (NullReferenceException) {
						poll = new PVote(chatId, 0, pollText, pointer.Anony, dBHandler, pointer.Lang);
					}
					break;
				case EPolls.doodle: 
					try {
						poll = new Doodle(chatId, pointer.LastPollId, pollText, pointer.Anony, dBHandler, pointer.Lang);
					} catch (NullReferenceException) {
						poll = new Doodle(chatId, 0, pollText, pointer.Anony, dBHandler, pointer.Lang);
					}
					break;
				case EPolls.board:
					try {
						poll = new Board(chatId, pointer.LastPollId, pollText, pointer.Anony, dBHandler, pointer.Lang);
					} catch (NullReferenceException) {
						poll = new Board(chatId, 0, pollText, pointer.Anony, dBHandler, pointer.Lang);
					}
					break;
				case EPolls.limitedDoodle:
					try {
						poll = new LimitedDoodle(chatId, pointer.LastPollId, pollText, pointer.Anony, dBHandler, pointer.Lang);
					} catch (NullReferenceException) {
						poll = new LimitedDoodle(chatId, 0, pollText, pointer.Anony, dBHandler, pointer.Lang);
					}
					break;
				default:
					try {
						poll = new PVote(chatId, pointer.LastPollId, pollText, pointer.Anony, dBHandler, pointer.Lang);
					} catch (NullReferenceException) {
						poll = new PVote(chatId, 0, pollText, pointer.Anony, dBHandler, pointer.Lang);
					}
					break;
			}
			pollDB.Add(poll);
			return poll;
		}

		public List<Poll> GetPolls(int chatId, string searchFor = null) {
				return dBHandler.GetPolls(chatId, searchFor: searchFor);
		}

		public List<Poll> GetPollsReverse(int chatId, int limit, string searchFor = null) {
			List<Poll> relevantPolls = GetPolls(chatId, searchFor);
			List<Poll> resultPolls = new List<Poll>();
			int count = relevantPolls.Count;
			for (int i = count - 1; i >= count - limit && i >= 0; i--) {
				resultPolls.Add(relevantPolls[i]);
			}
			return resultPolls;
		}

		public string GetPollPretty(int chatId) {
			string text = strings.GetString(Strings.stringsList.listYourPolls);
			List<Poll> polls = GetPolls(chatId);
			if (polls.Count == 0) {
				text = strings.GetString(Strings.stringsList.listNothingHere);
			} else {
				polls.ForEach(x => {
					text += "\n<b>" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strings.GetString((Strings.stringsList)Enum.Parse(typeof(Strings.stringsList), x.PollType.ToString()))) + "</b> " + HtmlSpecialChars.Encode(x.PollText.Replace("\u200F", "").Replace("\u202B", "").Replace("\u202E", "").Truncate(25)) + "\n-> /" + x.PollId + "\n";
				});
				text += strings.GetString(Strings.stringsList.deleteAllFooter);
			}
			return text;
		}

		public Poll GetPoll(int chatId, int pollId) {
			Poll poll;
			poll = pollDB.Find(x => (x.ChatId == chatId && x.PollId == pollId));
			if (poll == null) {
				return dBHandler.GetPolls(chatId, pollId:pollId)[0];
			}
			return poll;
		}

		public Poll GetLastPoll(Pointer pointer) {
			Poll poll = pollDB.Find(x => (x.ChatId == pointer.ChatId && x.PollId == pointer.LastPollId));
			if (poll == null) {
				try {
					poll = dBHandler.GetPolls(chatId: pointer.ChatId, pollId: pointer.LastPollId)[0];
				} catch (Exception e) {
					WJClubBotFrame.Notifications.log(e.ToString());
					poll = null;
				}
			}
			return poll;
		}

		public void RemoveLastPoll(int chatId) {
			pollDB.RemoveAt(pollDB.FindLastIndex(x => x.ChatId == chatId));
		}

		public void SetPoll(Poll poll) {
			dBHandler.AddToQueue(poll);
			pollDB[pollDB.FindIndex(x => (x.ChatId == poll.ChatId && x.PollId == poll.PollId))] = poll;
		}
	}
}

