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
    [Owned]
    public class TrackedCoin
    {
        [Key]
        public int TrackedId { get; set; }

        public string Coin { get; set; }
        public TraceSettings TraceSettings { get; set; } = new TraceSettings();

    }
}
