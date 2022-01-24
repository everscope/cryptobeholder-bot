using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    //Default:
    //TraceMode - onPriceChangeRelatively
    //Persent - 5
    //Time - 1 hour

    internal class TraceSettings
    {
       ///* public TraceMode TracingMode */{ get; set; }
        public decimal AbsoluteMax { get; set; }
        public decimal AbsoluteMin { get; set; }
        public decimal Persent { get; set; }
        //public TimeOnly Time { get; set; }

        public string chatIdAndCoin { get; set; }
    }

    public enum TraceMode
    {
        OnPriceChageAbsolutely,
        OnPriceChageRelatively,
        AfterTime
    }
}
