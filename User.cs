using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBeholderBot
{
    public class User
    {
        //[Key]
        //public int ChatId { get; set; }
        [Key]
        public int ChatId { get; set; }
        public string VsCurrency { get; set; } = "usd";
        public ICollection<TrackedCoin> TrackedCoins { get; set; }
    }
}
