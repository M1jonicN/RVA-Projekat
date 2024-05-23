using Common.DbModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public static class PibHelper
    {
        public static string GenerateUniquePIB(List<Gallery> _allGalleries)
        {
            string pib;
            do
            {
                pib = GeneratePIB();
            } while (_allGalleries.Any(g => g.PIB == pib));

            return pib;
        }

        public static readonly Random _random = new Random();

        private static string GeneratePIB()
        {
            // Generate a random number between 10000001 and 99999999
            int number = _random.Next(10000001, 99999999);

            // Convert the number to a string
            string pib = number.ToString("D8");

            // Calculate the control digit (simple example, customize as needed)
            int controlDigit = CalculateControlDigit(pib);

            // Append the control digit to the PIB
            return pib + controlDigit.ToString();
        }

        private static int CalculateControlDigit(string pib)
        {
            // Implement a control digit calculation (example provided)
            // This is a simple checksum calculation, you may need a different algorithm
            int sum = 0;
            for (int i = 0; i < pib.Length; i++)
            {
                sum += (pib[i] - '0') * (i + 1);
            }

            // Modulus 11 to get a single digit
            int controlDigit = sum % 11;
            return controlDigit;
        }
    }
}
