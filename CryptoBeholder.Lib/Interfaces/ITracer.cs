using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CryptoBeholder.Lib.Interfaces
{
    public interface ITracer
    {
        public Task Start(TelegramBotClient botClient, CancellationTokenSource cancellationTokenSource);
        public Task CheckUserCoins(int chatId, ITelegramBotClient botClient, CancellationToken cancellationToken);
        public bool IsCoinExisting(string coinName);
        public string GetCoinName(string coin);
        public List<string> GetVsCurrencies();
    }
}
