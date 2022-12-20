// using System.Reflection;
// using System.Runtime.CompilerServices;
// using System.Threading.Tasks.Sources;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;

// namespace GroupAgreeBot.Services;

// public sealed class RequestHandler : IHostedService {
//     private readonly ILogger<RequestHandler> _logger;
//     private readonly ITelegramBotClient _botClient;
//     private readonly List<Task> _requestTasks;
//     private readonly Router _router;

//     public RequestHandler(ILogger<RequestHandler> logger, ITelegramBotClient botClient, Router router) {
//         _logger = logger;
//         _botClient = botClient;
//         _router = router;
//         _requestTasks = new();
//     }

//     public async Task HandleUpdateAsync(ITelegramBotClient _botClient, Update update, CancellationToken cancellationToken) {
//         var requests = await _router.HandleAsync(update);
//         foreach (var request in requests) {
//             _requestTasks.Add(_botClient.MakeRequestAsync(request, cancellationToken));
//         }
//     }

//     public async Task HandleErrorAsync(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken) {
//         _logger.LogError(exception, "Error while handling update");
//     }

//     public Task StartAsync(CancellationToken cancellationToken) {
//         _botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: cancellationToken);
//         return Task.CompletedTask;
//     }

//     public Task StopAsync(CancellationToken cancellationToken) {
//         return Task.WhenAll(_requestTasks);
//     }
// }