using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;

namespace telegrambotgroupagree {
	public class PointerContainer {
		public PointerContainer (DBHandler dBHandler) {
			this.dBHandler = dBHandler;
		}

		private DBHandler dBHandler;

		public override string ToString () {
			return "NOT IMPLEMENTED";
		}

		public void Add(int chatId, string languageCode) {
			Pointer pointer = new Pointer(chatId, languageCode);
			dBHandler.AddToQueue(pointer);
		}

		public void Add(Pointer pointer) {
			dBHandler.AddToQueue(pointer);
		}

		public Pointer GetPointer (int chatId, string languageCode) {
			Pointer pointer = dBHandler.PointerQueue.Find(x => x.ChatId == chatId);
			if (pointer == null) {
				pointer = dBHandler.GetPointer(chatId);
				if (pointer == null) {
					pointer = new Pointer(chatId, languageCode);
				}
			}
			if (pointer.Lang == Strings.Langs.none)
				pointer.Lang = Strings.GetLangFromIEFT(languageCode);
			dBHandler.AddToQueue(pointer);
			return pointer;
		}

		/*public void SetPointer(int chatId, ENeedle? needle = null, EPolls? pollType = null, EAnony? anony = null) {
			Pointer pointer = pointerDB.Find(x => x.ChatId == chatId);
			if (needle != null)pointer.Needle = (ENeedle)needle;
			if (pollType != null) pointer.PollType = (EPolls)pollType;
			if (anony != null) pointer.Anony = (EAnony)anony;
			dBHandler.AddToQueue (pointer);
		}

		public void SetPointer(Pointer pointer) {
			dBHandler.AddToQueue(pointer);
			SetPointer(pointer.ChatId, pointer.Needle, pointer.PollType, pointer.Anony);
		}*/
	}
}

