using System;
using System.Collections.Generic;

namespace telegrambotgroupagree {
	public class MessageID {
		public long botChatID;
		public string inlineMessageId;
		public bool channel;
        public List<DateTime> last30Updates; 
	}
}