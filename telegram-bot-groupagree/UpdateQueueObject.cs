using System.Collections.Generic;

namespace telegrambotgroupagree {
	public class UpdateQueueObject {
		public Poll poll;
		public List<string> priorityUpdates;
		public bool important;
	}
}