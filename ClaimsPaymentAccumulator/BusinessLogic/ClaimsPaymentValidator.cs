using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClaimsPaymentAccumulator.Models;

namespace ClaimsPaymentAccumulator.BusinessLogic
{
    public class ClaimsPaymentValidator : IClaimsPaymentValidator
    {
        public bool OverrideDuplicatePayments { get; set; }
        public Action<string> Log { get; set; }
        public Payment ValidateGetPayment(string[] record, int lineNo)
        {
            if (record.Length == 4)
            {
                int originYear, developmentYear;
                decimal incrementalValue;
                if (string.IsNullOrEmpty(record[0].Trim()))
                {
                    Log($"Product name cannot be empty on Line Number {lineNo}. Skipping this record");
                    return null;
                }
                if (Int32.TryParse(record[1], out originYear) == false)
                {
                    Log($"Invalid origin year value on Line Number {lineNo}. Skipping this record");
                    return null;
                }
                if (!Regex.IsMatch(record[1].Trim(), "^(18|19|20|21)[0-9][0-9]$"))   //date year should fall from 1800 - 2100
                {
                    Log($"Invalid origin year value on Line Number {lineNo}. Skipping this record");
                    return null;
                }
                if (Int32.TryParse(record[2], out developmentYear) == false)
                {
                    Log($"Invalid development year value on Line Number {lineNo}. Skipping this record");
                    return null;
                }
                if (!Regex.IsMatch(record[2].Trim(), "^(18|19|20|21)[0-9][0-9]$"))   //date year should fall from 1800 - 2100
                {
                    Log($"Invalid development year value on Line Number {lineNo}. Skipping this record");
                    return null;
                }
                if (decimal.TryParse(record[3], out incrementalValue) == false)
                {
                    Log($"Invalid incremental value on Line Number {lineNo}. Skipping this record");
                    return null;
                }

                var payment = new Payment()
                {
                    Id = lineNo,
                    Product = record[0],
                    OriginYear = originYear,
                    DevelopmentYear = developmentYear,
                    IncrementalValue = incrementalValue
                };

                return payment;
            }

            Log($"Invalid data length on Line Number {lineNo}. Make sure to provide exactly 4 values. Skipping this record");

            return null;
        }

        public IEnumerable<Payment> ValidateAllPayments(IEnumerable<Payment> payments)
        {
            var updatedPayments = new List<Payment>();

            var originYears = payments.Select(payment => payment.OriginYear).ToList();
            originYears.Sort();

            var earliestOriginYear = originYears.First();
            var latestOriginYear = originYears.Last();
            
            var originDevelopmentYearPairs = new Dictionary<string, int>();
            
            foreach (var payment in payments)
            {
                if (payment.DevelopmentYear < payment.OriginYear)
                {
                    Log(
                        $"Excluding invalid payment on Line Number {payment.Id}. Development year is earlier than the origin year.");
                    continue;
                }
                if (payment.DevelopmentYear < earliestOriginYear || payment.DevelopmentYear > latestOriginYear)
                {
                    Log(
                        $"Excluding invalid payment on Line Number {payment.Id}. Development year is not within range of total origin years.");
                    continue;
                }

                var key = $"{payment.Product}-{payment.OriginYear}-{payment.DevelopmentYear}";
                if (!originDevelopmentYearPairs.ContainsKey(key))
                {
                    originDevelopmentYearPairs.Add(key, payment.Id);
                }
                else
                {
                    if (OverrideDuplicatePayments)
                    {
                        //Remove old record, so that new record will be used instead
                        Log(
                            $"Duplicate payment found on Line Number {payment.Id}. Former payment overridden.");
                        
                        var oldPaymentId = originDevelopmentYearPairs[key];
                        var oldPayment = updatedPayments.FirstOrDefault(x => x.Id == oldPaymentId);
                        updatedPayments.Remove(oldPayment);
                    }
                    else
                    {
                        Log(
                            $"Duplicate payment found on Line Number {payment.Id}. Skipping this record.");
                        continue;
                    }
                }

                updatedPayments.Add(payment);
            }

            return updatedPayments;
        }
    }
}
