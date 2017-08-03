namespace telegrambotgroupagree {
	public static class ArrayWorker {
		public static string Implode(this string[] input, string delimiter = "") {
			string text = "";
			foreach (string textlet in input) {
				text += textlet + delimiter;
			}
			if (input.Length < 0) {
				text = text.Substring(0, text.Length - delimiter.Length);
			}
			return text;
		}
	}
}

