using System;
namespace telegrambotgroupagree {
	public static class BoolWorker {
		public static string ToCheck(this bool input) {
			return (input ? "✅": "❌");
		}
	}
}

