namespace GroupAgreeTests;

public class PollServiceUnitTest {
    private readonly PollService _pollService;
    private readonly IGroupAgreeBotStorage _storage;
    private readonly ILogger<PollService> _logger;

    public PollServiceUnitTest(ITestOutputHelper output) {
        // constructor is called before each test, so we start with a clean storage
        _storage = new MockupStorage();
        _logger = new LoggerFactory().AddXUnit(output).CreateLogger<PollService>();
        _pollService = new PollService(_storage, _logger, DateTime.Now);
    }

    [Fact]
    public async Task Test_CreatePollAsync_CreatePoll() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Test_CreatePollAsync_RejectDuplicateIDs() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        result = await _pollService.CreatePollAsync(poll);
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Test_GetPollAsync_ReturnsTheSamePoll() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        var maybePoll = await _pollService.GetPollAsync(poll.Id);
        Assert.True(maybePoll.HasValue);
        // check if the poll is the same using reflection
        foreach (var property in typeof(Poll).GetProperties()) {
            Assert.Equal(property.GetValue(poll), property.GetValue(maybePoll.Value));
        }
    }

    [Fact]
    public async Task Test_GetPollAsync_ReturnsNoValue() {
        var maybePoll = await _pollService.GetPollAsync(Guid.NewGuid());
        Assert.True(maybePoll.HasNoValue);
    }

    [Fact]
    public async Task Test_DeletePollAsync_DeletesPoll() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        result = await _pollService.DeletePollAsync(poll.Id);
        Assert.True(result.IsSuccess);
        var maybePoll = await _pollService.GetPollAsync(poll.Id);
        Assert.True(maybePoll.HasNoValue);
    }

    [Fact]
    public async Task Test_DeletePollAsync_ReturnsFailure() {
        var result = await _pollService.DeletePollAsync(Guid.NewGuid());
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Test_AddOptionAsync_AddsOption() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        var option = new PollOption {
            Id = Guid.NewGuid(),
            Title = "Test Option",
            Description = "This is a test option",
        };
        result = await _pollService.AddOptionAsync(poll, option, poll.Creator);
        Assert.True(result.IsSuccess);
        var maybePoll = await _pollService.GetPollAsync(poll.Id);
        Assert.True(maybePoll.HasValue);
        Assert.Contains(option, maybePoll.Value.Options);
    }

    [Fact]
    public async Task Test_AddOptionAsync_RejectsDuplicateIDs() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        var option = new PollOption {
            Id = Guid.NewGuid(),
            Title = "Test Option",
            Description = "This is a test option",
        };
        result = await _pollService.AddOptionAsync(poll, option, poll.Creator);
        Assert.True(result.IsSuccess);
        result = await _pollService.AddOptionAsync(poll, option, poll.Creator);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Test_AddOptionAsync_AcceptsDuplicateTitles() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        var option = new PollOption {
            Id = Guid.NewGuid(),
            Title = "Test Option",
            Description = "This is a test option",
        };
        result = await _pollService.AddOptionAsync(poll, option, poll.Creator);
        Assert.True(result.IsSuccess);
        option = new PollOption {
            Id = Guid.NewGuid(),
            Title = "Test Option",
            Description = "This is a test option",
        };
        result = await _pollService.AddOptionAsync(poll, option, poll.Creator);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Test_AddOptionAsync_RejectsNonCreatorByDefault() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        var option = new PollOption {
            Id = Guid.NewGuid(),
            Title = "Test Option",
            Description = "This is a test option",
        };
        result = await _pollService.AddOptionAsync(poll, option, new GroupAgreeBotUser {
            Id = Guid.NewGuid(),
            TelegramId = 987654321,
            FirstName = "Test",
            LastName = "User",
        });
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task Test_AddOptionAsync_AcceptsNonCreatorIfAllowed() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
            PollSettings = new Dictionary<PollSetting, string> {
                { PollSetting.UsersCanAddOptions, "true" },
            },
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        var option = new PollOption {
            Id = Guid.NewGuid(),
            Title = "Test Option",
            Description = "This is a test option",
        };
        result = await _pollService.AddOptionAsync(poll, option, new GroupAgreeBotUser {
            Id = Guid.NewGuid(),
            TelegramId = 987654321,
            FirstName = "Test",
            LastName = "User",
        });
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Test_AddOptionAsync_RejectsNonCreatorIfNotAllowed() {
        var poll = new Poll {
            Id = Guid.NewGuid(),
            Title = "Test Poll",
            Description = "This is a test poll",
            Creator = new GroupAgreeBotUser {
                Id = Guid.NewGuid(),
                TelegramId = 123456789,
                FirstName = "Test",
                LastName = "User",
            },
            CreatedAt = DateTime.Now,
            PollSettings = new Dictionary<PollSetting, string> {
                { PollSetting.UsersCanAddOptions, "false" },
            },
        };
        var result = await _pollService.CreatePollAsync(poll);
        Assert.True(result.IsSuccess);
        var option = new PollOption {
            Id = Guid.NewGuid(),
            Title = "Test Option",
            Description = "This is a test option",
        };
        result = await _pollService.AddOptionAsync(poll, option, new GroupAgreeBotUser {
            Id = Guid.NewGuid(),
            TelegramId = 987654321,
            FirstName = "Test",
            LastName = "User",
        });
        Assert.True(result.IsFailure);
    }
}