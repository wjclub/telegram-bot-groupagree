using Newtonsoft.Json;

namespace GroupAgreeBot.Models;

public sealed class Poll {
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required GroupAgreeBotUser Creator { get; set; }

    public required DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; set; }

    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();

    public Dictionary<PollSetting, string> PollSettings { get; set; } = new();

    public bool IsExpired(DateTime now) {
        if(ExpiresAt is null)
            return false;
        return now > ExpiresAt;
    }

    public Poll Duplicate() {
        var serialized = JsonConvert.SerializeObject(this);
        return JsonConvert.DeserializeObject<Poll>(serialized)!;
    }
}

public sealed class PollOption {
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public ICollection<PollVote> Votes { get; set; } = new List<PollVote>();
}

public sealed class PollVote {
    public required GroupAgreeBotUser User { get; set; }
    public Dictionary<PollVoteSetting, string> Settings { get; set; } = new();
}

public enum PollSetting {
    MaxVotesPerUser,
    Anonymous,
    UsersCanAddOptions,
}

public enum PollVoteSetting {
    MaxVoteSlots,
}