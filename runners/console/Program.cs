// using GroupAgreeBot.Services;
// using Telegram.Bot;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Hosting;

// var host = Host.CreateDefaultBuilder(args);

// var services = host.ConfigureServices((context, services) => {
//     services.AddHttpClient("telegram_bot_client")
//             .AddTypedClient<ITelegramBotClient>((httpClient, services) =>
//             {
//                 TelegramBotClientOptions options = new("599164885:AAFeuIhKvahJWpMiGrTOaYdnjYo4o1qErkg");
//                 return new TelegramBotClient(options, httpClient);
//             });

//     // add logging
//     services.AddLogging(configure => configure.AddConsole());

//     // add storage
//     services.AddSingleton<IGroupAgreeBotStorage, MockupStorage>();

//     services.AddTransient<TelegramUpdateHandler>();

//     services.AddHostedService<RequestHandler>();
// });

// await host.RunConsoleAsync();