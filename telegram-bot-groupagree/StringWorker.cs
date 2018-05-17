using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace telegrambotgroupagree {
	public static class StringWorker {
		public static string Truncate(this string value, int maxLength, bool noPoints = false) {
			if (string.IsNullOrEmpty(value)) return value;
			StringInfo strInfo = new StringInfo(value);
			if (maxLength <= 3 || noPoints) 
				return strInfo.LengthInTextElements <= maxLength ? strInfo.String : strInfo.SubstringByTextElements(0, maxLength);
			return strInfo.LengthInTextElements <= maxLength ? strInfo.String : strInfo.SubstringByTextElements(0, maxLength - 3) + "...";
		}

		public static string UnmarkupUsernames(this string value, string markup = "b") {
			string front = "</" + markup + ">";
			string back = "<" + markup + ">";
			string pattern = @"(((?<=\s)|(^))[@#])([a-z_A-Z][a-z_A-Z0-9]{0,32})((?=[^a-z^_^A-Z^0-9]|$))";
			int globalOffset = 0;

			string returnValue = value;

			foreach (Match m in Regex.Matches(value, pattern)) {
				returnValue = returnValue.Substring(0,m.Index + globalOffset) + front + returnValue.Substring(m.Index + globalOffset, m.Length) + back + returnValue.Substring(m.Index + m.Length + globalOffset);
				globalOffset += front.Length + back.Length;
			}

			return returnValue;
		}

		public static string RemoveAppendingText(this string input, Match match = null) {
			match = (match ?? Poll.GetAppendingMatch(input));
			if (match.Success)
				return input.Substring(match.Index + match.Length + 2);
			return input;
		}
	}
}

