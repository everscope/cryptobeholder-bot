using CryptoBeholder.Lib.Responses;
using CryptoBeholder.Lib.Interfaces;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using Microsoft.Extensions.Configuration;

namespace CryptoBeholderBot
{
    public class Bot
    {
        private readonly string _botId = "5231381256:AAFolea3xHyRaPPg-Olf1E_hJIOIOEtWQ3A";
        private Dictionary<long, string> _usersCommand = new Dictionary<long, string>();
        private Dictionary<long, string> _traces = new Dictionary<long, string>();

        private Dictionary<long, int> _traceStage = new Dictionary<long, int>();

        private readonly IDatabaseReader _databaseReader;
        private readonly ITracer _tracer;

        private ITelegramBotClient _botClient;
        private CancellationToken _cancellationToken;

        public Bot(IDatabaseReader databaseReader,
            ITracer tracer)
        {
            _databaseReader = databaseReader;
            _databaseReader.Initialize();
            _tracer = tracer;
            ApiClient.Initialize();
        }

        public async Task MainAsync()
        {
            var botClient = new TelegramBotClient(_botId);
            _botClient = botClient;

            using var cts = new CancellationTokenSource();
            _cancellationToken = cts.Token;

            await _tracer.Start(botClient, cts);

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

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            if (update.Type != UpdateType.Message
                || update.Message!.Type != MessageType.Text)
            {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            //used string[] and ReplyKeyboardMarkup because \trace_new command
            //requiers sending two separated messages
            string?[] messageResponse = new string[2];
            ReplyKeyboardMarkup?[] keyboardMarkup = new ReplyKeyboardMarkup[2];

            if (messageText == EscapeCommand.Escape)
            {
                Escape(chatId);

                messageResponse[0] = EscapeCommand.EscapeResponce;
            }

            object[] responce;
            if (_usersCommand.ContainsKey(chatId))
            {
                responce = HandleSecondStageCommand(messageText, chatId);
            }
            else if (_traces.ContainsKey(chatId) && _traceStage.ContainsKey(chatId))
            {
                responce = HandleEditTraceSecondStage(messageText, chatId);
            }
            else
            {
                responce = HandleFirstStageCommand(messageText, chatId);
            }

            messageResponse = (string[])responce[0];
            keyboardMarkup = (ReplyKeyboardMarkup[])responce[1];

            if (messageResponse[0] != null)
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: messageResponse[0],
                    replyMarkup: keyboardMarkup[0] == null ? new ReplyKeyboardRemove() : keyboardMarkup[0],
                    cancellationToken: cancellationToken);
                if (messageResponse[1] != null)
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

        private object[] HandleSecondStageCommand(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];
            ReplyKeyboardMarkup?[] keyboardMarkup = new ReplyKeyboardMarkup[2];

            if (_usersCommand[chatId] == StartCommands.TraceNew)
            {
                messageResponse = TraceNewSecondStage(messageText, chatId);
            }
            else if (_usersCommand[chatId] == StartCommands.DeleteTrace)
            {
                messageResponse = DeleteTraceSecondStage(messageText, chatId);
            }
            else if (_usersCommand[chatId] == StartCommands.EditTrace)
            {
                object [] responce = EditTraceSecondStage(messageText, chatId);
                messageResponse = (string[])responce[0];
                keyboardMarkup = (ReplyKeyboardMarkup[])responce[1];
            }
            else if (_usersCommand[chatId] == StartCommands.ChangeVsCurrency)
            {
                messageResponse = ChangeVsCurrencySecondStage(messageText, chatId);
            }

            _usersCommand.Remove(chatId);

            return new object[] { messageResponse, keyboardMarkup };
        }

        private object[] HandleEditTraceSecondStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];
            ReplyKeyboardMarkup?[] keyboardMarkup = new ReplyKeyboardMarkup[2];

            string trace = _traces[chatId];

            var traceMode = _databaseReader.GetTraceMode(chatId, trace);

            if (_traceStage[chatId] == 0)
            {
                messageResponse = SetTraceMode(trace, traceMode, chatId);
            }
            else if (_traceStage[chatId] == 1)
            {
                if (traceMode == TraceMode.OnPriceChageAbsolutely)
                {
                    messageResponse = SetOnPriceChangeAbsolutelySettings(messageText, trace, chatId);
                }
                else if (traceMode == TraceMode.OnPriceChageRelatively)
                {
                   messageResponse = SetOnPriceChangeRelativelySettings(messageText, trace, chatId);
                }
                else if (traceMode == TraceMode.AfterTime)
                {
                    messageResponse = SetAfterTimeSettings(messageText, trace, chatId);
                }
            } 
            return new object[] { messageResponse, keyboardMarkup };
        }

        private object[] HandleFirstStageCommand(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];
            ReplyKeyboardMarkup?[] keyboardMarkup = new ReplyKeyboardMarkup[2];

            if (messageText == StartCommands.Start)
            {
                messageResponse = Start(chatId);
            }
            else if (messageText == StartCommands.TraceNew)
            {
                messageResponse = TraceNewFirstStage(messageText, chatId);
            }
            else if (messageText == StartCommands.DeleteTrace)
            {
                messageResponse = DeleteTraceFirstStage(messageText, chatId);
            }
            else if (messageText == StartCommands.ChangeVsCurrency)
            {
                messageResponse = ChangeVsCurrencyFirstStage(messageText, chatId);
            }
            else if (messageText == StartCommands.GetMyInfo)
            {
                GetMyInfo(chatId);
            }
            else if (messageText == StartCommands.TraceList)
            {
                messageResponse = GetTraceList(messageText, chatId);
            }
            else if (messageText == StartCommands.TraceSettingsList)
            {
                messageResponse = GetTraceListWithSettings(messageText, chatId);
            }
            else if (messageText == StartCommands.EditTrace)
            {
                messageResponse = EditTraceFirstStage(messageText, chatId);
            }
            else if (messageText == StartCommands.Help)
            {
                messageResponse = GetHelp();
            }

            return new object[] { messageResponse, keyboardMarkup };
        }


        private string[] TraceNewSecondStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];
            if (_tracer.IsCoinExisting(messageText))
            {
                try
                {
                    _databaseReader.AddTracketCoin(chatId, _tracer.GetCoinName(messageText));
                    messageResponse[0] = $"You are tracking {messageText} with default settings.";
                }
                catch
                {
                    messageResponse[0] = SecondStageCommandsResponces.TraceNewAlreadyTracked;
                }
            }
            else
            {
                messageResponse[0] = SecondStageCommandsResponces.TraceNewCoinNotFound;
            }
            return messageResponse;
        }

        private string[] DeleteTraceSecondStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];
            try
            {
                _databaseReader.RemoveTrackedCoin(chatId, messageText);
                messageResponse[0] = SecondStageCommandsResponces.DeleteTraceSuccesful;
            }
            catch
            {
                messageResponse[0] = SecondStageCommandsResponces.DeleteTraceNotFound;
            }
            return messageResponse;
        }

        private string[] ChangeVsCurrencySecondStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];

            if (_tracer.GetVsCurrencies().Contains(messageText.ToLower()))
            {
                messageResponse[0] = $"Your vs currency has been changed to {messageText}.";
                _databaseReader.ChangeVsCurrency(chatId, messageText);
            }
            else
            {
                messageResponse[0] = SecondStageCommandsResponces.ChangeVsCurrencyNoSuchCurrency;
            }

            return messageResponse;
        }

        private object[] EditTraceSecondStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];
            ReplyKeyboardMarkup?[] keyboardMarkup = new ReplyKeyboardMarkup[2];

            if (_databaseReader.IsCoinTracked(chatId, messageText))
            {
                _traces.Add(chatId, _databaseReader.GetCoinName(chatId, messageText));
                _traceStage.Add(chatId, 0);

                messageResponse[0] = SecondStageCommandsResponces.EditTrace;
                keyboardMarkup[0] = new(new[]{
                                new KeyboardButton [] { "On price change absolutely" },
                                new KeyboardButton [] { "On price change relatively" },
                                new KeyboardButton [] { "After time"}
                });
            }
            else
            {
                messageResponse[0] = SecondStageCommandsResponces.EditTraceNoSuchCoin;
            }

            return new object[] { messageResponse, keyboardMarkup };
        }

        private string[] SetTraceMode(string trace, TraceMode traceMode, long chatId)
        {
            string[] messageResponse = new string[2];

            _databaseReader.SetTraceMode(chatId, trace, traceMode);

            messageResponse[0] = TraceSecondStageResponse.TraceModeChanged;
            _traceStage[chatId] = 1;

            if (traceMode == TraceMode.OnPriceChageAbsolutely)
            {
                messageResponse[1] = TraceSecondStageResponse.OnPriceChangeRelativelyExample;
            }
            else if (traceMode == TraceMode.OnPriceChageRelatively)
            {
                messageResponse[1] = TraceSecondStageResponse.OnPriceChangeRelativelyExample;
            }
            else if (traceMode == TraceMode.AfterTime)
            {
                messageResponse[1] = TraceSecondStageResponse.OnPriceChangeAfterTimeExample;
            }

            return messageResponse;
        }
        private string[] SetOnPriceChangeAbsolutelySettings(string messageText, string trace, long chatId)
        {
            string[] messageResponse = new string[2];
            try
            {
                string[] priceString = messageText.Split('-');
                decimal min = decimal.Parse(priceString[0]);
                decimal max = decimal.Parse(priceString[1]);

                if (max < min)
                {
                    var temp = min;
                    min = max;
                    max = temp;
                }

                _databaseReader.SetMinValue(chatId, trace, min);
                _databaseReader.SetMaxValue(chatId, trace, max);
            }
            catch
            {
                messageResponse[0] = TraceSecondStageResponse.SomethingIsWrongNumbers;
            }
            _traceStage.Remove(chatId);
            _traces.Remove(chatId);

            return messageResponse;
        }

        private string[] SetOnPriceChangeRelativelySettings(string messageText, string trace, long chatId)
        {
            string[] messageResponse = new string[2];
            try
            {
                messageResponse[0] = TraceSecondStageResponse.PercentageSet;
                _databaseReader.SetPercents(chatId, trace, decimal.Parse(messageText));

                messageResponse[1] = TraceSecondStageResponse.AllSettingsSet;
            }
            catch
            {
                messageResponse[0] = TraceSecondStageResponse.SomethingIsWrongPercentage;
            }
            _traceStage.Remove(chatId);
            _traces.Remove(chatId);

            return messageResponse;
        }

        private string[] SetAfterTimeSettings(string messageText, string trace, long chatId)
        {
            string[] messageResponse = new string[2];
            try
            {
                string[] dateString = messageText.Split(':');
                int hours = Convert.ToInt32(dateString[0]);
                int minutes = Convert.ToInt32(dateString[1]);

                _databaseReader.SetTime(chatId, trace, new DateTime(0001, 01, 01, hours, minutes, 0));

                messageResponse[0] = TraceSecondStageResponse.TimePeriodSet;
                messageResponse[1] = TraceSecondStageResponse.AllSettingsSet;
            }
            catch
            {
                messageResponse[0] = TraceSecondStageResponse.SomethingIsWrongTime;
            }
            _traceStage.Remove(chatId);
            _traces.Remove(chatId);

            return messageResponse;
        }


        private string[] Start(long chatId)
        {
            string[] messageResponse = new string[2];
            Console.WriteLine($"bot started in {chatId}");

            messageResponse[0] = FirstStageCommandsResponces.StartResponce;
            _databaseReader.AddUser(chatId);

            return messageResponse;
        }

        private string[] TraceNewFirstStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];

            messageResponse[0] = FirstStageCommandsResponces.TraceNewResponce;
            _usersCommand.Add(chatId, messageText);

            return messageResponse;
        }

        private string[] DeleteTraceFirstStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];

            messageResponse[0] = FirstStageCommandsResponces.DeleteTraceResponce;
            _usersCommand.Add(chatId, messageText);

            return messageResponse;
        }

        private string[] ChangeVsCurrencyFirstStage(string messageText, long chatId)
        {
            string[] messageResponse = new string[2];

            messageResponse[0] = FirstStageCommandsResponces.ChangeVsCurrency;

            StringBuilder coins = new StringBuilder();
            foreach (string currency in _tracer.GetVsCurrencies())
            {
                coins.Append("- " + currency + "\n");
            }
            messageResponse[0] += coins.ToString();
            _usersCommand.Add(chatId, messageText);

            return messageResponse;
        }

        private void GetMyInfo(long chatId)
        {
            _tracer.CheckUserCoins((int)chatId, _botClient, _cancellationToken)
                .GetAwaiter().GetResult();
        }

        private string[] GetTraceList(string messageText, long chatId)
        {
            string [] messageResponse = new string [2];

            var trackedCoins = _databaseReader.GetTrackedCoins(chatId);

            if (trackedCoins.Count > 0)
            {
                messageResponse[0] = FirstStageCommandsResponces.TraceListFullResponce;

                StringBuilder coins = new StringBuilder();
                foreach (TrackedCoin tracked in trackedCoins)
                {
                    coins.Append("- " + tracked.Coin + "\n");
                }
                messageResponse[0] += coins.ToString();
            }
            else
            {
                messageResponse[0] = FirstStageCommandsResponces.TraceListEmptyResponce;
            }

            return messageResponse;
        }

        private string[] GetTraceListWithSettings(string messageText, long chatId)
        {
            string [] messageResponse = new string [2];

            var trackedCoins = _databaseReader.GetTrackedCoins(chatId);

            if (trackedCoins.Count > 0)
            {
                messageResponse[0] = FirstStageCommandsResponces.TraceListFullResponce;
                foreach (TrackedCoin tracked in trackedCoins)
                {
                    var traceMode = tracked.TraceSettings.TracingMode;

                    StringBuilder coins = new StringBuilder();
                    coins.Append(tracked.Coin + ": \n");


                    if (traceMode == TraceMode.OnPriceChageAbsolutely)
                    {
                        coins.Append("- Tracing mode: on change price absolutely \n"
                            + $"- Min price: { tracked.TraceSettings.AbsoluteMin} \n"
                            + $"- Max price: {tracked.TraceSettings.AbsoluteMax} \n");
                    }
                    else if (traceMode == TraceMode.OnPriceChageRelatively)
                    {
                        coins.Append("- Tracing mode: on change price relatively \n" +
                            $"- Percentage: {tracked.TraceSettings.Persent} \n");
                    }
                    else if (traceMode == TraceMode.AfterTime)
                    {
                        coins.Append("- Tracing mode: after time \n"
                            + $"- Time: {((DateTime)tracked.TraceSettings.Time).ToString("HH:mm")} \n");
                    }

                    coins.Append("\n \n");

                    messageResponse[0] += coins.ToString();
                }
            }
            else
            {
                messageResponse[0] = FirstStageCommandsResponces.TraceListEmptyResponce;
            }

            return messageResponse;
        }

        private string[] EditTraceFirstStage(string messageText, long chatId)
        {
            string [] messageResponse = new string[2];

            messageResponse[0] = FirstStageCommandsResponces.EditTraceResponce;
            _usersCommand.Add(chatId, messageText);

            return messageResponse;
        }

        private string[] GetHelp()
        {
            string[] messageResponse = new string[2];
            messageResponse[0] = FirstStageCommandsResponces.HelpResponce;
            return messageResponse;
        }


        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

        private void Escape(long chatId)
        {
            if (_usersCommand.ContainsKey(chatId))
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
        }
    }
}
