using System;
using System.Collections.Generic;
using System.Linq;

namespace gremlin_check
{
    public class Bill
    {
        public string OwedParty { get; set; }
        public decimal TotalOwed { get; set; }
        // public Tuple<string, decimal> PayingParties { get; set; }
    }
}