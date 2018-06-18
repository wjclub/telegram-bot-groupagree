using System;
using System.Runtime.Serialization;

namespace telegrambotgroupagree {
	[Serializable]
	internal class UpdateMessageIDInvalidException:Exception {
		public UpdateMessageIDInvalidException() {
		}

		public UpdateMessageIDInvalidException(string message) : base(message) {
		}

		public UpdateMessageIDInvalidException(string message, Exception innerException) : base(message, innerException) {
		}

		protected UpdateMessageIDInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}