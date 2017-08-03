using System.Web;

namespace telegrambotgroupagree {
	public class HtmlSpecialChars {
		public static string Encode (string input) {
			return HttpUtility.HtmlAttributeEncode (input);
		}
	}
}