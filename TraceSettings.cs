using Microsoft.EntityFrameworkCore;
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
    [Owned]
    public class TraceSettings
    {
        [Precision(12, 10)]
        public decimal? AbsoluteMax { get; set; }
        [Precision(12, 10)]
        public decimal? AbsoluteMin { get; set;}
        [Precision(3, 2)]
        public decimal? Persent { get; set; } = 5;
        public TraceMode? TracingMode { get; set; } = TraceMode.OnPriceChageRelatively;
        public DateTime? Time { get; set; } = new DateTime(0001, 1, 1, 1, 0, 0);
    }

    public enum TraceMode
    {
        OnPriceChageAbsolutely,
        OnPriceChageRelatively,
        AfterTime
    }
}
