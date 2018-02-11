using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimsPaymentAccumulator.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Product { get; set; }
        public int OriginYear { get; set; }
        public int DevelopmentYear { get; set; }
        public decimal IncrementalValue { get; set; } 
    }
}
