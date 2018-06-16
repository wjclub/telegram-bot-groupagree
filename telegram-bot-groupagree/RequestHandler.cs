using System;
using System.Collections.Generic;

namespace telegrambotgroupagree {
	public static class RequestHandler {
		//Limits for the telegram api, -1 means message won't be accepted because it is too long 
		public static readonly int[] requestsPerMinute = { 20, 4, -1};
		public static readonly int[] requestThresholds = { 512, 4096};
		
		//If user pressed more than 3 inline buttons in the last minute
        public static bool getUserRestricted(Pointer pointer) {
			//TODO Handle empty lastrequests
            if (pointer.LastRequests[2] - pointer.LastRequests[0] > TimeSpan.FromMinutes(1)) {
				return true;
			}
			return false;
        }

        public static UpdateAvailabilityList GetInstanceAvailableUpdates(Instance instance) {
			if (instance.retryAt >= DateTime.Now) {
				return UpdateAvailabilityList.FactoryNoUpdatesLeft();
			}
            return GetListFromLastUpdatesList(datesList:instance.last30Updates, max: 30, recommended: 25, cooldown:TimeSpan.FromSeconds(1));
        }

		public static UpdateAvailabilityList GetInlineMessageAvailableUpdates(string inlineMessageID, Poll poll) 
			=> GetListFromLastUpdatesList(
				poll.MessageIds.Find(x => x.inlineMessageId == inlineMessageID).last30Updates,
				max: 20,
				recommended: 15,
				cooldown:TimeSpan.FromMinutes(1));

		public static UpdateAvailabilityList GetListFromLastUpdatesList(List<DateTime> datesList, int max, int recommended, TimeSpan cooldown) {
            DateTime startingNow = DateTime.Now;
			UpdateAvailabilityList result = UpdateAvailabilityList.FactoryNoUpdatesLeft();
            for (int i = 0; i < datesList.Count; i++) {
                if (startingNow - datesList[i] > cooldown) {
                    result.maxUpdates = max - (Math.Min(max, i));
                    result.recommendedUpdates = recommended - Math.Min(recommended,i);
                    break;
                }
            }
            return result;
        }

		public static bool DoUpdate(UpdateAvailabilityList updateAvailabilityList, bool necessary)
			=> necessary ? updateAvailabilityList.maxUpdates > 0 : updateAvailabilityList.recommendedUpdates > 0;
	}
}