using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace telegrambotgroupagree {
	public enum ENeedle : short {
		nothing, pollText, pollDescription, firstOption, furtherOptions, closePoll, archivePoll, deletePoll, board, boardVote, messageAll, banned,
		limitedDoodleMaxVotes,
		addOption,
		addInstanceToken
	}

	public enum EAnony {
		personal, anonymous
	}

	public enum EPolls {
		vote = 0,
		doodle = 1,
		board = 2,
		limitedDoodle = 3
	}

	public class Pointer {
		public Pointer(int chatId, string ieftLang) : this(chatId, EPolls.vote, ENeedle.pollText, EAnony.personal, null, null, 0, Strings.GetLangFromIEFT(ieftLang), new List<DateTime>()) { }

		public Pointer(int chatId, EPolls pollType, ENeedle needle, EAnony anony, int? boardChatId, int? boardPollId, int lastPollId, Strings.Langs lang, List<DateTime> lastRequests) {
			this.ChatId = chatId;
			this.PollType = pollType;
			this.Needle = needle;
			this.Anony = anony;
			this.BoardChatId = boardChatId;
			this.BoardPollId = boardPollId;
			this.LastPollId = lastPollId;
			this.Lang = lang;
            this.LastRequests = lastRequests;
		}

		public int ChatId;
		public EPolls PollType;
		public ENeedle Needle;
		public EAnony Anony;
		public int? BoardChatId;
		public int? BoardPollId;
		public int LastPollId;
		public Strings.Langs Lang;
        public List<DateTime> LastRequests;

		public MySqlCommand GenerateCommand(MySqlConnection connection) {
			MySqlCommand command = new MySqlCommand();
			command.Connection = connection;
			command.CommandText = "REPLACE INTO pointer (chatId, needle, anony, pollType, boardChatId, boardPollId, lastPollId, lang) VALUES (?chatId, ?needle, ?anony, ?pollType, ?boardChatId, ?boardPollId, ?lastPollId, ?lang);";
			command.Parameters.AddWithValue("?chatId", ChatId);
			command.Parameters.AddWithValue("?needle", Needle);
			command.Parameters.AddWithValue("?anony", Anony);
			command.Parameters.AddWithValue("?pollType", PollType);
			command.Parameters.AddWithValue("?boardChatId", BoardChatId);
			command.Parameters.AddWithValue("?boardPollId", BoardPollId);
			command.Parameters.AddWithValue("?lastPollId", LastPollId);
			command.Parameters.AddWithValue("?lang", Lang);
			return command;
		}
	}
}