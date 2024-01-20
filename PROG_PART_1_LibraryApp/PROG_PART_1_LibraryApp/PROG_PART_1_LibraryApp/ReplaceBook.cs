using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PROG_PART_1_LibraryApp
{
    public class ReplaceBook
    {
        public int roundCount = 1;
        public int exercisesPlayed = 0;
        public int exerciseErrors = 0;
        public int errorfreerounds = 0;

        public List<string> callNumbersList;
        private Random random = new Random();

        public List<string> GenerateRandomCallNumbers(int count)
        {
            List<string> generatedCallNumbers = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string callNumber = $"{random.Next(1000):D03}.{random.Next(1000):D03} {RandomString(3)}";
                generatedCallNumbers.Add(callNumber);
            }
            return generatedCallNumbers;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Bubble Sort algorithm to check if a list is in ascending order
        public bool IsAscendingOrder(List<string> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (string.Compare(list[i - 1], list[i]) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckOrder(List<string> userOrder)
        {
            // Check if the user's order is in ascending order as strings
            bool isCorrectOrder = IsAscendingOrder(userOrder);

            if (isCorrectOrder)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
