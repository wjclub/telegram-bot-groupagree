using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WJClubBotFrame.Types;

namespace telegrambotgroupagree {
	public class RequestHandler {
		/*
			Limits for the telegram api, -1 means message won't be accepted because it is too long 
		*/
		public static int[] requestsPerMinute = { 20, 4, -1};
		public static int[] requestThresholds = { 512, 4096};
		
		//instance, user, poll
		private List<Poll> pollsToUpdateWhenBored;

		public bool getRestricted(Instance currentInstance, Poll currentPoll) {
			//TODO Make this work
			return false;
		}

		public bool getRestricted(Instance currentInstance, User currentUser) {
			return false;
		}

		public bool getToThrottle(Instance currentInstance, Poll currentPoll) {
			//TODO Make this work
			return false;
		}

		public bool getToThrottle(Instance currentInstance, User currentUser) {
			return false;
		}
	}
}
