# GroupAgree Bot
The [@groupagreebot](https://t.me/groupagreebot) is an advanced poll bot for Telegram. It features fully customizable polls for organizing your group chats and keeping the spam to a minimum.

An example: You want to hang out with your friends, but you want to find out when they have time. Normally, you would just ask when to meet, resulting in dozens of messages being send and staying on top of it becomes very hard. So, instead of whipping out the old pen and paper and counting people by hand, just ask the GroupAgree Bot kindly to do that for you:

![Screenshot](https://telegra.ph/file/ab7f9a071c55c4d42b1b2.png)
 
Just try it out yourself: Visit https://telegram.me/groupagreebot
 
The **license** for this project is AGPLv3, please read the license file in this repo before modifying or cloning this repo.

# How to set it up:

**You need**

Xamarin Studio / MonoDevelop: http://www.monodevelop.com/download/ (or another C# IDE)

(For Windows: .NET 4.5.X Developer Pack https://www.microsoft.com/net/targeting)

MySQL: https://dev.mysql.com/downloads/

JSON.NET: http://www.newtonsoft.com/json (should be integrated by now through nuget tho)

**Setup**

This is a C# project, so you need mono for non Microsoft operating systems or .NET 4.5 (or higher) for Windows to run it.

Start by installing Xamarin / MonoDevelop and MySQL.

Set you bot up through (@botfather)[https://t.me/botfather].
	Allow inline queries, disable groups and set inline query feedback to 100%.

Use the chat id as mysql username and the key as password (format: chat_id:botkey, like `1122334455:AABBCCDDEEFFGG11223344`).
	This is to make multiple instances on one machine possible.

__Temporary solution for the db layout__
Modify [this script](groupagreebot_v_4_0_database.sql) for the use with your bot (replace bot_chat_id with your bots chat id).

Start Xamarin / MonoDevelop (maybe you need to install the version control addon, but Xamarin/MonoDevelop should prompt you)

Go to Version Control and select Checkout, add this repo (or your fork ;D, seriously) and Checkout, now you should have the source code ready and imported in your IDE.

Modify the file GroupAgreeBot/Globals.cs with your bot key and name (I currently have three configurations, just use Debug for a local instance) and optionally WJClubBotFrame/Notifications.cs with your logging bots api key and your logging chats chat id (can be yours or any other chat, you just have to add the bot to it)

Now **release your source code** (seriously, once you start the bot you have to release the source code, read the license)

Click start in your IDE and you should be good to go, your bot should be up and running :D

Any questions or concerns, ask in (@groupagreebotdevelopment)[https://t.me/groupagreebotdevelopment] or pm us (@wjclub)[https://t.me/wjclub]
If you found bugs, want to contribute to this project etc, use GitHubs features, we love to see them in action. (And have no fear, we won't laugh at you for stupid issues)

Happy polling

browny99, the guy who wrote this whole piece of dark art