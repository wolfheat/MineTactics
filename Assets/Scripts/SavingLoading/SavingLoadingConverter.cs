using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class SavingLoadingConverter : MonoBehaviour
{

    // Split into 8 pieces? And make into char
    public static char[,] LevelToChar2DArray(int[,] mines, GameBox[,] boxes)
    {
        char[,] ans = new char[mines.GetLength(0),mines.GetLength(1)];

        for (int i = 0; i < mines.GetLength(0); i++)
        {
            for (int j = 0; j < mines.GetLength(1); j++)
            {
                if((mines[i, j] == -1))
                {
                    // Has mine
                    ans[i,j] = boxes[i,j].Marked ? '2':'1';
                }
                else
                {
                    // Has mine
                    ans[i, j] = boxes[i, j].UnSolved() ? '0' : '3';
                }
            }
        }
        return ans;
    }
    // Split into 8 pieces? And make into char
    public static string ComressToString(char[,] charArray)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < charArray.GetLength(1); i++)
        {
            for (int j = 0; j < charArray.GetLength(0); j++)
            {
                sb.Append(charArray[i, j]);
            }
        }

        // Read string array from start to end
        string unCompressed = sb.ToString();

        Debug.Log("Uncompressed: "+unCompressed);

        // Compress
        string compressed = Compress(unCompressed);
        Debug.Log("Compressed:   " + compressed);

        string deCompressed = UnComressString(compressed);
        Debug.Log("De-Compressed:" + deCompressed);


        return compressed;
    }

    public static string UnComressString(string compressed)
    {
        StringBuilder sb = new StringBuilder();

        int amt = 0;
        // read the uncompressed and store the compressed version
        for (int i = 0; i < compressed.Length; i++)
        {
            if (Char.IsLetter(compressed[i]))
            {
                // This Is a multiple value
                amt = CharToAmount(compressed[i]);
                continue;
            }
            else
            {
                // THis is the in type
                AddToStringBuilder(compressed[i]);
                amt = 1;
            }
        }
        // Addds the last item
        // AddToStringBuilder(amt); // Not needed for decompress right?
        return sb.ToString();

        // Local Method for adding
        void AddToStringBuilder(char c)
        {
            sb.Append(c,amt);
        }
    }
    private static string Compress(string full)
    {
        StringBuilder sb = new StringBuilder();

        char last = ' ';
        int amt = 0;
        // read the uncompressed and store the compressed version
        for (int i = 0; i < full.Length; i++)
        {
            if (full[i] == last)
            {
                // Same as last char keep counting
                amt++;
                continue;
            }
            else
            {
                // New character store and start new count
                AddToStringBuilder(amt);
                last = full[i];
                amt = 1;
            }
        }
        // Addds the last item
        AddToStringBuilder(amt);
        return sb.ToString();

        // Local Method for adding
        void AddToStringBuilder(int amt)
        {
            if (last != ' ') // Skip if first value
            {
                if (amt > 1) { 
                    char amtChar = AmountToChar(amt);
                    sb.Append(amtChar);
                }
                sb.Append(last);
            }
        }

    }

    private static int CharToAmount(char c)
    {

        // use large letters
        if (Char.IsUpper(c))
        {
            return c - 'A' + 26;
        }
        return (c - 'a' + 1);
    }
    
    private static char AmountToChar(int amt)
    {
        // use large letters
        if(amt > 26)
            return (char)('A'+(amt-26-1));
        return (char)('a' + amt-1);
    }


    // Use Hex coordinates to note opened clicks, also marked mines?
    // 9a3b4ac3a4b   - (means 9 + more)  so -4 == 13
    // better version use abcd to note type  1 = mine, 2 = flagged, 0 = uncleared, 3 = cleared, , e = goalToWin?
    // amount a-z then A-Z ?? 1-26 then 27-53 ? "da" means 4 uncleared mines

    // Have check for wrongly marked mines when saving?



}
