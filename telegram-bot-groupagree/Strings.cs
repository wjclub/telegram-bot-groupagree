using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WJClubBotFrame;

namespace telegrambotgroupagree {
	public class Strings {
		public enum langs {
			none,
			en,
			de,
			it,
			ptBR,
			he,
			uk,
			nb_NO,
			fa,
			sp,
			zh_TW,
			zh_HK,
			zh_CN,
			rus,
			fr,
			uz,
		};
		public enum stringsList {
			nothingToCancel,
			canceledThat,
			hasCanceled,
			creatingPoll,
			donation,
			setupPlease,
			boardAnswer,
			pollDoesntExist,
			unrecognizedCommand,
			wannaCreate,
			createPoll,
			createBoard,
			addedToPoll,
			alreadyDefined,
			done,
			boardSuccess,
			tooManyCharacters,
			inlineTextCreateNewPoll,
			finishedPollCreation,
			noPollsToFinish,
			seriouslyWannaDeleteThePoll,
			sure,
			no,
			roger,
			rogerSendIt,
			inlineButtonVote,
			inlineButtonDoodle,
			inlineButtonBoard,
			pollTypeDescription,
			callbackVotedFor,
			voteDoesntExist,
			startMessage,
			startMessageChangePollTypeButton,
			startMessagePersonalButton,
			startMessageAnonyButton,
			votedSuccessfully,
			callbackTookBack,
			listYourPolls,
			listNothingHere,
			vote,
			doodle,
			board,
			limitedDoodle,
			rendererMultiVotedSoFar,
			rendererSingleVotedSoFar,
			rendererZeroVotedSoFar,
			pollClosed,
			boardPagCantFind,
			boardPagBottomLine,
			boardShowMore,
			buttonVote,
			setLanguage,
			updatingPoll,
			skipThisStep,
			publish,
			publishWithLink,
			commPageClose,
			commPageReopen,
			commPageDelete,
			commPageRefresh,
			tryVotePollClosed,
			callbackLimitedMaxReached,
			limitedDoodleYouCanChooseSoMany,
			limitedDoodleEnterMaxVotes,
			limitedDoodleGiveMeANumberNothingElse,
			limitedDoodleGiveMeANumberSmallerOrEqualToOptionCount,
			inlineButtonLimitedDoodle,
			pollNotFound,
			seriouslyDeleteEverything,
			deletedEverything,
			sadlyEverythingThere,
			seriouslySureDeleteEverything,
			deleteAllFooter,
			commPageOptions,
			optionsForPoll,
			optionsPercentageNone,
			optionsSorted,
			optionsUnsorted,
			optionsAppendable,
			optionsUnappendable,
			optionsShareable,
			buttonAppend,
			appendSendMe,
			addOptionSuccess,
			addOptionTooManyChars,
			moderate,
			pollBeingEdited,
		};


		private langs currentLang = langs.en;
		public langs CurrentLang { get { return this.currentLang; } }

		Dictionary<langs, Dictionary<stringsList, string>> langStrings;
		public Dictionary<stringsList, string> StringsEn { get { return langStrings[langs.en]; } }
		Dictionary<langs, string> langNames;

		public Strings() {
			string stringsFile = "";
			try {
				using (StreamReader reader = new StreamReader(@"strings.json")) {
					stringsFile += reader.ReadToEnd();
				}
			} catch (FileNotFoundException) {
				
			}
			langStrings = JsonConvert.DeserializeObject<Dictionary<langs,Dictionary<stringsList, string>>>(stringsFile);
			string langNamesFile = "";
			try {
				using (StreamReader reader = new StreamReader(@"langnames.json")) {
					langNamesFile += reader.ReadToEnd();
				}
			} catch (FileNotFoundException) {

			}
			langNames = JsonConvert.DeserializeObject<Dictionary<langs, string>>(langNamesFile);
		}

		public void SetLanguage(langs lang) {
			this.currentLang = lang;
		}

		internal string GetLangName(langs lang) {
			return langNames[lang];
		}

		public string GetString(stringsList name) {
			try {
				return langStrings[(currentLang == langs.none ? langs.en : currentLang)][name];
			} catch (System.Collections.Generic.KeyNotFoundException) {
				/*#if DEBUG
				Notifications.log(string.Format("I couldn't find {1} in strings {0}", currentLang.ToString(), name.ToString()));
				#endif*/
				return langStrings[langs.en][name];
			}
		}

		public Dictionary<langs, Dictionary<stringsList, string>> GetMissingStrings() {
			Dictionary<langs, Dictionary<stringsList, string>> output = new Dictionary<langs, Dictionary<stringsList, string>>();
			for (int i = 1; i <= langNames.Count; i++) {
				output.Add((langs)i, new Dictionary<stringsList, string>());
				for (int j = 0; j < Enum.GetNames(typeof(stringsList)).Length; j++) {
					try {
						string temp = langStrings[(langs)i][(stringsList)j];
					} catch (System.Collections.Generic.KeyNotFoundException) {
						output[(langs)i].Add((stringsList)j, this.GetString((stringsList)j));
					}
				}
			}
			return output;
		}
	}
}