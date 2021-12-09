using System;
using MySql.Data.MySqlClient;

namespace telegrambotgroupagree {
	public class Botstats {
		public Botstats(string apikey) {
			MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
			this.apikey = apikey;
			string[] botsplit = apikey.Split(':');
			conn_string.Server = "localhost";
			conn_string.UserID = botsplit[0];
			chat_id = long.Parse(botsplit[0]);
			conn_string.Password = botsplit[1];
			conn_string.Database = "botstats";
			conn_string.Port = 3306;
			conn_string.CharacterSet = "utf8mb4";
			connection = new MySqlConnection(conn_string.ToString());
		}

		private long chat_id;
		private string apikey;
		private MySqlConnection connection;

		public void increase() {
			connection.Open();
			var command = connection.CreateCommand();
			command.CommandText = "INSERT INTO stats (chat_id,username,count) VALUES (" + chat_id + ", \"groupagreebot\", 0) ON DUPLICATE KEY UPDATE count = (count + 1)";
			command.ExecuteNonQuery();
			connection.Close();
		}
	}
}

