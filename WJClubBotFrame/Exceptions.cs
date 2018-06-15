using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WJClubBotFrame.Exceptions {
	public class MessageIDInvalid:Exception {
		public override string Message => "MTProto Exception: MESSAGE_ID_INVALID";
	}

	public class MessageTooLong : Exception {
		public override string Message => "MTProto Exception: MESSAGE_TOO_LONG";
	}

	public class TooManyRequests : Exception {
		public TooManyRequests (int retryAfter) {
			RetryAfter = retryAfter;
		}
		public override string Message => "Too Many Requests";
		public int RetryAfter;
	}
}
