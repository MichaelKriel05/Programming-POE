using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PROG_PART_1_LibraryApp
{
    public class IdentifyArea
    {
        public int roundCount = 0;
        public int correctCount = 0;
        private static Random random = new Random();


        //Method to shuffle the lists
        public List<T> ShuffleList<T>(List<T> inputList)
        {
            List<T> randomList = new List<T>();
            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count);
                randomList.Add(inputList[randomIndex]);
                inputList.RemoveAt(randomIndex);
            }
            return randomList;
        }

        //Getting random orders from list
        public List<T> GetRandomItems<T>(List<T> items, int count)
        {
            Random random = new Random();
            return items.OrderBy(x => random.Next()).Take(count).ToList();
        }

    }
}
