using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace telegrambotgroupagree {
	public class MessageID {
		public long botChatID;
		public string inlineMessageId;
		public bool channel;
		public bool messageIDInvalid;
        public List<DateTime> last30Updates; 
	}
}