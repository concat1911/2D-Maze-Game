// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ML.MazeGame
{
  public enum MazeDirection
  {
    North,
    East,
    South,
    West
  }

  public class MazeGenerator : MonoBehaviour
  {
    [Header("Properties")]
    [Tooltip("Delay between each generate step. Use for visualize purpose")]
    [SerializeField] float offsetX;
    [SerializeField] float offsetY;
    [SerializeField] Vector2Int size;
    [SerializeField] MazeCell cellPrefab;

    private MazeCell[,] cells;

    public void Generate()
    {
      //Clear child before generate
      ClearAllCells();

      cells = new MazeCell[size.x, size.y];
      List<MazeCell> activeCells = new List<MazeCell>();
      DoFirstGenerationStep(activeCells);

      while (activeCells.Count > 0) {
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

    private Vector2Int RandomMazeDirection
    {
      get
      {
        MazeDirection randomDir = (MazeDirection)Random.Range(0, 4);
        switch (randomDir)
        {
          case MazeDirection.North:
            return Vector2Int.up;
          case MazeDirection.South:
            return Vector2Int.down;
          case MazeDirection.East:
            return Vector2Int.right;
          case MazeDirection.West:
            return Vector2Int.left;
          default:
            return Vector2Int.one;
        }
      }
    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
      activeCells.Add(CreateCell(RandomCoordinates));
    }

    private void DoNextGenerationStep(List<MazeCell> activeCells)
    {
      int currentIndex = activeCells.Count - 1;
      MazeCell currentCell = activeCells[currentIndex];
      Vector2Int coordinates = currentCell.position + RandomMazeDirection;

      if (ContainsCoordinates(coordinates) && GetCell(coordinates) == null)
      {
        activeCells.Add(CreateCell(coordinates));
      }
      else
      {
        activeCells.RemoveAt(currentIndex);
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
