using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketDZ.Models
{
    public class Favorite
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
