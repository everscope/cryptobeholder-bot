using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    internal class TrackedCoin
    {
        public string ChatId { get; set; }
        public string ChatIdAndCoin { get; set; }
        public CoinGecko.Entities.Response.Coins.CoinList Coin { get; set; }
        public TraceSettings TraceSet { get; set; }
    }
}
