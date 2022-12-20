
using GroupAgreeBot.Models;
using GroupAgreeBot.Services;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace GroupAgreeTests;

public class PollServiceUnitTest {
    private readonly PollService _pollService;
    private readonly IGroupAgreeBotStorage _storage;
    private readonly ITestOutputHelper _output;

    public PollServiceUnitTest(ITestOutputHelper output) {
        _storage = new MockupStorage();
        _output = output;
        var logger = new LoggerFactory().AddXUnit(output).CreateLogger<PollService>();
        _pollService = new PollService(_storage, logger, DateTime.Now);
    }

    [Fact]
    public async Task CreatePollAsync() {
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
}