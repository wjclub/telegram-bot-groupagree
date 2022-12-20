namespace GroupAgreeBot.Services;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using CSharpFunctionalExtensions;
using GroupAgreeBot.Models;

public class MockupStorage : IGroupAgreeBotStorage {
    private readonly List<Poll> _polls = new();

    // public Task<Poll> GetPollAsync(Guid id) {
    //     return Task.FromResult(_polls.Find(p => p.Id == id) ?? throw new KeyNotFoundException());
    // }

    // public Task StorePollAsync(Poll poll) {
    //     var existingPoll = _polls.Find(p => p.Id == poll.Id);
    //     if (existingPoll != null) {
    //         _polls.Remove(existingPoll);
    //     }
    //     _polls.Add(poll);
    //     return Task.CompletedTask;
    // }

    // public Task DeletePollAsync(Guid id) {
    //     var existingPoll = _polls.Find(p => p.Id == id);
    //     if (existingPoll != null) {
    //         _polls.Remove(existingPoll);
    //     }
    //     return Task.CompletedTask;
    // }

    public Task<Result<IEnumerable<Poll>>> GetPollsAsync() {
        return Task.FromResult(Result.Success(_polls.AsEnumerable()));
    }

    public Task<Result<IEnumerable<Poll>>> GetPollsAsync(PollFilterOptions filter) {
        throw new NotImplementedException();
    }

    public async Task<Maybe<Poll>> GetPollAsync(Guid id) {
        var poll = _polls.Find(p => p.Id == id);
        return poll ?? Maybe<Poll>.None;
    }

    public async Task<Result> StorePollAsync(Poll poll, Poll? oldPoll)
    {
        // check if stored poll is the same as the old poll
        // if it is, replace it with the new poll
        // if it is not, return failure
        var storedPoll = await GetPollAsync(poll.Id);
        if (storedPoll.HasNoValue) {
            _polls.Add(poll);
            return Result.Success();
        }
        storedPoll.ToResult("Poll was not found")
            .Ensure(p => p == oldPoll, "Poll has been modified by another user")
            .Tap(p => _polls.Remove(p))
            .Tap(_ => _polls.Add(poll));
        return Result.Success();
    }

    public async Task<Result> DeletePollAsync(Guid id) {
        var poll = await GetPollAsync(id);
        if (poll.HasNoValue) {
            return Result.Failure("Poll was not found");
        }
        _polls.Remove(poll.Value);
        return Result.Success();
    }

    public Task<Result> DeletePollAsync(Poll poll) {
        return DeletePollAsync(poll.Id);
    }
}