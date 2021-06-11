// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MazeGame
{
  public class MazeGenerator : MonoBehaviour
  {
    [Header("Properties")]
    [Tooltip("Delay between each generate step. Use for visualize purpose")]
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] Vector2Int size;

    [Header("Prefabs")]
    [SerializeField] MazeCell cellPrefab;
    [SerializeField] MazePassage passagePrefab;
    [SerializeField] MazeWall wallPrefab;

    MazeCell[,] cells;

    public MazeCell[,] Cells{get{return cells;}}

    public void Generate()
    {
      //Clear child before generate
      ClearAllCells();

      cells = new MazeCell[size.x, size.y];
      List<MazeCell> activeCells = new List<MazeCell>();
      DoFirstGenerationStep(activeCells);

      while (activeCells.Count > 0)
      {
        DoNextGenerationStep(activeCells);
      }

      
    }

    private MazeCell CreateCell(Vector2Int pos)
    {
      MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
      cells[pos.x, pos.y] = newCell;
      newCell.position = pos;
      newCell.name = "Maze Cell " + pos.x + ", " + pos.y;
      newCell.transform.parent = transform;
      newCell.transform.localPosition = new Vector3(pos.x - size.x * offsetX + offsetX, pos.y - size.y * offsetY + offsetY, 0f);

      return newCell;
    }

    public Vector2Int RandomCoordinates
    {
      get
      {
        return new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y));
      }
    }

    public bool ContainsCoordinates(Vector2Int coordinate)
    {
      return coordinate.x >= 0 && coordinate.x < size.x && coordinate.y >= 0 && coordinate.y < size.y;
    }

    public MazeCell GetCell(Vector2Int coordinates)
    {
      return cells[coordinates.x, coordinates.y];
    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
      activeCells.Add(CreateCell(RandomCoordinates));
    }

    private void DoNextGenerationStep(List<MazeCell> activeCells)
    {
      int currentIndex = activeCells.Count - 1;
      MazeCell currentCell = activeCells[currentIndex];

      if (currentCell.IsFullyInitialized) {
        activeCells.RemoveAt(currentIndex);
        return;
      }

      MazeDirection direction = currentCell.RandomUninitializedDirection;
      Vector2Int coordinates = currentCell.position + MazeDirections.MazeDirectionToVector2Int(direction);

      if (ContainsCoordinates(coordinates))
      {
        MazeCell neighbor = GetCell(coordinates);
        if (neighbor == null)
        {
          neighbor = CreateCell(coordinates);
          CreatePassage(currentCell, neighbor, direction);
          activeCells.Add(neighbor);
        }
        else
        {
          CreateWall(currentCell, neighbor, direction);
        }
      }
      else
      {
        CreateWall(currentCell, null, direction);
      }
    }

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
      MazePassage passage = Instantiate(passagePrefab) as MazePassage;
      passage.Initialize(cell, otherCell, direction);
      passage = Instantiate(passagePrefab) as MazePassage;
      passage.Initialize(otherCell, cell, MazeDirections.GetOpposite(direction));
    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
      MazeWall wall = Instantiate(wallPrefab) as MazeWall;
      wall.Initialize(cell, otherCell, direction);
      if (otherCell != null)
      {
        wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(otherCell, cell, MazeDirections.GetOpposite(direction));
      }
    }

    private void ClearAllCells()
    {
      while (transform.childCount > 0)
      {
        DestroyImmediate(transform.GetChild(0).gameObject);
      }
    }
  }
}
