using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WJClubBotFrame.Types;

public class Instance {
    public long chatID { get; internal set; }
    public string key { get; internal set; }
    public string apikey {
        get {
            return $"{chatID}:{key}";
        } set {
            string[] valuesplit = value.Split(':');
            this.chatID = long.Parse(valuesplit[0]);
            this.key = valuesplit[1];
        }
    }

    public long offset;
    public User botUser;
    public User creator;
    public Task<Update[]> update;
	private List<DateTime> last30Updates;
	public List<DateTime> Last30Updates {
		get {
			if (last30Updates == null) {
				last30Updates = new List<DateTime>();
			}
			return last30Updates;
		}
		set => last30Updates = value;
	}
	public DateTime? retryAt;
}