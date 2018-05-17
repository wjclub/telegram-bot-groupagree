using Newtonsoft.Json;

namespace WJClubInternalApiFrame {
	public class InternalRequest {
		[JsonProperty(PropertyName = "ok", Required = Required.Always)]
		public bool Ok;
	}

	public class InternalUpdate {
		[JsonProperty(PropertyName = "id", Required = Required.Always)]
		public int ID;
		[JsonProperty(PropertyName = "data", Required = Required.Always)]
		public string Data;
	}
}