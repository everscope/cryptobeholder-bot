using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    public class TrackedCoin
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public string TrackedId { get; set; }
        public string Coin { get; set; }
        public User User { get; set; }

        public TraceSettings TraceSettings{ get; set; } = new TraceSettings();
    }
}
