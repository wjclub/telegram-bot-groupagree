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
            this.chatID = int.Parse(valuesplit[0]);
            this.key = valuesplit[1];
        }
    }

    public int offset;
    public User botUser;
    public User creator;
    public Task<Update[]> update;
    public List<DateTime> last30Updates;
	internal DateTime retryAt;
}