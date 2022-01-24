using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    internal class User
    {
        public long ChatId { get; set; }

        public List<TrackedCoin> TrackedCoins { get; set; }
        //public CoinGecko.Entities.Response.Simple.SupportedCurrencies VsCurrency { get; set; }
        public string VsCurrency { get; set; }
    }
}
