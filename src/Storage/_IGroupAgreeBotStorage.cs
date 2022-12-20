namespace GroupAgreeBot.Services;

using CSharpFunctionalExtensions;
using GroupAgreeBot.Models;

public interface IGroupAgreeBotStorage {
    public Task<Result<IEnumerable<Poll>>> GetPollsAsync();
    public Task<Result<IEnumerable<Poll>>> GetPollsAsync(PollFilterOptions filter);
    public Task<Maybe<Poll>> GetPollAsync(Guid id);
    /// <summary>
    /// Stores a poll. If the poll already exists, it will be updated.
    /// </summary>
    /// <param name="poll"></param>
    public Task<Result> StorePollAsync(Poll poll, Poll? oldPoll = null); //TODO check if old poll is currently in storage, if not, return error
    public Task<Result> DeletePollAsync(Guid id);
    public Task<Result> DeletePollAsync(Poll poll);
}

public sealed class PollFilterOptions {
    public bool IncludeExpired { get; set; } = true;
    public string FullTextSearch { get; set; } = string.Empty;
    public Guid ForUserId { get; set; } = Guid.Empty;
    public Guid PollId { get; set; } = Guid.Empty;
}