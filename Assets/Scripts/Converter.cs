using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WolfheatProductions
{
    public class Converter
    {
        // Takes an int array and scrambles it using FisherYates
        public static int[] FisherYatesScramble(int[] allPos)
        {
            int n = allPos.Length;
            for (int i = 0; i < n; i++)
            {
                int temp = allPos[i];
                // Rendom pos
                int random = Random.Range(0, n);
                allPos[i] = allPos[random];
                allPos[random] = temp;

            }
            return allPos;
        }

    }
}
