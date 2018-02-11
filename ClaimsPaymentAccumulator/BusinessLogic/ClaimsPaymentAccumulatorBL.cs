using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClaimsPaymentAccumulator.Models;

namespace ClaimsPaymentAccumulator.BusinessLogic
{
    public class ClaimsPaymentAccumulatorBL
    {
        public Action<string> Log { get; set; }
        private IClaimsPaymentIO PaymentIO { get; set; }
        private IClaimsPaymentValidator PaymentValidator { get; set; }
        private IEnumerable<Payment> Payments { get; set; }
        

        public ClaimsPaymentAccumulatorBL(IClaimsPaymentIO _claimsPaymentIo, IClaimsPaymentValidator _validator, Action<string> Log)
        {
            PaymentIO = _claimsPaymentIo;
            PaymentIO.Log = Log;
            
            PaymentValidator = _validator;
            PaymentValidator.Log = Log;

            PaymentIO.ValidateGetPaymentFunc = PaymentValidator.ValidateGetPayment;
        }

        public void ProcessPayments(string inputFile, string outputFile)
        {
            try
            {
                //read and do initial validation
                Payments = PaymentIO.ReadPayments(inputFile);

                //more validations
                Payments = PaymentValidator.ValidateAllPayments(Payments);

                //outpu
                PaymentIO.WriteCumulativePayments(outputFile, Payments);
            }
            catch (Exception e)
            {
                Log("Failed to process payments. " + e.Message);
            }
        }
    }
}
