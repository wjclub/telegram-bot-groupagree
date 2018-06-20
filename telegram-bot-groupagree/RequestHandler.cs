using System;
using System.Collections.Generic;

namespace telegrambotgroupagree {
	public static class RequestHandler {
		//Limits for the telegram api
		public static readonly int messageUnitSize = 512;
		public static readonly int maxMessageLength = 4096;
		public static readonly int maxUserRequestsPerMinute = 5;
		public static readonly int maxChatUpdatesPerMinute = 20;
		public static readonly int recommendedChatUpdatesPerMinute = 15;
		public static readonly int maxInstanceUpdatesPerSecond = 30;
		public static readonly int recommendedInstanceUpdatesPerSecond = 25;

		//If user pressed more than 5 inline buttons in the last minute
		public static bool getUserRestricted(Pointer pointer) {
			if (pointer == null) {
				throw new ArgumentNullException("pointer", "Empty pointer at 239857209843");
			} else if (pointer.LastRequests == null) {
				throw new ArgumentNullException("pointer.LastRequests", "Empty pointer.LastRequests at 4435093247934");
			}
			if (pointer.LastRequests.Count > maxUserRequestsPerMinute && pointer.LastRequests[maxUserRequestsPerMinute] - pointer.LastRequests[0] > TimeSpan.FromMinutes(1)) {
				return true;
			}
			return false;
        }

        public static UpdateAvailabilityList GetInstanceAvailableUpdates(Instance instance) {
			if (instance.retryAt >= DateTime.Now) {
				return UpdateAvailabilityList.FactoryZeroUpdatesLeft();
			}
            return GetListFromLastUpdatesList(
				datesList:instance.last30Updates, 
				max: maxInstanceUpdatesPerSecond, 
				recommended: recommendedInstanceUpdatesPerSecond, 
				cooldown:TimeSpan.FromSeconds(1));
        }

		public static UpdateAvailabilityList GetMessageIDAvailableUpdates(MessageID messageID) 
			=> GetListFromLastUpdatesList(
				messageID.last30Updates,
				max: maxChatUpdatesPerMinute,
				recommended: recommendedChatUpdatesPerMinute,
				cooldown:TimeSpan.FromMinutes(1));

		public static UpdateAvailabilityList GetListFromLastUpdatesList(List<DateTime> datesList, int max, int recommended, TimeSpan cooldown) {
            DateTime startingNow = DateTime.Now;
			UpdateAvailabilityList result = UpdateAvailabilityList.FactoryZeroUpdatesLeft();
            for (int i = 0; i < datesList.Count; i++) {
                if (startingNow - datesList[i] > cooldown) {
                    result.maxUpdates = max - (Math.Min(max, i));
                    result.recommendedUpdates = recommended - Math.Min(recommended,i);
                    break;
                }
            }
            return result;
        }

		public static bool DoUpdate(UpdateAvailabilityList updateAvailabilityList, int messageTextLength, bool necessary = false) {
			int updateWeight = IsFatUpdate(messageTextLength: messageTextLength) ? 4 : 1;
			if (necessary) {
				return updateAvailabilityList.maxUpdates >= updateWeight;
			} else {
				return updateAvailabilityList.recommendedUpdates >= updateWeight;
			}
		}

		public static bool IsFatUpdate(int messageTextLength) 
			=> messageTextLength >= messageUnitSize;
	}
}