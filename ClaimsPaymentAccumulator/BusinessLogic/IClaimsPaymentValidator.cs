using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClaimsPaymentAccumulator.Models;

namespace ClaimsPaymentAccumulator.BusinessLogic
{
    public interface IClaimsPaymentValidator
    {
        Action<string> Log { get; set; }
        Payment ValidateGetPayment(string[] record, int lineNo);
        IEnumerable<Payment> ValidateAllPayments(IEnumerable<Payment> payments);
    }
}
