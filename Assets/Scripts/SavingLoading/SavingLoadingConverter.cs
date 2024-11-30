using System;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class SavingLoadingConverter : MonoBehaviour
{

    // Split into 8 pieces? And make into char
    public static (int[,],string) LevelTo2DArray(int[,] mines, GameBox[,] boxes)
    {
        int[,] ans = new int[mines.GetLength(0),mines.GetLength(1)];
        int width = mines.GetLength(0);
        string pre = (width <= 9 ? "0" : "") + width;
        for (int i = 0; i < mines.GetLength(0); i++)
        {
            for (int j = 0; j < mines.GetLength(1); j++)
            {
                if((mines[i, j] == -1))
                {
                    // Has mine
                    ans[i,j] = boxes[i,j].Marked ? 2:1;
                }
                else
                {
                    // Has mine
                    ans[i, j] = boxes[i, j].UnSolved() ? 0 : 3;
                }
            }
        }
        return (ans, pre);
    }
    // Split into 8 pieces? And make into char
    public static string ComressToString(int[,] charArray,string pre)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(pre);
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

        // Insert test compressed here for testing purpose
        //unCompressed = "080000000000000000000000000000000000000000000000000000033122221222";
        //unCompressed = "  -----*****-----*****-----*****-----***40-----***50-----***60-----";


        // Compress
        string compressed = Compress(unCompressed);
        Debug.Log("Compressed:   " + compressed);

        /*
        string deCompressed = UnComressString(compressed);
        Debug.Log("De-Compressed:" + deCompressed);
        */

        return compressed;
    }

    public static string UnComressString(string compressed)
    {
        StringBuilder sb = new StringBuilder();
        Debug.Log("Uncompressing starting with "+compressed);
        int amt = 1;
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
            //Debug.Log("Adding "+c);
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
            if (full[i] == last && amt < 52)
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
            return c - 'A' + 27;
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

    internal static (int[,], int[,], int, int,int) StringLevelToGameArray(string deCompressed, bool editorcreateMode = false)
    {
        Debug.Log("Making game array from decompressed string: "+deCompressed);
        int totalSize = deCompressed.Length - 2;
        string gameDeCompressed = deCompressed.Substring(2, totalSize);
        //Debug.Log("Decompressed: "+deCompressed);
        int width = Int32.Parse(deCompressed.Substring(0, 2));
        int height = totalSize / width;
        int totMines = 0;

        Debug.Log("Width = "+width+" Height ="+height);

        if (totalSize % width != 0)
            Debug.Log("Loaded string is not correct length");

        int[,] newMines = new int[width, height];
        int[,] gameArray = new int[width, height];

        GameBox[,] upper = new GameBox[width, height];
        GameBox[,] lower = new GameBox[width, height];


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int value = gameDeCompressed[width * i + j] - '0';
                //Debug.Log("String to Game array value = "+value);
                // read the string
                if (value == 2 || value == 1)
                {
                    newMines[i, j] = -1;

                    totMines++;
                }
                gameArray[i,j] = value;
            }
        }
        
        // Exit without rotating or transposing if in editor mode
        if(editorcreateMode)
            return (newMines, gameArray, width, height, totMines);

        // Rotate? 180
        int rotate = Random.Range(0,4);
        if (rotate>0)
        {
            Debug.Log(" ROTATE "+(90*rotate)+"°! ");
            for (int i = 0; i < rotate; i++)
            {
                newMines = Rotate(newMines);
                gameArray = Rotate(gameArray);
            }
        }



        // Transpose
        bool transpose = Random.Range(0,2)==1?true:false;
        if (transpose)
        {
            Debug.Log(" TRANSPOSED! ");
            return (Transpose(newMines), Transpose(gameArray), height, width, totMines);
        }

        // Send Basic Level
        return (newMines, gameArray, width, height, totMines);
    }

    private static T[,] Rotate<T>(T[,] array)
    {
        int origWidth = array.GetLength(0);
        int origHeight = array.GetLength(1);

        T[,] ans = new T[origHeight, origWidth];
        for (int i = 0; i < origHeight; i++)
        {
            for (int j = 0; j < origWidth; j++)
            {
                ans[origWidth-j-1, i] = array[i, j];
            }
        }
        return ans;
    }

    private static T[,] Transpose<T>(T[,] array)
    {
        int origWidth = array.GetLength(0);
        int origHeight = array.GetLength(1);

        T[,] ans = new T[origHeight,origWidth];
        for (int i = 0; i < origHeight; i++)
        {
            for (int j = 0; j < origWidth; j++)
            {
                ans[j, i] = array[i,j];
            }
        }
        return ans;
    }


    // Use Hex coordinates to note opened clicks, also marked mines?
    // 9a3b4ac3a4b   - (means 9 + more)  so -4 == 13
    // better version use abcd to note type  1 = mine, 2 = flagged, 0 = uncleared, 3 = cleared, , e = goalToWin?
    // amount a-z then A-Z ?? 1-26 then 27-53 ? "da" means 4 uncleared mines

    // Have check for wrongly marked mines when saving?



}
