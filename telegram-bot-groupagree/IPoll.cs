namespace telegrambotgroupagree {
	public interface IPoll {
		string ToString();
		void AddOption(string text);
		bool Vote(int chat_id, int poll_id, string option);
		bool SetClosed(bool closed);
		bool SetArchived(bool archived);
	}
}