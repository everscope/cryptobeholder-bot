using CryptoBeholder.Lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBeholderBot
{
    public class Tracer : ITracer
    {
        public CoinGecko.Entities.Response.Coins.CoinList[] CoinsList { get; private set; }
        public List<CoinGecko.Entities.Response.Coins.CoinMarkets[]> CoinsData { get; private set; } =
                                                new List<CoinGecko.Entities.Response.Coins.CoinMarkets[]>();
        private List<string> VsCurrencies = new List<string>
        {
            "usd",
            "eth",
            "eur",
            "rub",
            "xrp",
            "btc"
        };

        private readonly IServiceProvider _serviceProvider;

        private readonly string _coinsDataRequestArgs = "&order=market_cap_desc&per_page=250&page=1&sparkline=false";

        public Tracer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Start(TelegramBotClient botClient, CancellationTokenSource cancellationTokenSource)
        {
            var coinListResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinList);
            if (coinListResponse.IsSuccessStatusCode)
            {
                CoinsList = await coinListResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinList[]>();
            }

            foreach (string currency in VsCurrencies)
            {

                var coinsDataResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinMarkets
                           + "?vs_currency=" + currency + _coinsDataRequestArgs);

                if (coinsDataResponse.IsSuccessStatusCode)
                {
                    CoinsData.Add(await coinsDataResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinMarkets[]>());
                }
            }

            new Thread(() =>
            {
                TracePrice();
            }).Start();

            new Thread(() =>
            {
                TrackChanges(botClient, cancellationTokenSource);
            }).Start();
        }

        public List<string> GetVsCurrencies() 
        {
            return VsCurrencies;
        }

        public bool IsCoinExisting(string coinName)
        {
            return CoinsList.Any(p => p.Name.ToLower() == coinName.ToLower());
        }

        public string GetCoinName(string coin)
        {
            return CoinsList.First(p => p.Name.ToLower() == coin.ToLower()).Name;
        }

        public async Task CheckUserCoins(int chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
        {
            int a = VsCurrencies.Count;
            int v = CoinsData.Count;
            if (VsCurrencies.Count < CoinsData.Count)
            {
                Message sentSecondMessage = await botClient.SendTextMessageAsync(
                     chatId: chatId,
                     text: "Please, wait till database will be initalizated.",
                     cancellationToken: cancellationToken);
            }
            else
            {
                User user = null;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var databaseReader = scope.ServiceProvider.GetRequiredService<IDatabaseReader>();
                    user = databaseReader.GetUser(chatId);
                }
                //var user = _databaseReader.GetUser(chatId);
                if (user != null)
                {

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append("Info: \n");
                    int index = VsCurrencies.FindIndex(p => p.Contains(user.VsCurrency));

                    foreach (TrackedCoin coin in user.TrackedCoins)
                    {
                        try
                        {
                            stringBuilder.Append($"- {coin.Coin}: {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice}" +
                                $" in {user.VsCurrency} \n");
                        }
                        catch
                        {
                            stringBuilder.Append("Coin data isn't loaded yet");
                        }

                    }

                    string message = stringBuilder.ToString();

                    Message sentSecondMessage = await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: message,
                            cancellationToken: cancellationToken);
                }
            }
        }

        private async void TracePrice()
        {

            while (CoinsData.Count < VsCurrencies.Count)
            {
                Thread.Sleep(1000);
            }

            while (true)
            {

                int i = 0;
                foreach (string currency in VsCurrencies)
                {
                    var coinsDataResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinMarkets
                               + "?vs_currency=" + currency + _coinsDataRequestArgs);

                    if (coinsDataResponse.IsSuccessStatusCode)
                    {
                        CoinsData[i] = await coinsDataResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinMarkets[]>();
                    }

                    i++;
                }

                Thread.Sleep(1000);
            }
        }

        private async void TrackChanges(TelegramBotClient botClient, CancellationTokenSource cancellationTokenSource)
        {
            while (CoinsData.Count < VsCurrencies.Count)
            {
                Thread.Sleep(1200);
            }

            string? message;

            while (true)
            {
                User[]? users = null;

                using (var scope = _serviceProvider.CreateScope())
                {
                    var databaseReader = scope.ServiceProvider.GetRequiredService<IDatabaseReader>();
                    users = databaseReader.GetAllUsers();
                }
                foreach (User user in users)
                {
                    int index = VsCurrencies.FindIndex(p => p.Contains(user.VsCurrency));
                    if (index == -1)
                    {
                        break;
                    }
                    foreach (TrackedCoin coin in user.TrackedCoins)
                    {
                        message = null;
                        if (coin.TraceSettings.TracingMode == TraceMode.OnPriceChageAbsolutely)
                        {
                            if (CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice > coin.TraceSettings.AbsoluteMax
                                    && !coin.TraceSettings.MaxIsReached)
                            {
                                message = $"Attention! \n Your tracked coin {coin.Coin} reached max or min price you selected. \n" +
                                    $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}";
                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var databaseReader = scope.ServiceProvider.GetRequiredService<IDatabaseReader>();
                                    databaseReader.SetAbsoluteMaxReached(user.ChatId, coin.Coin);
                                }
                            }
                            else if (CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice < coin.TraceSettings.AbsoluteMin
                                && !coin.TraceSettings.MinIsReached)
                            {
                                message = $"Attention! \n Your tracked coin {coin.Coin} reached max or min price you selected. \n" +
                                    $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}";

                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var databaseReader = scope.ServiceProvider.GetRequiredService<IDatabaseReader>();
                                    databaseReader.SetAbsoluteMinReached(user.ChatId, coin.Coin);
                                }

                            }
                        }
                        else if (coin.TraceSettings.TracingMode == TraceMode.OnPriceChageRelatively)
                        {
                            if (Math.Abs(Convert.ToDecimal(CoinsData[index].First(p => p.Name == coin.Coin)?.PriceChangePercentage24H))
                                    >= Math.Abs(Convert.ToDecimal(coin.TraceSettings.Persent)))
                            {
                                if (CoinsData[index].First(p => p.Name == coin.Coin).PriceChangePercentage24H >= 0
                                    && !coin.TraceSettings.PersentPositiveIsReached)
                                {
                                    message = $"Attention! \n Your tracked coin {coin.Coin} reached the percentage you selected. \n" +
                                    $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}, " +
                                    $"that is {CoinsData[index].First(p => p.Name == coin.Coin).PriceChange24H} % change.";

                                    using (var scope = _serviceProvider.CreateScope())
                                    {
                                        var databaseReader = scope.ServiceProvider.GetRequiredService<IDatabaseReader>();
                                        databaseReader.SetPercentMaxReached(user.ChatId, coin.Coin);
                                    }
                                }
                                else if (CoinsData[index].First(p => p.Name == coin.Coin).PriceChangePercentage24H < 0
                                    && !coin.TraceSettings.PersentNegativeIsReached)
                                {
                                    message = $"Attention! \n Your tracked coin {coin.Coin} reached the percentage you selected. \n" +
                                    $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}, " +
                                    $"that is {CoinsData[index].First(p => p.Name == coin.Coin).PriceChange24H} % change.";

                                    using (var scope = _serviceProvider.CreateScope())
                                    {
                                        var databaseReader = scope.ServiceProvider.GetRequiredService<IDatabaseReader>();
                                        databaseReader.SetPercentMinReached(user.ChatId, coin.Coin);
                                    }

                                }
                            }
                        }
                        else if (coin.TraceSettings.TracingMode == TraceMode.AfterTime)
                        {
                            if (DateTime.Now.TimeOfDay - coin.TraceSettings.Timestamp.TimeOfDay > coin.TraceSettings.Time.TimeOfDay)
                            {
                                message = $"Attention! \n Your tracked coin {coin.Coin} is now " +
                                    $"{CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}.";

                                using (var scope = _serviceProvider.CreateScope())
                                {
                                    var databaseReader = scope.ServiceProvider.GetRequiredService<IDatabaseReader>();
                                    databaseReader.SetTimeStamp(user.ChatId, coin.Coin);
                                }

                            }
                        }

                        if (message != null)
                        {
                            Message sentSecondMessage = await botClient.SendTextMessageAsync(
                                chatId: user.ChatId,
                                text: message,
                                cancellationToken: cancellationTokenSource.Token);
                        }
                    }
                }
            }
        }
    }
}
