using System.Data;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

var botToken = Console.ReadLine();
var api = new BotClient(botToken);

var updates = api.GetUpdates();
while (true) {
    if (updates.Any()) {
        foreach (var update in updates) {
            if(update.Message?.Text is string text) {
                api.SendMessage(new SendMessageArgs(update.Message.Chat.Id, text));
            }
        }
        var offset = updates.Last().UpdateId + 1;
        updates = api.GetUpdates(offset);
    } else {
        updates = api.GetUpdates();
    }
}