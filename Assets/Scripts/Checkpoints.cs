using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


public class Checkpoints : MonoBehaviour
{
    public Tilemap CheckPointTilemap;

    private List<Vector3> CheckPointPositions = new List<Vector3>();
    private List<Vector3> CheckPointPositionsLocked = new List<Vector3>();

    public Tilemap EndGoalTilemap;
    public Vector3 EndGoalPosition;
    
    public Tilemap StartTilemap;
    public Vector3 StartPosition;

    private Vector3 TempVector;
    
    public MazeTrain MazeGenScript;

    public bool checkPointsChecked;

    public List<Vector3> GetCheckPoints(){
        return CheckPointPositions;
    }

    void Update()
    {
        CheckPointPositions = CheckPointPositionsLocked;
        
        //Debug.Log(EndGoalPosition);

        if(!checkPointsChecked && MazeGenScript.GenerationComplete){
            
        // Iterate through all tiles in the Tilemap
        BoundsInt bounds = CheckPointTilemap.cellBounds;
        BoundsInt bounds2 = EndGoalTilemap.cellBounds;
        BoundsInt bounds3 = StartTilemap.cellBounds;
        
    

        foreach (var position in bounds.allPositionsWithin)
        {
            Vector3Int cellPosition = new Vector3Int(position.x, position.y, position.z);

            // Get the world position of the center of the current tile
            Vector3 worldPosition = CheckPointTilemap.GetCellCenterWorld(cellPosition);
            
            CheckPositionForTile(cellPosition);
        }

        checkPointsChecked = true;

        foreach (var position in bounds2.allPositionsWithin)
        {
            Vector3Int cellPosition = new Vector3Int(position.x, position.y, position.z);

            // Get the world position of the center of the current tile
            Vector3 worldPosition = EndGoalTilemap.GetCellCenterWorld(cellPosition);

            bool hasTile = EndGoalTilemap.HasTile(position);
            if (hasTile)
                {
                //Debug.Log($"Tile exists at position {position}");
                        
                TempVector = new Vector3(position.x, position.y, position.z);
                        
                // Add 0.5 to the x and y components
                TempVector.x += 0.5f;
                TempVector.y += 0.5f;
                

                EndGoalPosition = TempVector;
                //Debug.Log($"Tile at {cellPosition} has world position {worldPosition}");

            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        foreach (var position in bounds3.allPositionsWithin)
        {
            Vector3Int cellPosition = new Vector3Int(position.x, position.y, position.z);

            // Get the world position of the center of the current tile
            Vector3 worldPosition = StartTilemap.GetCellCenterWorld(cellPosition);

            bool hasTile = StartTilemap.HasTile(position);
            if (hasTile)
                {
                //Debug.Log($"Tile exists at position {position}");
                        
                TempVector = new Vector3(position.x, position.y, position.z);
                        
                // Add 0.5 to the x and y components
                TempVector.x += 0.5f;
                TempVector.y += 0.5f;
                

                StartPosition = TempVector;
                //Debug.Log($"Tile at {cellPosition} has world position {worldPosition}");

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }

    void CheckPositionForTile(Vector3Int position)
    {
        // Check if the Tilemap has a tile at the specified position
        bool hasTile = CheckPointTilemap.HasTile(position);

        if (hasTile)
        {
            //Debug.Log($"Tile exists at position {position}");

            TempVector = new Vector3(position.x, position.y, position.z);
            
            // Add 0.5 to the x and y components
            TempVector.x += 0.5f;
            TempVector.y += 0.5f;

            CheckPointPositions.Add(TempVector);
            CheckPointPositionsLocked.Add(TempVector);

        }
    }

    
}