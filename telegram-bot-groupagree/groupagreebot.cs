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
using System.Threading.Tasks;

namespace telegrambotgroupagree {
	public class GroupAgreeBot
	{
		private List<Instance> instances;
		private List<Task<Update[]>> instanceTasks;
		private PointerContainer pointerContainer;
		private PollContainer pollContainer;
		private DBHandler dBHandler;
		private Strings strings;

		public User BotInfo { get; internal set; }
		public Pointer CurrentPointer { get; internal set; }
		public Update CurrentUpdate { get; internal set; }

		public GroupAgreeBot(string dbName, string dbUser, string dbPassword) {
			instances = new List<Instance>();
			instanceTasks = new List<Task<Update[]>>();
			try {
				dBHandler = new DBHandler(dbName, dbUser, dbPassword);
			} catch (Exception e) {
				Notifications.log("SHIT\n\n" + e.ToString());
			}
			string instancesLog = "Starting instances:\n\n";
			List<Instance> constructorInstances = dBHandler.GetInstances();
			foreach (Instance currentLoopInstance in constructorInstances) {
				Task<Update[]> currentUpdate;
				User currentBotUser;
				try {
					Api.DeleteWebhook(currentLoopInstance.apikey);
					if ((currentBotUser = Api.GetMe(currentLoopInstance.apikey)) == null) {
						continue;
					}
					currentUpdate = Api.GetUpdatesAsync(currentLoopInstance.apikey, currentLoopInstance.offset);
				} catch (Exception e) {
					//TODO What exception?
					Notifications.log("Instances setup fail: \n" + e.ToString());
					continue;
				}
				this.instances.Add(new Instance {
					apikey = currentLoopInstance.apikey,
					offset = currentLoopInstance.offset,
					botUser = currentBotUser,
					creator = currentLoopInstance.creator,
					update = currentUpdate,
				});
				this.instanceTasks.Add(currentUpdate);
				instancesLog += $"{currentBotUser.FirstName} (@{currentBotUser.Username})\n\n";
			}
			Notifications.log(instancesLog);
			pointerContainer = new PointerContainer(dBHandler);
			strings = new Strings();
			pollContainer = new PollContainer(dBHandler, strings);
		}

		public override string ToString() {
			return JsonConvert.SerializeObject(BotInfo);
		}

		public async void Run() {
			while (!System.IO.File.Exists(@"cancer.wjdummy") && instances != null && instances.Count > 0) {
				Instance currentInstance = instances[Task.WaitAny(instances.Select(x => x.update).ToArray())];
				Update[] updates = await currentInstance.update;
				string apikey = currentInstance.apikey;
				int offset = currentInstance.offset;
				Globals.GlobalOptions.Apikey = currentInstance.apikey;
				Globals.GlobalOptions.Botname = currentInstance.botUser.Username;
				if (updates != null) {
					foreach (Update update in updates) {
						CurrentUpdate = update;
						currentInstance.offset = offset = update.UpdateId + 1;
						dBHandler.UpdateInstance(currentInstance.chatID, currentInstance.offset, currentInstance.last30Updates);
						if (update.Message != null) {
							Pointer pointer = pointerContainer.GetPointer(update.Message.From.Id, update.Message.From.LanguageCode);
							if (pointer != null)
								strings.SetLanguage(pointer.Lang);
							else
								strings.SetLanguage(Strings.Langs.none);
							if (update.Message.Text != null) {
								if (pointer == null && !update.Message.Text.StartsWith("/start", StringComparison.CurrentCulture)) {
									Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.setupPlease));
									continue;
								}
								if (update.Message.Entities != null && update.Message.Entities.Length > 0 && update.Message.Entities[0].Type == EEntityType.bot_command && update.Message.Entities[0].Offset == 0) {
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
													try {
														payload = update.Message.Text.Substring(7).Split(':');
													} catch (IndexOutOfRangeException) {
														notTheRightStart = true;
													}
													if (payload == null || payload.Length <= 1)
														notTheRightStart = true;
												}
												Poll poll;
												if (onlyUpdate && !notTheRightStart) {
													Console.WriteLine(update.Message.Text);
													try {
														dBHandler.AddToQueue(pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2])));
														Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.updatingPoll));
													} catch (FormatException) {
														notTheRightStart = true;
													} catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is IndexOutOfRangeException) {
														Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.pollNotFound));
													}
												} else {
													switch (payload[0]) {
														case "board":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id, update.Message.From.LanguageCode);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															pointer.BoardChatId = int.Parse(payload[1]);
															pointer.BoardPollId = int.Parse(payload[2]);
															pointer.Needle = ENeedle.board;
															poll = pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2]));
															try {
																Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.StringsList.boardAnswer), HtmlSpecialChars.Encode(poll.PollText).UnmarkupUsernames()));
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.pollNotFound));
															}
															break;
														case "vote":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id, update.Message.From.LanguageCode);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															pointer.BoardChatId = int.Parse(payload[1]);
															pointer.BoardPollId = int.Parse(payload[2]);
															pointer.Needle = ENeedle.nothing;
															try {
																pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2])).Send(apikey, strings, update.Message.Chat.Id, true);
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.pollNotFound));
															}
															break;
														case "pag":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id, update.Message.From.LanguageCode);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															try {
																pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2])).Send(apikey, strings, update.Message.Chat.Id, int.Parse(payload[3]));
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.pollNotFound));
															}
															break;
														case "append":
															if (pointer == null) {
																pointer = new Pointer(update.Message.From.Id, update.Message.From.LanguageCode);
																pointerContainer.Add(pointer);
																strings.SetLanguage(pointer.Lang);
															}
															try {
																poll = pollContainer.GetPoll(int.Parse(payload[1]), int.Parse(payload[2]));
																pointer.BoardChatId = int.Parse(payload[1]);
																pointer.BoardPollId = int.Parse(payload[2]);
																pointer.Needle = ENeedle.addOption;
																Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.StringsList.appendSendMe), HtmlSpecialChars.Encode(poll.PollText).UnmarkupUsernames()));
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.pollNotFound));
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
													pointerContainer.Add(update.Message.From.Id, update.Message.From.LanguageCode);
													pointer = pointerContainer.GetPointer(update.Message.From.Id, update.Message.From.LanguageCode);
												} else {
													pointer.Needle = ENeedle.pollText;
												}
												strings.SetLanguage(pointer.Lang);
												if (pointer.Lang == Strings.Langs.none) {
													LangMessage.Send(apikey, strings, pointer);
												} else {
													WelcomeMessage.Send(apikey, strings, pointer);
												}
											}
											break;
										case "cancel":
											if (pointer.Needle == ENeedle.firstOption || pointer.Needle == ENeedle.furtherOptions) {
												pollContainer.RemoveLastPoll(update.Message.From.Id);
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.hasCanceled));
											} else if (pointer.Needle != ENeedle.nothing) {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.canceledThat));
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.nothingToCancel));
											}
											pointer.Needle = ENeedle.nothing;
											break;
										case "list":
											Api.SendMessage(apikey, update.Message.Chat.Id, pollContainer.GetPollPretty(update.Message.From.Id));
											break;
										case "deleteall":
											Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.seriouslyDeleteEverything), replyMarkup: new InlineKeyboardMarkup {
												InlineKeyboard = new List<List<InlineKeyboardButton>>{
											new List<InlineKeyboardButton>{
												InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.sure), callbackData:"comm:deleteallsure"),
												InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.no), callbackData:"comm:deleteallno"),
											}
										}
											});
											break;
										case "donate":
											Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.donation), "1EPxtfGVCGp8NP3oF8kFqJG74D9AsWiqQ"));
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
										case "addinstance":
											if(update.Message.From.Id == 133909606) {
												pointer.Needle = ENeedle.addInstanceToken;
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.addInstanceSendToken));
											} break;
										/*case "testurl":
											Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, "INTERNAL TEST MESSAGE", replyMarkup: InlineMarkupGenerator.GetOneButtonMarkup(InlineKeyboardButton.Create("TEST THIS NOW", callbackData: "comm:url:t.me/" + Globals.GlobalOptions.Botname + "?start=from_inline")));
											break;*/
										/*case "burstme":
											for (int i = 0; i < 40; i++) {
												Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, "INTERNAL TEST MESSAGE NO " + i);
											}
											break;*/
										default:
											int editPoll;
											if (int.TryParse(command, out editPoll)) {
												try {
													pollContainer.GetPoll(update.Message.From.Id, editPoll).Send(apikey, strings, update.Message.Chat.Id);
												} catch (ArgumentOutOfRangeException) {
													Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.pollDoesntExist));
												}
											} else if (command.StartsWith("delete_", StringComparison.CurrentCulture)) {
												try {
													string[] commandSplit = command.Substring(7).Base53Decode().Split(':');
													if (commandSplit.Length != 3)
														throw new FormatException();
													if (int.TryParse(commandSplit[0], out int pollID)) {
														Poll poll = pollContainer.GetPoll(update.Message.From.Id, pollID);
														if (int.TryParse(commandSplit[1], out int optionID)) {
															try {
																string optionText = poll.GetOptionText(optionID);
																try {
																	if (poll.DeleteOption(optionID, commandSplit[2])) {
																		Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.StringsList.optionDeleteSuccess), poll.PollText, optionText), replyMarkup: InlineMarkupGenerator.GetTheModerationKeyboard(strings, update.Message.From.Id, pollID));
																	} else {
																		Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.aPollHasToHaveAtLeastOneOption));
																	}
																} catch (FormatException) {
																	Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.pollOptionDeleteErrorButPollStillFound), optionText));
																}
															} catch (ArgumentOutOfRangeException) {
																Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.optionNotFound), replyMarkup: InlineMarkupGenerator.GetTheModerationKeyboard(strings, update.Message.From.Id, pollID));
															}
														}
													} else {
														throw new System.FormatException();
													}
												} catch (System.FormatException) {
													Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.dontMessWithTheCommands));
												} catch (ArgumentOutOfRangeException) {
													Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.pollDoesntExist));
												}
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.unrecognizedCommand));
											}
											break;
									}
								} else {
									switch (pointer.Needle) {
										case ENeedle.nothing:
											Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.wannaCreate));
											break;
										case ENeedle.pollText:
											pointer.LastPollId++;
											dBHandler.AddToQueue(pollContainer.Add(pointer, update.Message.From.Id, update.Message.Text), false);
											//TODO Kinda test this shit
											//Board removed
											/*if (pointer.PollType == EPolls.board) {
												Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.StringsList.creatingBoardWannaAddDescription), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()), replyMarkup: new InlineKeyboardMarkup { InlineKeyboard = new List<List<InlineKeyboardButton>> { new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.addDescription), callbackData: "comm:add:description") }, new List<InlineKeyboardButton> { InlineKeyboardButton.Create(EmojiStore.Done + " " + strings.GetString(Strings.StringsList.done), callbackData: "comm:boarddone:" + pointer.ChatId + ":" + pointer.LastPollId) } } });
												pointer.Needle = ENeedle.nothing;
											} else {*/
												Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.StringsList.creatingPollWannaAddDescription), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()), replyMarkup: new InlineKeyboardMarkup { InlineKeyboard = new List<List<InlineKeyboardButton>> { new List<InlineKeyboardButton> { InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.addDescription), callbackData: "comm:add:description") } } });
												pointer.Needle = ENeedle.firstOption;
											/*}*/
											break;
										case ENeedle.pollDescription:
											Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(/*(pointer.PollType != EPolls.board ? */strings.GetString(Strings.StringsList.createPoll)/* : strings.GetString(Strings.StringsList.createBoard))*/, HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()));
											try {
												pollContainer.GetLastPoll(pointer).AddDescription(update.Message.Text);
												/*if (pointer.PollType == EPolls.board) {
													pollContainer.GetLastPoll(pointer).Send(apikey, strings, update.Message.Chat.Id);
													pointer.Needle = ENeedle.nothing;
												} else {*/
													pointer.Needle = ENeedle.furtherOptions;
													dBHandler.AddToQueue(pointer);
												/*}*/
											} catch (Exception e) {
												Notifications.log(e + "\n\n\n" + JsonConvert.SerializeObject(pointer));
												continue;
											}
											break;
										case ENeedle.firstOption:
										case ENeedle.furtherOptions:
											if (pollContainer.GetLastPoll(pointer).AddOption(update.Message.Text))
												Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.addedToPoll), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()), replyMarkup: InlineMarkupGenerator.GetOneButtonMarkup(InlineKeyboardButton.Create("\ud83d\udcbe " + strings.GetString(Strings.StringsList.done), callbackData: "comm:done")));
											else
												Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.alreadyDefined), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames()), replyMarkup: InlineMarkupGenerator.GetOneButtonMarkup(InlineKeyboardButton.Create("\ud83d\udcbe " + strings.GetString(Strings.StringsList.done), callbackData: "comm:done")));
											pointer.Needle = ENeedle.furtherOptions;
											break;
										/* not existent anymore...
										case ENeedle.board: {
											if (update.Message.Text.Length <= 120) {
												update.Message.Text = update.Message.Text.Replace("\n", " ");
												try {
													Poll poll = pollContainer.GetPoll((int)pointer.BoardChatId, (int)pointer.BoardPollId);
													poll.Vote(apikey, 0, update.Message.From, update.Message);
													Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.boardSuccess), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames(), HtmlSpecialChars.Encode(poll.PollText.Truncate(25)).UnmarkupUsernames()));
												} catch (Exception e) {
													Api.SendMessage(apikey, update.Message.Chat.Id, "Sorry, an error occured...\nPlease try again...");
													Notifications.log(e.ToString() + "\n\nThe board db error...\nAgain...");
												}
												pointer.Needle = ENeedle.nothing;
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.tooManyCharacters), update.Message.Text.Length));
											}
										}
										break; */
										case ENeedle.addOption: {
											if (update.Message.Text.Length <= 120) {
												update.Message.Text = update.Message.Text.Replace("\n", " ");
												try {
													Poll poll = pollContainer.GetPoll((int)pointer.BoardChatId, (int)pointer.BoardPollId);
													if (poll.PollVotes.Keys.Any((arg) => arg.RemoveAppendingText() != update.Message.Text)) {
														poll.AddOption("//BY:" + update.Message.From.Id + "//" + update.Message.Text);
														dBHandler.AddToQueue(poll);
														Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.addOptionSuccess), HtmlSpecialChars.Encode(update.Message.Text).UnmarkupUsernames(), HtmlSpecialChars.Encode(poll.PollText.Truncate(25)).UnmarkupUsernames()));
													} else {
														Api.SendMessage(Globals.GlobalOptions.Apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.optionAlreadyExists));
													}
												} catch (Exception e) {
													Api.SendMessage(apikey, update.Message.Chat.Id, "Sorry, an error occured...\nPlease try again...");
													Notifications.log(e.ToString() + "\n\nThe add option db error...\nAgain...");
												}
												pointer.Needle = ENeedle.nothing;
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, String.Format(strings.GetString(Strings.StringsList.addOptionTooManyChars), update.Message.Text.Length));
											}
										}
										break;
										case ENeedle.limitedDoodleMaxVotes:
											LimitedDoodle doodle = (LimitedDoodle)pollContainer.GetLastPoll(pointer);
											if (int.TryParse(update.Message.Text, out doodle.MaxVotes)) {
												if (doodle.MaxVotes <= doodle.PollVotes.Count && doodle.MaxVotes > 0) {
													pointer.Needle = ENeedle.nothing;
													doodle.Send(apikey, strings, update.Message.Chat.Id);
												} else {
													Api.SendMessage(apikey, update.Message.Chat.Id, string.Format(strings.GetString(Strings.StringsList.limitedDoodleGiveMeANumberSmallerOrEqualToOptionCount), doodle.PollVotes.Count));
												}
											} else {
												Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.limitedDoodleGiveMeANumberNothingElse));
											}
											break;
										case ENeedle.addInstanceToken: {
											string newApikey = update.Message.Text; //TODO Make this more versatille
											User newBotUser = Api.GetMe(newApikey);
											Instance newInstance = new Instance {
												chatID = newBotUser.Id,
												apikey = newApikey,
												botUser = newBotUser,
												creator = update.Message.From,
												update = Api.GetUpdatesAsync(newApikey, 0),
											};
											dBHandler.AddInstance(newInstance.chatID, newInstance.key, newInstance.creator);
											instances.Add(newInstance);
											Api.SendMessage(apikey, update.Message.Chat.Id, strings.GetString(Strings.StringsList.addInstanceSetParameters), replyMarkup:InlineMarkupGenerator.GetOneButtonMarkup(InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.addInstanceToQueueButton))));
										} break;
									}
								}
							}
						} else if (update.InlineQuery != null) {
							//Notifications.log(JsonConvert.SerializeObject(update.InlineQuery));
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
							Pointer pointer = pointerContainer.GetPointer(update.InlineQuery.From.Id, update.InlineQuery.From.LanguageCode);
							if (pointer != null)
								strings.SetLanguage(pointer.Lang);
							else
								strings.SetLanguage(Strings.Langs.none);
							pollContainer.GetPollsReverse(update.InlineQuery.From.Id, 50, pollText).ForEach(x => {
								InlineQueryResult result = x.Result(strings, apikey, channel);
								if (result != null) {
									list.Add(result);
								}
							});
							Api.AnswerInlineQuery(apikey, update.InlineQuery.Id, list, 0, true, strings.GetString(Strings.StringsList.inlineTextCreateNewPoll), "from_inline");
						} else if (update.CallbackQuery != null) {
							if (update.CallbackQuery.Data.StartsWith("comm:", StringComparison.CurrentCulture)) {
								Pointer pointer = pointerContainer.GetPointer(update.CallbackQuery.From.Id, update.CallbackQuery.From.LanguageCode);
								if (pointer != null)
									strings.SetLanguage(pointer.Lang);
								else
									strings.SetLanguage(Strings.Langs.none);
								string command = update.CallbackQuery.Data.Substring(5);
								string realCommand;
								if (command.Contains(":")) {
									realCommand = command.Split(':')[0];
								} else {
									realCommand = command;
								}
								string text = "";
								bool refresh = false;
								bool alert = false;
								string returnUrl = null;
								switch (realCommand) {
									case "url": {
										returnUrl = "t.me/" + Globals.GlobalOptions.Botname + "?start=" + command.Substring(realCommand.Length + 1);
										break;
									}
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
									/*case "board":
										pointer.PollType = EPolls.board;
										refresh = true;
										break;*/
									case "limitedDoodle":
										pointer.PollType = EPolls.limitedDoodle;
										refresh = true;
										break;
									case "done":
										if (pointer.Needle == ENeedle.furtherOptions) {
											if (pollContainer.GetLastPoll(pointer).PollType == EPolls.limitedDoodle) {
												Api.EditMessageText(apikey, strings.GetString(Strings.StringsList.limitedDoodleEnterMaxVotes), null, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
												pointer.Needle = ENeedle.limitedDoodleMaxVotes;
											} else {
												pollContainer.GetLastPoll(pointer).Send(apikey, strings, update.CallbackQuery.From.Id);
												pointer.Needle = ENeedle.nothing;
												text = strings.GetString(Strings.StringsList.finishedPollCreation);
											}
										} else {
											text = strings.GetString(Strings.StringsList.noPollsToFinish);
										}
										break;
									/*case "boarddone": {
										string[] splitBoardDoneData = command.Split(':');
										Poll poll = pollContainer.GetPoll(int.Parse(splitBoardDoneData[1]), int.Parse(splitBoardDoneData[2]));
										poll.FinishCreation();
										poll.Update(instances, currentInstance.chatID, strings, messageId: update.CallbackQuery.Message.MessageId);
										break;
									}*/
									case "close":
										string[] splitCloseData = command.Split(':');
										pollContainer.GetPoll(int.Parse(splitCloseData[1]), int.Parse(splitCloseData[2])).Close(instances, currentInstance.chatID, strings, update.CallbackQuery.Message.MessageId);
										break;
									case "reopen":
										string[] reopenData = command.Split(':');
										pollContainer.GetPoll(int.Parse(reopenData[1]), int.Parse(reopenData[2])).Reopen(instances, currentInstance.chatID, strings, update.CallbackQuery.Message.MessageId);
										break;
									case "delete":
										string[] splitData = command.Split(':');
										try {
											string pollText = pollContainer.GetPoll(int.Parse(splitData[1]), int.Parse(splitData[2])).PollText;
											Api.EditMessageText(apikey, String.Format(strings.GetString(Strings.StringsList.seriouslyWannaDeleteThePoll), HtmlSpecialChars.Encode(pollText).UnmarkupUsernames()), new InlineKeyboardMarkup {
												InlineKeyboard = new List<List<InlineKeyboardButton>>{
											new List<InlineKeyboardButton>{
												InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.sure), callbackData:"comm:sure" + command),
												InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.no), callbackData:"comm:no" + command),
											}
										}
											}, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										} catch (ArgumentOutOfRangeException) {
											text = strings.GetString(Strings.StringsList.pollAlreadyDeleted);
											alert = true;
											Api.EditMessageReplyMarkup(apikey, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, replyMarkup: null);
										}
										break;
									case "suredelete":
										string[] splitDeleteData = command.Split(':');
										if (update.CallbackQuery.From.Id == int.Parse(splitDeleteData[1]))
											pollContainer.GetPoll(int.Parse(splitDeleteData[1]), int.Parse(splitDeleteData[2])).Delete(instances, currentInstance.chatID, strings, update.CallbackQuery.Message.MessageId);
										break;
									case "nodelete":
										string[] splitNoDeleteData = command.Split(':');
									if (update.CallbackQuery.From.Id == int.Parse(splitNoDeleteData[1]))
										pollContainer.GetPoll(int.Parse(splitNoDeleteData[1]), int.Parse(splitNoDeleteData[2])).Update(instances, currentInstance.chatID, strings, noApproximation: true, messageId:update.CallbackQuery.Message.MessageId, currentText:"", newChatId:update.CallbackQuery.Message.Chat.Id, vote:false);
										break;
									case "deleteallsure":
										Api.EditMessageText(apikey, strings.GetString(Strings.StringsList.seriouslySureDeleteEverything), new InlineKeyboardMarkup {
											InlineKeyboard = new List<List<InlineKeyboardButton>>{
											new List<InlineKeyboardButton>{
												InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.sure), callbackData:"comm:seriouslysuredeleteall"),
												InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.no), callbackData:"comm:deleteallno"),
											}
										}
										}, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, update.CallbackQuery.InlineMessageId);
										break;
									case "seriouslysuredeleteall": {
										List<Poll> allPolls = pollContainer.GetPolls(update.CallbackQuery.From.Id);
										foreach (Poll poll in allPolls) {
											poll.DeleteFromDeleteAll(apikey, strings);
										}
										Api.EditMessageText(apikey, strings.GetString(Strings.StringsList.deletedEverything), chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.MessageId, inlineMessageId: update.CallbackQuery.InlineMessageId);
										break;
									}
									case "deleteallno":
										Api.EditMessageText(apikey, strings.GetString(Strings.StringsList.sadlyEverythingThere), chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.MessageId, inlineMessageId: update.CallbackQuery.InlineMessageId);
										break;
									case "update":
										string[] splitUpdateData = command.Split(':');
									pollContainer.GetPoll(int.Parse(splitUpdateData[1]), int.Parse(splitUpdateData[2])).Update(instances, currentInstance.chatID, strings, noApproximation:true, messageId:update.CallbackQuery.Message.MessageId, currentText:update.CallbackQuery.Message.Text);
										break;
									case "options": {
										string[] splitOptionsData = command.Split(':');
										if (splitOptionsData[1] == update.CallbackQuery.From.Id.ToString())
											pollContainer.GetPoll(int.Parse(splitOptionsData[1]), int.Parse(splitOptionsData[2])).UpdateWithOptionsPane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
									}
									break;
									case "percentage": {
										string[] splitOptionsData = command.Split(':');
										if (splitOptionsData[2] == update.CallbackQuery.From.Id.ToString()) {
											Poll poll = pollContainer.GetPoll(int.Parse(splitOptionsData[2]), int.Parse(splitOptionsData[3]));
											poll.SetPercentage((PercentageBars.Bars)Enum.Parse(typeof(PercentageBars.Bars), splitOptionsData[1]));
											poll.UpdateWithOptionsPane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
										}
									}
									break;
									case "sorted":
									case "unsorted": {
										string[] splitOptionsData = command.Split(':');
										if (splitOptionsData[1] == update.CallbackQuery.From.Id.ToString()) {
											Poll poll = pollContainer.GetPoll(int.Parse(splitOptionsData[1]), int.Parse(splitOptionsData[2]));
											poll.SetSorted(!poll.Sorted);
											poll.UpdateWithOptionsPane(apikey, strings, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text);
										}
									}
									break;
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
									case "clone": {
										string[] splitCloneData = command.Split(':');
										if (splitCloneData[1] == update.CallbackQuery.From.Id.ToString()) {
											Poll poll = pollContainer.GetPoll(update.CallbackQuery.From.Id, int.Parse(splitCloneData[2]));
											//Poll newPoll = poll.CloneAndClean(pointer);
											var oldPollType = pointer.PollType;
											var oldAnony = pointer.Anony;
											pointer.PollType = poll.PollType;
											pointer.Anony = poll.Anony;
											pointer.LastPollId++;
											Poll newPoll = pollContainer.Add(pointer, update.CallbackQuery.From.Id, poll.PollText);
											newPoll.AddDescription(poll.PollDescription);
											/*if (poll.PollType != EPolls.board)*/
												foreach (string optionTitle in poll.PollVotes.Keys) {
													newPoll.AddOption(optionTitle);
												}
											if (poll.PollType == EPolls.limitedDoodle) {
												LimitedDoodle doodle = (LimitedDoodle)poll;
												LimitedDoodle newDoodle = (LimitedDoodle)newPoll;
												newDoodle.MaxVotes = doodle.MaxVotes;
											}
											dBHandler.AddToQueue(newPoll);
											newPoll.Send(Globals.GlobalOptions.Apikey, strings, update.CallbackQuery.From.Id);
											pointer.PollType = oldPollType;
											pointer.Anony = oldAnony;
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
									}
									break;
									case "skip":
										string stuff = command.Split(':')[1];
										if (stuff == "description") {
											pointer.Needle = ENeedle.firstOption;/*
											if (pointer.PollType == EPolls.board) {
												Poll poll = pollContainer.GetLastPoll(pointer);
												poll.FinishCreation();
												poll.Update(instances, currentInstance.chatID, strings, messageId: update.CallbackQuery.Message.MessageId);
												pointer.Needle = ENeedle.nothing;
											} else {*/
												Api.EditMessageText(apikey, strings.GetString(Strings.StringsList.rogerSendIt), chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.MessageId);
											/*}*/
										}
										break;
									case "add": {
										string stuffing = command.Split(':')[1];
										if (stuffing == "description") {
											pointer.Needle = ENeedle.pollDescription;
											Api.EditMessageText(apikey, strings.GetString(Strings.StringsList.sendPollDescription), chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.MessageId, replyMarkup: InlineMarkupGenerator.GetOneButtonMarkup(InlineKeyboardButton.Create(strings.GetString(Strings.StringsList.skipThisStep), callbackData: "comm:skip:description")));
										} //TODO continue here
										break;
									}
									case "iwannavote":
										try {
											string[] splitVoteData = Cryptography.Decrypt(command.Substring(11), apikey).Split(':');
											pollContainer.GetPoll(int.Parse(splitVoteData[0]), int.Parse(splitVoteData[1])).Update(instances, currentInstance.chatID, strings, true, update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text, vote: true);
										} catch (System.FormatException) {
											string[] splitVoteData = command.Split(':');
											pollContainer.GetPoll(int.Parse(splitVoteData[1]), int.Parse(splitVoteData[2])).Update(instances, currentInstance.chatID, strings, true, update.CallbackQuery.Message.MessageId, "", update.CallbackQuery.Message.Chat.Id);
											text = strings.GetString(Strings.StringsList.updatingPoll);
											alert = true;
										}

										break;
									case "pag":
										string[] splitPagData = command.Split(':');
										pollContainer.GetPoll(int.Parse(splitPagData[1]), int.Parse(splitPagData[2])).Update(apikey, strings, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, int.Parse(splitPagData[3]), true);
										break;
									case "lang":
										string[] langSplit = command.Split(':');
										strings.SetLanguage(pointer.Lang = (Strings.Langs)Enum.Parse(typeof(Strings.Langs), langSplit[1]));
										LangMessage.Refresh(apikey, strings, pointer, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										break;
									case "showWelcome":
										pointer.Needle = ENeedle.pollText;
										WelcomeMessage.Refresh(apikey, strings, pointer, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										break;
									default:
										//TODO Add message
									break;
								}
								if (returnUrl == null) {
									Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, text, alert);
									if (refresh)
										WelcomeMessage.Refresh(apikey, strings, pointer, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
								} else {
									Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, "Testing", url: returnUrl);
								}
							} else if (update.CallbackQuery.Data.StartsWith("page:", StringComparison.CurrentCulture)) {
								Pointer pointer = pointerContainer.GetPointer(update.CallbackQuery.From.Id, update.CallbackQuery.From.LanguageCode);
								if (pointer != null)
									strings.SetLanguage(pointer.Lang);
								else
									strings.SetLanguage(Strings.Langs.none);
								string page = update.CallbackQuery.Data.Substring(5);
								Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id);
								switch (page) {
									case "chpoll":
										InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup {
											InlineKeyboard = new List<List<InlineKeyboardButton>>{
										new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.StringsList.inlineButtonVote),(pointer.PollType == EPolls.vote ? "✅" : "☑️")), callbackData: "comm:vote") },
										new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.StringsList.inlineButtonDoodle), (pointer.PollType == EPolls.doodle ? "✅" : "☑️")), callbackData: "comm:doodle") },

										new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.StringsList.inlineButtonLimitedDoodle), (pointer.PollType == EPolls.limitedDoodle ? "✅" : "☑️")), callbackData: "comm:limitedDoodle") },
										//new List<InlineKeyboardButton>() { InlineKeyboardButton.Create(String.Format(strings.GetString(Strings.StringsList.inlineButtonBoard), (pointer.PollType == EPolls.board ? "✅" : "☑️")), callbackData: "comm:board") },
									}
										};
										Api.EditMessageText(apikey, strings.GetString(Strings.StringsList.pollTypeDescriptionNoMoreBoard), inlineKeyboard, update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId);
										break;
								}
							} else {
								Pointer pointer = pointerContainer.GetPointer(update.CallbackQuery.From.Id, update.CallbackQuery.From.LanguageCode);
								if (pointer != null)
									strings.SetLanguage(pointer.Lang);
								else
									strings.SetLanguage(Strings.Langs.none);
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
										Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, strings.GetString(Strings.StringsList.tryVotePollClosed), showAlert:true);
									} else {
										if (poll.GetType() == typeof(LimitedDoodle)) {
											LimitedDoodle doodle = (LimitedDoodle)poll;
											if (doodle.Vote(apikey, int.Parse(splitData[2]), update.CallbackQuery.From, update.CallbackQuery.Message, out bool tooMuchVotes, update.CallbackQuery.InlineMessageId)) {
												Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.StringsList.callbackVotedFor), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key));
											} else {
												if (tooMuchVotes)
													Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.StringsList.callbackLimitedMaxReached), doodle.MaxVotes));
												else
													Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.StringsList.callbackTookBack), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key));
											}
										} else {
											if (poll.Vote(apikey, int.Parse(splitData[2]), update.CallbackQuery.From, update.CallbackQuery.Message, update.CallbackQuery.InlineMessageId)) {
												Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.StringsList.callbackVotedFor), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key.RemoveAppendingText()));
											} else {
												Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, String.Format(strings.GetString(Strings.StringsList.callbackTookBack), poll.PollVotes.ElementAt(int.Parse(splitData[2])).Key.RemoveAppendingText()));
											}
										}
									}
									if (update.CallbackQuery.InlineMessageId == null)
										poll.Update(instances, currentInstance.chatID, strings, false, /*TODO Request Handler here*/update.CallbackQuery.Message.MessageId, update.CallbackQuery.Message.Text, update.CallbackQuery.Message.Chat.Id);
								} catch (Exception) {
									Api.AnswerCallbackQuery(apikey, update.CallbackQuery.Id, strings.GetString(Strings.StringsList.voteDoesntExist));
								}
							}
						} else if (update.ChosenInlineResult != null) {
							string[] splitData = update.ChosenInlineResult.ResultId.Split(':');
							pollContainer.GetPoll(int.Parse(splitData[0]), int.Parse(splitData[1])).AddToMessageIDs(new MessageID {
								botChatID = currentInstance.chatID,
								inlineMessageId = update.ChosenInlineResult.InlineMessageId,
								channel = update.ChosenInlineResult.Query.StartsWith("$c:", StringComparison.CurrentCulture),
							});
						}
					}
					//TODO: API HERE
				}
				dBHandler.FlushToDB(strings, instances, currentInstance.chatID);
				currentInstance.update = Api.GetUpdatesAsync(currentInstance.apikey, offset);
			}
			Notifications.log("The bot has been shut down properly...");
		}
	}
}