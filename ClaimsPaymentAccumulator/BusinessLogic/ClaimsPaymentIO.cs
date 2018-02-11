using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClaimsPaymentAccumulator.Models;


namespace ClaimsPaymentAccumulator.BusinessLogic
{
    public class ClaimsPaymentIO : IClaimsPaymentIO
    {
        public Action<string> Log { get; set; }

        public Func<string[], int, Payment> ValidateGetPaymentFunc { get; set; }

        public IEnumerable<Payment> ReadPayments(string inputFile)
        {
            var payments = new List<Payment>();
            char[] delimiters = { ',' };
            using (StreamReader reader = new StreamReader(inputFile))
            {
                var lineNo = 1;
                while (true)
                {
                    string line = reader.ReadLine();

                    if (lineNo == 1) //Skip headings
                    {
                        lineNo++;
                        continue;
                    }
                    
                    if (line == null)
                    {
                        break;
                    }

                    string[] parts = line.Split(delimiters);
                    var payment = ValidateGetPaymentFunc(parts, lineNo);

                    if (payment != null)
                        payments.Add(payment);
                    // Console.WriteLine("{0} field(s)", parts.Length);


                    lineNo++;
                }
            }

            //Do validation here or on another class, esp if there are multiple entries per development year
            return payments;
        }

        public void WriteCumulativePayments(string outputFile, IEnumerable<Payment> payments)
        {
            var originYears = payments.Select(payment => payment.OriginYear).ToList();
            originYears.Sort();

            var earliestOriginYear = originYears.First();
            var latestOriginYear = originYears.Last();


            using (StreamWriter writer = File.CreateText(outputFile))
            {
                writer.WriteLine($"{earliestOriginYear}, {latestOriginYear - earliestOriginYear + 1}");

                foreach (var paymentProducts in payments.GroupBy(payment => payment.Product)) //iterate all products
                {
                    StringBuilder output = new StringBuilder();

                    output.Append($"{paymentProducts.Key}, ");

                    for (int currentOriginYear = earliestOriginYear;
                        currentOriginYear <= latestOriginYear;
                        currentOriginYear++) //iterate all origin years
                    {

                        var YearIncrementsDictionary = paymentProducts
                            .Where(payment => payment.OriginYear == currentOriginYear)
                            .ToDictionary(key => $"{key.OriginYear}-{key.DevelopmentYear}",
                                value => value.IncrementalValue);

                        decimal accumulativeValue = 0;
                        for (int developmentYear = currentOriginYear;
                            developmentYear <= latestOriginYear;
                            developmentYear++) //iterate all development years per origin year
                        {
                            decimal currentIncrementalValue;
                            if (YearIncrementsDictionary.TryGetValue($"{currentOriginYear}-{developmentYear}",
                                out currentIncrementalValue))
                            {
                                accumulativeValue += currentIncrementalValue;
                            }
                            else
                            {
                                accumulativeValue += 0;
                            }

                            output.Append($"{accumulativeValue:G29}, ");
                        }


                    }


                    //remove last trailing comma
                    var productValues = output.ToString();
                    writer.Write(productValues.TrimEnd(',', ' '));
                    writer.WriteLine();
                }

                writer.Flush();
            }
        }
    }
}
