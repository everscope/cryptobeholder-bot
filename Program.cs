using CryptoBeholderBot;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using CoinGecko;

namespace CryptoBeholderBot {
    public static class Program {
        private static CoinGecko.Entities.Response.Simple.SupportedCurrencies [] _vsCurrencies;
        private static CoinGecko.Entities.Response.Coins.CoinList[] _coinsList;
        private static Dictionary<long, string> _usersCommand = new Dictionary<long, string>();

        public static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            ApiClient.Initialize();
            await InitializeStartData();
    
            

            var botClient = new TelegramBotClient("5231381256:AAFolea3xHyRaPPg-Olf1E_hJIOIOEtWQ3A");

            using var cts = new CancellationTokenSource();

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

        async static Task InitializeStartData()
        {
            var coinListResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinList);
            if (coinListResponse.IsSuccessStatusCode)
            {
                _coinsList = await coinListResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinList[]>();
            }

            var vsCurrencyResponce = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.SimpleApiEndPoints.SimpleSupportedVsCurrencies);
            if (vsCurrencyResponce.IsSuccessStatusCode)
            {
                _vsCurrencies = await vsCurrencyResponce.Content.ReadAsAsync<CoinGecko.Entities.Response.Simple.SupportedCurrencies[]>();
            }
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

            string? messageResponse = null;

            if (_usersCommand.ContainsKey(chatId)){
                string command = _usersCommand[chatId];

                switch (command)
                {
                    case "/trace_new":
                        if(_coinsList.Any(p => p.Name == messageText))
                        {
                            messageResponse = $"You are tracking {messageText} with default settings.";
                        }
                        else
                        {
                            messageResponse = $"There is no such coin";
                        }
                        _usersCommand.Remove(chatId);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (messageText)
                {
                    case "/start":
                        Console.WriteLine($"bot started in {chatId}");
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                        new KeyboardButton[] { "Help me", "Call me ☎️" },
                        })
                        {
                            ResizeKeyboard = true
                        };

                        break;
                    case "/trace_new":
                        messageResponse = "Enter coin's name:";
                        _usersCommand.Add(chatId, messageText);
                        break;
                    case "/delete_trace":
                        break;
                    case "/change_vs_currency":
                        break;
                    default:
                        break;
                }
            }

            if(messageResponse != null)
            {
                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: messageResponse,
                        replyMarkup: null,
                        cancellationToken: cancellationToken);
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

        //static Task ConnectDatabase()
        //{
        //    _userContext = new User(options => );
        //    //throw new NotImplementedException();
        //}
    }
}
