using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClaimsPaymentAccumulator.BusinessLogic;

namespace ClaimsPaymentAccumulator
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Starting claims payment processing.");
            Console.WriteLine();

            //use these 4 test data
            var file = "claims-data1.csv";
            //var file = "claims-data2.csv";
            //var file = "claims-data-witherror.csv";
            //var file = "claims-data-witherror2.csv";

            var inputFile = $"inputs\\{file}";
            var outputFile = $"outputs\\cumulative-{file}";

            var paymentProcessor = new ClaimsPaymentIO();
            var paymentValidator = new ClaimsPaymentValidator {OverrideDuplicatePayments = true};

            Console.WriteLine($"Reading incremental claims data on {inputFile}.");
            Console.WriteLine();

            var claimsPaymentAccumulator = new ClaimsPaymentAccumulatorBL(paymentProcessor, paymentValidator, Console.WriteLine);
            claimsPaymentAccumulator.ProcessPayments(inputFile, outputFile);
            
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"Cumulative claims data written to {outputFile}.");
            Console.ReadKey();
        }
    }
}
