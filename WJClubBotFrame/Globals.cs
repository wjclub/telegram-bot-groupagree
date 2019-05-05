namespace WJClubBotFrame {
    public static class Globals
    {
        public static string LoggingKey { get { return GlobalOptions.LoggingKey; } }
        public static string LoggingChat { get { return GlobalOptions.LoggingChat; } }

        public class Options
        {
            public string LoggingKey = null;
            public string LoggingChat = null;
        }

        public static Options GlobalOptions;

    }
}