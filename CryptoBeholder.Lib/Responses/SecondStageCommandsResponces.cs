namespace CryptoBeholder.Lib.Responses
{
    public class SecondStageCommandsResponces
    {
        public static readonly string TraceNewAlreadyTracked = "You are already tracking this coin.";
        public static readonly string TraceNewCoinNotFound = "There is no such coin.";
        public static readonly string DeleteTraceSuccesful = "Coin has been successfully deleted.";
        public static readonly string DeleteTraceNotFound = "You are not tracking such coin";
        public static readonly string EditTrace = "Select trace mode. Use /help for more info.";
        public static readonly string EditTraceNoSuchCoin = "You are not tracking such coin.";
        public static readonly string ChangeVsCurrencyNoSuchCurrency = "There is no such vs currency." +
            " Check if it's written correctly.";
    }
}
