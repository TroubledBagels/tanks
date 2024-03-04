using System;
using System.Collections;
using System.Collections.Generic;

namespace SortAlgorithm
{
    class ListSort
    {
        public static Tuple<List<int>, List<int>> SortBtoS(List<int> Unsorted, List<int> UnsortedOrder) //Defines function
        {
            bool isSorted = false; //Creates bool to check if sorted
            while (!isSorted) //While not sorted
            {
                bool SomethingChanged = false; //Sets if something has changed to false
                for (int i = 0; i < Unsorted.Count; i++) //For every item in the list
                {
                    if (i + 1 > Unsorted.Count - 1) //If the next term doesn't exist
                    {
                        //Do nothing   
                    }
                    else if (Unsorted[i] > Unsorted[i + 1]) //If then next term is bigger than the current term
                    {
                        int temp = Unsorted[i + 1]; //Sets the next term to a temporary variable
                        Unsorted[i + 1] = Unsorted[i]; //Sets the next term to the current term
                        Unsorted[i] = temp; //Sets the "current" term to the temporary variable
                        int tempOrder = UnsortedOrder[i + 1]; //Does the same for the order
                        UnsortedOrder[i + 1] = UnsortedOrder[i];
                        UnsortedOrder[i] = tempOrder;
                        SomethingChanged = true; //Confirms something changed
                    }
                }
                if (!SomethingChanged) //If nothing changed
                {
                    isSorted = true; //End the loop
                }
            }
            foreach (int i in UnsortedOrder)
            {
                Console.WriteLine(i);
            }
            UnsortedOrder.Reverse();
            Unsorted.Reverse();
            return Tuple.Create(Unsorted, UnsortedOrder); //Return the sorted list
        }
        public static Tuple<List<int>, List<int>> SortStoB(List<int> Unsorted, List<int> UnsortedOrder) //Defines function
        {
            bool isSorted = false; //Creates bool to check if sorted
            while (!isSorted) //While not sorted
            {
                bool SomethingChanged = false; //Sets if something has changed to false
                for (int i = 0; i < Unsorted.Count; i++) //For every item in the list
                {
                    if (i + 1 > Unsorted.Count - 1) //If the next term doesn't exist
                    {
                        //Do nothing   
                    }
                    else if (Unsorted[i] > Unsorted[i + 1]) //If then next term is bigger than the current term
                    {
                        int temp = Unsorted[i + 1]; //Sets the next term to a temporary variable
                        Unsorted[i + 1] = Unsorted[i]; //Sets the next term to the current term
                        Unsorted[i] = temp; //Sets the "current" term to the temporary variable
                        int tempOrder = UnsortedOrder[i + 1]; //Does the same for the order
                        UnsortedOrder[i + 1] = UnsortedOrder[i];
                        UnsortedOrder[i] = tempOrder;
                        SomethingChanged = true; //Confirms something changed
                    }
                }
                if (!SomethingChanged) //If nothing changed
                {
                    isSorted = true; //End the loop
                }
            }
            foreach (int i in UnsortedOrder)
            {
                Console.WriteLine(i);
            }
            return Tuple.Create(Unsorted, UnsortedOrder); //Return the sorted list
        }
        /*static void Main() 
        {
            Random rdm = new Random();

            List<int> Scores = new List<int>();
            List<int> PlayerOrder = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                Scores.Add(rdm.Next());
                PlayerOrder.Add(i + 1);
                Console.WriteLine(Scores[i]);
                Console.WriteLine(PlayerOrder[i]);
            }

            Tuple<List<int>, List<int>> Out = Sort(Scores, PlayerOrder);

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(Out.Item1[i]);
                Console.WriteLine(Out.Item2[i]);
            }
        }*/
    }
}
