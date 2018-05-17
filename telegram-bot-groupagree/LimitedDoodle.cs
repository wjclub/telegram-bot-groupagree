using System;
using System.Collections.Generic;
using WJClubBotFrame.Types;
using WJClubBotFrame.Methods;
using WJClubBotFrame;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StringEdit = System.Globalization.CultureInfo;

namespace telegrambotgroupagree {
	public class LimitedDoodle : Doodle {
		#region constructors
		public LimitedDoodle(int chatId, int pollId, string pollText, EAnony anony, DBHandler dBHandler, Strings.Langs lang) : base(chatId, pollId, pollText, anony, dBHandler, lang) {
			this.pollType = EPolls.limitedDoodle;
		}
		public LimitedDoodle(int chatId, int pollId, string pollText, string pollDescription, EAnony anony, bool closed, PercentageBars.Bars percentageBar, bool appendable, bool sorted, bool archived, Dictionary<string, List<User>> pollVotes, List<MessageID> messageIds, List<User> people, int maxVotes, DBHandler dBHandler, Strings.Langs lang) : base(chatId, pollId, pollText, pollDescription, anony, closed, percentageBar, appendable, sorted, archived, pollVotes, messageIds, people, dBHandler, lang) {
			this.pollType = EPolls.limitedDoodle;
			this.MaxVotes = maxVotes;
		}
		#endregion

		public int MaxVotes;

		public bool Vote(string apikey, int optionNr, User user, Message message, out bool tooMuchVotes, string inlineMessageId = null) {
			dBHandler.AddToQueue(this);
			tooMuchVotes = false;
			bool result;
			if (pollVotes.ElementAt(optionNr).Value.Exists(x => x.Id == user.Id)) {
				pollVotes.ElementAt(optionNr).Value.RemoveAt(pollVotes.ElementAt(optionNr).Value.FindIndex(x => x.Id == user.Id));
				bool notThere = true;
				foreach (KeyValuePair<string, List<User>> x in pollVotes) {
					if (x.Value.Exists(y => y.Id == user.Id))
						notThere = false;
				}
				if (notThere)
					people.RemoveAll(z => z.Id == user.Id);
				result = false;
			} else {
				int votedCount = 0;
				foreach (KeyValuePair<string, List<User>> x in pollVotes) {
					if (x.Value.Exists(y => y.Id == user.Id))
						votedCount++;
				}
				if (votedCount < MaxVotes) {
					pollVotes.ElementAt(optionNr).Value.Add(user);
					people.RemoveAll(x => x.Id == user.Id);
					people.Add(user);
					result = true;
				} else {
					tooMuchVotes = true;
					result = false;
				}
			}
			return result;
		}

		public override bool Vote(string apikey, int optionNr, User user, Message message, string inlineMessageId = null) {
			throw new NotImplementedException("Buddy\nPlease just use the other method provided");
		}

		public override string RenderPollConfig(Strings strings) {
			if (Anony == EAnony.personal)
				return strings.GetString(Strings.StringsList.inlineDescriptionPersonalLimitedDoodle);
			return strings.GetString(Strings.StringsList.inlineDescriptionAnonymousLimitedDoodle);
		}

		public override string RenderOptionalInsert(Strings strings) {
			return "ℹ️ " + string.Format(strings.GetString(Strings.StringsList.limitedDoodleYouCanChooseSoMany), MaxVotes, pollVotes.Count) + "\n";
		}

		public override MySqlCommand GenerateCommand(MySqlConnection connection, long currentBotChatID, Strings strings, List<Instance> instances, bool change = true) {
			MySqlCommand command = new MySqlCommand();
			command.Connection = connection;
			if (delete) {
				command.CommandText = "DELETE FROM `polls` WHERE `chatid`=?chatid and`pollid`=?pollid;";
				command.Parameters.AddWithValue("?chatid", chatId);
				command.Parameters.AddWithValue("?pollid", pollId);
			} else {
				command.CommandText = "REPLACE INTO polls (chatid, pollid, pollText, pollDescription, pollVotes, messageIds, anony, closed, percentageBar, appendable, sorted, pollType, people, archived, lang, maxVotes) VALUES (?chatid, ?pollid, ?pollText, ?pollDescription, ?pollVotes, ?messageIds, ?anony, ?closed, ?percentageBar, ?appendable, ?sorted, ?pollType, ?people, ?archived, ?lang, ?maxVotes);";
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
				command.Parameters.AddWithValue("?maxVotes", MaxVotes);
				if (change)
					Update(instances, currentBotChatID, strings);
			}
			return command;
		}
	}
}

