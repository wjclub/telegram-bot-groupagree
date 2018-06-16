namespace telegrambotgroupagree {
	public class UpdateAvailabilityList {
		public int maxUpdates;
		public int recommendedUpdates;

		public static UpdateAvailabilityList FactoryNoUpdatesLeft() => new UpdateAvailabilityList {
			maxUpdates = 0,
			recommendedUpdates = 0,
		};
	}
}
