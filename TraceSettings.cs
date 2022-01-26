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
        public int Id { get; set; }
        public decimal? AbsoluteMax { get; set; }
        public decimal? AbsoluteMin { get; set;}
        public decimal? Persent { get; set; } = 5;
        public string? TracingMode { get; set; } = TraceMode.OnPriceChageRelatively.ToString();
        public DateTime? Time { get; set; } = new DateTime(0001, 1, 1, 1, 0, 0);

        public TrackedCoin TrackedCoin { get; set; }
    }

    public enum TraceMode
    {
        OnPriceChageAbsolutely,
        OnPriceChageRelatively,
        AfterTime
    }
}
