using System.Reflection;
using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using GroupAgreeBot.Models;
using Microsoft.Extensions.Logging;

namespace GroupAgreeBot.Services;

public sealed class PollService {
    private readonly IGroupAgreeBotStorage _storage;
    private readonly ILogger<PollService> _logger;
    private readonly DateTime _now;

    public PollService(IGroupAgreeBotStorage storage, ILogger<PollService> logger, DateTime now) {
        _storage = storage;
        _logger = logger;
        _now = now;
    }

    public async Task<Result> CreatePollAsync(Poll poll) {
        if ((await _storage.GetPollAsync(poll.Id)).HasValue) {
            return Result.Failure("Poll already exists");
        }
        return await _storage.StorePollAsync(poll);
    }

    public Task<Maybe<Poll>> GetPollAsync(Guid id) {
        return _storage.GetPollAsync(id);
    }

    public async Task<Result> DeletePollAsync(Guid id) {
        var poll = await _storage.GetPollAsync(id);
        if (poll.HasNoValue) {
            return Result.Failure("Poll not found");
        }

        return await _storage.DeletePollAsync(poll.Value);
    }

    public async Task<Result> VoteAsync(GroupAgreeBotUser user, Poll poll, Guid optionId) {
        var unmodifiedPoll = poll.Duplicate();
        if (poll.IsExpired(_now)) {
            return Result.Failure("Poll is expired");
        }

        var option = poll.Options.FirstOrDefault(o => o.Id == optionId);
        if(option is null)
            return Result.Failure("The selected option does not exist");

        // Check if the user has already voted for this option
        // if they did, remove their vote
        var userVote = option.Votes.FirstOrDefault(v => v.User.Id == user.Id);
        if (userVote is not null) {
            option.Votes.Remove(userVote);
            return await _storage.StorePollAsync(poll, unmodifiedPoll);
        }
        IEnumerable<PollVote> userVotes = poll.Options.SelectMany(o => o.Votes).Where(v => v.User.Id == user.Id);
        long userVotesCount = userVotes.Count();

        // Check maximum votes per user, users can vote multiple times if the poll allows it
        // If maxVotesPerUser is 1, the current option will be deselected if the user has already voted, otherwise show error message
        if (poll.PollSettings.TryGetValue(PollSetting.MaxVotesPerUser, out var maxVotesPerUser)) {
            if (!long.TryParse(maxVotesPerUser, out long maxVotes))
                return Result.Failure("Invalid max votes per user setting"); // TODO maybe throw exception? this state should never happen
            if(maxVotes <= 0)
                return Result.Failure("This poll is locked and cannot be voted on");
            if (userVotesCount >= maxVotes && maxVotes != 1) {
                return Result.Failure("You have already voted the maximum number of times");
            }
        }

        // add the vote
        option.Votes.Add(new PollVote {
            User = user,
        });

        return await _storage.StorePollAsync(poll, unmodifiedPoll);
    }

    public async Task<Result> AddOptionAsync(Poll poll, PollOption option, GroupAgreeBotUser addedByUser) {
        var unmodifiedPoll = poll.Duplicate();
        if (poll.IsExpired(_now)) {
            return Result.Failure("Poll is expired");
        }

        if (poll.Options.Any(o => o.Id == option.Id)) {
            return Result.Failure("Option already exists");
        }

        bool usersCanAddOptions = false; // TODO make configurable using config file or database
        // get setting value from poll settings and try to parse it to bool, assume false
        if (poll.PollSettings.TryGetValue(PollSetting.UsersCanAddOptions, out var usersCanAddOptionsString)
            && !bool.TryParse(usersCanAddOptionsString, out usersCanAddOptions)) {
            _logger.LogWarning("Invalid UsersCanAddOptions setting for poll {PollId}", poll.Id);
        }

        if (poll.Creator != addedByUser && !usersCanAddOptions) {
            return Result.Failure("You are not allowed to add options");
        }

        poll.Options.Add(option);
        return await _storage.StorePollAsync(poll, unmodifiedPoll);
    }

    public async Task<Result> DeleteOptionAsync(Poll poll, Guid optionId, GroupAgreeBotUser user) {
        var unmodifiedPoll = poll.Duplicate();
        if (poll.IsExpired(_now)) {
            return Result.Failure("Poll is expired");
        }

        var option = poll.Options.FirstOrDefault(o => o.Id == optionId);
        if (option is null) {
            return Result.Failure("Option does not exist");
        }

        if (poll.Creator != user) {
            return Result.Failure("You are not allowed to delete options");
        }

        poll.Options.Remove(option);
        return await _storage.StorePollAsync(poll, unmodifiedPoll);
    }

    public async Task<Result> EditOptionAsync(Poll poll, Guid optionId, GroupAgreeBotUser user, string newTitle, string? newDescription = null) {
        var unmodifiedPoll = poll.Duplicate();
        if (poll.IsExpired(_now)) {
            return Result.Failure("Poll is expired");
        }

        var option = poll.Options.FirstOrDefault(o => o.Id == optionId);
        if (option is null) {
            return Result.Failure("Option does not exist");
        }

        if (poll.Creator != user) {
            return Result.Failure("You are not allowed to edit options");
        }

        option.Title = newTitle;
        if (newDescription is not null)
            option.Description = newDescription;
        return await _storage.StorePollAsync(poll, unmodifiedPoll);
    }

    public async Task<Result> EditPollAsync(Poll poll, GroupAgreeBotUser user, string newTitle, string? newDescription = null) {
        var unmodifiedPoll = poll.Duplicate();
        if (poll.IsExpired(_now)) {
            return Result.Failure("Poll is expired");
        }

        if (poll.Creator != user) {
            return Result.Failure("You are not allowed to edit this poll");
        }

        poll.Title = newTitle;
        if (newDescription is not null)
            poll.Description = newDescription;
        return await _storage.StorePollAsync(poll, unmodifiedPoll);
    }

    public async Task<Result> EditPollSettingAsync(Poll poll, GroupAgreeBotUser user, PollSetting setting, string? value = null, bool resetToDefault = false) {
        var unmodifiedPoll = poll.Duplicate();
        if (poll.IsExpired(_now)) {
            return Result.Failure("Poll is expired");
        }
        // todo refactor into user auth service or whatever
        if (poll.Creator != user) {
            return Result.Failure("You are not allowed to edit this poll");
        }

        if(resetToDefault) {
            poll.PollSettings.Remove(setting);
        } else {
            poll.PollSettings[setting] = value ?? string.Empty;
        }

        return await _storage.StorePollAsync(poll, unmodifiedPoll);
    }

    // something you can subscribe to to get notified when a poll is updated
    // BIG TODO: learn how to do this properly
    // read https://learn.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/event-based-asynchronous-pattern-overview
    // and https://learn.microsoft.com/en-us/dotnet/standard/events/observer-design-pattern ig
    // public event EventHandler<Poll>? PollUpdated;
}