using System;

namespace telegrambotgroupagree {
	public static class PercentageBars {
		static readonly string[] none = { "", "" };
		static readonly string[] dots = { "●" , "○" };
		static readonly string[] thumbs = { "\ud83d\udc4d", "" };

		public enum Bars {
			none, dots, thumbs
		}

		public static string RenderPercentage(double percentage, Bars bar, bool pipes = false) {
			string output = "";
			if (percentage >= 0 && percentage <= 1) {
				percentage = percentage * 10;
				for (int i = 0; i < 10; i++) {
					if (i + 1 <= Math.Round(percentage))
						output += GetIconArray(bar)[0];
					else
						output += GetIconArray(bar)[1];
				}
			}
			return output != "" ? (pipes ? "┆ " : "") + output + " (" + Math.Round(percentage*10) + "%)\n" : output;
		}

		public static string[] GetIconArray(Bars bar) {
			switch (bar) {
				case Bars.none: return none;
				case Bars.dots: return dots;
				case Bars.thumbs: return thumbs;
				default: return none;
			}
		}
	}
}