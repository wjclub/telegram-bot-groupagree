using System;
using System.Collections.Generic;
using System.Linq;
using WJClubBotFrame.Types;

namespace telegrambotgroupagree {
	public static class DictionaryWorker {
		public static Dictionary<TKey, TValue> CloneDictionary<TKey, TValue> (this Dictionary<TKey, TValue> original) {
			Dictionary<TKey, TValue> ret = new Dictionary<TKey, TValue>(original.Count,
																	original.Comparer);
			foreach (KeyValuePair<TKey, TValue> entry in original) {
				ret.Add(entry.Key, (TValue)entry.Value);
			}
			return ret;
		}

		public static Dictionary<TKey, TValue> Sort<TKey, TValue> (this Dictionary<TKey, TValue> toSort) where TValue:List<User> {
			var myList = toSort.ToList();
			myList.Sort((x, y) => y.Value.Count.CompareTo(x.Value.Count));
			toSort = myList.ToDictionary(x => x.Key, x => x.Value);	
			return toSort;
		}
	}
}

