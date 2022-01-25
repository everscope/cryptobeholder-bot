using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    //Default:
    //TraceMode - onPriceChangeRelatively
    //Persent - 5
    //Time - 1 hour

    public class TraceSettings
    {
        [Key]
        public string CoinId { get; set; }
        public decimal? AbsoluteMax { get; set; }
        public decimal? AbsoluteMin { get; set;}
        public decimal? Persent { get; set; }
        public string? TracingMode { get; set; }
        public DateTime? Time { get; set; }

        public TrackedCoin TrackedCoin { get; set; }
    }

    public enum TraceMode
    {
        OnPriceChageAbsolutely,
        OnPriceChageRelatively,
        AfterTime
    }
}
