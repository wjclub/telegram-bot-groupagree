using System;
using Newtonsoft.Json;

namespace WJClubBotFrame {
	public class Samples {
		public static WJClubBotFrame.Types.Update GetSampleUpdate() {
			return JsonConvert.DeserializeObject<WJClubBotFrame.Types.Update>("{\"update_id\":993737344,\n\"message\":{\"message_id\":1390,\"from\":{\"id\":133909606,\"first_name\":\"Jan\",\"last_name\":\"Braun\",\"username\":\"browny99\"},\"chat\":{\"id\":133909606,\"first_name\":\"Jan\",\"last_name\":\"Braun\",\"username\":\"browny99\",\"type\":\"private\"},\"date\":1471513975,\"text\":\"\\/start\",\"entities\":[{\"type\":\"bot_command\",\"offset\":0,\"length\":6}]}}");
		}
	}
}

