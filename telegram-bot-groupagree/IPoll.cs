namespace telegrambotgroupagree {
	public interface IPoll {
		string ToString();
		void AddOption(string text);
		bool Vote(long chat_id, long poll_id, string option);
		bool SetClosed(bool closed);
		bool SetArchived(bool archived);
	}
}