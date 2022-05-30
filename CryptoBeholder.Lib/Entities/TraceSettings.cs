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
        [Precision(18, 3)]
        public decimal? AbsoluteMax { get; set; }
        public bool MaxIsReached { get; set; } = false;
        [Precision(18, 3)]
        public decimal AbsoluteMin { get; set;}
        public bool MinIsReached { get; set; } = false;
        [Precision(5, 2)]
        public decimal? Persent { get; set; } = 5;
        public bool PersentPositiveIsReached { get; set; } = false;
        public bool PersentNegativeIsReached { get; set; } = false;
        public TraceMode TracingMode { get; set; } = TraceMode.OnPriceChageRelatively;
        public DateTime Time { get; set; } = new DateTime(0001, 1, 1, 1, 0, 0);
        public DateTime Timestamp { get; set; } = new DateTime(0001, 1, 1, 1, 0, 0);
        public bool IsNotificationSent { get; set; } = false;
    }

    public enum TraceMode
    {
        OnPriceChageAbsolutely,
        OnPriceChageRelatively,
        AfterTime
    }
}
