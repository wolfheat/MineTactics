using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class LevelCreator : MonoBehaviour
{

    [SerializeField] private int gameWidth = 10;
    [SerializeField] private int gameHeight = 6;

    [SerializeField] private GameBox mineBoxPrefab;
    [SerializeField] private GameBox unclearedBoxPrefab;
    [SerializeField] private GameObject boxHolder;


    // Should there be one actual board and one visual board? yes? Only mines are necessarey to describe actual board

    int[,] mines;

    
    void Start()
    {
        Debug.Log("Create Level");
        mines = new int[gameWidth, gameHeight];

        CreateLevel();
        DrawLevel();

    }

    private void DrawLevel()
    {
        for (int i = 0; i < gameWidth; i++)
        {
            for (int j = 0; j < gameHeight; j++)
            {
                // Make mine
                Instantiate(mines[i, j] == 1 ? mineBoxPrefab:unclearedBoxPrefab, boxHolder.transform.position+ new Vector3(i, j, 0), Quaternion.identity, boxHolder.transform);
            }
        }
    }

    private void CreateLevel()
    {
        for (int i = 0; i < gameWidth; i++)
        {
            for (int j = 0; j < gameHeight; j++)
            {
                mines[i, j] = Random.Range(0,2);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
