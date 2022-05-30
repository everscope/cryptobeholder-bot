namespace CryptoBeholder.Lib.Responses
{
    public static class FirstStageCommandsResponces
    {
        public static readonly string StartResponce = "Hello! This is very useful bot" +
            " for tracking crypto currency.To learn how to use it, print '/help'";
        public static readonly string TraceNewResponce = "Enter coin's name:";
        public static readonly string DeleteTraceResponce = "Enter name of coin to stop tracking. " +
            "All your settings, that are connected with this coin, will be deleted too.";
        public static readonly string ChangeVsCurrency = "Here is a list of allowed currencies. " +
            "Select one of them for price displaying: \n";
        public static readonly string TraceListFullResponce = "You are tracking: \n";
        public static readonly string TraceListEmptyResponce = "You are not tracking anything yet.";
        public static readonly string EditTraceResponce = "Select coin to edit:";
        public static readonly string HelpResponce = "How to use this bot: \n" +
            "To add new coin to track, use '/trace_new'. Then type name of coin. If coin will be found, you will track it with default settings." +
            "Default settings are: TraceMode: On price change relatively, percentage: 5% \n \n" +
            "Tracing modes: \n" +
            "- On price change absolutely: you will get notification, when price will reach max or minimum value you selected." +
            " Once it reached max (min), you will get next notification only when price will reach min (max) value. Or you can change your settings. \n" +
            "- On price change relatively: you will get notification, when price change per last 24 hours(in %) will reach value you selected." +
            "Next time notification will be send in the same case as in On price change absolutely. \n" +
            "- After time: you will get notification once per time you selected. \n \n" +
            "To edit trace settings type 'edit_trace' and follow instructions. \n" +
            "To delete trace use '/delete_trace' \n" +
            "To check list of coins you are tracking, you can use '/trace_list' or '/trace_settings_list'. \n" +
            "If you want to get info about coin's prices you are tracking, use '/get_my_info'.";
    }
}
