using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using WJClubBotFrame.Types;
using Api = WJClubBotFrame.Methods.Api;
using System.Linq;
using WJClubBotFrame;
using System.Web;
using System.Text;

namespace telegrambotgroupagree {
	public class GroupAgreeBot {
		private string apikey;
		private int offset;
		private PointerContainer pointerContainer;
		private PollContainer pollContainer;
		private DBHandler dBHandler;
		private Strings strings;

		public User BotInfo { get; internal set; }
		public Pointer CurrentPointer { get; internal set; }
		public Update CurrentUpdate { get; internal set; }

		public GroupAgreeBot(string apikey) {
			this.apikey = apikey;
			this.BotInfo = Api.GetMe(apikey);
			Api.DeleteWebhook(apikey);
			try {
				using (StreamReader reader = new StreamReader(@"offset.txt")) {
					offset = int.Parse(reader.ReadLine());
				}
			} catch (FileNotFoundException) {
				using (StreamWriter writer = new StreamWriter("offset.txt", false)) {
					writer.Write(0);
				}
				offset = 0;
			}
			dBHandler = new DBHandler(apikey);
			pointerContainer = new PointerContainer(dBHandler);
			strings = new Strings();
			pollContainer = new PollContainer(dBHandler, strings);
		}

		public override string ToString() {
			return JsonConvert.SerializeObject(BotInfo);
		}

		public void Run() {
			while (!System.IO.File.Exists(@"cancer.wjdummy")) {
				Update[] updates = Api.GetUpdates(apikey, offset);
				if (updates != null) {
					foreach (Update update in updates) {
						CurrentUpdate = update;
						offset = update.UpdateId + 1;
						using (StreamWriter writeOffset = new StreamWriter(@"offset.txt", false)) {
							writeOffset.Write(offset);
						}
						if (update.Message != null) {
							Pointer pointer = pointerContainer.GetPointer(update.Message.From.Id);
							if (pointer != null)
								strings.SetLanguage(pointer.Lang);
							else 
								strings.SetLanguage(Strings.langs.none);
							if (update.Message.Text != null) {
								if (pointer == null && !update.Message.Text.StartsWith("/start", StringComparison.CurrentCulture)) {
									Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.setupPlease));
									continue;
								}
								if (update.Message.Entities != null && update.Message.Entities[0].Type == EEntityType.bot_command && update.Message.Entities[0].Offset == 0) {
									string command = update.Message.Text.Substring(update.Message.Entities[0].Offset + 1, update.Message.Entities[0].Lenght - 1);
									switch (command) {
										case "start":
											bool notTheRightStart = false;
											if (!(update.Message.Text == "/start" || update.Message.Text == "/start from_inline")) {
												string[] payload = null;
												bool onlyUpdate = false;
												try {
													payload = Cryptography.Decrypt(update.Message.Text.Substring(7), apikey).Split(':');
												} catch (System.FormatException) {
													onlyUpdate = true;
													payload = update.Message.Text.Substring(7).Split(':');
													if (payload == null || payload.Length <= 1)
														notTheRightStart = true;
												}
												Poll poll;
												if (onlyUpdate) {
													Console.WriteLine(update.Message.Text);
													try {
														dBHandler.AddToQueue(pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2])));
														Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.updatingPoll));
													} catch (FormatException) {
														notTheRightStart = true;
													} catch (ArgumentOutOfRangeException) {
														Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.pollNotFound));
													}
												} else {
													switch (payload[0]) {
														case "board":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															pointer.BoardChatId = int.Parse(payload[1]);
															pointer.BoardPollId = int.Parse(payload[2]);
															pointer.Needle = ENeedle.board;
															poll = pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2]));
															try {
																Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.stringsList.boardAnswer), HtmlSpecialChars.Encode(poll.PollText).UnmarkupUsernames()));
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.pollNotFound));
															}
															break;
														case "vote":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															pointer.BoardChatId = int.Parse(payload[1]);
															pointer.BoardPollId = int.Parse(payload[2]);
															pointer.Needle = ENeedle.nothing;
															try {
																pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2])).Send(apikey, strings, update.Message.Chat.Id, true);
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.pollNotFound));
															}
															break;
														case "pag":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															try {
																pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2])).Send(apikey, strings, update.Message.Chat.Id, int.Parse(payload[3]));
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.pollNotFound));
															}
															break;
														case "append":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															try {
																poll = pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2]));
																pointer.BoardChatId = int.Parse(payload[1]);
																pointer.BoardPollId = int.Parse(payload[2]);
																pointer.Needle = ENeedle.addOption;
																Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.stringsList.appendSendMe), HtmlSpecialChars.Encode(poll.PollText).UnmarkupUsernames()));
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.pollNotFound));
															}
															break;
														default:
															notTheRightStart = true;
															break;
													}
												}
											} 
											if (update.Message.Text == "/start" || update.Message.Text == "/start from_inline" || notTheRightStart) {
												if (pointer == null) {
													pointerContainer.Add(update.Message.From.Id);
													pointer = pointerContainer.GetPointer(update.Message.From.Id);
												} else {
													pointer.Needle = ENeedle.pollText;
												}
												strings.SetLanguage(pointer.Lang);
												if (pointer.Lang == Strings.langs.none) {
													LangMessage.Send(apikey, strings, pointer);
												} else {
													WelcomeMessage.Send(apikey, strings, pointer);
												}
											} 
											break;
										case "cancel":
											if (pointer.Needle == ENeedle.firstOption || pointer.Needle == ENeedle.furtherOptions) {
												pollContainer.RemoveLastPoll(update.Message.From.Id);
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.hasCanceled));
											} else if (pointer.Needle != ENeedle.nothing) {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.canceledThat));
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.nothingToCancel));
											}
											pointer.Needle = ENeedle.nothing;
											break;
										case "list":
											Api.SendMessage(apikey, update.Message.Chat.Id, pollContainer.GetPollPretty(update.Message.From.Id));
											break;
										case "deleteall":
											Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.seriouslyDeleteEverything), replyMarkup:new InlineKeyboardMarkup {
												InlineKeyboard = new List<List<InlineKeyboardButton>>{
												new List<InlineKeyboardButton>{
													InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.sure), callbackData:"comm:deleteallsure"),
													InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.no), callbackData:"comm:deleteallno"),
												}
											}});
											break;
										case "donate":
											Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.stringsList.donation),"1EPxtfGVCGp8NP3oF8kFqJG74D9AsWiqQ"));
										break;
										case "lang":
											LangMessage.Send(apikey, strings, pointer);
											break;
										case "debug_missing_strings":
											var missingStrings = strings.GetMissingStrings();
											for (int i = 0; i < missingStrings.Count; i++) {
												if (missingStrings.ElementAt(i).Value.Count > 0)
													Notifications.log(missingStrings.Keys.ElementAt(i).ToString() + " " + CustomJsonStuff.JsonEnhancer.FormatJson(JsonConvert.SerializeObject(missingStrings.ElementAt(i).Value)));
											}
											break;
										case "debug_strings_en":
											Notifications.log(CustomJsonStuff.JsonEnhancer.FormatJson(JsonConvert.SerializeObject(strings.StringsEn)));
											break;
										case "crc32":
											Api.SendMessage(apikey, update.Message.Chat.Id, "/" + ((update.Message.From.FirstName + update.Message.From.LastName).HashCRC32()));
											break;
										case "base53":
											Api.SendMessage(apikey, update.Message.Chat.Id, string.Format("/{0}", HttpServerUtility.UrlTokenEncode(Encoding.GetEncoding("UTF-8").GetBytes(string.Format("13:5:{0}", "hey there".HashCRC32()).ToCharArray()))));
											break;
										default:
											int editPoll;
											if (int.TryParse(command, out editPoll)) {
												try {
													pollContainer.GetPoll(update.Message.From.Id, editPoll).Send(apikey, strings, update.Message.Chat.Id);
												} catch (ArgumentOutOfRangeException) {
													Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.pollDoesntExist));
												}
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.unrecognizedCommand));
											}
											break;
									}
								} else {
									switch (pointer.Needle) {
										case ENeedle.nothing:
											Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.wannaCreate));
											break;
										case ENeedle.pollText:
											pointer.LastPollId++;
											dBHandler.AddToQueue(pollContainer.Add(pointer, update.Message.From.Id, update.Message.Text), false);
											Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.stringsList.creatingPoll),HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()), replyMarkup: new InlineKeyboardMarkup { InlineKeyboard = new List<List<InlineKeyboardButton>> { new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.skipThisStep), callbackData: "comm:skip:description") } } });
											pointer.Needle = ENeedle.pollDescription;
											break;
										case ENeedle.pollDescription:
											Api.SendMessage(apikey, update.Message.Chat.Id, String.Format((pointer.PollType != EPolls.board ? strings.GetString(Strings.stringsList.createPoll) : strings.GetString(Strings.stringsList.createBoard)), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()));
											try {
												pollContainer.GetLastPoll(pointer).AddDescription(update.Message.Text);
												if (pointer.PollType == EPolls.board) {
													pollContainer.GetLastPoll(pointer).Send(apikey, strings, update.Message.Chat.Id);
													pointer.Needle = ENeedle.nothing;
												} else {
													pointer.Needle = ENeedle.furtherOptions;
													dBHandler.AddToQueue(pointer);
												}
											} catch (Exception e) {
												Notifications.log(e + "\n\n\n" + JsonConvert.SerializeObject(pointer));
												continue;
											}
											break;
										case ENeedle.firstOption:
										case ENeedle.furtherOptions:
											if (pollContainer.GetLastPoll(pointer).AddOption(update.Message.Text))
												Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.stringsList.addedToPoll),HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()), replyMarkup: InlineMarkupGenerator.GetOneButtonMarkup(InlineKeyboardButton.Create("\ud83d\udcbe " + strings.GetString(Strings.stringsList.done), callbackData: "comm:done")));
											else
												Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.stringsList.alreadyDefined),HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()), replyMarkup: InlineMarkupGenerator.GetOneButtonMarkup(InlineKeyboardButton.Create("\ud83d\udcbe " + strings.GetString(Strings.stringsList.done), callbackData: "comm:done")));
											pointer.Needle = ENeedle.furtherOptions;
											break;
										case ENeedle.board: {
											if (update.Message.Text.Length <= 120) {
												update.Message.Text = update.Message.Text.Replace("\n", " ");
												try {
													Poll poll = pollContainer.GetPoll((int)pointer.BoardChatId, (int)pointer.BoardPollId);
													poll.Vote(apikey, 0, update.Message.From, update.Message);
													Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.stringsList.boardSuccess), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames(), HtmlSpecialChars.Encode(poll.PollText.Truncate(25)).UnmarkupUsernames()));
												} catch (Exception e) {
													Api.SendMessage(apikey, update.Message.Chat.Id, "Sorry, an error occured...\nPlease try again...");
													Notifications.log(e.ToString() + "\n\nThe board db error...\nAgain...");
												}
												pointer.Needle = ENeedle.nothing;
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.stringsList.tooManyCharacters),update.Message.Text.Length));
											}
										} break;
										case ENeedle.addOption: {
											if (update.Message.Text.Length <= 120) {
													update.Message.Text = update.Message.Text.Replace("\n", " ");
													try {
														Poll poll = pollContainer.GetPoll((int)pointer.BoardChatId, (int)pointer.BoardPollId);
														poll.AddOption(update.Message.Text);
														dBHandler.AddToQueue(poll);
														Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.stringsList.addOptionSuccess), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames(), HtmlSpecialChars.Encode(poll.PollText.Truncate(25)).UnmarkupUsernames()));
													} catch (Exception e) {
														Api.SendMessage(apikey, update.Message.Chat.Id, "Sorry, an error occured...\nPlease try again...");
														Notifications.log(e.ToString() + "\n\nThe add option db error...\nAgain...");
													}
													pointer.Needle = ENeedle.nothing;
												} else {
													Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.stringsList.addOptionTooManyChars), update.Message.Text.Length));
												}	
										} break;
										case ENeedle.limitedDoodleMaxVotes:
											LimitedDoodle doodle = (LimitedDoodle)pollContainer.GetLastPoll(pointer);
											if (int.TryParse(update.Message.Text, out doodle.MaxVotes)) {
												if (doodle.MaxVotes <= doodle.PollVotes.Count && doodle.MaxVotes > 0) {
													pointer.Needle = ENeedle.nothing;
													doodle.Send(apikey, strings, update.Message.Chat.Id);
												} else {
													Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.stringsList.limitedDoodleGiveMeANumberSmallerOrEqualToOptionCount), doodle.PollVotes.Count));
												}
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.stringsList.limitedDoodleGiveMeANumberNothingElse));
											}
											break;
									}
								}
							}
						} else if (update.InlineQuery != null) {
							List<InlineQueryResult> list = new List<InlineQueryResult>();
							bool channel = false;
							string pollText = update.InlineQuery.Query;
							if (update.InlineQuery.Query.StartsWith("$", StringComparison.CurrentCulture)) {
								if (update.InlineQuery.Query.Length >= 4)
									pollText = update.InlineQuery.Query.Substring(3);
								char command = (char)0;
								if (update.InlineQuery.Query.Length >= 2)
									command = update.InlineQuery.Query.ToCharArray(1, 1)[0];
								switch (command) {
									case 'c':
										channel = true;
										break;
								}
							}
							Pointer pointer = pointerContainer.GetPointer(update.InlineQuery.From.Id);
							if (pointer != null)
								strings.SetLanguage(pointer.Lang);
							else 
								strings.SetLanguage(Strings.langs.none);
							pollContainer.GetPollsReverse(update.InlineQuery.From.Id, 50, pollText).ForEach(x => {
								InlineQueryResult result = x.Result(strings, apikey, channel);
								if (result != null)
									list.Add(result);
							});
							Api.AnswerInlineQuery(apikey, update.InlineQuery.Id, list, 0, true, strings.GetString(Strings.stringsList.inlineTextCreateNewPoll), "from_inline");
						} else if (update.CallbackQuery != null) {
							if (update.CallbackQuery.Data.StartsWith("comm:", StringComparison.CurrentCulture)) {
								Pointer pointer = pointerContainer.GetPointer(update.CallbackQuery.From.Id);
								if (pointer != null)
									strings.SetLanguage(pointer.Lang);
								else 
									strings.SetLanguage(Strings.langs.none);
								string command = update.CallbackQuery.Data.Substring(5);
								string realCommand;
								if (command.Contains(":")) {
									realCommand = command.Split(':')[0];
								} else
									realCommand = command;
								string text = "";
								bool refresh = false;
								bool alert = false;
								switch (realCommand) {
									case "anony":
										if (pointer.Anony != EAnony.anonymous) {
											pointer.Anony = EAnony.anonymous;
											refresh = true;
										}
										break;
									case "pers":
										if (pointer.Anony != EAnony.personal) {
											pointer.Anony = EAnony.personal;
											refresh = true;
										}
										break;
									case "vote":
										pointer.PollType = EPolls.vote;
										refresh = true;
										break;
									case "doodle":
										pointer.PollType = EPolls.doodle;
										refresh = true;
										break;
									case "board":
										pointer.PollType = EPolls.board;
										refresh = true;
										break;
									case "limitedDoodle":
										pointer.PollType = EPolls.limitedDoodle;
										refresh = true;
										break;
									case "done":
										if (pointer.Needle == ENeedle.furtherOptions) {
											if (pollContainer.GetLastPoll(pointer).PollType == EPolls.limitedDoodle) {
												Api.EditMessageText(apikey, strings.GetString(Strings.stringsList.limitedDoodleEnterMaxVotes), null, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
												pointer.Needle = ENeedle.limitedDoodleMaxVotes;
											} else {
												pollContainer.GetLastPoll(pointer).Send(apikey, strings, update.CallbackQuery.From.Id);
												pointer.Needle = ENeedle.nothing;
												text = strings.GetString(Strings.stringsList.finishedPollCreation);
											}
										} else {
											text = strings.GetString(Strings.stringsList.noPollsToFinish);
										}
										break;
									case "close":
										string[] splitCloseData = command.Split(':');
										pollContainer.GetPoll(int.Parse(splitCloseData[1]), int.Parse(splitCloseData[2])).Close(apikey, strings, update.CallbackQuery.Message.MessageId);
										break;
									case "reopen":
										string[] reopenData = command.Split(':');
										pollContainer.GetPoll(int.Parse(reopenData[1]), int.Parse(reopenData[2])).Reopen(apikey, strings, update.CallbackQuery.Message.MessageId);
										break;
									case "delete":
										string[] splitData = command.Split(':');
										string pollText = pollContainer.GetPoll(int.Parse(splitData[1]), int.Parse(splitData[2])).PollText;
										Api.EditMessageText(apikey, String.Format(strings.GetString(Strings.stringsList.seriouslyWannaDeleteThePoll),HtmlSpecialChars.Encode(pollText).UnmarkupUsernames()), new InlineKeyboardMarkup{
											InlineKeyboard = new List<List<InlineKeyboardButton>>{
												new List<InlineKeyboardButton>{
													InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.sure), callbackData:"comm:sure" + command),
													InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.no), callbackData:"comm:no" + command),
												}
											}
										}, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										break;
									case "suredelete":
										string[] splitDeleteData = command.Split(':');
										if(update.CallbackQuery.From.Id == int.Parse(splitDeleteData[1]))
											pollContainer.GetPoll(int.Parse(splitDeleteData[1]), int.Parse(splitDeleteData[2])).Delete(apikey, strings, update.CallbackQuery.Message.MessageId);
										break;
									case "nodelete":
										string[] splitNoDeleteData = command.Split(':');
										if (update.CallbackQuery.From.Id == int.Parse(splitNoDeleteData[1]))
											pollContainer.GetPoll(int.Parse(splitNoDeleteData[1]), int.Parse(splitNoDeleteData[2])).Update(apikey, strings, update.CallbackQuery.Message.MessageId, "", update.CallbackQuery.Message.Chat.Id, false);
										break;
									case "deleteallsure":
										Api.EditMessageText(apikey, strings.GetString(Strings.stringsList.seriouslySureDeleteEverything), new InlineKeyboardMarkup {
											InlineKeyboard = new List<List<InlineKeyboardButton>>{
												new List<InlineKeyboardButton>{
													InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.sure), callbackData:"comm:seriouslysuredeleteall"),
													InlineKeyboardButton.Create(strings.GetString(Strings.stringsList.no), callbackData:"comm:deleteallno"),
												}
											}
										}, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, update.CallbackQuery.InlineMessageId);
										break;
									case "seriouslysuredeleteall":
										List<Poll> allPolls = pollContainer.GetPolls(update.CallbackQuery.From.Id);
										foreach (Poll poll in allPolls) {
											poll.DeleteFromDeleteAll(apikey,strings);
										}
										Api.EditMessageText(apikey, strings.GetString(Strings.stringsList.deletedEverything), chatId:update.CallbackQuery.Message.Chat.Id, messageId:update.CallbackQuery.Message.MessageId, inlineMessageId:update.CallbackQuery.InlineMessageId);
										break;
									case "deleteallno":
										Api.EditMessageText(apikey, strings.GetString(Strings.stringsList.sadlyEverythingThere), chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.MessageId, inlineMessageId: update.CallbackQuery.InlineMessageId);
										break;
									case "update":
										string[] splitUpdateData = command.Split(':');
										pollContainer.GetPoll(int.Parse(splitUpdateData[1]), int.Parse(splitUpdateData[2])).Update(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
										break;
									case "options": {
										string[] splitOptionsData = command.Split(':');
										if (splitOptionsData[1] == update.CallbackQuery.From.Id.ToString())
											pollContainer.GetPoll(int.Parse(splitOptionsData[1]), int.Parse(splitOptionsData[2])).UpdateWithOptionsPane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
									} break;
									case "percentage": {
										string[] splitOptionsData = command.Split(':');
										if (splitOptionsData[2] == update.CallbackQuery.From.Id.ToString()) {
											Poll poll = pollContainer.GetPoll(int.Parse(splitOptionsData[2]), int.Parse(splitOptionsData[3]));
											poll.SetPercentage((PercentageBars.Bars)Enum.Parse(typeof(PercentageBars.Bars), splitOptionsData[1]));
					                        poll.UpdateWithOptionsPane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
										}
									} break;
									case "sorted":
									case "unsorted": {
										string[] splitOptionsData = command.Split(':');
											if (splitOptionsData[1] == update.CallbackQuery.From.Id.ToString()) {
												Poll poll = pollContainer.GetPoll(int.Parse(splitOptionsData[1]), int.Parse(splitOptionsData[2]));
												poll.SetSorted(!poll.Sorted);
												poll.UpdateWithOptionsPane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
											}
									} break;
									case "appendable":
									case "unappendable": {
											string[] splitOptionsData = command.Split(':');
											if (splitOptionsData[1] == update.CallbackQuery.From.Id.ToString()) {
												Poll poll = pollContainer.GetPoll(int.Parse(splitOptionsData[1]), int.Parse(splitOptionsData[2]));
												poll.SetAppendable(!poll.Appendable);
												poll.UpdateWithOptionsPane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
											}
										}
										break;
									case "moderate": {
											string[] splitModerateData = command.Split(':');
											if (splitModerateData[1] == update.CallbackQuery.From.Id.ToString()) {
												Poll poll = pollContainer.GetPoll(int.Parse(splitModerateData[1]), int.Parse(splitModerateData[2]));
												poll.UpdateWithModeratePane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
											}
										//TODO:Add moderation function
									} break;
									case "skip":
										string stuff = command.Split(':')[1];
										if (stuff == "description") {
											pointer.Needle = ENeedle.firstOption;
											if (pointer.PollType == EPolls.board) {
												pollContainer.GetLastPoll(pointer).Send(apikey, strings, update.CallbackQuery.Message.Chat.Id);
												pointer.Needle = ENeedle.nothing;
											} else {
												Api.EditMessageText(apikey, (pointer.PollType == EPolls.board ? strings.GetString(Strings.stringsList.roger) : strings.GetString(Strings.stringsList.rogerSendIt)), chatId: update.CallbackQuery.Message.Chat.Id, messageId:update.CallbackQuery.Message.MessageId);
											}
										}
										break;
									case "iwannavote":
										try {
											string[] splitVoteData = Cryptography.Decrypt(command.Substring(11), apikey).Split(':');
											pollContainer.GetPoll(int.Parse(splitVoteData[0]), int.Parse(splitVoteData[1])).Update(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text, vote: true);
										} catch (System.FormatException) {
											string[] splitVoteData = command.Split(':');
											pollContainer.GetPoll(int.Parse(splitVoteData[1]), int.Parse(splitVoteData[2])).Update(apikey,strings,update.CallbackQuery.Message.MessageId,"",update.CallbackQuery.Message.Chat.Id);
											text = strings.GetString(Strings.stringsList.updatingPoll);
											alert = true;
										}

										break;
									case "pag":
										string[] splitPagData = command.Split(':');
										pollContainer.GetPoll(int.Parse(splitPagData[1]), int.Parse(splitPagData[2])).Update(apikey, strings, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, int.Parse(splitPagData[3]));
										break;
									case "lang":
										string[] langSplit = command.Split(':');
										strings.SetLanguage(pointer.Lang = (Strings.langs)Enum.Parse(typeof(Strings.langs), langSplit[1]));
										LangMessage.Refresh(apikey, strings, pointer, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										break;
									case "showWelcome":
										pointer.Needle = ENeedle.pollText;
										WelcomeMessage.Refresh(apikey, strings, pointer, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										break;
								}
								Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, text, alert);
								if (refresh)
									WelcomeMessage.Refresh(apikey, strings, pointer, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
							} else if (update.CallbackQuery.Data.StartsWith("page:", StringComparison.CurrentCulture)) {
								Pointer pointer = pointerContainer.GetPointer(update.CallbackQuery.From.Id);
								if (pointer != null)
									strings.SetLanguage(pointer.Lang);
								else
									strings.SetLanguage(Strings.langs.none);
								string page = update.CallbackQuery.Data.Substring(5);
								Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id);
								switch (page) {
									case "chpoll":
										InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup();
										inlineKeyboard.InlineKeyboard = new List<List<InlineKeyboardButton>>{
											new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.inlineButtonVote),(pointer.PollType == EPolls.vote ? "✅" : "☑️")), callbackData: "comm:vote") },
											new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.inlineButtonDoodle), (pointer.PollType == EPolls.doodle ? "✅" : "☑️")), callbackData: "comm:doodle") },

											new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.inlineButtonLimitedDoodle), (pointer.PollType == EPolls.limitedDoodle ? "✅" : "☑️")), callbackData: "comm:limitedDoodle") },
											new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.stringsList.inlineButtonBoard), (pointer.PollType == EPolls.board ? "✅" : "☑️")), callbackData: "comm:board") },
										};
										Api.EditMessageText(apikey, strings.GetString(Strings.stringsList.pollTypeDescription), inlineKeyboard, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										break;
								}
							} else {
								Pointer pointer = pointerContainer.GetPointer(update.CallbackQuery.From.Id);
								if (pointer != null)
									strings.SetLanguage(pointer.Lang);
								else
									strings.SetLanguage(Strings.langs.none);
								string[] splitData = null;
								bool onlyUpdate = false;
								try {
									splitData = Cryptography.Decrypt(update.CallbackQuery.Data, apikey).Split(':');
								} catch (System.FormatException) {
									splitData = update.CallbackQuery.Data.Split(':');
									onlyUpdate = true;
								}
								Poll poll;
								try {
									poll = pollContainer.GetPoll(int.Parse(splitData[0]), int.Parse(splitData[1]));
									if (onlyUpdate) {
										dBHandler.AddToQueue(poll);
										Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, "Updating...", true);
									} else if (poll.Closed) {
										Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, strings.GetString(Strings.stringsList.tryVotePollClosed));
									} else {
										if (poll.GetType() == typeof(LimitedDoodle)) {
											LimitedDoodle doodle = (LimitedDoodle)poll;
											bool tooMuchVotes;
											if (doodle.Vote(apikey, int.Parse(splitData[2]), update.CallbackQuery.From, update.CallbackQuery.Message, out tooMuchVotes, update.CallbackQuery.InlineMessageId)) {
												Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.stringsList.callbackVotedFor), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key));
											} else {
												if (tooMuchVotes)
													Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.stringsList.callbackLimitedMaxReached), doodle.MaxVotes));
												else
													Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.stringsList.callbackTookBack), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key));
											}
										} else {
											if (poll.Vote(apikey, int.Parse(splitData[2]), update.CallbackQuery.From, update.CallbackQuery.Message, update.CallbackQuery.InlineMessageId)) {
												Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.stringsList.callbackVotedFor), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key));
											} else {
												Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.stringsList.callbackTookBack), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key));
											}
										}
									}
									if (update.CallbackQuery.InlineMessageId == null)
										poll.Update(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text, update.CallbackQuery.Message.Chat.Id);
								} catch (Exception) {
									Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, strings.GetString(Strings.stringsList.voteDoesntExist));
								}
							}
						} else if (update.ChosenInlineResult != null) {
							string[] splitData = update.ChosenInlineResult.ResultId.Split(':');
							pollContainer.GetPoll(int.Parse(splitData[0]), int.Parse(splitData[1])).AddToMessageIDs(new MessageID {
								inlineMessageId = update.ChosenInlineResult.InlineMessageId,
								channel = update.ChosenInlineResult.Query.StartsWith("$c:", StringComparison.CurrentCulture),
							});
						}
					}
				}
				dBHandler.FlushToDB(strings);
			}
		}
	}
}