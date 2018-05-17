using System.Text;
using System.Web;

namespace telegrambotgroupagree {
	public static class HashWorker {
		public static string Base53Encode(string input) {
			return HttpServerUtility.UrlTokenEncode(Encoding.GetEncoding("UTF-8").GetBytes(input.ToCharArray()));
		}

		public static string Base53Decode(this string input) {
			return Encoding.GetEncoding("UTF-8").GetString(HttpServerUtility.UrlTokenDecode(input));
		}
	}
}
