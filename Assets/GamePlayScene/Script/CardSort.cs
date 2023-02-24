using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardSort
{
    public class CSM
    {
        public static void QuickSort(List<float> array, int p, int r)
        {
            if (p < r)
            {
                int q = Partition(array, p, r);
                QuickSort(array, p, q - 1);
                QuickSort(array, q + 1, r);
            }
        }

        public static int Partition(List<float> array, int p, int r)
        {
            int q = p;
            for (int j = p; j < r; j++)
            {
                if (array[j] <= array[r])
                {
                    Swap(array, q, j);
                    q++;
                }
            }
            Swap(array, q, r);
            return q;
        }

        public static void Swap(List<float> array, int beforeIndex, int foreIndex)
        {
            var tmp = array[beforeIndex];
            array[beforeIndex] = array[foreIndex];
            array[foreIndex] = tmp;
        }
    }
}