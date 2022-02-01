using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBeholderBot
{
    internal class Tracer
    {
        //public static CoinGecko.Entities.Response.Coins.CoinMarkets[] CoinsData { get; private set; }
        //public static CoinGecko.Entities.Response.Simple.SupportedCurrencies VsCurrencies { get; private set; }
        public static CoinGecko.Entities.Response.Coins.CoinList[] CoinsList { get; private set; }
        public static List<CoinGecko.Entities.Response.Coins.CoinMarkets[]> CoinsData { get; private set; } =
                                                new List<CoinGecko.Entities.Response.Coins.CoinMarkets[]>();
        public static List<string> VsCurrencies { get; private set; } = new List<string>
        {
            "usd",
            "eth",
            "eur",
            "rub",
            "xrp",
            "btc"
        };

        private static UserContext _userContext;
            
        //public delegate void OnTraced();

        //public static event OnTraced OnTracedAbsolute;
        //public static event OnTraced OnTracedRelative;
        //public static event OnTraced OnTracedTime;


        public static async Task Start(TelegramBotClient botClient, CancellationTokenSource cancellationTokenSource)
        {
            //_userContext = new UserContext();
            var coinListResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinList);
            if (coinListResponse.IsSuccessStatusCode)
            {
                CoinsList = await coinListResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinList[]>();
            }

            //var coinsDataResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinMarkets
            //                            + "?vs_currency=usd&page=1&sparkline=false");
            //if (coinsDataResponse.IsSuccessStatusCode)
            //{
            //    CoinsData = await coinsDataResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinMarkets []>();
            //}

            foreach (string currency in VsCurrencies)
            {

                var coinsDataResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinMarkets
                           + "?vs_currency=" + currency + "&order=market_cap_desc&per_page=250&page=1&sparkline=false");

                if (coinsDataResponse.IsSuccessStatusCode)
                {
                    CoinsData.Add(await coinsDataResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinMarkets[]>());
                }
            }

            new Thread(() =>
            {
                Trace();
            }).Start();

            new Thread(() =>
            {
                TrackChanges(botClient, cancellationTokenSource);
            }).Start();
        }



        private static async void Trace()
        {

            while (CoinsData.Count < VsCurrencies.Count)
            {
                Thread.Sleep(1000);
            }

            //while (true)
            //{

            //    int i = 0;
            //    foreach (string currency in VsCurrencies)
            //    {
            //        var coinsDataResponse = await ApiClient.Client.GetAsync(CoinGecko.ApiEndPoints.CoinsApiEndPoints.CoinMarkets
            //                   + "?vs_currency=" + currency + "&order=market_cap_desc&per_page=250&page=1&sparkline=false");

            //        if (coinsDataResponse.IsSuccessStatusCode)
            //        {
            //            CoinsData[i] = await coinsDataResponse.Content.ReadAsAsync<CoinGecko.Entities.Response.Coins.CoinMarkets[]>();
            //        }

            //        i++;
            //    }

            //    Thread.Sleep(1000);
            //}
        }

        private static async void TrackChanges(TelegramBotClient botClient, CancellationTokenSource cancellationTokenSource)
        {
            while (/*_userContext == null *//*&&*/ CoinsData.Count < VsCurrencies.Count)
            {
                Thread.Sleep(1200);
            }

            string? message = null;

            while (true)
            {
                using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions
                {
                    IsolationLevel = System.Transactions.IsolationLevel.Snapshot
                }))
                {
                    using (var data = new UserContext())
                    {
                        foreach (User user in data.Users)
                        {
                            int index = VsCurrencies.FindIndex(p => p.Contains(user.VsCurrency));
                            if (index == -1)
                            {
                                break;
                            }
                            foreach (TrackedCoin coin in user.TrackedCoins)
                            {
                                message = null;
                                switch (coin.TraceSettings.TracingMode)
                                {
                                    case TraceMode.OnPriceChageAbsolutely:
                                        //TracedChangeHandler(user.ChatId, coin.Coin, coin.TraceSettings.AbsoluteMax, coin.TraceSettings.AbsoluteMin);
                                        if (CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice > coin.TraceSettings.AbsoluteMax
                                            && !coin.TraceSettings.MaxIsReached)
                                        /*|| CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice < coin.TraceSettings.AbsoluteMin*/
                                        {
                                            message = $"Attention! \n Your tracked coin {coin.Coin} reached max or min price you selected. \n" +
                                                $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}";

                                            UserContext userUpdate = new UserContext();

                                            userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                .MaxIsReached = true;
                                            userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                .MinIsReached = true;
                                            await userUpdate.SaveChangesAsync();
                                        }
                                        else if (CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice < coin.TraceSettings.AbsoluteMin
                                            && !coin.TraceSettings.MinIsReached)
                                        {
                                            message = $"Attention! \n Your tracked coin {coin.Coin} reached max or min price you selected. \n" +
                                                $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}";

                                            UserContext userUpdate = new UserContext();

                                            userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                .MaxIsReached = false;
                                            userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                .MinIsReached = true;
                                            await userUpdate.SaveChangesAsync();
                                        }
                                        break;
                                    case TraceMode.OnPriceChageRelatively:
                                        if (Math.Abs(Convert.ToDecimal(CoinsData[index].First(p => p.Name == coin.Coin).PriceChangePercentage24H))
                                            >= Math.Abs(Convert.ToDecimal(coin.TraceSettings.Persent)))
                                        {
                                            if (CoinsData[index].First(p => p.Name == coin.Coin).PriceChangePercentage24H >= 0
                                                && !coin.TraceSettings.PersentPositiveIsReached)
                                            {
                                                message = $"Attention! \n Your tracked coin {coin.Coin} reached the percentage you selected. \n" +
                                                $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}, " +
                                                $"that is {CoinsData[index].First(p => p.Name == coin.Coin).PriceChange24H} % change.";

                                                UserContext userUpdate = new UserContext();

                                                userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                    .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                    .PersentPositiveIsReached = true;
                                                userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                    .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                    .PersentNegativeIsReached = false;
                                                await userUpdate.SaveChangesAsync();
                                            }
                                            else if (CoinsData[index].First(p => p.Name == coin.Coin).PriceChangePercentage24H < 0
                                                && !coin.TraceSettings.PersentNegativeIsReached)
                                            {
                                                message = $"Attention! \n Your tracked coin {coin.Coin} reached the percentage you selected. \n" +
                                                $"Now it's {CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}, " +
                                                $"that is {CoinsData[index].First(p => p.Name == coin.Coin).PriceChange24H} % change.";

                                                UserContext userUpdate = new UserContext();

                                                userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                    .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                    .PersentPositiveIsReached = false;
                                                userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                    .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                    .PersentNegativeIsReached = true;
                                                await userUpdate.SaveChangesAsync();
                                            }
                                        }
                                        break;
                                    case TraceMode.AfterTime:
                                        if (DateTime.Now.TimeOfDay - coin.TraceSettings.Timestamp.TimeOfDay > coin.TraceSettings.Time.TimeOfDay)
                                        {
                                            message = $"Attention! \n Your tracked coin {coin.Coin} is now " +
                                                $"{CoinsData[index].First(p => p.Name == coin.Coin).CurrentPrice} in {user.VsCurrency}.";

                                            //Program._userContext.Users.First(p => p.ChatId == user.ChatId)
                                            //    .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                            //    .Timestamp = DateTime.Now;
                                            //await Program._userContext.SaveChangesAsync();

                                            UserContext userUpdate = new UserContext();

                                            userUpdate.Users.First(p => p.ChatId == user.ChatId)
                                                .TrackedCoins.First(p => p.Coin == coin.Coin).TraceSettings
                                                .Timestamp = DateTime.Now;
                                            await userUpdate.SaveChangesAsync();

                                        }
                                        break;
                                    default:
                                        break;
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
    }
}
