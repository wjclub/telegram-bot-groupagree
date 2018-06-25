using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace telegrambotgroupagree {
	public class MessageID {
		public long botChatID;
		public string inlineMessageId;
		public bool channel;
		public bool messageIDInvalid;
		private List<DateTime> last30Updates;
		public List<DateTime> Last30Updates {
			get {
				if (last30Updates == null) {
					last30Updates = new List<DateTime>();
				}
				return last30Updates;
			}
			set => last30Updates = value;
		}
	}
}