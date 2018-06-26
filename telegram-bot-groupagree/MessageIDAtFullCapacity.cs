using System;
using System.Runtime.Serialization;

namespace telegrambotgroupagree {
	[Serializable]
	internal class MessageIDAtFullCapacity:Exception {
		public MessageIDAtFullCapacity() {
		}

		public MessageIDAtFullCapacity(string message) : base(message) {
		}

		public MessageIDAtFullCapacity(string message, Exception innerException) : base(message, innerException) {
		}

		protected MessageIDAtFullCapacity(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}