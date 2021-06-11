// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

namespace ML.MazeGame
{
  public class PathFinding : MonoBehaviour
  {
    [Header("Properties")]
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int2 startPosition;
    [SerializeField] int2 endPosition;
    [SerializeField] Sprite startNodeSprite;
    [SerializeField] Sprite endNodeSprite;
    [SerializeField] bool speedTest;

    [Header("Refs")]
    [Tooltip("transform which contain all cell node")]
    [SerializeField] Transform cellsParent;
    MazeCell[,] cells;

    //Private
    const int MOVE_STRAIGHT_COST = 10;
    const int MOVE_DIAGONAL_COST = 14;

    #region Mono
    private void Start()
    {
      //Test speed
      if (!speedTest) return;
      float temp = Time.realtimeSinceStartup;
      FindPath();

      Debug.Log("Path Finding Time : " + (Time.realtimeSinceStartup - temp).ToString("f4") + " ms");
    }
    #endregion

    #region Functions
    public void FindPath()
    {
      GetAllNode();

      NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

      for (int x = 0; x < gridSize.x; x++)
      {
        for (int y = 0; y < gridSize.y; y++)
        {
          // Create new node
          PathNode pathNode = new PathNode();
          pathNode.x = x;
          pathNode.y = y;
          pathNode.index = CalculateIndex(x, y, gridSize.x);

          pathNode.gCost = int.MaxValue;
          pathNode.hCost = CalculateHCost(new int2(x, y), endPosition);
          pathNode.CalculateFCost();

          // pathNode.isWalkable = true;

          //invalid at default
          pathNode.parrentIndex = -1;

          pathNodeArray[pathNode.index] = pathNode;
        }
      }

      //Initial Start Node
      PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
      startNode.gCost = 0;
      startNode.CalculateFCost();

      pathNodeArray[startNode.index] = startNode;

      //Open list and close list for searching
      NativeList<int> openList = new NativeList<int>(Allocator.Temp);
      NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

      openList.Add(startNode.index);

      int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);
      while (openList.Length > 0)
      {
        int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
        PathNode currentNode = pathNodeArray[currentNodeIndex];

        if (currentNodeIndex == endNodeIndex)
        {
          //Path found
          break;
        }

        //If path not found, remove current node from openlist array
        for (int i = 0; i < openList.Length; i++)
        {
          if (openList[i] == currentNodeIndex)
          {
            openList.RemoveAtSwapBack(i);
            break;
          }
        }

        closedList.Add(currentNodeIndex);

        List<MazePassage> neighbourCells = cells[currentNode.x, currentNode.y].AllPassage();
        //Neighbour offset
        for (int i = 0; i < neighbourCells.Count; i++)
        {
          MazeCell neighbourCell = neighbourCells[i].otherCell;
          int2 neighbourPosition = new int2(neighbourCell.position.x, neighbourCell.position.y);

          int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);

          if (closedList.Contains(neighbourNodeIndex))
          {
            //Node already searched
            continue;
          }

          PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
          //if(!neighbourNode.isWalkable) continue;

          int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

          int tentativeGCost = currentNode.gCost + CalculateHCost(currentNodePosition, neighbourPosition);
          if (tentativeGCost < neighbourNode.gCost)
          {
            neighbourNode.parrentIndex = currentNodeIndex;
            neighbourNode.gCost = tentativeGCost;
            neighbourNode.CalculateFCost();
            pathNodeArray[neighbourNodeIndex] = neighbourNode;

            if (!openList.Contains(neighbourNode.index))
            {
              openList.Add(neighbourNode.index);
            }
          }
        }
      }

      PathNode endNode = pathNodeArray[endNodeIndex];
      if (endNode.parrentIndex == -1)
      {
        //Path not found
      }
      else
      {
        //Path founded
        NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

        for (int i = 0; i < path.Length; i++)
        {
          int2 pathPos = path[i];

          cells[pathPos.x, pathPos.y].gameObject.GetComponent<SpriteRenderer>().enabled = true;

          if (i == 0)
          {
            cells[pathPos.x, pathPos.y].gameObject.GetComponent<SpriteRenderer>().sprite = endNodeSprite;
          }

          if (i == path.Length - 1)
          {
            cells[pathPos.x, pathPos.y].gameObject.GetComponent<SpriteRenderer>().sprite = startNodeSprite;
          }
        }

        path.Dispose();
      }

      //Dispose all temp array
      openList.Dispose();
      closedList.Dispose();
      pathNodeArray.Dispose();
    }

    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
    {
      if (endNode.parrentIndex == -1)
      {
        //path not found
        return new NativeList<int2>(Allocator.Temp);
      }
      else
      {
        //path found
        NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
        path.Add(new int2(endNode.x, endNode.y));

        PathNode currentNode = endNode;
        while (currentNode.parrentIndex != -1)
        {
          PathNode parrentNode = pathNodeArray[currentNode.parrentIndex];
          path.Add(new int2(parrentNode.x, parrentNode.y));
          currentNode = parrentNode;
        }
        return path;
      }
    }

    private int CalculateIndex(int x, int y, int width) { return x + y * width; }

    private int CalculateHCost(int2 startPos, int2 endPos)
    {
      int xDistance = math.abs(startPos.x - endPos.x);
      int yDistance = math.abs(startPos.y - endPos.y);

      int remaining = math.abs(xDistance - yDistance);

      return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
    {
      PathNode lowestCostPathNode = pathNodeArray[openList[0]];

      for (int i = 0; i < openList.Length; i++)
      {
        PathNode testPathNode = pathNodeArray[openList[i]];
        if (testPathNode.fCost < lowestCostPathNode.fCost)
        {
          lowestCostPathNode = testPathNode;
        }
      }
      return lowestCostPathNode.index;
    }

    public void GetAllNode()
    {
      MazeCell[] allCells = cellsParent.gameObject.GetComponentsInChildren<MazeCell>();
      cells = new MazeCell[gridSize.x, gridSize.y];

      for (int i = 0; i < allCells.Length; i++)
      {
        MazeCell cell = allCells[i];
        cells[cell.position.x, cell.position.y] = cell;
      }
    }
    #endregion
  }

  struct PathNode
  {
    public int x;
    public int y;
    public int index;

    public int gCost;
    public int hCost;
    public int fCost;

    //The node index which come to this node
    public int parrentIndex;

    // public bool isWalkable;

    public void CalculateFCost()
    {
      fCost = gCost + hCost;
    }
  }
}
