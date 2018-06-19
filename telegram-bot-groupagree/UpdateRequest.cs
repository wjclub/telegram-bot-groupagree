using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegrambotgroupagree {
	public class UpdateRequest {
		public Poll poll;
		public List<string> alreadyUpdated;
		public EKindOfUpdate kindOfUpdate;
	}
}
