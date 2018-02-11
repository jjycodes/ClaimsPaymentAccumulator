using System;
using System.Collections.Generic;
using ClaimsPaymentAccumulator.Models;

namespace ClaimsPaymentAccumulator.BusinessLogic
{
    public interface IClaimsPaymentIO
    {
        Action<string> Log { get; set; }
        Func<string[], int, Payment> ValidateGetPaymentFunc { get; set; }
        IEnumerable<Payment> ReadPayments(string inputFile);
        void WriteCumulativePayments(string outputFile, IEnumerable<Payment> payments);
    }
}