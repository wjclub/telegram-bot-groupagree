using System.Collections.Generic;

namespace telegrambotgroupagree {
	public class UpdateQueueObject {
		public Poll poll;
		public List<string> priorityUpdates;
		public List<string> doneUpdates;
		public bool important;
		public long fromBotID;
	}
}