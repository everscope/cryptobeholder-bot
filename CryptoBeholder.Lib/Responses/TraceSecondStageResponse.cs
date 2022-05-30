namespace CryptoBeholder.Lib.Responses
{
    public class TraceSecondStageResponse
    {
        public static readonly string OnPriceChangeAbsolutely = "On price change absolutely";
        public static readonly string OnPriceChangeRelatively = "On price change relatively";
        public static readonly string AfterTime = "After time";
        public static readonly string TraceModeChanged = "Trace mode has been changed.";
        public static readonly string OnPriceChangeAbsolutelyExample = "Select price limits " +
            "after achieving which you will get notification. " +
            "Max and min price should look like this: 14000-54000.";
        public static readonly string OnPriceChangeRelativelyExample = "Select price limit " +
            "in percentage after achieving which you will get notification:" +
            "Percentage should look like this: 25,58.";
        public static readonly string OnPriceChangeAfterTimeExample = "Select time period " +
            "(in hours) after which you will get notification. " +
            "Time should look like this: 1:45 (hours:minutes, max is 24 hours).";
        public static readonly string MinAndMaxWasSet = "Your min and max price is set.";
        public static readonly string AllSettingsSet = "All coin settings are set now.";
        public static readonly string SomethingIsWrongNumbers = "Something is wrong with your numbers.";
        public static readonly string PercentageSet = "Your percentage is set.";
        public static readonly string SomethingIsWrongPercentage = "Something is wrong with you percents.";
        public static readonly string TimePeriodSet = "Your time period is set.";
        public static readonly string SomethingIsWrongTime = "Something is wrong with your time.";
    }
}
