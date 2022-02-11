using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace CryptoBeholderBot {
    public static class Program {

        private static Dictionary<long, string> _usersCommand = new Dictionary<long, string>();
        private static Dictionary<long, string> _traces = new Dictionary<long, string>();

        private static Dictionary<long, int> _traceStage = new Dictionary<long, int>();

        private static UserContext _userContext;

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            ApiClient.Initialize();

            _userContext = new UserContext();
            _userContext.Database.EnsureCreated();

            var botClient = new TelegramBotClient("5231381256:AAFolea3xHyRaPPg-Olf1E_hJIOIOEtWQ3A");

            using var cts = new CancellationTokenSource();

            await Tracer.Start(botClient, cts);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();

            cts.Cancel();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type != UpdateType.Message)
            {
                return;
            }

            if (update.Message!.Type != MessageType.Text)
            {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            //used string[] and ReplyKeyboardMarkup because \trace_new command
            //requiers sending two separated messages
            string?[] messageResponse = new string [2];
            ReplyKeyboardMarkup? [] keyboardMarkup = new ReplyKeyboardMarkup [2];

            if(messageText == "/escape")
            {
                if(_usersCommand.ContainsKey(chatId))
                {
                    _usersCommand.Remove(chatId);
                }

                if (_traces.ContainsKey(chatId))
                {
                    _traces.Remove(chatId);
                }

                if (_traceStage.ContainsKey(chatId))
                {
                    _traceStage.Remove(chatId);
                }

                messageResponse[0] = "Command aborted, no active commands now";
            }

            if (_usersCommand.ContainsKey(chatId))
            {
                switch (_usersCommand[chatId])
                {
                    case "/trace_new":
                        if (Tracer.CoinsList.Any(p => p.Name.ToLower() == messageText.ToLower()))
                        {
                            if (_userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                .Any(p => p.Coin.ToLower() == messageText.ToLower()))
                            {
                                messageResponse[0] = "You are already tracking this coin.";
                            }
                            else
                            {
                                messageResponse[0] = $"You are tracking {messageText} with default settings.";

                                _userContext.Users.First(p => p.ChatId == chatId)
                                    .TrackedCoins.Add(new TrackedCoin() { Coin = Tracer.CoinsList.First(
                                        p => p.Name.ToLower() == messageText.ToLower()).Name});
                            }

                        }
                        else
                        {
                            messageResponse[0] = $"There is no such coin.";
                        }
                        break;
                    case "/delete_trace":
                        var user = _userContext.Users.First(p => p.ChatId == chatId);
                        if (user.TrackedCoins.Any(p => p.Coin.ToLower() == messageText.ToLower()))
                        {
                            messageResponse[0] = "Coin has been successfully deleted.";
                            var toRemove = _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                .First(p => p.Coin.ToLower() == messageText.ToLower());
                            user.TrackedCoins.Remove(toRemove);
                        }
                        break;
                    case "/edit_trace":
                        if (_userContext.Users.First(p => p.ChatId == chatId).TrackedCoins.Any(p => p.Coin.ToLower() == messageText.ToLower()))
                        {
                            _traces.Add(chatId, _userContext.Users.First(p => p.ChatId == chatId)
                                                .TrackedCoins.First(p => p.Coin.ToLower() == messageText.ToLower()).Coin);
                            _traceStage.Add(chatId, 0);

                            messageResponse[0] = "Select trace mode. Use /help for more info.";
                            keyboardMarkup[0] = new(new[]{
                                new KeyboardButton [] { "On price change absolutely" },
                                new KeyboardButton [] { "On price change relatively" },
                                new KeyboardButton [] { "After time"}
                            });
                        }
                        else
                        {
                            messageResponse[0] = "You are not tracking such coin.";
                        }
                        break;
                    case "/change_vs_currency":
                        if (Tracer.VsCurrencies.Contains(messageText.ToLower()))
                        {
                            messageResponse[0] = $"Your vs currency has been changed to {messageText}.";
                            _userContext.Users.First(p => p.ChatId == chatId).VsCurrency = messageText.ToLower();
                        }
                        else
                        {
                            messageResponse[0] = "There is no such vs currency. Check if it's written correctly.";
                        }
                        break;
                    default:
                        break;
                }
                _userContext.SaveChanges();
                _usersCommand.Remove(chatId);
            }
            else if (_traces.ContainsKey(chatId) && _traceStage.ContainsKey(chatId))
            {
                string trace = _traces[chatId];

                var traceMode = _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                    .TracingMode;

                switch (_traceStage[chatId])
                {
                    case 0:
                        switch (messageText)
                        {
                            case "On price change absolutely":
                                _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin == trace).TraceSettings
                                    .TracingMode = TraceMode.OnPriceChageAbsolutely;
                                break;
                            case "On price change relatively":
                                _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin == trace).TraceSettings
                                    .TracingMode = TraceMode.OnPriceChageRelatively;
                                break;
                            case "After time":
                                _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin == trace).TraceSettings
                                    .TracingMode = TraceMode.AfterTime;
                                break;
                            default:
                                break;
                        }
                        messageResponse[0] = "Trace mode has been changed.";
                        _userContext.SaveChanges();
                        _traceStage[chatId] = 1;

                        traceMode = _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                    .TracingMode;

                        switch (traceMode)
                        {
                            case TraceMode.OnPriceChageAbsolutely:
                                messageResponse[1] = "Select price limits after achieving which you will get notification. " +
                                    "Max and min price should look like this: 14000-54000.";
                                break;
                            case TraceMode.OnPriceChageRelatively:
                                messageResponse[1] = "Select price limit in percentage after achieving which you will get notification:" +
                                   "Percentage should look like this: 25,58.";
                                break;
                            case TraceMode.AfterTime:
                                messageResponse[1] = "Select time period (in hours) after which you will get notification. " +
                                    "Time should look like this: 1:45 (hours:minutes, max is 24 hours).";
                                break;
                            default:
                                break;
                        }
                        break;
                    case 1:
                        switch (traceMode)
                        {
                            case TraceMode.OnPriceChageAbsolutely:
                                try
                                {
                                    string[] priceString = messageText.Split('-');
                                    if (Convert.ToDecimal(priceString[0]) < Convert.ToDecimal(priceString[1]))
                                    {

                                        messageResponse[0] = "Your min and max price is set.";
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin == trace).TraceSettings
                                            .AbsoluteMin = Convert.ToDecimal(priceString[0]);
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin == trace).TraceSettings
                                            .AbsoluteMax = Convert.ToDecimal(priceString[1]);
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin == trace).TraceSettings
                                            .MaxIsReached = false;
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin == trace).TraceSettings
                                            .MinIsReached = false;
                                        messageResponse[1] = "All coin settings are set now.";
                                    }
                                    else
                                    {
                                        messageResponse[0] = "Something is wrong with your numbers.";
                                    }
                                }
                                catch
                                {
                                    messageResponse[0] = "Something is wrong with your numbers.";
                                }
                                _traceStage.Remove(chatId);
                                _traces.Remove(chatId);
                                break;
                            case TraceMode.OnPriceChageRelatively:
                                try
                                {
                                    messageResponse[0] = "Your percentage is set.";
                                    _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .Persent = Convert.ToDecimal(messageText);
                                    _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                          .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                          .PersentNegativeIsReached = false;
                                    _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                        .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                        .PersentPositiveIsReached = false;
                                    messageResponse[1] = "All coin settings are set now.";
                                }
                                catch
                                {
                                    messageResponse[0] = "Something is wrong with you percents.";

                                }
                                _traceStage.Remove(chatId);
                                _traces.Remove(chatId);
                                break;
                            case TraceMode.AfterTime:
                                try
                                {


                                    string[] dateString = messageText.Split(':');
                                    int hours = Convert.ToInt32(dateString[0]);
                                    int minutes = Convert.ToInt32(dateString[1]);

                                    if (hours < 24 && minutes < 60)
                                    {
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .Time = new DateTime(0001, 01, 01, hours, minutes, 0);
                                        messageResponse[0] = "Your time period is set.";
                                        messageResponse[1] = "All coin settings are set now.";

                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .Timestamp = DateTime.Now;
                                    }
                                    else
                                    {
                                        messageResponse[0] = "Something is wrong with your time.";

                                    }

                                }
                                catch
                                {
                                    messageResponse[0] = "Something is wrong with your time.";
                                }
                                _traceStage.Remove(chatId);
                                _traces.Remove(chatId);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                _userContext.SaveChanges();

            }
            else
            {
                switch (messageText)
                {
                    case "/start":
                        Console.WriteLine($"bot started in {chatId}");

                        messageResponse[0] =
                            "Hello! This is very useful bot for tracking crypto currency. To learn how to use it, print '/help'";

                        if (!_userContext.Users.Any(p => p.ChatId == (int)chatId))
                        {
                            _userContext.Users.Add(new User() { ChatId = (int)chatId});
                            _userContext.SaveChanges();
                        }
                        break;
                    case "/trace_new":
                        messageResponse[0] = "Enter coin's name:";
                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/delete_trace":
                        messageResponse[0] = "Enter name of coin to stop tracking. " +
                            "All your settings, that are connected with this coin, will be deleted too.";
                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/change_vs_currency":
                        messageResponse[0] = "Here is a list of allowed currencies. Select one of them for price displaying: \n";
                        foreach (string currency in Tracer.VsCurrencies)
                        {
                            messageResponse[0] += "- " + currency + "\n"; 
                        }
                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/get_my_info":
                        Tracer.CheckUserCoins((int)chatId, botClient, cancellationToken);
                        break;
                    case "/trace_list":
                        var user = _userContext.Users.FirstOrDefault(p => p.ChatId == chatId);

                        if (user.TrackedCoins.Count > 0)
                        { 
                            messageResponse[0] = "You are tracking: \n";

                            foreach (TrackedCoin tracked in user.TrackedCoins)
                            {
                                messageResponse[0] += "- " + tracked.Coin + "\n";
                            }
                        }
                        else
                        {
                            messageResponse[0] = "You are not tracking anything yet.";
                        }
                        break;
                    case "/trace_settings_list":
                        var user_2 = _userContext.Users.FirstOrDefault(p => p.ChatId == chatId);

                        if(user_2.TrackedCoins.Count > 0)
                        {
                            messageResponse[0] = "You are tracking: \n \n";
                            foreach (TrackedCoin tracked in user_2.TrackedCoins)
                            {
                                var traceMode = tracked.TraceSettings.TracingMode;

                                messageResponse[0] += tracked.Coin + ": \n";

                                switch (traceMode)
                                {
                                    case TraceMode.OnPriceChageAbsolutely:
                                        messageResponse[0] += "- Tracing mode: on change price absolutely \n"
                                            + $"- Min price: { tracked.TraceSettings.AbsoluteMin} \n"
                                            + $"- Max price: {tracked.TraceSettings.AbsoluteMax} \n";
                                        break;
                                    case TraceMode.OnPriceChageRelatively:
                                        messageResponse[0] += "- Tracing mode: on change price relatively \n" +
                                            $"- Percentage: {tracked.TraceSettings.Persent} \n";
                                        break;
                                    case TraceMode.AfterTime:
                                        messageResponse[0] += "- Tracing mode: after time \n"
                                        +$"- Time: {((DateTime)tracked.TraceSettings.Time).ToString("HH:mm")} \n";

                                        break;
                                    default:
                                        break;
                                }

                                messageResponse[0] += "\n \n";
                            }
                        }
                        else
                        {
                            messageResponse[0] = "You are not tracking anything yet.";
                        }
                        break;
                    case "/edit_trace":
                        messageResponse[0] = "Select coin to edit:";

                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/help":
                        messageResponse[0] = "How to use this bot: \n" +
                            "To add new coin to track, use '/trace_new'. Then type name of coin. If coin will be found, you will track it with default settings." +
                            "Default settings are: TraceMode: On price change relatively, percentage: 5% \n \n" +
                            "Tracing modes: \n" +
                            "- On price change absolutely: you will get notification, when price will reach max or minimum value you selected." +
                            " Once it reached max (min), you will get next notification only when price will reach min (max) value. Or you can change your settings. \n" +
                            "- On price change relatively: you will get notification, when price change per last 24 hours(in %) will reach value you selected." +
                            "Next time notification will be send in the same case as in On price change absolutely. \n" +
                            "- After time: you will get notification once per time you selected. \n \n;" +
                            "To edit trace settings type 'edit_trace' and follow instructions. \n" +
                            "To delete trace use '/delete_trace' \n" +
                            "To check list of coins you are tracking, you can use '/trace_list' or '/trace_settings_list'. \n" +
                            "If you want to get info about coin's prices you are tracking, use '/get_my_info'.";
                        break;
                    default:
                        break;
                }
            }

            if(messageResponse[0] != null)
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageResponse[0],
                        replyMarkup: keyboardMarkup[0] == null ? new ReplyKeyboardRemove() : keyboardMarkup[0],
                        cancellationToken: cancellationToken);
                if(messageResponse[1] != null)
                {
                    Message sentSecondMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageResponse[1],
                        replyMarkup: keyboardMarkup[1] == null ? new ReplyKeyboardRemove() : keyboardMarkup[1],
                        cancellationToken: cancellationToken);
                }

            }

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
