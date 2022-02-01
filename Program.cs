using CryptoBeholderBot;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using CoinGecko;
using System.Globalization;

namespace CryptoBeholderBot {
    public static class Program {

        private static Dictionary<long, string> _usersCommand = new Dictionary<long, string>();
        private static Dictionary<long, string> _traces = new Dictionary<long, string>();

        private static Dictionary<long, int> _traceStage = new Dictionary<long, int>();

        public static UserContext _userContext;

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            ApiClient.Initialize();

            _userContext = new UserContext();

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

        async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
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
                string command = _usersCommand[chatId];

                switch (command)
                {
                    case "/trace_new":
                        if (Tracer.CoinsList.Any(p => p.Name.ToLower() == messageText.ToLower()))
                        {
                            if (_userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                .Any(p => p.Coin.ToLower() == messageText.ToLower()))
                            {
                                messageResponse[0] = "You are already tracking this coin.";
                                break;
                            }
                            messageResponse[0] = $"You are tracking {messageText} with default settings.";

                            _userContext.Users.First(p => p.ChatId == chatId)
                                .TrackedCoins.Add(new TrackedCoin() { Coin = messageText });
                        }
                        else
                        {
                            messageResponse[0] = $"There is no such coin";
                        }
                        break;
                    case "/delete_trace":
                        var user = _userContext.Users.First(p => p.ChatId == chatId);
                        if (user.TrackedCoins.Any(p => p.Coin.ToLower() == messageText.ToLower()))
                        {
                            messageResponse[0] = "Coin has been succesfuly deleted.";
                            var toRemove = _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                .First(p => p.Coin.ToLower() == messageText.ToLower());
                            user.TrackedCoins.Remove(toRemove);
                        }
                        break;
                    case "/edit_trace":
                        if (_userContext.Users.First(p => p.ChatId == chatId).TrackedCoins.Any(p => p.Coin.ToLower() == messageText.ToLower()))
                        {
                            _traces.Add(chatId, messageText);
                            _traceStage.Add(chatId, 0);

                            messageResponse[0] = "Select trace mode. Use /help for more info";
                            keyboardMarkup[0] = new(new[]{
                                new KeyboardButton [] { "On price chage absolutely" },
                                new KeyboardButton [] { "On price chage relatively" },
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
                            messageResponse[0] = $"Your vs currency has been changed to {messageText}";
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
                            case "On price chage absolutely":
                                _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                    .TracingMode = TraceMode.OnPriceChageAbsolutely;
                                break;
                            case "On price chage relatively":
                                _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                    .TracingMode = TraceMode.OnPriceChageRelatively;
                                break;
                            case "After time":
                                _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                    .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
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
                                    .TracingMode; ;

                        switch (traceMode)
                        {
                            case TraceMode.OnPriceChageAbsolutely:
                                messageResponse[1] = "Select price limits after achiving which you will get notification. " +
                                    "Max and min price should look like this: 14000-54000";
                                break;
                            case TraceMode.OnPriceChageRelatively:
                                messageResponse[1] = "Select price limit in percentage after achiving which you will get notification:" +
                                   "Persentage should look like this: 25,58";
                                break;
                            case TraceMode.AfterTime:
                                messageResponse[1] = "Select time period (in hours) after which you will get notification. " +
                                    "Time should look like this: 1:45 (hours:minutes, max is 24 hours)";
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
                                    if (priceString.Length == 2 && Convert.ToDecimal(priceString[0]) < Convert.ToDecimal(priceString[1]))
                                    {

                                        messageResponse[0] = "Your min and max price is set";
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .AbsoluteMin = Convert.ToDecimal(priceString[0]);
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .AbsoluteMax = Convert.ToDecimal(priceString[1]);
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .MaxIsReached = false;
                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .MinIsReached = false;
                                        messageResponse[1] = "All coin settings are set now";
                                    }
                                    else
                                    {
                                        messageResponse[0] = "Something is wrong with your numbers.";
                                        _traceStage.Remove(chatId);
                                        _traces.Remove(chatId);
                                        break;
                                    }
                                }
                                catch
                                {
                                    messageResponse[0] = "Something is wrong with your numbers.";
                                    _traceStage.Remove(chatId);
                                    _traces.Remove(chatId);
                                    break;
                                }
                                break;
                            case TraceMode.OnPriceChageRelatively:
                                try
                                {
                                    messageResponse[0] = "Your percentage is set";
                                    _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .AbsoluteMax = Convert.ToDecimal(messageText);
                                    _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                          .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                          .PersentNegativeIsReached = false;
                                    _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                        .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                        .PersentPositiveIsReached = false;
                                    _traceStage.Remove(chatId);
                                    _traces.Remove(chatId);
                                    messageResponse[1] = "All coin settings are set now";
                                    break;
                                }
                                catch
                                {
                                    messageResponse[0] = "Something is wrong with you percents.";
                                    _traceStage.Remove(chatId);
                                    _traces.Remove(chatId);
                                    break;
                                }
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
                                        _traceStage.Remove(chatId);
                                        _traces.Remove(chatId);
                                        messageResponse[0] = "Your time period is set";
                                        messageResponse[1] = "All coin settings are set now";

                                        _userContext.Users.First(p => p.ChatId == chatId).TrackedCoins
                                            .First(p => p.Coin.ToLower() == trace.ToLower()).TraceSettings
                                            .Timestamp = DateTime.Now;

                                        break;
                                    }
                                    else
                                    {
                                        messageResponse[0] = "Something is wrong with your time";
                                        _traceStage.Remove(chatId);
                                        _traces.Remove(chatId);
                                    }

                                }
                                catch
                                {
                                    messageResponse[0] = "Something is wrong with your time";
                                    _traceStage.Remove(chatId);
                                    _traces.Remove(chatId);
                                    break;
                                }
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
                            "Hello! This is very usefull bot for tracking crypto currency";

                        if (!_userContext.Users.Any(p => p.ChatId == (int)chatId))
                        {
                            _userContext.Users.Add(new User() { ChatId = (int)chatId});
                            await _userContext.SaveChangesAsync();
                        }
                        break;
                    case "/trace_new":
                        messageResponse[0] = "Enter coin's name:";
                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/delete_trace":
                        messageResponse[0] = "Enter name of coin to stop tracking. " +
                            "All your settings, that are connected with this coin, will be delleted too.";
                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/change_vs_currency":
                        messageResponse[0] = "Here is a list of allowed currencies. Sellect one of them for price displaying: \n";
                        foreach (string currency in Tracer.VsCurrencies)
                        {
                            messageResponse[0] += "- " + currency + "\n"; 
                        }
                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/trace_list":
                        var user = _userContext.Users.FirstOrDefault(p => p.ChatId == chatId);
                        
                        if(user?.TrackedCoins != null)
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

                        if(user_2.TrackedCoins != null)
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
                                            + "- Min price:" + tracked.TraceSettings.AbsoluteMin + "\n"
                                            + "- Max price:" + tracked.TraceSettings.AbsoluteMax + "\n";
                                        break;
                                    case TraceMode.OnPriceChageRelatively:
                                        messageResponse[0] += "-" + "Tracing mode: on change price relatively \n"
                                            + "- Percentage:" + tracked.TraceSettings.Persent + "\n"
                                            + "- Time:" + ((DateTime)tracked.TraceSettings.Time).ToString("HH:mm") + "\n";
                                        break;
                                    case TraceMode.AfterTime:
                                        messageResponse[0] += "-" + "Tracing mode: after time \n";
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

        //public static async Task SendMessage(int chatId, TelegramBotClient botClient,
        //    CancellationToken cancellationToken, string text, ReplyKeyboardMarkup? keyboardMarkup = null)
        //{
        //    Message sentSecondMessage = await botClient.SendTextMessageAsync(
        //        chatId: chatId,
        //        text: text,
        //        replyMarkup: keyboardMarkup,
        //        cancellationToken: cancellationToken);
        //}
    }
}
