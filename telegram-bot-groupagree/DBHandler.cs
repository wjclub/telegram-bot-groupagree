using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json;
using WJClubBotFrame;
using WJClubBotFrame.Types;

namespace telegrambotgroupagree {
	public class DBHandler {
		public DBHandler(string dbName, string dbUser, string dbPassword) {
			pollQueue = new List<QueueObject>();
			pointerQueue = new List<Pointer>();
			MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder {
				Server = "127.0.0.1", //for compatability reasons... this forces TCP for MariaDB
				UserID = dbUser,
				Password = dbPassword,
				Database = dbName,
				Port = 3306,
				CharacterSet = "utf8mb4"
			};
			connection = new MySqlConnection(connectionStringBuilder.ToString());
		}

		private MySqlConnection connection;

		private List<QueueObject> pollQueue;

		public List<QueueObject> PollQueue { get { return this.pollQueue; } }

		private List<Pointer> pointerQueue;

		public List<Pointer> PointerQueue { get { return this.pointerQueue; } }

		public List<Instance> GetInstances() {
			connection.Open();
			MySqlCommand command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM instances;";
			List<Instance> instance = new List<Instance>();
			MySqlDataReader reader = command.ExecuteReader();
			while (reader.Read()) {
				instance.Add(new Instance {
					apikey = reader["chat_id"] + ":" + reader["key"],
					offset = int.Parse(reader["offset"].ToString()),
					creator = JsonConvert.DeserializeObject<User>(reader["owner"].ToString()),
					botUser = null,
					Last30Updates = JsonConvert.DeserializeObject<List<DateTime>>(reader["last_30_updates"].ToString()),
					retryAt = null,
					//retryAt = reader["retry_at"] != null ? new MySqlDateTime(reader["retry_at"].ToString()).GetDateTime() : (DateTime?) null,
				});
			}
			connection.Close();
			return instance;
		}

		public override string ToString() {
			return JsonConvert.SerializeObject(this, new JsonSerializerSettings {
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			});
		}

		public void AddToQueue(Poll poll, bool change = true, bool forceNoApproximation = false) {
			pollQueue.RemoveAll(x => (x.Poll.ChatId == poll.ChatId && x.Poll.PollId == poll.PollId));
			pollQueue.Add(new QueueObject {
				Poll = poll,
				UpdateMessages = change,
				ForceNoApproximation = forceNoApproximation,
			});
		}

		public List<Poll> GetPolls(long? chatId = null, int? pollId = null, string searchFor = null, bool reverse = false, int limit = 50) {
			connection.Open();
			List<Poll> allPolls = new List<Poll>();
			MySqlCommand command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM polls WHERE";
			if (chatId != null) {
				command.CommandText += " chatid = '" + chatId + "'";
				if (pollId != null) {
					command.CommandText += " AND pollid = '" + pollId + "'";
				} else if (searchFor != null && searchFor != "") {
					searchFor = "%" + searchFor + "%";
					command.CommandText += " AND pollText LIKE ?searchFor";
					command.Parameters.AddWithValue("?searchFor", searchFor);
				}
				command.CommandText += " AND archived = 1";
			} else {
				command.CommandText += " archived = 0";
			}
			if (reverse)
				command.CommandText += " ORDER BY pollid DESC LIMIT " + limit;
			command.CommandText += ";";
			try {
				MySqlDataReader reader;
				reader = command.ExecuteReader();
				while (reader.Read()) {
					switch ((EPolls)Enum.Parse(typeof(EPolls), reader["pollType"].ToString())) {
						case EPolls.vote:
							allPolls.Add(new PVote(int.Parse(reader["chatid"].ToString()), int.Parse(reader["pollid"].ToString()), reader["pollText"].ToString(), reader["pollDescription"].ToString(), (EAnony)Enum.Parse(typeof(EAnony), reader["anony"].ToString()), (reader["closed"].ToString() == "1" || false), (PercentageBars.Bars)Enum.Parse(typeof(PercentageBars.Bars), reader["percentageBar"].ToString()), (reader["appendable"].ToString() == "1" || false), (reader["sorted"].ToString() == "1" || false), (reader["archived"].ToString() == "1" || false), JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(reader["pollVotes"].ToString()), JsonConvert.DeserializeObject<List<MessageID>>(reader["messageIds"].ToString()), this, (Strings.Langs)Enum.Parse(typeof(Strings.Langs), reader["lang"].ToString())));
							break;
						case EPolls.doodle:
							allPolls.Add(new Doodle(int.Parse(reader["chatid"].ToString()), int.Parse(reader["pollid"].ToString()), reader["pollText"].ToString(), reader["pollDescription"].ToString(), (EAnony)Enum.Parse(typeof(EAnony), reader["anony"].ToString()), (reader["closed"].ToString() == "1" || false), (PercentageBars.Bars)Enum.Parse(typeof(PercentageBars.Bars), reader["percentageBar"].ToString()), (reader["appendable"].ToString() == "1" || false), (reader["sorted"].ToString() == "1" || false), (reader["archived"].ToString() == "1" || false), JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(reader["pollVotes"].ToString()), JsonConvert.DeserializeObject<List<MessageID>>(reader["messageIds"].ToString()), JsonConvert.DeserializeObject<List<User>>(reader["people"].ToString()), this, (Strings.Langs)Enum.Parse(typeof(Strings.Langs), reader["lang"].ToString())));
							break;
						case EPolls.board:
							allPolls.Add(new Board(int.Parse(reader["chatid"].ToString()), int.Parse(reader["pollid"].ToString()), reader["pollText"].ToString(), reader["pollDescription"].ToString(), (EAnony)Enum.Parse(typeof(EAnony), reader["anony"].ToString()), (reader["closed"].ToString() == "1" || false), (reader["archived"].ToString() == "1" || false), this, JsonConvert.DeserializeObject<Dictionary<int, BoardVote>>(reader["pollVotes"].ToString()), JsonConvert.DeserializeObject<List<MessageID>>(reader["messageIds"].ToString()), (Strings.Langs)Enum.Parse(typeof(Strings.Langs), reader["lang"].ToString())));
							break;
						case EPolls.limitedDoodle:
							allPolls.Add(new LimitedDoodle(int.Parse(reader["chatid"].ToString()), int.Parse(reader["pollid"].ToString()), reader["pollText"].ToString(), reader["pollDescription"].ToString(), (EAnony)Enum.Parse(typeof(EAnony), reader["anony"].ToString()), (reader["closed"].ToString() == "1" || false), (PercentageBars.Bars)Enum.Parse(typeof(PercentageBars.Bars), reader["percentageBar"].ToString()), (reader["appendable"].ToString() == "1" || false), (reader["sorted"].ToString() == "1" || false), (reader["archived"].ToString() == "1" || false), JsonConvert.DeserializeObject<Dictionary<string, List<User>>>(reader["pollVotes"].ToString()), JsonConvert.DeserializeObject<List<MessageID>>(reader["messageIds"].ToString()), JsonConvert.DeserializeObject<List<User>>(reader["people"].ToString()), int.Parse(reader["maxVotes"].ToString()), this,  (Strings.Langs)Enum.Parse(typeof(Strings.Langs),reader["lang"].ToString())));
							break;
					}
				}
			} catch (Exception e) {
				Notifications.log(e.ToString());
			} finally {
				connection.Close();
			}
			return allPolls;
		}

		public void UpdateInstance(long chatID, int offset, List<DateTime> last30Updates, DateTime? retryAt) {
			connection.Open();
			MySqlCommand command = connection.CreateCommand();
			command.CommandText = $"UPDATE instances SET offset = ?offset, last_30_updates = ?last_30_updates, retry_at = ?retry_at WHERE chat_id = ?chat_id;";
			command.Parameters.AddWithValue("?chat_id", chatID);
			command.Parameters.AddWithValue("?offset", offset);
			command.Parameters.AddWithValue("?last_30_updates", JsonConvert.SerializeObject(last30Updates));
			command.Parameters.AddWithValue("?retry_at", retryAt);
			command.ExecuteNonQuery();
			connection.Close();
		}

		public void AddInstance(long chatID, string key, User owner) {
			connection.Open();
			MySqlCommand command = connection.CreateCommand();
			command.CommandText = "INSERT INTO instances (`chat_id`, `key`, `owner`, `creation_date`) VALUES (?chat_id, ?key, ?owner, CURRENT_TIMESTAMP());";
			command.Parameters.AddWithValue("?chat_id", chatID);
			command.Parameters.AddWithValue("?key", key);
			command.Parameters.AddWithValue("?owner", JsonConvert.SerializeObject(owner));
			command.ExecuteNonQuery();
			connection.Close();
		}

		public void AddToQueue(Pointer pointer) {
			try {
				pointerQueue.RemoveAll(x => x.ChatId == pointer.ChatId);
			} catch (NullReferenceException e) {
				Notifications.log("##_DBHANDLER_ADD_TO_QUEUE_##\n" + e.ToString());
			}
			pointerQueue.Add(pointer);
		}

		public Pointer GetPointer(int chatID) {
			connection.Open();
			MySqlCommand command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM pointer WHERE chatId = " + chatID;
			Pointer pointer = null;
			try {
				MySqlDataReader reader;
				reader = command.ExecuteReader();
				if (reader.Read())
					pointer = ParsePointer(reader);
				return pointer;
			} catch (Exception e) {
				Notifications.log(e.ToString());
				throw new Exception("ChatID " + chatID + " has screwed up the pointer...");
			} finally {
				connection.Close();
			}
		}

		private Pointer ParsePointer(MySqlDataReader reader) {
			int? boardChatId = null, boardPollId = null;
			boardChatId = int.TryParse(reader["boardChatId"].ToString(), out int temp) ? (int?)temp : null;
			boardPollId = int.TryParse(reader["boardPollId"].ToString(), out temp) ? (int?)temp : null;
			return new Pointer(int.Parse(reader["chatId"].ToString()), (EPolls)Enum.Parse(typeof(EPolls), reader["pollType"].ToString()), (ENeedle)Enum.Parse(typeof(ENeedle), reader["needle"].ToString()), (EAnony)Enum.Parse(typeof(EAnony), reader["anony"].ToString()), boardChatId, boardPollId, int.Parse(reader["lastPollId"].ToString()), (Strings.Langs)Enum.Parse(typeof(Strings.Langs), reader["lang"].ToString()), new List<DateTime>() /*TODO Get LastRequests from DB*/);
		}

		public void FlushToDB(Strings strings, List<Instance> instances, long currentBotChatID) {
			connection.Open(); //TODO Thinking... implement request handler here probably?
			pollQueue.ForEach(x => x.Poll.GenerateCommand(connection, currentBotChatID, strings, instances, forceNoApproximation:x.ForceNoApproximation, change:x.UpdateMessages).ExecuteNonQuery());
			pollQueue.Clear();
			try {
				pointerQueue.ForEach(x => x.GenerateCommand(connection).ExecuteNonQuery());
				pointerQueue.Clear();
			} catch (Exception e) {
				Notifications.log(e.ToString());
			}
			connection.Close();
		}

		public async Task UpdatePollsFromQueue(Strings strings, List<Instance> instances) {
			List<QueueObject> updateQueue = pollQueue.FindAll(x => x.UpdateMessages);
			if (updateQueue != null && updateQueue.Count > 0) {
				//Update important stuff first
				foreach (QueueObject obj in updateQueue) {
					if (obj.PriorityUpdates != null && obj.PriorityUpdates.Count > 0) {
						await obj.Poll.Update(instances, strings, obj);
					}
				}
				foreach (QueueObject obj in updateQueue) {
					if (obj.PriorityUpdates == null || obj.PriorityUpdates.Count == 0) {
						await obj.Poll.Update(instances, strings, obj);
					}
				}
			}
		}
	}

	public class QueueObject {
		public Poll Poll;
		public bool UpdateMessages;
		public bool ForceNoApproximation;
		public List<string> PriorityUpdates;
		public List<string> DoneUpdates;
		public long FromBotID;
	}
}