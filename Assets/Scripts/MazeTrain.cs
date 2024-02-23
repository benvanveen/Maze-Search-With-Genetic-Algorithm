using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MazeTrain : MonoBehaviour
{


    public Tilemap tilemap;
    public TileBase tileToPlace;

    public Tilemap checkpointTilemap;
    public TileBase checkpointTile;

    public Tilemap startTilemap;
    public TileBase startTile;

    public Tilemap endTilemap;
    public TileBase endTile;
        // Create a new instance of the Random class
    

    public bool GenerationComplete;

    private bool mazeComplete;

    private int leftCount;
    private int rightCount;

    private int currentLength;
    public float wallFraction;
    public float checkpointFraction;

    public int n = 4; // replace with your desired length
    public int m = 3; // replace with your desired height

    private int rows;
    private int cols;

    private int x;
    private int y;

    public int[,] matrix;
    private int[,] mazeMatrix;

    public Slider MazeHeight;
    public Slider MazeLength;
    public Slider CheckPointFrequency;

    void Start()
    {
        n = Mathf.RoundToInt(MazeHeight.value);
        m = Mathf.RoundToInt(MazeLength.value);
        
        checkpointFraction = CheckPointFrequency.value;

        GenerationComplete = false;
        matrix = GenerateRandomMatrix(n, m);

        mazeMatrix = CreateZeroMatrix(2*n-1, 2*m-1);

        // Print the generated matrix to the Unity console

        System.Random random = new System.Random();
        PrintMatrix(matrix);

        rows = matrix.GetLength(0);
        cols = matrix.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                mazeMatrix[2*i,2*j] = 0;

                x=i;

                y=j;

                if(x==rows-1){
                    x=rows-2;
                }
                if(y==cols-1){
                    y=cols-2;
                }

                if(matrix[x,y]==matrix[x+1,y]){
                    mazeMatrix[2*x+1,2*y] = 0;
                } else{
                    mazeMatrix[2*x+1,2*y] = 1;
                }

                if(matrix[x,y]==matrix[x,y+1]){
                    mazeMatrix[2*x,2*y+1] = 0;
                } else{
                    mazeMatrix[2*x,2*y+1] = 1;
                }
                
                mazeMatrix[2*x+1,2*y+1] = 1;

            }
        }

        GenerateTilemap(mazeMatrix);
        
        PrintMatrix(mazeMatrix);

        GenerationComplete = true;

    }

    int[,] GenerateRandomMatrix(int n, int m)
    {
        int[,] matrix = new int[n, m];
        System.Random random = new System.Random();

        // Fill the matrix with random integers 1, 2, and 3
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                matrix[i, j] = random.Next(1, 4); // Generates random integers from 1 to 3
            }
        }

        return matrix;
    }

    void PrintMatrix(int[,] matrix)
    {
        int n = matrix.GetLength(0);
        int m = matrix.GetLength(1);

        for (int i = 0; i < n; i++)
        {
            string row = "";
            for (int j = 0; j < m; j++)
            {
                row += matrix[i, j] + " ";
            }
        }
    }

    void GenerateTilemap(int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                if (matrix[i, j] == 0 || i == 0 || j == 0)
                {
                    // Place the tile at the corresponding position in the Tilemap
                    tilemap.SetTile(new Vector3Int(j, -i, 0), tileToPlace);
                } else {
                    float randomFloat  = Random.Range(0f, 1f);

                    if(randomFloat<checkpointFraction){

                        // Place the tile at the corresponding position in the Tilemap
                        checkpointTilemap.SetTile(new Vector3Int(j, -i, 0), checkpointTile);
                    }
                }

                if(i==1 && j==(2*m)-3){
                    endTilemap.SetTile(new Vector3Int(j, -i, 0), startTile);
                }
                if(j==1 && i==2*n-3){
                    startTilemap.SetTile(new Vector3Int(j, -i, 0), endTile);
                }
                if((j==2 && i==2*n-3) || (j==1 && i==2*n-4) || (j==2 && i==2*n-4)){
                    tilemap.SetTile(new Vector3Int(j, -i, 0), null);
                }
                if((i==2 && j==(2*m)-3) || (i==1 && j==(2*m)-4) || (i==2 && j==(2*m)-4)){
                    tilemap.SetTile(new Vector3Int(j, -i, 0), null);
                }
            }
        }
    }

    public static int[,] CreateZeroMatrix(int n, int m)
    {
        int[,] matrix = new int[n, m];

        // Initialize all elements to zero
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                matrix[i, j] = 0;
            }
        }

        return matrix;
    }

    void FixedUpdate()
    {
    }
}